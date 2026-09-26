// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Tweens.Generators;

namespace testbed.Tests.Core;

public class StructuredGeneratorTests
{
    private static readonly CSharpParseOptions ParseOptions = new(LanguageVersion.CSharp12);
    private static readonly MetadataReference[] References = ((string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")!)
        .Split(Path.PathSeparator)
        .Where(path => Path.GetDirectoryName(path) == Path.GetDirectoryName(typeof(object).Assembly.Location))
        .Select(path => MetadataReference.CreateFromFile(path))
        .ToArray();

    // Compile an independent library to exercise semantic discovery and generated
    // C# without relying on Godot or existing generated definitions.
    private const string Contracts = """
        #nullable enable
        using System;
        namespace tweens.gd
        {
            public readonly record struct TweenOptions
            {
                public double Duration { get; init; }
                public Func<float, float>? EaseFunction { get; init; }
                internal void CopyTo(TweenOptionsBuilder target) { }
            }
            public class TweenOptionsBuilder
            {
                public double Duration { get; set; }
                public Func<float, float>? EaseFunction { get; set; }
                public static int Ignored { get; set; }
                public int ReadOnly => 0;
            }
            public interface ITweenDefinition<TTarget, TValue> where TTarget : class where TValue : struct
            {
                internal TweenDefinition<TTarget, TValue> CreatePlayback();
            }
            public class TweenInstance<TTarget, TValue> where TTarget : class where TValue : struct { }
            public abstract class TweenDefinition<TTarget, TValue> : TweenOptionsBuilder
                where TTarget : class where TValue : struct
            {
                public TValue? From { get; set; }
                public TValue? To { get; set; }
                public Action<TweenInstance<TTarget, TValue>>? OnAdd { get; set; }
                public Action<TweenInstance<TTarget, TValue>>? OnStart { get; set; }
                public Action<TweenInstance<TTarget, TValue>, TValue>? OnUpdate { get; set; }
                public Action<TweenInstance<TTarget, TValue>>? OnEnd { get; set; }
                public Action<TweenInstance<TTarget, TValue>>? OnCancel { get; set; }
                public Action<TweenInstance<TTarget, TValue>>? OnFinally { get; set; }
            }
            public class PropertyTween<TTarget, TValue> : TweenDefinition<TTarget, TValue>
                where TTarget : class where TValue : struct
            {
                public PropertyTween(Func<TTarget, TValue> getter, Action<TTarget, TValue> setter,
                    Func<TValue, TValue, float, TValue> interpolate) { }
            }
        }
        namespace Targets { public class Widget { } }
        """;

    private const string Adapter = """
        using Base = tweens.gd.PropertyTween<Targets.Widget, float>;
        namespace tweens.gd
        {
            public sealed partial class OpacityTween : Base
            {
                public OpacityTween() : base(_ => 0, (_, _) => { }, (a, b, t) => a) { }
            }
            public sealed partial class OpacityTween { }
        }
        namespace Other
        {
            public sealed class IgnoredTween() : Base(_ => 0, (_, _) => { }, (a, b, t) => a);
        }
        """;

    [Fact]
    public void DiscoversAliasedBaseTypesAndPartialAdaptersAndEmitsCompilableDefinitions()
    {
        var (_, output, result) = Run(CreateCompilation(Contracts, Adapter));
        Assert.Equal(new[] { "Opacity.g.cs", "Property.g.cs" },
            result.GeneratedSources.Select(source => source.HintName).Order().ToArray());
        var definition = output.GetTypeByMetadataName("Tweens.Opacity")!;
        Assert.True(definition.IsReadOnly);
        var contract = Assert.Single(definition.Interfaces, type => type.Name == "ITweenDefinition");
        Assert.Equal("Targets.Widget", contract.TypeArguments[0].ToDisplayString());
        Assert.Equal(SpecialType.System_Single, contract.TypeArguments[1].SpecialType);
        Assert.True(Assert.IsAssignableFrom<IPropertySymbol>(Assert.Single(definition.GetMembers("Duration"))).SetMethod!.IsInitOnly);
        Assert.Empty(definition.GetMembers("Ignored"));
        Assert.Empty(definition.GetMembers("ReadOnly"));
        var ease = Assert.IsAssignableFrom<IPropertySymbol>(Assert.Single(definition.GetMembers("EaseFunction")));
        Assert.Equal(NullableAnnotation.Annotated, ease.NullableAnnotation);
    }

    [Fact]
    public void EmitsShaderAndCustomPropertyConstructorsWithGenericConstraints()
    {
        const string shaders = """
            namespace tweens.gd
            {
                public sealed class ShaderParameterTween<TValue>(string parameter)
                    : TweenDefinition<Targets.Widget, TValue> where TValue : struct;
                public abstract class InstanceShaderParameterTween<TNode, TValue>(string parameter)
                    : TweenDefinition<TNode, TValue> where TNode : class where TValue : struct;
                public sealed class CanvasItemInstanceShaderParameterTween<TValue>(string parameter)
                    : InstanceShaderParameterTween<Targets.Widget, TValue>(parameter) where TValue : struct;
                public sealed class GeometryInstanceShaderParameterTween<TValue>(string parameter)
                    : InstanceShaderParameterTween<Targets.Widget, TValue>(parameter) where TValue : struct;
            }
            """;
        var (_, output, result) = Run(CreateCompilation(Contracts, shaders));
        Assert.Equal(4, result.GeneratedSources.Length);
        foreach (var name in new[] { "ShaderParameter", "CanvasItemInstanceShaderParameter", "GeometryInstanceShaderParameter" })
        {
            var type = output.GetTypeByMetadataName("Tweens." + name + "`1")!;
            Assert.True(Assert.Single(type.TypeParameters).HasValueTypeConstraint);
            Assert.Contains(type.InstanceConstructors, constructor => constructor.Parameters.Length == 1
                && constructor.Parameters[0].Type.SpecialType == SpecialType.System_String);
        }
        var property = output.GetTypeByMetadataName("Tweens.Property`2")!;
        Assert.True(property.TypeParameters[0].HasReferenceTypeConstraint);
        Assert.True(property.TypeParameters[1].HasValueTypeConstraint);
        Assert.Contains(property.InstanceConstructors, constructor => constructor.Parameters.Length == 3);
    }

    [Fact]
    public void OptionChangesAndAdapterRemovalUpdateGeneratedSources()
    {
        var compilation = CreateCompilation(Contracts, Adapter);
        var (driver, _, _) = Run(compilation);
        var changedContracts = Contracts.Replace("public double Duration", "public double Delay");
        compilation = compilation.ReplaceSyntaxTree(compilation.SyntaxTrees.First(), Parse(changedContracts));
        var changed = Run(compilation, driver);
        var definition = changed.Output.GetTypeByMetadataName("Tweens.Opacity")!;
        Assert.Single(definition.GetMembers("Delay"));
        Assert.Empty(definition.GetMembers("Duration"));

        compilation = compilation.RemoveSyntaxTrees(compilation.SyntaxTrees.Last());
        var (_, removed, result) = Run(compilation, changed.Driver);
        Assert.Equal("Property.g.cs", Assert.Single(result.GeneratedSources).HintName);
        Assert.Null(removed.GetTypeByMetadataName("Tweens.Opacity"));
    }

    [Fact]
    public void UnrelatedEditsKeepDefinitionModelsCached()
    {
        var unrelated = Parse("public class Unrelated { }");
        var compilation = CreateCompilation(Contracts, Adapter).AddSyntaxTrees(unrelated);
        var (driver, _, _) = Run(compilation);
        compilation = compilation.ReplaceSyntaxTree(unrelated, Parse("public class Unrelated { public int Value; }"));
        var (_, _, result) = Run(compilation, driver);
        Assert.All(result.TrackedSteps["Definitions"].SelectMany(step => step.Outputs),
            output => Assert.True(output.Reason is IncrementalStepRunReason.Cached or IncrementalStepRunReason.Unchanged));
    }

    private static SyntaxTree Parse(string source) => CSharpSyntaxTree.ParseText(source, ParseOptions);

    private static CSharpCompilation CreateCompilation(params string[] sources) => CSharpCompilation.Create(
        "GeneratorTests", sources.Select(Parse), References,
        new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

    private static (GeneratorDriver Driver, Compilation Output, GeneratorRunResult Result) Run(
        CSharpCompilation compilation, GeneratorDriver? driver = null)
    {
        driver ??= CSharpGeneratorDriver.Create(
            generators: [new StructuredDefinitionGenerator().AsSourceGenerator()],
            parseOptions: ParseOptions,
            driverOptions: new GeneratorDriverOptions(IncrementalGeneratorOutputKind.None, trackIncrementalGeneratorSteps: true));
        driver = driver.RunGeneratorsAndUpdateCompilation(compilation, out var output, out var diagnostics);
        Assert.Empty(diagnostics);
        Assert.Empty(output.GetDiagnostics().Where(diagnostic => diagnostic.Severity == DiagnosticSeverity.Error));
        return (driver, output, Assert.Single(driver.GetRunResult().Results));
    }
}

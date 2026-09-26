// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using System;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Tweens.Generators;

[Generator(LanguageNames.CSharp)]
public sealed class StructuredDefinitionGenerator : IIncrementalGenerator
{
    private static readonly SymbolDisplayFormat TypeFormat = SymbolDisplayFormat.FullyQualifiedFormat
        .WithMiscellaneousOptions(SymbolDisplayFormat.FullyQualifiedFormat.MiscellaneousOptions
            | SymbolDisplayMiscellaneousOptions.IncludeNullableReferenceTypeModifier);

    private static readonly string[] Callbacks = { "OnAdd", "OnStart", "OnUpdate", "OnEnd", "OnCancel", "OnFinally" };

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var options = context.SyntaxProvider.CreateSyntaxProvider(
                static (node, _) => node is ClassDeclarationSyntax { Identifier.ValueText: "TweenOptionsBuilder" },
                static (syntax, token) => ReadOptions(syntax, token))
            .Where(static source => source is not null)
            .Collect()
            .Select(static (sources, _) => sources.SingleOrDefault());

        // Keep symbols inside the semantic transform. Value tuples of strings give
        // downstream steps value equality, so unrelated edits don't re-emit sources.
        var definitions = context.SyntaxProvider.CreateSyntaxProvider(
                static (node, _) => node is ClassDeclarationSyntax declaration
                    && declaration.Identifier.ValueText.EndsWith("Tween", StringComparison.Ordinal),
                static (syntax, token) => ReadDefinition(syntax, token))
            .Where(static definition => definition.HasValue)
            .Select(static (definition, _) => definition!.Value)
            .WithTrackingName("Definitions");

        context.RegisterSourceOutput(definitions.Combine(options), static (output, input) =>
        {
            if (input.Right is null) return;
            var (name, adapter, target, value, kind) = input.Left;
            output.AddSource(name + ".g.cs", SourceText.From(
                Render(name, adapter, target, value, kind, input.Right), Encoding.UTF8));
        });
    }

    private static INamedTypeSymbol? ReadType(GeneratorSyntaxContext context, CancellationToken token)
    {
        var symbol = context.SemanticModel.GetDeclaredSymbol((ClassDeclarationSyntax)context.Node, token) as INamedTypeSymbol;
        if (symbol is null || symbol.ContainingType is not null
            || symbol.ContainingNamespace.ToDisplayString() != "tweens.gd") return null;
        // A partial class is one adapter, regardless of how many declarations it has.
        var first = symbol.DeclaringSyntaxReferences[0];
        return first.SyntaxTree == context.Node.SyntaxTree && first.Span == context.Node.Span ? symbol : null;
    }

    private static string? ReadOptions(GeneratorSyntaxContext context, CancellationToken token)
    {
        var symbol = ReadType(context, token);
        if (symbol is null) return null;
        var source = new StringBuilder();
        foreach (var property in symbol.GetMembers().OfType<IPropertySymbol>())
        {
            if (property.IsStatic || property.IsIndexer
                || property.GetMethod?.DeclaredAccessibility != Accessibility.Public
                || property.SetMethod?.DeclaredAccessibility != Accessibility.Public) continue;
            var name = "@" + property.Name;
            source.Append("    public ").Append(property.Type.ToDisplayString(TypeFormat)).Append(' ').Append(name)
                .Append(" { get => Options.").Append(name)
                .Append("; init => Options = Options with { ").Append(name).Append(" = value }; }\n");
        }
        return source.ToString();
    }

    private static (string Name, string Adapter, string Target, string Value, DefinitionKind Kind)? ReadDefinition(
        GeneratorSyntaxContext context, CancellationToken token)
    {
        var symbol = ReadType(context, token);
        if (symbol is null || symbol.IsAbstract || symbol.DeclaredAccessibility != Accessibility.Public) return null;

        var kind = GetDefinitionKind(symbol);
        if (kind is null) return null;

        for (var parent = symbol.BaseType; parent is not null; parent = parent.BaseType)
        {
            if (parent.Name != "TweenDefinition" || parent.Arity != 2
                || parent.ContainingNamespace.ToDisplayString() != "tweens.gd") continue;
            return (symbol.Name.Substring(0, symbol.Name.Length - "Tween".Length),
                symbol.ToDisplayString(TypeFormat), parent.TypeArguments[0].ToDisplayString(TypeFormat),
                parent.TypeArguments[1].ToDisplayString(TypeFormat), kind.Value);
        }
        return null;
    }

    private static DefinitionKind? GetDefinitionKind(INamedTypeSymbol symbol)
    {
        if (symbol.Name == "PropertyTween" && symbol.Arity == 2)
            return DefinitionKind.CustomProperty;
        if (symbol.IsSealed && symbol.Arity == 1 && symbol.Name is
            "ShaderParameterTween" or "CanvasItemInstanceShaderParameterTween" or "GeometryInstanceShaderParameterTween")
            return DefinitionKind.ShaderParameter;
        if (symbol.IsSealed && symbol.Arity == 0
            && symbol.BaseType is { Name: "PropertyTween", Arity: 2 } parent
            && parent.ContainingNamespace.ToDisplayString() == "tweens.gd"
            && symbol.InstanceConstructors.Any(constructor => constructor.Parameters.Length == 0
                && constructor.DeclaredAccessibility == Accessibility.Public))
            return DefinitionKind.BuiltIn;
        return null;
    }

    private static string Render(string name, string adapter, string target, string value, DefinitionKind kind, string options)
    {
        var generic = kind == DefinitionKind.CustomProperty ? "<TTarget, TValue>"
            : kind == DefinitionKind.ShaderParameter ? "<TValue>" : "";
        var instance = "TweenInstance<" + target + ", " + value + ">";
        var binding = "TweenDefinition<" + target + ", " + value + ">";
        var contract = "ITweenDefinition<" + target + ", " + value + ">";
        var source = new StringBuilder(
            "// SPDX-License-Identifier: MIT\n// SPDX-FileCopyrightText: 2026 Moritz Voss\n\n"
            + "// <auto-generated />\n#nullable enable\nusing global::System;\nusing global::tweens.gd;\n\nnamespace Tweens;\n\n");
        source.Append("/// <summary>Reusable immutable ").Append(name)
            .Append(" configuration. Each start creates independent playback.</summary>\n")
            .Append("public readonly record struct ").Append(name).Append(generic).Append(" : ").Append(contract);
        if (kind == DefinitionKind.CustomProperty) source.Append("\n    where TTarget : class where TValue : struct");
        else if (kind == DefinitionKind.ShaderParameter) source.Append("\n    where TValue : struct");
        source.Append("\n{\n    public TweenOptions Options { get; init; }\n").Append(options).Append('\n')
            .Append("    public ").Append(value).Append("? From { get; init; }\n")
            .Append("    public ").Append(value).Append("? To { get; init; }\n");
        foreach (var callback in Callbacks)
            source.Append("    public Action<").Append(instance).Append(callback == "OnUpdate" ? ", " + value : "")
                .Append(">? ").Append(callback).Append(" { get; init; }\n");

        if (kind == DefinitionKind.ShaderParameter)
            source.Append("\n    public string Parameter { get; init; }\n    public ").Append(name)
                .Append("(string parameter) => Parameter = parameter;\n");
        else if (kind == DefinitionKind.CustomProperty)
            source.Append("\n    public Func<TTarget, TValue> Getter { get; init; }\n")
                .Append("    public Action<TTarget, TValue> Setter { get; init; }\n")
                .Append("    public Func<TValue, TValue, float, TValue> Interpolate { get; init; }\n\n")
                .Append("    public Property(Func<TTarget, TValue> getter, Action<TTarget, TValue> setter, Func<TValue, TValue, float, TValue> interpolate)\n")
                .Append("    {\n        Getter = getter;\n        Setter = setter;\n        Interpolate = interpolate;\n    }\n");

        source.Append('\n').Append("    ").Append(binding).Append(' ').Append(contract).Append(".CreatePlayback()\n    {\n")
            .Append("        var playback = new ").Append(adapter)
            .Append(kind == DefinitionKind.ShaderParameter ? "(Parameter)"
                : kind == DefinitionKind.CustomProperty ? "(Getter, Setter, Interpolate)" : "()")
            .Append("\n        {\n            From = From,\n            To = To,\n");
        foreach (var callback in Callbacks)
            source.Append("            ").Append(callback).Append(" = ").Append(callback).Append(",\n");
        source.Append("        };\n        Options.CopyTo(playback);\n        return playback;\n    }\n}\n");
        return source.ToString();
    }

    private enum DefinitionKind { BuiltIn, ShaderParameter, CustomProperty }
}

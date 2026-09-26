// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using System.Reflection;
using System.Runtime.ExceptionServices;
using Godot;
using tweens.gd.Tests.Support;
using twodog.Testing;
using twodog.Testing.Xunit;

namespace tweens.gd.Tests.Integration;

/// <summary>
/// Every convenience method starts its adapter on the automatic runner. Configure overloads see a definition carrying
/// the arguments; options overloads copy the options while the explicit duration wins.
/// </summary>
[Collection<HeadlessCollection>]
public class ExtensionCatalogTests(HeadlessFixture godot)
{
    private static readonly TweenOptions Options = new() { Duration = 99, Delay = 0.25, Fill = FillMode.Both };

    [Fact]
    public void SkewWorksThroughOptionsConfiguratorsAndGeneratedDefinitions()
    {
        using var scope = new SceneScope(godot);
        var options = scope.Add(new Node2D()).TweenPositionX(16, 1, new TweenOptions { Skew = 2 });
        var configured = scope.Add(new Node2D()).TweenPositionX(16, 1, d => d.Skew = 0.5);
        var generated = scope.Add(new Node2D()).Tween(new Tweens.Position2DX { To = 16, Duration = 1, Skew = 2 });
        scope.Advance(0.25);
        Assert.Equal(1, options.Value);
        Assert.Equal(8, configured.Value);
        Assert.Equal(1, generated.Value);
    }

    private static Dictionary<string, MethodInfo[]> Families() => typeof(TweenExtensions)
        .GetMethods(BindingFlags.Public | BindingFlags.Static)
        .Where(method => !method.IsGenericMethodDefinition && method.Name != nameof(TweenExtensions.CancelTweens))
        .GroupBy(method => $"{method.Name}({method.GetParameters()[0].ParameterType.Name})")
        .ToDictionary(group => group.Key, group => group.ToArray());

    public static TheoryData<string> Names() => new(Families().Keys.Order());

    [Fact]
    public void EveryFamilyHasBothConfigureAndOptionsOverloads()
    {
        var families = Families();
        Assert.True(families.Count > 300, $"Only {families.Count} extension families were found.");
        foreach (var (name, overloads) in families)
        {
            Assert.Contains(overloads, method => Configure(method) is not null);
            Assert.Contains(overloads, method => method.GetParameters().Any(p => p.ParameterType == typeof(TweenOptions)));
            Assert.Single(overloads.Select(Configure).OfType<ParameterInfo>().Select(p => p.ParameterType).Distinct());
            Assert.All(overloads, method => Assert.StartsWith(method.Name, name));
        }
    }

    [Theory]
    [MemberData(nameof(Names))]
    public void ExtensionStartsItsAdapter(string family)
    {
        var overloads = Families()[family];
        var adapter = Adapter(overloads);
        foreach (var method in overloads)
        {
            var arguments = adapter.BaseType!.GetGenericArguments();
            try
            {
                GetType().GetMethod(nameof(Exercise), BindingFlags.NonPublic | BindingFlags.Instance)!
                    .MakeGenericMethod(arguments).Invoke(this, [method, adapter]);
            }
            catch (TargetInvocationException error) when (error.InnerException is not null)
            {
                ExceptionDispatchInfo.Capture(error.InnerException).Throw();
            }
        }
    }

    private static ParameterInfo? Configure(MethodInfo method) => method.GetParameters().SingleOrDefault(parameter =>
        parameter.ParameterType.IsGenericType && parameter.ParameterType.GetGenericTypeDefinition() == typeof(Action<>));

    private static Type Adapter(MethodInfo[] overloads)
        => overloads.Select(Configure).First(parameter => parameter is not null)!.ParameterType.GetGenericArguments()[0];

    private void Exercise<TTarget, TValue>(MethodInfo method, Type adapter) where TTarget : class where TValue : struct
    {
        using var scope = new SceneScope(godot);
        var target = (TTarget)(object)Targets.Create(scope, method.GetParameters()[0].ParameterType);
        TValue Read()
        {
            using var scheduler = new TweenScheduler();
            var probe = scheduler.Add(target, (TweenDefinition<TTarget, TValue>)Activator.CreateInstance(adapter)!);
            probe.Cancel();
            return probe.Value;
        }
        var to = (TValue)Values.Perturb(Read());

        var owners = method.GetParameters().Any(p => p.Name == "owner" && p.IsOptional) ? new[] { null, scope.Root } : new Node?[] { null };
        foreach (var owner in owners)
        {
            var captured = new List<TweenDefinition<TTarget, TValue>>();
            foreach (var configure in new Action<TweenDefinition<TTarget, TValue>>?[] { captured.Add, null })
            {
                var arguments = Arguments(method, target, to, scope, owner, configure, adapter);
                var current = Read();
                var tween = (TweenInstance<TTarget, TValue>)method.Invoke(null, arguments)!;
                Assert.Same(target, tween.Target);
                Assert.Equal(current, tween.Value);
                if (Configure(method) is null)
                {
                    // Delay from the options, duration from the argument.
                    scope.Advance(0.75);
                    Assert.Equal(0.5f, tween.Progress);
                }
                tween.Cancel();
                if (Configure(method) is null) break;
            }
            if (Configure(method) is null) continue;
            var definition = Assert.Single(captured);
            Assert.IsType(adapter, definition);
            Assert.Equal(to, definition.To);
            Assert.Equal(1, definition.Duration);
        }
    }

    private static object?[] Arguments<TTarget, TValue>(MethodInfo method, TTarget target, TValue to, SceneScope scope,
        Node? owner, Action<TweenDefinition<TTarget, TValue>>? configure, Type adapter)
        where TTarget : class where TValue : struct
    {
        var parameters = method.GetParameters();
        var arguments = new object?[parameters.Length];
        arguments[0] = target;
        arguments[1] = to;
        arguments[2] = 1d;
        for (var i = 3; i < parameters.Length; i++)
        {
            var type = parameters[i].ParameterType;
            arguments[i] = type == typeof(SceneTree) ? scope.Tree
                : type == typeof(TweenOptions) ? Options
                : type == typeof(Node) ? parameters[i].IsOptional ? owner : scope.Root
                : configure is null ? null
                : typeof(ExtensionCatalogTests).GetMethod(nameof(Forward), BindingFlags.NonPublic | BindingFlags.Static)!
                    .MakeGenericMethod(adapter, typeof(TTarget), typeof(TValue)).Invoke(null, [configure]);
        }
        return arguments;
    }

    private static Action<TAdapter> Forward<TAdapter, TTarget, TValue>(Action<TweenDefinition<TTarget, TValue>> sink)
        where TAdapter : TweenDefinition<TTarget, TValue> where TTarget : class where TValue : struct
        => definition => sink(definition);
}

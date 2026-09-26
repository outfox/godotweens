// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using System.Reflection;
using System.Runtime.ExceptionServices;
using Godot;
using tweens.gd.Tests.Support;
using twodog.Testing;
using twodog.Testing.Xunit;

namespace tweens.gd.Tests.Integration;

/// <summary>Every built-in property adapter writes, reads back and restores its Godot property.</summary>
[Collection<HeadlessCollection>]
public class AdapterCatalogTests(HeadlessFixture godot)
{
    public static IEnumerable<Type> AdapterTypes() => typeof(TweenScheduler).Assembly.GetExportedTypes()
        .Where(type => type is { IsSealed: true, IsGenericType: false, BaseType.IsGenericType: true }
            && type.BaseType.GetGenericTypeDefinition() == typeof(PropertyTween<,>))
        .OrderBy(type => type.Name);

    public static TheoryData<Type> Adapters() => new(AdapterTypes().Where(type => !IsValueTween(type)));

    // Value tweens only report samples; they have no Godot property.
    private static bool IsValueTween(Type adapter) => adapter.BaseType!.GetGenericArguments()[0] == typeof(Node);

    [Theory]
    [MemberData(nameof(Adapters))]
    public void AdapterDrivesItsProperty(Type adapter)
    {
        var arguments = adapter.BaseType!.GetGenericArguments();
        try
        {
            GetType().GetMethod(nameof(Verify), BindingFlags.NonPublic | BindingFlags.Instance)!
                .MakeGenericMethod(arguments).Invoke(this, [adapter]);
        }
        catch (TargetInvocationException error) when (error.InnerException is not null)
        {
            ExceptionDispatchInfo.Capture(error.InnerException).Throw();
        }
    }

    private void Verify<TTarget, TValue>(Type adapter) where TTarget : class where TValue : struct
    {
        using var scope = new SceneScope(godot);
        using var scheduler = new TweenScheduler();
        var target = (TTarget)(object)Targets.Create(scope, typeof(TTarget));
        TweenDefinition<TTarget, TValue> Create() => (TweenDefinition<TTarget, TValue>)Activator.CreateInstance(adapter)!;
        TValue Read()
        {
            var probe = scheduler.Add(target, Create());
            probe.Cancel();
            return probe.Value;
        }

        // Start from a written in-range value; some defaults are sentinels, such as -1 for "unlimited".
        var start = Create();
        start.To = (TValue)Values.Perturb(Read());
        scheduler.Add(target, start);
        scheduler.Update(0);
        var initial = Read();
        Values.AssertClose(start.To, initial, "after an instant write");
        var to = (TValue)Values.Perturb(initial);
        var forward = Create();
        forward.To = to;
        forward.Duration = 1;
        var tween = scheduler.Add(target, forward);
        scheduler.Update(0.5);
        Values.AssertClose(Values.Interpolate(initial, to, 0.5f), Read(), "at the midpoint");
        scheduler.Update(0.5);
        Assert.Equal(Reason.Completed, tween.CompletionReason);
        Values.AssertClose(to, Read(), "at the end");

        // Non-retaining playback restores the value captured when it started.
        var restoring = Create();
        restoring.To = initial;
        restoring.Duration = 1;
        restoring.Fill = FillMode.None;
        scheduler.Add(target, restoring);
        scheduler.Update(0.5);
        Values.AssertClose(Values.Interpolate(to, initial, 0.5f), Read(), "while restoring");
        scheduler.Update(0.5);
        Values.AssertClose(to, Read(), "after restoring");
    }

    [Fact]
    public void ValueTweensReportSamplesWithoutAProperty()
    {
        using var scope = new SceneScope(godot);
        using var scheduler = new TweenScheduler();
        var valueTweens = AdapterTypes().Where(IsValueTween).ToList();
        Assert.Equal(8, valueTweens.Count);
        foreach (var adapter in valueTweens)
        {
            var value = adapter.BaseType!.GetGenericArguments()[1];
            GetType().GetMethod(nameof(Sample), BindingFlags.NonPublic | BindingFlags.Static)!
                .MakeGenericMethod(value).Invoke(null, [scheduler, scope.Root, adapter]);
        }
    }

    private static void Sample<TValue>(TweenScheduler scheduler, Node node, Type adapter) where TValue : struct
    {
        var definition = (TweenDefinition<Node, TValue>)Activator.CreateInstance(adapter)!;
        var reported = new List<TValue>();
        definition.Duration = 1;
        definition.OnUpdate = (_, value) => reported.Add(value);
        var tween = scheduler.Add(node, definition);
        var start = tween.Value;
        var to = (TValue)Values.Perturb(start);
        tween.Cancel();
        definition.To = to;
        scheduler.Add(node, definition);
        scheduler.Update(0.5);
        scheduler.Update(0.5);
        Assert.Equal(2, reported.Count);
        Values.AssertClose(Values.Interpolate(start, to, 0.5f), reported[0], adapter.Name);
        Values.AssertClose(to, reported[1], adapter.Name);
    }
}

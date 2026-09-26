// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using Godot;
using tweens.gd.Tests.Support;
using twodog.Testing.Xunit;
using Engine = twodog.Engine;

namespace tweens.gd.Tests.Lifecycle;

/// <summary>Shutdown needs its own engine: each test starts one, in a test process of their own.</summary>
[CollectionDefinition(nameof(EngineLifecycleCollection), DisableParallelization = true)]
public sealed class EngineLifecycleCollection;

[Collection(nameof(EngineLifecycleCollection))]
public class EngineShutdownTests
{
    /// <summary>Arranges on a fresh engine, shuts it down, then verifies. Godot must report no errors throughout.</summary>
    private static Task AfterShutdown(Func<Engine, Action> arrange) => GodotTestThread.Run(() =>
    {
        var engine = new Engine("tweens.gd.tests", Engine.ResolveProjectDir(), "--headless") { CaptureErrors = true };
        Action verify;
        try
        {
            engine.Start();
            verify = arrange(engine);
        }
        finally
        {
            // Finalize collected Godot wrappers while their engine is alive; a later engine would reuse what they free.
            GC.Collect();
            GC.WaitForPendingFinalizers();
            engine.Dispose();
        }
        verify();
        Assert.Empty(engine.Errors.Drain());
        return ValueTask.FromResult(true);
    });

    [Fact]
    public Task ShutdownDisposesAnAttachedRunnersTweens() => AfterShutdown(engine =>
    {
        var node = new Node2D();
        engine.Tree.Root.AddChild(node);
        var tween = node.TweenPositionX(10, 100);
        engine.Iteration();
        Assert.True(TweenRuntime.GetRunner(engine.Tree).IsInsideTree());
        return () => Assert.Equal(Reason.RunnerDisposed, tween.CompletionReason);
    });

    [Fact]
    public Task ShutdownBeforeAttachmentFreesThePendingRunner() => AfterShutdown(engine =>
    {
        var tree = engine.Tree;
        var material = new StandardMaterial3D();
        var tween = material.TweenMetallic(1, 100, tree);
        var runner = TweenRuntime.GetRunner(tree);
        Assert.False(runner.IsInsideTree());
        Exception? closing = null, exited = null;
        tree.Root.TreeExiting += () =>
        {
            try { material.TweenRoughness(0, 1, tree); }
            catch (Exception error) { closing = error; }
        };
        tree.Root.TreeExited += () =>
        {
            try { material.TweenRoughness(0, 1, tree); }
            catch (Exception error) { exited = error; }
        };
        return () =>
        {
            Assert.Equal(Reason.RunnerDisposed, tween.CompletionReason);
            Assert.False(GodotObject.IsInstanceValid(runner));
            Assert.IsType<InvalidOperationException>(closing);
            Assert.IsType<ArgumentException>(exited);
        };
    });

    [Fact]
    public Task TweensBoundToADisposedTreeSettle() => AfterShutdown(engine =>
    {
        var scheduler = new TweenScheduler();
        var tween = scheduler.AddCore(new Box(), new PlainTween { Duration = 1 }, null, engine.Tree);
        scheduler.Update(0.5);
        Assert.Equal(0.5f, tween.Progress);
        return () =>
        {
            scheduler.Update(0.5);
            Assert.Equal(Reason.RunnerDisposed, tween.CompletionReason);
            scheduler.Dispose();
        };
    });
}

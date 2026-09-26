// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using Godot;
using tweens.gd.Tests.Support;
using twodog.Testing;
using twodog.Testing.Xunit;

namespace tweens.gd.Tests.Integration;

/// <summary>Playback follows the lifetime and pause state of its target, owner node and scene tree.</summary>
[Collection<HeadlessCollection>]
public class LifetimeTests(HeadlessFixture godot)
{
    [Fact]
    public async Task QueueFreedTargetsSettleAsTargetFreed()
    {
        using var scope = new SceneScope(godot);
        var node = scope.Add(new Node2D());
        var cancelled = false;
        var tween = node.TweenPositionX(10, 1, d => d.OnCancel = _ => cancelled = true);
        node.QueueFree();
        scope.Frames();
        Assert.Equal(Reason.TargetFreed, await tween.End);
        Assert.True(cancelled);
    }

    [Fact]
    public void TargetsQueuedForDeletionStopAtTheNextUpdate()
    {
        using var scope = new SceneScope(godot);
        var node = scope.Add(new Node2D());
        var tween = node.TweenPositionX(10, 1);
        node.QueueFree();
        scope.Advance(0.5);
        Assert.Equal(Reason.TargetFreed, tween.CompletionReason);
        Assert.Equal(0, node.Position.X);
    }

    [Fact]
    public void RemovingTheTargetFromTheTreeSettlesAsOwnerExited()
    {
        using var scope = new SceneScope(godot);
        var node = scope.Add(new Node2D());
        var tween = node.TweenPositionX(10, 1);
        scope.Root.RemoveChild(node);
        Assert.Equal(Reason.OwnerExited, tween.CompletionReason);
        node.Free();
    }

    [Fact]
    public void FinishedTweensNoLongerObserveTheirOwner()
    {
        using var scope = new SceneScope(godot);
        var node = scope.Add(new Node2D());
        var tween = node.TweenPositionX(10, 1);
        scope.Advance(1);
        Assert.Equal(Reason.Completed, tween.CompletionReason);
        scope.Root.RemoveChild(node);
        Assert.Equal(Reason.Completed, tween.CompletionReason);
        node.Free();
    }

    [Fact]
    public void SuppressedCallbacksSkipInvalidTargets()
    {
        using var scope = new SceneScope(godot);
        var calls = 0;
        var freed = scope.Add(new Node2D()).TweenPositionX(10, 1, d =>
        {
            d.SuppressCallbacksWhenTargetInvalid = true;
            d.OnFinally = _ => calls++;
        });
        var queued = scope.Add(new Node2D());
        var cancelled = queued.TweenPositionX(10, 1, d =>
        {
            d.SuppressCallbacksWhenTargetInvalid = true;
            d.OnFinally = _ => calls++;
        });
        ((Node2D)freed.Target).QueueFree();
        queued.QueueFree();
        cancelled.Cancel();
        scope.Frames();
        Assert.Equal(Reason.TargetFreed, freed.CompletionReason);
        Assert.Equal(Reason.Cancelled, cancelled.CompletionReason);
        Assert.Equal(0, calls);
    }

    [Fact]
    public void SuppressedCallbacksSkipOwnersLeavingTheTree()
    {
        using var scope = new SceneScope(godot);
        var node = scope.Add(new Node2D());
        var calls = 0;
        var tween = node.TweenPositionX(10, 1, d =>
        {
            d.SuppressCallbacksWhenTargetInvalid = true;
            d.OnFinally = _ => calls++;
        });
        scope.Root.RemoveChild(node);
        Assert.Equal(Reason.OwnerExited, tween.CompletionReason);
        Assert.Equal(0, calls);
        node.Free();
    }

    [Fact]
    public void ResourceTweensFollowTheirOwnerNode()
    {
        using var scope = new SceneScope(godot);
        var material = scope.Track(new StandardMaterial3D());
        var removed = scope.Add(new Node());
        var queued = scope.Add(new Node());
        var byRemoval = material.TweenMetallic(1, 1, removed);
        var byQueue = material.TweenRoughness(0, 1, queued);
        scope.Root.RemoveChild(removed);
        Assert.Equal(Reason.OwnerExited, byRemoval.CompletionReason);
        queued.QueueFree();
        scope.Advance(0.25);
        Assert.Equal(Reason.OwnerExited, byQueue.CompletionReason);
        removed.Free();
    }

    [Fact]
    public void DisposedResourceTargetsSettleAsTargetFreed()
    {
        using var scope = new SceneScope(godot);
        var material = new StandardMaterial3D();
        var tween = material.TweenMetallic(1, 1, godot.Tree);
        material.Dispose();
        scope.Advance(0.25);
        Assert.Equal(Reason.TargetFreed, tween.CompletionReason);
    }

    [Fact]
    public void TargetsInvalidatedWhilePreparingAreRejected()
    {
        using var scope = new SceneScope(godot);
        var node = scope.Add(new Node2D());
        var material = new StandardMaterial3D();
        Assert.Throws<ArgumentException>(() => node.Tween(new PreparingTween<Node2D>(n => n.QueueFree())));
        Assert.Throws<ArgumentException>(() => material.Tween(new PreparingTween<StandardMaterial3D>(m => m.Dispose()), godot.Tree));
    }

    [Fact]
    public void OwnersInvalidatedWhilePreparingAreRejected()
    {
        using var scope = new SceneScope(godot);
        var material = scope.Track(new StandardMaterial3D());
        var freed = scope.Add(new Node());
        var removed = scope.Add(new Node());
        Assert.Throws<ArgumentException>(() => material.Tween(new PreparingTween<StandardMaterial3D>(_ => freed.Free()), freed));
        Assert.Throws<ArgumentException>(() => material.Tween(
            new PreparingTween<StandardMaterial3D>(_ => scope.Root.RemoveChild(removed)), removed));
        removed.Free();
    }

    [Fact]
    public void TargetsFreedDuringTheInitialReadSettleImmediately()
    {
        using var scope = new SceneScope(godot);
        var node = scope.Add(new Node2D());
        var tween = node.Tween(new PropertyTween<Node2D, float>(n =>
        {
            n.QueueFree();
            return 0;
        }, (_, _) => { }, Interpolators.Float) { Duration = 1 });
        Assert.Equal(Reason.TargetFreed, tween.CompletionReason);
    }

    [Fact]
    public void OwnersLeavingDuringTheInitialReadSettleImmediately()
    {
        using var scope = new SceneScope(godot);
        var owner = scope.Add(new Node());
        var material = scope.Track(new StandardMaterial3D());
        var tween = material.Tween(new PropertyTween<StandardMaterial3D, float>(m =>
        {
            scope.Root.RemoveChild(owner);
            return m.Metallic;
        }, (m, v) => m.Metallic = v, Interpolators.Float) { To = 1, Duration = 1 }, owner);
        Assert.Equal(Reason.OwnerExited, tween.CompletionReason);
        owner.Free();
    }

    [Fact]
    public void BoundTweensFollowTheirNodesProcessMode()
    {
        using var scope = new SceneScope(godot);
        var node = scope.Add(new Node2D { ProcessMode = Node.ProcessModeEnum.Disabled });
        var bound = node.TweenPositionX(10, 1);
        var always = node.TweenRotation(1, 1, d => d.PauseMode = TweenPauseMode.Always);
        var treeBound = node.TweenScale(Vector2.One * 2, 1, d => d.PauseMode = TweenPauseMode.SceneTree);
        scope.Advance(0.5);
        Assert.Equal(0, bound.Progress);
        Assert.Equal(0.5f, always.Progress);
        Assert.Equal(0.5f, treeBound.Progress);
    }

    [Fact]
    public void PausedTreesHoldTreeFollowingTweens()
    {
        using var scope = new SceneScope(godot);
        var node = scope.Add(new Node2D());
        var material = scope.Track(new StandardMaterial3D());
        var bound = node.TweenPositionX(10, 1);
        var treeBound = node.TweenRotation(1, 1, d => d.PauseMode = TweenPauseMode.SceneTree);
        var always = node.TweenScale(Vector2.One * 2, 1, d => d.PauseMode = TweenPauseMode.Always);
        var resource = material.TweenMetallic(1, 1, godot.Tree);
        var ownedResource = material.TweenRoughness(0, 1, godot.Tree, owner: node);
        godot.Tree.Paused = true;
        try
        {
            scope.Advance(0.5);
            Assert.Equal(0, bound.Progress);
            Assert.Equal(0, treeBound.Progress);
            Assert.Equal(0.5f, always.Progress);
            Assert.Equal(0, resource.Progress);
            Assert.Equal(0, ownedResource.Progress);
        }
        finally { godot.Tree.Paused = false; }
        scope.Advance(0.5);
        Assert.Equal(0.5f, resource.Progress);
    }

    [Fact]
    public void LanesAdvanceIndependently()
    {
        using var scope = new SceneScope(godot);
        var node = scope.Add(new Node2D());
        var process = node.TweenPositionX(10, 1);
        var physics = node.TweenRotation(1, 1, d => d.ProcessMode = TweenProcessMode.Physics);
        scope.Advance(0.5);
        scope.Advance(0.25, TweenProcessMode.Physics);
        Assert.Equal(0.5f, process.Progress);
        Assert.Equal(0.25f, physics.Progress);
    }

    [Fact]
    public void ScheduledNodeOwnersBindPlainTargets()
    {
        using var scope = new SceneScope(godot);
        using var scheduler = new TweenScheduler();
        var owner = scope.Add(new Node { ProcessMode = Node.ProcessModeEnum.Disabled });
        var box = new Box();
        var tween = scheduler.Add(box, new PlainTween { To = 1, Duration = 1 }, owner);
        scheduler.Update(0.5);
        Assert.Equal(0, box.Value);
        owner.ProcessMode = Node.ProcessModeEnum.Inherit;
        scheduler.Update(0.5);
        Assert.Equal(0.5f, box.Value);
        owner.QueueFree();
        scheduler.Update(0.5);
        Assert.Equal(Reason.OwnerExited, tween.CompletionReason);
    }

    [Fact]
    public async Task FailuresOnTheAutomaticRunnerAreReportedToGodot()
    {
        using var scope = new SceneScope(godot);
        var tween = scope.Add(new Node2D()).TweenPositionX(10, 1, d => d.OnStart = _ => throw new FormatException("broken start"));
        // The error log still records the report; only its console output is silenced.
        Engine.PrintErrorMessages = false;
        try { scope.Advance(0.5); }
        finally { Engine.PrintErrorMessages = true; }
        await Assert.ThrowsAsync<FormatException>(() => tween.End);
        godot.Errors.Expect("broken start");
    }

    [Fact]
    public void CurveEasingSamplesAPrivateCopy()
    {
        using var scope = new SceneScope(godot);
        var curve = scope.Track(new Curve());
        curve.AddPoint(new Vector2(0, 0));
        curve.AddPoint(new Vector2(0.5f, 0.8f));
        curve.AddPoint(new Vector2(1, 1));
        var node = scope.Add(new Node2D());
        var tween = node.TweenPositionX(10, 1, d => d.Curve = curve);
        curve.ClearPoints();
        scope.Advance(0.5);
        Assert.Equal(8, node.Position.X, 3);
        scope.Advance(0.5);
        Assert.Equal(Reason.Completed, tween.CompletionReason);
        Assert.Throws<ArgumentException>(() => node.TweenPositionX(10, 1, d =>
        {
            d.Curve = curve;
            d.EaseFunction = static x => x;
        }));
    }

    private sealed class PreparingTween<TTarget>(Action<TTarget> prepare) : TweenDefinition<TTarget, float>
        where TTarget : GodotObject
    {
        protected override void Prepare(TTarget target) => prepare(target);
        protected override float Read(TTarget target) => 0;
        protected override void Write(TTarget target, float value) { }
        protected override float Interpolate(float from, float to, float weight) => to;
    }
}

// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using Godot;
using tweens.gd;
using twodog.Testing;
using twodog.Testing.Xunit;

namespace testbed.Tests;

[Collection<HeadlessCollection>]
public class NodeTweenTests(HeadlessFixture godot)
{
    private T Attach<T>(T node) where T : Node
    {
        godot.Tree.Root.AddChild(node);
        return node;
    }

    [Fact]
    public async Task AutomaticRunnerBootstrapsFromReadyAndCompletesOnEngineThread()
    {
        var node = Attach(new RunnerReadyProbe());
        try
        {
            var thread = System.Environment.CurrentManagedThreadId;
            var continuationThread = -1;
            var task = Observe();
            for (var i = 0; i < 20 && !task.IsCompleted; i++) godot.Engine.Iteration();
            Assert.True(task.IsCompleted, "Engine-pumped async completion must not hang.");
            await task;
            Assert.Equal(thread, continuationThread);
            Assert.Equal(new Vector2(20, 30), node.Position);

            async Task Observe()
            {
                Assert.Equal(Reason.Completed, await node.Movement!.End);
                continuationThread = System.Environment.CurrentManagedThreadId;
                node.Tween(new Position2DTween { To = new Vector2(30, 40), Duration = 1 }).Cancel();
            }
        }
        finally { node.Free(); }
    }

    [Fact]
    public async Task QueueFreeSettlesPausedTweenAndSuppressesCallbacks()
    {
        var node = Attach(new Node2D());
        var calls = 0;
        var tween = node.Tween(new Position2DTween { Duration = 10,
            SuppressCallbacksWhenTargetInvalid = true, OnFinally = _ => calls++ });
        tween.Pause();
        node.QueueFree();
        for (var i = 0; i < 5 && !tween.IsTerminal; i++) godot.Engine.Iteration();
        Assert.True(tween.End.IsCompleted);
        Assert.Equal(Reason.TargetFreed, await tween.End);
        Assert.Equal(0, calls);
    }

    [Fact]
    public async Task TreeRemovalSettlesWaiterImmediatelyEvenWhenTreePaused()
    {
        var node = Attach(new Node());
        var tween = node.Tween(new FloatTween { Duration = 100 });
        try
        {
            godot.Tree.Paused = true;
            godot.Tree.Root.RemoveChild(node);
            Assert.True(tween.End.IsCompleted);
            Assert.Equal(Reason.OwnerExited, await tween.End);
        }
        finally { godot.Tree.Paused = false; node.Free(); }
    }

    [Fact]
    public async Task GroupStartsDefinitionsForBaseTypesTogether()
    {
        var sprite = Attach(new Sprite2D());
        try
        {
            // Scale2DTween targets Node2D and ModulateAlphaTween targets CanvasItem.
            var group = sprite.Tween(new Scale2DTween { To = new Vector2(2, 2), Duration = 0.05 },
                new ModulateAlphaTween { To = 0, Duration = 0.05 });
            Assert.Equal(2, group.Members.Count);
            for (var i = 0; i < 50 && !group.IsTerminal; i++) godot.Engine.Iteration();
            Assert.Equal(Reason.Completed, await group.End);
            Assert.Equal(new Vector2(2, 2), sprite.Scale);
            Assert.Equal(0, sprite.Modulate.A);
        }
        finally { sprite.Free(); }
    }

    [Fact]
    public void GroupStartFailureCancelsTheTweensAlreadyStarted()
    {
        var node = Attach(new Node2D());
        try
        {
            var started = new List<TweenInstance>();
            var valid = new Position2DXTween { To = 10, Duration = 1, OnAdd = started.Add };
            var invalid = new Position2DYTween { To = 10, Duration = 1, Offset = 2 };
            Assert.Throws<ArgumentOutOfRangeException>(() => node.Tween(valid, invalid));
            Assert.Equal(TweenState.Cancelled, Assert.Single(started).State);
        }
        finally { node.Free(); }
    }

    [Fact]
    public void OptionsOverloadCopiesOptionsButKeepsExplicitDuration()
    {
        var node = Attach(new Node2D());
        // An offset beyond options.Duration would throw if that duration were applied.
        var options = new TweenOptions { Duration = 1, Offset = 5 };
        try
        {
            var offset = node.TweenPositionX(10, 10, options);
            var delayed = node.TweenPositionY(10, 10, new TweenOptions { Delay = 1000 });
            godot.Engine.Iteration();
            Assert.InRange(offset.Progress, 0.5f, 0.9f);
            Assert.Equal(TweenState.Delayed, delayed.State);
            Assert.Equal(1, options.Duration);
            var defaults = node.TweenPositionX(10, 10, default(TweenOptions));
            defaults.Cancel();
        }
        finally { node.Free(); }
    }

    [Fact]
    public void AxisAndAlphaTweensPreserveConcurrentChanges()
    {
        using var scheduler = new TweenScheduler();
        var node = Attach(new Node2D { Position = new Vector2(2, 3), Modulate = new Color(1, 0, 0, 1) });
        try
        {
            scheduler.Add(node, new Position2DXTween { To = 12, Duration = 1 });
            scheduler.Add(node, new Position2DYTween { To = 23, Duration = 1 });
            scheduler.Add<CanvasItem, float>(node, new ModulateAlphaTween { To = 0, Duration = 1 });
            node.Modulate = new Color(0, 1, 0, 1);
            scheduler.Update(0.5);
            Assert.Equal(new Vector2(7, 13), node.Position);
            Assert.Equal(new Color(0, 1, 0, 0.5f), node.Modulate);
        }
        finally { node.Free(); }
    }

    [Fact]
    public void LocalAndGlobalTransformAdaptersRespectParentsAndScale()
    {
        using var scheduler = new TweenScheduler();
        var parent = Attach(new Node3D { Position = new Vector3(10, 0, 0) });
        var child = new Node3D { Scale = new Vector3(2, 3, 4) };
        parent.AddChild(child);
        try
        {
            scheduler.Add(child, new GlobalPosition3DTween { To = new Vector3(30, 0, 0), Duration = 1 });
            scheduler.Add(child, new Quaternion3DTween { To = new Quaternion(Vector3.Up, Mathf.Pi / 2), Duration = 1 });
            scheduler.Update(0.5);
            Assert.True(child.GlobalPosition.IsEqualApprox(new Vector3(20, 0, 0)));
            Assert.True(child.Position.IsEqualApprox(new Vector3(10, 0, 0)));
            Assert.True(child.Quaternion.IsEqualApprox(new Quaternion(Vector3.Up, Mathf.Pi / 4)));
            Assert.True(child.Scale.IsEqualApprox(new Vector3(2, 3, 4)));
        }
        finally { parent.Free(); }
    }

    [Fact]
    public void OwnerPauseModesAndInstancePauseAreIndependent()
    {
        using var scheduler = new TweenScheduler();
        var node = Attach(new Node2D());
        try
        {
            var bound = scheduler.Add(node, new Position2DXTween { To = 10, Duration = 1 });
            var always = scheduler.Add(node, new Position2DYTween { To = 10, Duration = 1, PauseMode = TweenPauseMode.Always });
            godot.Tree.Paused = true;
            scheduler.Update(0.5);
            Assert.Equal(new Vector2(0, 5), node.Position);
            always.Pause();
            node.ProcessMode = Node.ProcessModeEnum.Always;
            scheduler.Update(0.5);
            Assert.Equal(new Vector2(5, 5), node.Position);
            bound.Cancel(); always.Cancel();
        }
        finally { godot.Tree.Paused = false; node.Free(); }
    }

    [Fact]
    public void FreeFromCallbackPreventsPropertyWrite()
    {
        using var scheduler = new TweenScheduler();
        var node = Attach(new Node2D());
        var tween = scheduler.Add(node, new Position2DTween { To = Vector2.One, OnStart = _ => node.Free() });
        scheduler.Update(1);
        Assert.True(tween.IsTerminal);
        Assert.Null(tween.Error);
    }

    [Fact]
    public void CancelDescendantsDoesNotCancelSibling()
    {
        var parent = Attach(new Node()); var child = new Node(); parent.AddChild(child);
        var sibling = Attach(new Node());
        try
        {
            var a = parent.Tween(new FloatTween { Duration = 10 });
            var b = child.Tween(new FloatTween { Duration = 10 });
            var c = sibling.Tween(new FloatTween { Duration = 10 });
            parent.CancelTweens(true);
            Assert.True(a.IsTerminal); Assert.True(b.IsTerminal); Assert.False(c.IsTerminal);
        }
        finally { parent.Free(); sibling.Free(); }
    }

    [Fact]
    public async Task RemovingRunnerSettlesWaitersAndNextAdditionCreatesFreshRunner()
    {
        var node = Attach(new Node());
        try
        {
            var a = node.Tween(new FloatTween { Duration = 10 });
            godot.Engine.Iteration();
            var runner = TweenRuntime.GetRunner(node);
            runner.Free();
            Assert.Equal(Reason.RunnerDisposed, await a.End);
            var b = node.Tween(new FloatTween());
            Assert.NotSame(runner, TweenRuntime.GetRunner(node));
            for (var i = 0; i < 10 && !b.IsTerminal; i++) godot.Engine.Iteration();
            Assert.Equal(TweenState.Completed, b.State);
        }
        finally { node.Free(); }
    }

    [Fact]
    public void CurveIsSnapshottedAndOverridesEaseEnum()
    {
        using var scheduler = new TweenScheduler();
        using var curve = new Curve();
        curve.AddPoint(new Vector2(0, 0.25f));
        curve.AddPoint(new Vector2(1, 0.75f));
        var node = Attach(new Node2D());
        try
        {
            scheduler.Add(node, new Position2DXTween { From = 0, To = 10, Duration = 1, Curve = curve, Ease = EaseType.ExpoIn });
            curve.SetPointValue(1, 0);
            scheduler.Update(1);
            Assert.Equal(7.5f, node.Position.X);
            Assert.Throws<ArgumentException>(() => scheduler.Add(node,
                new Position2DXTween { Curve = curve, EaseFunction = x => x }));
        }
        finally { node.Free(); }
    }

    [Fact]
    public void UnscaledRunnerContinuesAtZeroTimeScaleAndPhysicsLaneRuns()
    {
        var node = Attach(new Node());
        var previousScale = Engine.TimeScale;
        try
        {
            Engine.TimeScale = 0;
            var scaled = node.Tween(new FloatTween { From = 0, To = 1, Duration = 1 });
            var unscaled = node.Tween(new FloatTween { From = 0, To = 1, Duration = 1, UseUnscaledTime = true });
            for (var i = 0; i < 10; i++) godot.Engine.Iteration();
            Assert.Equal(0, scaled.Progress);
            Assert.True(unscaled.Progress > 0);
            var physics = node.Tween(new FloatTween { Duration = 0, ProcessMode = TweenProcessMode.Physics });
            // Drive the runner callback deterministically; normal engine pumping is covered above.
            TweenRuntime.GetRunner(node)._PhysicsProcess(0);
            Assert.Equal(TweenState.Completed, physics.State);
        }
        finally { Engine.TimeScale = previousScale; node.Free(); }
    }

    [Fact]
    public void RejectsDetachedOrQueuedOwner()
    {
        var detached = new Node();
        try { Assert.Throws<ArgumentException>(() => detached.Tween(new FloatTween())); }
        finally { detached.Free(); }
        var queued = Attach(new Node());
        queued.QueueFree();
        Assert.Throws<ArgumentException>(() => queued.Tween(new FloatTween()));
        godot.Engine.Iteration();
    }

    [Fact]
    public void ControlAudioAndLightPropertiesInterpolate()
    {
        using var scheduler = new TweenScheduler();
        var container = Attach(new Node());
        var control = new Control { Position = new Vector2(10, 20), Size = new Vector2(100, 100) };
        var range = new ProgressBar { MinValue = 0, MaxValue = 200, Value = 20 };
        var audio = new AudioStreamPlayer { VolumeDb = -20, PitchScale = 1 };
        var audio2D = new AudioStreamPlayer2D { VolumeLinear = 1 };
        var audio3D = new AudioStreamPlayer3D { PitchScale = 1 };
        var light = new OmniLight3D { LightEnergy = 1, OmniRange = 10 };
        var spot = new SpotLight3D { SpotAngle = 20 };
        var light2D = new PointLight2D { Energy = 1 };
        foreach (var node in new Node[] { control, range, audio, audio2D, audio3D, light, spot, light2D }) container.AddChild(node);
        try
        {
            scheduler.Add(control, new ControlPositionTween { To = new Vector2(30, 40), Duration = 1 });
            scheduler.Add<Godot.Range, double>(range, new RangeValueTween { To = 100, Duration = 1 });
            scheduler.Add(audio, new AudioVolumeDbTween { To = 0, Duration = 1 });
            scheduler.Add(audio2D, new AudioVolumeLinear2DTween { To = 0, Duration = 1 });
            scheduler.Add(audio3D, new AudioPitchScale3DTween { To = 2, Duration = 1 });
            scheduler.Add<Light3D, float>(light, new LightEnergy3DTween { To = 3, Duration = 1 });
            scheduler.Add(light, new OmniRangeTween { To = 20, Duration = 1 });
            scheduler.Add(spot, new SpotAngleTween { To = 40, Duration = 1 });
            scheduler.Add<Light2D, float>(light2D, new LightEnergy2DTween { To = 3, Duration = 1 });
            scheduler.Update(0.5);
            Assert.Equal(new Vector2(20, 30), control.Position);
            Assert.Equal(60, range.Value);
            Assert.Equal(-10, audio.VolumeDb);
            Assert.InRange(audio2D.VolumeLinear, 0.499f, 0.501f);
            Assert.Equal(1.5f, audio3D.PitchScale);
            Assert.Equal(2, light.LightEnergy); Assert.Equal(15, light.OmniRange);
            Assert.Equal(30, spot.SpotAngle); Assert.Equal(2, light2D.Energy);
        }
        finally { container.Free(); }
    }
}

// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using Godot;
using tweens.gd;
using twodog.Testing;
using twodog.Testing.Xunit;

namespace testbed.Tests;

[Collection<HeadlessCollection>]
public class ResourceTweenTests(HeadlessFixture godot)
{
    private T Attach<T>(T node) where T : Node { godot.Tree.Root.AddChild(node); return node; }

    [Fact]
    public void SharedMaterialIsUpdatedWithoutCloningAndSurvivesMeshRemoval()
    {
        using var material = new StandardMaterial3D { Roughness = 0 };
        using var replacement = new StandardMaterial3D();
        var first = Attach(new MeshInstance3D { MaterialOverride = material });
        var second = Attach(new MeshInstance3D { MaterialOverride = material });
        try
        {
            var tween = material.TweenRoughness(1, 1, godot.Tree);
            var scheduler = TweenRuntime.GetRunner(godot.Tree).Scheduler;
            scheduler.Update(0.5);
            Assert.Same(material, first.MaterialOverride); Assert.Same(material, second.MaterialOverride);
            Assert.Equal(0.5f, ((BaseMaterial3D)second.MaterialOverride).Roughness);
            first.MaterialOverride = replacement;
            first.Free();
            scheduler.Update(0.5);
            Assert.Equal(1, material.Roughness);
            Assert.Equal(TweenState.Completed, tween.State);
            Assert.True(GodotObject.IsInstanceValid(material));
            Assert.Same(material, second.MaterialOverride);
        }
        finally { if (GodotObject.IsInstanceValid(first)) first.Free(); second.Free(); }
    }

    [Fact]
    public async Task OwnerCancellationIsIsolatedFromOtherOwnersAndTreeScope()
    {
        using var material = new StandardMaterial3D();
        var first = Attach(new Node()); var second = Attach(new Node());
        try
        {
            var a = first.Tween(material, new MaterialRoughnessTween { Duration = 10 });
            var b = material.TweenMetallic(1, 10, second);
            var c = material.TweenAlbedoAlpha(0, 10, godot.Tree);
            first.CancelTweens(true);
            Assert.Equal(Reason.Cancelled, a.CompletionReason);
            Assert.False(b.IsTerminal); Assert.False(c.IsTerminal);
            b.Pause(); godot.Tree.Root.RemoveChild(second);
            Assert.Equal(Reason.OwnerExited, await b.Completion);
            Assert.False(c.IsTerminal); c.Cancel();
            Assert.True(GodotObject.IsInstanceValid(material));
        }
        finally { first.Free(); second.Free(); }
    }

    [Fact]
    public void TreeScopeAndOwnerScopeHaveIndependentPauseRules()
    {
        using var material = new StandardMaterial3D { Roughness = 0, Metallic = 0, MetallicSpecular = 0 };
        var owner = Attach(new Node { ProcessMode = Node.ProcessModeEnum.Always });
        try
        {
            var tree = material.TweenRoughness(1, 1, godot.Tree);
            var bound = material.TweenMetallic(1, 1, godot.Tree, owner: owner);
            var always = material.TweenMetallicSpecular(1, 1, godot.Tree, d => d.PauseMode = TweenPauseMode.Always);
            godot.Tree.Paused = true;
            var scheduler = TweenRuntime.GetRunner(godot.Tree).Scheduler;
            scheduler.Update(0.5);
            Assert.Equal(0, material.Roughness); Assert.Equal(0.5f, material.Metallic); Assert.Equal(0.5f, material.MetallicSpecular);
            always.Pause(); scheduler.Update(0.5); Assert.Equal(0.5f, material.MetallicSpecular);
            godot.Tree.Paused = false; scheduler.Update(0.5); Assert.Equal(0.5f, material.Roughness);
            tree.Cancel(); bound.Cancel(); always.Cancel();
        }
        finally { godot.Tree.Paused = false; owner.Free(); }
    }

    [Theory]
    [InlineData(false)] [InlineData(true)]
    public async Task DisposedResourceSettlesEvenWhenPausedAndCanSuppressCallbacks(bool suppress)
    {
        using var scheduler = new TweenScheduler();
        var material = new StandardMaterial3D(); var calls = 0;
        var tween = scheduler.Add(material, new MaterialRoughnessTween { Duration = 10,
            SuppressCallbacksWhenTargetInvalid = suppress, OnFinally = _ => calls++ });
        tween.Pause(); material.Dispose(); scheduler.Update(0);
        Assert.Equal(Reason.TargetFreed, await tween.Completion);
        Assert.Equal(suppress ? 0 : 1, calls);
        Assert.Equal(0, scheduler.ActiveCount);
        Assert.Throws<ArgumentException>(() => scheduler.Add(material, new MaterialRoughnessTween()));
    }

    [Theory]
    [InlineData(0)] [InlineData(1)] [InlineData(2)] [InlineData(3)]
    public async Task ResourceDisposedFromCallbacksCannotBeWrittenAgain(int phase)
    {
        using var scheduler = new TweenScheduler();
        var material = new StandardMaterial3D();
        var definition = new MaterialRoughnessTween { Duration = 1, To = 0, Fill = FillMode.None };
        if (phase == 0) definition.OnAdd = _ => material.Dispose();
        if (phase == 1) definition.OnStart = _ => material.Dispose();
        if (phase == 2) definition.EaseFunction = t => { material.Dispose(); return t; };
        if (phase == 3) definition.OnUpdate = (_, _) => material.Dispose();
        var tween = scheduler.Add(material, definition); scheduler.Update(1);
        Assert.Equal(Reason.TargetFreed, await tween.Completion);
        Assert.Null(tween.Error);
    }

    [Fact]
    public async Task ManualOwnerBindingAndAutomaticRunnerTeardownReleaseOnlyPlayback()
    {
        using var material = new StandardMaterial3D();
        using var scheduler = new TweenScheduler();
        var owner = Attach(new Node());
        var manual = scheduler.Add(material, new MaterialRoughnessTween { Duration = 10 }, owner);
        owner.Free(); Assert.Equal(Reason.OwnerExited, await manual.Completion);
        var automatic = material.TweenRoughness(0, 10, godot.Tree);
        godot.Engine.Iteration();
        TweenRuntime.GetRunner(godot.Tree).Free();
        Assert.Equal(Reason.RunnerDisposed, await automatic.Completion);
        Assert.True(GodotObject.IsInstanceValid(material));
        var fresh = material.TweenRoughness(0, 0, godot.Tree);
        TweenRuntime.GetRunner(godot.Tree).Scheduler.Update(0);
        Assert.Equal(TweenState.Completed, fresh.State);
    }

    [Fact]
    public void MaterialComponentsPreserveConcurrentEditsAndDefinitionsAreReusable()
    {
        using var material = new StandardMaterial3D { AlbedoColor = Colors.White, Uv1Offset = Vector3.Zero };
        using var duplicate = (StandardMaterial3D)material.Duplicate();
        using var scheduler = new TweenScheduler();
        var definition = new MaterialAlbedoAlphaTween { To = 0, Duration = 1 };
        scheduler.Add(material, definition); scheduler.Add(duplicate, definition);
        definition.To = 1;
        scheduler.Add(material, new MaterialUv1OffsetXTween { To = 2, Duration = 1 });
        scheduler.Add(material, new MaterialUv1OffsetYTween { To = 4, Duration = 1 });
        material.AlbedoColor = Colors.Red; material.Uv1Offset = new Vector3(0, 0, 7);
        scheduler.Update(0.5);
        Assert.Equal(new Color(1, 0, 0, 0.5f), material.AlbedoColor);
        Assert.Equal(new Color(1, 1, 1, 0.5f), duplicate.AlbedoColor);
        Assert.Equal(new Vector3(1, 2, 7), material.Uv1Offset);
        Assert.Equal(BaseMaterial3D.TransparencyEnum.Disabled, material.Transparency);
        Assert.False(material.EmissionEnabled);
    }

    [Fact]
    public void InvalidOwnerIsRejectedBeforeMaterialOrCallbacksAreTouched()
    {
        using var material = new StandardMaterial3D { Roughness = 0.75f };
        var detached = new Node(); var calls = 0;
        try
        {
            Assert.Throws<ArgumentException>(() => material.Tween(new MaterialRoughnessTween { To = 0,
                OnAdd = _ => calls++ }, godot.Tree, detached));
            Assert.Equal(0, calls); Assert.Equal(0.75f, material.Roughness);
        }
        finally { detached.Free(); }
    }

    [Fact]
    public async Task QueuedResourceOwnerCancelsPausedPlaybackWithoutDisposingMaterial()
    {
        using var material = new StandardMaterial3D();
        var owner = Attach(new Node());
        var tween = material.TweenRoughness(0, 10, owner);
        tween.Pause(); owner.QueueFree();
        TweenRuntime.GetRunner(godot.Tree).Scheduler.Update(0);
        Assert.Equal(Reason.OwnerExited, await tween.Completion);
        Assert.True(GodotObject.IsInstanceValid(material));
        godot.Engine.Iteration();
    }

    [Fact]
    public void PreparationCannotReadADisposedTargetAndAlwaysReleasesItsBinding()
    {
        using var scheduler = new TweenScheduler();
        var material = new StandardMaterial3D();
        var released = 0;
        Assert.Throws<ArgumentException>(() => scheduler.Add(material, new DisposingDefinition(() => released++)));
        Assert.Equal(1, released); Assert.Equal(0, scheduler.ActiveCount);
    }
    private sealed class DisposingDefinition(Action release) : TweenDefinition<StandardMaterial3D, float>
    {
        protected override void Prepare(StandardMaterial3D material) => material.Dispose();
        protected override float Read(StandardMaterial3D material) => throw new Exception("Read must not run after disposal.");
        protected override void Write(StandardMaterial3D material, float value) => throw new Exception("Write must not run after disposal.");
        protected override float Interpolate(float from, float to, float weight) => to;
        protected override void Release() => release();
    }
}

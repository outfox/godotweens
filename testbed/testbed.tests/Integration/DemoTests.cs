// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using Godot;
using tweens.gd;
using twodog.Testing;
using twodog.Testing.Xunit;
namespace testbed.Tests;


[Collection<HeadlessCollection>]
public class DemoTests(HeadlessFixture godot)
{
    private void Pump() { for (var i = 0; i < 3; i++) godot.Engine.Iteration(); }

    [Theory]
    [InlineData(nameof(BouncingBall))] [InlineData(nameof(SlimeHop))] [InlineData(nameof(JellyCube))]
    [InlineData(nameof(JellyButton))] [InlineData(nameof(AsyncDelivery))]
    public void AnimationPartialsRemainTrackedAcrossPhasesAndCallbackCreatedTweens(string name)
    {
        // Sample launch, flight, impact, recovery, and subsequent repeats; include the button's OnEnd tween.
        for (var updates = 0; updates <= 100; updates += 4)
        {
            var stage = new Control { Size = new Vector2(600, 300) };
            godot.Tree.Root.AddChild(stage);
            var type = typeof(GalleryEffect).Assembly.GetType("testbed." + name)!;
            var effect = (GalleryEffect)Activator.CreateInstance(type)!;
            effect.Attach(stage);
            var scheduler = TweenRuntime.GetRunner(stage).Scheduler;
            try
            {
                effect.Start(1.8, EaseType.CubicInOut, true);
                var sequence = effect.Sequence!;
                for (var i = 0; i < updates; i++) scheduler.Update(0.05);
                Assert.Null(effect.Error);
                effect.Pause(true);
                Assert.True(effect.AllPaused);
                var active = effect.ActiveCount;
                scheduler.Update(5);
                Assert.Equal(active, effect.ActiveCount);

                effect.Stop();
                Assert.True(sequence.IsCompletedSuccessfully);
                scheduler.Update(5);
                Assert.Equal(0, scheduler.ActiveCount);
                Assert.Equal(0, effect.ActiveCount);

                effect.Start(1.8, EaseType.CubicInOut, true);
                Assert.True(effect.ActiveCount > 0);
                Assert.NotSame(sequence, effect.Sequence);
                effect.Stop();
                Assert.True(effect.Sequence!.IsCompletedSuccessfully);
                scheduler.Update(5);
                Assert.Equal(0, scheduler.ActiveCount);
            }
            finally { effect.Stop(); stage.Free(); effect.ReleaseResources(); }
        }
        Pump();
        Assert.Empty(godot.Errors.Drain());
    }

    [Fact]
    public void PathTweenGroupPreservesTrailingTimingPauseAndCancellation()
    {
        var stage = new Control { Size = new Vector2(600, 300) };
        godot.Tree.Root.AddChild(stage);
        var effect = new CurveFollower2D();
        effect.Attach(stage);
        var view = stage.GetChild<SubViewportContainer>(0).GetChild<SubViewport>(0);
        var followers = view.GetChildren().OfType<Path2D>().Single().GetChildren().OfType<PathFollow2D>().ToArray();
        var leader = followers[^1];
        var scheduler = TweenRuntime.GetRunner(stage).Scheduler;
        try
        {
            effect.Start(1, EaseType.Linear, true);
            Assert.Equal(9, effect.ActiveCount);
            scheduler.Update(0.17);
            Assert.True(leader.ProgressRatio > followers[0].ProgressRatio);
            Assert.True(followers[0].ProgressRatio > followers[1].ProgressRatio);
            Assert.True(followers[1].ProgressRatio > 0);
            Assert.Equal(0, followers[2].ProgressRatio);

            var progress = followers.Select(f => f.ProgressRatio).ToArray();
            effect.Pause(true);
            Assert.True(effect.AllPaused);
            scheduler.Update(5);
            Assert.Equal(progress, followers.Select(f => f.ProgressRatio).ToArray());
            effect.Pause(false);
            scheduler.Update(0.33);
            Assert.InRange(leader.ProgressRatio, 0.499f, 0.501f);
            scheduler.Update(0.5);
            Assert.InRange(leader.ProgressRatio, 0.999f, 1);
            scheduler.Update(0.65);
            Assert.InRange(leader.ProgressRatio, 0.499f, 0.501f);
            Assert.Null(effect.Error);

            effect.Stop();
            Assert.Equal(0, effect.ActiveCount);
            scheduler.Update(5);
            Assert.Equal(0, scheduler.ActiveCount);
            Assert.InRange(leader.ProgressRatio, 0.499f, 0.501f);
        }
        finally { effect.Stop(); stage.Free(); effect.ReleaseResources(); }
        Pump();
        Assert.Empty(godot.Errors.Drain());
    }

    [Theory]
    [InlineData(0)] [InlineData(12)] [InlineData(24)] [InlineData(36)] [InlineData(48)]
    [InlineData(60)] [InlineData(72)] [InlineData(84)] [InlineData(96)]
    public void CardCompletionGroupsPauseCancelAndRestartThroughoutTheSequence(int updates)
    {
        var stage = new Control { Size = new Vector2(600, 300) };
        godot.Tree.Root.AddChild(stage);
        var cards = new CardDeal();
        cards.Attach(stage);
        var scheduler = TweenRuntime.GetRunner(stage).Scheduler;
        try
        {
            cards.Start(1.8, EaseType.CubicInOut, true);
            Assert.Equal(10, cards.ActiveCount);
            var firstRun = cards.Sequence!;
            for (var i = 0; i < updates; i++) scheduler.Update(0.05);
            Assert.False(firstRun.IsCompleted);
            Assert.True(cards.ActiveCount > 0);
            Assert.Null(cards.Error);

            cards.Pause(true);
            Assert.True(cards.AllPaused);
            var active = cards.ActiveCount;
            scheduler.Update(5);
            Assert.Equal(active, cards.ActiveCount);
            Assert.False(firstRun.IsCompleted);
            cards.Pause(false);

            cards.Stop();
            Assert.True(firstRun.IsCompletedSuccessfully);
            Assert.Equal(0, cards.ActiveCount);
            scheduler.Update(5);
            Assert.Equal(0, scheduler.ActiveCount);

            cards.Start(1.8, EaseType.CubicInOut, true);
            Assert.Equal(10, cards.ActiveCount);
            Assert.NotSame(firstRun, cards.Sequence);
            cards.Stop();
            Assert.True(cards.Sequence!.IsCompletedSuccessfully);
            scheduler.Update(5);
            Assert.Equal(0, scheduler.ActiveCount);
        }
        finally { cards.Stop(); stage.Free(); cards.ReleaseResources(); }
        Pump();
        Assert.Empty(godot.Errors.Drain());
    }

    [Fact]
    public void GalleryNavigationPauseCancelAndRestartDoNotLeakPlayback()
    {
        var scene = GD.Load<PackedScene>("res://main.tscn");
        var demo = scene.Instantiate<testbed.TweenDemo>();
        godot.Tree.Root.AddChild(demo);
        try
        {
            Pump(); Assert.True(demo.IsPlaying); Assert.True(demo.DemoTweenCount >= 9);
            var sequence = demo.ChainTask!;
            demo.TogglePause(); Assert.True(demo.CurrentPage!.AllPaused);
            demo.StopDemo(); Assert.False(demo.IsPlaying); Assert.Equal(0, demo.DemoTweenCount);
            Assert.True(sequence.IsCompletedSuccessfully);
            // Repeat navigation exercises both owner-bound material cleanup and deferred page starts.
            for (var round = 0; round < 2; round++)
            for (var index = 0; index < testbed.TweenDemo.PageNames.Length; index++)
            {
                var old = demo.CurrentPage!;
                demo.SelectPage(index); Pump();
                Assert.False(GodotObject.IsInstanceValid(old)); Assert.Equal(0, old.ActiveCount);
                Assert.Equal(index, demo.SelectedPage); Assert.Null(demo.CurrentPage!.Error);
                if (demo.CurrentPage is not ShadersPage) Assert.True(demo.DemoTweenCount >= 7);
                demo.TogglePause(); Assert.True(demo.CurrentPage.AllPaused);
                demo.TogglePause();
                demo.RestartDemo(); Pump(); Assert.Null(demo.CurrentPage!.Error);
                demo.StopDemo(); Assert.Equal(0, demo.DemoTweenCount);
            }
            demo.SelectPage(1); demo.SelectPage(4); demo.SelectPage(0); Pump();
            Assert.Equal(0, demo.SelectedPage); Assert.True(demo.DemoTweenCount >= 9);
            Assert.Throws<ArgumentOutOfRangeException>(() => demo.SelectPage(99));
        }
        finally { demo.Free(); }
    }
    [Fact]
    public void FreeingBeforeThePageStartsDropsTheDeferredStart()
    {
        var demo = new testbed.TweenDemo(); godot.Tree.Root.AddChild(demo);
        demo.Free();
        Pump();
        Assert.Empty(godot.Errors.Drain());
    }
    [Fact]
    public void LeavingDuringAnAwaitedSequenceSettlesItAndDisposesThePage()
    {
        var demo = new testbed.TweenDemo(); godot.Tree.Root.AddChild(demo); Pump();
        var sequence = demo.ChainTask!; var page = demo.CurrentPage!;
        demo.Free();
        Assert.True(sequence.IsCompletedSuccessfully);
        Assert.False(GodotObject.IsInstanceValid(page)); Assert.Equal(0, page.ActiveCount);
    }
}

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

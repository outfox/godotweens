// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using Godot;
using GodotWeens;
using twodog.Testing;
using twodog.Testing.Xunit;

namespace testbed.Tests;

[Collection<HeadlessCollection>]
public class DemoTests(HeadlessFixture godot)
{
    [Fact]
    public void DemoCanPauseCancelAndRestartWithoutLeakingTweens()
    {
        var scene = GD.Load<PackedScene>("res://main.tscn");
        var demo = scene.Instantiate<testbed.TweenDemo>();
        godot.Tree.Root.AddChild(demo);
        try
        {
            for (var i = 0; i < 3; i++) godot.Engine.Iteration();
            Assert.True(demo.IsPlaying);
            Assert.Equal(5, demo.DemoTweenCount);
            demo.TogglePause();
            demo.StopDemo();
            Assert.False(demo.IsPlaying);
            Assert.Equal(0, demo.DemoTweenCount);
            Assert.True(demo.ChainTask!.IsCompletedSuccessfully);
            demo.RestartDemo();
            Assert.Equal(5, demo.DemoTweenCount);
            demo.RestartDemo();
            Assert.Equal(5, demo.DemoTweenCount);
            demo.StopDemo();
            Assert.Equal(0, demo.DemoTweenCount);
        }
        finally { demo.Free(); }
    }
}

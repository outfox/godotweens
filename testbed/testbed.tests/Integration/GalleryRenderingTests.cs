// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using Godot;
using godotweens;
using twodog.Testing;
using twodog.Testing.Xunit;
namespace testbed.Tests;

[Collection<RenderingCollection>]
[Trait("Category", "Rendering")]
public class GalleryRenderingTests(Fixture godot)
{
    [Theory]
    [InlineData(0)] [InlineData(1)] [InlineData(2)] [InlineData(3)] [InlineData(4)] [InlineData(5)]
    public void EachPagePlaysPausesAndReleasesItsNativeScene(int index)
    {
        var demo = new testbed.TweenDemo(); godot.Tree.Root.AddChild(demo);
        try
        {
            demo.SelectPage(index);
            for (var i = 0; i < 4; i++) godot.Engine.Iteration();
            Assert.True(demo.IsPlaying); Assert.True(demo.DemoTweenCount >= 6);
            var page = demo.CurrentPage!;
            var scheduler = TweenRuntime.GetRunner(demo).Scheduler;
            scheduler.Update(0.7); Assert.Null(page.Error);
            demo.TogglePause(); Assert.True(page.AllPaused);
            var particles = Descendants(page).OfType<CpuParticles2D>().FirstOrDefault();
            if (particles is not null) Assert.Equal(0, particles.SpeedScale);
            scheduler.Update(0.5); Assert.Null(page.Error);
            demo.TogglePause();
            if (particles is not null) Assert.True(particles.SpeedScale > 0);
            scheduler.Update(0.7); Assert.Null(page.Error);
            Assert.True(Descendants(page).OfType<SubViewport>().All(v => v.Size.X > 0 && v.Size.Y > 0));
            demo.RestartDemo();
            Assert.Equal(0, page.ActiveCount); Assert.False(GodotObject.IsInstanceValid(page));
            for (var i = 0; i < 3; i++) godot.Engine.Iteration();
            Assert.True(demo.DemoTweenCount >= 6); Assert.Null(demo.CurrentPage!.Error);
            demo.StopDemo(); Assert.Equal(0, demo.DemoTweenCount);
        }
        finally { demo.Free(); }
    }
    private static IEnumerable<Node> Descendants(Node node)
    {
        foreach (var child in node.GetChildren()) { yield return child; foreach (var nested in Descendants(child)) yield return nested; }
    }
}

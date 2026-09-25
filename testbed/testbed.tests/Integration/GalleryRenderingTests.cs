// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using Godot;
using tweens.gd;
using twodog.Testing;
using twodog.Testing.Xunit;
namespace testbed.Tests;

[Collection<RenderingCollection>]
[Trait("Category", "Rendering")]
public class GalleryRenderingTests(Fixture godot)
{
    [Fact]
    public void SourceLayoutUsesExtraSpaceAndCanShrinkBackWithoutClipping()
    {
        var window = godot.Tree.Root;
        var originalSize = window.Size;
        var demo = new TweenDemo();
        godot.Tree.Root.AddChild(demo);
        demo.SetAnchorsAndOffsetsPreset(Control.LayoutPreset.FullRect);
        try
        {
            demo.CurrentPage!.ShowSource(0);
            Vector2 startingCodeSize = default;
            foreach (var size in new[] { new Vector2I(1440, 720), new Vector2I(1920, 1080), new Vector2I(1200, 640), new Vector2I(1440, 720) })
            {
                window.Size = size;
                for (var i = 0; i < 6; i++) godot.Engine.Iteration();
                var code = demo.CurrentPage.SourceView.Code;
                var stage = Descendants(demo).OfType<Control>().Single(c => c.Name == "PreviewStage" && c.IsVisibleInTree());
                foreach (var control in new Control[] { code, stage })
                {
                    var rect = control.GetGlobalRect();
                    Assert.True(rect.Position.X >= 0 && rect.Position.Y >= 0);
                    Assert.True(rect.End.X <= size.X + 1 && rect.End.Y <= size.Y + 1);
                }
                Assert.InRange(stage.Size.X / stage.Size.Y, 1.99f, 2.01f);
                if (startingCodeSize == default) startingCodeSize = code.Size;
                if (size.X == 1920)
                {
                    Assert.True(code.Size.X > startingCodeSize.X);
                    Assert.True(code.Size.Y > startingCodeSize.Y);
                }
            }
            demo.CurrentPage.ShowGallery();
            for (var i = 0; i < 3; i++) godot.Engine.Iteration();
            Assert.Equal(4, Descendants(demo).OfType<Control>().Count(c => c.Name == "PreviewStage" && c.IsVisibleInTree()));
        }
        finally { demo.Free(); window.Size = originalSize; }
    }

    [Theory]
    [InlineData(0)] [InlineData(1)] [InlineData(2)] [InlineData(3)] [InlineData(4)] [InlineData(5)] [InlineData(6)] [InlineData(7)]
    public void EachPagePlaysAndReleasesItsNativeScene(int index)
    {
        var demo = new testbed.TweenDemo(); godot.Tree.Root.AddChild(demo);
        try
        {
            demo.SelectPage(index);
            for (var i = 0; i < 4; i++) godot.Engine.Iteration();
            var page = demo.CurrentPage!;
            var animation = Assert.IsAssignableFrom<Task>(page.SequenceTask);
            var scheduler = TweenRuntime.GetRunner(demo).Scheduler;
            scheduler.Update(1.9);
            Assert.False(animation.IsCompleted);
            Assert.True(Descendants(page).OfType<SubViewport>().All(v => v.Size.X > 0 && v.Size.Y > 0));
            var sourceButtons = Descendants(page).OfType<Button>().Where(b => b.Text == "View C#").ToArray();
            for (var effect = 0; effect < sourceButtons.Length; effect++)
            {
                sourceButtons[effect].EmitSignal(BaseButton.SignalName.Pressed);
                godot.Engine.Iteration();
                var view = page.SourceView;
                Assert.Equal(effect, page.SelectedEffect);
                Assert.EndsWith(".Animation.cs", view.Source.Path);
                Assert.Equal(Godot.FileAccess.GetFileAsString("res://" + view.Source.Path).Replace("\r\n", "\n"), view.Code.Text);
                Assert.True(view.Source.TweenLine > 0);
                Assert.True(view.Code.Size.X > 0 && view.Code.Size.Y > 0);
            }
            demo.RestartDemo();
            Assert.True(animation.IsCompletedSuccessfully);
            Assert.False(GodotObject.IsInstanceValid(page));
            for (var i = 0; i < 3; i++) godot.Engine.Iteration();
            Assert.False(Assert.IsAssignableFrom<Task>(demo.CurrentPage!.SequenceTask).IsCompleted);
        }
        finally { demo.Free(); }
        Assert.Empty(godot.Errors.Drain());
    }
    private static IEnumerable<Node> Descendants(Node node)
    {
        foreach (var child in node.GetChildren()) { yield return child; foreach (var nested in Descendants(child)) yield return nested; }
    }
}

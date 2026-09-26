// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using Godot;
using twodog.Testing;
using twodog.Testing.Xunit;
using FileAccess = Godot.FileAccess;

namespace testbed.Tests.Integration;

[Collection<HeadlessCollection>]
public class GallerySourceTests(HeadlessFixture godot)
{
    private void Pump() { for (var i = 0; i < 3; i++) godot.Engine.Iteration(); }

    [Fact]
    public void EveryEffectEmbedsItsAnimationPartialAndCompanionWithoutTrackingInTheAnimation()
    {
        var types = typeof(GalleryEffect).Assembly.GetTypes()
            .Where(t => t.IsSubclassOf(typeof(GalleryEffect)) && !t.IsAbstract && !t.IsNested).ToArray();
        Assert.Equal(32, types.Length);
        foreach (var type in types)
        {
            var effect = (GalleryEffect)Activator.CreateInstance(type)!;
            var animation = GallerySource.ForEffect(effect);
            var setup = GallerySource.ForSetup(effect);
            Assert.EndsWith(type.Name + ".Animation.cs", animation.Path);
            Assert.EndsWith(type.Name + ".cs", setup.Path);
            Assert.Contains("public sealed partial class " + type.Name, animation.Text);
            Assert.Contains("public sealed partial class " + type.Name, setup.Text);
            Assert.Contains(".Tween", animation.Text);
            Assert.Contains("async Task", animation.Text);
            foreach (var hidden in new[] { "Play(", "Keep(", "TrackTweens(", "Generation", "protected override void Build(", "IEnumerable<TweenInstance>", "yield return" })
                Assert.DoesNotContain(hidden, animation.Text);
            foreach (var source in new[] { animation, setup })
                Assert.Equal(FileAccess.GetFileAsString("res://" + source.Path).Replace("\r\n", "\n"), source.Text);
        }
    }

    [Fact]
    public void EveryCardDisplaysTheExactCompiledSourceWithoutRestartingPlayback()
    {
        var demo = new TweenDemo();
        godot.Tree.Root.AddChild(demo);
        try
        {
            for (var index = 0; index < TweenDemo.PageNames.Length; index++)
            {
                demo.SelectPage(index); Pump();
                var page = demo.CurrentPage!;
                var buttons = Descendants(page).OfType<Button>().Where(b => b.Text == "View C#").ToArray();
                Assert.NotEmpty(buttons);
                var sequence = page.SequenceTask;
                for (var effect = 0; effect < buttons.Length; effect++)
                {
                    buttons[effect].EmitSignal(BaseButton.SignalName.Pressed);
                    var view = page.SourceView;
                    Assert.Equal(effect, page.SelectedEffect);
                    Assert.True(view.Visible);
                    Assert.False(view.Code.Editable);
                    Assert.True(view.Code.GuttersDrawLineNumbers);
                    Assert.IsType<CodeHighlighter>(view.Code.SyntaxHighlighter);
                    Assert.Equal(FileAccess.GetFileAsString("res://" + view.Source.Path).Replace("\r\n", "\n"), view.Code.Text);
                    Assert.True(view.Source.TweenLine > 0);
                    Assert.Equal(view.Source.TweenLine, view.Code.GetCaretLine());
                    Assert.Same(sequence, page.SequenceTask);
                }
                page.ShowGallery();
                Assert.Equal(-1, page.SelectedEffect);
                Assert.False(page.SourceView.Visible);
                Assert.All(buttons, button => Assert.True(button.IsVisibleInTree()));
            }
        }
        finally { demo.Free(); }
        Pump();
        Assert.Empty(godot.Errors.Drain());
    }

    [Fact]
    public void HelpersAndSourceSelectionSurviveRestartAndDisposeWithThePage()
    {
        var demo = new TweenDemo();
        godot.Tree.Root.AddChild(demo); Pump();
        try
        {
            demo.CurrentPage!.ShowSource(1);
            var view = demo.CurrentPage.SourceView;
            var files = Descendants(view).OfType<OptionButton>().Single();
            Assert.Equal("Animation", files.GetItemText(0));
            Assert.Equal("Scene & playback", files.GetItemText(1));
            var animationPath = view.Source.Path;
            for (var i = 1; i < files.ItemCount; i++)
            {
                files.Select(i);
                files.EmitSignal(OptionButton.SignalName.ItemSelected, i);
                Assert.Equal(FileAccess.GetFileAsString("res://" + view.Source.Path).Replace("\r\n", "\n"), view.Code.Text);
                Assert.Equal(Math.Max(0, view.Source.TweenLine), view.Code.GetCaretLine());
                if (i == 1) Assert.Equal(animationPath.Replace(".Animation.cs", ".cs"), view.Source.Path);
            }
            demo.RestartDemo(); Pump();
            Assert.False(GodotObject.IsInstanceValid(view));
            Assert.Equal(1, demo.CurrentPage!.SelectedEffect);
            Assert.True(demo.CurrentPage.SourceView.Visible);
            demo.SelectPage(2); Pump();
            Assert.Equal(-1, demo.CurrentPage!.SelectedEffect);
        }
        finally { demo.Free(); }
        Pump();
        Assert.Empty(godot.Errors.Drain());
    }

    private static IEnumerable<Node> Descendants(Node node)
    {
        foreach (var child in node.GetChildren())
        {
            yield return child;
            foreach (var nested in Descendants(child)) yield return nested;
        }
    }
}

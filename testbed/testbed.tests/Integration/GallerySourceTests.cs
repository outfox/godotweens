// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using Godot;
using FileAccess = Godot.FileAccess;
using twodog.Testing;
using twodog.Testing.Xunit;
namespace testbed.Tests;

[Collection<HeadlessCollection>]
public class GallerySourceTests(HeadlessFixture godot)
{
    private void Pump() { for (var i = 0; i < 3; i++) godot.Engine.Iteration(); }

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
                var count = page.ActiveCount;
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
                    Assert.Equal(count, page.ActiveCount);
                    Assert.Null(page.Error);
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
    public void HelpersAndSourceSelectionSurvivePlaybackControlsAndDisposeWithThePage()
    {
        var demo = new TweenDemo();
        godot.Tree.Root.AddChild(demo); Pump();
        try
        {
            demo.CurrentPage!.ShowSource(1);
            var view = demo.CurrentPage.SourceView;
            var files = Descendants(view).OfType<OptionButton>().Single();
            for (var i = 1; i < files.ItemCount; i++)
            {
                files.Select(i);
                files.EmitSignal(OptionButton.SignalName.ItemSelected, i);
                Assert.Equal(FileAccess.GetFileAsString("res://" + view.Source.Path).Replace("\r\n", "\n"), view.Code.Text);
                Assert.Equal(0, view.Code.GetCaretLine());
            }
            demo.TogglePause(); Assert.True(demo.CurrentPage.AllPaused);
            demo.RestartDemo(); Pump();
            Assert.False(GodotObject.IsInstanceValid(view));
            Assert.Equal(1, demo.CurrentPage!.SelectedEffect);
            Assert.True(demo.CurrentPage.SourceView.Visible);
            Assert.True(demo.IsPlaying);
            demo.StopDemo(); Assert.Equal(0, demo.DemoTweenCount);
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

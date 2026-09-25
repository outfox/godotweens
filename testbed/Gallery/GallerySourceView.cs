// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using System.Collections.Generic;
using Godot;
namespace testbed;

/// <summary>Read-only, selectable C# source beside the running example.</summary>
public partial class GallerySourceView : VBoxContainer
{
    private readonly List<Resource> resources = [];
    private GallerySource example = null!;
    private OptionButton files = null!;
    private Label path = null!;
    private Button entry = null!, copy = null!;
    public CodeEdit Code { get; private set; } = null!;
    public GallerySource Source { get; private set; } = null!;

    public override void _Ready()
    {
        CustomMinimumSize = new Vector2(420, 420);
        SizeFlagsHorizontal = SizeFlags.ExpandFill;
        SizeFlagsVertical = SizeFlags.ExpandFill;
        SizeFlagsStretchRatio = 1.6f;
        AddThemeConstantOverride("separation", 10);

        var toolbar = this.Add(new HBoxContainer());
        files = toolbar.Add(new OptionButton { SizeFlagsHorizontal = SizeFlags.ExpandFill });
        foreach (var name in new[] { "Example", "Playback helpers", "Scene helpers", "Palette", "Node helpers", "Shader panels" }) files.AddItem(name);
        files.ItemSelected += index => ShowFile((int)index);
        copy = toolbar.Add(new Button { Text = "Copy file", TooltipText = "Copy the complete source file" });
        copy.Pressed += () => { DisplayServer.ClipboardSet(Code.Text); copy.Text = "Copied!"; };

        path = this.Add(GalleryTheme.Label("", 12, Palette.Muted));
        path.TextOverrunBehavior = TextServer.OverrunBehavior.TrimEllipsis;
        Code = this.Add(new CodeEdit
        {
            Name = "SourceCode", Editable = false, GuttersDrawLineNumbers = true,
            GuttersLineNumbersMinDigits = 2, HighlightCurrentLine = true,
            SizeFlagsHorizontal = SizeFlags.ExpandFill, SizeFlagsVertical = SizeFlags.ExpandFill,
            ScrollPastEndOfFile = true, ContextMenuEnabled = true,
            WrapMode = TextEdit.LineWrappingMode.Boundary,
        });
        var font = Own(GD.Load<FontFile>("res://Fonts/JetBrainsMono-Regular.ttf"));
        Code.AddThemeFontOverride("font", font);
        Code.AddThemeFontSizeOverride("font_size", 13);
        Code.AddThemeColorOverride("font_color", Palette.Soft);
        Code.AddThemeColorOverride("font_readonly_color", Palette.Soft);
        Code.AddThemeColorOverride("line_number_color", Palette.Muted);
        Code.AddThemeColorOverride("background_color", Palette.Stage);
        Code.AddThemeColorOverride("current_line_color", Palette.Raised);
        Code.AddThemeColorOverride("selection_color", new Color("355276"));
        var box = Own(GalleryTheme.Box(Palette.Stage, 10, 1, null, 10, 12));
        Code.AddThemeStyleboxOverride("normal", box);
        Code.AddThemeStyleboxOverride("read_only", box);
        var focus = Own(GalleryTheme.Box(Colors.Transparent, 10, 1, Palette.Mint));
        focus.DrawCenter = false;
        Code.AddThemeStyleboxOverride("focus", focus);
        Code.SyntaxHighlighter = Own(CreateHighlighter());

        var actions = this.Add(new HBoxContainer());
        entry = actions.Add(new Button { Text = "Tween entry", TooltipText = "Jump to Animate(), where this example starts its tweens" });
        entry.Pressed += JumpToTween;
        var top = actions.Add(new Button { Text = "File start" });
        top.Pressed += () => Jump(0);
        var wrap = actions.Add(new CheckBox { Text = "Wrap", ButtonPressed = true });
        wrap.Toggled += enabled => Code.WrapMode = enabled ? TextEdit.LineWrappingMode.Boundary : TextEdit.LineWrappingMode.None;
        this.Add(GalleryTheme.Label("Full source from this build · select text to copy", 12, Palette.Muted));
    }

    public void ShowEffect(GalleryEffect effect)
    {
        example = GallerySource.ForEffect(effect);
        files.Select(0);
        ShowFile(0);
    }

    private void ShowFile(int index)
    {
        Source = index switch
        {
            1 => GallerySource.Load("GalleryEffect.cs"),
            2 => GallerySource.Load("GalleryEffect.Stage.cs"),
            3 => GallerySource.Load("Palette.cs"),
            4 => GallerySource.Load("NodeExtensions.cs"),
            5 => GallerySource.Load("SplitPanel.cs"),
            _ => example,
        };
        path.Text = Source.Path;
        path.TooltipText = Source.Path;
        Code.Text = Source.Text;
        Code.Deselect();
        entry.Disabled = Source.TweenLine < 0;
        copy.Text = "Copy file";
        Jump(Source.TweenLine < 0 ? 0 : Source.TweenLine);
    }

    private void JumpToTween() => Jump(Source.TweenLine < 0 ? 0 : Source.TweenLine);

    private void Jump(int line)
    {
        Code.SetCaretLine(line);
        Code.SetCaretColumn(0);
        Code.SetLineAsFirstVisible(line);
        Code.ScrollHorizontal = 0;
    }

    private static CodeHighlighter CreateHighlighter()
    {
        var highlighter = new CodeHighlighter
        {
            NumberColor = Palette.Mint, SymbolColor = Palette.Muted,
            FunctionColor = Palette.Blue, MemberVariableColor = Palette.Soft,
        };
        foreach (var word in ("using namespace public private protected internal sealed abstract partial static readonly const " +
            "override virtual async await return if else for foreach while in is not null true false new var void bool byte " +
            "int long float double string object out ref params get set switch case default try catch throw typeof with").Split(' '))
            highlighter.AddKeywordColor(word, new Color("d5a6ef"));
        foreach (var word in ("Vector2 Vector3 Vector4 Vector2I Color Colors Math MathF Mathf Task TweenOptions TweenInstance " +
            "EaseType TweenState TweenCompletionReason Node Node2D Node3D Control Stage Palette GalleryEffect " +
            "Polygon2D Line2D ShaderMaterial StandardMaterial3D Camera2D Camera3D").Split(' '))
            highlighter.AddKeywordColor(word, Palette.Mint);
        highlighter.AddColorRegion("//", "", Palette.Muted, true);
        highlighter.AddColorRegion("/*", "*/", Palette.Muted);
        highlighter.AddColorRegion("\"\"\"", "\"\"\"", Palette.Amber);
        highlighter.AddColorRegion("\"", "\"", Palette.Amber);
        highlighter.AddColorRegion("'", "'", Palette.Amber);
        return highlighter;
    }

    private T Own<T>(T resource) where T : Resource { resources.Add(resource); return resource; }

    /// <summary>Called after the page's controls have been freed.</summary>
    public void ReleaseResources()
    {
        foreach (var resource in resources) resource.Dispose();
        resources.Clear();
    }
}

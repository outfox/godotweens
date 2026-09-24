// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using System;
using System.Collections.Generic;
using Godot;
namespace testbed;

/// <summary>Palette and shared control theme for the gallery shell and its pages.</summary>
public static class GalleryTheme
{
    public static readonly Color Background = new("0e1620"), Surface = new("1a2635"), Stage = new("111b27"),
        Raised = new("22334a"), RaisedHover = new("2b405c"), Outline = new("2d4057"), Text = new("eef4fa"),
        Soft = new("c9d6e2"), Disabled = new("5f7185"), Selected = new("1d3a3c");

    public static StyleBoxFlat Box(Color color, int radius = 12, int border = 0, Color? outline = null, int x = 16, int y = 14)
    {
        var box = new StyleBoxFlat { BgColor = color, BorderColor = outline ?? Outline,
            ContentMarginLeft = x, ContentMarginRight = x, ContentMarginTop = y, ContentMarginBottom = y,
            AntiAliasing = true };
        box.SetCornerRadiusAll(radius); box.SetBorderWidthAll(border); return box;
    }

    /// <summary>Builds the root theme. Every created resource is added to <paramref name="owned"/> for disposal.</summary>
    public static Theme Build(List<Resource> owned)
    {
        T Own<T>(T resource) where T : Resource { owned.Add(resource); return resource; }
        StyleBoxFlat Flat(Color color, int radius = 8, int border = 0, Color? outline = null, int x = 14, int y = 8)
            => Own(Box(color, radius, border, outline, x, y));
        var theme = Own(new Theme());
        var empty = Own(new StyleBoxEmpty());
        var focus = Flat(Colors.Transparent, 8, 2, GalleryPage.Mint with { A = 0.7f }); focus.DrawCenter = false;

        theme.SetStylebox("normal", "Button", Flat(Raised, 8, 1));
        theme.SetStylebox("hover", "Button", Flat(RaisedHover, 8, 1, new Color("3d5775")));
        theme.SetStylebox("pressed", "Button", Flat(Selected, 8, 1, GalleryPage.Mint));
        theme.SetStylebox("hover_pressed", "Button", Flat(Selected, 8, 1, GalleryPage.Mint));
        theme.SetStylebox("disabled", "Button", Flat(new Color("172230"), 8, 1, new Color("1f2d3d")));
        theme.SetStylebox("focus", "Button", focus);
        foreach (var (name, color) in new[] { ("font_color", Soft), ("font_hover_color", Text), ("font_focus_color", Soft),
            ("font_pressed_color", GalleryPage.Mint), ("font_hover_pressed_color", GalleryPage.Mint), ("font_disabled_color", Disabled) })
        {
            theme.SetColor(name, "Button", color); theme.SetColor(name, "CheckBox", color);
        }
        theme.SetColor("font_pressed_color", "CheckBox", Text); theme.SetColor("font_hover_pressed_color", "CheckBox", Text);
        // Theme lookup walks the class chain per theme, so CheckBox would otherwise inherit the Button boxes.
        foreach (var state in new[] { "normal", "hover", "pressed", "hover_pressed", "disabled" })
            theme.SetStylebox(state, "CheckBox", empty);
        theme.SetStylebox("focus", "CheckBox", focus);
        theme.SetConstant("h_separation", "CheckBox", 8);
        theme.SetIcon("checked", "CheckBox", Own(CheckIcon(true)));
        theme.SetIcon("unchecked", "CheckBox", Own(CheckIcon(false)));

        theme.SetStylebox("panel", "PopupMenu", Flat(Surface, 8, 1, Outline, 6, 6));
        theme.SetStylebox("hover", "PopupMenu", Flat(RaisedHover, 6));
        theme.SetColor("font_color", "PopupMenu", Soft); theme.SetColor("font_hover_color", "PopupMenu", Text);
        theme.SetColor("font_accelerator_color", "PopupMenu", Disabled);

        theme.SetStylebox("slider", "HSlider", Flat(new Color("2a3b51"), 3, 0, null, 0, 3));
        theme.SetStylebox("grabber_area", "HSlider", Flat(GalleryPage.Mint, 3, 0, null, 0, 3));
        theme.SetStylebox("grabber_area_highlight", "HSlider", Flat(new Color("9ae9c8"), 3, 0, null, 0, 3));
        theme.SetIcon("grabber", "HSlider", Own(Dot(Text)));
        theme.SetIcon("grabber_highlight", "HSlider", Own(Dot(Colors.White)));

        foreach (var bar in new[] { "VScrollBar", "HScrollBar" })
        {
            theme.SetStylebox("scroll", bar, Flat(Stage, 4, 0, null, 4, 4));
            theme.SetStylebox("grabber", bar, Flat(new Color("3f5672"), 4, 0, null, 4, 4));
            theme.SetStylebox("grabber_highlight", bar, Flat(new Color("56718f"), 4, 0, null, 4, 4));
            theme.SetStylebox("grabber_pressed", bar, Flat(GalleryPage.Mint, 4, 0, null, 4, 4));
        }
        var line = Own(new StyleBoxLine { Color = new Color("243447"), Thickness = 1 });
        theme.SetStylebox("separator", "HSeparator", line); theme.SetConstant("separation", "HSeparator", 1);
        theme.SetColor("font_color", "Label", Text);
        return theme;
    }

    private static ImageTexture Dot(Color color)
    {
        const int size = 20;
        using var image = Image.CreateEmpty(size, size, false, Image.Format.Rgba8);
        for (var y = 0; y < size; y++) for (var x = 0; x < size; x++)
        {
            var d = new Vector2(x + 0.5f - size / 2f, y + 0.5f - size / 2f).Length();
            var ring = Mathf.Clamp(size / 2f - 0.5f - d, 0, 1);
            var core = Mathf.Clamp(size / 2f - 3.5f - d, 0, 1);
            image.SetPixel(x, y, GalleryPage.Mint.Lerp(color, core) with { A = ring });
        }
        return ImageTexture.CreateFromImage(image);
    }

    private static ImageTexture CheckIcon(bool on)
    {
        const int size = 18;
        using var image = Image.CreateEmpty(size, size, false, Image.Format.Rgba8);
        for (var y = 0; y < size; y++) for (var x = 0; x < size; x++)
        {
            var p = new Vector2(x + 0.5f, y + 0.5f);
            // Signed distance to a rounded square, used for both fill and outline coverage.
            var q = (p - new Vector2(size / 2f, size / 2f)).Abs() - new Vector2(size / 2f - 4, size / 2f - 4);
            var box = new Vector2(Math.Max(q.X, 0), Math.Max(q.Y, 0)).Length() + Math.Min(Math.Max(q.X, q.Y), 0) - 3.5f;
            var inside = Mathf.Clamp(0.5f - box, 0, 1);
            if (!on)
            {
                var border = inside * Mathf.Clamp(box + 2.5f, 0, 1);
                image.SetPixel(x, y, new Color("7890a8") with { A = border });
                continue;
            }
            var tick = Math.Min(Segment(p, new(4.5f, 9.5f), new(7.5f, 12.5f)), Segment(p, new(7.5f, 12.5f), new(13.5f, 5.5f)));
            var mark = Mathf.Clamp(1.9f - tick, 0, 1);
            image.SetPixel(x, y, GalleryPage.Mint.Lerp(Background, mark) with { A = inside });
        }
        return ImageTexture.CreateFromImage(image);
    }

    private static float Segment(Vector2 p, Vector2 a, Vector2 b)
    {
        var t = Mathf.Clamp((p - a).Dot(b - a) / (b - a).LengthSquared(), 0, 1);
        return p.DistanceTo(a + (b - a) * t);
    }
}

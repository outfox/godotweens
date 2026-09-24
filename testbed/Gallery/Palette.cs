// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using Godot;
namespace testbed;

/// <summary>Colors shared by the gallery shell, its theme and every effect.</summary>
public static class Palette
{
    public static readonly Color Background = new("0e1620");
    public static readonly Color Surface = new("1a2635");
    public static readonly Color Stage = new("111b27");
    public static readonly Color Raised = new("22334a");
    public static readonly Color RaisedHover = new("2b405c");
    public static readonly Color Selected = new("1d3a3c");
    public static readonly Color Outline = new("2d4057");
    public static readonly Color Track = new("4d8190");

    public static readonly Color Text = new("eef4fa");
    public static readonly Color Soft = new("c9d6e2");
    public static readonly Color Muted = new("a9bccd");
    public static readonly Color Disabled = new("5f7185");

    public static readonly Color Mint = new("79deb4");
    public static readonly Color Amber = new("f2bc74");
    public static readonly Color Blue = new("8caaff");
}

// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using Godot;
namespace testbed;

/// <summary>A rounded panel whose fill is split at the <c>amount</c> uniform, marked by a bright seam.</summary>
public static class SplitPanel
{
    public static readonly Vector2 Size = new(165, 100);
    public static readonly Vector2 Left = new(-180, -50), Right = new(15, -50);

    /// <summary>Everything after the <c>amount</c> uniform declaration, which differs between shared and instance uniforms.</summary>
    public const string Body = """
        float rounded(vec2 uv, vec2 size, float r) {
            vec2 q = abs((uv - 0.5) * size) - size * 0.5 + r;
            return 1.0 - smoothstep(-0.75, 0.75, length(max(q, 0.0)) + min(max(q.x, q.y), 0.0) - r);
        }
        void fragment() {
            float edge = smoothstep(amount - 0.03, amount + 0.03, UV.x);
            vec3 fill = mix(vec3(0.47,0.87,0.70), vec3(0.62,0.95,0.80), UV.y * 0.6);
            vec3 c = mix(fill, vec3(0.17,0.26,0.38), edge);
            c += vec3(0.9) * (1.0 - smoothstep(0.0, 0.012, abs(UV.x - amount)));
            COLOR = vec4(c, rounded(UV, vec2(165.0, 100.0), 12.0));
        }
        """;

    public static ColorRect Add(Node parent, ShaderMaterial material, Vector2 position)
    {
        var rect = new ColorRect { Material = material, Position = position, Size = Size, MouseFilter = Control.MouseFilterEnum.Ignore };
        parent.AddChild(rect);
        return rect;
    }

    /// <summary>Labels the panels at <see cref="Left"/> and <see cref="Right"/>.</summary>
    public static void Caption(Node parent, string left, string right)
    {
        Label(parent, left, new Vector2(-165, 60));
        Label(parent, right, new Vector2(40, 60));
    }

    private static void Label(Node parent, string text, Vector2 position)
    {
        var label = GalleryTheme.Label(text, 12, Palette.Soft);
        label.Position = position;
        parent.AddChild(label);
    }
}

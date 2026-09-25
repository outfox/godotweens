// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using Godot;
using tweens.gd;
namespace testbed;

public sealed partial class TypedUniforms : GalleryEffect
{
    private const string DotGrid = """
        shader_type canvas_item;
        uniform vec4 tint : source_color = vec4(0.47, 0.87, 0.70, 1.0);
        uniform vec2 offset = vec2(0.0);
        void fragment() {
            vec2 p = (UV + offset) * vec2(8.0, 3.0);
            float d = length(fract(p) - 0.5);
            float rings = smoothstep(0.25, 0.30, d);
            vec3 dot = tint.rgb * (1.15 - d * 1.2);
            vec3 c = mix(dot, vec3(0.15,0.23,0.34), rings) + tint.rgb * 0.18 * (1.0 - smoothstep(0.3, 0.45, d)) * rings;
            vec2 q = abs((UV - 0.5) * vec2(360.0, 130.0)) - vec2(180.0, 65.0) + 14.0;
            float mask = 1.0 - smoothstep(-0.75, 0.75, length(max(q, 0.0)) + min(max(q.x, q.y), 0.0) - 14.0);
            COLOR = vec4(c, mask);
        }
        """;

    private ShaderMaterial material = null!;

    public override string Title => "Color & Vector2";
    public override string Caption => "Color controls tint; Vector2 controls pattern offset.";

    protected override void Build()
    {
        material = Shader(DotGrid);
        View().Add(new ColorRect
        {
            Material = material, Position = new Vector2(-180, -65), Size = new Vector2(360, 130),
            MouseFilter = Control.MouseFilterEnum.Ignore,
        });
    }

    protected override void Animate() => TrackTweens(CreateAnimation());
}

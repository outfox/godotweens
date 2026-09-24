// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using Godot;
using godotweens;
namespace testbed;

public partial class ShadersPage : GalleryPage
{
    public override string Heading => "Shaders";
    public override string Description => "Shared and per-instance uniforms with typed values.";
    private ShaderMaterial shared = null!, typed = null!;
    private ColorRect first = null!, second = null!;
    private MeshInstance3D deform = null!;
    private bool rendering;
    private ShaderMaterial Shader(string code) => Own(new ShaderMaterial { Shader = Own(new Godot.Shader { Code = code }) });
    private static ColorRect Patch(Node parent, ShaderMaterial material, Vector2 position, Vector2 size)
    {
        var rect = new ColorRect { Material = material, Position = position, Size = size, MouseFilter = MouseFilterEnum.Ignore };
        parent.AddChild(rect); return rect;
    }
    private static void Caption(Node parent, string text, Vector2 position)
    {
        var label = Text(text, 12, Soft); label.Position = position; parent.AddChild(label);
    }
    protected override void Build()
    {
        rendering = DisplayServer.GetName() != "headless";
        if (!rendering)
        {
            Card("Renderer Required", "Run the desktop testbed with rendering enabled.")
                .AddChild(Text("Unavailable in headless mode.", 17, Amber));
            return;
        }
        // Rounded mask for the 165x100 patches, plus a bright seam where the uniform splits the fill.
        const string body = """
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
        var twins = View(Card("01 / Shared Uniform", "One float uniform updates both panels."));
        shared = Shader("shader_type canvas_item; uniform float amount = 0.15; " + body);
        Patch(twins, shared, new Vector2(-180, -50), new Vector2(165, 100));
        Patch(twins, shared, new Vector2(15, -50), new Vector2(165, 100));
        Caption(twins, "SAME MATERIAL", new Vector2(-165, 60)); Caption(twins, "SAME VALUE", new Vector2(40, 60));
        var instances = View(Card("02 / Instance Uniforms", "Independent CanvasItem values on a shared material."));
        var instance = Shader("shader_type canvas_item; instance uniform float amount = 0.15; " + body);
        first = Patch(instances, instance, new Vector2(-180, -50), new Vector2(165, 100));
        second = Patch(instances, instance, new Vector2(15, -50), new Vector2(165, 100));
        second.SetInstanceShaderParameter("amount", 0.85f);
        Caption(instances, "INSTANCE A", new Vector2(-165, 60)); Caption(instances, "INSTANCE B", new Vector2(40, 60));

        var color = View(Card("03 / Color & Vector2", "Color controls tint; Vector2 controls pattern offset."));
        typed = Shader("""
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
            """);
        Patch(color, typed, new Vector2(-180, -65), new Vector2(360, 130));
        var (surface, lens) = World(Card("04 / 3D Instance Uniform", "Left: tweened displacement. Right: default value."));
        var material = Shader("""
            shader_type spatial;
            instance uniform float amplitude = 0.0;
            void vertex() { VERTEX += NORMAL * sin(VERTEX.y * 16.0) * amplitude; }
            void fragment() { ALBEDO = vec3(0.47,0.87,0.70); ROUGHNESS = 0.28; }
            """);
        Floor(surface, -0.75f); lens.Position = new Vector3(0, 1.0f, 3.6f); lens.LookAt(Vector3.Zero);
        deform = Mesh(surface, new SphereMesh { Radius = 0.65f, Height = 1.3f, RadialSegments = 64, Rings = 32 }, material, new Vector3(-1, 0, 0));
        Mesh(surface, new SphereMesh { Radius = 0.65f, Height = 1.3f }, material, new Vector3(1, 0, 0));
    }
    protected override void Animate()
    {
        if (!rendering) return;
        Keep(shared.TweenShaderParameter("amount", 0.85f, Seconds, this, Cycle));
        Keep(first.TweenInstanceShaderParameter("amount", 0.85f, Seconds, Cycle));
        Keep(second.TweenInstanceShaderParameter("amount", 0.15f, Seconds, Cycle));
        Keep(typed.TweenShaderParameter("tint", Amber, Seconds, this, Cycle));
        Keep(typed.TweenShaderParameter("offset", new Vector2(0.25f, 0.33f), Seconds, this, Cycle));
        Keep(deform.TweenInstanceShaderParameter("amplitude", 0.22f, Seconds, Cycle));
    }
}

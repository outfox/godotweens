// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using Godot;
using godotweens;
namespace testbed;

public partial class ShadersPage : GalleryPage
{
    public override string Heading => "Shaders";
    public override string Description => "Compare shared uniforms with per-node overrides, then animate typed vectors, colors, and a 3D surface.";
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
        var label = Text(text, 12, Muted); label.Position = position; parent.AddChild(label);
    }
    protected override void Build()
    {
        rendering = DisplayServer.GetName() != "headless";
        if (!rendering)
        {
            Card("A renderer is required", "Open the desktop testbed to explore shader defaults and per-instance uniforms.")
                .AddChild(Text("Shaders need a graphics context.", 17, Amber));
            return;
        }
        const string body = "void fragment() { float edge = smoothstep(amount - 0.03, amount + 0.03, UV.x); vec3 c = mix(vec3(0.47,0.87,0.70), vec3(0.13,0.21,0.32), edge); COLOR = vec4(c,1.0); }";
        var twins = View(Card("01 / Shared uniform", "One float uniform on a shared ShaderMaterial. Both panels update together."));
        shared = Shader("shader_type canvas_item; uniform float amount = 0.15; " + body);
        Patch(twins, shared, new Vector2(-180, -50), new Vector2(165, 100));
        Patch(twins, shared, new Vector2(15, -50), new Vector2(165, 100));
        Caption(twins, "SAME MATERIAL", new Vector2(-165, 60)); Caption(twins, "SAME VALUE", new Vector2(40, 60));
        var instances = View(Card("02 / Shared material, independent values", "Two instance uniforms move in opposite directions without cloning the material."));
        var instance = Shader("shader_type canvas_item; instance uniform float amount = 0.15; " + body);
        first = Patch(instances, instance, new Vector2(-180, -50), new Vector2(165, 100));
        second = Patch(instances, instance, new Vector2(15, -50), new Vector2(165, 100));
        second.SetInstanceShaderParameter("amount", 0.85f);
        Caption(instances, "INSTANCE A", new Vector2(-165, 60)); Caption(instances, "INSTANCE B", new Vector2(40, 60));

        var color = View(Card("03 / Color + vector uniforms", "A typed Color shifts the palette while Vector2 moves the pattern. No shader clock."));
        typed = Shader("""
            shader_type canvas_item;
            uniform vec4 tint : source_color = vec4(0.47, 0.87, 0.70, 1.0);
            uniform vec2 offset = vec2(0.0);
            void fragment() {
                vec2 p = (UV + offset) * vec2(8.0, 3.0);
                float rings = smoothstep(0.25, 0.30, length(fract(p) - 0.5));
                COLOR = vec4(mix(tint.rgb, vec3(0.10,0.16,0.25), rings), 1.0);
            }
            """);
        Patch(color, typed, new Vector2(-180, -65), new Vector2(360, 130));
        var (surface, _) = World(Card("04 / A per-instance 3D pulse", "Only the left sphere changes its displacement uniform. The right sphere is the reference."));
        var material = Shader("""
            shader_type spatial;
            instance uniform float amplitude = 0.0;
            void vertex() { VERTEX += NORMAL * sin(VERTEX.y * 16.0) * amplitude; }
            void fragment() { ALBEDO = vec3(0.47,0.87,0.70); ROUGHNESS = 0.28; }
            """);
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

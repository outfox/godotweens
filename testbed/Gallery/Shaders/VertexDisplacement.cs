// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using Godot;
using godotweens;
namespace testbed;

public sealed class VertexDisplacement : GalleryEffect
{
    private const string Ripple = """
        shader_type spatial;
        instance uniform float amplitude = 0.0;
        void vertex() { VERTEX += NORMAL * sin(VERTEX.y * 16.0) * amplitude; }
        void fragment() { ALBEDO = vec3(0.47,0.87,0.70); ROUGHNESS = 0.28; }
        """;

    private MeshInstance3D deformed = null!;

    public override string Title => "3D Instance Uniform";
    public override string Caption => "Left: tweened displacement. Right: default value.";

    protected override void Build()
    {
        var scene = World();
        scene.Camera.Position = new Vector3(0, 1.0f, 3.6f);
        scene.Camera.LookAt(Vector3.Zero);
        Floor(scene.View, -0.75f);

        var material = Shader(Ripple);
        var detailed = new SphereMesh { Radius = 0.65f, Height = 1.3f, RadialSegments = 64, Rings = 32 };
        deformed = Mesh(scene.View, detailed, material, new Vector3(-1, 0, 0));
        Mesh(scene.View, new SphereMesh { Radius = 0.65f, Height = 1.3f }, material, new Vector3(1, 0, 0));
    }

    protected override void Animate() => Keep(deformed.TweenInstanceShaderParameter("amplitude", 0.22f, Seconds, Cycle));
}

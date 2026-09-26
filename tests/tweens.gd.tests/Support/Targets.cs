// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using Godot;

namespace tweens.gd.Tests.Support;

/// <summary>Builds live targets for adapter types, set up so every animated property accepts samples.</summary>
public static class Targets
{
    public static Type Concrete(Type type) =>
        type == typeof(CanvasItem) ? typeof(Node2D) :
        type == typeof(Godot.Range) ? typeof(ProgressBar) :
        type == typeof(SpriteBase3D) ? typeof(Sprite3D) :
        type == typeof(GeometryInstance3D) ? typeof(MeshInstance3D) :
        type == typeof(Light2D) ? typeof(PointLight2D) :
        type == typeof(Light3D) ? typeof(OmniLight3D) :
        type == typeof(BaseMaterial3D) ? typeof(StandardMaterial3D) : type;

    /// <summary>Creates a node inside the scope, or a tracked resource.</summary>
    public static GodotObject Create(SceneScope scope, Type type)
    {
        var target = (GodotObject)Activator.CreateInstance(Concrete(type))!;
        if (target is Resource resource) return scope.Track(resource);
        var node = (Node)target;
        if (node is Control control)
        {
            control.Size = new Vector2(100, 200);
            control.OffsetTransformEnabled = true;
        }
        switch (node)
        {
            case Godot.Range range: range.Step = 0; break;
            case Label label: label.Text = new string('a', 100); break;
            case RichTextLabel rich: rich.Text = new string('a', 100); break;
            case Sprite2D sprite: sprite.Hframes = 10; sprite.RegionEnabled = true; break;
            case AnimatedSprite2D animated: animated.SpriteFrames = Frames(scope); break;
            case AnimatedSprite3D animated: animated.SpriteFrames = Frames(scope); break;
            case GpuParticles2D particles: particles.Emitting = false; break;
            case GpuParticles3D particles: particles.Emitting = false; break;
            case CpuParticles2D particles: particles.Emitting = false; break;
            case CpuParticles3D particles: particles.Emitting = false; break;
        }
        switch (node)
        {
            case PathFollow2D follow:
            {
                var curve = scope.Track(new Curve2D());
                curve.AddPoint(Vector2.Zero);
                curve.AddPoint(new Vector2(100, 0));
                scope.Add(new Path2D { Curve = curve }).AddChild(follow);
                follow.Loop = false;
                break;
            }
            case PathFollow3D follow:
            {
                var curve = scope.Track(new Curve3D());
                curve.AddPoint(Vector3.Zero);
                curve.AddPoint(new Vector3(100, 0, 0));
                scope.Add(new Path3D { Curve = curve }).AddChild(follow);
                follow.Loop = false;
                break;
            }
            default:
                scope.Add(node);
                break;
        }
        if (node is ScrollContainer scroll)
        {
            scroll.GetHScrollBar().MaxValue = 1000;
            scroll.GetHScrollBar().Page = 100;
            scroll.GetVScrollBar().MaxValue = 1000;
            scroll.GetVScrollBar().Page = 100;
        }
        return node;
    }

    private static SpriteFrames Frames(SceneScope scope)
    {
        var frames = scope.Track(new SpriteFrames());
        for (var i = 0; i < 10; i++) frames.AddFrame("default", null!);
        return frames;
    }
}

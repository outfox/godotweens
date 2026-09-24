// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using System;
using Godot;
using godotweens;
namespace testbed;

public partial class EffectsPage : GalleryPage
{
    public override string Heading => "Drawing & particles";
    public override string Description => "Line, polygon, particle, and 2D light properties.";
    private Line2D ribbon = null!;
    private Polygon2D polygon = null!;
    private CpuParticles2D particles = null!;
    private PointLight2D light = null!;
    private double particleSpeed = 1;
    protected override void Build()
    {
        var drawing = View(Card("01 / Line2D", "Width and DefaultColor on a fixed path."));
        ribbon = Line(drawing, [new(-170, 35), new(-100, -40), new(-25, 25), new(55, -45), new(160, 30)], Mint, 3);
        ribbon.BeginCapMode = Line2D.LineCapMode.Round; ribbon.EndCapMode = Line2D.LineCapMode.Round;
        ribbon.JointMode = Line2D.LineJointMode.Round;
        var shapes = View(Card("02 / Polygon2D", "Color, offset, and rotation."));
        polygon = new Polygon2D { Polygon = [new(-45, -40), new(35, -50), new(65, 15), new(0, 50), new(-60, 15)], Color = Mint };
        shapes.AddChild(polygon);
        var stream = View(Card("03 / CPUParticles2D", "Spread, gravity, color, and position. Pause stops simulation."));
        particles = new CpuParticles2D { Amount = 120, Lifetime = 2, Preprocess = 1, Direction = Vector2.Right,
            Spread = 8, InitialVelocityMin = 70, InitialVelocityMax = 110, Gravity = Vector2.Zero,
            ScaleAmountMin = 2, ScaleAmountMax = 4, Color = Mint, Position = new Vector2(-100, 25),
            LocalCoords = false };
        stream.AddChild(particles);
        var lit = View(Card("04 / PointLight2D", "TextureScale, energy, and position."));
        var modulate = new CanvasModulate { Color = new Color("263143") }; lit.AddChild(modulate);
        for (var y = -3; y < 3; y++) for (var x = -7; x < 7; x++)
        {
            var cell = new Polygon2D { Polygon = [new(0, 0), new(27, 0), new(27, 27), new(0, 27)],
                Position = new Vector2(x * 32, y * 32), Color = (x + y) % 2 == 0 ? Mint : Blue };
            lit.AddChild(cell);
        }
        using var image = Image.CreateEmpty(128, 128, false, Image.Format.Rgba8);
        for (var y = 0; y < 128; y++) for (var x = 0; x < 128; x++)
        {
            var a = Math.Max(0, 1 - new Vector2(x - 63.5f, y - 63.5f).Length() / 64);
            image.SetPixel(x, y, new Color(a, a, a, 1));
        }
        light = new PointLight2D { Texture = Own(ImageTexture.CreateFromImage(image)), Energy = 0.7f,
            TextureScale = 1.2f, Position = new Vector2(-100, 0), Color = new Color("fff0cd") }; lit.AddChild(light);
    }
    protected override void Animate()
    {
        Keep(ribbon.TweenWidth(16, Seconds, Cycle));
        Keep(ribbon.TweenDefaultColor(Blue, Seconds, Cycle));
        Keep(polygon.TweenColor(Amber, Seconds, Cycle));
        Keep(polygon.TweenOffset(new Vector2(40, 0), Seconds, Cycle));
        Keep(polygon.TweenRotation(Mathf.Pi, Seconds * 2, Cycle));
        Keep(particles.TweenSpread(75, Seconds, Cycle));
        Keep(particles.TweenGravity(new Vector2(15, -55), Seconds, Cycle));
        Keep(particles.TweenColor(Amber, Seconds, Cycle));
        Keep(particles.TweenPositionY(-30, Seconds, Cycle));
        Keep(light.TweenTextureScale(2.5f, Seconds, Cycle));
        Keep(light.TweenEnergy(2, Seconds, Cycle));
        Keep(light.TweenPositionX(100, Seconds, Cycle));
    }
    public override void Pause(bool pause)
    {
        base.Pause(pause);
        if (pause) { particleSpeed = particles.SpeedScale; particles.SpeedScale = 0; }
        else particles.SpeedScale = particleSpeed;
    }
    public override void Stop() { base.Stop(); if (GodotObject.IsInstanceValid(particles)) particles.SpeedScale = 0; }
}

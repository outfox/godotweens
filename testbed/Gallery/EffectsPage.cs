// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using System;
using System.Linq;
using Godot;
using godotweens;
namespace testbed;

public partial class EffectsPage : GalleryPage
{
    public override string Heading => "Drawing & particles";
    public override string Description => "Line, polygon, particle, and 2D light properties.";
    private Line2D ribbon = null!, glow = null!;
    private Polygon2D[] echoes = [];
    private Polygon2D polygon = null!;
    private CpuParticles2D particles = null!;
    private PointLight2D light = null!;
    private double particleSpeed = 1;
    protected override void Build()
    {
        var drawing = View(Card("01 / Line2D", "Width and DefaultColor on a fixed path, with a matching glow pass."));
        Vector2[] zigzag = [new(-170, 35), new(-100, -40), new(-25, 25), new(55, -45), new(160, 30)];
        glow = Line(drawing, zigzag, Mint with { A = 0.28f }, 12);
        ribbon = Line(drawing, zigzag, Mint, 3);
        foreach (var line in new[] { glow, ribbon })
        {
            line.BeginCapMode = Line2D.LineCapMode.Round; line.EndCapMode = Line2D.LineCapMode.Round;
            line.JointMode = Line2D.LineJointMode.Round;
        }
        foreach (var point in zigzag) { Blob(drawing, 4, 4, GalleryTheme.Background, point); Blob(drawing, 2.5f, 2.5f, Colors.White, point); }
        var shapes = View(Card("02 / Polygon2D", "Color, offset, and rotation. Echoes replay the tweens with a Delay."));
        Vector2[] outline = [new(-45, -40), new(35, -50), new(65, 15), new(0, 50), new(-60, 15)];
        echoes = Enumerable.Range(1, 3).Reverse().Select(e => new Polygon2D { Polygon = outline, Color = Mint,
            Modulate = Colors.White with { A = 0.32f - e * 0.08f }, Antialiased = true }).ToArray();
        foreach (var echo in echoes) shapes.AddChild(echo);
        polygon = new Polygon2D { Polygon = outline, Color = Mint, Antialiased = true };
        shapes.AddChild(polygon);
        polygon.AddChild(new Line2D { Points = outline, Closed = true, Width = 2, DefaultColor = Colors.White with { A = 0.5f },
            JointMode = Line2D.LineJointMode.Round, Antialiased = true });
        var stream = View(Card("03 / CPUParticles2D", "Spread, gravity, color, and position. Pause stops simulation."));
        particles = new CpuParticles2D { Amount = 160, Lifetime = 2, Preprocess = 1, Direction = Vector2.Right,
            Spread = 8, InitialVelocityMin = 70, InitialVelocityMax = 110, Gravity = Vector2.Zero,
            ScaleAmountMin = 2, ScaleAmountMax = 5, Color = Mint, Position = new Vector2(-120, 25),
            LocalCoords = false, ColorRamp = Own(new Gradient { Colors = [Colors.White, Colors.White with { A = 0 }], Offsets = [0.5f, 1] }) };
        stream.AddChild(particles);
        Blob(particles, 16, 16, Mint with { A = 0.12f }).ShowBehindParent = true;
        particles.AddChild(new Polygon2D { Polygon = Rounded(10, 7, 3), Color = new Color("3a4f69"), Position = new Vector2(-8, 0) });
        Blob(particles, 3, 3, Colors.White);
        var lit = View(Card("04 / PointLight2D", "TextureScale, energy, and position."));
        var modulate = new CanvasModulate { Color = new Color("3a4a64") }; lit.AddChild(modulate);
        for (var y = -3; y < 3; y++) for (var x = -7; x < 7; x++)
        {
            var cell = new Polygon2D { Polygon = Rounded(13.5f, 13.5f, 5).Select(p => p + new Vector2(13.5f, 13.5f)).ToArray(), Antialiased = true,
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
        Blob(light, 6, 6, new Color("fff0cd")); Blob(light, 3, 3, Colors.White);
    }
    protected override void Animate()
    {
        Keep(ribbon.TweenWidth(16, Seconds, Cycle));
        Keep(ribbon.TweenDefaultColor(Blue, Seconds, Cycle));
        Keep(glow.TweenWidth(40, Seconds, Cycle));
        Keep(glow.TweenDefaultColor(Blue with { A = 0.28f }, Seconds, Cycle));
        for (var e = 0; e < echoes.Length; e++)
        {
            var delay = (echoes.Length - e) * 0.08;
            void Echo(TweenOptions d) { Cycle(d); d.Delay = delay; }
            Keep(echoes[e].TweenColor(Amber, Seconds, Echo));
            Keep(echoes[e].TweenOffset(new Vector2(40, 0), Seconds, Echo));
            Keep(echoes[e].TweenRotation(Mathf.Pi, Seconds * 2, Echo));
        }
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

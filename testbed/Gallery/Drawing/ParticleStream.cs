// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using Godot;
using tweens.gd;
namespace testbed;

public sealed partial class ParticleStream : GalleryEffect
{
    private CpuParticles2D particles = null!;

    public override string Title => "CPUParticles2D";
    public override string Caption => "Spread, gravity, color, and position.";

    protected override void Build()
    {
        var view = View();
        var fadeOut = Own(new Gradient { Colors = [Colors.White, Colors.White with { A = 0 }], Offsets = [0.5f, 1] });
        particles = view.Add(new CpuParticles2D
        {
            Amount = 160, Lifetime = 2, Preprocess = 1, LocalCoords = false, Position = new Vector2(-120, 25),
            Direction = Vector2.Right, Spread = 8, Gravity = Vector2.Zero, InitialVelocityMin = 70, InitialVelocityMax = 110,
            ScaleAmountMin = 2, ScaleAmountMax = 5, Color = Palette.Mint, ColorRamp = fadeOut,
        });

        Blob(particles, 16, 16, Palette.Mint with { A = 0.12f }).ShowBehindParent = true;
        particles.Add(new Polygon2D { Polygon = Rounded(10, 7, 3), Color = new Color("3a4f69"), Position = new Vector2(-8, 0) });
        Blob(particles, 3, 3, Colors.White);
    }

    protected override void Animate() => Sequence = Run(AnimateAsync());
}

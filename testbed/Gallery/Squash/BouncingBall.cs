// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using System;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using tweens.gd;
namespace testbed;

public sealed class BouncingBall : GalleryEffect
{
    private const float Ground = 62, Apex = Ground - 96, Stride = 75, Bounds = 151;
    private static readonly Vector2 Crouched = new(0.72f, 1.32f), Falling = new(0.8f, 1.25f), Squashed = new(1.55f, 0.55f);

    private Node2D ball = null!, spin = null!;
    private Polygon2D shadow = null!;
    private Line2D ring = null!;
    private Polygon2D[] dust = [];
    private int direction = 1;

    public override string Title => "Bouncing Ball";
    public override string Caption => "Stretch in flight, squash on impact, shadow and dust per landing.";

    protected override void Build()
    {
        var view = View();
        var start = new Vector2(-150, Ground);
        Line(view, [new(-210, Ground), new(210, Ground)], Palette.Outline, 2);
        shadow = Blob(view, 22, 5, new Color(0, 0, 0, 0.45f), start + new Vector2(0, 2));

        ring = view.Add(new Line2D
        {
            Points = Ellipse(24, 6, 40), Closed = true, DefaultColor = Palette.Amber, Width = 3, Antialiased = true,
            Position = start, Modulate = Colors.Transparent,
        });

        dust = Enumerable.Range(0, 6).Select(_ => Blob(view, 3.5f, 3.5f, Palette.Soft)).ToArray();
        foreach (var mote in dust) mote.Modulate = Colors.Transparent;

        ball = view.Add(new Node2D { Position = start });
        spin = ball.Add(new Node2D { Position = new Vector2(0, -20) });
        Blob(spin, 20, 20, Palette.Amber);
        Vector2[] stripe = [new(-19.5f, -4), new(19.5f, -4), new(19.5f, 4), new(-19.5f, 4)];
        spin.Add(new Polygon2D { Polygon = stripe, Color = new Color("d8893a") });
        Blob(ball, 5, 3.5f, new Color(1, 1, 1, 0.55f), new Vector2(-8, -30)).Rotation = -0.6f;
    }

    protected override void Animate() => Sequence = Repeat(Bounce);

    private async Task<bool> Bounce(int run)
    {
        var air = 0.36 * Tempo;
        if (Math.Abs(ball.Position.X + direction * Stride) > Bounds) direction = -direction;
        var target = ball.Position.X + direction * Stride;

        var crouch = Keep(ball.TweenScale(Crouched, 0.07 * Tempo, t => t.Ease = EaseType.QuadOut));
        if (!await Finished(run, crouch)) return false;

        Keep(spin.TweenRotation(spin.Rotation + direction * MathF.PI, air * 2));
        Keep(ball.TweenPositionX(target, air * 2));
        Keep(shadow.TweenPositionX(target, air * 2));

        void Rising(TweenOptions t) => t.Ease = EaseType.QuadOut;
        var rise = Finished(run,
            Keep(ball.TweenPositionY(Apex, air, Rising)),
            Keep(ball.TweenScale(Vector2.One, air, Rising)),
            Keep(shadow.TweenScale(new Vector2(0.4f, 0.4f), air, Rising)));
        if (!await rise) return false;

        void Dropping(TweenOptions t) => t.Ease = EaseType.QuadIn;
        var fall = Finished(run,
            Keep(ball.TweenPositionY(Ground, air, Dropping)),
            Keep(ball.TweenScale(Falling, air, Dropping)),
            Keep(shadow.TweenScale(Vector2.One, air, Dropping)));
        if (!await fall) return false;

        Ripple(target);
        KickUpDust(target);
        return await Finished(run, Keep(ball.TweenScale(Squashed, 0.06 * Tempo, t => t.Ease = EaseType.QuadOut)));
    }

    private void Ripple(float x)
    {
        var duration = 0.5 * Tempo;
        ring.Position = new Vector2(x, Ground);
        Keep(ring.TweenScale(new Vector2(2.2f, 2.2f), duration, t => { t.From = new Vector2(0.4f, 0.4f); t.Ease = EaseType.QuartOut; }));
        Keep(ring.TweenModulateAlpha(0, duration, t => { t.From = 1; t.Ease = EaseType.QuadIn; }));
    }

    private void KickUpDust(float x)
    {
        var duration = 0.45 * Tempo;
        for (var i = 0; i < dust.Length; i++)
        {
            var side = i % 2 == 0 ? -1 : 1;
            var row = i / 2;
            var landing = new Vector2(x + side * (26 + row * 16), Ground - 10 - row * 5);
            dust[i].Position = new Vector2(x + side * 14, Ground - 3);
            dust[i].Scale = Vector2.One * (1.4f - row * 0.3f);
            Keep(dust[i].TweenPosition(landing, duration, t => t.Ease = EaseType.QuartOut));
            Keep(dust[i].TweenModulateAlpha(0, duration, t => { t.From = 0.9f; t.Ease = EaseType.QuadIn; }));
        }
    }
}

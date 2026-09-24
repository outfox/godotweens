// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using System;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using godotweens;
namespace testbed;

public partial class SquashPage : GalleryPage
{
    public override string Heading => "Squash & stretch";
    public override string Description => "Anticipation, overshoot, and volume-preserving scale, chained with async sequences.";
    private const float Ground = 62, Rest = 52;
    private Node2D ball = null!, spin = null!, slime = null!, flip = null!, eyes = null!, pupils = null!;
    private Polygon2D shadow = null!;
    private Line2D ring = null!;
    private Polygon2D[] dust = [], drops = [], shards = [], pills = [];
    private Control shaker = null!;
    private Button tap = null!;
    private Line2D burst = null!;
    private Label plus = null!, score = null!;
    private TweenInstance? squish;
    private float total, shown;
    private int ballDirection = 1, slimeDirection = 1, hop;
    private static readonly Color Ink = new("0e1620");

    protected override void Build()
    {
        BuildBall(View(Card("01 / Bouncing Ball", "Stretch in flight, squash on impact, shadow and dust per landing.")));
        BuildSlime(View(Card("02 / Slime Hop", "Anticipation, launch stretch, flips, elastic landing, blinking eyes.")));
        BuildButton(Card("03 / Jelly Button", "Click it! Elastic pop, arcing confetti, rolling score, and shake."));
        BuildWave(View(Card("04 / Squash Wave", "Per-item Delay turns one looping tween into a wave. Uses Easing.")));
    }

    private void BuildBall(SubViewport view)
    {
        Line(view, [new(-210, Ground), new(210, Ground)], GalleryTheme.Outline, 2);
        shadow = Blob(view, 22, 5, new Color(0, 0, 0, 0.45f), new Vector2(-150, Ground + 2));
        ring = Line(view, Ellipse(24, 6, 40), Amber, 3); ring.Closed = true;
        ring.Position = new Vector2(-150, Ground); ring.Modulate = Colors.Transparent;
        dust = Enumerable.Range(0, 6).Select(_ => Blob(view, 3.5f, 3.5f, new Color("c9d6e2"))).ToArray();
        foreach (var mote in dust) mote.Modulate = Colors.Transparent;
        ball = new Node2D { Position = new Vector2(-150, Ground) }; view.AddChild(ball);
        spin = new Node2D { Position = new Vector2(0, -20) }; ball.AddChild(spin);
        Blob(spin, 20, 20, Amber);
        spin.AddChild(new Polygon2D { Polygon = [new(-19.5f, -4), new(19.5f, -4), new(19.5f, 4), new(-19.5f, 4)], Color = new Color("d8893a") });
        Blob(ball, 5, 3.5f, new Color(1, 1, 1, 0.55f), new Vector2(-8, -30)).Rotation = -0.6f;
    }

    private void BuildSlime(SubViewport view)
    {
        Line(view, [new(-210, Rest), new(210, Rest)], GalleryTheme.Outline, 2);
        drops = Enumerable.Range(0, 5).Select(_ => Blob(view, 4, 4, Mint)).ToArray();
        foreach (var drop in drops) drop.Modulate = Colors.Transparent;
        slime = new Node2D { Position = new Vector2(-135, Rest) }; view.AddChild(slime);
        flip = new Node2D { Position = new Vector2(0, -20) }; slime.AddChild(flip);
        // A dome: round on top, nearly flat underneath, centred on the flip pivot.
        var dome = Enumerable.Range(0, 40).Select(i =>
        {
            var a = i * MathF.Tau / 40;
            return new Vector2(MathF.Cos(a) * 36, MathF.Sin(a) * (MathF.Sin(a) > 0 ? 20 : 38));
        }).ToArray();
        flip.AddChild(new Polygon2D { Polygon = dome, Color = Mint, Antialiased = true });
        Blob(flip, 8, 4, new Color(1, 1, 1, 0.5f), new Vector2(-17, -24)).Rotation = -0.5f;
        eyes = new Node2D { Position = new Vector2(0, -8) }; flip.AddChild(eyes);
        foreach (var x in new[] { -11f, 11f }) Blob(eyes, 6, 7.5f, Colors.White, new Vector2(x, 0));
        pupils = new Node2D(); eyes.AddChild(pupils);
        foreach (var x in new[] { -11f, 11f }) Blob(pupils, 3.2f, 4, Ink, new Vector2(x + 1, 1));
    }

    private void BuildButton(Control stage)
    {
        var anchor = new Control { MouseFilter = MouseFilterEnum.Ignore }; stage.AddChild(anchor);
        anchor.SetAnchorsAndOffsetsPreset(LayoutPreset.Center);
        shaker = new Control { MouseFilter = MouseFilterEnum.Ignore }; anchor.AddChild(shaker);
        var fx = new Node2D { Position = new Vector2(0, 8) }; shaker.AddChild(fx);
        burst = Line(fx, Ellipse(60, 30, 48), Mint, 4); burst.Closed = true; burst.Modulate = Colors.Transparent;
        Color[] palette = [Mint, Amber, Blue, new("ff8fa3"), Colors.White];
        shards = Enumerable.Range(0, 18).Select(i => new Polygon2D
        {
            Polygon = [new(-5, -2.5f), new(5, -2.5f), new(5, 2.5f), new(-5, 2.5f)], Color = palette[i % palette.Length],
            Modulate = Colors.Transparent
        }).ToArray();
        foreach (var shard in shards) fx.AddChild(shard);
        tap = new Button { Text = "TAP", FocusMode = FocusModeEnum.None, Size = new Vector2(150, 58),
            Position = new Vector2(-75, -21), PivotOffset = new Vector2(75, 58) };
        tap.AddThemeFontSizeOverride("font_size", 24);
        foreach (var state in new[] { "normal", "hover", "pressed", "hover_pressed" })
        {
            var box = Own(GalleryTheme.Box(state.Contains("hover") ? new Color("97ecc8") : Mint, 18, 0));
            box.ShadowColor = new Color(0, 0, 0, 0.35f); box.ShadowSize = 8; box.ShadowOffset = new Vector2(0, 4);
            tap.AddThemeStyleboxOverride(state, box);
        }
        foreach (var color in new[] { "font_color", "font_hover_color", "font_pressed_color", "font_hover_pressed_color" })
            tap.AddThemeColorOverride(color, Ink);
        tap.Pressed += () => Pop(Generation);
        shaker.AddChild(tap);
        plus = Text("+10", 22, Amber); plus.Modulate = Colors.Transparent; plus.Position = new Vector2(-18, -60); shaker.AddChild(plus);
        var label = Text("SCORE", 12, Muted); stage.AddChild(label); label.SetAnchorsPreset(LayoutPreset.TopRight);
        (label.OffsetLeft, label.OffsetRight, label.OffsetTop) = (-110, -64, 19);
        score = Text("000", 26, GalleryTheme.Text); score.HorizontalAlignment = HorizontalAlignment.Right;
        stage.AddChild(score); score.SetAnchorsPreset(LayoutPreset.TopRight);
        (score.OffsetLeft, score.OffsetRight, score.OffsetTop, score.OffsetBottom) = (-64, -14, 6, 42);
        score.PivotOffset = new Vector2(36, 18);
    }

    private void BuildWave(SubViewport view)
    {
        const int count = 15;
        pills = new Polygon2D[count];
        for (var i = 0; i < count; i++)
        {
            var t = i / (count - 1f);
            var color = t < 0.5f ? Mint.Lerp(Blue, t * 2) : Blue.Lerp(Amber, t * 2 - 1);
            // Rounded pill anchored at its base, so ScaleY grows upward from the floor.
            var outline = Ellipse(7, 7, 12, new Vector2(0, -7)).Where(p => p.Y >= -7)
                .Concat(Ellipse(7, 7, 12, new Vector2(0, -29)).Where(p => p.Y <= -29)).OrderBy(p => MathF.Atan2(p.Y + 18, p.X)).ToArray();
            pills[i] = new Polygon2D { Polygon = outline, Color = color, Antialiased = true, Position = new Vector2((i - 7) * 26, 58) };
            view.AddChild(pills[i]);
        }
        Line(view, [new(-210, 59), new(210, 59)], GalleryTheme.Outline, 2);
    }

    protected override void Animate()
    {
        foreach (var (pill, i) in pills.Select((p, i) => (p, i)))
        {
            void Wave(TweenOptions d) { Cycle(d); d.Delay = i * 0.07 * Tempo; d.RepeatInterval = 0.1; d.PingPongInterval = 0.05; }
            Keep(pill.TweenScaleY(2.6f, Seconds * 0.5, Wave));
            Keep(pill.TweenScaleX(0.62f, Seconds * 0.5, Wave));
            Keep(pill.TweenColor(pill.Color.Lightened(0.45f), Seconds * 0.5, Wave));
        }
        Keep(eyes.TweenScaleY(0.1f, 0.07, d => { d.UsePingPong = true; d.IsInfinite = true; d.RepeatInterval = 2.2; d.Delay = 0.9; }));
        SequenceTask = Task.WhenAll(Repeat(Bounce), Repeat(Hop), Repeat(AutoTap));
    }

    private async Task<bool> Bounce(int run)
    {
        var t = Tempo; var air = 0.36 * t;
        var x = ball.Position.X;
        if (Math.Abs(x + ballDirection * 75) > 151) ballDirection = -ballDirection;
        var target = x + ballDirection * 75;
        if (!await All(run, Keep(ball.TweenScale(new Vector2(0.72f, 1.32f), 0.07 * t, d => d.Ease = EaseType.QuadOut)))) return false;
        Keep(spin.TweenRotation(spin.Rotation + ballDirection * MathF.PI, air * 2));
        Keep(ball.TweenPositionX(target, air * 2)); Keep(shadow.TweenPositionX(target, air * 2));
        if (!await All(run, Keep(ball.TweenPositionY(Ground - 96, air, d => d.Ease = EaseType.QuadOut)),
                Keep(ball.TweenScale(Vector2.One, air, d => d.Ease = EaseType.QuadOut)),
                Keep(shadow.TweenScale(new Vector2(0.4f, 0.4f), air, d => d.Ease = EaseType.QuadOut)))) return false;
        if (!await All(run, Keep(ball.TweenPositionY(Ground, air, d => d.Ease = EaseType.QuadIn)),
                Keep(ball.TweenScale(new Vector2(0.8f, 1.25f), air, d => d.Ease = EaseType.QuadIn)),
                Keep(shadow.TweenScale(Vector2.One, air, d => d.Ease = EaseType.QuadIn)))) return false;
        ring.Position = new Vector2(target, Ground);
        Keep(ring.TweenScale(new Vector2(2.2f, 2.2f), 0.5 * t, d => { d.From = new Vector2(0.4f, 0.4f); d.Ease = EaseType.QuartOut; }));
        Keep(ring.TweenModulateAlpha(0, 0.5 * t, d => { d.From = 1; d.Ease = EaseType.QuadIn; }));
        for (var i = 0; i < dust.Length; i++)
        {
            var side = i % 2 == 0 ? -1 : 1; var reach = 26 + i / 2 * 16;
            dust[i].Position = new Vector2(target + side * 14, Ground - 3); dust[i].Scale = Vector2.One * (1.4f - i / 2 * 0.3f);
            Keep(dust[i].TweenPosition(new Vector2(target + side * reach, Ground - 10 - i / 2 * 5), 0.45 * t, d => d.Ease = EaseType.QuartOut));
            Keep(dust[i].TweenModulateAlpha(0, 0.45 * t, d => { d.From = 0.9f; d.Ease = EaseType.QuadIn; }));
        }
        return await All(run, Keep(ball.TweenScale(new Vector2(1.55f, 0.55f), 0.06 * t, d => d.Ease = EaseType.QuadOut)));
    }

    private async Task<bool> Hop(int run)
    {
        var t = Tempo;
        var x = slime.Position.X;
        if (Math.Abs(x + slimeDirection * 90) > 136) slimeDirection = -slimeDirection;
        var target = x + slimeDirection * 90;
        Keep(pupils.TweenPositionX(slimeDirection * 3, 0.2 * t, d => d.Ease = EaseType.BackOut));
        if (!await All(run, Keep(slime.TweenScale(new Vector2(1.38f, 0.6f), 0.32 * t, d => d.Ease = EaseType.SineOut)))) return false;
        if (!await All(run, Keep(slime.TweenScale(new Vector2(0.68f, 1.45f), 0.08 * t, d => d.Ease = EaseType.QuadOut)))) return false;
        var air = 0.32 * t;
        Keep(slime.TweenPositionX(target, air * 2, d => d.Ease = EaseType.SineInOut));
        if (++hop % 2 == 0) Keep(flip.TweenRotation(slimeDirection * MathF.Tau, air * 2, d => { d.From = 0; d.Ease = EaseType.CubicInOut; }));
        if (!await All(run, Keep(slime.TweenPositionY(Rest - 78, air, d => d.Ease = EaseType.QuadOut)),
                Keep(slime.TweenScale(Vector2.One, air, d => d.Ease = EaseType.QuadOut)))) return false;
        if (!await All(run, Keep(slime.TweenPositionY(Rest, air, d => d.Ease = EaseType.QuadIn)),
                Keep(slime.TweenScale(new Vector2(0.82f, 1.25f), air, d => d.Ease = EaseType.QuadIn)))) return false;
        for (var i = 0; i < drops.Length; i++)
        {
            var angle = MathF.PI + (i + 0.5f) / drops.Length * MathF.PI;
            var start = new Vector2(target, Rest - 6);
            drops[i].Position = start; drops[i].Scale = Vector2.One * (i % 2 == 0 ? 1 : 0.7f);
            Keep(drops[i].TweenPosition(start + new Vector2(MathF.Cos(angle) * 58, MathF.Sin(angle) * 34), 0.4 * t,
                d => d.Ease = EaseType.QuartOut));
            Keep(drops[i].TweenModulateAlpha(0, 0.4 * t, d => { d.From = 1; d.Ease = EaseType.CubicIn; }));
        }
        if (!await All(run, Keep(slime.TweenScale(new Vector2(1.6f, 0.5f), 0.06 * t, d => d.Ease = EaseType.QuadOut)))) return false;
        return await All(run, Keep(slime.TweenScale(Vector2.One, 0.75 * t, d => d.Ease = EaseType.ElasticOut)));
    }

    private async Task<bool> AutoTap(int run)
    {
        if (!await Wait(run, 1.5 * Tempo)) return false;
        Pop(run); return true;
    }

    private void Pop(int run)
    {
        var t = Tempo;
        squish?.Cancel();
        squish = Keep(tap.TweenScale(new Vector2(1.3f, 0.7f), 0.07 * t, d =>
        {
            d.Ease = EaseType.QuadOut;
            d.OnEnd = _ => { if (run == Generation) squish = Keep(tap.TweenScale(Vector2.One, 0.8 * t, e => e.Ease = EaseType.ElasticOut)); };
        }));
        Keep(shaker.TweenPosition(new Vector2(9, 5), 0.4 * t, d => { d.From = Vector2.Zero; d.EaseFunction = Shake; }));
        Keep(burst.TweenScale(new Vector2(2.1f, 2.1f), 0.6 * t, d => { d.From = new Vector2(0.7f, 0.7f); d.Ease = EaseType.QuartOut; }));
        Keep(burst.TweenModulateAlpha(0, 0.6 * t, d => { d.From = 1; d.Ease = EaseType.QuadIn; }));
        var random = new Random();
        foreach (var shard in shards)
        {
            var angle = (float)(random.NextDouble() * MathF.PI * 1.2 + MathF.PI * 0.9);
            var reach = 90 + (float)random.NextDouble() * 110;
            var landing = new Vector2(MathF.Cos(angle) * reach, 70 + (float)random.NextDouble() * 20);
            shard.Position = Vector2.Zero; shard.Rotation = angle; shard.Modulate = Colors.White;
            Keep(shard.TweenPositionX(landing.X, 0.9 * t, d => d.Ease = EaseType.QuartOut));
            // 3t² − 2t dips to −⅓ before landing at 1: a launch arc from a single scalar tween.
            var lift = 1 + (float)random.NextDouble();
            Keep(shard.TweenPositionY(landing.Y, 0.9 * t, d => d.EaseFunction = w => (2 + lift) * w * w - (1 + lift) * w));
            Keep(shard.TweenRotation(angle + (float)(random.NextDouble() * 12 - 6), 0.9 * t, d => d.Ease = EaseType.QuadOut));
            Keep(shard.TweenModulateAlpha(0, 0.3 * t, d => d.Delay = 0.6 * t));
        }
        plus.Modulate = Colors.White;
        Keep(plus.TweenPositionY(-95, 0.7 * t, d => { d.From = -50; d.Ease = EaseType.QuartOut; }));
        Keep(plus.TweenModulateAlpha(0, 0.3 * t, d => d.Delay = 0.4 * t));
        total += 10;
        Keep(this.TweenFloat(total, 0.5 * t, d =>
        {
            d.From = shown; d.Ease = EaseType.CubicOut;
            d.OnUpdate = (_, value) => { shown = value; score.Text = $"{value:000}"; };
        }));
        Keep(score.TweenScale(Vector2.One, 0.6 * t, d => { d.From = new Vector2(1.45f, 1.45f); d.Ease = EaseType.ElasticOut; }));
    }
}

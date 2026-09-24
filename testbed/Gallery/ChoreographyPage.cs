// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using System;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using godotweens;
namespace testbed;

public partial class ChoreographyPage : GalleryPage
{
    public override string Heading => "Choreography";
    public override string Description => "Staggered delays, echo trails, OnUpdate drivers, and a 3D jelly cube with camera shake.";
    private static readonly (EaseType Ease, string Name)[] Lanes =
    [
        (EaseType.Linear, "Linear"), (EaseType.SineInOut, "Sine"), (EaseType.CubicInOut, "Cubic"), (EaseType.ExpoInOut, "Expo"),
        (EaseType.BackInOut, "Back"), (EaseType.ElasticOut, "Elastic"), (EaseType.BounceOut, "Bounce"),
    ];
    private static readonly string[] Ranks = ["10", "J", "Q", "K", "A"];
    private static readonly Color Paper = new("eef4fa");
    private Node2D[] cards = [], backs = [], faces = [];
    private Polygon2D[][] racers = [];
    private Line2D trailA = null!, trailB = null!, armA = null!, armB = null!;
    private Polygon2D penA = null!, penB = null!, sun = null!;
    private Node3D feet = null!, tumble = null!;
    private MeshInstance3D wave = null!;
    private StandardMaterial3D jelly = null!, ripple = null!;
    private Camera3D lens = null!;
    private int landings;

    protected override void Build()
    {
        BuildCards(View(Card("01 / Card Deal", "Staggered deal, sequential flips, hero lift, gather and toss.")));
        BuildRace(View(Card("02 / Easing Race", "Seven eases, each chased by delayed echoes of the same tween.")));
        BuildOrbits(View(Card("03 / Spirograph", "One looping TweenFloat drives both epicycles through OnUpdate.")));
        BuildJelly(World(Card("04 / Jelly Cube", "3D squash, tumble, shockwave, color shift, and camera shake.")));
    }

    private void BuildCards(SubViewport view)
    {
        cards = new Node2D[Ranks.Length]; backs = new Node2D[Ranks.Length]; faces = new Node2D[Ranks.Length];
        for (var i = 0; i < Ranks.Length; i++)
        {
            cards[i] = new Node2D { Position = new Vector2(0, 170) }; view.AddChild(cards[i]);
            cards[i].AddChild(new Polygon2D { Polygon = Rounded(29, 41, 6), Color = new Color(0, 0, 0, 0.35f), Position = new Vector2(2, 4) });
            backs[i] = new Node2D(); cards[i].AddChild(backs[i]);
            backs[i].AddChild(new Polygon2D { Polygon = Rounded(27, 39, 5), Color = new Color("3a5bb8"), Antialiased = true });
            backs[i].AddChild(new Line2D { Points = Rounded(20, 32, 3), Closed = true, Width = 2, DefaultColor = Blue });
            Diamond(backs[i], Vector2.Zero, Blue, 9);
            faces[i] = new Node2D { Visible = false }; cards[i].AddChild(faces[i]);
            faces[i].AddChild(new Polygon2D { Polygon = Rounded(27, 39, 5), Color = Paper, Antialiased = true });
            var ink = i == Ranks.Length - 1 ? new Color("e0566f") : new Color("24344a");
            var rank = Text(Ranks[i], 26, ink); rank.HorizontalAlignment = HorizontalAlignment.Center;
            rank.Size = new Vector2(54, 36); rank.Position = new Vector2(-27, -18); faces[i].AddChild(rank);
            var pip = Text(Ranks[i], 11, ink); pip.Position = new Vector2(-23, -38); faces[i].AddChild(pip);
        }
    }

    private void BuildRace(SubViewport view)
    {
        racers = new Polygon2D[Lanes.Length][];
        for (var lane = 0; lane < Lanes.Length; lane++)
        {
            var y = (lane - 3) * 24f;
            var color = Mint.Lerp(Blue, lane / 6f);
            var name = Text(Lanes[lane].Name, 12, Muted); name.Position = new Vector2(-208, y - 9); view.AddChild(name);
            Line(view, [new(-120, y), new(120, y)], GalleryTheme.Outline, 1);
            // Echoes first so the leading dot draws on top.
            racers[lane] = Enumerable.Range(0, 4).Reverse().Select(e =>
                Blob(view, 6 - e, 6 - e, color with { A = 1 - e * 0.26f }, new Vector2(-120, y))).Reverse().ToArray();
        }
    }

    private void BuildOrbits(SubViewport view)
    {
        foreach (var r in new[] { 40f, 86f }) { var guide = Line(view, Ellipse(r, r, 64), new Color("1f2d3f"), 1); guide.Closed = true; }
        Line2D Trail(Color color) => new()
        {
            Width = 3.5f, JointMode = Line2D.LineJointMode.Round, Antialiased = true,
            Gradient = Own(new Gradient { Colors = [color with { A = 0 }, color], Offsets = [0, 1] })
        };
        trailA = Trail(Mint); trailB = Trail(Amber); view.AddChild(trailA); view.AddChild(trailB);
        armA = Line(view, [], new Color("56718f"), 1.5f); armB = Line(view, [], new Color("56718f"), 1.5f);
        sun = Blob(view, 9, 9, Blue);
        penA = Blob(view, 5, 5, Mint); penB = Blob(view, 5, 5, Amber);
    }

    private void BuildJelly((SubViewport View, Camera3D Camera) world)
    {
        var (view, camera) = world;
        lens = camera; camera.Position = new Vector3(0, 1.3f, 4.2f); camera.LookAt(new Vector3(0, 0.3f, 0));
        Floor(view, -0.6f);
        ripple = Own(new StandardMaterial3D { AlbedoColor = Mint with { A = 0 }, Transparency = BaseMaterial3D.TransparencyEnum.Alpha,
            ShadingMode = BaseMaterial3D.ShadingModeEnum.Unshaded });
        wave = Mesh(view, new TorusMesh { InnerRadius = 0.5f, OuterRadius = 0.53f, Rings = 64 }, ripple, new Vector3(0, -0.58f, 0));
        feet = new Node3D { Position = new Vector3(0, -0.6f, 0) }; view.AddChild(feet);
        tumble = new Node3D { Position = new Vector3(0, 0.45f, 0) }; feet.AddChild(tumble);
        jelly = Surface(Mint); jelly.Roughness = 0.2f;
        Mesh(tumble, new BoxMesh { Size = new Vector3(0.9f, 0.9f, 0.9f) }, jelly);
    }

    protected override void Animate()
    {
        for (var lane = 0; lane < racers.Length; lane++)
            for (var e = 0; e < racers[lane].Length; e++)
            {
                var ease = Lanes[lane].Ease; var delay = e * 0.05;
                Keep(racers[lane][e].TweenPositionX(120, Seconds, d =>
                {
                    d.Ease = ease; d.UsePingPong = true; d.IsInfinite = true;
                    d.PingPongInterval = 0.3; d.RepeatInterval = 0.3; d.Delay = delay;
                }));
            }
        Keep(sun.TweenScale(new Vector2(1.35f, 1.35f), 0.6 * Tempo, d => { d.Ease = EaseType.SineInOut; d.UsePingPong = true; d.IsInfinite = true; }));
        Keep(this.TweenFloat(1, 7 * Tempo, d => { d.From = 0; d.IsInfinite = true; d.OnUpdate = (_, v) => Orbit(v); }));
        SequenceTask = Task.WhenAll(Repeat(Deal), Repeat(Jump));
    }

    private void Orbit(float v)
    {
        var a = v * MathF.Tau;
        // Integer frequency ratios keep both curves closed, so the loop wraps seamlessly.
        var jointA = new Vector2(MathF.Cos(a * 2), MathF.Sin(a * 2)) * 58;
        var tipA = jointA + new Vector2(MathF.Cos(-a * 8), MathF.Sin(-a * 8)) * 26;
        var jointB = new Vector2(MathF.Cos(-a), MathF.Sin(-a)) * 76;
        var tipB = jointB + new Vector2(MathF.Cos(a * 11), MathF.Sin(a * 11)) * 12;
        armA.Points = [Vector2.Zero, jointA, tipA]; armB.Points = [Vector2.Zero, jointB, tipB];
        penA.Position = tipA; penB.Position = tipB;
        foreach (var (trail, tip) in new[] { (trailA, tipA), (trailB, tipB) })
        {
            trail.AddPoint(tip);
            while (trail.GetPointCount() > 420) trail.RemovePoint(0);
        }
    }

    private async Task<bool> Deal(int run)
    {
        var t = Tempo; var n = cards.Length;
        for (var i = 0; i < n; i++)
        {
            cards[i].Position = new Vector2(0, 170); cards[i].Rotation = 0; cards[i].Scale = Vector2.One;
            cards[i].Modulate = Colors.White; backs[i].Visible = true; faces[i].Visible = false;
        }
        var dealt = Enumerable.Range(0, n).SelectMany(i =>
        {
            var offset = i - (n - 1) / 2f;
            void Stagger(TweenOptions d) { d.Delay = i * 0.1 * t; d.Ease = EaseType.BackOut; }
            return new TweenInstance[]
            {
                Keep(cards[i].TweenPosition(new Vector2(offset * 64, MathF.Abs(offset) * 7 + 4), 0.5 * t, Stagger)),
                Keep(cards[i].TweenRotation(offset * 0.13f, 0.5 * t, Stagger)),
            };
        }).ToArray();
        if (!await All(run, dealt)) return false;
        if (!(await Task.WhenAll(Enumerable.Range(0, n).Select(i => Flip(run, i, i * 0.09 * t, true)))).All(ok => ok)) return false;
        var hero = cards[n - 1];
        if (!await All(run, Keep(hero.TweenPositionY(hero.Position.Y - 26, 0.35 * t, d => d.Ease = EaseType.BackOut)),
                Keep(hero.TweenScale(new Vector2(1.18f, 1.18f), 0.35 * t, d => d.Ease = EaseType.BackOut)),
                Keep(hero.TweenRotation(0, 0.35 * t, d => d.Ease = EaseType.BackOut)))) return false;
        if (!await Wait(run, 0.6 * t)) return false;
        var gathered = Enumerable.Range(0, n).SelectMany(i =>
        {
            void Stagger(TweenOptions d) { d.Delay = (n - 1 - i) * 0.05 * t; d.Ease = EaseType.CubicInOut; }
            return new TweenInstance[]
            {
                Keep(cards[i].TweenPosition(new Vector2(0, -i * 2), 0.35 * t, Stagger)),
                Keep(cards[i].TweenRotation(0, 0.35 * t, Stagger)), Keep(cards[i].TweenScale(Vector2.One, 0.35 * t, Stagger)),
            };
        }).ToArray();
        if (!await All(run, gathered)) return false;
        if (!(await Task.WhenAll(Enumerable.Range(0, n).Select(i => Flip(run, i, 0, false)))).All(ok => ok)) return false;
        var tossed = Enumerable.Range(0, n).SelectMany(i =>
        {
            void Stagger(TweenOptions d) { d.Delay = i * 0.04 * t; d.Ease = EaseType.BackIn; }
            return new TweenInstance[]
            {
                Keep(cards[i].TweenPosition(new Vector2((i - 2) * 30, -170), 0.45 * t, Stagger)),
                Keep(cards[i].TweenRotation((i - 2) * 0.4f, 0.45 * t, Stagger)),
            };
        }).ToArray();
        return await All(run, tossed);
    }

    private async Task<bool> Flip(int run, int i, double delay, bool faceUp)
    {
        var t = Tempo;
        if (!await All(run, Keep(cards[i].TweenScaleX(0, 0.1 * t, d => { d.Delay = delay; d.Ease = EaseType.QuadIn; })))) return false;
        backs[i].Visible = !faceUp; faces[i].Visible = faceUp;
        return await All(run, Keep(cards[i].TweenScaleX(1, 0.22 * t, d => d.Ease = EaseType.BackOut)));
    }

    private async Task<bool> Jump(int run)
    {
        var t = Tempo;
        if (!await All(run, Keep(feet.TweenScale(new Vector3(1.35f, 0.62f, 1.35f), 0.3 * t, d => d.Ease = EaseType.SineOut)))) return false;
        if (!await All(run, Keep(feet.TweenScale(new Vector3(0.72f, 1.42f, 0.72f), 0.09 * t, d => d.Ease = EaseType.QuadOut)))) return false;
        var air = 0.36 * t;
        // Any combination of quarter turns leaves the cube looking identical, so rotation resets after landing.
        Vector3[] turns = [new(MathF.PI / 2, 0, 0), new(0, MathF.PI / 2, MathF.PI / 2), new(0, 0, -MathF.PI / 2)];
        Keep(tumble.TweenRotation(turns[landings % turns.Length], air * 2, d => { d.From = Vector3.Zero; d.Ease = EaseType.CubicInOut; }));
        if (!await All(run, Keep(feet.TweenPositionY(1.0f, air, d => d.Ease = EaseType.QuadOut)),
                Keep(feet.TweenScale(Vector3.One, air, d => d.Ease = EaseType.QuadOut)))) return false;
        if (!await All(run, Keep(feet.TweenPositionY(-0.6f, air, d => d.Ease = EaseType.QuadIn)),
                Keep(feet.TweenScale(new Vector3(0.84f, 1.24f, 0.84f), air, d => d.Ease = EaseType.QuadIn)))) return false;
        tumble.Rotation = Vector3.Zero;
        Color[] palette = [Blue, Amber, Mint];
        Keep(jelly.TweenAlbedoColor(palette[landings++ % palette.Length], 0.3 * t, this));
        Keep(wave.TweenScale(new Vector3(2.6f, 1, 2.6f), 0.7 * t, d => { d.From = new Vector3(0.9f, 1, 0.9f); d.Ease = EaseType.QuartOut; }));
        Keep(ripple.TweenAlbedoAlpha(0, 0.7 * t, this, d => { d.From = 0.9f; d.Ease = EaseType.QuadIn; }));
        Keep(lens.TweenVOffset(0.06f, 0.4 * t, d => { d.From = 0; d.EaseFunction = Shake; }));
        Keep(lens.TweenHOffset(0.035f, 0.4 * t, d => { d.From = 0; d.EaseFunction = w => Shake(MathF.Min(1, w * 1.3f)); }));
        if (!await All(run, Keep(feet.TweenScale(new Vector3(1.5f, 0.55f, 1.5f), 0.06 * t, d => d.Ease = EaseType.QuadOut)))) return false;
        return await All(run, Keep(feet.TweenScale(Vector3.One, 0.8 * t, d => d.Ease = EaseType.ElasticOut)));
    }
}

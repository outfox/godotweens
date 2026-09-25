// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using System.Linq;
using Godot;
using tweens.gd;
namespace testbed;

public sealed class EasingRace : GalleryEffect
{
    private const float StartLine = -120, FinishLine = 120, LaneHeight = 24;
    private const int Echoes = 3;
    private static readonly (EaseType Ease, string Name)[] Lanes =
    [
        (EaseType.Linear, "Linear"), (EaseType.SineInOut, "Sine"), (EaseType.CubicInOut, "Cubic"), (EaseType.ExpoInOut, "Expo"),
        (EaseType.BackInOut, "Back"), (EaseType.ElasticOut, "Elastic"), (EaseType.BounceOut, "Bounce"),
    ];

    /// <summary>Per lane: the leading dot followed by its echoes.</summary>
    private Polygon2D[][] racers = [];

    public override string Title => "Easing Race";
    public override string Caption => "Seven eases, each chased by delayed echoes of the same tween.";

    protected override void Build()
    {
        var view = View();
        racers = Lanes.Select((lane, index) =>
        {
            var y = (index - Lanes.Length / 2) * LaneHeight;
            var color = Palette.Mint.Lerp(Palette.Blue, index / (Lanes.Length - 1f));
            var name = GalleryTheme.Label(lane.Name, 12, Palette.Muted);
            name.Position = new Vector2(-208, y - 9);
            view.AddChild(name);
            Line(view, [new(StartLine, y), new(FinishLine, y)], Palette.Outline, 1);

            // Faintest echo first, so the leading dot draws on top.
            var trail = Enumerable.Range(0, Echoes + 1).Reverse()
                .Select(e => Blob(view, 6 - e, 6 - e, color with { A = 1 - e * 0.26f }, new Vector2(StartLine, y)));
            return trail.Reverse().ToArray();
        }).ToArray();
    }

    protected override void Animate()
    {
        for (var lane = 0; lane < racers.Length; lane++)
            for (var position = 0; position < racers[lane].Length; position++)
            {
                var ease = Lanes[lane].Ease;
                var delay = position * 0.05;
                void Race(TweenOptions t)
                {
                    t.Ease = ease;
                    t.UsePingPong = true;
                    t.IsInfinite = true;
                    t.PingPongInterval = 0.3;
                    t.RepeatInterval = 0.3;
                    t.Delay = delay;
                }
                Keep(racers[lane][position].TweenPositionX(FinishLine, Seconds, Race));
            }
    }
}

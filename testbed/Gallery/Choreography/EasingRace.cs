// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using System.Linq;
using Godot;
namespace testbed;

public sealed partial class EasingRace : GalleryEffect
{
    private const int Echoes = 3;

    /// <summary>Per lane: the leading dot followed by its echoes.</summary>
    private Polygon2D[][] racers = [];

    public override string Title => "Easing Race";
    public override string Caption => "Seven eases, each chased by delayed echoes of the same tween.";

    protected override void Build()
    {
        var view = View();
        racers = Lanes.Select((lane, index) =>
        {
            var y = (index - (Lanes.Length - 1) / 2f) * LaneHeight;
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

    protected override void Animate() => Sequence = Run(AnimateAsync());
}

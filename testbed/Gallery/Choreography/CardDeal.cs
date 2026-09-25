// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using System;
using System.Linq;
using Godot;
using tweens.gd;
namespace testbed;

public sealed partial class CardDeal : GalleryEffect
{
    private static readonly string[] Ranks = ["10", "J", "Q", "K", "A"];
    private static readonly Color Paper = new("eef4fa"), Ink = new("24344a"), Red = new("e0566f"), CardBack = new("3a5bb8");

    private sealed record PlayingCard(Node2D Body, Node2D Back, Node2D Face)
    {
        public void Show(bool faceUp)
        {
            Back.Visible = !faceUp;
            Face.Visible = faceUp;
        }
    }

    private PlayingCard[] deck = [];

    public override string Title => "Card Deal";
    public override string Caption => "Staggered deal, sequential flips, hero lift, gather and toss.";

    protected override void Build()
    {
        var view = View();
        deck = Ranks.Select((rank, i) => CreateCard(view, rank, i == Ranks.Length - 1 ? Red : Ink)).ToArray();
    }

    private static PlayingCard CreateCard(Node parent, string rank, Color ink)
    {
        var body = parent.Add(new Node2D { Position = DeckPosition });
        body.Add(new Polygon2D { Polygon = Rounded(29, 41, 6), Color = new Color(0, 0, 0, 0.35f), Position = new Vector2(2, 4) });

        var back = body.Add(new Node2D());
        back.Add(new Polygon2D { Polygon = Rounded(27, 39, 5), Color = CardBack, Antialiased = true });
        back.Add(new Line2D { Points = Rounded(20, 32, 3), Closed = true, Width = 2, DefaultColor = Palette.Blue });
        Diamond(back, Vector2.Zero, Palette.Blue, 9);

        var face = body.Add(new Node2D { Visible = false });
        face.Add(new Polygon2D { Polygon = Rounded(27, 39, 5), Color = Paper, Antialiased = true });
        var center = GalleryTheme.Label(rank, 26, ink);
        center.HorizontalAlignment = HorizontalAlignment.Center;
        center.Size = new Vector2(54, 36);
        center.Position = new Vector2(-27, -18);
        face.AddChild(center);
        var corner = GalleryTheme.Label(rank, 11, ink);
        corner.Position = new Vector2(-23, -38);
        face.AddChild(corner);

        return new PlayingCard(body, back, face);
    }

    protected override void Animate() => Sequence = Repeat(Deal);
}

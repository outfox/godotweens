// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using System;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using godotweens;
namespace testbed;

public sealed class CardDeal : GalleryEffect
{
    private static readonly string[] Ranks = ["10", "J", "Q", "K", "A"];
    private static readonly Vector2 DeckPosition = new(0, 170);
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

    private async Task<bool> Deal(int run)
    {
        Reset();
        return await Finished(run, Spread())
            && await FlipAll(run, faceUp: true, stagger: 0.09 * Tempo)
            && await Finished(run, LiftHero())
            && await Wait(run, 0.6 * Tempo)
            && await Finished(run, Gather())
            && await FlipAll(run, faceUp: false, stagger: 0)
            && await Finished(run, Toss());
    }

    private void Reset()
    {
        foreach (var card in deck)
        {
            card.Body.Position = DeckPosition;
            card.Body.Rotation = 0;
            card.Body.Scale = Vector2.One;
            card.Body.Modulate = Colors.White;
            card.Show(faceUp: false);
        }
    }

    /// <summary>Fans the cards out in an arc, one after another.</summary>
    private TweenInstance[] Spread() => deck.SelectMany((card, i) =>
    {
        var offset = i - (deck.Length - 1) / 2f;
        var spot = new Vector2(offset * 64, MathF.Abs(offset) * 7 + 4);
        var duration = 0.5 * Tempo;
        void Stagger(TweenOptions t) { t.Delay = i * 0.1 * Tempo; t.Ease = EaseType.BackOut; }
        return new TweenInstance[]
        {
            Keep(card.Body.TweenPosition(spot, duration, Stagger)),
            Keep(card.Body.TweenRotation(offset * 0.13f, duration, Stagger)),
        };
    }).ToArray();

    private TweenInstance[] LiftHero()
    {
        var hero = deck[^1].Body;
        var duration = 0.35 * Tempo;
        void Pop(TweenOptions t) => t.Ease = EaseType.BackOut;
        return
        [
            Keep(hero.TweenPositionY(hero.Position.Y - 26, duration, Pop)),
            Keep(hero.TweenScale(new Vector2(1.18f, 1.18f), duration, Pop)),
            Keep(hero.TweenRotation(0, duration, Pop)),
        ];
    }

    /// <summary>Stacks the cards back in the center, last card first.</summary>
    private TweenInstance[] Gather() => deck.SelectMany((card, i) =>
    {
        var duration = 0.35 * Tempo;
        void Stagger(TweenOptions t) { t.Delay = (deck.Length - 1 - i) * 0.05 * Tempo; t.Ease = EaseType.CubicInOut; }
        return new TweenInstance[]
        {
            Keep(card.Body.TweenPosition(new Vector2(0, -i * 2), duration, Stagger)),
            Keep(card.Body.TweenRotation(0, duration, Stagger)),
            Keep(card.Body.TweenScale(Vector2.One, duration, Stagger)),
        };
    }).ToArray();

    /// <summary>Throws the stack off the top of the stage.</summary>
    private TweenInstance[] Toss() => deck.SelectMany((card, i) =>
    {
        var duration = 0.45 * Tempo;
        void Stagger(TweenOptions t) { t.Delay = i * 0.04 * Tempo; t.Ease = EaseType.BackIn; }
        return new TweenInstance[]
        {
            Keep(card.Body.TweenPosition(new Vector2((i - 2) * 30, -170), duration, Stagger)),
            Keep(card.Body.TweenRotation((i - 2) * 0.4f, duration, Stagger)),
        };
    }).ToArray();

    private async Task<bool> FlipAll(int run, bool faceUp, double stagger)
    {
        var flips = await Task.WhenAll(deck.Select((card, i) => Flip(run, card, i * stagger, faceUp)));
        return flips.All(completed => completed);
    }

    /// <summary>Squeezes the card to zero width, swaps its side, then springs it back open.</summary>
    private async Task<bool> Flip(int run, PlayingCard card, double delay, bool faceUp)
    {
        var fold = Keep(card.Body.TweenScaleX(0, 0.1 * Tempo, t => { t.Delay = delay; t.Ease = EaseType.QuadIn; }));
        if (!await Finished(run, fold)) return false;
        card.Show(faceUp);
        return await Finished(run, Keep(card.Body.TweenScaleX(1, 0.22 * Tempo, t => t.Ease = EaseType.BackOut)));
    }
}

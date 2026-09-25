// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using System;
using System.Linq;
using Godot;
using tweens.gd;
namespace testbed;

// Scene setup and playback bookkeeping are in CardDeal.cs.
public sealed partial class CardDeal
{
    private static readonly Vector2 DeckPosition = new(0, 170);

    /// <summary>Fans the cards out in an arc, one after another.</summary>
    private TweenInstance[] Spread() => deck.SelectMany((card, i) =>
    {
        var offset = i - (deck.Length - 1) / 2f;
        var spot = new Vector2(offset * 64, MathF.Abs(offset) * 7 + 4);
        var duration = 0.5 * Tempo;
        void Stagger(TweenOptions t) { t.Delay = i * 0.1 * Tempo; t.Ease = EaseType.BackOut; }
        return new TweenInstance[]
        {
            card.Body.TweenPosition(spot, duration, Stagger),
            card.Body.TweenRotation(offset * 0.13f, duration, Stagger),
        };
    }).ToArray();

    private TweenInstance[] LiftHero()
    {
        var hero = deck[^1].Body;
        var duration = 0.35 * Tempo;
        void Pop(TweenOptions t) => t.Ease = EaseType.BackOut;
        return
        [
            hero.TweenPositionY(hero.Position.Y - 26, duration, Pop),
            hero.TweenScale(new Vector2(1.18f, 1.18f), duration, Pop),
            hero.TweenRotation(0, duration, Pop),
        ];
    }

    /// <summary>Stacks the cards back in the center, last card first.</summary>
    private TweenInstance[] Gather() => deck.SelectMany((card, i) =>
    {
        var duration = 0.35 * Tempo;
        void Stagger(TweenOptions t) { t.Delay = (deck.Length - 1 - i) * 0.05 * Tempo; t.Ease = EaseType.CubicInOut; }
        return new TweenInstance[]
        {
            card.Body.TweenPosition(new Vector2(0, -i * 2), duration, Stagger),
            card.Body.TweenRotation(0, duration, Stagger),
            card.Body.TweenScale(Vector2.One, duration, Stagger),
        };
    }).ToArray();

    /// <summary>Throws the stack off the top of the stage.</summary>
    private TweenInstance[] Toss() => deck.SelectMany((card, i) =>
    {
        var duration = 0.45 * Tempo;
        void Stagger(TweenOptions t) { t.Delay = i * 0.04 * Tempo; t.Ease = EaseType.BackIn; }
        return new TweenInstance[]
        {
            card.Body.TweenPosition(new Vector2((i - 2) * 30, -170), duration, Stagger),
            card.Body.TweenRotation((i - 2) * 0.4f, duration, Stagger),
        };
    }).ToArray();

    private TweenInstance Fold(PlayingCard card, double delay) =>
        card.Body.TweenScaleX(0, 0.1 * Tempo, t => { t.Delay = delay; t.Ease = EaseType.QuadIn; });

    private TweenInstance Unfold(PlayingCard card) =>
        card.Body.TweenScaleX(1, 0.22 * Tempo, t => t.Ease = EaseType.BackOut);
}

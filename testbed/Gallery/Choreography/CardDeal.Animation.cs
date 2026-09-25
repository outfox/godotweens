// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using System;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using tweens.gd;
namespace testbed;

// Scene setup is in CardDeal.cs.
public sealed partial class CardDeal
{
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

    private async Task<bool> Deal()
    {
        Reset();
        return await Spread()
            && await FlipAll(faceUp: true, stagger: 0.09 * Tempo)
            && await LiftHero()
            && await Wait(0.6 * Tempo)
            && await Gather()
            && await FlipAll(faceUp: false, stagger: 0)
            && await Toss();
    }

    private async Task<bool> FlipAll(bool faceUp, double stagger)
    {
        var flips = await Task.WhenAll(deck.Select((card, i) => Flip(card, i * stagger, faceUp)));
        return flips.All(completed => completed);
    }

    /// <summary>Squeezes the card to zero width, swaps its side, then springs it back open.</summary>
    private async Task<bool> Flip(PlayingCard card, double delay, bool faceUp)
    {
        if (await Fold(card, delay).End != Reason.Completed) return false;
        card.Show(faceUp);
        return await Unfold(card).End == Reason.Completed;
    }

    private static readonly Vector2 DeckPosition = new(0, 170);

    /// <summary>Fans the cards out in an arc, one after another.</summary>
    private async Task<bool> Spread()
    {
        var tweens = deck.SelectMany((card, i) =>
        {
            var offset = i - (deck.Length - 1) / 2f;
            var spot = new Vector2(offset * 64, MathF.Abs(offset) * 7 + 4);
            var duration = 0.5 * Tempo;
            var delay = i * 0.1 * Tempo;
            return new TweenInstance[]
            {
                card.Body.TweenPosition(spot, duration, options =>
                {
                    options.Delay = delay;
                    options.Ease = EaseType.BackOut;
                }),
                card.Body.TweenRotation(offset * 0.13f, duration, options =>
                {
                    options.Delay = delay;
                    options.Ease = EaseType.BackOut;
                }),
            };
        });
        return await Group.Of([.. tweens]).End == Reason.Completed;
    }

    private async Task<bool> LiftHero()
    {
        var hero = deck[^1].Body;
        var duration = 0.35 * Tempo;
        return await Group.Of([
            hero.TweenPositionY(hero.Position.Y - 26, duration, options => options.Ease = EaseType.BackOut),
            hero.TweenScale(new Vector2(1.18f, 1.18f), duration, options => options.Ease = EaseType.BackOut),
            hero.TweenRotation(0, duration, options => options.Ease = EaseType.BackOut),
        ]).End == Reason.Completed;
    }

    /// <summary>Stacks the cards back in the center, last card first.</summary>
    private async Task<bool> Gather()
    {
        var tweens = deck.SelectMany((card, i) =>
        {
            var duration = 0.35 * Tempo;
            var delay = (deck.Length - 1 - i) * 0.05 * Tempo;
            return new TweenInstance[]
            {
                card.Body.TweenPosition(new Vector2(0, -i * 2), duration, options =>
                {
                    options.Delay = delay;
                    options.Ease = EaseType.CubicInOut;
                }),
                card.Body.TweenRotation(0, duration, options =>
                {
                    options.Delay = delay;
                    options.Ease = EaseType.CubicInOut;
                }),
                card.Body.TweenScale(Vector2.One, duration, options =>
                {
                    options.Delay = delay;
                    options.Ease = EaseType.CubicInOut;
                }),
            };
        });
        return await Group.Of([.. tweens]).End == Reason.Completed;
    }

    /// <summary>Throws the stack off the top of the stage.</summary>
    private async Task<bool> Toss()
    {
        var tweens = deck.SelectMany((card, i) =>
        {
            var duration = 0.45 * Tempo;
            var delay = i * 0.04 * Tempo;
            return new TweenInstance[]
            {
                card.Body.TweenPosition(new Vector2((i - 2) * 30, -170), duration, options =>
                {
                    options.Delay = delay;
                    options.Ease = EaseType.BackIn;
                }),
                card.Body.TweenRotation((i - 2) * 0.4f, duration, options =>
                {
                    options.Delay = delay;
                    options.Ease = EaseType.BackIn;
                }),
            };
        });
        return await Group.Of([.. tweens]).End == Reason.Completed;
    }

    private TweenInstance Fold(PlayingCard card, double delay) =>
        card.Body.TweenScaleX(0, 0.1 * Tempo, options =>
        {
            options.Delay = delay;
            options.Ease = EaseType.QuadIn;
        });

    private TweenInstance Unfold(PlayingCard card) =>
        card.Body.TweenScaleX(1, 0.22 * Tempo, options => options.Ease = EaseType.BackOut);
}

// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using tweens.gd;
namespace testbed;

// Scene setup is in AsyncDelivery.cs.
public sealed partial class AsyncDelivery
{
    private async Task Deliver()
    {
        try
        {
            _ = Run(Report(0, "1 / Position"));
            if (await Outward().End != Reason.Completed) return;

            _ = Run(Report(1, "2 / Position + rotation"));
            if (!await ReturnAndTurn()) return;

            await Report(2, "3 / Complete");
        }
        catch (Exception error)
        {
            GD.PushError(error.ToString());
        }
    }

    private const float Left = -150, Right = 150, Rail = 15;

    private TweenInstance Outward() => courier.TweenPositionX(Right, Seconds, options => options.Ease = Ease);

    // Both tweens start together after the outward leg completes.
    private async Task<bool> ReturnAndTurn()
    {
        return await Group.Of([
            courier.TweenPositionX(Left, Seconds, options => options.Ease = Ease),
            courier.TweenRotation(Mathf.Tau, Seconds, options => options.Ease = Ease),
        ]).End == Reason.Completed;
    }

    /// <summary>Shows the step's label and lights its progress dot, and every dot before it.</summary>
    private async Task Report(int step, string text)
    {
        var tweens = new List<TweenInstance>();
        status.Text = text;
        for (var i = 0; i < steps.Length; i++)
            tweens.Add(steps[i].TweenColor(i <= step ? Palette.Mint : Palette.Outline, 0.2));
        tweens.Add(steps[step].TweenScale(Vector2.One, 0.5, options =>
        {
            options.From = new Vector2(2, 2);
            options.Ease = EaseType.ElasticOut;
        }));
        await Group.Of([.. tweens]).End;
    }
}

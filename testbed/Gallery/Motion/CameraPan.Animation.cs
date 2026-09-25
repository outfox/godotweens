// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using System.Threading.Tasks;
using Godot;
using tweens.gd;
namespace testbed;

// Scene setup is in CameraPan.cs.
public sealed partial class CameraPan
{
    private async Task AnimateAsync()
    {
        const double pulse = 1.2;
        await Group.Of([
            camera.TweenZoom(new Vector2(1.8f, 1.8f), Seconds, Cycle),
            camera.TweenOffset(new Vector2(90, 25), Seconds, Cycle),
            beacon.TweenScale(new Vector2(2.4f, 2.4f), pulse, options =>
            {
                options.From = Vector2.One;
                options.Ease = EaseType.QuartOut;
                options.Repeats = TweenOptions.Infinite;
            }),
            beacon.TweenModulateAlpha(0, pulse, options =>
            {
                options.From = 1;
                options.Ease = EaseType.QuadIn;
                options.Repeats = TweenOptions.Infinite;
            }),
        ]).End;
    }

    private void Cycle(TweenOptionsBuilder options)
    {
        options.Ease = DefaultEase;
        options.UsePingPong = true;
        options.Repeats = TweenOptions.Infinite;
        options.RepeatInterval = 0.25;
        options.PingPongInterval = 0.15;
    }
}

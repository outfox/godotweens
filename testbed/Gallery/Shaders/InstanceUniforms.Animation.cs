// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using System.Threading.Tasks;
using tweens.gd;
namespace testbed;

// Scene setup is in InstanceUniforms.cs.
public sealed partial class InstanceUniforms
{
    private readonly Tweens.CanvasItemInstanceShaderParameter<float> amount = new("amount")
    {
        Ease = DefaultEase,
        UsePingPong = true,
        Repeats = TweenOptions.Infinite,
        RepeatInterval = 0.25,
        PingPongInterval = 0.15,
    };

    private async Task AnimateAsync()
    {
        var amountDefinition = amount with { Duration = Seconds };
        await Group.Of([
            first.Tween(amountDefinition with { To = 0.85f }),
            second.Tween(amountDefinition with { To = 0.15f }),
        ]).End;
    }
}

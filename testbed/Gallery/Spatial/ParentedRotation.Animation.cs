// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using System.Threading.Tasks;
using Godot;
using tweens.gd;
namespace testbed;

// Scene setup is in ParentedRotation.cs.
public sealed partial class ParentedRotation
{
    private async Task AnimateAsync()
    {
        var orientation = Quaternion.FromEuler(new Vector3(0.5f, 2.5f, 0.8f));
        await Group.Of([
            cube.TweenGlobalQuaternion(orientation, Seconds, Cycle),
            cube.TweenScale(new Vector3(1.4f, 0.7f, 1.1f), Seconds, Cycle),
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

// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using System.Collections.Generic;
using Godot;
using tweens.gd;
namespace testbed;

// Scene setup and playback bookkeeping are in ScrollingList.cs.
public sealed partial class ScrollingList
{
    private IEnumerable<TweenInstance> CreateAnimation() =>
    [
        scroll.TweenScrollVertical(200, Seconds * 2, Cycle),
    ];
}

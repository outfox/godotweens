// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using Godot;
using GodotWeens;

namespace testbed;

// Scene script in the consumer assembly verifies bootstrap while the root is adding children.
public partial class RunnerReadyProbe : Node2D
{
    public TweenInstance<Node2D, Vector2>? Movement { get; private set; }
    public override void _Ready() => Movement = this.Tween(new Position2DTween { To = new Vector2(20, 30) });
}

// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using Godot;

namespace tweens.gd;

/// <summary>Animates AnimatedSprite2D.SpeedScale in Godot property units.</summary>
public sealed class AnimatedSprite2DSpeedScaleTween() : PropertyTween<AnimatedSprite2D, float>(
    static n => n.SpeedScale, static (n, value) => n.SpeedScale = value, Interpolators.Float);

/// <summary>Animates AnimatedSprite3D.SpeedScale in Godot property units.</summary>
public sealed class AnimatedSprite3DSpeedScaleTween() : PropertyTween<AnimatedSprite3D, float>(
    static n => n.SpeedScale, static (n, value) => n.SpeedScale = value, Interpolators.Float);

/// <summary>Animates AnimationPlayer.SpeedScale in Godot property units.</summary>
public sealed class AnimationPlayerSpeedScaleTween() : PropertyTween<AnimationPlayer, float>(
    static n => n.SpeedScale, static (n, value) => n.SpeedScale = value, Interpolators.Float);

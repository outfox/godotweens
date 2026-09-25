// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using Godot;

namespace tweens.gd;

public static partial class TweenExtensions
{
    /// <summary>Starts a AnimatedSprite2DSpeedScaleTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<AnimatedSprite2D, float> TweenSpeedScale(this AnimatedSprite2D target,
        float to, double duration, Action<AnimatedSprite2DSpeedScaleTween>? configure = null)
        => target.Tween(ConfigureDefinition(new AnimatedSprite2DSpeedScaleTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a AnimatedSprite3DSpeedScaleTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<AnimatedSprite3D, float> TweenSpeedScale(this AnimatedSprite3D target,
        float to, double duration, Action<AnimatedSprite3DSpeedScaleTween>? configure = null)
        => target.Tween(ConfigureDefinition(new AnimatedSprite3DSpeedScaleTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a AnimationPlayerSpeedScaleTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<AnimationPlayer, float> TweenSpeedScale(this AnimationPlayer target,
        float to, double duration, Action<AnimationPlayerSpeedScaleTween>? configure = null)
        => target.Tween(ConfigureDefinition(new AnimationPlayerSpeedScaleTween { To = to, Duration = duration }, configure));
}

// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using Godot;

namespace godotweens;

public static partial class TweenExtensions
{
    /// <summary>Starts a ControlPivotOffsetTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<Control, Vector2> TweenPivotOffset(this Control target,
        Vector2 to, double duration, Action<ControlPivotOffsetTween>? configure = null)
        => target.Tween(ConfigureDefinition(new ControlPivotOffsetTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a ControlPivotOffsetXTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<Control, float> TweenPivotOffsetX(this Control target,
        float to, double duration, Action<ControlPivotOffsetXTween>? configure = null)
        => target.Tween(ConfigureDefinition(new ControlPivotOffsetXTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a ControlPivotOffsetYTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<Control, float> TweenPivotOffsetY(this Control target,
        float to, double duration, Action<ControlPivotOffsetYTween>? configure = null)
        => target.Tween(ConfigureDefinition(new ControlPivotOffsetYTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a ControlPivotOffsetRatioTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<Control, Vector2> TweenPivotOffsetRatio(this Control target,
        Vector2 to, double duration, Action<ControlPivotOffsetRatioTween>? configure = null)
        => target.Tween(ConfigureDefinition(new ControlPivotOffsetRatioTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a ControlPivotOffsetRatioXTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<Control, float> TweenPivotOffsetRatioX(this Control target,
        float to, double duration, Action<ControlPivotOffsetRatioXTween>? configure = null)
        => target.Tween(ConfigureDefinition(new ControlPivotOffsetRatioXTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a ControlPivotOffsetRatioYTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<Control, float> TweenPivotOffsetRatioY(this Control target,
        float to, double duration, Action<ControlPivotOffsetRatioYTween>? configure = null)
        => target.Tween(ConfigureDefinition(new ControlPivotOffsetRatioYTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a ControlCustomMinimumSizeTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<Control, Vector2> TweenCustomMinimumSize(this Control target,
        Vector2 to, double duration, Action<ControlCustomMinimumSizeTween>? configure = null)
        => target.Tween(ConfigureDefinition(new ControlCustomMinimumSizeTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a ControlCustomMinimumSizeXTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<Control, float> TweenCustomMinimumSizeX(this Control target,
        float to, double duration, Action<ControlCustomMinimumSizeXTween>? configure = null)
        => target.Tween(ConfigureDefinition(new ControlCustomMinimumSizeXTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a ControlCustomMinimumSizeYTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<Control, float> TweenCustomMinimumSizeY(this Control target,
        float to, double duration, Action<ControlCustomMinimumSizeYTween>? configure = null)
        => target.Tween(ConfigureDefinition(new ControlCustomMinimumSizeYTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a ControlCustomMaximumSizeTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<Control, Vector2> TweenCustomMaximumSize(this Control target,
        Vector2 to, double duration, Action<ControlCustomMaximumSizeTween>? configure = null)
        => target.Tween(ConfigureDefinition(new ControlCustomMaximumSizeTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a ControlCustomMaximumSizeXTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<Control, float> TweenCustomMaximumSizeX(this Control target,
        float to, double duration, Action<ControlCustomMaximumSizeXTween>? configure = null)
        => target.Tween(ConfigureDefinition(new ControlCustomMaximumSizeXTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a ControlCustomMaximumSizeYTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<Control, float> TweenCustomMaximumSizeY(this Control target,
        float to, double duration, Action<ControlCustomMaximumSizeYTween>? configure = null)
        => target.Tween(ConfigureDefinition(new ControlCustomMaximumSizeYTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a ControlOffsetTransformPositionTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<Control, Vector2> TweenOffsetTransformPosition(this Control target,
        Vector2 to, double duration, Action<ControlOffsetTransformPositionTween>? configure = null)
        => target.Tween(ConfigureDefinition(new ControlOffsetTransformPositionTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a ControlOffsetTransformPositionXTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<Control, float> TweenOffsetTransformPositionX(this Control target,
        float to, double duration, Action<ControlOffsetTransformPositionXTween>? configure = null)
        => target.Tween(ConfigureDefinition(new ControlOffsetTransformPositionXTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a ControlOffsetTransformPositionYTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<Control, float> TweenOffsetTransformPositionY(this Control target,
        float to, double duration, Action<ControlOffsetTransformPositionYTween>? configure = null)
        => target.Tween(ConfigureDefinition(new ControlOffsetTransformPositionYTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a ControlOffsetTransformPositionRatioTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<Control, Vector2> TweenOffsetTransformPositionRatio(this Control target,
        Vector2 to, double duration, Action<ControlOffsetTransformPositionRatioTween>? configure = null)
        => target.Tween(ConfigureDefinition(new ControlOffsetTransformPositionRatioTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a ControlOffsetTransformPositionRatioXTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<Control, float> TweenOffsetTransformPositionRatioX(this Control target,
        float to, double duration, Action<ControlOffsetTransformPositionRatioXTween>? configure = null)
        => target.Tween(ConfigureDefinition(new ControlOffsetTransformPositionRatioXTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a ControlOffsetTransformPositionRatioYTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<Control, float> TweenOffsetTransformPositionRatioY(this Control target,
        float to, double duration, Action<ControlOffsetTransformPositionRatioYTween>? configure = null)
        => target.Tween(ConfigureDefinition(new ControlOffsetTransformPositionRatioYTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a ControlOffsetTransformScaleTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<Control, Vector2> TweenOffsetTransformScale(this Control target,
        Vector2 to, double duration, Action<ControlOffsetTransformScaleTween>? configure = null)
        => target.Tween(ConfigureDefinition(new ControlOffsetTransformScaleTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a ControlOffsetTransformScaleXTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<Control, float> TweenOffsetTransformScaleX(this Control target,
        float to, double duration, Action<ControlOffsetTransformScaleXTween>? configure = null)
        => target.Tween(ConfigureDefinition(new ControlOffsetTransformScaleXTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a ControlOffsetTransformScaleYTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<Control, float> TweenOffsetTransformScaleY(this Control target,
        float to, double duration, Action<ControlOffsetTransformScaleYTween>? configure = null)
        => target.Tween(ConfigureDefinition(new ControlOffsetTransformScaleYTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a ControlOffsetTransformPivotTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<Control, Vector2> TweenOffsetTransformPivot(this Control target,
        Vector2 to, double duration, Action<ControlOffsetTransformPivotTween>? configure = null)
        => target.Tween(ConfigureDefinition(new ControlOffsetTransformPivotTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a ControlOffsetTransformPivotXTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<Control, float> TweenOffsetTransformPivotX(this Control target,
        float to, double duration, Action<ControlOffsetTransformPivotXTween>? configure = null)
        => target.Tween(ConfigureDefinition(new ControlOffsetTransformPivotXTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a ControlOffsetTransformPivotYTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<Control, float> TweenOffsetTransformPivotY(this Control target,
        float to, double duration, Action<ControlOffsetTransformPivotYTween>? configure = null)
        => target.Tween(ConfigureDefinition(new ControlOffsetTransformPivotYTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a ControlOffsetTransformPivotRatioTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<Control, Vector2> TweenOffsetTransformPivotRatio(this Control target,
        Vector2 to, double duration, Action<ControlOffsetTransformPivotRatioTween>? configure = null)
        => target.Tween(ConfigureDefinition(new ControlOffsetTransformPivotRatioTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a ControlOffsetTransformPivotRatioXTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<Control, float> TweenOffsetTransformPivotRatioX(this Control target,
        float to, double duration, Action<ControlOffsetTransformPivotRatioXTween>? configure = null)
        => target.Tween(ConfigureDefinition(new ControlOffsetTransformPivotRatioXTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a ControlOffsetTransformPivotRatioYTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<Control, float> TweenOffsetTransformPivotRatioY(this Control target,
        float to, double duration, Action<ControlOffsetTransformPivotRatioYTween>? configure = null)
        => target.Tween(ConfigureDefinition(new ControlOffsetTransformPivotRatioYTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a ControlSizeFlagsStretchRatioTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<Control, float> TweenSizeFlagsStretchRatio(this Control target,
        float to, double duration, Action<ControlSizeFlagsStretchRatioTween>? configure = null)
        => target.Tween(ConfigureDefinition(new ControlSizeFlagsStretchRatioTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a ControlOffsetTransformRotationTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<Control, float> TweenOffsetTransformRotation(this Control target,
        float to, double duration, Action<ControlOffsetTransformRotationTween>? configure = null)
        => target.Tween(ConfigureDefinition(new ControlOffsetTransformRotationTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a ControlAnchorLeftTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<Control, float> TweenAnchorLeft(this Control target,
        float to, double duration, Action<ControlAnchorLeftTween>? configure = null)
        => target.Tween(ConfigureDefinition(new ControlAnchorLeftTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a ControlOffsetLeftTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<Control, float> TweenOffsetLeft(this Control target,
        float to, double duration, Action<ControlOffsetLeftTween>? configure = null)
        => target.Tween(ConfigureDefinition(new ControlOffsetLeftTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a ControlAnchorTopTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<Control, float> TweenAnchorTop(this Control target,
        float to, double duration, Action<ControlAnchorTopTween>? configure = null)
        => target.Tween(ConfigureDefinition(new ControlAnchorTopTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a ControlOffsetTopTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<Control, float> TweenOffsetTop(this Control target,
        float to, double duration, Action<ControlOffsetTopTween>? configure = null)
        => target.Tween(ConfigureDefinition(new ControlOffsetTopTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a ControlAnchorRightTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<Control, float> TweenAnchorRight(this Control target,
        float to, double duration, Action<ControlAnchorRightTween>? configure = null)
        => target.Tween(ConfigureDefinition(new ControlAnchorRightTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a ControlOffsetRightTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<Control, float> TweenOffsetRight(this Control target,
        float to, double duration, Action<ControlOffsetRightTween>? configure = null)
        => target.Tween(ConfigureDefinition(new ControlOffsetRightTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a ControlAnchorBottomTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<Control, float> TweenAnchorBottom(this Control target,
        float to, double duration, Action<ControlAnchorBottomTween>? configure = null)
        => target.Tween(ConfigureDefinition(new ControlAnchorBottomTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a ControlOffsetBottomTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<Control, float> TweenOffsetBottom(this Control target,
        float to, double duration, Action<ControlOffsetBottomTween>? configure = null)
        => target.Tween(ConfigureDefinition(new ControlOffsetBottomTween { To = to, Duration = duration }, configure));
}

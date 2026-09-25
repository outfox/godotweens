// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using Godot;

namespace tweens.gd;

public static partial class TweenExtensions
{
    /// <summary>Starts a ControlPositionTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<Control, Vector2> TweenPosition(this Control target,
        Vector2 to, double duration, Action<ControlPositionTween>? configure = null)
        => target.Tween(ConfigureDefinition(new ControlPositionTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a ControlPositionXTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<Control, float> TweenPositionX(this Control target,
        float to, double duration, Action<ControlPositionXTween>? configure = null)
        => target.Tween(ConfigureDefinition(new ControlPositionXTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a ControlPositionYTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<Control, float> TweenPositionY(this Control target,
        float to, double duration, Action<ControlPositionYTween>? configure = null)
        => target.Tween(ConfigureDefinition(new ControlPositionYTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a ControlGlobalPositionTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<Control, Vector2> TweenGlobalPosition(this Control target,
        Vector2 to, double duration, Action<ControlGlobalPositionTween>? configure = null)
        => target.Tween(ConfigureDefinition(new ControlGlobalPositionTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a ControlGlobalPositionXTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<Control, float> TweenGlobalPositionX(this Control target,
        float to, double duration, Action<ControlGlobalPositionXTween>? configure = null)
        => target.Tween(ConfigureDefinition(new ControlGlobalPositionXTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a ControlGlobalPositionYTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<Control, float> TweenGlobalPositionY(this Control target,
        float to, double duration, Action<ControlGlobalPositionYTween>? configure = null)
        => target.Tween(ConfigureDefinition(new ControlGlobalPositionYTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a ControlSizeTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<Control, Vector2> TweenSize(this Control target,
        Vector2 to, double duration, Action<ControlSizeTween>? configure = null)
        => target.Tween(ConfigureDefinition(new ControlSizeTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a ControlSizeXTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<Control, float> TweenSizeX(this Control target,
        float to, double duration, Action<ControlSizeXTween>? configure = null)
        => target.Tween(ConfigureDefinition(new ControlSizeXTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a ControlSizeYTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<Control, float> TweenSizeY(this Control target,
        float to, double duration, Action<ControlSizeYTween>? configure = null)
        => target.Tween(ConfigureDefinition(new ControlSizeYTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a ControlScaleTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<Control, Vector2> TweenScale(this Control target,
        Vector2 to, double duration, Action<ControlScaleTween>? configure = null)
        => target.Tween(ConfigureDefinition(new ControlScaleTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a ControlScaleXTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<Control, float> TweenScaleX(this Control target,
        float to, double duration, Action<ControlScaleXTween>? configure = null)
        => target.Tween(ConfigureDefinition(new ControlScaleXTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a ControlScaleYTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<Control, float> TweenScaleY(this Control target,
        float to, double duration, Action<ControlScaleYTween>? configure = null)
        => target.Tween(ConfigureDefinition(new ControlScaleYTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a ControlRotationTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<Control, float> TweenRotation(this Control target,
        float to, double duration, Action<ControlRotationTween>? configure = null)
        => target.Tween(ConfigureDefinition(new ControlRotationTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a RangeValueTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<Godot.Range, double> TweenValue(this Godot.Range target,
        double to, double duration, Action<RangeValueTween>? configure = null)
        => target.Tween(ConfigureDefinition(new RangeValueTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a ControlAnchorMinTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<Control, Vector2> TweenAnchorMin(this Control target,
        Vector2 to, double duration, Action<ControlAnchorMinTween>? configure = null)
        => target.Tween(ConfigureDefinition(new ControlAnchorMinTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a ControlAnchorMaxTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<Control, Vector2> TweenAnchorMax(this Control target,
        Vector2 to, double duration, Action<ControlAnchorMaxTween>? configure = null)
        => target.Tween(ConfigureDefinition(new ControlAnchorMaxTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a ControlOffsetsTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<Control, Vector4> TweenOffsets(this Control target,
        Vector4 to, double duration, Action<ControlOffsetsTween>? configure = null)
        => target.Tween(ConfigureDefinition(new ControlOffsetsTween { To = to, Duration = duration }, configure));
}

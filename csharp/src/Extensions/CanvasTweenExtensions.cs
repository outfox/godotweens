// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using Godot;

namespace tweens.gd;

public static partial class TweenExtensions
{
    /// <summary>Starts a CanvasLayerOffsetTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<CanvasLayer, Vector2> TweenOffset(this CanvasLayer target,
        Vector2 to, double duration, Action<CanvasLayerOffsetTween>? configure = null)
        => target.Tween(ConfigureDefinition(new CanvasLayerOffsetTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a CanvasLayerOffsetTween. Copies the options; the explicit duration takes precedence.</summary>
    public static TweenInstance<CanvasLayer, Vector2> TweenOffset(this CanvasLayer target,
        Vector2 to, double duration, TweenOptions options)
        => target.Tween(ApplyOptions(new CanvasLayerOffsetTween { To = to, Duration = duration }, options));

    /// <summary>Starts a CanvasLayerOffsetXTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<CanvasLayer, float> TweenOffsetX(this CanvasLayer target,
        float to, double duration, Action<CanvasLayerOffsetXTween>? configure = null)
        => target.Tween(ConfigureDefinition(new CanvasLayerOffsetXTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a CanvasLayerOffsetXTween. Copies the options; the explicit duration takes precedence.</summary>
    public static TweenInstance<CanvasLayer, float> TweenOffsetX(this CanvasLayer target,
        float to, double duration, TweenOptions options)
        => target.Tween(ApplyOptions(new CanvasLayerOffsetXTween { To = to, Duration = duration }, options));

    /// <summary>Starts a CanvasLayerOffsetYTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<CanvasLayer, float> TweenOffsetY(this CanvasLayer target,
        float to, double duration, Action<CanvasLayerOffsetYTween>? configure = null)
        => target.Tween(ConfigureDefinition(new CanvasLayerOffsetYTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a CanvasLayerOffsetYTween. Copies the options; the explicit duration takes precedence.</summary>
    public static TweenInstance<CanvasLayer, float> TweenOffsetY(this CanvasLayer target,
        float to, double duration, TweenOptions options)
        => target.Tween(ApplyOptions(new CanvasLayerOffsetYTween { To = to, Duration = duration }, options));

    /// <summary>Starts a CanvasLayerScaleTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<CanvasLayer, Vector2> TweenScale(this CanvasLayer target,
        Vector2 to, double duration, Action<CanvasLayerScaleTween>? configure = null)
        => target.Tween(ConfigureDefinition(new CanvasLayerScaleTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a CanvasLayerScaleTween. Copies the options; the explicit duration takes precedence.</summary>
    public static TweenInstance<CanvasLayer, Vector2> TweenScale(this CanvasLayer target,
        Vector2 to, double duration, TweenOptions options)
        => target.Tween(ApplyOptions(new CanvasLayerScaleTween { To = to, Duration = duration }, options));

    /// <summary>Starts a CanvasLayerScaleXTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<CanvasLayer, float> TweenScaleX(this CanvasLayer target,
        float to, double duration, Action<CanvasLayerScaleXTween>? configure = null)
        => target.Tween(ConfigureDefinition(new CanvasLayerScaleXTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a CanvasLayerScaleXTween. Copies the options; the explicit duration takes precedence.</summary>
    public static TweenInstance<CanvasLayer, float> TweenScaleX(this CanvasLayer target,
        float to, double duration, TweenOptions options)
        => target.Tween(ApplyOptions(new CanvasLayerScaleXTween { To = to, Duration = duration }, options));

    /// <summary>Starts a CanvasLayerScaleYTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<CanvasLayer, float> TweenScaleY(this CanvasLayer target,
        float to, double duration, Action<CanvasLayerScaleYTween>? configure = null)
        => target.Tween(ConfigureDefinition(new CanvasLayerScaleYTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a CanvasLayerScaleYTween. Copies the options; the explicit duration takes precedence.</summary>
    public static TweenInstance<CanvasLayer, float> TweenScaleY(this CanvasLayer target,
        float to, double duration, TweenOptions options)
        => target.Tween(ApplyOptions(new CanvasLayerScaleYTween { To = to, Duration = duration }, options));

    /// <summary>Starts a CanvasLayerRotationTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<CanvasLayer, float> TweenRotation(this CanvasLayer target,
        float to, double duration, Action<CanvasLayerRotationTween>? configure = null)
        => target.Tween(ConfigureDefinition(new CanvasLayerRotationTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a CanvasLayerRotationTween.
    /// Copies the options; the explicit duration takes precedence.</summary>
    public static TweenInstance<CanvasLayer, float> TweenRotation(this CanvasLayer target,
        float to, double duration, TweenOptions options)
        => target.Tween(ApplyOptions(new CanvasLayerRotationTween { To = to, Duration = duration }, options));

    /// <summary>Starts a CanvasModulateColorTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<CanvasModulate, Color> TweenColor(this CanvasModulate target,
        Color to, double duration, Action<CanvasModulateColorTween>? configure = null)
        => target.Tween(ConfigureDefinition(new CanvasModulateColorTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a CanvasModulateColorTween.
    /// Copies the options; the explicit duration takes precedence.</summary>
    public static TweenInstance<CanvasModulate, Color> TweenColor(this CanvasModulate target,
        Color to, double duration, TweenOptions options)
        => target.Tween(ApplyOptions(new CanvasModulateColorTween { To = to, Duration = duration }, options));

    /// <summary>Starts a CanvasModulateColorAlphaTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<CanvasModulate, float> TweenColorAlpha(this CanvasModulate target,
        float to, double duration, Action<CanvasModulateColorAlphaTween>? configure = null)
        => target.Tween(ConfigureDefinition(new CanvasModulateColorAlphaTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a CanvasModulateColorAlphaTween.
    /// Copies the options; the explicit duration takes precedence.</summary>
    public static TweenInstance<CanvasModulate, float> TweenColorAlpha(this CanvasModulate target,
        float to, double duration, TweenOptions options)
        => target.Tween(ApplyOptions(new CanvasModulateColorAlphaTween { To = to, Duration = duration }, options));

    /// <summary>Starts a Parallax2DScrollOffsetTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<Parallax2D, Vector2> TweenScrollOffset(this Parallax2D target,
        Vector2 to, double duration, Action<Parallax2DScrollOffsetTween>? configure = null)
        => target.Tween(ConfigureDefinition(new Parallax2DScrollOffsetTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a Parallax2DScrollOffsetTween.
    /// Copies the options; the explicit duration takes precedence.</summary>
    public static TweenInstance<Parallax2D, Vector2> TweenScrollOffset(this Parallax2D target,
        Vector2 to, double duration, TweenOptions options)
        => target.Tween(ApplyOptions(new Parallax2DScrollOffsetTween { To = to, Duration = duration }, options));

    /// <summary>Starts a Parallax2DScrollOffsetXTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<Parallax2D, float> TweenScrollOffsetX(this Parallax2D target,
        float to, double duration, Action<Parallax2DScrollOffsetXTween>? configure = null)
        => target.Tween(ConfigureDefinition(new Parallax2DScrollOffsetXTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a Parallax2DScrollOffsetXTween.
    /// Copies the options; the explicit duration takes precedence.</summary>
    public static TweenInstance<Parallax2D, float> TweenScrollOffsetX(this Parallax2D target,
        float to, double duration, TweenOptions options)
        => target.Tween(ApplyOptions(new Parallax2DScrollOffsetXTween { To = to, Duration = duration }, options));

    /// <summary>Starts a Parallax2DScrollOffsetYTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<Parallax2D, float> TweenScrollOffsetY(this Parallax2D target,
        float to, double duration, Action<Parallax2DScrollOffsetYTween>? configure = null)
        => target.Tween(ConfigureDefinition(new Parallax2DScrollOffsetYTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a Parallax2DScrollOffsetYTween.
    /// Copies the options; the explicit duration takes precedence.</summary>
    public static TweenInstance<Parallax2D, float> TweenScrollOffsetY(this Parallax2D target,
        float to, double duration, TweenOptions options)
        => target.Tween(ApplyOptions(new Parallax2DScrollOffsetYTween { To = to, Duration = duration }, options));

    /// <summary>Starts a Parallax2DScrollScaleTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<Parallax2D, Vector2> TweenScrollScale(this Parallax2D target,
        Vector2 to, double duration, Action<Parallax2DScrollScaleTween>? configure = null)
        => target.Tween(ConfigureDefinition(new Parallax2DScrollScaleTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a Parallax2DScrollScaleTween.
    /// Copies the options; the explicit duration takes precedence.</summary>
    public static TweenInstance<Parallax2D, Vector2> TweenScrollScale(this Parallax2D target,
        Vector2 to, double duration, TweenOptions options)
        => target.Tween(ApplyOptions(new Parallax2DScrollScaleTween { To = to, Duration = duration }, options));

    /// <summary>Starts a Parallax2DScrollScaleXTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<Parallax2D, float> TweenScrollScaleX(this Parallax2D target,
        float to, double duration, Action<Parallax2DScrollScaleXTween>? configure = null)
        => target.Tween(ConfigureDefinition(new Parallax2DScrollScaleXTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a Parallax2DScrollScaleXTween.
    /// Copies the options; the explicit duration takes precedence.</summary>
    public static TweenInstance<Parallax2D, float> TweenScrollScaleX(this Parallax2D target,
        float to, double duration, TweenOptions options)
        => target.Tween(ApplyOptions(new Parallax2DScrollScaleXTween { To = to, Duration = duration }, options));

    /// <summary>Starts a Parallax2DScrollScaleYTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<Parallax2D, float> TweenScrollScaleY(this Parallax2D target,
        float to, double duration, Action<Parallax2DScrollScaleYTween>? configure = null)
        => target.Tween(ConfigureDefinition(new Parallax2DScrollScaleYTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a Parallax2DScrollScaleYTween.
    /// Copies the options; the explicit duration takes precedence.</summary>
    public static TweenInstance<Parallax2D, float> TweenScrollScaleY(this Parallax2D target,
        float to, double duration, TweenOptions options)
        => target.Tween(ApplyOptions(new Parallax2DScrollScaleYTween { To = to, Duration = duration }, options));

    /// <summary>Starts a Parallax2DAutoscrollTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<Parallax2D, Vector2> TweenAutoscroll(this Parallax2D target,
        Vector2 to, double duration, Action<Parallax2DAutoscrollTween>? configure = null)
        => target.Tween(ConfigureDefinition(new Parallax2DAutoscrollTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a Parallax2DAutoscrollTween.
    /// Copies the options; the explicit duration takes precedence.</summary>
    public static TweenInstance<Parallax2D, Vector2> TweenAutoscroll(this Parallax2D target,
        Vector2 to, double duration, TweenOptions options)
        => target.Tween(ApplyOptions(new Parallax2DAutoscrollTween { To = to, Duration = duration }, options));

    /// <summary>Starts a Parallax2DAutoscrollXTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<Parallax2D, float> TweenAutoscrollX(this Parallax2D target,
        float to, double duration, Action<Parallax2DAutoscrollXTween>? configure = null)
        => target.Tween(ConfigureDefinition(new Parallax2DAutoscrollXTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a Parallax2DAutoscrollXTween.
    /// Copies the options; the explicit duration takes precedence.</summary>
    public static TweenInstance<Parallax2D, float> TweenAutoscrollX(this Parallax2D target,
        float to, double duration, TweenOptions options)
        => target.Tween(ApplyOptions(new Parallax2DAutoscrollXTween { To = to, Duration = duration }, options));

    /// <summary>Starts a Parallax2DAutoscrollYTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<Parallax2D, float> TweenAutoscrollY(this Parallax2D target,
        float to, double duration, Action<Parallax2DAutoscrollYTween>? configure = null)
        => target.Tween(ConfigureDefinition(new Parallax2DAutoscrollYTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a Parallax2DAutoscrollYTween.
    /// Copies the options; the explicit duration takes precedence.</summary>
    public static TweenInstance<Parallax2D, float> TweenAutoscrollY(this Parallax2D target,
        float to, double duration, TweenOptions options)
        => target.Tween(ApplyOptions(new Parallax2DAutoscrollYTween { To = to, Duration = duration }, options));
}

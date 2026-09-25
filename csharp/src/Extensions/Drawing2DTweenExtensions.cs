// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using Godot;

namespace tweens.gd;

public static partial class TweenExtensions
{
    /// <summary>Starts a Sprite2DOffsetTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<Sprite2D, Vector2> TweenOffset(this Sprite2D target,
        Vector2 to, double duration, Action<Sprite2DOffsetTween>? configure = null)
        => target.Tween(ConfigureDefinition(new Sprite2DOffsetTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a Sprite2DOffsetTween. Copies the options; the explicit duration takes precedence.</summary>
    public static TweenInstance<Sprite2D, Vector2> TweenOffset(this Sprite2D target,
        Vector2 to, double duration, TweenOptions options)
        => target.Tween(ApplyOptions(new Sprite2DOffsetTween { To = to, Duration = duration }, options));

    /// <summary>Starts a Sprite2DOffsetXTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<Sprite2D, float> TweenOffsetX(this Sprite2D target,
        float to, double duration, Action<Sprite2DOffsetXTween>? configure = null)
        => target.Tween(ConfigureDefinition(new Sprite2DOffsetXTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a Sprite2DOffsetXTween. Copies the options; the explicit duration takes precedence.</summary>
    public static TweenInstance<Sprite2D, float> TweenOffsetX(this Sprite2D target,
        float to, double duration, TweenOptions options)
        => target.Tween(ApplyOptions(new Sprite2DOffsetXTween { To = to, Duration = duration }, options));

    /// <summary>Starts a Sprite2DOffsetYTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<Sprite2D, float> TweenOffsetY(this Sprite2D target,
        float to, double duration, Action<Sprite2DOffsetYTween>? configure = null)
        => target.Tween(ConfigureDefinition(new Sprite2DOffsetYTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a Sprite2DOffsetYTween. Copies the options; the explicit duration takes precedence.</summary>
    public static TweenInstance<Sprite2D, float> TweenOffsetY(this Sprite2D target,
        float to, double duration, TweenOptions options)
        => target.Tween(ApplyOptions(new Sprite2DOffsetYTween { To = to, Duration = duration }, options));

    /// <summary>Starts a Sprite2DRegionRectTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<Sprite2D, Rect2> TweenRegionRect(this Sprite2D target,
        Rect2 to, double duration, Action<Sprite2DRegionRectTween>? configure = null)
        => target.Tween(ConfigureDefinition(new Sprite2DRegionRectTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a Sprite2DRegionRectTween. Copies the options; the explicit duration takes precedence.</summary>
    public static TweenInstance<Sprite2D, Rect2> TweenRegionRect(this Sprite2D target,
        Rect2 to, double duration, TweenOptions options)
        => target.Tween(ApplyOptions(new Sprite2DRegionRectTween { To = to, Duration = duration }, options));

    /// <summary>Starts a Line2DWidthTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<Line2D, float> TweenWidth(this Line2D target,
        float to, double duration, Action<Line2DWidthTween>? configure = null)
        => target.Tween(ConfigureDefinition(new Line2DWidthTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a Line2DWidthTween. Copies the options; the explicit duration takes precedence.</summary>
    public static TweenInstance<Line2D, float> TweenWidth(this Line2D target,
        float to, double duration, TweenOptions options)
        => target.Tween(ApplyOptions(new Line2DWidthTween { To = to, Duration = duration }, options));

    /// <summary>Starts a Line2DDefaultColorTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<Line2D, Color> TweenDefaultColor(this Line2D target,
        Color to, double duration, Action<Line2DDefaultColorTween>? configure = null)
        => target.Tween(ConfigureDefinition(new Line2DDefaultColorTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a Line2DDefaultColorTween. Copies the options; the explicit duration takes precedence.</summary>
    public static TweenInstance<Line2D, Color> TweenDefaultColor(this Line2D target,
        Color to, double duration, TweenOptions options)
        => target.Tween(ApplyOptions(new Line2DDefaultColorTween { To = to, Duration = duration }, options));

    /// <summary>Starts a Line2DDefaultColorAlphaTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<Line2D, float> TweenDefaultColorAlpha(this Line2D target,
        float to, double duration, Action<Line2DDefaultColorAlphaTween>? configure = null)
        => target.Tween(ConfigureDefinition(new Line2DDefaultColorAlphaTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a Line2DDefaultColorAlphaTween.
    /// Copies the options; the explicit duration takes precedence.</summary>
    public static TweenInstance<Line2D, float> TweenDefaultColorAlpha(this Line2D target,
        float to, double duration, TweenOptions options)
        => target.Tween(ApplyOptions(new Line2DDefaultColorAlphaTween { To = to, Duration = duration }, options));

    /// <summary>Starts a Polygon2DColorTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<Polygon2D, Color> TweenColor(this Polygon2D target,
        Color to, double duration, Action<Polygon2DColorTween>? configure = null)
        => target.Tween(ConfigureDefinition(new Polygon2DColorTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a Polygon2DColorTween. Copies the options; the explicit duration takes precedence.</summary>
    public static TweenInstance<Polygon2D, Color> TweenColor(this Polygon2D target,
        Color to, double duration, TweenOptions options)
        => target.Tween(ApplyOptions(new Polygon2DColorTween { To = to, Duration = duration }, options));

    /// <summary>Starts a Polygon2DColorAlphaTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<Polygon2D, float> TweenColorAlpha(this Polygon2D target,
        float to, double duration, Action<Polygon2DColorAlphaTween>? configure = null)
        => target.Tween(ConfigureDefinition(new Polygon2DColorAlphaTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a Polygon2DColorAlphaTween.
    /// Copies the options; the explicit duration takes precedence.</summary>
    public static TweenInstance<Polygon2D, float> TweenColorAlpha(this Polygon2D target,
        float to, double duration, TweenOptions options)
        => target.Tween(ApplyOptions(new Polygon2DColorAlphaTween { To = to, Duration = duration }, options));

    /// <summary>Starts a Polygon2DOffsetTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<Polygon2D, Vector2> TweenOffset(this Polygon2D target,
        Vector2 to, double duration, Action<Polygon2DOffsetTween>? configure = null)
        => target.Tween(ConfigureDefinition(new Polygon2DOffsetTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a Polygon2DOffsetTween. Copies the options; the explicit duration takes precedence.</summary>
    public static TweenInstance<Polygon2D, Vector2> TweenOffset(this Polygon2D target,
        Vector2 to, double duration, TweenOptions options)
        => target.Tween(ApplyOptions(new Polygon2DOffsetTween { To = to, Duration = duration }, options));

    /// <summary>Starts a Polygon2DOffsetXTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<Polygon2D, float> TweenOffsetX(this Polygon2D target,
        float to, double duration, Action<Polygon2DOffsetXTween>? configure = null)
        => target.Tween(ConfigureDefinition(new Polygon2DOffsetXTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a Polygon2DOffsetXTween. Copies the options; the explicit duration takes precedence.</summary>
    public static TweenInstance<Polygon2D, float> TweenOffsetX(this Polygon2D target,
        float to, double duration, TweenOptions options)
        => target.Tween(ApplyOptions(new Polygon2DOffsetXTween { To = to, Duration = duration }, options));

    /// <summary>Starts a Polygon2DOffsetYTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<Polygon2D, float> TweenOffsetY(this Polygon2D target,
        float to, double duration, Action<Polygon2DOffsetYTween>? configure = null)
        => target.Tween(ConfigureDefinition(new Polygon2DOffsetYTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a Polygon2DOffsetYTween. Copies the options; the explicit duration takes precedence.</summary>
    public static TweenInstance<Polygon2D, float> TweenOffsetY(this Polygon2D target,
        float to, double duration, TweenOptions options)
        => target.Tween(ApplyOptions(new Polygon2DOffsetYTween { To = to, Duration = duration }, options));

    /// <summary>Starts a Polygon2DTextureOffsetTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<Polygon2D, Vector2> TweenTextureOffset(this Polygon2D target,
        Vector2 to, double duration, Action<Polygon2DTextureOffsetTween>? configure = null)
        => target.Tween(ConfigureDefinition(new Polygon2DTextureOffsetTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a Polygon2DTextureOffsetTween.
    /// Copies the options; the explicit duration takes precedence.</summary>
    public static TweenInstance<Polygon2D, Vector2> TweenTextureOffset(this Polygon2D target,
        Vector2 to, double duration, TweenOptions options)
        => target.Tween(ApplyOptions(new Polygon2DTextureOffsetTween { To = to, Duration = duration }, options));

    /// <summary>Starts a Polygon2DTextureOffsetXTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<Polygon2D, float> TweenTextureOffsetX(this Polygon2D target,
        float to, double duration, Action<Polygon2DTextureOffsetXTween>? configure = null)
        => target.Tween(ConfigureDefinition(new Polygon2DTextureOffsetXTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a Polygon2DTextureOffsetXTween.
    /// Copies the options; the explicit duration takes precedence.</summary>
    public static TweenInstance<Polygon2D, float> TweenTextureOffsetX(this Polygon2D target,
        float to, double duration, TweenOptions options)
        => target.Tween(ApplyOptions(new Polygon2DTextureOffsetXTween { To = to, Duration = duration }, options));

    /// <summary>Starts a Polygon2DTextureOffsetYTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<Polygon2D, float> TweenTextureOffsetY(this Polygon2D target,
        float to, double duration, Action<Polygon2DTextureOffsetYTween>? configure = null)
        => target.Tween(ConfigureDefinition(new Polygon2DTextureOffsetYTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a Polygon2DTextureOffsetYTween.
    /// Copies the options; the explicit duration takes precedence.</summary>
    public static TweenInstance<Polygon2D, float> TweenTextureOffsetY(this Polygon2D target,
        float to, double duration, TweenOptions options)
        => target.Tween(ApplyOptions(new Polygon2DTextureOffsetYTween { To = to, Duration = duration }, options));

    /// <summary>Starts a Polygon2DTextureScaleTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<Polygon2D, Vector2> TweenTextureScale(this Polygon2D target,
        Vector2 to, double duration, Action<Polygon2DTextureScaleTween>? configure = null)
        => target.Tween(ConfigureDefinition(new Polygon2DTextureScaleTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a Polygon2DTextureScaleTween.
    /// Copies the options; the explicit duration takes precedence.</summary>
    public static TweenInstance<Polygon2D, Vector2> TweenTextureScale(this Polygon2D target,
        Vector2 to, double duration, TweenOptions options)
        => target.Tween(ApplyOptions(new Polygon2DTextureScaleTween { To = to, Duration = duration }, options));

    /// <summary>Starts a Polygon2DTextureScaleXTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<Polygon2D, float> TweenTextureScaleX(this Polygon2D target,
        float to, double duration, Action<Polygon2DTextureScaleXTween>? configure = null)
        => target.Tween(ConfigureDefinition(new Polygon2DTextureScaleXTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a Polygon2DTextureScaleXTween.
    /// Copies the options; the explicit duration takes precedence.</summary>
    public static TweenInstance<Polygon2D, float> TweenTextureScaleX(this Polygon2D target,
        float to, double duration, TweenOptions options)
        => target.Tween(ApplyOptions(new Polygon2DTextureScaleXTween { To = to, Duration = duration }, options));

    /// <summary>Starts a Polygon2DTextureScaleYTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<Polygon2D, float> TweenTextureScaleY(this Polygon2D target,
        float to, double duration, Action<Polygon2DTextureScaleYTween>? configure = null)
        => target.Tween(ConfigureDefinition(new Polygon2DTextureScaleYTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a Polygon2DTextureScaleYTween.
    /// Copies the options; the explicit duration takes precedence.</summary>
    public static TweenInstance<Polygon2D, float> TweenTextureScaleY(this Polygon2D target,
        float to, double duration, TweenOptions options)
        => target.Tween(ApplyOptions(new Polygon2DTextureScaleYTween { To = to, Duration = duration }, options));

    /// <summary>Starts a Polygon2DTextureRotationTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<Polygon2D, float> TweenTextureRotation(this Polygon2D target,
        float to, double duration, Action<Polygon2DTextureRotationTween>? configure = null)
        => target.Tween(ConfigureDefinition(new Polygon2DTextureRotationTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a Polygon2DTextureRotationTween.
    /// Copies the options; the explicit duration takes precedence.</summary>
    public static TweenInstance<Polygon2D, float> TweenTextureRotation(this Polygon2D target,
        float to, double duration, TweenOptions options)
        => target.Tween(ApplyOptions(new Polygon2DTextureRotationTween { To = to, Duration = duration }, options));
}

// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using Godot;

namespace tweens.gd;

public static partial class TweenExtensions
{
    /// <summary>Starts a Camera2DZoomTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<Camera2D, Vector2> TweenZoom(this Camera2D target,
        Vector2 to, double duration, Action<Camera2DZoomTween>? configure = null)
        => target.Tween(ConfigureDefinition(new Camera2DZoomTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a Camera2DZoomTween. Copies the options; the explicit duration takes precedence.</summary>
    public static TweenInstance<Camera2D, Vector2> TweenZoom(this Camera2D target,
        Vector2 to, double duration, TweenOptions options)
        => target.Tween(ApplyOptions(new Camera2DZoomTween { To = to, Duration = duration }, options));

    /// <summary>Starts a Camera2DZoomXTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<Camera2D, float> TweenZoomX(this Camera2D target,
        float to, double duration, Action<Camera2DZoomXTween>? configure = null)
        => target.Tween(ConfigureDefinition(new Camera2DZoomXTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a Camera2DZoomXTween. Copies the options; the explicit duration takes precedence.</summary>
    public static TweenInstance<Camera2D, float> TweenZoomX(this Camera2D target,
        float to, double duration, TweenOptions options)
        => target.Tween(ApplyOptions(new Camera2DZoomXTween { To = to, Duration = duration }, options));

    /// <summary>Starts a Camera2DZoomYTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<Camera2D, float> TweenZoomY(this Camera2D target,
        float to, double duration, Action<Camera2DZoomYTween>? configure = null)
        => target.Tween(ConfigureDefinition(new Camera2DZoomYTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a Camera2DZoomYTween. Copies the options; the explicit duration takes precedence.</summary>
    public static TweenInstance<Camera2D, float> TweenZoomY(this Camera2D target,
        float to, double duration, TweenOptions options)
        => target.Tween(ApplyOptions(new Camera2DZoomYTween { To = to, Duration = duration }, options));

    /// <summary>Starts a Camera2DOffsetTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<Camera2D, Vector2> TweenOffset(this Camera2D target,
        Vector2 to, double duration, Action<Camera2DOffsetTween>? configure = null)
        => target.Tween(ConfigureDefinition(new Camera2DOffsetTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a Camera2DOffsetTween. Copies the options; the explicit duration takes precedence.</summary>
    public static TweenInstance<Camera2D, Vector2> TweenOffset(this Camera2D target,
        Vector2 to, double duration, TweenOptions options)
        => target.Tween(ApplyOptions(new Camera2DOffsetTween { To = to, Duration = duration }, options));

    /// <summary>Starts a Camera2DOffsetXTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<Camera2D, float> TweenOffsetX(this Camera2D target,
        float to, double duration, Action<Camera2DOffsetXTween>? configure = null)
        => target.Tween(ConfigureDefinition(new Camera2DOffsetXTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a Camera2DOffsetXTween. Copies the options; the explicit duration takes precedence.</summary>
    public static TweenInstance<Camera2D, float> TweenOffsetX(this Camera2D target,
        float to, double duration, TweenOptions options)
        => target.Tween(ApplyOptions(new Camera2DOffsetXTween { To = to, Duration = duration }, options));

    /// <summary>Starts a Camera2DOffsetYTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<Camera2D, float> TweenOffsetY(this Camera2D target,
        float to, double duration, Action<Camera2DOffsetYTween>? configure = null)
        => target.Tween(ConfigureDefinition(new Camera2DOffsetYTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a Camera2DOffsetYTween. Copies the options; the explicit duration takes precedence.</summary>
    public static TweenInstance<Camera2D, float> TweenOffsetY(this Camera2D target,
        float to, double duration, TweenOptions options)
        => target.Tween(ApplyOptions(new Camera2DOffsetYTween { To = to, Duration = duration }, options));

    /// <summary>Starts a Camera3DFovTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<Camera3D, float> TweenFov(this Camera3D target,
        float to, double duration, Action<Camera3DFovTween>? configure = null)
        => target.Tween(ConfigureDefinition(new Camera3DFovTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a Camera3DFovTween. Copies the options; the explicit duration takes precedence.</summary>
    public static TweenInstance<Camera3D, float> TweenFov(this Camera3D target,
        float to, double duration, TweenOptions options)
        => target.Tween(ApplyOptions(new Camera3DFovTween { To = to, Duration = duration }, options));

    /// <summary>Starts a Camera3DSizeTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<Camera3D, float> TweenSize(this Camera3D target,
        float to, double duration, Action<Camera3DSizeTween>? configure = null)
        => target.Tween(ConfigureDefinition(new Camera3DSizeTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a Camera3DSizeTween. Copies the options; the explicit duration takes precedence.</summary>
    public static TweenInstance<Camera3D, float> TweenSize(this Camera3D target,
        float to, double duration, TweenOptions options)
        => target.Tween(ApplyOptions(new Camera3DSizeTween { To = to, Duration = duration }, options));

    /// <summary>Starts a Camera3DHOffsetTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<Camera3D, float> TweenHOffset(this Camera3D target,
        float to, double duration, Action<Camera3DHOffsetTween>? configure = null)
        => target.Tween(ConfigureDefinition(new Camera3DHOffsetTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a Camera3DHOffsetTween. Copies the options; the explicit duration takes precedence.</summary>
    public static TweenInstance<Camera3D, float> TweenHOffset(this Camera3D target,
        float to, double duration, TweenOptions options)
        => target.Tween(ApplyOptions(new Camera3DHOffsetTween { To = to, Duration = duration }, options));

    /// <summary>Starts a Camera3DVOffsetTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<Camera3D, float> TweenVOffset(this Camera3D target,
        float to, double duration, Action<Camera3DVOffsetTween>? configure = null)
        => target.Tween(ConfigureDefinition(new Camera3DVOffsetTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a Camera3DVOffsetTween. Copies the options; the explicit duration takes precedence.</summary>
    public static TweenInstance<Camera3D, float> TweenVOffset(this Camera3D target,
        float to, double duration, TweenOptions options)
        => target.Tween(ApplyOptions(new Camera3DVOffsetTween { To = to, Duration = duration }, options));

    /// <summary>Starts a Camera3DNearTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<Camera3D, float> TweenNear(this Camera3D target,
        float to, double duration, Action<Camera3DNearTween>? configure = null)
        => target.Tween(ConfigureDefinition(new Camera3DNearTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a Camera3DNearTween. Copies the options; the explicit duration takes precedence.</summary>
    public static TweenInstance<Camera3D, float> TweenNear(this Camera3D target,
        float to, double duration, TweenOptions options)
        => target.Tween(ApplyOptions(new Camera3DNearTween { To = to, Duration = duration }, options));

    /// <summary>Starts a Camera3DFarTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<Camera3D, float> TweenFar(this Camera3D target,
        float to, double duration, Action<Camera3DFarTween>? configure = null)
        => target.Tween(ConfigureDefinition(new Camera3DFarTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a Camera3DFarTween. Copies the options; the explicit duration takes precedence.</summary>
    public static TweenInstance<Camera3D, float> TweenFar(this Camera3D target,
        float to, double duration, TweenOptions options)
        => target.Tween(ApplyOptions(new Camera3DFarTween { To = to, Duration = duration }, options));

    /// <summary>Starts a Camera3DFrustumOffsetTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<Camera3D, Vector2> TweenFrustumOffset(this Camera3D target,
        Vector2 to, double duration, Action<Camera3DFrustumOffsetTween>? configure = null)
        => target.Tween(ConfigureDefinition(new Camera3DFrustumOffsetTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a Camera3DFrustumOffsetTween.
    /// Copies the options; the explicit duration takes precedence.</summary>
    public static TweenInstance<Camera3D, Vector2> TweenFrustumOffset(this Camera3D target,
        Vector2 to, double duration, TweenOptions options)
        => target.Tween(ApplyOptions(new Camera3DFrustumOffsetTween { To = to, Duration = duration }, options));

    /// <summary>Starts a Camera3DFrustumOffsetXTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<Camera3D, float> TweenFrustumOffsetX(this Camera3D target,
        float to, double duration, Action<Camera3DFrustumOffsetXTween>? configure = null)
        => target.Tween(ConfigureDefinition(new Camera3DFrustumOffsetXTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a Camera3DFrustumOffsetXTween.
    /// Copies the options; the explicit duration takes precedence.</summary>
    public static TweenInstance<Camera3D, float> TweenFrustumOffsetX(this Camera3D target,
        float to, double duration, TweenOptions options)
        => target.Tween(ApplyOptions(new Camera3DFrustumOffsetXTween { To = to, Duration = duration }, options));

    /// <summary>Starts a Camera3DFrustumOffsetYTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<Camera3D, float> TweenFrustumOffsetY(this Camera3D target,
        float to, double duration, Action<Camera3DFrustumOffsetYTween>? configure = null)
        => target.Tween(ConfigureDefinition(new Camera3DFrustumOffsetYTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a Camera3DFrustumOffsetYTween.
    /// Copies the options; the explicit duration takes precedence.</summary>
    public static TweenInstance<Camera3D, float> TweenFrustumOffsetY(this Camera3D target,
        float to, double duration, TweenOptions options)
        => target.Tween(ApplyOptions(new Camera3DFrustumOffsetYTween { To = to, Duration = duration }, options));
}

// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using Godot;

namespace tweens.gd;

public static partial class TweenExtensions
{
    /// <summary>Starts a Skew2DTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<Node2D, float> TweenSkew(this Node2D target,
        float to, double duration, Action<Skew2DTween>? configure = null)
        => target.Tween(ConfigureDefinition(new Skew2DTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a Skew2DTween. Copies the options; the explicit duration takes precedence.</summary>
    public static TweenInstance<Node2D, float> TweenSkew(this Node2D target,
        float to, double duration, TweenOptions options)
        => target.Tween(ApplyOptions(new Skew2DTween { To = to, Duration = duration }, options));

    /// <summary>Starts a GlobalSkew2DTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<Node2D, float> TweenGlobalSkew(this Node2D target,
        float to, double duration, Action<GlobalSkew2DTween>? configure = null)
        => target.Tween(ConfigureDefinition(new GlobalSkew2DTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a GlobalSkew2DTween. Copies the options; the explicit duration takes precedence.</summary>
    public static TweenInstance<Node2D, float> TweenGlobalSkew(this Node2D target,
        float to, double duration, TweenOptions options)
        => target.Tween(ApplyOptions(new GlobalSkew2DTween { To = to, Duration = duration }, options));

    /// <summary>Starts a GlobalScale2DTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<Node2D, Vector2> TweenGlobalScale(this Node2D target,
        Vector2 to, double duration, Action<GlobalScale2DTween>? configure = null)
        => target.Tween(ConfigureDefinition(new GlobalScale2DTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a GlobalScale2DTween. Copies the options; the explicit duration takes precedence.</summary>
    public static TweenInstance<Node2D, Vector2> TweenGlobalScale(this Node2D target,
        Vector2 to, double duration, TweenOptions options)
        => target.Tween(ApplyOptions(new GlobalScale2DTween { To = to, Duration = duration }, options));

    /// <summary>Starts a GlobalScale2DXTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<Node2D, float> TweenGlobalScaleX(this Node2D target,
        float to, double duration, Action<GlobalScale2DXTween>? configure = null)
        => target.Tween(ConfigureDefinition(new GlobalScale2DXTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a GlobalScale2DXTween. Copies the options; the explicit duration takes precedence.</summary>
    public static TweenInstance<Node2D, float> TweenGlobalScaleX(this Node2D target,
        float to, double duration, TweenOptions options)
        => target.Tween(ApplyOptions(new GlobalScale2DXTween { To = to, Duration = duration }, options));

    /// <summary>Starts a GlobalScale2DYTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<Node2D, float> TweenGlobalScaleY(this Node2D target,
        float to, double duration, Action<GlobalScale2DYTween>? configure = null)
        => target.Tween(ConfigureDefinition(new GlobalScale2DYTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a GlobalScale2DYTween. Copies the options; the explicit duration takes precedence.</summary>
    public static TweenInstance<Node2D, float> TweenGlobalScaleY(this Node2D target,
        float to, double duration, TweenOptions options)
        => target.Tween(ApplyOptions(new GlobalScale2DYTween { To = to, Duration = duration }, options));

    /// <summary>Starts a GlobalQuaternion3DTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<Node3D, Quaternion> TweenGlobalQuaternion(this Node3D target,
        Quaternion to, double duration, Action<GlobalQuaternion3DTween>? configure = null)
        => target.Tween(ConfigureDefinition(new GlobalQuaternion3DTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a GlobalQuaternion3DTween. Copies the options; the explicit duration takes precedence.</summary>
    public static TweenInstance<Node3D, Quaternion> TweenGlobalQuaternion(this Node3D target,
        Quaternion to, double duration, TweenOptions options)
        => target.Tween(ApplyOptions(new GlobalQuaternion3DTween { To = to, Duration = duration }, options));
}

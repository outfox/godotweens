// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using Godot;

namespace godotweens;

public static partial class TweenExtensions
{
    /// <summary>Starts a FloatTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<Node, float> TweenFloat(this Node target,
        float to, double duration, Action<FloatTween>? configure = null)
        => target.Tween(ConfigureDefinition(new FloatTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a DoubleTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<Node, double> TweenDouble(this Node target,
        double to, double duration, Action<DoubleTween>? configure = null)
        => target.Tween(ConfigureDefinition(new DoubleTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a Vector2Tween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<Node, Vector2> TweenVector2(this Node target,
        Vector2 to, double duration, Action<Vector2Tween>? configure = null)
        => target.Tween(ConfigureDefinition(new Vector2Tween { To = to, Duration = duration }, configure));

    /// <summary>Starts a Vector3Tween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<Node, Vector3> TweenVector3(this Node target,
        Vector3 to, double duration, Action<Vector3Tween>? configure = null)
        => target.Tween(ConfigureDefinition(new Vector3Tween { To = to, Duration = duration }, configure));

    /// <summary>Starts a Vector4Tween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<Node, Vector4> TweenVector4(this Node target,
        Vector4 to, double duration, Action<Vector4Tween>? configure = null)
        => target.Tween(ConfigureDefinition(new Vector4Tween { To = to, Duration = duration }, configure));

    /// <summary>Starts a ColorTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<Node, Color> TweenColor(this Node target,
        Color to, double duration, Action<ColorTween>? configure = null)
        => target.Tween(ConfigureDefinition(new ColorTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a QuaternionTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<Node, Quaternion> TweenQuaternion(this Node target,
        Quaternion to, double duration, Action<QuaternionTween>? configure = null)
        => target.Tween(ConfigureDefinition(new QuaternionTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a Rect2Tween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<Node, Rect2> TweenRect2(this Node target,
        Rect2 to, double duration, Action<Rect2Tween>? configure = null)
        => target.Tween(ConfigureDefinition(new Rect2Tween { To = to, Duration = duration }, configure));
}

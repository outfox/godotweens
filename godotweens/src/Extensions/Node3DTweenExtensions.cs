// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using Godot;

namespace godotweens;

public static partial class TweenExtensions
{
    /// <summary>Starts a Position3DTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<Node3D, Vector3> TweenPosition(this Node3D target,
        Vector3 to, double duration, Action<Position3DTween>? configure = null)
        => target.Tween(ConfigureDefinition(new Position3DTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a Position3DXTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<Node3D, float> TweenPositionX(this Node3D target,
        float to, double duration, Action<Position3DXTween>? configure = null)
        => target.Tween(ConfigureDefinition(new Position3DXTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a Position3DYTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<Node3D, float> TweenPositionY(this Node3D target,
        float to, double duration, Action<Position3DYTween>? configure = null)
        => target.Tween(ConfigureDefinition(new Position3DYTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a Position3DZTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<Node3D, float> TweenPositionZ(this Node3D target,
        float to, double duration, Action<Position3DZTween>? configure = null)
        => target.Tween(ConfigureDefinition(new Position3DZTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a GlobalPosition3DTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<Node3D, Vector3> TweenGlobalPosition(this Node3D target,
        Vector3 to, double duration, Action<GlobalPosition3DTween>? configure = null)
        => target.Tween(ConfigureDefinition(new GlobalPosition3DTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a GlobalPosition3DXTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<Node3D, float> TweenGlobalPositionX(this Node3D target,
        float to, double duration, Action<GlobalPosition3DXTween>? configure = null)
        => target.Tween(ConfigureDefinition(new GlobalPosition3DXTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a GlobalPosition3DYTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<Node3D, float> TweenGlobalPositionY(this Node3D target,
        float to, double duration, Action<GlobalPosition3DYTween>? configure = null)
        => target.Tween(ConfigureDefinition(new GlobalPosition3DYTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a GlobalPosition3DZTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<Node3D, float> TweenGlobalPositionZ(this Node3D target,
        float to, double duration, Action<GlobalPosition3DZTween>? configure = null)
        => target.Tween(ConfigureDefinition(new GlobalPosition3DZTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a Scale3DTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<Node3D, Vector3> TweenScale(this Node3D target,
        Vector3 to, double duration, Action<Scale3DTween>? configure = null)
        => target.Tween(ConfigureDefinition(new Scale3DTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a Scale3DXTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<Node3D, float> TweenScaleX(this Node3D target,
        float to, double duration, Action<Scale3DXTween>? configure = null)
        => target.Tween(ConfigureDefinition(new Scale3DXTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a Scale3DYTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<Node3D, float> TweenScaleY(this Node3D target,
        float to, double duration, Action<Scale3DYTween>? configure = null)
        => target.Tween(ConfigureDefinition(new Scale3DYTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a Scale3DZTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<Node3D, float> TweenScaleZ(this Node3D target,
        float to, double duration, Action<Scale3DZTween>? configure = null)
        => target.Tween(ConfigureDefinition(new Scale3DZTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a Rotation3DTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<Node3D, Vector3> TweenRotation(this Node3D target,
        Vector3 to, double duration, Action<Rotation3DTween>? configure = null)
        => target.Tween(ConfigureDefinition(new Rotation3DTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a Rotation3DXTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<Node3D, float> TweenRotationX(this Node3D target,
        float to, double duration, Action<Rotation3DXTween>? configure = null)
        => target.Tween(ConfigureDefinition(new Rotation3DXTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a Rotation3DYTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<Node3D, float> TweenRotationY(this Node3D target,
        float to, double duration, Action<Rotation3DYTween>? configure = null)
        => target.Tween(ConfigureDefinition(new Rotation3DYTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a Rotation3DZTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<Node3D, float> TweenRotationZ(this Node3D target,
        float to, double duration, Action<Rotation3DZTween>? configure = null)
        => target.Tween(ConfigureDefinition(new Rotation3DZTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a GlobalRotation3DTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<Node3D, Vector3> TweenGlobalRotation(this Node3D target,
        Vector3 to, double duration, Action<GlobalRotation3DTween>? configure = null)
        => target.Tween(ConfigureDefinition(new GlobalRotation3DTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a GlobalRotation3DXTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<Node3D, float> TweenGlobalRotationX(this Node3D target,
        float to, double duration, Action<GlobalRotation3DXTween>? configure = null)
        => target.Tween(ConfigureDefinition(new GlobalRotation3DXTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a GlobalRotation3DYTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<Node3D, float> TweenGlobalRotationY(this Node3D target,
        float to, double duration, Action<GlobalRotation3DYTween>? configure = null)
        => target.Tween(ConfigureDefinition(new GlobalRotation3DYTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a GlobalRotation3DZTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<Node3D, float> TweenGlobalRotationZ(this Node3D target,
        float to, double duration, Action<GlobalRotation3DZTween>? configure = null)
        => target.Tween(ConfigureDefinition(new GlobalRotation3DZTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a Quaternion3DTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<Node3D, Quaternion> TweenQuaternion(this Node3D target,
        Quaternion to, double duration, Action<Quaternion3DTween>? configure = null)
        => target.Tween(ConfigureDefinition(new Quaternion3DTween { To = to, Duration = duration }, configure));
}

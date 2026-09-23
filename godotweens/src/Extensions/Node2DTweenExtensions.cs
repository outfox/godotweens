// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using Godot;

namespace godotweens;

public static partial class TweenExtensions
{
    /// <summary>Starts a Position2DTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<Node2D, Vector2> TweenPosition(this Node2D target,
        Vector2 to, double duration, Action<Position2DTween>? configure = null)
        => target.Tween(ConfigureDefinition(new Position2DTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a Position2DXTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<Node2D, float> TweenPositionX(this Node2D target,
        float to, double duration, Action<Position2DXTween>? configure = null)
        => target.Tween(ConfigureDefinition(new Position2DXTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a Position2DYTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<Node2D, float> TweenPositionY(this Node2D target,
        float to, double duration, Action<Position2DYTween>? configure = null)
        => target.Tween(ConfigureDefinition(new Position2DYTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a GlobalPosition2DTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<Node2D, Vector2> TweenGlobalPosition(this Node2D target,
        Vector2 to, double duration, Action<GlobalPosition2DTween>? configure = null)
        => target.Tween(ConfigureDefinition(new GlobalPosition2DTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a GlobalPosition2DXTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<Node2D, float> TweenGlobalPositionX(this Node2D target,
        float to, double duration, Action<GlobalPosition2DXTween>? configure = null)
        => target.Tween(ConfigureDefinition(new GlobalPosition2DXTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a GlobalPosition2DYTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<Node2D, float> TweenGlobalPositionY(this Node2D target,
        float to, double duration, Action<GlobalPosition2DYTween>? configure = null)
        => target.Tween(ConfigureDefinition(new GlobalPosition2DYTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a Scale2DTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<Node2D, Vector2> TweenScale(this Node2D target,
        Vector2 to, double duration, Action<Scale2DTween>? configure = null)
        => target.Tween(ConfigureDefinition(new Scale2DTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a Scale2DXTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<Node2D, float> TweenScaleX(this Node2D target,
        float to, double duration, Action<Scale2DXTween>? configure = null)
        => target.Tween(ConfigureDefinition(new Scale2DXTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a Scale2DYTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<Node2D, float> TweenScaleY(this Node2D target,
        float to, double duration, Action<Scale2DYTween>? configure = null)
        => target.Tween(ConfigureDefinition(new Scale2DYTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a Rotation2DTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<Node2D, float> TweenRotation(this Node2D target,
        float to, double duration, Action<Rotation2DTween>? configure = null)
        => target.Tween(ConfigureDefinition(new Rotation2DTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a GlobalRotation2DTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<Node2D, float> TweenGlobalRotation(this Node2D target,
        float to, double duration, Action<GlobalRotation2DTween>? configure = null)
        => target.Tween(ConfigureDefinition(new GlobalRotation2DTween { To = to, Duration = duration }, configure));
}

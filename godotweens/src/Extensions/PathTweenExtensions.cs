// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using Godot;

namespace godotweens;

public static partial class TweenExtensions
{
    /// <summary>Starts a PathFollow2DProgressTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<PathFollow2D, float> TweenProgress(this PathFollow2D target,
        float to, double duration, Action<PathFollow2DProgressTween>? configure = null)
        => target.Tween(ConfigureDefinition(new PathFollow2DProgressTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a PathFollow2DProgressRatioTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<PathFollow2D, float> TweenProgressRatio(this PathFollow2D target,
        float to, double duration, Action<PathFollow2DProgressRatioTween>? configure = null)
        => target.Tween(ConfigureDefinition(new PathFollow2DProgressRatioTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a PathFollow2DHOffsetTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<PathFollow2D, float> TweenHOffset(this PathFollow2D target,
        float to, double duration, Action<PathFollow2DHOffsetTween>? configure = null)
        => target.Tween(ConfigureDefinition(new PathFollow2DHOffsetTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a PathFollow2DVOffsetTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<PathFollow2D, float> TweenVOffset(this PathFollow2D target,
        float to, double duration, Action<PathFollow2DVOffsetTween>? configure = null)
        => target.Tween(ConfigureDefinition(new PathFollow2DVOffsetTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a PathFollow3DProgressTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<PathFollow3D, float> TweenProgress(this PathFollow3D target,
        float to, double duration, Action<PathFollow3DProgressTween>? configure = null)
        => target.Tween(ConfigureDefinition(new PathFollow3DProgressTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a PathFollow3DProgressRatioTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<PathFollow3D, float> TweenProgressRatio(this PathFollow3D target,
        float to, double duration, Action<PathFollow3DProgressRatioTween>? configure = null)
        => target.Tween(ConfigureDefinition(new PathFollow3DProgressRatioTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a PathFollow3DHOffsetTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<PathFollow3D, float> TweenHOffset(this PathFollow3D target,
        float to, double duration, Action<PathFollow3DHOffsetTween>? configure = null)
        => target.Tween(ConfigureDefinition(new PathFollow3DHOffsetTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a PathFollow3DVOffsetTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<PathFollow3D, float> TweenVOffset(this PathFollow3D target,
        float to, double duration, Action<PathFollow3DVOffsetTween>? configure = null)
        => target.Tween(ConfigureDefinition(new PathFollow3DVOffsetTween { To = to, Duration = duration }, configure));
}

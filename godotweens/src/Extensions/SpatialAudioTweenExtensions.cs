// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using Godot;

namespace godotweens;

public static partial class TweenExtensions
{
    /// <summary>Starts a AudioStreamPlayer2DPanningStrengthTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<AudioStreamPlayer2D, float> TweenPanningStrength(this AudioStreamPlayer2D target,
        float to, double duration, Action<AudioStreamPlayer2DPanningStrengthTween>? configure = null)
        => target.Tween(ConfigureDefinition(new AudioStreamPlayer2DPanningStrengthTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a AudioStreamPlayer2DMaxDistanceTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<AudioStreamPlayer2D, float> TweenMaxDistance(this AudioStreamPlayer2D target,
        float to, double duration, Action<AudioStreamPlayer2DMaxDistanceTween>? configure = null)
        => target.Tween(ConfigureDefinition(new AudioStreamPlayer2DMaxDistanceTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a AudioStreamPlayer3DPanningStrengthTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<AudioStreamPlayer3D, float> TweenPanningStrength(this AudioStreamPlayer3D target,
        float to, double duration, Action<AudioStreamPlayer3DPanningStrengthTween>? configure = null)
        => target.Tween(ConfigureDefinition(new AudioStreamPlayer3DPanningStrengthTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a AudioStreamPlayer3DMaxDistanceTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<AudioStreamPlayer3D, float> TweenMaxDistance(this AudioStreamPlayer3D target,
        float to, double duration, Action<AudioStreamPlayer3DMaxDistanceTween>? configure = null)
        => target.Tween(ConfigureDefinition(new AudioStreamPlayer3DMaxDistanceTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a AudioStreamPlayer2DAttenuationTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<AudioStreamPlayer2D, float> TweenAttenuation(this AudioStreamPlayer2D target,
        float to, double duration, Action<AudioStreamPlayer2DAttenuationTween>? configure = null)
        => target.Tween(ConfigureDefinition(new AudioStreamPlayer2DAttenuationTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a AudioStreamPlayer3DUnitSizeTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<AudioStreamPlayer3D, float> TweenUnitSize(this AudioStreamPlayer3D target,
        float to, double duration, Action<AudioStreamPlayer3DUnitSizeTween>? configure = null)
        => target.Tween(ConfigureDefinition(new AudioStreamPlayer3DUnitSizeTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a AudioStreamPlayer3DEmissionAngleDegreesTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<AudioStreamPlayer3D, float> TweenEmissionAngleDegrees(this AudioStreamPlayer3D target,
        float to, double duration, Action<AudioStreamPlayer3DEmissionAngleDegreesTween>? configure = null)
        => target.Tween(ConfigureDefinition(new AudioStreamPlayer3DEmissionAngleDegreesTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a AudioStreamPlayer3DEmissionAngleFilterAttenuationDbTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<AudioStreamPlayer3D, float> TweenEmissionAngleFilterAttenuationDb(this AudioStreamPlayer3D target,
        float to, double duration, Action<AudioStreamPlayer3DEmissionAngleFilterAttenuationDbTween>? configure = null)
        => target.Tween(ConfigureDefinition(new AudioStreamPlayer3DEmissionAngleFilterAttenuationDbTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a AudioStreamPlayer3DAttenuationFilterCutoffHzTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<AudioStreamPlayer3D, float> TweenAttenuationFilterCutoffHz(this AudioStreamPlayer3D target,
        float to, double duration, Action<AudioStreamPlayer3DAttenuationFilterCutoffHzTween>? configure = null)
        => target.Tween(ConfigureDefinition(new AudioStreamPlayer3DAttenuationFilterCutoffHzTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a AudioStreamPlayer3DAttenuationFilterDbTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<AudioStreamPlayer3D, float> TweenAttenuationFilterDb(this AudioStreamPlayer3D target,
        float to, double duration, Action<AudioStreamPlayer3DAttenuationFilterDbTween>? configure = null)
        => target.Tween(ConfigureDefinition(new AudioStreamPlayer3DAttenuationFilterDbTween { To = to, Duration = duration }, configure));
}

// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using Godot;

namespace godotweens;

public static partial class TweenExtensions
{
    /// <summary>Starts a AudioVolumeDbTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<AudioStreamPlayer, float> TweenVolumeDb(this AudioStreamPlayer target,
        float to, double duration, Action<AudioVolumeDbTween>? configure = null)
        => target.Tween(ConfigureDefinition(new AudioVolumeDbTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a AudioVolumeLinearTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<AudioStreamPlayer, float> TweenVolumeLinear(this AudioStreamPlayer target,
        float to, double duration, Action<AudioVolumeLinearTween>? configure = null)
        => target.Tween(ConfigureDefinition(new AudioVolumeLinearTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a AudioPitchScaleTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<AudioStreamPlayer, float> TweenPitchScale(this AudioStreamPlayer target,
        float to, double duration, Action<AudioPitchScaleTween>? configure = null)
        => target.Tween(ConfigureDefinition(new AudioPitchScaleTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a AudioVolumeDb2DTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<AudioStreamPlayer2D, float> TweenVolumeDb(this AudioStreamPlayer2D target,
        float to, double duration, Action<AudioVolumeDb2DTween>? configure = null)
        => target.Tween(ConfigureDefinition(new AudioVolumeDb2DTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a AudioVolumeLinear2DTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<AudioStreamPlayer2D, float> TweenVolumeLinear(this AudioStreamPlayer2D target,
        float to, double duration, Action<AudioVolumeLinear2DTween>? configure = null)
        => target.Tween(ConfigureDefinition(new AudioVolumeLinear2DTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a AudioPitchScale2DTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<AudioStreamPlayer2D, float> TweenPitchScale(this AudioStreamPlayer2D target,
        float to, double duration, Action<AudioPitchScale2DTween>? configure = null)
        => target.Tween(ConfigureDefinition(new AudioPitchScale2DTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a AudioVolumeDb3DTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<AudioStreamPlayer3D, float> TweenVolumeDb(this AudioStreamPlayer3D target,
        float to, double duration, Action<AudioVolumeDb3DTween>? configure = null)
        => target.Tween(ConfigureDefinition(new AudioVolumeDb3DTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a AudioVolumeLinear3DTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<AudioStreamPlayer3D, float> TweenVolumeLinear(this AudioStreamPlayer3D target,
        float to, double duration, Action<AudioVolumeLinear3DTween>? configure = null)
        => target.Tween(ConfigureDefinition(new AudioVolumeLinear3DTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a AudioPitchScale3DTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<AudioStreamPlayer3D, float> TweenPitchScale(this AudioStreamPlayer3D target,
        float to, double duration, Action<AudioPitchScale3DTween>? configure = null)
        => target.Tween(ConfigureDefinition(new AudioPitchScale3DTween { To = to, Duration = duration }, configure));
}

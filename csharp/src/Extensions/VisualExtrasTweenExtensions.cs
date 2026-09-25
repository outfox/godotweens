// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using Godot;

namespace tweens.gd;

public static partial class TweenExtensions
{
    /// <summary>Starts a SpringArm3DSpringLengthTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<SpringArm3D, float> TweenSpringLength(this SpringArm3D target,
        float to, double duration, Action<SpringArm3DSpringLengthTween>? configure = null)
        => target.Tween(ConfigureDefinition(new SpringArm3DSpringLengthTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a SpringArm3DSpringLengthTween.
    /// Copies the options; the explicit duration takes precedence.</summary>
    public static TweenInstance<SpringArm3D, float> TweenSpringLength(this SpringArm3D target,
        float to, double duration, TweenOptions options)
        => target.Tween(ApplyOptions(new SpringArm3DSpringLengthTween { To = to, Duration = duration }, options));

    /// <summary>Starts a DecalModulateTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<Decal, Color> TweenModulate(this Decal target,
        Color to, double duration, Action<DecalModulateTween>? configure = null)
        => target.Tween(ConfigureDefinition(new DecalModulateTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a DecalModulateTween. Copies the options; the explicit duration takes precedence.</summary>
    public static TweenInstance<Decal, Color> TweenModulate(this Decal target,
        Color to, double duration, TweenOptions options)
        => target.Tween(ApplyOptions(new DecalModulateTween { To = to, Duration = duration }, options));

    /// <summary>Starts a DecalModulateAlphaTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<Decal, float> TweenModulateAlpha(this Decal target,
        float to, double duration, Action<DecalModulateAlphaTween>? configure = null)
        => target.Tween(ConfigureDefinition(new DecalModulateAlphaTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a DecalModulateAlphaTween. Copies the options; the explicit duration takes precedence.</summary>
    public static TweenInstance<Decal, float> TweenModulateAlpha(this Decal target,
        float to, double duration, TweenOptions options)
        => target.Tween(ApplyOptions(new DecalModulateAlphaTween { To = to, Duration = duration }, options));

    /// <summary>Starts a DecalSizeTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<Decal, Vector3> TweenSize(this Decal target,
        Vector3 to, double duration, Action<DecalSizeTween>? configure = null)
        => target.Tween(ConfigureDefinition(new DecalSizeTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a DecalSizeTween. Copies the options; the explicit duration takes precedence.</summary>
    public static TweenInstance<Decal, Vector3> TweenSize(this Decal target,
        Vector3 to, double duration, TweenOptions options)
        => target.Tween(ApplyOptions(new DecalSizeTween { To = to, Duration = duration }, options));

    /// <summary>Starts a DecalSizeXTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<Decal, float> TweenSizeX(this Decal target,
        float to, double duration, Action<DecalSizeXTween>? configure = null)
        => target.Tween(ConfigureDefinition(new DecalSizeXTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a DecalSizeXTween. Copies the options; the explicit duration takes precedence.</summary>
    public static TweenInstance<Decal, float> TweenSizeX(this Decal target,
        float to, double duration, TweenOptions options)
        => target.Tween(ApplyOptions(new DecalSizeXTween { To = to, Duration = duration }, options));

    /// <summary>Starts a DecalSizeYTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<Decal, float> TweenSizeY(this Decal target,
        float to, double duration, Action<DecalSizeYTween>? configure = null)
        => target.Tween(ConfigureDefinition(new DecalSizeYTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a DecalSizeYTween. Copies the options; the explicit duration takes precedence.</summary>
    public static TweenInstance<Decal, float> TweenSizeY(this Decal target,
        float to, double duration, TweenOptions options)
        => target.Tween(ApplyOptions(new DecalSizeYTween { To = to, Duration = duration }, options));

    /// <summary>Starts a DecalSizeZTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<Decal, float> TweenSizeZ(this Decal target,
        float to, double duration, Action<DecalSizeZTween>? configure = null)
        => target.Tween(ConfigureDefinition(new DecalSizeZTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a DecalSizeZTween. Copies the options; the explicit duration takes precedence.</summary>
    public static TweenInstance<Decal, float> TweenSizeZ(this Decal target,
        float to, double duration, TweenOptions options)
        => target.Tween(ApplyOptions(new DecalSizeZTween { To = to, Duration = duration }, options));

    /// <summary>Starts a DecalEmissionEnergyTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<Decal, float> TweenEmissionEnergy(this Decal target,
        float to, double duration, Action<DecalEmissionEnergyTween>? configure = null)
        => target.Tween(ConfigureDefinition(new DecalEmissionEnergyTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a DecalEmissionEnergyTween.
    /// Copies the options; the explicit duration takes precedence.</summary>
    public static TweenInstance<Decal, float> TweenEmissionEnergy(this Decal target,
        float to, double duration, TweenOptions options)
        => target.Tween(ApplyOptions(new DecalEmissionEnergyTween { To = to, Duration = duration }, options));

    /// <summary>Starts a FogVolumeSizeTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<FogVolume, Vector3> TweenSize(this FogVolume target,
        Vector3 to, double duration, Action<FogVolumeSizeTween>? configure = null)
        => target.Tween(ConfigureDefinition(new FogVolumeSizeTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a FogVolumeSizeTween. Copies the options; the explicit duration takes precedence.</summary>
    public static TweenInstance<FogVolume, Vector3> TweenSize(this FogVolume target,
        Vector3 to, double duration, TweenOptions options)
        => target.Tween(ApplyOptions(new FogVolumeSizeTween { To = to, Duration = duration }, options));

    /// <summary>Starts a FogVolumeSizeXTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<FogVolume, float> TweenSizeX(this FogVolume target,
        float to, double duration, Action<FogVolumeSizeXTween>? configure = null)
        => target.Tween(ConfigureDefinition(new FogVolumeSizeXTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a FogVolumeSizeXTween. Copies the options; the explicit duration takes precedence.</summary>
    public static TweenInstance<FogVolume, float> TweenSizeX(this FogVolume target,
        float to, double duration, TweenOptions options)
        => target.Tween(ApplyOptions(new FogVolumeSizeXTween { To = to, Duration = duration }, options));

    /// <summary>Starts a FogVolumeSizeYTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<FogVolume, float> TweenSizeY(this FogVolume target,
        float to, double duration, Action<FogVolumeSizeYTween>? configure = null)
        => target.Tween(ConfigureDefinition(new FogVolumeSizeYTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a FogVolumeSizeYTween. Copies the options; the explicit duration takes precedence.</summary>
    public static TweenInstance<FogVolume, float> TweenSizeY(this FogVolume target,
        float to, double duration, TweenOptions options)
        => target.Tween(ApplyOptions(new FogVolumeSizeYTween { To = to, Duration = duration }, options));

    /// <summary>Starts a FogVolumeSizeZTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<FogVolume, float> TweenSizeZ(this FogVolume target,
        float to, double duration, Action<FogVolumeSizeZTween>? configure = null)
        => target.Tween(ConfigureDefinition(new FogVolumeSizeZTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a FogVolumeSizeZTween. Copies the options; the explicit duration takes precedence.</summary>
    public static TweenInstance<FogVolume, float> TweenSizeZ(this FogVolume target,
        float to, double duration, TweenOptions options)
        => target.Tween(ApplyOptions(new FogVolumeSizeZTween { To = to, Duration = duration }, options));
}

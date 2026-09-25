// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using Godot;

namespace tweens.gd;

public static partial class TweenExtensions
{
    /// <summary>Starts a PointLight2DTextureScaleTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<PointLight2D, float> TweenTextureScale(this PointLight2D target,
        float to, double duration, Action<PointLight2DTextureScaleTween>? configure = null)
        => target.Tween(ConfigureDefinition(new PointLight2DTextureScaleTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a PointLight2DTextureScaleTween.
    /// Copies the options; the explicit duration takes precedence.</summary>
    public static TweenInstance<PointLight2D, float> TweenTextureScale(this PointLight2D target,
        float to, double duration, TweenOptions options)
        => target.Tween(ApplyOptions(new PointLight2DTextureScaleTween { To = to, Duration = duration }, options));

    /// <summary>Starts a PointLight2DHeightTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<PointLight2D, float> TweenHeight(this PointLight2D target,
        float to, double duration, Action<PointLight2DHeightTween>? configure = null)
        => target.Tween(ConfigureDefinition(new PointLight2DHeightTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a PointLight2DHeightTween. Copies the options; the explicit duration takes precedence.</summary>
    public static TweenInstance<PointLight2D, float> TweenHeight(this PointLight2D target,
        float to, double duration, TweenOptions options)
        => target.Tween(ApplyOptions(new PointLight2DHeightTween { To = to, Duration = duration }, options));

    /// <summary>Starts a PointLight2DOffsetTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<PointLight2D, Vector2> TweenOffset(this PointLight2D target,
        Vector2 to, double duration, Action<PointLight2DOffsetTween>? configure = null)
        => target.Tween(ConfigureDefinition(new PointLight2DOffsetTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a PointLight2DOffsetTween. Copies the options; the explicit duration takes precedence.</summary>
    public static TweenInstance<PointLight2D, Vector2> TweenOffset(this PointLight2D target,
        Vector2 to, double duration, TweenOptions options)
        => target.Tween(ApplyOptions(new PointLight2DOffsetTween { To = to, Duration = duration }, options));

    /// <summary>Starts a PointLight2DOffsetXTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<PointLight2D, float> TweenOffsetX(this PointLight2D target,
        float to, double duration, Action<PointLight2DOffsetXTween>? configure = null)
        => target.Tween(ConfigureDefinition(new PointLight2DOffsetXTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a PointLight2DOffsetXTween.
    /// Copies the options; the explicit duration takes precedence.</summary>
    public static TweenInstance<PointLight2D, float> TweenOffsetX(this PointLight2D target,
        float to, double duration, TweenOptions options)
        => target.Tween(ApplyOptions(new PointLight2DOffsetXTween { To = to, Duration = duration }, options));

    /// <summary>Starts a PointLight2DOffsetYTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<PointLight2D, float> TweenOffsetY(this PointLight2D target,
        float to, double duration, Action<PointLight2DOffsetYTween>? configure = null)
        => target.Tween(ConfigureDefinition(new PointLight2DOffsetYTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a PointLight2DOffsetYTween.
    /// Copies the options; the explicit duration takes precedence.</summary>
    public static TweenInstance<PointLight2D, float> TweenOffsetY(this PointLight2D target,
        float to, double duration, TweenOptions options)
        => target.Tween(ApplyOptions(new PointLight2DOffsetYTween { To = to, Duration = duration }, options));

    /// <summary>Starts a Light2DShadowColorTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<Light2D, Color> TweenShadowColor(this Light2D target,
        Color to, double duration, Action<Light2DShadowColorTween>? configure = null)
        => target.Tween(ConfigureDefinition(new Light2DShadowColorTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a Light2DShadowColorTween. Copies the options; the explicit duration takes precedence.</summary>
    public static TweenInstance<Light2D, Color> TweenShadowColor(this Light2D target,
        Color to, double duration, TweenOptions options)
        => target.Tween(ApplyOptions(new Light2DShadowColorTween { To = to, Duration = duration }, options));

    /// <summary>Starts a Light2DShadowColorAlphaTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<Light2D, float> TweenShadowColorAlpha(this Light2D target,
        float to, double duration, Action<Light2DShadowColorAlphaTween>? configure = null)
        => target.Tween(ConfigureDefinition(new Light2DShadowColorAlphaTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a Light2DShadowColorAlphaTween.
    /// Copies the options; the explicit duration takes precedence.</summary>
    public static TweenInstance<Light2D, float> TweenShadowColorAlpha(this Light2D target,
        float to, double duration, TweenOptions options)
        => target.Tween(ApplyOptions(new Light2DShadowColorAlphaTween { To = to, Duration = duration }, options));

    /// <summary>Starts a Light3DLightTemperatureTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<Light3D, float> TweenLightTemperature(this Light3D target,
        float to, double duration, Action<Light3DLightTemperatureTween>? configure = null)
        => target.Tween(ConfigureDefinition(new Light3DLightTemperatureTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a Light3DLightTemperatureTween.
    /// Copies the options; the explicit duration takes precedence.</summary>
    public static TweenInstance<Light3D, float> TweenLightTemperature(this Light3D target,
        float to, double duration, TweenOptions options)
        => target.Tween(ApplyOptions(new Light3DLightTemperatureTween { To = to, Duration = duration }, options));

    /// <summary>Starts a Light3DLightIndirectEnergyTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<Light3D, float> TweenLightIndirectEnergy(this Light3D target,
        float to, double duration, Action<Light3DLightIndirectEnergyTween>? configure = null)
        => target.Tween(ConfigureDefinition(new Light3DLightIndirectEnergyTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a Light3DLightIndirectEnergyTween.
    /// Copies the options; the explicit duration takes precedence.</summary>
    public static TweenInstance<Light3D, float> TweenLightIndirectEnergy(this Light3D target,
        float to, double duration, TweenOptions options)
        => target.Tween(ApplyOptions(new Light3DLightIndirectEnergyTween { To = to, Duration = duration }, options));

    /// <summary>Starts a Light3DLightVolumetricFogEnergyTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<Light3D, float> TweenLightVolumetricFogEnergy(this Light3D target,
        float to, double duration, Action<Light3DLightVolumetricFogEnergyTween>? configure = null)
        => target.Tween(ConfigureDefinition(new Light3DLightVolumetricFogEnergyTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a Light3DLightVolumetricFogEnergyTween.
    /// Copies the options; the explicit duration takes precedence.</summary>
    public static TweenInstance<Light3D, float> TweenLightVolumetricFogEnergy(this Light3D target,
        float to, double duration, TweenOptions options)
        => target.Tween(ApplyOptions(new Light3DLightVolumetricFogEnergyTween { To = to, Duration = duration }, options));

    /// <summary>Starts a Light3DShadowOpacityTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<Light3D, float> TweenShadowOpacity(this Light3D target,
        float to, double duration, Action<Light3DShadowOpacityTween>? configure = null)
        => target.Tween(ConfigureDefinition(new Light3DShadowOpacityTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a Light3DShadowOpacityTween.
    /// Copies the options; the explicit duration takes precedence.</summary>
    public static TweenInstance<Light3D, float> TweenShadowOpacity(this Light3D target,
        float to, double duration, TweenOptions options)
        => target.Tween(ApplyOptions(new Light3DShadowOpacityTween { To = to, Duration = duration }, options));

    /// <summary>Starts a OmniLight3DOmniAttenuationTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<OmniLight3D, float> TweenOmniAttenuation(this OmniLight3D target,
        float to, double duration, Action<OmniLight3DOmniAttenuationTween>? configure = null)
        => target.Tween(ConfigureDefinition(new OmniLight3DOmniAttenuationTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a OmniLight3DOmniAttenuationTween.
    /// Copies the options; the explicit duration takes precedence.</summary>
    public static TweenInstance<OmniLight3D, float> TweenOmniAttenuation(this OmniLight3D target,
        float to, double duration, TweenOptions options)
        => target.Tween(ApplyOptions(new OmniLight3DOmniAttenuationTween { To = to, Duration = duration }, options));

    /// <summary>Starts a SpotLight3DSpotAttenuationTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<SpotLight3D, float> TweenSpotAttenuation(this SpotLight3D target,
        float to, double duration, Action<SpotLight3DSpotAttenuationTween>? configure = null)
        => target.Tween(ConfigureDefinition(new SpotLight3DSpotAttenuationTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a SpotLight3DSpotAttenuationTween.
    /// Copies the options; the explicit duration takes precedence.</summary>
    public static TweenInstance<SpotLight3D, float> TweenSpotAttenuation(this SpotLight3D target,
        float to, double duration, TweenOptions options)
        => target.Tween(ApplyOptions(new SpotLight3DSpotAttenuationTween { To = to, Duration = duration }, options));

    /// <summary>Starts a SpotLight3DSpotAngleAttenuationTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<SpotLight3D, float> TweenSpotAngleAttenuation(this SpotLight3D target,
        float to, double duration, Action<SpotLight3DSpotAngleAttenuationTween>? configure = null)
        => target.Tween(ConfigureDefinition(new SpotLight3DSpotAngleAttenuationTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a SpotLight3DSpotAngleAttenuationTween.
    /// Copies the options; the explicit duration takes precedence.</summary>
    public static TweenInstance<SpotLight3D, float> TweenSpotAngleAttenuation(this SpotLight3D target,
        float to, double duration, TweenOptions options)
        => target.Tween(ApplyOptions(new SpotLight3DSpotAngleAttenuationTween { To = to, Duration = duration }, options));
}

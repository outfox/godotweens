// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using Godot;

namespace godotweens;

public static partial class TweenExtensions
{
    /// <summary>Starts a LightColor2DTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<Light2D, Color> TweenColor(this Light2D target,
        Color to, double duration, Action<LightColor2DTween>? configure = null)
        => target.Tween(ConfigureDefinition(new LightColor2DTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a LightEnergy2DTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<Light2D, float> TweenEnergy(this Light2D target,
        float to, double duration, Action<LightEnergy2DTween>? configure = null)
        => target.Tween(ConfigureDefinition(new LightEnergy2DTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a LightColor3DTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<Light3D, Color> TweenLightColor(this Light3D target,
        Color to, double duration, Action<LightColor3DTween>? configure = null)
        => target.Tween(ConfigureDefinition(new LightColor3DTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a LightEnergy3DTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<Light3D, float> TweenLightEnergy(this Light3D target,
        float to, double duration, Action<LightEnergy3DTween>? configure = null)
        => target.Tween(ConfigureDefinition(new LightEnergy3DTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a OmniRangeTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<OmniLight3D, float> TweenOmniRange(this OmniLight3D target,
        float to, double duration, Action<OmniRangeTween>? configure = null)
        => target.Tween(ConfigureDefinition(new OmniRangeTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a SpotRangeTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<SpotLight3D, float> TweenSpotRange(this SpotLight3D target,
        float to, double duration, Action<SpotRangeTween>? configure = null)
        => target.Tween(ConfigureDefinition(new SpotRangeTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a SpotAngleTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<SpotLight3D, float> TweenSpotAngle(this SpotLight3D target,
        float to, double duration, Action<SpotAngleTween>? configure = null)
        => target.Tween(ConfigureDefinition(new SpotAngleTween { To = to, Duration = duration }, configure));
}

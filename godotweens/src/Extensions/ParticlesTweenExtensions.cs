// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using Godot;

namespace godotweens;

public static partial class TweenExtensions
{
    /// <summary>Starts a GpuParticles2DSpeedScaleTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<GpuParticles2D, double> TweenSpeedScale(this GpuParticles2D target,
        double to, double duration, Action<GpuParticles2DSpeedScaleTween>? configure = null)
        => target.Tween(ConfigureDefinition(new GpuParticles2DSpeedScaleTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a GpuParticles2DLifetimeTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<GpuParticles2D, double> TweenLifetime(this GpuParticles2D target,
        double to, double duration, Action<GpuParticles2DLifetimeTween>? configure = null)
        => target.Tween(ConfigureDefinition(new GpuParticles2DLifetimeTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a GpuParticles2DExplosivenessTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<GpuParticles2D, float> TweenExplosiveness(this GpuParticles2D target,
        float to, double duration, Action<GpuParticles2DExplosivenessTween>? configure = null)
        => target.Tween(ConfigureDefinition(new GpuParticles2DExplosivenessTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a GpuParticles2DRandomnessTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<GpuParticles2D, float> TweenRandomness(this GpuParticles2D target,
        float to, double duration, Action<GpuParticles2DRandomnessTween>? configure = null)
        => target.Tween(ConfigureDefinition(new GpuParticles2DRandomnessTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a GpuParticles2DAmountRatioTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<GpuParticles2D, float> TweenAmountRatio(this GpuParticles2D target,
        float to, double duration, Action<GpuParticles2DAmountRatioTween>? configure = null)
        => target.Tween(ConfigureDefinition(new GpuParticles2DAmountRatioTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a GpuParticles3DSpeedScaleTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<GpuParticles3D, double> TweenSpeedScale(this GpuParticles3D target,
        double to, double duration, Action<GpuParticles3DSpeedScaleTween>? configure = null)
        => target.Tween(ConfigureDefinition(new GpuParticles3DSpeedScaleTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a GpuParticles3DLifetimeTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<GpuParticles3D, double> TweenLifetime(this GpuParticles3D target,
        double to, double duration, Action<GpuParticles3DLifetimeTween>? configure = null)
        => target.Tween(ConfigureDefinition(new GpuParticles3DLifetimeTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a GpuParticles3DExplosivenessTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<GpuParticles3D, float> TweenExplosiveness(this GpuParticles3D target,
        float to, double duration, Action<GpuParticles3DExplosivenessTween>? configure = null)
        => target.Tween(ConfigureDefinition(new GpuParticles3DExplosivenessTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a GpuParticles3DRandomnessTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<GpuParticles3D, float> TweenRandomness(this GpuParticles3D target,
        float to, double duration, Action<GpuParticles3DRandomnessTween>? configure = null)
        => target.Tween(ConfigureDefinition(new GpuParticles3DRandomnessTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a GpuParticles3DAmountRatioTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<GpuParticles3D, float> TweenAmountRatio(this GpuParticles3D target,
        float to, double duration, Action<GpuParticles3DAmountRatioTween>? configure = null)
        => target.Tween(ConfigureDefinition(new GpuParticles3DAmountRatioTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a CpuParticles2DSpeedScaleTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<CpuParticles2D, double> TweenSpeedScale(this CpuParticles2D target,
        double to, double duration, Action<CpuParticles2DSpeedScaleTween>? configure = null)
        => target.Tween(ConfigureDefinition(new CpuParticles2DSpeedScaleTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a CpuParticles2DLifetimeTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<CpuParticles2D, double> TweenLifetime(this CpuParticles2D target,
        double to, double duration, Action<CpuParticles2DLifetimeTween>? configure = null)
        => target.Tween(ConfigureDefinition(new CpuParticles2DLifetimeTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a CpuParticles2DExplosivenessTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<CpuParticles2D, float> TweenExplosiveness(this CpuParticles2D target,
        float to, double duration, Action<CpuParticles2DExplosivenessTween>? configure = null)
        => target.Tween(ConfigureDefinition(new CpuParticles2DExplosivenessTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a CpuParticles2DRandomnessTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<CpuParticles2D, float> TweenRandomness(this CpuParticles2D target,
        float to, double duration, Action<CpuParticles2DRandomnessTween>? configure = null)
        => target.Tween(ConfigureDefinition(new CpuParticles2DRandomnessTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a CpuParticles2DDirectionTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<CpuParticles2D, Vector2> TweenDirection(this CpuParticles2D target,
        Vector2 to, double duration, Action<CpuParticles2DDirectionTween>? configure = null)
        => target.Tween(ConfigureDefinition(new CpuParticles2DDirectionTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a CpuParticles2DDirectionXTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<CpuParticles2D, float> TweenDirectionX(this CpuParticles2D target,
        float to, double duration, Action<CpuParticles2DDirectionXTween>? configure = null)
        => target.Tween(ConfigureDefinition(new CpuParticles2DDirectionXTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a CpuParticles2DDirectionYTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<CpuParticles2D, float> TweenDirectionY(this CpuParticles2D target,
        float to, double duration, Action<CpuParticles2DDirectionYTween>? configure = null)
        => target.Tween(ConfigureDefinition(new CpuParticles2DDirectionYTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a CpuParticles2DGravityTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<CpuParticles2D, Vector2> TweenGravity(this CpuParticles2D target,
        Vector2 to, double duration, Action<CpuParticles2DGravityTween>? configure = null)
        => target.Tween(ConfigureDefinition(new CpuParticles2DGravityTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a CpuParticles2DGravityXTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<CpuParticles2D, float> TweenGravityX(this CpuParticles2D target,
        float to, double duration, Action<CpuParticles2DGravityXTween>? configure = null)
        => target.Tween(ConfigureDefinition(new CpuParticles2DGravityXTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a CpuParticles2DGravityYTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<CpuParticles2D, float> TweenGravityY(this CpuParticles2D target,
        float to, double duration, Action<CpuParticles2DGravityYTween>? configure = null)
        => target.Tween(ConfigureDefinition(new CpuParticles2DGravityYTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a CpuParticles2DSpreadTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<CpuParticles2D, float> TweenSpread(this CpuParticles2D target,
        float to, double duration, Action<CpuParticles2DSpreadTween>? configure = null)
        => target.Tween(ConfigureDefinition(new CpuParticles2DSpreadTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a CpuParticles2DEmissionSphereRadiusTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<CpuParticles2D, float> TweenEmissionSphereRadius(this CpuParticles2D target,
        float to, double duration, Action<CpuParticles2DEmissionSphereRadiusTween>? configure = null)
        => target.Tween(ConfigureDefinition(new CpuParticles2DEmissionSphereRadiusTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a CpuParticles2DEmissionRectExtentsTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<CpuParticles2D, Vector2> TweenEmissionRectExtents(this CpuParticles2D target,
        Vector2 to, double duration, Action<CpuParticles2DEmissionRectExtentsTween>? configure = null)
        => target.Tween(ConfigureDefinition(new CpuParticles2DEmissionRectExtentsTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a CpuParticles2DEmissionRectExtentsXTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<CpuParticles2D, float> TweenEmissionRectExtentsX(this CpuParticles2D target,
        float to, double duration, Action<CpuParticles2DEmissionRectExtentsXTween>? configure = null)
        => target.Tween(ConfigureDefinition(new CpuParticles2DEmissionRectExtentsXTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a CpuParticles2DEmissionRectExtentsYTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<CpuParticles2D, float> TweenEmissionRectExtentsY(this CpuParticles2D target,
        float to, double duration, Action<CpuParticles2DEmissionRectExtentsYTween>? configure = null)
        => target.Tween(ConfigureDefinition(new CpuParticles2DEmissionRectExtentsYTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a CpuParticles2DColorTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<CpuParticles2D, Color> TweenColor(this CpuParticles2D target,
        Color to, double duration, Action<CpuParticles2DColorTween>? configure = null)
        => target.Tween(ConfigureDefinition(new CpuParticles2DColorTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a CpuParticles2DColorAlphaTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<CpuParticles2D, float> TweenColorAlpha(this CpuParticles2D target,
        float to, double duration, Action<CpuParticles2DColorAlphaTween>? configure = null)
        => target.Tween(ConfigureDefinition(new CpuParticles2DColorAlphaTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a CpuParticles3DSpeedScaleTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<CpuParticles3D, double> TweenSpeedScale(this CpuParticles3D target,
        double to, double duration, Action<CpuParticles3DSpeedScaleTween>? configure = null)
        => target.Tween(ConfigureDefinition(new CpuParticles3DSpeedScaleTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a CpuParticles3DLifetimeTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<CpuParticles3D, double> TweenLifetime(this CpuParticles3D target,
        double to, double duration, Action<CpuParticles3DLifetimeTween>? configure = null)
        => target.Tween(ConfigureDefinition(new CpuParticles3DLifetimeTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a CpuParticles3DExplosivenessTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<CpuParticles3D, float> TweenExplosiveness(this CpuParticles3D target,
        float to, double duration, Action<CpuParticles3DExplosivenessTween>? configure = null)
        => target.Tween(ConfigureDefinition(new CpuParticles3DExplosivenessTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a CpuParticles3DRandomnessTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<CpuParticles3D, float> TweenRandomness(this CpuParticles3D target,
        float to, double duration, Action<CpuParticles3DRandomnessTween>? configure = null)
        => target.Tween(ConfigureDefinition(new CpuParticles3DRandomnessTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a CpuParticles3DDirectionTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<CpuParticles3D, Vector3> TweenDirection(this CpuParticles3D target,
        Vector3 to, double duration, Action<CpuParticles3DDirectionTween>? configure = null)
        => target.Tween(ConfigureDefinition(new CpuParticles3DDirectionTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a CpuParticles3DDirectionXTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<CpuParticles3D, float> TweenDirectionX(this CpuParticles3D target,
        float to, double duration, Action<CpuParticles3DDirectionXTween>? configure = null)
        => target.Tween(ConfigureDefinition(new CpuParticles3DDirectionXTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a CpuParticles3DDirectionYTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<CpuParticles3D, float> TweenDirectionY(this CpuParticles3D target,
        float to, double duration, Action<CpuParticles3DDirectionYTween>? configure = null)
        => target.Tween(ConfigureDefinition(new CpuParticles3DDirectionYTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a CpuParticles3DDirectionZTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<CpuParticles3D, float> TweenDirectionZ(this CpuParticles3D target,
        float to, double duration, Action<CpuParticles3DDirectionZTween>? configure = null)
        => target.Tween(ConfigureDefinition(new CpuParticles3DDirectionZTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a CpuParticles3DGravityTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<CpuParticles3D, Vector3> TweenGravity(this CpuParticles3D target,
        Vector3 to, double duration, Action<CpuParticles3DGravityTween>? configure = null)
        => target.Tween(ConfigureDefinition(new CpuParticles3DGravityTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a CpuParticles3DGravityXTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<CpuParticles3D, float> TweenGravityX(this CpuParticles3D target,
        float to, double duration, Action<CpuParticles3DGravityXTween>? configure = null)
        => target.Tween(ConfigureDefinition(new CpuParticles3DGravityXTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a CpuParticles3DGravityYTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<CpuParticles3D, float> TweenGravityY(this CpuParticles3D target,
        float to, double duration, Action<CpuParticles3DGravityYTween>? configure = null)
        => target.Tween(ConfigureDefinition(new CpuParticles3DGravityYTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a CpuParticles3DGravityZTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<CpuParticles3D, float> TweenGravityZ(this CpuParticles3D target,
        float to, double duration, Action<CpuParticles3DGravityZTween>? configure = null)
        => target.Tween(ConfigureDefinition(new CpuParticles3DGravityZTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a CpuParticles3DSpreadTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<CpuParticles3D, float> TweenSpread(this CpuParticles3D target,
        float to, double duration, Action<CpuParticles3DSpreadTween>? configure = null)
        => target.Tween(ConfigureDefinition(new CpuParticles3DSpreadTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a CpuParticles3DEmissionSphereRadiusTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<CpuParticles3D, float> TweenEmissionSphereRadius(this CpuParticles3D target,
        float to, double duration, Action<CpuParticles3DEmissionSphereRadiusTween>? configure = null)
        => target.Tween(ConfigureDefinition(new CpuParticles3DEmissionSphereRadiusTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a CpuParticles3DEmissionBoxExtentsTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<CpuParticles3D, Vector3> TweenEmissionBoxExtents(this CpuParticles3D target,
        Vector3 to, double duration, Action<CpuParticles3DEmissionBoxExtentsTween>? configure = null)
        => target.Tween(ConfigureDefinition(new CpuParticles3DEmissionBoxExtentsTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a CpuParticles3DEmissionBoxExtentsXTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<CpuParticles3D, float> TweenEmissionBoxExtentsX(this CpuParticles3D target,
        float to, double duration, Action<CpuParticles3DEmissionBoxExtentsXTween>? configure = null)
        => target.Tween(ConfigureDefinition(new CpuParticles3DEmissionBoxExtentsXTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a CpuParticles3DEmissionBoxExtentsYTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<CpuParticles3D, float> TweenEmissionBoxExtentsY(this CpuParticles3D target,
        float to, double duration, Action<CpuParticles3DEmissionBoxExtentsYTween>? configure = null)
        => target.Tween(ConfigureDefinition(new CpuParticles3DEmissionBoxExtentsYTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a CpuParticles3DEmissionBoxExtentsZTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<CpuParticles3D, float> TweenEmissionBoxExtentsZ(this CpuParticles3D target,
        float to, double duration, Action<CpuParticles3DEmissionBoxExtentsZTween>? configure = null)
        => target.Tween(ConfigureDefinition(new CpuParticles3DEmissionBoxExtentsZTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a CpuParticles3DColorTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<CpuParticles3D, Color> TweenColor(this CpuParticles3D target,
        Color to, double duration, Action<CpuParticles3DColorTween>? configure = null)
        => target.Tween(ConfigureDefinition(new CpuParticles3DColorTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a CpuParticles3DColorAlphaTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<CpuParticles3D, float> TweenColorAlpha(this CpuParticles3D target,
        float to, double duration, Action<CpuParticles3DColorAlphaTween>? configure = null)
        => target.Tween(ConfigureDefinition(new CpuParticles3DColorAlphaTween { To = to, Duration = duration }, configure));
}

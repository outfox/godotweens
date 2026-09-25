// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using Godot;

namespace tweens.gd;

public static partial class TweenExtensions
{
    /// <summary>Starts a SpriteBase3DModulateTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<SpriteBase3D, Color> TweenModulate(this SpriteBase3D target,
        Color to, double duration, Action<SpriteBase3DModulateTween>? configure = null)
        => target.Tween(ConfigureDefinition(new SpriteBase3DModulateTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a SpriteBase3DModulateAlphaTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<SpriteBase3D, float> TweenModulateAlpha(this SpriteBase3D target,
        float to, double duration, Action<SpriteBase3DModulateAlphaTween>? configure = null)
        => target.Tween(ConfigureDefinition(new SpriteBase3DModulateAlphaTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a SpriteBase3DOffsetTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<SpriteBase3D, Vector2> TweenOffset(this SpriteBase3D target,
        Vector2 to, double duration, Action<SpriteBase3DOffsetTween>? configure = null)
        => target.Tween(ConfigureDefinition(new SpriteBase3DOffsetTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a SpriteBase3DOffsetXTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<SpriteBase3D, float> TweenOffsetX(this SpriteBase3D target,
        float to, double duration, Action<SpriteBase3DOffsetXTween>? configure = null)
        => target.Tween(ConfigureDefinition(new SpriteBase3DOffsetXTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a SpriteBase3DOffsetYTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<SpriteBase3D, float> TweenOffsetY(this SpriteBase3D target,
        float to, double duration, Action<SpriteBase3DOffsetYTween>? configure = null)
        => target.Tween(ConfigureDefinition(new SpriteBase3DOffsetYTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a SpriteBase3DPixelSizeTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<SpriteBase3D, float> TweenPixelSize(this SpriteBase3D target,
        float to, double duration, Action<SpriteBase3DPixelSizeTween>? configure = null)
        => target.Tween(ConfigureDefinition(new SpriteBase3DPixelSizeTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a Label3DModulateTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<Label3D, Color> TweenModulate(this Label3D target,
        Color to, double duration, Action<Label3DModulateTween>? configure = null)
        => target.Tween(ConfigureDefinition(new Label3DModulateTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a Label3DModulateAlphaTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<Label3D, float> TweenModulateAlpha(this Label3D target,
        float to, double duration, Action<Label3DModulateAlphaTween>? configure = null)
        => target.Tween(ConfigureDefinition(new Label3DModulateAlphaTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a Label3DOutlineModulateTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<Label3D, Color> TweenOutlineModulate(this Label3D target,
        Color to, double duration, Action<Label3DOutlineModulateTween>? configure = null)
        => target.Tween(ConfigureDefinition(new Label3DOutlineModulateTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a Label3DOutlineModulateAlphaTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<Label3D, float> TweenOutlineModulateAlpha(this Label3D target,
        float to, double duration, Action<Label3DOutlineModulateAlphaTween>? configure = null)
        => target.Tween(ConfigureDefinition(new Label3DOutlineModulateAlphaTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a Label3DOffsetTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<Label3D, Vector2> TweenOffset(this Label3D target,
        Vector2 to, double duration, Action<Label3DOffsetTween>? configure = null)
        => target.Tween(ConfigureDefinition(new Label3DOffsetTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a Label3DOffsetXTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<Label3D, float> TweenOffsetX(this Label3D target,
        float to, double duration, Action<Label3DOffsetXTween>? configure = null)
        => target.Tween(ConfigureDefinition(new Label3DOffsetXTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a Label3DOffsetYTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<Label3D, float> TweenOffsetY(this Label3D target,
        float to, double duration, Action<Label3DOffsetYTween>? configure = null)
        => target.Tween(ConfigureDefinition(new Label3DOffsetYTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a Label3DPixelSizeTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<Label3D, float> TweenPixelSize(this Label3D target,
        float to, double duration, Action<Label3DPixelSizeTween>? configure = null)
        => target.Tween(ConfigureDefinition(new Label3DPixelSizeTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a GeometryInstance3DTransparencyTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<GeometryInstance3D, float> TweenTransparency(this GeometryInstance3D target,
        float to, double duration, Action<GeometryInstance3DTransparencyTween>? configure = null)
        => target.Tween(ConfigureDefinition(new GeometryInstance3DTransparencyTween { To = to, Duration = duration }, configure));
}

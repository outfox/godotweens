// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using Godot;

namespace godotweens;

public static partial class TweenExtensions
{
    /// <summary>Starts a ColorRectColorTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<ColorRect, Color> TweenColor(this ColorRect target,
        Color to, double duration, Action<ColorRectColorTween>? configure = null)
        => target.Tween(ConfigureDefinition(new ColorRectColorTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a ColorRectColorAlphaTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<ColorRect, float> TweenColorAlpha(this ColorRect target,
        float to, double duration, Action<ColorRectColorAlphaTween>? configure = null)
        => target.Tween(ConfigureDefinition(new ColorRectColorAlphaTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a LabelVisibleRatioTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<Label, float> TweenVisibleRatio(this Label target,
        float to, double duration, Action<LabelVisibleRatioTween>? configure = null)
        => target.Tween(ConfigureDefinition(new LabelVisibleRatioTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a RichTextLabelVisibleRatioTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<RichTextLabel, float> TweenVisibleRatio(this RichTextLabel target,
        float to, double duration, Action<RichTextLabelVisibleRatioTween>? configure = null)
        => target.Tween(ConfigureDefinition(new RichTextLabelVisibleRatioTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a TextureProgressBarTintUnderTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<TextureProgressBar, Color> TweenTintUnder(this TextureProgressBar target,
        Color to, double duration, Action<TextureProgressBarTintUnderTween>? configure = null)
        => target.Tween(ConfigureDefinition(new TextureProgressBarTintUnderTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a TextureProgressBarTintUnderAlphaTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<TextureProgressBar, float> TweenTintUnderAlpha(this TextureProgressBar target,
        float to, double duration, Action<TextureProgressBarTintUnderAlphaTween>? configure = null)
        => target.Tween(ConfigureDefinition(new TextureProgressBarTintUnderAlphaTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a TextureProgressBarTintOverTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<TextureProgressBar, Color> TweenTintOver(this TextureProgressBar target,
        Color to, double duration, Action<TextureProgressBarTintOverTween>? configure = null)
        => target.Tween(ConfigureDefinition(new TextureProgressBarTintOverTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a TextureProgressBarTintOverAlphaTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<TextureProgressBar, float> TweenTintOverAlpha(this TextureProgressBar target,
        float to, double duration, Action<TextureProgressBarTintOverAlphaTween>? configure = null)
        => target.Tween(ConfigureDefinition(new TextureProgressBarTintOverAlphaTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a TextureProgressBarTintProgressTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<TextureProgressBar, Color> TweenTintProgress(this TextureProgressBar target,
        Color to, double duration, Action<TextureProgressBarTintProgressTween>? configure = null)
        => target.Tween(ConfigureDefinition(new TextureProgressBarTintProgressTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a TextureProgressBarTintProgressAlphaTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<TextureProgressBar, float> TweenTintProgressAlpha(this TextureProgressBar target,
        float to, double duration, Action<TextureProgressBarTintProgressAlphaTween>? configure = null)
        => target.Tween(ConfigureDefinition(new TextureProgressBarTintProgressAlphaTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a TextureProgressBarRadialInitialAngleTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<TextureProgressBar, float> TweenRadialInitialAngle(this TextureProgressBar target,
        float to, double duration, Action<TextureProgressBarRadialInitialAngleTween>? configure = null)
        => target.Tween(ConfigureDefinition(new TextureProgressBarRadialInitialAngleTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a TextureProgressBarRadialFillDegreesTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<TextureProgressBar, float> TweenRadialFillDegrees(this TextureProgressBar target,
        float to, double duration, Action<TextureProgressBarRadialFillDegreesTween>? configure = null)
        => target.Tween(ConfigureDefinition(new TextureProgressBarRadialFillDegreesTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a TextureProgressBarRadialCenterOffsetTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<TextureProgressBar, Vector2> TweenRadialCenterOffset(this TextureProgressBar target,
        Vector2 to, double duration, Action<TextureProgressBarRadialCenterOffsetTween>? configure = null)
        => target.Tween(ConfigureDefinition(new TextureProgressBarRadialCenterOffsetTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a TextureProgressBarRadialCenterOffsetXTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<TextureProgressBar, float> TweenRadialCenterOffsetX(this TextureProgressBar target,
        float to, double duration, Action<TextureProgressBarRadialCenterOffsetXTween>? configure = null)
        => target.Tween(ConfigureDefinition(new TextureProgressBarRadialCenterOffsetXTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a TextureProgressBarRadialCenterOffsetYTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<TextureProgressBar, float> TweenRadialCenterOffsetY(this TextureProgressBar target,
        float to, double duration, Action<TextureProgressBarRadialCenterOffsetYTween>? configure = null)
        => target.Tween(ConfigureDefinition(new TextureProgressBarRadialCenterOffsetYTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a TextureProgressBarTextureProgressOffsetTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<TextureProgressBar, Vector2> TweenTextureProgressOffset(this TextureProgressBar target,
        Vector2 to, double duration, Action<TextureProgressBarTextureProgressOffsetTween>? configure = null)
        => target.Tween(ConfigureDefinition(new TextureProgressBarTextureProgressOffsetTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a TextureProgressBarTextureProgressOffsetXTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<TextureProgressBar, float> TweenTextureProgressOffsetX(this TextureProgressBar target,
        float to, double duration, Action<TextureProgressBarTextureProgressOffsetXTween>? configure = null)
        => target.Tween(ConfigureDefinition(new TextureProgressBarTextureProgressOffsetXTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a TextureProgressBarTextureProgressOffsetYTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<TextureProgressBar, float> TweenTextureProgressOffsetY(this TextureProgressBar target,
        float to, double duration, Action<TextureProgressBarTextureProgressOffsetYTween>? configure = null)
        => target.Tween(ConfigureDefinition(new TextureProgressBarTextureProgressOffsetYTween { To = to, Duration = duration }, configure));
}

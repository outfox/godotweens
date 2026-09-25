// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using Godot;

namespace tweens.gd;

public static partial class TweenExtensions
{
    /// <summary>Starts a LabelVisibleCharactersTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<Label, int> TweenVisibleCharacters(this Label target,
        int to, double duration, Action<LabelVisibleCharactersTween>? configure = null)
        => target.Tween(ConfigureDefinition(new LabelVisibleCharactersTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a LabelVisibleCharactersTween.
    /// Copies the options; the explicit duration takes precedence.</summary>
    public static TweenInstance<Label, int> TweenVisibleCharacters(this Label target,
        int to, double duration, TweenOptions options)
        => target.Tween(ApplyOptions(new LabelVisibleCharactersTween { To = to, Duration = duration }, options));

    /// <summary>Starts a RichTextLabelVisibleCharactersTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<RichTextLabel, int> TweenVisibleCharacters(this RichTextLabel target,
        int to, double duration, Action<RichTextLabelVisibleCharactersTween>? configure = null)
        => target.Tween(ConfigureDefinition(new RichTextLabelVisibleCharactersTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a RichTextLabelVisibleCharactersTween.
    /// Copies the options; the explicit duration takes precedence.</summary>
    public static TweenInstance<RichTextLabel, int> TweenVisibleCharacters(this RichTextLabel target,
        int to, double duration, TweenOptions options)
        => target.Tween(ApplyOptions(new RichTextLabelVisibleCharactersTween { To = to, Duration = duration }, options));

    /// <summary>Starts a ScrollContainerScrollHorizontalTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<ScrollContainer, int> TweenScrollHorizontal(this ScrollContainer target,
        int to, double duration, Action<ScrollContainerScrollHorizontalTween>? configure = null)
        => target.Tween(ConfigureDefinition(new ScrollContainerScrollHorizontalTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a ScrollContainerScrollHorizontalTween.
    /// Copies the options; the explicit duration takes precedence.</summary>
    public static TweenInstance<ScrollContainer, int> TweenScrollHorizontal(this ScrollContainer target,
        int to, double duration, TweenOptions options)
        => target.Tween(ApplyOptions(new ScrollContainerScrollHorizontalTween { To = to, Duration = duration }, options));

    /// <summary>Starts a ScrollContainerScrollVerticalTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<ScrollContainer, int> TweenScrollVertical(this ScrollContainer target,
        int to, double duration, Action<ScrollContainerScrollVerticalTween>? configure = null)
        => target.Tween(ConfigureDefinition(new ScrollContainerScrollVerticalTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a ScrollContainerScrollVerticalTween.
    /// Copies the options; the explicit duration takes precedence.</summary>
    public static TweenInstance<ScrollContainer, int> TweenScrollVertical(this ScrollContainer target,
        int to, double duration, TweenOptions options)
        => target.Tween(ApplyOptions(new ScrollContainerScrollVerticalTween { To = to, Duration = duration }, options));

    /// <summary>Starts a Sprite2DFrameTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<Sprite2D, int> TweenFrame(this Sprite2D target,
        int to, double duration, Action<Sprite2DFrameTween>? configure = null)
        => target.Tween(ConfigureDefinition(new Sprite2DFrameTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a Sprite2DFrameTween. Copies the options; the explicit duration takes precedence.</summary>
    public static TweenInstance<Sprite2D, int> TweenFrame(this Sprite2D target,
        int to, double duration, TweenOptions options)
        => target.Tween(ApplyOptions(new Sprite2DFrameTween { To = to, Duration = duration }, options));

    /// <summary>Starts a AnimatedSprite2DFrameTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<AnimatedSprite2D, int> TweenFrame(this AnimatedSprite2D target,
        int to, double duration, Action<AnimatedSprite2DFrameTween>? configure = null)
        => target.Tween(ConfigureDefinition(new AnimatedSprite2DFrameTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a AnimatedSprite2DFrameTween.
    /// Copies the options; the explicit duration takes precedence.</summary>
    public static TweenInstance<AnimatedSprite2D, int> TweenFrame(this AnimatedSprite2D target,
        int to, double duration, TweenOptions options)
        => target.Tween(ApplyOptions(new AnimatedSprite2DFrameTween { To = to, Duration = duration }, options));

    /// <summary>Starts a AnimatedSprite3DFrameTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<AnimatedSprite3D, int> TweenFrame(this AnimatedSprite3D target,
        int to, double duration, Action<AnimatedSprite3DFrameTween>? configure = null)
        => target.Tween(ConfigureDefinition(new AnimatedSprite3DFrameTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a AnimatedSprite3DFrameTween.
    /// Copies the options; the explicit duration takes precedence.</summary>
    public static TweenInstance<AnimatedSprite3D, int> TweenFrame(this AnimatedSprite3D target,
        int to, double duration, TweenOptions options)
        => target.Tween(ApplyOptions(new AnimatedSprite3DFrameTween { To = to, Duration = duration }, options));
}

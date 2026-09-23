// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using Godot;

namespace godotweens;

public static partial class TweenExtensions
{
    /// <summary>Starts a ModulateTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<CanvasItem, Color> TweenModulate(this CanvasItem target,
        Color to, double duration, Action<ModulateTween>? configure = null)
        => target.Tween(ConfigureDefinition(new ModulateTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a ModulateAlphaTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<CanvasItem, float> TweenModulateAlpha(this CanvasItem target,
        float to, double duration, Action<ModulateAlphaTween>? configure = null)
        => target.Tween(ConfigureDefinition(new ModulateAlphaTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a SelfModulateTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<CanvasItem, Color> TweenSelfModulate(this CanvasItem target,
        Color to, double duration, Action<SelfModulateTween>? configure = null)
        => target.Tween(ConfigureDefinition(new SelfModulateTween { To = to, Duration = duration }, configure));

    /// <summary>Starts a SelfModulateAlphaTween. Configure runs before snapshotting and can override any definition option.</summary>
    public static TweenInstance<CanvasItem, float> TweenSelfModulateAlpha(this CanvasItem target,
        float to, double duration, Action<SelfModulateAlphaTween>? configure = null)
        => target.Tween(ConfigureDefinition(new SelfModulateAlphaTween { To = to, Duration = duration }, configure));
}

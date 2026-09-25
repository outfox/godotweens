// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using Godot;

namespace tweens.gd;

/// <summary>Animates CanvasItem Modulate; values use Godot property units.</summary>
public sealed class ModulateTween() : PropertyTween<CanvasItem, Color>(
    static n => n.Modulate, static (n, value) => n.Modulate = value, Interpolators.Color);

/// <summary>Animates CanvasItem ModulateAlpha; values use Godot property units.</summary>
public sealed class ModulateAlphaTween() : PropertyTween<CanvasItem, float>(
    static n => n.Modulate.A, static (n, value) => { var current = n.Modulate; current.A = value; n.Modulate = current; }, Interpolators.Float);

/// <summary>Animates CanvasItem SelfModulate; values use Godot property units.</summary>
public sealed class SelfModulateTween() : PropertyTween<CanvasItem, Color>(
    static n => n.SelfModulate, static (n, value) => n.SelfModulate = value, Interpolators.Color);

/// <summary>Animates CanvasItem SelfModulateAlpha; values use Godot property units.</summary>
public sealed class SelfModulateAlphaTween() : PropertyTween<CanvasItem, float>(
    static n => n.SelfModulate.A, static (n, value) => { var current = n.SelfModulate; current.A = value; n.SelfModulate = current; }, Interpolators.Float);


// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using Godot;

namespace tweens.gd;

/// <summary>Animates Node Float; values use Godot property units.</summary>
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public sealed class FloatTween() : PropertyTween<Node, float>(
    static _ => 0, static (_, _) => {}, Interpolators.Float);

/// <summary>Animates Node Double; values use Godot property units.</summary>
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public sealed class DoubleTween() : PropertyTween<Node, double>(
    static _ => 0, static (_, _) => {}, Interpolators.Double);

/// <summary>Animates Node Vector2; values use Godot property units.</summary>
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public sealed class Vector2Tween() : PropertyTween<Node, Vector2>(
    static _ => Vector2.Zero, static (_, _) => {}, Interpolators.Vector2);

/// <summary>Animates Node Vector3; values use Godot property units.</summary>
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public sealed class Vector3Tween() : PropertyTween<Node, Vector3>(
    static _ => Vector3.Zero, static (_, _) => {}, Interpolators.Vector3);

/// <summary>Animates Node Vector4; values use Godot property units.</summary>
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public sealed class Vector4Tween() : PropertyTween<Node, Vector4>(
    static _ => Vector4.Zero, static (_, _) => {}, Interpolators.Vector4);

/// <summary>Animates Node Color; values use Godot property units.</summary>
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public sealed class ColorTween() : PropertyTween<Node, Color>(
    static _ => new Color(0, 0, 0, 0), static (_, _) => {}, Interpolators.Color);

/// <summary>Animates Node Quaternion; values use Godot property units.</summary>
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public sealed class QuaternionTween() : PropertyTween<Node, Quaternion>(
    static _ => Quaternion.Identity, static (_, _) => {}, Interpolators.Quaternion);

/// <summary>Animates Node Rect2; values use Godot property units.</summary>
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public sealed class Rect2Tween() : PropertyTween<Node, Rect2>(
    static _ => default(Rect2), static (_, _) => {}, Interpolators.Rect2);

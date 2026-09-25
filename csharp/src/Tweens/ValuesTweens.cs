// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using Godot;

namespace tweens.gd;

/// <summary>Animates Node Float; values use Godot property units.</summary>
public sealed class FloatTween() : PropertyTween<Node, float>(
    static n => 0, static (n, value) => {}, Interpolators.Float);

/// <summary>Animates Node Double; values use Godot property units.</summary>
public sealed class DoubleTween() : PropertyTween<Node, double>(
    static n => 0, static (n, value) => {}, Interpolators.Double);

/// <summary>Animates Node Vector2; values use Godot property units.</summary>
public sealed class Vector2Tween() : PropertyTween<Node, Vector2>(
    static n => Vector2.Zero, static (n, value) => {}, Interpolators.Vector2);

/// <summary>Animates Node Vector3; values use Godot property units.</summary>
public sealed class Vector3Tween() : PropertyTween<Node, Vector3>(
    static n => Vector3.Zero, static (n, value) => {}, Interpolators.Vector3);

/// <summary>Animates Node Vector4; values use Godot property units.</summary>
public sealed class Vector4Tween() : PropertyTween<Node, Vector4>(
    static n => Vector4.Zero, static (n, value) => {}, Interpolators.Vector4);

/// <summary>Animates Node Color; values use Godot property units.</summary>
public sealed class ColorTween() : PropertyTween<Node, Color>(
    static n => new Color(0, 0, 0, 0), static (n, value) => {}, Interpolators.Color);

/// <summary>Animates Node Quaternion; values use Godot property units.</summary>
public sealed class QuaternionTween() : PropertyTween<Node, Quaternion>(
    static n => Quaternion.Identity, static (n, value) => {}, Interpolators.Quaternion);

/// <summary>Animates Node Rect2; values use Godot property units.</summary>
public sealed class Rect2Tween() : PropertyTween<Node, Rect2>(
    static n => default(Rect2), static (n, value) => {}, Interpolators.Rect2);

// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using Godot;

namespace GodotWeens;

/// <summary>Animates Light2D LightColor2D; values use Godot property units.</summary>
public sealed class LightColor2DTween() : PropertyTween<Light2D, Color>(
    static n => n.Color, static (n, value) => n.Color = value, Interpolators.Color);

/// <summary>Animates Light2D LightEnergy2D; values use Godot property units.</summary>
public sealed class LightEnergy2DTween() : PropertyTween<Light2D, float>(
    static n => n.Energy, static (n, value) => n.Energy = value, Interpolators.Float);

/// <summary>Animates Light3D LightColor3D; values use Godot property units.</summary>
public sealed class LightColor3DTween() : PropertyTween<Light3D, Color>(
    static n => n.LightColor, static (n, value) => n.LightColor = value, Interpolators.Color);

/// <summary>Animates Light3D LightEnergy3D; values use Godot property units.</summary>
public sealed class LightEnergy3DTween() : PropertyTween<Light3D, float>(
    static n => n.LightEnergy, static (n, value) => n.LightEnergy = value, Interpolators.Float);

/// <summary>Animates OmniLight3D OmniRange; values use Godot property units.</summary>
public sealed class OmniRangeTween() : PropertyTween<OmniLight3D, float>(
    static n => n.OmniRange, static (n, value) => n.OmniRange = value, Interpolators.Float);

/// <summary>Animates SpotLight3D SpotRange; values use Godot property units.</summary>
public sealed class SpotRangeTween() : PropertyTween<SpotLight3D, float>(
    static n => n.SpotRange, static (n, value) => n.SpotRange = value, Interpolators.Float);

/// <summary>Animates SpotLight3D SpotAngle; values use Godot property units.</summary>
public sealed class SpotAngleTween() : PropertyTween<SpotLight3D, float>(
    static n => n.SpotAngle, static (n, value) => n.SpotAngle = value, Interpolators.Float);


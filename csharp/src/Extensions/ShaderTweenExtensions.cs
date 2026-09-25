// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using Godot;
namespace tweens.gd;

public static partial class TweenExtensions
{
    /// <summary>Animate a shared material uniform in a tree, optionally binding playback to a node.</summary>
    public static TweenInstance<ShaderMaterial, TValue> TweenShaderParameter<TValue>(this ShaderMaterial target,
        string parameter, TValue to, double duration, SceneTree tree,
        Action<ShaderParameterTween<TValue>>? configure = null, Node? owner = null) where TValue : struct
        => target.Tween(ConfigureDefinition(new ShaderParameterTween<TValue>(parameter) { To = to, Duration = duration }, configure), tree, owner);

    /// <summary>Animate a shared material uniform in a tree, optionally binding playback to a node.
    /// Copies the options; the explicit duration takes precedence.</summary>
    public static TweenInstance<ShaderMaterial, TValue> TweenShaderParameter<TValue>(this ShaderMaterial target,
        string parameter, TValue to, double duration, SceneTree tree,
        TweenOptions options, Node? owner = null) where TValue : struct
        => target.Tween(ApplyOptions(new ShaderParameterTween<TValue>(parameter) { To = to, Duration = duration }, options), tree, owner);

    /// <summary>Animate a shared material uniform with node-bound lifetime.</summary>
    public static TweenInstance<ShaderMaterial, TValue> TweenShaderParameter<TValue>(this ShaderMaterial target,
        string parameter, TValue to, double duration, Node owner,
        Action<ShaderParameterTween<TValue>>? configure = null) where TValue : struct
        => target.Tween(ConfigureDefinition(new ShaderParameterTween<TValue>(parameter) { To = to, Duration = duration }, configure), owner);

    /// <summary>Animate a shared material uniform with node-bound lifetime.
    /// Copies the options; the explicit duration takes precedence.</summary>
    public static TweenInstance<ShaderMaterial, TValue> TweenShaderParameter<TValue>(this ShaderMaterial target,
        string parameter, TValue to, double duration, Node owner,
        TweenOptions options) where TValue : struct
        => target.Tween(ApplyOptions(new ShaderParameterTween<TValue>(parameter) { To = to, Duration = duration }, options), owner);

    public static TweenInstance<CanvasItem, TValue> TweenInstanceShaderParameter<TValue>(this CanvasItem target,
        string parameter, TValue to, double duration, Action<CanvasItemInstanceShaderParameterTween<TValue>>? configure = null)
        where TValue : struct
        => target.Tween(ConfigureDefinition(new CanvasItemInstanceShaderParameterTween<TValue>(parameter) { To = to, Duration = duration }, configure));

    public static TweenInstance<CanvasItem, TValue> TweenInstanceShaderParameter<TValue>(this CanvasItem target,
        string parameter, TValue to, double duration, TweenOptions options)
        where TValue : struct
        => target.Tween(ApplyOptions(new CanvasItemInstanceShaderParameterTween<TValue>(parameter) { To = to, Duration = duration }, options));

    public static TweenInstance<GeometryInstance3D, TValue> TweenInstanceShaderParameter<TValue>(this GeometryInstance3D target,
        string parameter, TValue to, double duration, Action<GeometryInstanceShaderParameterTween<TValue>>? configure = null)
        where TValue : struct
        => target.Tween(ConfigureDefinition(new GeometryInstanceShaderParameterTween<TValue>(parameter) { To = to, Duration = duration }, configure));

    public static TweenInstance<GeometryInstance3D, TValue> TweenInstanceShaderParameter<TValue>(this GeometryInstance3D target,
        string parameter, TValue to, double duration, TweenOptions options)
        where TValue : struct
        => target.Tween(ApplyOptions(new GeometryInstanceShaderParameterTween<TValue>(parameter) { To = to, Duration = duration }, options));
}

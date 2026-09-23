// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using Godot;
namespace godotweens;

/// <summary>Shared implementation of node-owned, typed instance uniforms. Restoration preserves absence of an override.</summary>
public abstract class InstanceShaderParameterTween<TNode, TValue>(string parameter) : TweenDefinition<TNode, TValue>
    where TNode : Node where TValue : struct
{
    public string Parameter { get; set; } = parameter;
    private StringName? name;
    private InstanceShaderWatch? watch;
    private bool hadOverride;
    protected abstract Variant GetParameter(TNode target, StringName name);
    protected abstract Variant GetDefault(TNode target, StringName name);
    protected abstract void SetParameter(TNode target, StringName name, Variant value);
    protected override void Prepare(TNode target)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(Parameter);
        _ = ShaderValues<TValue>.Type;
        if (From is { } from) ShaderValues<TValue>.Validate(from);
        if (To is { } to) ShaderValues<TValue>.Validate(to);
        name = new StringName(Parameter);
        watch = new InstanceShaderWatch(target);
        var properties = target.GetPropertyList();
        foreach (var property in properties)
        {
            using (property)
            {
                if (property["name"].AsString() != "instance_shader_parameters/" + Parameter) continue;
                ShaderValues<TValue>.ValidateType((Variant.Type)property["type"].AsInt32());
                hadOverride = ((PropertyUsageFlags)property["usage"].AsInt64() & PropertyUsageFlags.Storage) != 0;
                return;
            }
        }
        throw new ArgumentException($"Node has no instance uniform named '{Parameter}'. Check its effective shader and renderer.", nameof(Parameter));
    }
    protected override TValue Read(TNode target)
    {
        watch!.Validate();
        using var value = hadOverride ? GetParameter(target, name!) : GetDefault(target, name!);
        return ShaderValues<TValue>.Read(value);
    }
    protected override void Write(TNode target, TValue value)
    {
        watch!.Validate();
        using var variant = ShaderValues<TValue>.Write(value);
        SetParameter(target, name!, variant);
    }
    protected override void Restore(TNode target, TValue initial)
    {
        watch!.Validate();
        if (hadOverride) Write(target, initial);
        else SetParameter(target, name!, default);
    }
    protected override TValue Interpolate(TValue from, TValue to, float weight)
        => ShaderValues<TValue>.Interpolate(from, to, weight);
    protected override void Release()
    {
        watch?.Dispose(); watch = null;
        name?.Dispose(); name = null;
    }
}

public sealed class CanvasItemInstanceShaderParameterTween<TValue>(string parameter)
    : InstanceShaderParameterTween<CanvasItem, TValue>(parameter) where TValue : struct
{
    protected override Variant GetParameter(CanvasItem target, StringName name) => target.GetInstanceShaderParameter(name);
    protected override Variant GetDefault(CanvasItem target, StringName name)
        => RenderingServer.CanvasItemGetInstanceShaderParameterDefaultValue(target.GetCanvasItem(), name);
    protected override void SetParameter(CanvasItem target, StringName name, Variant value) => target.SetInstanceShaderParameter(name, value);
}

public sealed class GeometryInstanceShaderParameterTween<TValue>(string parameter)
    : InstanceShaderParameterTween<GeometryInstance3D, TValue>(parameter) where TValue : struct
{
    protected override Variant GetParameter(GeometryInstance3D target, StringName name) => target.GetInstanceShaderParameter(name);
    protected override Variant GetDefault(GeometryInstance3D target, StringName name)
        => RenderingServer.InstanceGeometryGetShaderParameterDefaultValue(target.GetInstance(), name);
    protected override void SetParameter(GeometryInstance3D target, StringName name, Variant value) => target.SetInstanceShaderParameter(name, value);
}

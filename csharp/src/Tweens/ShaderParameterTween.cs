// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using Godot;
namespace tweens.gd;

/// <summary>Animates a typed uniform on the exact shared ShaderMaterial. Names and bindings are captured per playback.</summary>
public sealed class ShaderParameterTween<TValue>(string parameter) : TweenDefinition<ShaderMaterial, TValue>
    where TValue : struct
{
    public string Parameter { get; set; } = parameter;
    private StringName? name;
    private ShaderWatch? watch;
    private bool hadOverride;

    protected override void Prepare(ShaderMaterial target)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(Parameter);
        _ = ShaderValues<TValue>.Type;
        if (From is { } from) ShaderValues<TValue>.Validate(from);
        if (To is { } to) ShaderValues<TValue>.Validate(to);
        name = new StringName(Parameter);
        var shader = target.Shader;
        watch = new ShaderWatch(shader);
        using var uniforms = shader.GetShaderUniformList();
        foreach (var entry in uniforms)
        {
            using (entry)
            using (var uniform = entry.AsGodotDictionary())
            {
                if (uniform["name"].AsString() != Parameter) continue;
                ShaderValues<TValue>.ValidateType((Variant.Type)uniform["type"].AsInt32());
                return;
            }
        }
        throw new ArgumentException($"Shader has no material uniform named '{Parameter}'.", nameof(Parameter));
    }
    protected override TValue Read(ShaderMaterial target)
    {
        watch!.Validate(target.Shader);
        using var value = target.GetShaderParameter(name!);
        hadOverride = value.VariantType != Variant.Type.Nil;
        if (hadOverride) return ShaderValues<TValue>.Read(value);
        using var defaultValue = RenderingServer.ShaderGetParameterDefault(target.Shader.GetRid(), name!);
        return ShaderValues<TValue>.Read(defaultValue);
    }
    protected override void Write(ShaderMaterial target, TValue value)
    {
        watch!.Validate(target.Shader);
        using var variant = ShaderValues<TValue>.Write(value);
        target.SetShaderParameter(name!, variant);
    }
    protected override void Restore(ShaderMaterial target, TValue initial)
    {
        watch!.Validate(target.Shader);
        if (hadOverride) Write(target, initial);
        else target.SetShaderParameter(name!, default);
    }
    protected override TValue Interpolate(TValue from, TValue to, float weight)
        => ShaderValues<TValue>.Interpolate(from, to, weight);
    protected override void Release()
    {
        watch?.Dispose(); watch = null;
        name?.Dispose(); name = null;
    }
}

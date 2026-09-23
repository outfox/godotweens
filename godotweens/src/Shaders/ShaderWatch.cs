// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using Godot;
namespace godotweens;

// Material.Changed does not reliably report shader replacement outside the editor.
// Check shader identity on writes and watch Shader.Changed for edits/includes.
internal sealed class ShaderWatch : IDisposable
{
    private readonly Shader shader;
    private bool changed;
    internal ShaderWatch(Shader shader)
    {
        if (!GodotObject.IsInstanceValid(shader)) throw new ArgumentException("A live shader is required.");
        this.shader = shader;
        shader.Changed += Changed;
    }
    private void Changed() => changed = true;
    internal void Validate(Shader? current)
    {
        if (changed || !GodotObject.IsInstanceValid(shader) || current != shader)
            throw new InvalidOperationException("The shader binding changed during playback. Start a new tween for the new shader.");
    }
    public void Dispose()
    {
        if (GodotObject.IsInstanceValid(shader)) shader.Changed -= Changed;
    }
}

// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using Godot;
using twodog.Testing;

namespace tweens.gd.Tests.Support;

/// <summary>A per-test subtree of the fixture's root. Disposal frees the subtree and the tracked resources.</summary>
public sealed class SceneScope : IDisposable
{
    private readonly HeadlessFixture godot;
    private readonly List<GodotObject> resources = [];

    public Node Root { get; } = new() { Name = "Scope" };
    public SceneTree Tree => godot.Tree;

    public SceneScope(HeadlessFixture godot)
    {
        this.godot = godot;
        godot.Tree.Root.AddChild(Root);
    }

    public T Add<T>(T node) where T : Node
    {
        Root.AddChild(node);
        return node;
    }

    public T Track<T>(T resource) where T : GodotObject
    {
        resources.Add(resource);
        return resource;
    }

    /// <summary>Iterates the engine; the automatic runner advances by real frame time.</summary>
    public void Frames(int count = 1)
    {
        for (var i = 0; i < count; i++) godot.Engine.Iteration();
    }

    /// <summary>Iterates until the condition holds, pausing briefly so real time and physics steps pass.</summary>
    public void FramesUntil(Func<bool> condition, int limit = 500)
    {
        for (var i = 0; i < limit && !condition(); i++)
        {
            Thread.Sleep(2);
            godot.Engine.Iteration();
        }
        Assert.True(condition(), "The condition did not hold within the frame limit.");
    }

    /// <summary>Drives the automatic runner's scheduler by exact deltas, independent of frame timing.</summary>
    public void Advance(double delta, TweenProcessMode mode = TweenProcessMode.Process)
        => TweenRuntime.GetRunner(godot.Tree).Scheduler.Update(delta, delta, mode);

    public void Dispose()
    {
        if (GodotObject.IsInstanceValid(Root)) Root.Free();
        foreach (var resource in resources)
            if (GodotObject.IsInstanceValid(resource)) resource.Dispose();
    }
}

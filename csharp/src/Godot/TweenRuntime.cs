// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using Godot;
using System.Runtime.CompilerServices;

namespace tweens.gd;

/// <summary>Automatic per-tree runtime. No autoload or 2dog host changes are needed.</summary>
public static class TweenRuntime
{
    private static readonly Dictionary<SceneTree, TweenRunner> runners = new();
    private static readonly ConditionalWeakTable<SceneTree, object> closingTrees = new();

    internal static void EnsureMainThread()
    {
        if (OS.GetThreadCallerId() != OS.GetMainThreadId())
            throw new InvalidOperationException("Godot tweens must be created on Godot's main thread.");
    }

    internal static void ValidateTree(SceneTree tree)
    {
        EnsureMainThread();
        if (!GodotObject.IsInstanceValid(tree) || !GodotObject.IsInstanceValid(tree.Root) || !tree.Root.IsInsideTree())
            throw new ArgumentException("The scene tree must be live and initialized.", nameof(tree));
        if (closingTrees.TryGetValue(tree, out _))
            throw new InvalidOperationException("Cannot start a tween while the scene tree is shutting down.");
    }

    internal static void ValidateOwner(Node owner)
    {
        EnsureMainThread();
        if (!GodotObject.IsInstanceValid(owner) || owner.IsQueuedForDeletion() || !owner.IsInsideTree())
            throw new ArgumentException("The owner must be a valid node inside the scene tree.", nameof(owner));
    }

    internal static TweenRunner GetRunner(Node owner)
    {
        ValidateOwner(owner);
        return GetRunner(owner.GetTree());
    }

    internal static TweenRunner GetRunner(SceneTree tree)
    {
        ValidateTree(tree);
        if (runners.TryGetValue(tree, out var existing)) return existing;
        var runner = new TweenRunner();
        runner.Initialize(tree);
        runners.Add(tree, runner);
        // Adding a sibling from _Ready can find the root busy adding children.
        Callable.From(() =>
        {
            if (GodotObject.IsInstanceValid(runner) && runners.TryGetValue(tree, out var current) && current == runner)
                tree.Root.AddChild(runner);
        }).CallDeferred();
        return runner;
    }

    internal static void Remove(SceneTree tree, TweenRunner runner)
    {
        if (runners.TryGetValue(tree, out var current) && current == runner) runners.Remove(tree);
    }

    internal static void MarkClosing(SceneTree tree) => closingTrees.GetValue(tree, static _ => new object());

    /// <summary>Number of unfinished tweens in this owner's automatic runtime.</summary>
    public static int GetActiveCount(Node owner)
    {
        ValidateOwner(owner);
        return runners.TryGetValue(owner.GetTree(), out var runner) ? runner.Scheduler.ActiveCount : 0;
    }

    internal static void Cancel(Node owner, bool descendants)
    {
        ValidateOwner(owner);
        if (runners.TryGetValue(owner.GetTree(), out var runner)) runner.Scheduler.CancelOwner(owner, descendants);
    }
}

public static partial class TweenExtensions
{
    /// <summary>Start a typed tween, automatically owned by the target's scene-tree lifetime.</summary>
    public static TweenInstance<TTarget, TValue> Tween<TTarget, TValue>(this TTarget target,
        ITweenDefinition<TTarget, TValue> definition) where TTarget : Node where TValue : struct
        => TweenRuntime.GetRunner(target).Scheduler.Add(target, definition);

    /// <summary>Start several definitions together as one group, owned by the target's scene-tree lifetime.</summary>
    public static Group Tween<TTarget>(this TTarget target, ITweenDefinition<TTarget> first,
        ITweenDefinition<TTarget> second, params ITweenDefinition<TTarget>[] rest) where TTarget : Node
    {
        ArgumentNullException.ThrowIfNull(rest);
        ITweenDefinition<TTarget>[] definitions = [first, second, .. rest];
        if (Array.IndexOf(definitions, null) >= 0)
            throw new ArgumentNullException(nameof(rest), "Definitions cannot be null.");
        var scheduler = TweenRuntime.GetRunner(target).Scheduler;
        var started = new TweenInstance[definitions.Length];
        var count = 0;
        try
        {
            for (; count < definitions.Length; count++) started[count] = definitions[count].AddTo(scheduler, target);
        }
        catch
        {
            for (var i = 0; i < count; i++) started[i].Cancel();
            throw;
        }
        return Group.Of(started);
    }

    public static void CancelTweens(this Node owner, bool includeChildren = false)
        => TweenRuntime.Cancel(owner, includeChildren);
}

// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using Godot;

namespace godotweens;

/// <summary>A manually driven scheduler, also used by the automatic Godot runner.</summary>
public sealed class TweenScheduler : IDisposable
{
    private readonly List<TweenInstance> instances = [];
    private readonly int thread = System.Environment.CurrentManagedThreadId;
    private bool updating, disposed;
    /// <summary>Reported after a failed tween is cleaned up. Exceptions in observers are ignored.</summary>
    public event Action<Exception>? UnhandledException;
    public int ActiveCount
    {
        get
        {
            EnsureThread();
            var count = 0;
            foreach (var instance in instances) if (!instance.IsTerminal) count++;
            return count;
        }
    }

    public TweenInstance<TTarget, TValue> Add<TTarget, TValue>(TTarget target,
        TweenDefinition<TTarget, TValue> definition) where TTarget : class where TValue : struct
        => AddCore(target, definition, target as Node, null);

    /// <summary>Animate a separate target, binding playback to an in-tree owner node.</summary>
    public TweenInstance<TTarget, TValue> Add<TTarget, TValue>(TTarget target,
        TweenDefinition<TTarget, TValue> definition, Node owner) where TTarget : class where TValue : struct
    {
        ArgumentNullException.ThrowIfNull(owner);
        return AddCore(target, definition, owner, null);
    }

    internal TweenInstance<TTarget, TValue> AddCore<TTarget, TValue>(TTarget target,
        TweenDefinition<TTarget, TValue> definition, Node? owner, SceneTree? tree)
        where TTarget : class where TValue : struct
    {
        EnsureThread();
        ObjectDisposedException.ThrowIf(disposed, this);
        ArgumentNullException.ThrowIfNull(target);
        ArgumentNullException.ThrowIfNull(definition);
        if (target is GodotObject native)
        {
            TweenRuntime.EnsureMainThread();
            if (!GodotObject.IsInstanceValid(native))
                throw new ArgumentException("The tween target has been disposed.", nameof(target));
        }
        if (target is Node node && !ReferenceEquals(node, owner))
            throw new ArgumentException("Node targets must use their own node lifetime.", nameof(owner));
        if (owner is not null) TweenRuntime.ValidateOwner(owner);
        if (tree is not null) TweenRuntime.ValidateTree(tree);
        if (owner is not null && tree is not null && owner.GetTree() != tree)
            throw new ArgumentException("The owner must belong to the supplied scene tree.", nameof(owner));
        var instance = new TweenInstance<TTarget, TValue>(this, target, definition, owner, tree);
        if (disposed)
        {
            instance.Finish(TweenCompletionReason.RunnerDisposed);
            return instance;
        }
        // A custom getter may remove/dispose an owner or target. Observe this before binding signals.
        instances.Add(instance);
        if (instance.CheckTarget())
        {
            instance.BindLifetime();
            instance.Initialize();
        }
        return instance;
    }

    /// <summary>Advance one lane. New tweens added during callbacks wait for the next Update.</summary>
    public void Update(double delta, double? unscaledDelta = null, TweenProcessMode mode = TweenProcessMode.Process)
    {
        EnsureThread();
        ObjectDisposedException.ThrowIf(disposed, this);
        Playback.Nonnegative(delta, nameof(delta));
        Playback.Nonnegative(unscaledDelta ?? delta, nameof(unscaledDelta));
        if (!Enum.IsDefined(mode)) throw new ArgumentOutOfRangeException(nameof(mode));
        if (updating) throw new InvalidOperationException("Recursive scheduler updates are not supported.");
        updating = true;
        try
        {
            var count = instances.Count;
            for (var i = 0; i < count && !disposed; i++)
            {
                var instance = instances[i];
                // Lifetimes are checked even for a paused or different-lane tween.
                if (instance.CheckTarget() && instance.Mode == mode && instance.CanAdvance())
                    instance.Advance(instance.Unscaled ? unscaledDelta ?? delta : delta);
            }
        }
        finally
        {
            updating = false;
            Compact();
        }
    }

    public void CancelAll()
    {
        EnsureThread();
        var count = instances.Count;
        for (var i = 0; i < count && i < instances.Count; i++) instances[i].Cancel();
    }

    internal void CancelOwner(Node owner, bool descendants)
    {
        var count = instances.Count;
        for (var i = 0; i < count && i < instances.Count; i++)
        {
            var instance = instances[i];
            if (instance.IsTerminal) continue;
            var node = instance.Owner;
            if (node == owner || (descendants && GodotObject.IsInstanceValid(node) && owner.IsAncestorOf(node)))
                instance.Cancel();
        }
    }

    private void Compact()
    {
        var kept = 0;
        for (var i = 0; i < instances.Count; i++)
            if (!instances[i].IsTerminal) instances[kept++] = instances[i];
        if (kept < instances.Count) instances.RemoveRange(kept, instances.Count - kept);
    }

    internal void EnsureThread()
    {
        if (thread != System.Environment.CurrentManagedThreadId)
            throw new InvalidOperationException("Use the scheduler on its creating thread (Godot's main thread for node tweens).");
    }

    internal void Report(Exception error)
    {
        try { UnhandledException?.Invoke(error); }
        catch { /* An error observer must not interrupt other tweens or teardown. */ }
    }

    public void Dispose()
    {
        EnsureThread();
        if (disposed) return;
        disposed = true;
        var count = instances.Count;
        for (var i = 0; i < count; i++) instances[i].Finish(TweenCompletionReason.RunnerDisposed);
        if (!updating) instances.Clear();
        UnhandledException = null;
    }
}

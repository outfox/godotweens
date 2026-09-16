// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using Godot;

namespace GodotWeens;

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
    {
        EnsureThread();
        ObjectDisposedException.ThrowIf(disposed, this);
        ArgumentNullException.ThrowIfNull(target);
        ArgumentNullException.ThrowIfNull(definition);
        var owner = target as Node;
        if (owner is not null) TweenRuntime.ValidateOwner(owner);
        var instance = new TweenInstance<TTarget, TValue>(this, target, definition, owner);
        instances.Add(instance);
        instance.BindLifetime();
        instance.Initialize();
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

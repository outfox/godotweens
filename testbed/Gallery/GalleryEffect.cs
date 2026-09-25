// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using tweens.gd;
namespace testbed;

/// <summary>One card of a gallery page: builds its visuals on a stage, then plays and owns its tweens.</summary>
public abstract partial class GalleryEffect
{
    private const double DefaultSeconds = 1.8;
    private readonly List<TweenInstance> handles = [];
    private readonly List<Resource> resources = [];

    public abstract string Title { get; }
    public abstract string Caption { get; }

    /// <summary>The async choreography started by Animate, if the effect has one.</summary>
    public Task? Sequence { get; protected set; }
    public int ActiveCount => handles.Count(h => !h.IsTerminal);
    public bool AllPaused => handles.Where(h => !h.IsTerminal).All(h => h.IsPaused);
    public string? Error => handles.FirstOrDefault(h => h.Error is not null)?.Error?.Message;

    /// <summary>The card's drawing area. Resource tweens bind to it, so they stop with the page.</summary>
    protected Control Stage { get; private set; } = null!;
    protected double Seconds { get; private set; }
    protected EaseType Ease { get; private set; }
    protected bool PingPong { get; private set; }

    /// <summary>Incremented by Stop. Async steps compare it with the run they started in.</summary>
    protected int Generation { get; private set; }

    /// <summary>Timing scale relative to the default leg duration, for choreographed sequences.</summary>
    protected double Tempo => Seconds / DefaultSeconds;

    public void Attach(Control stage)
    {
        Stage = stage;
        Build();
    }

    public void Start(double seconds, EaseType ease, bool pingPong)
    {
        Seconds = seconds;
        Ease = ease;
        PingPong = pingPong;
        Animate();
    }

    protected abstract void Build();
    protected abstract void Animate();

    public virtual void Pause(bool paused)
    {
        foreach (var handle in handles) handle.IsPaused = paused;
    }

    public virtual void Stop()
    {
        Generation++;
        foreach (var handle in handles.ToArray()) handle.Cancel();
        handles.Clear();
    }

    /// <summary>Called after the page is freed, so no node still references these resources.</summary>
    public void ReleaseResources()
    {
        foreach (var resource in resources) resource.Dispose();
        resources.Clear();
    }

    /// <summary>The shared repeating timing: selected easing and ping-pong, looping forever.</summary>
    protected void Cycle(TweenOptions options)
    {
        options.Ease = Ease;
        options.UsePingPong = PingPong;
        options.Repeats = TweenOptions.Infinite;
        options.RepeatInterval = 0.25;
        options.PingPongInterval = 0.15;
    }

    /// <summary>Cycle, starting <paramref name="delay"/> seconds late. Used for echoes and trails.</summary>
    protected Action<TweenOptions> CycleAfter(double delay) => options =>
    {
        Cycle(options);
        options.Delay = delay;
    };

    protected T Keep<T>(T handle) where T : TweenInstance
    {
        // Looping sequences create handles indefinitely; faulted ones stay so Error can report them.
        if (handles.Count > 256) handles.RemoveAll(h => h.State is TweenState.Completed or TweenState.Cancelled);
        handles.Add(handle);
        return handle;
    }

    /// <summary>Registers a group with the testbed's pause, cancellation, and error controls.</summary>
    protected void TrackTweens(IEnumerable<TweenInstance> tweens)
    {
        foreach (var tween in tweens) Keep(tween);
    }

    protected T Own<T>(T resource) where T : Resource
    {
        resources.Add(resource);
        return resource;
    }

    /// <summary>Tracks a new group; true when all complete without the effect being stopped since <paramref name="run"/>.</summary>
    protected async Task<bool> Finished(int run, params TweenInstance[] tweens)
    {
        TrackTweens(tweens);
        // A group hands its timeline to the next step; Task.WhenAll would drop up to a frame per step.
        var reason = await Group.Of(tweens).End;
        return run == Generation && reason == Reason.Completed;
    }

    /// <summary>Tracks a newly created group for the playback controls, then awaits its completion results.</summary>
    protected Task<bool> Finished(params TweenInstance[] tweens) => Finished(Generation, tweens);

    protected Task<bool> Wait(int run, double seconds) => Finished(run, Stage.TweenFloat(1, seconds, t => t.From = 0));

    protected Task<bool> Wait(double seconds) => Finished(Stage.TweenFloat(1, seconds, t => t.From = 0));

    /// <summary>Repeats completion-driven steps without exposing testbed run identifiers to the example.</summary>
    protected Task Repeat(Func<Task<bool>> step) => Repeat(_ => step());

    /// <summary>Repeats an async step until it reports false, e.g. after Stop.</summary>
    protected async Task Repeat(Func<int, Task<bool>> step)
    {
        var run = Generation;
        try
        {
            while (run == Generation && await step(run)) { }
        }
        catch (Exception error)
        {
            GD.PushError(error.ToString());
        }
    }

    /// <summary>Decaying oscillation that ends at the start value; use as EaseFunction for shakes.</summary>
    protected static float Shake(float t) => MathF.Sin(t * 42) * (1 - t) * (1 - t);
}

// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using Godot;
namespace tweens.gd;

public static partial class TweenExtensions
{
    /// <summary>Animate the exact resource in a scene tree. Without an owner, playback follows the tree lifetime.</summary>
    public static TweenInstance<TResource, TValue> Tween<TResource, TValue>(this TResource target,
        TweenDefinition<TResource, TValue> definition, SceneTree tree, Node? owner = null)
        where TResource : Resource where TValue : struct
        => TweenRuntime.GetRunner(tree).Scheduler.AddCore(target, definition, owner, tree);

    /// <summary>Animate the exact resource, binding playback to an owner node. Shared resources remain shared.</summary>
    public static TweenInstance<TResource, TValue> Tween<TResource, TValue>(this TResource target,
        TweenDefinition<TResource, TValue> definition, Node owner)
        where TResource : Resource where TValue : struct
    {
        TweenRuntime.ValidateOwner(owner);
        return target.Tween(definition, owner.GetTree(), owner);
    }

    /// <summary>Animate a resource using this node's playback lifetime.</summary>
    public static TweenInstance<TResource, TValue> Tween<TResource, TValue>(this Node owner,
        TResource target, TweenDefinition<TResource, TValue> definition)
        where TResource : Resource where TValue : struct
        => target.Tween(definition, owner);
}

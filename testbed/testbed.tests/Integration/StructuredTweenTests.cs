// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using Godot;
using tweens.gd;
using twodog.Testing;
using twodog.Testing.Xunit;

namespace testbed.Tests.Integration;

[Collection<HeadlessCollection>]
public class StructuredTweenTests(HeadlessFixture godot)
{
    [Fact]
    public async Task NodeEntryPointInfersBaseTargetAndValueTypesAndGroupsMixedValues()
    {
        var node = new Sprite2D();
        godot.Tree.Root.AddChild(node);
        try
        {
            TweenInstance<Node2D, Vector2> movement = node.Tween(new Tweens.Position2D
            {
                To = new Vector2(10, 20), Duration = 1,
            });
            TweenInstance<CanvasItem, float> fade = node.Tween(new Tweens.ModulateAlpha
            {
                To = 0, Duration = 1,
            });
            var scheduler = TweenRuntime.GetRunner(node).Scheduler;
            scheduler.Update(1);
            Assert.Equal(Reason.Completed, await Group.Of([movement, fade]).End);
            Assert.Equal(new Vector2(10, 20), node.Position);
            Assert.Equal(0, node.Modulate.A);

            var group = node.Tween(
                new Tweens.Position2D { To = Vector2.Zero, Duration = 1 },
                new Tweens.ModulateAlpha { To = 1, Duration = 1 });
            scheduler.Update(1);
            Assert.Equal(Reason.Completed, await group.End);
            Assert.Equal(Vector2.Zero, node.Position);
            Assert.Equal(1, node.Modulate.A);
        }
        finally { node.Free(); }
    }

    [Fact]
    public async Task ResourceEntryPointsPreserveTreeAndOwnerLifetimes()
    {
        using var material = new StandardMaterial3D();
        material.Roughness = 0;
        var owner = new Node();
        godot.Tree.Root.AddChild(owner);
        try
        {
            var roughness = new Tweens.MaterialRoughness { To = 1, Duration = 1 };
            TweenInstance<BaseMaterial3D, float> tree = material.Tween(roughness, godot.Tree);
            var bound = material.Tween(new Tweens.MaterialMetallic { To = 1, Duration = 1 }, owner);
            var ownerFirst = owner.Tween(material, new Tweens.MaterialAlbedoAlpha { To = 0, Duration = 1 });
            godot.Tree.Root.RemoveChild(owner);
            Assert.Equal(Reason.OwnerExited, await bound.End);
            Assert.Equal(Reason.OwnerExited, await ownerFirst.End);
            Assert.False(tree.IsTerminal);
            TweenRuntime.GetRunner(godot.Tree).Scheduler.Update(1);
            Assert.Equal(Reason.Completed, await tree.End);
            Assert.Equal(1, material.Roughness);
        }
        finally { owner.Free(); }
    }
}

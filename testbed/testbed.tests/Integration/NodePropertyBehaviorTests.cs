// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using System.Reflection;
using Godot;
using tweens.gd;
using twodog.Testing;
using twodog.Testing.Xunit;

namespace testbed.Tests;

[Collection<HeadlessCollection>]
public class NodePropertyBehaviorTests(HeadlessFixture godot)
{
    [Fact]
    public void EveryBuiltInDefinitionHasAContractAndOneConvenienceExtension()
    {
        var special = new[] { typeof(ControlAnchorMinTween), typeof(ControlAnchorMaxTween), typeof(ControlOffsetsTween),
            typeof(GlobalQuaternion3DTween), typeof(FloatTween), typeof(DoubleTween), typeof(Vector2Tween),
            typeof(Vector3Tween), typeof(Vector4Tween), typeof(ColorTween), typeof(QuaternionTween), typeof(Rect2Tween) };
        var covered = ExpandedAdapterTests.Cases().Select(c => (Type)c[0]).Concat(special).ToHashSet();
        var definitions = typeof(FloatTween).Assembly.GetExportedTypes().Where(t => !t.IsAbstract && !t.ContainsGenericParameters &&
            t.BaseType is { IsGenericType: true } b && b.GetGenericTypeDefinition() == typeof(PropertyTween<,>) && typeof(Node).IsAssignableFrom(b.GetGenericArguments()[0])).ToHashSet();
        Assert.Equal(definitions.OrderBy(t => t.Name), covered.OrderBy(t => t.Name));
        foreach (var definition in definitions)
        {
            var configured = Assert.Single(typeof(TweenExtensions).GetMethods(), m => m.GetParameters().Length == 4 &&
                m.GetParameters()[3].ParameterType == typeof(Action<>).MakeGenericType(definition));
            var parameters = configured.GetParameters().Take(3).Select(p => p.ParameterType).Append(typeof(TweenOptions));
            Assert.NotNull(typeof(TweenExtensions).GetMethod(configured.Name, parameters.ToArray()));
        }
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void WorldQuaternionPreservesGlobalOriginAndScaleUnderTransformedParents(bool nonuniformParent)
    {
        var parent = new Node3D { Position = new Vector3(10, 20, 30), Rotation = new Vector3(0.2f, 0.4f, -0.1f),
            Scale = nonuniformParent ? new Vector3(2, 3, 4) : Vector3.One * 2 };
        var child = new Node3D { Position = new Vector3(3, 4, 5), Scale = new Vector3(0.7f, 1.2f, 1.8f),
            RotationOrder = EulerOrder.Zxy, Rotation = new Vector3(0.1f, 0.3f, 0.2f) };
        godot.Tree.Root.AddChild(parent); parent.AddChild(child);
        try
        {
            var start = Quaternion.FromEuler(child.GlobalRotation);
            var end = Quaternion.FromEuler(new Vector3(0.8f, -0.6f, 0.2f));
            var localScale = child.Scale;
            var origin = child.GlobalPosition;
            var scale = child.GlobalBasis.Scale;
            var tween = child.TweenGlobalQuaternion(-end, 1);
            var scheduler = TweenRuntime.GetRunner(child).Scheduler;
            scheduler.Update(0.5);
            var midway = Quaternion.FromEuler(child.GlobalRotation);
            Assert.True(Math.Abs(midway.Dot(start.Slerp(end, 0.5f))) > 0.99999f);
            ExpandedAdapterTests.Close(origin, child.GlobalPosition);
            ExpandedAdapterTests.Close(scale, child.GlobalBasis.Scale);
            if (!nonuniformParent) ExpandedAdapterTests.Close(localScale, child.Scale);
            scheduler.Update(0.5);
            Assert.True(Math.Abs(Quaternion.FromEuler(child.GlobalRotation).Dot(end)) > 0.99999f);
            ExpandedAdapterTests.Close(origin, child.GlobalPosition);
            ExpandedAdapterTests.Close(scale, child.GlobalBasis.Scale);
            Assert.Equal(TweenState.Completed, tween.State);
        }
        finally { parent.Free(); }
    }

    [Fact]
    public void GlobalScaleAndSkewRespectA2DParent()
    {
        var parent = new Node2D { Rotation = 0.3f, Scale = new Vector2(2, 3), Position = new Vector2(10, 20) };
        var child = new Node2D { Position = new Vector2(4, 5) };
        godot.Tree.Root.AddChild(parent); parent.AddChild(child);
        try
        {
            var origin = child.GlobalPosition;
            var rotation = child.GlobalRotation;
            var scale = child.GlobalScale;
            child.TweenGlobalScale(new Vector2(4, 6), 1);

            var scheduler = TweenRuntime.GetRunner(child).Scheduler;
            scheduler.Update(0.5);
            ExpandedAdapterTests.Close((scale + new Vector2(4, 6)) / 2, child.GlobalScale);
            child.CancelTweens();
            var expected = new Node2D { Transform = child.Transform };
            parent.AddChild(expected);
            expected.GlobalSkew = 0.2f;
            child.TweenGlobalSkew(0.4f, 1);
            scheduler.Update(0.5);
            // Godot decomposes skew relative to its parent; compare the native setter contract.
            ExpandedAdapterTests.Close(expected.GlobalScale, child.GlobalScale);
            Assert.InRange(Math.Abs(child.GlobalSkew - expected.GlobalSkew), 0, 0.0001f);
            ExpandedAdapterTests.Close(origin, child.GlobalPosition);
            Assert.InRange(Math.Abs(child.GlobalRotation - rotation), 0, 0.0001f);
        }
        finally { parent.Free(); }
    }

    [Fact]
    public void ComponentExtensionsComposeAndKeepExternallyChangedComponents()
    {
        var camera = new Camera2D { Zoom = new Vector2(1, 2) };
        var sprite = new Sprite3D { Modulate = new Color(0.2f, 0.3f, 0.4f, 0.8f) };
        godot.Tree.Root.AddChild(camera); godot.Tree.Root.AddChild(sprite);
        try
        {
            camera.TweenZoomX(3, 1);
            camera.TweenZoomY(6, 1);
            sprite.TweenModulateAlpha(0, 1);
            sprite.Modulate = new Color(0.7f, 0.1f, 0.6f, 0.8f);
            TweenRuntime.GetRunner(camera).Scheduler.Update(0.5);
            ExpandedAdapterTests.Close(new Vector2(2, 4), camera.Zoom);
            ExpandedAdapterTests.Close(new Color(0.7f, 0.1f, 0.6f, 0.4f), sprite.Modulate);
        }
        finally { camera.Free(); sprite.Free(); }
    }

    [Fact]
    public void PathsMoveFollowersAndApplyOffsetsInSceneUnits()
    {
        using var scene2 = new ExpandedAdapterTests.TestScene(godot, typeof(PathFollow2D));
        using var scene3 = new ExpandedAdapterTests.TestScene(godot, typeof(PathFollow3D));
        var follower2 = (PathFollow2D)scene2.Target;
        var follower3 = (PathFollow3D)scene3.Target;
        follower2.TweenProgressRatio(1, 1);
        follower3.TweenProgress(100, 1);
        TweenRuntime.GetRunner(follower2).Scheduler.Update(0.5);
        ExpandedAdapterTests.Close(new Vector2(50, 0), follower2.Position);
        ExpandedAdapterTests.Close(new Vector3(50, 0, 0), follower3.Position);
        Assert.InRange(Math.Abs(follower3.ProgressRatio - 0.5f), 0, 0.0001f);
        TweenRuntime.GetRunner(follower2).Scheduler.Update(0.5);
        ExpandedAdapterTests.Close(new Vector2(100, 0), follower2.Position);
        ExpandedAdapterTests.Close(new Vector3(100, 0, 0), follower3.Position);
    }

    [Fact]
    public void LayoutEdgesPushOppositesAndOffsetTransformsWorkInContainers()
    {
        var container = new HBoxContainer { Size = new Vector2(300, 100) };
        var control = new Control { CustomMinimumSize = new Vector2(100, 40), OffsetTransformEnabled = true };
        godot.Tree.Root.AddChild(container); container.AddChild(control);
        try
        {
            godot.Engine.Iteration();
            var position = control.Position;
            var size = control.Size;
            control.TweenOffsetTransformPosition(new Vector2(20, 30), 1);
            control.TweenOffsetTransformScale(new Vector2(2, 3), 1);
            TweenRuntime.GetRunner(control).Scheduler.Update(0.5);
            ExpandedAdapterTests.Close(new Vector2(10, 15), control.OffsetTransformPosition);
            ExpandedAdapterTests.Close(new Vector2(1.5f, 2), control.OffsetTransformScale);
            ExpandedAdapterTests.Close(position, control.Position);
            ExpandedAdapterTests.Close(size, control.Size);
        }
        finally { container.Free(); }
        using var scene = new ExpandedAdapterTests.TestScene(godot, typeof(Control));
        var node = (Control)scene.Target;
        node.AnchorRight = 0.2f;
        node.TweenAnchorLeft(0.8f, 1);
        TweenRuntime.GetRunner(node).Scheduler.Update(1);
        Assert.Equal(0.8f, node.AnchorLeft);
        Assert.Equal(0.8f, node.AnchorRight);
        node.CancelTweens();
        node.TweenAnchorMin(new Vector2(0.1f, 0.2f), 0);
        node.TweenAnchorMax(new Vector2(0.8f, 0.9f), 0);
        node.TweenOffsets(new Vector4(10, 20, 30, 40), 0);
        TweenRuntime.GetRunner(node).Scheduler.Update(0);
        ExpandedAdapterTests.Close(0.1f, node.AnchorLeft); ExpandedAdapterTests.Close(0.2f, node.AnchorTop);
        ExpandedAdapterTests.Close(0.8f, node.AnchorRight); ExpandedAdapterTests.Close(0.9f, node.AnchorBottom);
        Assert.Equal(10, node.OffsetLeft); Assert.Equal(20, node.OffsetTop);
        Assert.Equal(30, node.OffsetRight); Assert.Equal(40, node.OffsetBottom);
    }

    [Fact]
    public void ExtensionsApplyConfigurationBeforeStartingAndUseOwnerLifetime()
    {
        var camera = new Camera2D(); godot.Tree.Root.AddChild(camera);
        try
        {
            var callbacks = 0;
            Camera2DZoomTween? captured = null;
            var tween = camera.TweenZoom(new Vector2(9, 9), 1, definition =>
            {
                captured = definition;
                definition.From = new Vector2(2, 4);
                definition.To = new Vector2(6, 8);
                definition.Delay = 0.5;
                definition.Ease = EaseType.QuadIn;
                definition.OnStart = _ => callbacks++;
            });
            captured!.To = Vector2.Zero;
            var scheduler = TweenRuntime.GetRunner(camera).Scheduler;
            scheduler.Update(0.5);
            ExpandedAdapterTests.Close(new Vector2(2, 4), camera.Zoom);
            scheduler.Update(0.5);
            ExpandedAdapterTests.Close(new Vector2(3, 5), camera.Zoom);
            Assert.Equal(1, callbacks);
            tween.Pause(); camera.QueueFree(); scheduler.Update(0);
            Assert.Equal(Reason.TargetFreed, tween.CompletionReason);
            Assert.True(tween.End.IsCompletedSuccessfully);
        }
        finally { if (GodotObject.IsInstanceValid(camera)) camera.Free(); }
    }

    [Fact]
    public void ConfigurationFailureDoesNotRegisterPlayback()
    {
        var camera = new Camera2D(); godot.Tree.Root.AddChild(camera);
        try
        {
            var before = TweenRuntime.GetActiveCount(camera);
            Assert.Throws<InvalidOperationException>(() => camera.TweenZoom(Vector2.One, 1,
                _ => throw new InvalidOperationException("configuration")));
            Assert.Equal(before, TweenRuntime.GetActiveCount(camera));
        }
        finally { camera.Free(); }
    }

    [Fact]
    public void ConvenienceCallCompletesThroughEnginePumping()
    {
        var label = new Label { Text = "abcdefghij", VisibleRatio = 0 };
        godot.Tree.Root.AddChild(label);
        try
        {
            var tween = label.TweenVisibleRatio(1, 0);
            for (var i = 0; i < 10 && !tween.IsTerminal; i++) godot.Engine.Iteration();
            Assert.Equal(Reason.Completed, tween.CompletionReason);
            Assert.Equal(1, label.VisibleRatio);
            Assert.True(label.VisibleCharacters < 0 || label.VisibleCharacters == 10);
        }
        finally { label.Free(); }
    }

    [Fact]
    public void AllValueConvenienceMethodsDeliverConfiguredValues()
    {
        var node = new Node(); godot.Tree.Root.AddChild(node);
        try
        {
            float f = 0; double d = 0; Vector2 v2 = default; Vector3 v3 = default; Vector4 v4 = default;
            Color color = default; Quaternion q = default; Rect2 rect = default;
            node.TweenFloat(4, 0, x => x.OnUpdate = (_, v) => f = v);
            node.TweenDouble(8, 0, x => x.OnUpdate = (_, v) => d = v);
            node.TweenVector2(new Vector2(1, 2), 0, x => x.OnUpdate = (_, v) => v2 = v);
            node.TweenVector3(new Vector3(1, 2, 3), 0, x => x.OnUpdate = (_, v) => v3 = v);
            node.TweenVector4(new Vector4(1, 2, 3, 4), 0, x => x.OnUpdate = (_, v) => v4 = v);
            node.TweenColor(Colors.Red, 0, x => x.OnUpdate = (_, v) => color = v);
            node.TweenQuaternion(Quaternion.Identity, 0, x => x.OnUpdate = (_, v) => q = v);
            node.TweenRect2(new Rect2(1, 2, 3, 4), 0, x => x.OnUpdate = (_, v) => rect = v);
            TweenRuntime.GetRunner(node).Scheduler.Update(0);
            Assert.Equal(4, f); Assert.Equal(8, d); Assert.Equal(new Vector2(1, 2), v2);
            Assert.Equal(new Vector3(1, 2, 3), v3); Assert.Equal(new Vector4(1, 2, 3, 4), v4);
            Assert.Equal(Colors.Red, color); Assert.Equal(Quaternion.Identity, q); Assert.Equal(new Rect2(1, 2, 3, 4), rect);
        }
        finally { node.Free(); }
    }
}

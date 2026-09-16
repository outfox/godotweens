// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using System.Reflection;
using Godot;
using GodotWeens;
using twodog.Testing;
using twodog.Testing.Xunit;

namespace testbed.Tests;

[Collection<HeadlessCollection>]
public class AdapterCatalogTests(HeadlessFixture godot)
{
    // Declarative API-to-property contract. Test access is independent of adapter getters/setters.
    public static IEnumerable<object[]> Cases()
    {
        yield return [typeof(Position2DTween), "Position", ""];
        yield return [typeof(Position2DXTween), "Position", "X"];
        yield return [typeof(Position2DYTween), "Position", "Y"];
        yield return [typeof(GlobalPosition2DTween), "GlobalPosition", ""];
        yield return [typeof(GlobalPosition2DXTween), "GlobalPosition", "X"];
        yield return [typeof(GlobalPosition2DYTween), "GlobalPosition", "Y"];
        yield return [typeof(Scale2DTween), "Scale", ""];
        yield return [typeof(Scale2DXTween), "Scale", "X"];
        yield return [typeof(Scale2DYTween), "Scale", "Y"];
        yield return [typeof(Position3DTween), "Position", ""];
        yield return [typeof(Position3DXTween), "Position", "X"];
        yield return [typeof(Position3DYTween), "Position", "Y"];
        yield return [typeof(Position3DZTween), "Position", "Z"];
        yield return [typeof(GlobalPosition3DTween), "GlobalPosition", ""];
        yield return [typeof(GlobalPosition3DXTween), "GlobalPosition", "X"];
        yield return [typeof(GlobalPosition3DYTween), "GlobalPosition", "Y"];
        yield return [typeof(GlobalPosition3DZTween), "GlobalPosition", "Z"];
        yield return [typeof(Scale3DTween), "Scale", ""];
        yield return [typeof(Scale3DXTween), "Scale", "X"];
        yield return [typeof(Scale3DYTween), "Scale", "Y"];
        yield return [typeof(Scale3DZTween), "Scale", "Z"];
        yield return [typeof(Rotation3DTween), "Rotation", ""];
        yield return [typeof(Rotation3DXTween), "Rotation", "X"];
        yield return [typeof(Rotation3DYTween), "Rotation", "Y"];
        yield return [typeof(Rotation3DZTween), "Rotation", "Z"];
        yield return [typeof(GlobalRotation3DTween), "GlobalRotation", ""];
        yield return [typeof(GlobalRotation3DXTween), "GlobalRotation", "X"];
        yield return [typeof(GlobalRotation3DYTween), "GlobalRotation", "Y"];
        yield return [typeof(GlobalRotation3DZTween), "GlobalRotation", "Z"];
        yield return [typeof(Rotation2DTween), "Rotation", ""];
        yield return [typeof(GlobalRotation2DTween), "GlobalRotation", ""];
        yield return [typeof(Quaternion3DTween), "Quaternion", ""];
        yield return [typeof(ControlPositionTween), "Position", ""];
        yield return [typeof(ControlPositionXTween), "Position", "X"];
        yield return [typeof(ControlPositionYTween), "Position", "Y"];
        yield return [typeof(ControlGlobalPositionTween), "GlobalPosition", ""];
        yield return [typeof(ControlGlobalPositionXTween), "GlobalPosition", "X"];
        yield return [typeof(ControlGlobalPositionYTween), "GlobalPosition", "Y"];
        yield return [typeof(ControlSizeTween), "Size", ""];
        yield return [typeof(ControlSizeXTween), "Size", "X"];
        yield return [typeof(ControlSizeYTween), "Size", "Y"];
        yield return [typeof(ControlScaleTween), "Scale", ""];
        yield return [typeof(ControlScaleXTween), "Scale", "X"];
        yield return [typeof(ControlScaleYTween), "Scale", "Y"];
        yield return [typeof(ControlRotationTween), "Rotation", ""];
        yield return [typeof(RangeValueTween), "Value", ""];
        yield return [typeof(ModulateTween), "Modulate", ""];
        yield return [typeof(ModulateAlphaTween), "Modulate", "A"];
        yield return [typeof(SelfModulateTween), "SelfModulate", ""];
        yield return [typeof(SelfModulateAlphaTween), "SelfModulate", "A"];
        yield return [typeof(AudioVolumeDbTween), "VolumeDb", ""];
        yield return [typeof(AudioVolumeLinearTween), "VolumeLinear", ""];
        yield return [typeof(AudioPitchScaleTween), "PitchScale", ""];
        yield return [typeof(AudioVolumeDb2DTween), "VolumeDb", ""];
        yield return [typeof(AudioVolumeLinear2DTween), "VolumeLinear", ""];
        yield return [typeof(AudioPitchScale2DTween), "PitchScale", ""];
        yield return [typeof(AudioVolumeDb3DTween), "VolumeDb", ""];
        yield return [typeof(AudioVolumeLinear3DTween), "VolumeLinear", ""];
        yield return [typeof(AudioPitchScale3DTween), "PitchScale", ""];
        yield return [typeof(LightColor2DTween), "Color", ""];
        yield return [typeof(LightEnergy2DTween), "Energy", ""];
        yield return [typeof(LightColor3DTween), "LightColor", ""];
        yield return [typeof(LightEnergy3DTween), "LightEnergy", ""];
        yield return [typeof(OmniRangeTween), "OmniRange", ""];
        yield return [typeof(SpotRangeTween), "SpotRange", ""];
        yield return [typeof(SpotAngleTween), "SpotAngle", ""];
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void AdapterSamplesExpectedGodotProperty(Type adapter, string property, string component)
    {
        var arguments = adapter.BaseType!.GetGenericArguments();
        var nodeType = arguments[0];
        var concrete = nodeType == typeof(CanvasItem) ? typeof(Node2D)
            : nodeType == typeof(Light2D) ? typeof(PointLight2D)
            : nodeType == typeof(Light3D) ? typeof(OmniLight3D)
            : nodeType == typeof(Godot.Range) ? typeof(ProgressBar) : nodeType;
        var node = (Node)Activator.CreateInstance(concrete)!;
        godot.Tree.Root.AddChild(node);
        try
        {
            if (node is Godot.Range range) range.Step = 0;
            var prop = concrete.GetProperty(property)!;
            var initial = Sample(prop.PropertyType, 0.2f);
            prop.SetValue(node, initial);
            var definition = (TweenOptions)Activator.CreateInstance(adapter)!;
            definition.Duration = 1;
            var target = Sample(arguments[1], 0.6f);
            adapter.GetProperty("To")!.SetValue(definition, target);
            var method = GetType().GetMethod(nameof(Execute), BindingFlags.NonPublic | BindingFlags.Static)!;
            method.MakeGenericMethod(arguments).Invoke(null, [node, definition]);

            object expected;
            if (component.Length == 0)
                expected = Sample(prop.PropertyType, 0.4f);
            else
            {
                expected = initial;
                prop.PropertyType.GetField(component)!.SetValue(expected, 0.4f);
            }
            AssertClose(expected, prop.GetValue(node)!);
        }
        finally { node.Free(); }
    }

    private static void Execute<TTarget, TValue>(TTarget node, TweenDefinition<TTarget, TValue> definition)
        where TTarget : class where TValue : struct
    {
        using var scheduler = new TweenScheduler();
        scheduler.Add(node, definition);
        scheduler.Update(0.5);
    }

    private static object Sample(Type type, float amount)
    {
        if (type == typeof(float)) return amount;
        if (type == typeof(double)) return (double)amount;
        if (type == typeof(Vector2)) return new Vector2(amount, amount);
        if (type == typeof(Vector3)) return new Vector3(amount, amount, amount);
        if (type == typeof(Color)) return new Color(amount, amount, amount, amount);
        if (type == typeof(Quaternion)) return new Quaternion(Vector3.Up, amount);
        throw new NotSupportedException(type.Name);
    }

    private static void AssertClose(object expected, object actual)
    {
        var close = expected switch
        {
            float x => Math.Abs(x - (float)actual) < 0.0001,
            double x => Math.Abs(x - (double)actual) < 0.0001,
            Vector2 x => x.IsEqualApprox((Vector2)actual),
            Vector3 x => x.IsEqualApprox((Vector3)actual),
            Color x => x.IsEqualApprox((Color)actual),
            Quaternion x => x.IsEqualApprox((Quaternion)actual),
            _ => false,
        };
        Assert.True(close, $"Expected {expected}, got {actual}.");
    }

    [Fact]
    public void AnchorPairsAndOffsetsUseGodotLayoutUnits()
    {
        using var scheduler = new TweenScheduler();
        var parent = new Control { Size = new Vector2(1000, 1000) };
        godot.Tree.Root.AddChild(parent);
        var node = new Control(); parent.AddChild(node);
        try
        {
            scheduler.Add(node, new ControlAnchorMinTween { To = new Vector2(0.2f, 0.4f), Duration = 1 });
            scheduler.Add(node, new ControlAnchorMaxTween { To = new Vector2(0.6f, 0.8f), Duration = 1 });
            scheduler.Add(node, new ControlOffsetsTween { From = Vector4.Zero, To = new Vector4(10, 20, 30, 40), Duration = 1 });
            scheduler.Update(0.5);
            Assert.Equal(0.1f, node.AnchorLeft); Assert.Equal(0.2f, node.AnchorTop);
            Assert.Equal(0.3f, node.AnchorRight); Assert.Equal(0.4f, node.AnchorBottom);
            Assert.Equal(5, node.OffsetLeft); Assert.Equal(10, node.OffsetTop);
            Assert.Equal(15, node.OffsetRight); Assert.Equal(20, node.OffsetBottom);
        }
        finally { parent.Free(); }
    }

    [Fact]
    public void ValueTweensDeliverTypedUpdatesWithTheirDefaults()
    {
        var owner = new Node(); godot.Tree.Root.AddChild(owner);
        try
        {
            Check(new FloatTween(), 10f, 5f);
            Check(new DoubleTween(), 10d, 5d);
            Check(new Vector2Tween(), new Vector2(2, 4), new Vector2(1, 2));
            Check(new Vector3Tween(), new Vector3(2, 4, 6), new Vector3(1, 2, 3));
            Check(new Vector4Tween(), new Vector4(2, 4, 6, 8), new Vector4(1, 2, 3, 4));
            Check(new ColorTween(), new Color(1, 0, 0, 1), new Color(0.5f, 0, 0, 0.5f));
            Check(new Rect2Tween(), new Rect2(2, 4, 6, 8), new Rect2(1, 2, 3, 4));
            var q = new QuaternionTween { To = new Quaternion(Vector3.Up, 0.6f), Duration = 1 };
            Quaternion output = default; q.OnUpdate = (_, value) => output = value;
            using var scheduler = new TweenScheduler();
            scheduler.Add(owner, q); scheduler.Update(0.5);
            Assert.True(output.IsEqualApprox(new Quaternion(Vector3.Up, 0.3f)));

            void Check<T>(TweenDefinition<Node, T> definition, T target, T expected) where T : struct
            {
                definition.To = target; definition.Duration = 1;
                T output = default;
                definition.OnUpdate = (_, value) => output = value;
                using var driver = new TweenScheduler();
                driver.Add(owner, definition); driver.Update(0.5);
                Assert.Equal(expected, output);
            }
        }
        finally { owner.Free(); }
    }
}


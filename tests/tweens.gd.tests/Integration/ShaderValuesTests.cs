// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using Godot;
using twodog.Testing;
using twodog.Testing.Xunit;

namespace tweens.gd.Tests.Integration;

/// <summary>Conversions between typed tween values and shader uniform variants.</summary>
[Collection<HeadlessCollection>]
public class ShaderValuesTests(HeadlessFixture godot)
{
    [Fact]
    public void EverySupportedTypeMapsToItsVariantType()
    {
        Assert.NotNull(godot.Tree);
        Assert.Equal(Variant.Type.Float, ShaderValues<float>.Type);
        Assert.Equal(Variant.Type.Float, ShaderValues<double>.Type);
        Assert.Equal(Variant.Type.Int, ShaderValues<int>.Type);
        Assert.Equal(Variant.Type.Vector2, ShaderValues<Vector2>.Type);
        Assert.Equal(Variant.Type.Vector3, ShaderValues<Vector3>.Type);
        Assert.Equal(Variant.Type.Vector4, ShaderValues<Vector4>.Type);
        Assert.Equal(Variant.Type.Color, ShaderValues<Color>.Type);
        Assert.Throws<NotSupportedException>(() => ShaderValues<Quaternion>.Type);
    }

    [Fact]
    public void ValuesRoundTripThroughVariants()
    {
        RoundTrip(0.5f);
        RoundTrip(0.25);
        RoundTrip(-7);
        RoundTrip(new Vector2(1, 2));
        RoundTrip(new Vector3(1, 2, 3));
        RoundTrip(new Vector4(1, 2, 3, 4));
        RoundTrip(new Color(0.1f, 0.2f, 0.3f, 0.4f));

        static void RoundTrip<T>(T value) where T : struct
        {
            using var variant = ShaderValues<T>.Write(value);
            Assert.Equal(ShaderValues<T>.Type, variant.VariantType);
            Assert.Equal(value, ShaderValues<T>.Read(variant));
        }
    }

    [Fact]
    public void ReadingRejectsMismatchedAndOutOfRangeVariants()
    {
        using var text = Variant.From("text");
        var mismatch = Assert.Throws<ArgumentException>(() => ShaderValues<float>.Read(text));
        Assert.Contains("String", mismatch.Message);
        using var huge = Variant.From(long.MaxValue);
        Assert.Throws<OverflowException>(() => ShaderValues<int>.Read(huge));
        using var nan = Variant.From(float.NaN);
        Assert.Throws<ArgumentException>(() => ShaderValues<float>.Read(nan));
        Assert.Throws<NotSupportedException>(() => ShaderValues<Quaternion>.Read(text));
    }

    [Fact]
    public void ValidationRequiresFiniteComponents()
    {
        ShaderValues<int>.Validate(int.MaxValue);
        Assert.Throws<ArgumentException>(() => ShaderValues<float>.Validate(float.PositiveInfinity));
        Assert.Throws<ArgumentException>(() => ShaderValues<double>.Validate(double.NaN));
        Assert.Throws<ArgumentException>(() => ShaderValues<Vector2>.Validate(new Vector2(float.NaN, 0)));
        Assert.Throws<ArgumentException>(() => ShaderValues<Vector3>.Validate(new Vector3(0, float.NaN, 0)));
        Assert.Throws<ArgumentException>(() => ShaderValues<Vector4>.Validate(new Vector4(0, 0, float.NaN, 0)));
        foreach (var color in new[] { new Color(float.NaN, 0, 0), new Color(0, float.NaN, 0), new Color(0, 0, float.NaN), new Color(0, 0, 0, float.NaN) })
            Assert.Throws<ArgumentException>(() => ShaderValues<Color>.Validate(color));
        Assert.Throws<NotSupportedException>(() => ShaderValues<Quaternion>.Validate(Quaternion.Identity));
        Assert.Throws<ArgumentException>(() => ShaderValues<float>.Write(float.NaN));
    }

    [Fact]
    public void InterpolationUsesTheTypedInterpolators()
    {
        Assert.Equal(1.5f, ShaderValues<float>.Interpolate(1, 2, 0.5f));
        Assert.Equal(1.5, ShaderValues<double>.Interpolate(1, 2, 0.5f));
        Assert.Equal(2, ShaderValues<int>.Interpolate(1, 2, 0.5f));
        Assert.Equal(new Vector2(1, 1), ShaderValues<Vector2>.Interpolate(Vector2.Zero, new Vector2(2, 2), 0.5f));
        Assert.Equal(new Vector3(1, 1, 1), ShaderValues<Vector3>.Interpolate(Vector3.Zero, new Vector3(2, 2, 2), 0.5f));
        Assert.Equal(new Vector4(1, 1, 1, 1), ShaderValues<Vector4>.Interpolate(Vector4.Zero, new Vector4(2, 2, 2, 2), 0.5f));
        Assert.Equal(new Color(0.5f, 0.5f, 0.5f, 0.5f), ShaderValues<Color>.Interpolate(new Color(0, 0, 0, 0), new Color(1, 1, 1, 1), 0.5f));
        Assert.Throws<NotSupportedException>(() => ShaderValues<Quaternion>.Interpolate(Quaternion.Identity, Quaternion.Identity, 0.5f));
    }
}

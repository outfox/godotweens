// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using Godot;

namespace tweens.gd;

/// <summary>Value interpolation preserving easing overshoot; integer results saturate at Int32 limits.</summary>
public static class Interpolators
{
    /// <summary>Rounds to nearest with ties away from zero; overshoot saturates at Int32 limits.</summary>
    public static int Int(int from, int to, float weight)
    {
        if (!float.IsFinite(weight)) throw new ArgumentOutOfRangeException(nameof(weight));
        var value = from + ((double)to - from) * weight;
        return (int)Math.Clamp(Math.Round(value, MidpointRounding.AwayFromZero), int.MinValue, int.MaxValue);
    }

    public static float Float(float from, float to, float weight) => from + (to - from) * weight;
    public static double Double(double from, double to, float weight) => from + (to - from) * weight;
    public static Vector2 Vector2(Vector2 from, Vector2 to, float weight) => from.Lerp(to, weight);
    public static Vector3 Vector3(Vector3 from, Vector3 to, float weight) => from.Lerp(to, weight);
    public static Vector4 Vector4(Vector4 from, Vector4 to, float weight) => from.Lerp(to, weight);
    public static Color Color(Color from, Color to, float weight) => from.Lerp(to, weight);
    public static Rect2 Rect2(Rect2 from, Rect2 to, float weight)
        => new(from.Position.Lerp(to.Position, weight), from.Size.Lerp(to.Size, weight));
    public static Quaternion Quaternion(Quaternion from, Quaternion to, float weight)
    {
        if (from.LengthSquared() == 0 || to.LengthSquared() == 0)
            throw new ArgumentException("Quaternion endpoints must be nonzero.");
        return from.Normalized().Slerp(to.Normalized(), weight).Normalized();
    }
}

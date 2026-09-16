// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using Godot;

namespace GodotWeens;

/// <summary>Unclamped value interpolation, preserving back/elastic overshoot.</summary>
public static class Interpolators
{
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

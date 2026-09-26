// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using Godot;

namespace tweens.gd.Tests.Support;

/// <summary>Sample values and tolerant comparison for every value type the adapters animate.</summary>
public static class Values
{
    /// <summary>A nearby value inside the property ranges Godot enforces for default-initialized nodes.</summary>
    public static object Perturb(object value) => value switch
    {
        float x => Nudge(x),
        double x => (double)Nudge((float)x),
        int x => x + 3,
        Vector2 x => new Vector2(Nudge(x.X), Nudge(x.Y)),
        Vector3 x => new Vector3(Nudge(x.X), Nudge(x.Y), Nudge(x.Z)),
        Vector4 x => new Vector4(Nudge(x.X), Nudge(x.Y), Nudge(x.Z), Nudge(x.W)),
        Color x => new Color(Nudge(x.R), Nudge(x.G), Nudge(x.B), Nudge(x.A)),
        Quaternion x => (x * new Quaternion(Vector3.Up, 0.5f)).Normalized(),
        Rect2 x => new Rect2(x.Position + new Vector2(0.5f, 0.5f), x.Size + new Vector2(0.5f, 0.5f)),
        _ => throw new NotSupportedException(value.GetType().Name),
    };

    // Zero and negative sentinels (such as -1 for "unlimited") move to a small positive value instead.
    private static float Nudge(float x) => x <= 0 ? 0.5f : x * 0.75f;

    public static object Interpolate(object from, object to, float weight) => from switch
    {
        float x => Interpolators.Float(x, (float)to, weight),
        double x => Interpolators.Double(x, (double)to, weight),
        int x => Interpolators.Int(x, (int)to, weight),
        Vector2 x => Interpolators.Vector2(x, (Vector2)to, weight),
        Vector3 x => Interpolators.Vector3(x, (Vector3)to, weight),
        Vector4 x => Interpolators.Vector4(x, (Vector4)to, weight),
        Color x => Interpolators.Color(x, (Color)to, weight),
        Quaternion x => Interpolators.Quaternion(x, (Quaternion)to, weight),
        Rect2 x => Interpolators.Rect2(x, (Rect2)to, weight),
        _ => throw new NotSupportedException(from.GetType().Name),
    };

    public static bool Close(object expected, object actual, float tolerance = 1e-3f) => expected switch
    {
        float x => Math.Abs(x - (float)actual) <= tolerance * Math.Max(1, Math.Abs(x)),
        double x => Math.Abs(x - (double)actual) <= tolerance * Math.Max(1, Math.Abs(x)),
        int x => x == (int)actual,
        Vector2 x => (x - (Vector2)actual).Length() <= tolerance * Math.Max(1, x.Length()),
        Vector3 x => (x - (Vector3)actual).Length() <= tolerance * Math.Max(1, x.Length()),
        Vector4 x => (x - (Vector4)actual).Length() <= tolerance * Math.Max(1, x.Length()),
        Color x => Close(new Vector4(x.R, x.G, x.B, x.A), ToVector((Color)actual), tolerance),
        Quaternion x => Math.Abs(x.Normalized().Dot(((Quaternion)actual).Normalized())) >= 1 - tolerance,
        Rect2 x => Close(x.Position, ((Rect2)actual).Position, tolerance) && Close(x.Size, ((Rect2)actual).Size, tolerance),
        _ => throw new NotSupportedException(expected.GetType().Name),
    };

    private static Vector4 ToVector(Color color) => new(color.R, color.G, color.B, color.A);

    public static void AssertClose(object expected, object actual, string because = "")
        => Assert.True(Close(expected, actual), $"Expected {expected}, got {actual}. {because}");
}

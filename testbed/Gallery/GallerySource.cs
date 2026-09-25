// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using System;
using System.IO;
using System.Linq;
namespace testbed;

/// <summary>Source from this build, available without a checkout or exported loose .cs files.</summary>
public sealed record GallerySource(string Path, string Text)
{
    public int TweenLine => Path.EndsWith(".Animation.cs", StringComparison.Ordinal)
        ? Array.FindIndex(Text.Split('\n'), line => line.StartsWith("public sealed partial class ", StringComparison.Ordinal)) + 2
        : Array.FindIndex(Text.Split('\n'), line => line.Contains("protected override void Animate()", StringComparison.Ordinal));

    public static GallerySource ForEffect(GalleryEffect effect)
    {
        var type = effect.GetType();
        // The nested headless-renderer notice is a placeholder, not an animated effect.
        return type.DeclaringType is { } parent ? Load(parent.Name + ".cs") : Load(type.Name + ".Animation.cs");
    }

    public static GallerySource ForSetup(GalleryEffect effect) =>
        Load((effect.GetType().DeclaringType ?? effect.GetType()).Name + ".cs");

    public static GallerySource Load(string fileName)
    {
        var assembly = typeof(GallerySource).Assembly;
        var resource = assembly.GetManifestResourceNames().Single(name =>
            name.StartsWith("GallerySource/", StringComparison.Ordinal) &&
            name.Replace('\\', '/').EndsWith("/" + fileName, StringComparison.Ordinal));
        using var stream = assembly.GetManifestResourceStream(resource)!;
        using var reader = new StreamReader(stream);
        return new GallerySource(resource.Replace('\\', '/').Replace("GallerySource/", "Gallery/"),
            reader.ReadToEnd().Replace("\r\n", "\n"));
    }
}

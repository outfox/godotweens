// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using System;
using System.IO;
using System.Linq;
namespace testbed;

/// <summary>Source from this build, available without a checkout or exported loose .cs files.</summary>
public sealed record GallerySource(string Path, string Text)
{
    public int TweenLine => Array.FindIndex(Text.Split('\n'), line => line.Contains("protected override void Animate()", StringComparison.Ordinal));

    public static GallerySource ForEffect(GalleryEffect effect)
    {
        var type = effect.GetType();
        return Load((type.DeclaringType ?? type).Name + ".cs");
    }

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

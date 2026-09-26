// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using Godot;
using Engine = twodog.Engine;

namespace testbed;

internal static class Program
{
    // STA matches how godot.exe runs its main thread on Windows: OLE (drag & drop,
    // IME, native dialogs) fails to initialize on the MTA thread .NET uses by default.
    // No effect on Linux/macOS.
    [STAThread]
    private static void Main(string[] args)
    {
        var options = ParseArguments(args);
        Run(options);
        if (options.RestartCheck) Run(options with { Snapshot = null, GallerySnapshots = null });
    }

    private sealed record LaunchOptions(string[] Forwarded, string? Snapshot, string? GallerySnapshots,
        int GalleryPage, int GallerySource, bool RestartCheck);

    private static LaunchOptions ParseArguments(string[] args)
    {
        var forwarded = new List<string>();
        string? snapshot = null;
        var restartCheck = false;
        string? gallerySnapshots = null;
        var galleryPage = 0;
        var gallerySource = -1;
        for (var i = 0; i < args.Length; i++)
        {
            switch (args[i])
            {
                case "--snapshot" when i + 1 < args.Length:
                    snapshot = Path.GetFullPath(args[++i]);
                    break;
                case "--gallery-snapshots" when i + 1 < args.Length:
                    gallerySnapshots = Path.GetFullPath(args[++i]);
                    break;
                case "--gallery-page" when i + 1 < args.Length:
                    galleryPage = int.Parse(args[++i]);
                    break;
                case "--gallery-source" when i + 1 < args.Length:
                    gallerySource = int.Parse(args[++i]);
                    break;
                case "--restart-check":
                    restartCheck = true;
                    break;
                default:
                    forwarded.Add(args[i]);
                    break;
            }
        }
        return new LaunchOptions(forwarded.ToArray(), snapshot, gallerySnapshots, galleryPage, gallerySource, restartCheck);
    }

    private static void Run(LaunchOptions options)
    {
        // The default constructor finds raw project content during development
        // and the exe-adjacent .pck after publish. Arguments are forwarded to Godot.
        using var engine = new Engine("testbed", args: options.Forwarded);
        engine.Start();
        PrintStartup(engine.Tree);

        var gallery = engine.Tree.CurrentScene as TweenDemo;
        SelectPage(gallery, options.GallerySnapshots is null ? options.GalleryPage : 0, options.GallerySource);
        var capturedPages = 0;

        // Iteration() returns true when Godot wants to quit.
        var frame = 0;
        while (!engine.Iteration())
        {
            frame++;
            if (frame == 20 && options.Snapshot is { } snapshot)
            {
                SaveScreenshot(engine.Tree, snapshot);
                Console.WriteLine($"Saved {snapshot}");
                engine.Tree.Quit();
            }
            if (options.GallerySnapshots is null || frame % 60 != 0 || gallery is null) continue;
            var target = Path.Combine(options.GallerySnapshots, $"{capturedPages + 1:00}.png");
            SaveScreenshot(engine.Tree, target);
            Console.WriteLine($"Captured {TweenDemo.PageNames[capturedPages]}");
            if (++capturedPages == TweenDemo.PageNames.Length) engine.Tree.Quit();
            else SelectPage(gallery, capturedPages, options.GallerySource);
        }

        Console.WriteLine("Shutting down...");
    }

    private static void PrintStartup(SceneTree tree)
    {
        if (tree.CurrentScene is { } scene)
            GD.Print($"2dog is running '{scene.Name}'!");
        else
            GD.Print("2dog is running (no run/main_scene set in project.godot).");
        Console.WriteLine("Close the window to quit.");
    }

    private static void SelectPage(TweenDemo? gallery, int page, int source)
    {
        gallery?.SelectPage(page);
        if (source >= 0) gallery?.CurrentPage?.ShowSource(source);
    }

    private static void SaveScreenshot(SceneTree tree, string path)
    {
        using var image = tree.Root.GetTexture().GetImage();
        if (image.IsEmpty()) throw new InvalidOperationException("Screenshot requires a rendering display driver.");
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        var result = image.SavePng(path);
        if (result != Error.Ok) throw new IOException($"Screenshot failed: {result} ({path})");
    }
}

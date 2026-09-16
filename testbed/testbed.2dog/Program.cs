// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using Godot;
using Engine = twodog.Engine;

internal static class Program
{
    // STA matches how godot.exe runs its main thread on Windows: OLE (drag & drop,
    // IME, native dialogs) fails to initialize on the MTA thread .NET uses by default.
    // No effect on Linux/macOS.
    [STAThread]
    private static void Main(string[] args)
    {
        var forwarded = new List<string>();
        string? snapshot = null;
        var restartCheck = false;
        for (var i = 0; i < args.Length; i++)
        {
            if (args[i] == "--snapshot" && i + 1 < args.Length) snapshot = Path.GetFullPath(args[++i]);
            else if (args[i] == "--restart-check") restartCheck = true;
            else forwarded.Add(args[i]);
        }
        Run(forwarded.ToArray(), snapshot);
        if (restartCheck) Run(forwarded.ToArray(), null);
    }

    private static void Run(string[] args, string? snapshot)
    {
        // The default constructor finds raw project content during development
        // and the exe-adjacent .pck after publish. Arguments are forwarded to Godot.
        using var engine = new Engine("testbed", args: args);
        engine.Start();

        if (engine.Tree.CurrentScene is { } scene)
            GD.Print($"2dog is running '{scene.Name}'!");
        else
            GD.Print("2dog is running (no run/main_scene set in project.godot).");
        Console.WriteLine("Close the window to quit.");

        // Iteration() returns true when Godot wants to quit.
        var frame = 0;
        while (!engine.Iteration())
        {
            if (++frame == 20 && snapshot is not null)
            {
                using var image = engine.Tree.Root.GetTexture().GetImage();
                if (image.IsEmpty()) throw new InvalidOperationException("Screenshot requires a rendering display driver.");
                Directory.CreateDirectory(Path.GetDirectoryName(snapshot)!);
                var result = image.SavePng(snapshot);
                if (result != Error.Ok) throw new IOException($"Screenshot failed: {result}");
                Console.WriteLine($"Saved {snapshot}");
                engine.Tree.Quit();
            }
        }

        Console.WriteLine("Shutting down...");
    }
}

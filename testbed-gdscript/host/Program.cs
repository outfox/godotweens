// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss
using Godot;

internal static class Program
{
    [STAThread]
    private static int Main(string[] args)
    {
        // Engine.Start changes cwd to the Godot project; resolve caller paths first.
        var outputIndex = Array.IndexOf(args, "--benchmark-output");
        var benchmarkOutput = outputIndex >= 0 && outputIndex + 1 < args.Length
            ? Path.GetFullPath(args[outputIndex + 1]) : null;
        using var engine = new twodog.Engine("gdscript.2dog", args: ["--headless", "--fixed-fps", "60"]);
        engine.Start();
        foreach (var source in Directory.EnumerateFiles(ProjectSettings.GlobalizePath("res://addons/tweens_gd"), "*.gd"))
        {
            var script = ResourceLoader.Load<GDScript>($"res://addons/tweens_gd/{Path.GetFileName(source)}");
            if (script is null || !script.CanInstantiate())
            {
                Console.Error.WriteLine($"GDScript failed to compile: {source}");
                return 1;
            }
        }
        var tests = engine.Tree.CurrentScene;
        if (tests is null || !tests.HasMethod("run_tests"))
        {
            Console.Error.WriteLine("GDScript suite failed to load (check parser errors above).");
            return 1;
        }
        tests.Call("run_tests");
        for (var frame = 0; frame < 120 && !tests.Get("finished").AsBool(); frame++)
            engine.Iteration();
        if (!tests.Get("finished").AsBool())
        {
            Console.Error.WriteLine("GDScript suite did not finish within 120 frames.");
            return 1;
        }
        var failures = tests.Get("failures").AsGodotArray();
        foreach (var failure in failures) Console.Error.WriteLine(failure.AsString());
        Console.WriteLine($"GDScript: {tests.Get("checks").AsInt32()} checks, {failures.Count} failures.");
        if (args.Contains("--benchmark") && failures.Count == 0)
        {
            var json = tests.Call("benchmark").AsString();
            if (string.IsNullOrEmpty(json))
            {
                Console.Error.WriteLine("GDScript benchmark failed; see script errors above.");
                return 1;
            }
            Console.WriteLine(json);
            if (benchmarkOutput is not null)
            {
                Directory.CreateDirectory(Path.GetDirectoryName(benchmarkOutput)!);
                File.WriteAllText(benchmarkOutput, json);
            }
        }
        return failures.Count == 0 ? 0 : 1;
    }
}

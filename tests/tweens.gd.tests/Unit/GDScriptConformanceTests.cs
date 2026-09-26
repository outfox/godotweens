// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss
using System.Text.Json;

namespace tweens.gd.Tests.Unit;

public class GDScriptConformanceTests
{
    [Fact]
    public void SharedTimelineAndEasingFixturesMatchCSharp()
    {
        using var data = JsonDocument.Parse(File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "conformance", "timelines.json")));
        var tolerance = data.RootElement.GetProperty("tolerance").GetDouble();
        foreach (var test in data.RootElement.GetProperty("cases").EnumerateArray())
        {
            var o = test.GetProperty("options");
            double Number(string key) => o.TryGetProperty(key, out var value) ? value.GetDouble() : 0;
            var options = new TweenOptions
            {
                Duration = Number("duration"), Delay = Number("delay"), Offset = Number("offset"),
                Repeats = (int)Number("repeats"), RepeatInterval = Number("repeat_interval"),
                PingPongInterval = Number("ping_pong_interval"),
                UsePingPong = o.TryGetProperty("use_ping_pong", out var ping) && ping.GetBoolean(),
            };
            var clock = new Playback(options);
            if (test.TryGetProperty("credit", out var credit)) clock.Credit(credit.GetDouble());
            foreach (var sample in test.GetProperty("samples").EnumerateArray())
            {
                clock.Advance(sample.GetProperty("delta").GetDouble());
                Assert.True(Math.Abs(clock.Progress - sample.GetProperty("progress").GetDouble()) <= tolerance, test.GetProperty("name").GetString());
                Assert.Equal(sample.GetProperty("state").GetInt32(), (int)clock.State);
                if (sample.TryGetProperty("overshoot", out var overshoot))
                    Assert.InRange(Math.Abs(clock.Overshoot - overshoot.GetDouble()), 0, tolerance);
            }
        }
        foreach (var sample in data.RootElement.GetProperty("easing").EnumerateArray())
        {
            var actual = Easing.Evaluate((EaseType)sample.GetProperty("ease").GetInt32(), sample.GetProperty("t").GetSingle());
            Assert.InRange(Math.Abs(actual - sample.GetProperty("value").GetDouble()), 0, tolerance);
        }
    }
}

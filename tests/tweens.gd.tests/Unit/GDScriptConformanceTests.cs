// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss
using System.Text.Json;
using tweens.gd.Tests.Support;

namespace tweens.gd.Tests.Unit;

public class GDScriptConformanceTests
{
    [Fact]
    public async Task SharedGroupFixturesMatchCSharp()
    {
        using var data = JsonDocument.Parse(File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "conformance", "groups.json")));
        var tolerance = data.RootElement.GetProperty("tolerance").GetDouble();
        foreach (var test in data.RootElement.GetProperty("cases").EnumerateArray())
        {
            using var scheduler = new TweenScheduler();
            var members = test.GetProperty("durations").EnumerateArray()
                .Select(duration => scheduler.Add(new Box(), new PlainTween { To = 1, Duration = duration.GetDouble() })).ToArray();
            var group = Group.Of(test.GetProperty("order").EnumerateArray().Select(index => members[index.GetInt32()]).ToArray());
            var target = new Box();
            TweenInstance? next = null;
            var continuation = Continue();
            foreach (var sample in test.GetProperty("samples").EnumerateArray())
            {
                scheduler.Update(sample.GetProperty("delta").GetDouble());
                Assert.Equal(sample.GetProperty("settled").GetBoolean(), group.IsTerminal);
                Assert.Equal(group.IsTerminal, next is not null);
            }
            Assert.True(continuation.IsCompletedSuccessfully, test.GetProperty("name").GetString());
            await continuation;
            Assert.Equal(0, target.Value);
            scheduler.Update(test.GetProperty("next_delta").GetDouble());
            Assert.InRange(Math.Abs(target.Value - test.GetProperty("next_value").GetDouble()), 0, tolerance);

            async Task Continue()
            {
                Assert.Equal(Reason.Completed, await group.End);
                next = scheduler.Add(target, new PlainTween { To = 1, Duration = 1 });
            }
        }
    }

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

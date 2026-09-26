// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using tweens.gd;
using twodog.Testing;
using twodog.Testing.Xunit;

namespace testbed.Tests.Integration;

[Collection<HeadlessCollection>]
public class RunnerTests(HeadlessFixture godot)
{
    [Fact]
    public void LibraryNodeReceivesGodotCallbacks()
    {
        var runner = new TweenRunner();
        godot.Tree.Root.AddChild(runner);
        try
        {
            for (var i = 0; i < 5; i++) godot.Engine.Iteration();
            Assert.True(runner.Ticks > 0);
        }
        finally { runner.Free(); }
    }
}

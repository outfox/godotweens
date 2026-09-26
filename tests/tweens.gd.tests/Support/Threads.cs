// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using System.Runtime.ExceptionServices;

namespace tweens.gd.Tests.Support;

public static class Threads
{
    /// <summary>Runs the action on a new thread and rethrows its failure here, keeping the caller's thread.</summary>
    public static void RunElsewhere(Action action)
    {
        Exception? failure = null;
        var thread = new Thread(() =>
        {
            try { action(); }
            catch (Exception error) { failure = error; }
        });
        thread.Start();
        thread.Join();
        if (failure is not null) ExceptionDispatchInfo.Capture(failure).Throw();
    }
}

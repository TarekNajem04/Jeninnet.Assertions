//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
// GC.GetAllocatedBytesForCurrentThread requires .NET Core 2.0+, so nothing in this file applies to .NET Standard 2.0.
#if !NETSTANDARD2_0
namespace Jeninnet.Assertions.Utilities;

/// <summary>Measures thread-allocated bytes around a delegate (requires .NET Core 2.0+).</summary>
internal static class Allocation {
    /// <summary>Runs <paramref name="action"/> and returns the bytes it allocated on the current thread.</summary>
    /// <param name="action">The delegate to measure.</param>
    /// <returns>Allocated bytes (never negative).</returns>
    internal static long Measure(Action action) {
        Collect();
        var before = GC.GetAllocatedBytesForCurrentThread();
        action();
        return GC.GetAllocatedBytesForCurrentThread() - before;
    }

    /// <summary>Runs <paramref name="action"/> and returns the bytes it allocated on the current thread.</summary>
    /// <param name="action">The delegate to measure.</param>
    /// <returns>The allocated bytes.</returns>
    internal static async Task<long> MeasureAsync(Func<Task> action) {
        Collect();
        var before = GC.GetAllocatedBytesForCurrentThread();
        await action();
        return GC.GetAllocatedBytesForCurrentThread() - before;
    }

    /// <summary>Formats a byte count invariantly (B/KB/MB, one decimal trimmed).</summary>
    /// <param name="bytes">The byte count.</param>
    /// <returns>Human-readable size, e.g. <c>512 B</c>, <c>1.5 KB</c>.</returns>
    internal static string FormatBytes(long bytes) {
        if(bytes < 1024) {
            return AssertionScope.FormatInvariant(bytes) + " B";
        }

        if(bytes < 1024 * 1024) {
            return Trimmed(bytes / 1024.0) + " KB";
        }

        return Trimmed(bytes / (1024.0 * 1024.0)) + " MB";
    }

    /// <summary>Formats a size quotient without trailing zeros using invariant culture.</summary>
    /// <param name="value">The numeric size value.</param>
    /// <returns>The trimmed numeric text.</returns>
    private static string Trimmed(double value) => value.ToString("0.#", CultureInfo.InvariantCulture);

    /// <summary>Forces a full collection cycle to stabilize allocation measurements.</summary>
    private static void Collect() {
        // Forced collection is the point here: it stabilizes allocation measurements (S1215 waived deliberately).
#pragma warning disable S1215
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
#pragma warning restore S1215
    }
}
#endif

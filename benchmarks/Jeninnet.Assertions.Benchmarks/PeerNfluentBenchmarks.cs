//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
using NFluent;
using NFluent.Kernel;

namespace PeerBenchmarks;

/// <summary>
/// Measures NFluent 3.1.0 on the same hot scenarios as <c>HotVerbBenchmarks</c>.
/// NFluent has no aggregating scope; the scope scenario measures two sequential
/// <c>Check.That</c> statements, which fail fast, and is therefore not a direct
/// semantic match for <c>AssertionScope</c>.
/// </summary>
[MemoryDiagnoser]
[SimpleJob(warmupCount: 3, iterationCount: 5)]
public class PeerNfluentBenchmarks {
    private const int NUMBER = 42;
    private const string TEXT = "the quick brown fox jumps over the lazy dog";
    private static readonly int[] _numbers = [1, 2, 3];

    /// <summary>Measures a successful integer equality.</summary>
    [Benchmark]
    public void Be_Int_Success() => Check.That(NUMBER).IsEqualTo(42);

    /// <summary>Measures a failing integer equality (message formatting included).</summary>
    [Benchmark]
    public void Be_Int_Failure() => Fail(() => Check.That(NUMBER).IsEqualTo(43));

    /// <summary>Measures a successful string equality.</summary>
    [Benchmark]
    public void Be_String_Success() => Check.That(TEXT).IsEqualTo("the quick brown fox jumps over the lazy dog");

    /// <summary>Measures a failing string equality (message formatting included).</summary>
    [Benchmark]
    public void Be_String_Failure() => Fail(() => Check.That(TEXT).IsEqualTo("the quick brown fox jumps over the lazy cat"));

    /// <summary>Measures a successful boolean check.</summary>
    [Benchmark]
    public void BeTrue_Success() => Check.That(true).IsTrue();

    /// <summary>Measures a successful substring search.</summary>
    [Benchmark]
    public void Contain_String_Success() => Check.That(TEXT).Contains("brown");

    /// <summary>Measures a successful collection count check.</summary>
    [Benchmark]
    public void HaveCount_Success() => Check.That(_numbers).HasSize(3);

    /// <summary>Measures two sequential successful checks (no aggregating scope in NFluent).</summary>
    [Benchmark]
    public void Scope_Success() {
        Check.That(NUMBER).IsEqualTo(42);
        Check.That(TEXT).Contains("fox");
    }

    /// <summary>Measures two sequential checks with the first failing (fail-fast, no aggregation).</summary>
    [Benchmark]
    public void Scope_Failure() => Fail(() => {
        Check.That(NUMBER).IsEqualTo(43);
        Check.That(TEXT).Contains("cat");
    });

    private static void Fail(Action action) {
        try {
            action();
        }
        catch(FluentCheckException) {
            // The formatted failure is the measured outcome.
        }
    }
}

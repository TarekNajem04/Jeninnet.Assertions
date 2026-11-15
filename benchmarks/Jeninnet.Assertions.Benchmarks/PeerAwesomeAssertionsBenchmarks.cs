//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
using AwesomeAssertions;
using AwesomeAssertions.Execution;

namespace PeerBenchmarks;

/// <summary>Measures AwesomeAssertions 9.6.0 on the same hot scenarios as <c>HotVerbBenchmarks</c>.</summary>
[MemoryDiagnoser]
[SimpleJob(warmupCount: 3, iterationCount: 5)]
public class PeerAwesomeAssertionsBenchmarks {
    private const int NUMBER = 42;
    private const string TEXT = "the quick brown fox jumps over the lazy dog";
    private static readonly int[] _numbers = [1, 2, 3];

    /// <summary>Measures a successful integer equality.</summary>
    [Benchmark]
    public void Be_Int_Success() => NUMBER.Should().Be(42);

    /// <summary>Measures a failing integer equality (message formatting included).</summary>
    [Benchmark]
    public void Be_Int_Failure() => Fail(() => NUMBER.Should().Be(43));

    /// <summary>Measures a successful string equality.</summary>
    [Benchmark]
    public void Be_String_Success() => TEXT.Should().Be("the quick brown fox jumps over the lazy dog");

    /// <summary>Measures a failing string equality (message formatting included).</summary>
    [Benchmark]
    public void Be_String_Failure() => Fail(() => TEXT.Should().Be("the quick brown fox jumps over the lazy cat"));

    /// <summary>Measures a successful boolean check.</summary>
    [Benchmark]
    public void BeTrue_Success() => true.Should().BeTrue();

    /// <summary>Measures a successful substring search.</summary>
    [Benchmark]
    public void Contain_String_Success() => TEXT.Should().Contain("brown");

    /// <summary>Measures a successful collection count check.</summary>
    [Benchmark]
    public void HaveCount_Success() => _numbers.Should().HaveCount(3);

    /// <summary>Measures a scope that observes one successful assertion.</summary>
    [Benchmark]
    public void Scope_Success() {
        using var scope = new AssertionScope();
        NUMBER.Should().Be(42);
    }

    /// <summary>Measures a scope that collects and discards one failure.</summary>
    [Benchmark]
    public void Scope_Failure() {
        using var scope = new AssertionScope();
        NUMBER.Should().Be(43);
        _ = scope.Discard();
    }

    private static void Fail(Action action) {
        try {
            action();
        }
        catch(AssertionFailedException) {
            // The formatted failure is the measured outcome.
        }
    }
}

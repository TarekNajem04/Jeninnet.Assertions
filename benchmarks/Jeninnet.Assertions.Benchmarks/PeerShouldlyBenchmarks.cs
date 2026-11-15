//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
using Shouldly;

namespace PeerBenchmarks;

/// <summary>
/// Measures Shouldly 4.3.0 on the same hot scenarios as <c>HotVerbBenchmarks</c>.
/// Shouldly has no aggregating <c>AssertionScope</c>; the scope scenario uses
/// <c>ShouldSatisfyAllConditions</c>, which evaluates every condition and aggregates failures.
/// </summary>
[MemoryDiagnoser]
[SimpleJob(warmupCount: 3, iterationCount: 5)]
public class PeerShouldlyBenchmarks {
    private const int NUMBER = 42;
    private const string TEXT = "the quick brown fox jumps over the lazy dog";
    private static readonly int[] _numbers = [1, 2, 3];

    /// <summary>Measures a successful integer equality.</summary>
    [Benchmark]
    public void Be_Int_Success() => NUMBER.ShouldBe(42);

    /// <summary>Measures a failing integer equality (message formatting included).</summary>
    [Benchmark]
    public void Be_Int_Failure() => Fail(() => NUMBER.ShouldBe(43));

    /// <summary>Measures a successful string equality.</summary>
    [Benchmark]
    public void Be_String_Success() => TEXT.ShouldBe("the quick brown fox jumps over the lazy dog");

    /// <summary>Measures a failing string equality (message formatting included).</summary>
    [Benchmark]
    public void Be_String_Failure() => Fail(() => TEXT.ShouldBe("the quick brown fox jumps over the lazy cat"));

    /// <summary>Measures a successful boolean check.</summary>
    [Benchmark]
    public void BeTrue_Success() => true.ShouldBeTrue();

    /// <summary>Measures a successful substring search.</summary>
    [Benchmark]
    public void Contain_String_Success() => TEXT.ShouldContain("brown");

    /// <summary>Measures a successful collection count check (Shouldly has no <c>ShouldHaveCount</c>).</summary>
    [Benchmark]
    public void HaveCount_Success() => _numbers.Length.ShouldBe(3);

    /// <summary>Measures two successful conditions aggregated by <c>ShouldSatisfyAllConditions</c>.</summary>
    [Benchmark]
    public void Scope_Success() => NUMBER.ShouldSatisfyAllConditions(
        () => NUMBER.ShouldBe(42),
        () => TEXT.ShouldContain("fox"));

    /// <summary>Measures two conditions (one failing) aggregated by <c>ShouldSatisfyAllConditions</c>.</summary>
    [Benchmark]
    public void Scope_Failure() => Fail(() => NUMBER.ShouldSatisfyAllConditions(
        () => NUMBER.ShouldBe(43),
        () => TEXT.ShouldContain("cat")));

    private static void Fail(Action action) {
        try {
            action();
        }
        catch(ShouldAssertException) {
            // The formatted failure is the measured outcome.
        }
    }
}

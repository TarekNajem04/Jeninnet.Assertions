//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Benchmarks;

/// <summary>Measures the allocation and time cost of the hot success and failure verbs.</summary>
[MemoryDiagnoser]
[SimpleJob(warmupCount: 3, iterationCount: 5)]
public class HotVerbBenchmarks {
    private const int NUMBER = 42;
    private const string TEXT = "the quick brown fox jumps over the lazy dog";
    private static readonly int[] _numbers = [1, 2, 3];
    private static readonly Action _throwingAction = static () => throw new InvalidOperationException("boom");

    /// <summary>Measures a successful integer equality.</summary>
    [Benchmark]
    public void Be_Int_Success() => NUMBER.Should().Be(42);

    /// <summary>Measures a failing integer equality (message formatting included).</summary>
    [Benchmark]
    public void Be_Int_Failure() => Fail(() => NUMBER.Should().Be(43));

    /// <summary>Measures a successful string equality.</summary>
    [Benchmark]
    public void Be_String_Success() => TEXT.Should().Be("the quick brown fox jumps over the lazy dog");

    /// <summary>Measures a failing string equality (diff and message formatting included).</summary>
    [Benchmark]
    public void Be_String_Failure() => Fail(() => TEXT.Should().Be("the quick brown fox jumps over the lazy cat"));

    /// <summary>Measures a successful boolean check.</summary>
    [Benchmark]
    public void BeTrue_Success() => true.Should().BeTrue();

    /// <summary>Measures a failing boolean check.</summary>
    [Benchmark]
    public void BeTrue_Failure() => Fail(() => false.Should().BeTrue());

    /// <summary>Measures a successful substring search.</summary>
    [Benchmark]
    public void Contain_String_Success() => TEXT.Should().Contain("brown");

    /// <summary>Measures a successful collection membership check.</summary>
    [Benchmark]
    public void Contain_Collection_Success() => _numbers.Should().Contain(2);

    /// <summary>Measures a successful collection count check.</summary>
    [Benchmark]
    public void HaveCount_Success() => _numbers.Should().HaveCount(3);

    /// <summary>Measures a successful exception assertion over a throwing action.</summary>
    [Benchmark]
    public void Throw_Success() => _throwingAction.Should().Throw<InvalidOperationException>();

    private static void Fail(Action action) {
        try {
            action();
        }
        catch(AssertionFailedException) {
            // The formatted failure is the measured outcome.
        }
    }
}

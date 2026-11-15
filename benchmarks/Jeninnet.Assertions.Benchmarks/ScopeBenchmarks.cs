//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Benchmarks;

/// <summary>Measures the overhead an <see cref="AssertionScope"/> adds on success and aggregation.</summary>
[MemoryDiagnoser]
[SimpleJob(warmupCount: 3, iterationCount: 5)]
public class ScopeBenchmarks {
    /// <summary>Gets or sets the number of failures collected inside the scope.</summary>
    [Params(1, 10)]
    public int Failures { get; set; }

    /// <summary>Measures a scope that observes one successful assertion.</summary>
    [Benchmark]
    public void Scope_Success() {
        using var scope = new AssertionScope();
        "actual".Should().Be("actual");
    }

    /// <summary>Measures a scope that collects and discards the configured number of failures.</summary>
    [Benchmark]
    public void Scope_Failures() {
        using var scope = new AssertionScope();
        for(var index = 0; index < Failures; index++) {
            "actual".Should().Be("expected");
        }

        _ = scope.Discard();
    }
}

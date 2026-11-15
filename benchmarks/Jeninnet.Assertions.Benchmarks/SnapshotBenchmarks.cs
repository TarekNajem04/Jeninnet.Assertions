//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Benchmarks;

/// <summary>Measures snapshot verification, including file IO and one scrubber (kept out of the hot path).</summary>
[MemoryDiagnoser]
[SimpleJob(warmupCount: 3, iterationCount: 5)]
public class SnapshotBenchmarks {
    private const string TEXT = "order-42 confirmed";
    private static readonly Regex _digits = new(@"\d+", RegexOptions.Compiled | RegexOptions.CultureInvariant, TimeSpan.FromSeconds(1));
    private string _directory = "";

    /// <summary>Creates the verified snapshot in a temporary directory.</summary>
    [GlobalSetup]
    public void Setup() {
        _directory = Path.Combine(Path.GetTempPath(), "jeninnet-snapshot-benchmark", Guid.NewGuid().ToString("N", CultureInfo.InvariantCulture));
        Directory.CreateDirectory(_directory);
        TEXT.Should().MatchSnapshot("benchmark", _directory, options => options.Scrub(Scrub).DisableDiff().AutoVerify());
    }

    /// <summary>Removes the temporary snapshot directory.</summary>
    [GlobalCleanup]
    public void Cleanup() {
        if(Directory.Exists(_directory)) {
            Directory.Delete(_directory, recursive: true);
        }
    }

    /// <summary>Measures a verified string snapshot with a digit scrubber.</summary>
    [Benchmark]
    public void MatchSnapshot_String_WithScrubber() =>
        TEXT.Should().MatchSnapshot("benchmark", _directory, options => options.Scrub(Scrub).DisableDiff());

    private static string Scrub(string value) => _digits.Replace(value, "N");
}

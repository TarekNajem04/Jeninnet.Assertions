//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Tests;

[TestClass]
public class EquivalencyToleranceTests {
    [TestMethod]
    public void DateTime_WithinTolerance_Passes_IncludingTheBoundary() {
        var expected = new Telemetry { UpdatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) };
        var actual = new Telemetry { UpdatedAt = expected.UpdatedAt.AddMilliseconds(500) };

        actual.Should().BeEquivalentTo(expected, o => o.WithDateTimeTolerance(TimeSpan.FromSeconds(1)));

        var boundary = new Telemetry { UpdatedAt = expected.UpdatedAt.AddSeconds(1) };
        boundary.Should().BeEquivalentTo(expected, o => o.WithDateTimeTolerance(TimeSpan.FromSeconds(1)));
    }

    [TestMethod]
    public void DateTime_OutsideTolerance_FailsWithTheToleranceNote() {
        var expected = new Telemetry { UpdatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) };
        var actual = new Telemetry { UpdatedAt = expected.UpdatedAt.AddSeconds(5) };

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => actual.Should().BeEquivalentTo(expected, o => o.WithDateTimeTolerance(TimeSpan.FromSeconds(1))));
        Assert.AreEqual("Expected actual to be equivalent, but UpdatedAt was 01/01/2026 00:00:05 instead of 01/01/2026 00:00:00. The difference 00:00:05 exceeds the allowed tolerance 00:00:01.", ex.Message);
    }

    [TestMethod]
    public void DateTimeOffset_OffsetDifference_NormalizesToInstants() {
        var expected = new Telemetry { SyncStamp = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero) };
        var actual = new Telemetry { SyncStamp = new DateTimeOffset(2026, 1, 1, 2, 0, 0, TimeSpan.FromHours(2)).AddMilliseconds(500) };

        actual.Should().BeEquivalentTo(expected, o => o.WithDateTimeTolerance(TimeSpan.FromSeconds(1)));

        var late = new Telemetry { SyncStamp = new DateTimeOffset(2026, 1, 1, 2, 0, 3, TimeSpan.FromHours(2)) };
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => late.Should().BeEquivalentTo(expected, o => o.WithDateTimeTolerance(TimeSpan.FromSeconds(1))));
        Assert.AreEqual("Expected late to be equivalent, but SyncStamp was 01/01/2026 02:00:03 +02:00 instead of 01/01/2026 00:00:00 +00:00. The difference 00:00:03 exceeds the allowed tolerance 00:00:01.", ex.Message);
    }

    [TestMethod]
    public void Decimal_WithinTolerance_Passes_IncludingTheBoundary() {
        var expected = new Telemetry { Amount = 10.00m };
        var actual = new Telemetry { Amount = 10.005m };

        actual.Should().BeEquivalentTo(expected, o => o.WithNumericTolerance(0.01));

        var boundary = new Telemetry { Amount = 10.01m };
        boundary.Should().BeEquivalentTo(expected, o => o.WithNumericTolerance(0.01));
    }

    [TestMethod]
    public void Decimal_OutsideTolerance_FailsWithTheToleranceNote() {
        var expected = new Telemetry { Amount = 10.00m };
        var actual = new Telemetry { Amount = 10.02m };

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => actual.Should().BeEquivalentTo(expected, o => o.WithNumericTolerance(0.01)));
        Assert.AreEqual("Expected actual to be equivalent, but Amount was 10.02 instead of 10.00. The difference 0.02 exceeds the allowed tolerance 0.01.", ex.Message);
    }

    [TestMethod]
    public void Double_OutsideTolerance_FailsWithTheToleranceNote() {
        var expected = new Telemetry { Ratio = 0.25 };
        var actual = new Telemetry { Ratio = 0.5 };

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => actual.Should().BeEquivalentTo(expected, o => o.WithNumericTolerance(0.1)));
        Assert.AreEqual("Expected actual to be equivalent, but Ratio was 0.5 instead of 0.25. The difference 0.25 exceeds the allowed tolerance 0.1.", ex.Message);
    }

    [TestMethod]
    public void Integers_StayExact_EvenWithNumericTolerance() {
        var expected = new Telemetry { Attempts = 2 };
        var actual = new Telemetry { Attempts = 3 };

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => actual.Should().BeEquivalentTo(expected, o => o.WithNumericTolerance(5)));
        Assert.AreEqual("Expected actual to be equivalent, but Attempts was 3 instead of 2.", ex.Message);
    }

    [TestMethod]
    public void For_Variant_ScopesToOneMember() {
        var expected = new Telemetry {
            UpdatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
        };
        var actual = new Telemetry {
            UpdatedAt = expected.UpdatedAt.AddSeconds(2),
            CreatedAt = expected.CreatedAt.AddSeconds(2),
        };

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => actual.Should().BeEquivalentTo(
            expected, o => o.WithDateTimeToleranceFor(TimeSpan.FromSeconds(5), x => x.UpdatedAt)));
        Assert.AreEqual("Expected actual to be equivalent, but CreatedAt was 01/01/2026 00:00:02 instead of 01/01/2026 00:00:00.", ex.Message);
    }

    [TestMethod]
    public void For_Variant_OverridesTheGraphWideTolerance() {
        var expected = new Telemetry {
            UpdatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
        };
        var actual = new Telemetry {
            UpdatedAt = expected.UpdatedAt.AddSeconds(2),
            CreatedAt = expected.CreatedAt.AddSeconds(2),
        };

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => actual.Should().BeEquivalentTo(
            expected, o => o.WithDateTimeTolerance(TimeSpan.FromSeconds(1)).WithDateTimeToleranceFor(TimeSpan.FromSeconds(5), x => x.UpdatedAt)));
        Assert.AreEqual("Expected actual to be equivalent, but CreatedAt was 01/01/2026 00:00:02 instead of 01/01/2026 00:00:00. The difference 00:00:02 exceeds the allowed tolerance 00:00:01.", ex.Message);
    }

    [TestMethod]
    public void CustomStep_PrecedesTheToleranceComparison() {
        var expected = new Telemetry { UpdatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) };
        var actual = new Telemetry { UpdatedAt = expected.UpdatedAt.AddMilliseconds(500) };

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => actual.Should().BeEquivalentTo(
            expected, o => o.Using(new FailingDateStep()).WithDateTimeTolerance(TimeSpan.FromHours(1))));
        Assert.Contains("the step owns this node", ex.Message);
    }

    [TestMethod]
    public void ToleranceGuards_RejectNegativeValuesAndNullSelectors() {
        var options = new SelfReferenceEquivalencyOptions<Telemetry>();

        var negativeTime = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => options.WithDateTimeTolerance(TimeSpan.FromSeconds(-1)));
        Assert.AreEqual("tolerance", negativeTime.ParamName);

        var negativeTimeFor = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => options.WithDateTimeToleranceFor(TimeSpan.FromSeconds(-1), x => x.UpdatedAt));
        Assert.AreEqual("tolerance", negativeTimeFor.ParamName);

        var negativeNumeric = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => options.WithNumericTolerance(-0.1));
        Assert.AreEqual("epsilon", negativeNumeric.ParamName);

        var negativeNumericFor = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => options.WithNumericToleranceFor(-0.1, x => x.Ratio));
        Assert.AreEqual("epsilon", negativeNumericFor.ParamName);

        var nullTimeSelector = Assert.ThrowsExactly<ArgumentNullException>(() => options.WithDateTimeToleranceFor<DateTime>(TimeSpan.FromSeconds(1), null!));
        Assert.AreEqual("selector", nullTimeSelector.ParamName);

        var nullNumericSelector = Assert.ThrowsExactly<ArgumentNullException>(() => options.WithNumericToleranceFor<double>(0.1, null!));
        Assert.AreEqual("selector", nullNumericSelector.ParamName);
    }

    private sealed class Telemetry {
        public decimal Amount { get; set; }

        public int Attempts { get; set; }

        public DateTime CreatedAt { get; set; }

        public double Ratio { get; set; }

        public DateTimeOffset SyncStamp { get; set; }

        public DateTime UpdatedAt { get; set; }
    }

    private sealed class FailingDateStep : EquivalencyStep {
        public override bool Handles(Comparands comparands) => comparands.Expectation is DateTime;

        public override bool TryHandle(Comparands comparands, IObjectGraph objectGraph, IEquivalencyStepContext context) {
            context.Fail("the step owns this node");
            return true;
        }
    }
}

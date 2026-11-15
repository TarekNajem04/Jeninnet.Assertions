//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Tests;

[TestClass]
public class StringParsingTests {
    [TestMethod]
    public void BeParsableInto_ParsesAndExposesTheValue() {
        var result = "42".Should().BeParsableInto<int>();
        Assert.AreEqual(42, result.Which);

        var guid = "f81d4fae-7dec-11d0-a765-00a0c91e6bf6".Should().BeParsableInto<Guid>();
        Assert.AreEqual(new Guid("f81d4fae-7dec-11d0-a765-00a0c91e6bf6"), guid.Which);

        var timestamp = "2026-11-15T12:30:00".Should().BeParsableInto<DateTime>();
        Assert.AreEqual(new DateTime(2026, 11, 15, 12, 30, 0, DateTimeKind.Unspecified), timestamp.Which);
    }

    [TestMethod]
    public void BeParsableInto_FailsWithTheTargetTypeAndReason() {
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => "abc".Should().BeParsableInto<int>());

        Assert.Contains("to be parsable into Int32", ex.Message);
        Assert.Contains("could not be parsed", ex.Message);
        Assert.DoesNotContain("..", ex.Message);
    }

    [TestMethod]
    public void BeParsableInto_UsesInvariantCultureByDefaultAndHonorsAnExplicitProvider() {
        var invariant = "1.5".Should().BeParsableInto<decimal>();
        Assert.AreEqual(1.5m, invariant.Which);

        var german = "1,5".Should().BeParsableInto<decimal>(CultureInfo.GetCultureInfo("de-DE"));
        Assert.AreEqual(1.5m, german.Which);

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => "1.234,56".Should().BeParsableInto<decimal>());
        Assert.Contains("to be parsable into Decimal", ex.Message);

        var grouped = "1.234,56".Should().BeParsableInto<decimal>(CultureInfo.GetCultureInfo("de-DE"));
        Assert.AreEqual(1234.56m, grouped.Which);
    }

    [TestMethod]
    public void NotBeParsableInto_PassAndFail() {
        "abc".Should().NotBeParsableInto<int>();

        const string subject = "42";
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => subject.Should().NotBeParsableInto<int>());
        Assert.AreEqual("Expected subject with value '42' not to be parsable into Int32, but it was.", ex.Message);
    }

    [TestMethod]
    public void NotBeParsableInto_HonorsAnExplicitProvider() {
        const string subject = "1.234,56";
        subject.Should().NotBeParsableInto<decimal>();

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() =>
            subject.Should().NotBeParsableInto<decimal>(CultureInfo.GetCultureInfo("de-DE")));
        Assert.Contains("not to be parsable into Decimal", ex.Message);
    }

    [TestMethod]
    public void BeParsableInto_HandlesNullAndBecause() {
        const string? subject = null;
        var nullEx = Assert.ThrowsExactly<AssertionFailedException>(() => subject.Should().BeParsableInto<int>());
        Assert.Contains("but it was null", nullEx.Message);
        subject.Should().NotBeParsableInto<int>();

        const string invalid = "abc";
        var becauseEx = Assert.ThrowsExactly<AssertionFailedException>(() =>
            invalid.Should().BeParsableInto<int>("because {0}", "the configuration is numeric"));
        Assert.Contains("because the configuration is numeric", becauseEx.Message);
    }

    [TestMethod]
    public void BeParsableInto_InsideScopeReturnsDefaultAndCollectsFailures() {
        using var scope = new AssertionScope();

        var invalid = "abc".Should().BeParsableInto<int>();
        var missing = ((string)null!).Should().BeParsableInto<int>();

        Assert.AreEqual(0, invalid.Which);
        Assert.AreEqual(0, missing.Which);
        Assert.AreEqual(2, scope.Discard().Length);
    }

    [TestMethod]
    public void BeParsableInto_IsLocalizedInArabic() {
        using var _ = new CultureScope("ar");

        const string subject = "abc";
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => subject.Should().BeParsableInto<int>());

        Assert.StartsWith("المتوقع من subject", ex.Message, StringComparison.Ordinal);
        Assert.Contains("Int32", ex.Message);
    }
}

//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Tests;

[TestClass]
public class TimeOnlyTests {
    private static readonly TimeOnly _morning = new(8, 30, 15);

    [TestMethod]
    public void Be_PassAndFail() {
        _morning.Should().Be(new TimeOnly(8, 30, 15));

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => _morning.Should().Be(new TimeOnly(9, 0, 0)));
        Assert.Contains("Expected _morning to be", ex.Message);
        Assert.Contains("but found", ex.Message);
    }

    [TestMethod]
    public void Ordering_PassAndFail() {
        _morning.Should().BeBefore(new TimeOnly(9, 0)).And.BeAfter(new TimeOnly(8, 0))
            .And.BeOnOrBefore(_morning).And.NotBeAfter(new TimeOnly(9, 0));

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => _morning.Should().BeAfter(new TimeOnly(9, 0)));
        Assert.Contains("Expected _morning to be after", ex.Message);
    }

    [TestMethod]
    public void BeCloseTo_PassAndFail() {
        _morning.Should().BeCloseTo(_morning.AddMinutes(2), TimeSpan.FromMinutes(5));

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => _morning.Should().BeCloseTo(_morning.AddMinutes(2), TimeSpan.FromSeconds(5)));
        Assert.Contains("to be close to", ex.Message);
        Assert.Contains("but found", ex.Message);
    }

    [TestMethod]
    public void BeCloseTo_MeasuresShortestClockDistance() {
        var beforeMidnight = new TimeOnly(23, 59);
        beforeMidnight.Should().BeCloseTo(new TimeOnly(0, 1), TimeSpan.FromMinutes(5));
    }

    [TestMethod]
    public void Components_PassAndFail() {
        _morning.Should().HaveHour(8).And.HaveMinute(30).And.HaveSecond(15).And.HaveMillisecond(0);

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => _morning.Should().HaveHour(9));
        Assert.AreEqual("Expected _morning to have hour 9, but found 8.", ex.Message);
    }

    [TestMethod]
    public void PluralAliases_MatchSingulars() {
        _morning.Should().HaveHours(8).And.HaveMinutes(30).And.HaveSeconds(15).And.HaveMilliseconds(0);
        _morning.Should().NotHaveHours(9).And.NotHaveMinutes(31).And.NotHaveSeconds(16).And.NotHaveMilliseconds(1);

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => _morning.Should().HaveHours(9));
        Assert.AreEqual("Expected _morning to have hour 9, but found 8.", ex.Message);

        TimeOnly? present = _morning;
        present.Should().HaveHours(8).And.NotHaveHours(9);

        TimeOnly? missing = null;
        var nullEx = Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().HaveHours(8));
        Assert.Contains("but found <null>.", nullEx.Message);

        var nullNotEx = Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().NotHaveHours(8));
        Assert.AreEqual("Expected missing not to have hour 8, but found <null>.", nullNotEx.Message);
    }

    [TestMethod]
    public void Nullable_Behaviors() {
        TimeOnly? present = _morning;
        present.Should().Be(_morning).And.HaveValue();

        TimeOnly? missing = null;
        missing.Should().Be(null).And.BeNull();
        missing.Should().NotBe(_morning).And.NotBeCloseTo(_morning, TimeSpan.FromMinutes(1));

        var before = Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().BeBefore(_morning));
        Assert.Contains("but found <null>.", before.Message);
    }

    [TestMethod]
    public void NullableHaveValue_PassAndFail() {
        TimeOnly? present = _morning;
        present.Should().HaveValue();

        TimeOnly? missing = null;
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().HaveValue());
        Assert.AreEqual("Expected missing to not be null, but found null.", ex.Message);
    }

    [TestMethod]
    public void NullableNotHaveValue_PassAndFail() {
        TimeOnly? missing = null;
        missing.Should().NotHaveValue();

        TimeOnly? present = _morning;
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => present.Should().NotHaveValue());
        Assert.Contains("Expected present to be null, but found", ex.Message);
    }

    [TestMethod]
    public void BeOneOf_PassAndFail() {
        _morning.Should().BeOneOf(_morning.AddHours(-1), _morning);

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => _morning.Should().BeOneOf(_morning.AddHours(1)));
        Assert.Contains("Expected _morning to be one of", ex.Message);
        Assert.Contains("but found", ex.Message);

        Assert.ThrowsExactly<ArgumentNullException>(() => _morning.Should().BeOneOf((TimeOnly[])null!));

        TimeOnly? missing = null;
        missing.Should().BeOneOf(null, _morning);
    }

    [TestMethod]
    public void NotBeOnOr_Boundaries() {
        _morning.Should().NotBeOnOrAfter(_morning.AddHours(1));
        _morning.Should().NotBeOnOrBefore(_morning.AddHours(-1));

        var after = Assert.ThrowsExactly<AssertionFailedException>(() => _morning.Should().NotBeOnOrAfter(_morning));
        Assert.Contains("Expected _morning not to be on or after", after.Message);

        var before = Assert.ThrowsExactly<AssertionFailedException>(() => _morning.Should().NotBeOnOrBefore(_morning));
        Assert.Contains("Expected _morning not to be on or before", before.Message);

        TimeOnly? missing = null;
        missing.Should().NotBeOnOrAfter(_morning);
        missing.Should().NotBeOnOrBefore(_morning);
    }

    [TestMethod]
    public void NotHaveComponents_PassAndFail() {
        _morning.Should().NotHaveHour(9).And.NotHaveMinute(31).And.NotHaveSecond(16).And.NotHaveMillisecond(1);

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => _morning.Should().NotHaveSecond(15));
        Assert.AreEqual("Expected _morning not to have second 15, but found 15.", ex.Message);

        TimeOnly? missing = null;
        var nullHour = Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().NotHaveHour(8));
        Assert.AreEqual("Expected missing not to have hour 8, but found <null>.", nullHour.Message);
    }
}

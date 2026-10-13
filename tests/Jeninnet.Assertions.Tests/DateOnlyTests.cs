//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Tests;

[TestClass]
public class DateOnlyTests {
    private static readonly DateOnly _day = new(2026, 5, 10);

    [TestMethod]
    public void Be_PassAndFail() {
        _day.Should().Be(new DateOnly(2026, 5, 10));

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => _day.Should().Be(new DateOnly(2026, 5, 11)));
        Assert.AreEqual("Expected _day to be 05/11/2026, but found 05/10/2026.", ex.Message);
    }

    [TestMethod]
    public void Ordering_PassAndFail() {
        _day.Should().BeBefore(new DateOnly(2026, 6, 1)).And.BeAfter(new DateOnly(2026, 1, 1))
            .And.BeOnOrBefore(_day).And.BeOnOrAfter(_day);

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => _day.Should().NotBeAfter(new DateOnly(2026, 1, 1)));
        Assert.AreEqual("Expected _day not to be after 01/01/2026, but found 05/10/2026.", ex.Message);
    }

    [TestMethod]
    public void Components_PassAndFail() {
        _day.Should().HaveYear(2026).And.HaveMonth(5).And.HaveDay(10);

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => _day.Should().HaveMonth(6));
        Assert.AreEqual("Expected _day to have month 6, but found 5.", ex.Message);
    }

    [TestMethod]
    public void BeOneOf_PassAndFail() {
        _day.Should().BeOneOf(_day.AddDays(-1), _day);

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => _day.Should().BeOneOf(_day.AddDays(1)));
        Assert.AreEqual("Expected _day to be one of 05/11/2026, but found 05/10/2026.", ex.Message);

        Assert.ThrowsExactly<ArgumentNullException>(() => _day.Should().BeOneOf((DateOnly[])null!));

        DateOnly? missing = null;
        missing.Should().BeOneOf(null, _day);
    }

    [TestMethod]
    public void NotBeOnOr_Boundaries() {
        _day.Should().NotBeOnOrAfter(_day.AddDays(1));
        _day.Should().NotBeOnOrBefore(_day.AddDays(-1));

        var after = Assert.ThrowsExactly<AssertionFailedException>(() => _day.Should().NotBeOnOrAfter(_day));
        Assert.AreEqual("Expected _day not to be on or after 05/10/2026, but found 05/10/2026.", after.Message);

        var before = Assert.ThrowsExactly<AssertionFailedException>(() => _day.Should().NotBeOnOrBefore(_day));
        Assert.AreEqual("Expected _day not to be on or before 05/10/2026, but found 05/10/2026.", before.Message);

        DateOnly? missing = null;
        missing.Should().NotBeOnOrAfter(_day);
        missing.Should().NotBeOnOrBefore(_day);
    }

    [TestMethod]
    public void NotHaveComponents_PassAndFail() {
        _day.Should().NotHaveYear(2025).And.NotHaveMonth(6).And.NotHaveDay(11);

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => _day.Should().NotHaveMonth(5));
        Assert.AreEqual("Expected _day not to have month 5, but found 5.", ex.Message);

        DateOnly? missing = null;
        var nullMonth = Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().NotHaveMonth(5));
        Assert.AreEqual("Expected missing not to have month 5, but found <null>.", nullMonth.Message);
    }

    [TestMethod]
    public void Nullable_Behaviors() {
        DateOnly? present = _day;
        present.Should().Be(_day).And.HaveValue();

        DateOnly? missing = null;
        missing.Should().Be(null).And.BeNull();
        missing.Should().NotBe(_day).And.NotBeBefore(_day);

        var before = Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().BeBefore(_day));
        Assert.AreEqual("Expected missing to be before 05/10/2026, but found <null>.", before.Message);
    }

    [TestMethod]
    public void NullableHaveValue_PassAndFail() {
        DateOnly? present = _day;
        present.Should().HaveValue();

        DateOnly? missing = null;
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().HaveValue());
        Assert.AreEqual("Expected missing to not be null, but found null.", ex.Message);
    }

    [TestMethod]
    public void NullableNotHaveValue_PassAndFail() {
        DateOnly? missing = null;
        missing.Should().NotHaveValue();

        DateOnly? present = _day;
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => present.Should().NotHaveValue());
        Assert.AreEqual("Expected present to be null, but found 05/10/2026.", ex.Message);
    }
}

//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Tests;

[TestClass]
public class TimeOnlyCoverageTests {
    private static readonly TimeOnly _morning = new(8, 30, 15);

    [TestMethod]
    public void FailingVerbs_ReportEveryMessage() {
        using var scope = new AssertionScope();

        _morning.Should().NotBe(_morning);
        _morning.Should().BeBefore(_morning);
        _morning.Should().NotBeBefore(_morning.AddHours(1));
        _morning.Should().BeOnOrBefore(_morning.AddHours(-1));
        _morning.Should().NotBeAfter(_morning.AddHours(-1));
        _morning.Should().BeOnOrAfter(_morning.AddHours(1));
        _morning.Should().BeCloseTo(_morning.AddHours(1), TimeSpan.FromMinutes(5));
        _morning.Should().NotBeCloseTo(_morning, TimeSpan.FromMinutes(5));
        _morning.Should().HaveMinute(31);
        _morning.Should().HaveSecond(16);
        _morning.Should().NotHaveHour(8);
        _morning.Should().NotHaveMinute(30);
        _morning.Should().NotHaveMillisecond(0);

        var failures = scope.Discard();

        Assert.HasCount(13, failures);
        Assert.AreEqual("Expected _morning not to be 08:30, but found 08:30.", failures[0]);
        Assert.AreEqual("Expected _morning to be before 08:30, but found 08:30.", failures[1]);
        Assert.AreEqual("Expected _morning not to be before 09:30, but found 08:30.", failures[2]);
        Assert.AreEqual("Expected _morning to be on or before 07:30, but found 08:30.", failures[3]);
        Assert.AreEqual("Expected _morning not to be after 07:30, but found 08:30.", failures[4]);
        Assert.AreEqual("Expected _morning to be on or after 09:30, but found 08:30.", failures[5]);
        Assert.AreEqual("Expected _morning to be close to 09:30 (±00:05:00), but found 08:30.", failures[6]);
        Assert.AreEqual("Expected _morning not to be close to 08:30 (±00:05:00), but found 08:30.", failures[7]);
        Assert.AreEqual("Expected _morning to have minute 31, but found 30.", failures[8]);
        Assert.AreEqual("Expected _morning to have second 16, but found 15.", failures[9]);
        Assert.AreEqual("Expected _morning not to have hour 8, but found 8.", failures[10]);
        Assert.AreEqual("Expected _morning not to have minute 30, but found 30.", failures[11]);
        Assert.AreEqual("Expected _morning not to have millisecond 0, but found 0.", failures[12]);
    }

    [TestMethod]
    public void CloseTo_Guards_ThrowArgumentOutOfRange() {
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => _morning.Should().BeCloseTo(_morning, TimeSpan.FromMilliseconds(-1)));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => _morning.Should().NotBeCloseTo(_morning, TimeSpan.FromMilliseconds(-1)));
    }

    [TestMethod]
    public void Nullable_FailingVerbs_ReportEveryMessage() {
        TimeOnly? missing = null;
        TimeOnly? present = _morning;
        present.Should().BeCloseTo(_morning.AddMinutes(1), TimeSpan.FromMinutes(5));

        using var scope = new AssertionScope();

        missing.Should().Be(_morning);
        present.Should().NotBe(_morning);
        missing.Should().BeBefore(_morning);
        present.Should().NotBeBefore(_morning.AddHours(1));
        missing.Should().BeOnOrBefore(_morning);
        present.Should().BeOnOrBefore(_morning.AddHours(-1));
        missing.Should().BeAfter(_morning);
        present.Should().BeAfter(_morning.AddHours(1));
        present.Should().NotBeAfter(_morning.AddHours(-1));
        missing.Should().BeOnOrAfter(_morning);
        present.Should().BeOnOrAfter(_morning.AddHours(1));
        missing.Should().BeCloseTo(_morning, TimeSpan.FromMinutes(5));
        present.Should().BeCloseTo(_morning.AddHours(1), TimeSpan.FromMinutes(5));
        present.Should().NotBeCloseTo(_morning, TimeSpan.FromMinutes(5));
        missing.Should().HaveHour(8);
        present.Should().HaveHour(9);
        missing.Should().HaveMinute(30);
        present.Should().HaveMinute(31);
        missing.Should().HaveSecond(15);
        present.Should().HaveSecond(16);
        missing.Should().HaveMillisecond(0);
        present.Should().HaveMillisecond(1);
        present.Should().HaveMinutes(30);
        present.Should().HaveSeconds(15);
        present.Should().HaveMilliseconds(0);
        present.Should().NotHaveMinutes(31);
        present.Should().NotHaveSeconds(16);
        present.Should().NotHaveMilliseconds(1);
        present.Should().NotBeOnOrAfter(_morning);
        present.Should().NotBeOnOrBefore(_morning);
        missing.Should().NotHaveHour(8);
        present.Should().NotHaveHour(8);
        missing.Should().NotHaveMinute(30);
        present.Should().NotHaveMinute(30);
        missing.Should().NotHaveSecond(15);
        present.Should().NotHaveSecond(15);
        missing.Should().NotHaveMillisecond(0);
        present.Should().NotHaveMillisecond(0);
        present.Should().BeOneOf(_morning.AddHours(1));

        var failures = scope.Discard();

        Assert.HasCount(33, failures);
        Assert.AreEqual("Expected missing to be 08:30, but found <null>.", failures[0]);
        Assert.AreEqual("Expected present not to be 08:30, but found 08:30.", failures[1]);
        Assert.AreEqual("Expected missing to be before 08:30, but found <null>.", failures[2]);
        Assert.AreEqual("Expected present not to be before 09:30, but found 08:30.", failures[3]);
        Assert.AreEqual("Expected missing to be on or before 08:30, but found <null>.", failures[4]);
        Assert.AreEqual("Expected present to be on or before 07:30, but found 08:30.", failures[5]);
        Assert.AreEqual("Expected missing to be after 08:30, but found <null>.", failures[6]);
        Assert.AreEqual("Expected present to be after 09:30, but found 08:30.", failures[7]);
        Assert.AreEqual("Expected present not to be after 07:30, but found 08:30.", failures[8]);
        Assert.AreEqual("Expected missing to be on or after 08:30, but found <null>.", failures[9]);
        Assert.AreEqual("Expected present to be on or after 09:30, but found 08:30.", failures[10]);
        Assert.AreEqual("Expected missing to be close to 08:30 (±00:05:00), but found <null>.", failures[11]);
        Assert.AreEqual("Expected present to be close to 09:30 (±00:05:00), but found 08:30.", failures[12]);
        Assert.AreEqual("Expected present not to be close to 08:30 (±00:05:00), but found 08:30.", failures[13]);
        Assert.AreEqual("Expected missing to have hour 8, but found <null>.", failures[14]);
        Assert.AreEqual("Expected present to have hour 9, but found 8.", failures[15]);
        Assert.AreEqual("Expected missing to have minute 30, but found <null>.", failures[16]);
        Assert.AreEqual("Expected present to have minute 31, but found 30.", failures[17]);
        Assert.AreEqual("Expected missing to have second 15, but found <null>.", failures[18]);
        Assert.AreEqual("Expected present to have second 16, but found 15.", failures[19]);
        Assert.AreEqual("Expected missing to have millisecond 0, but found <null>.", failures[20]);
        Assert.AreEqual("Expected present to have millisecond 1, but found 0.", failures[21]);
        Assert.AreEqual("Expected present not to be on or after 08:30, but found 08:30.", failures[22]);
        Assert.AreEqual("Expected present not to be on or before 08:30, but found 08:30.", failures[23]);
        Assert.AreEqual("Expected missing not to have hour 8, but found <null>.", failures[24]);
        Assert.AreEqual("Expected present not to have hour 8, but found 8.", failures[25]);
        Assert.AreEqual("Expected missing not to have minute 30, but found <null>.", failures[26]);
        Assert.AreEqual("Expected present not to have minute 30, but found 30.", failures[27]);
        Assert.AreEqual("Expected missing not to have second 15, but found <null>.", failures[28]);
        Assert.AreEqual("Expected present not to have second 15, but found 15.", failures[29]);
        Assert.AreEqual("Expected missing not to have millisecond 0, but found <null>.", failures[30]);
        Assert.AreEqual("Expected present not to have millisecond 0, but found 0.", failures[31]);
        Assert.AreEqual("Expected present to be one of 09:30, but found 08:30.", failures[32]);
    }

    [TestMethod]
    public void Nullable_Guards_ThrowForNullCandidatesAndNegativePrecision() {
        TimeOnly? present = _morning;

        Assert.ThrowsExactly<ArgumentNullException>(() => present.Should().BeOneOf((TimeOnly?[])null!));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => present.Should().BeCloseTo(_morning, TimeSpan.FromMilliseconds(-1)));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => present.Should().NotBeCloseTo(_morning, TimeSpan.FromMilliseconds(-1)));
    }
}

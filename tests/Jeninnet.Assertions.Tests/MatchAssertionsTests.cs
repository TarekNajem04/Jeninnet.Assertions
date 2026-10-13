//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Tests;

[TestClass]
public partial class MatchAssertionsTests {
    [TestMethod]
    public void Succeed_PassFailAndNull() {
        FourDigitsRegex().Match("2026-01").Should().Succeed();

        var failed = FourDigitsRegex().Match("nope");
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => failed.Should().Succeed());
        Assert.AreEqual("Expected failed to be a successful match, but it failed.", ex.Message);

        Match? missing = null;
        var nullEx = Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().Succeed());
        Assert.AreEqual("Expected missing to be a successful match, but the match is null.", nullEx.Message);
    }

    [TestMethod]
    public void HaveValue_PassFailAndFailedMatch() {
        var match = IsoDateRegex().Match("2026-01-02");
        match.Should().HaveValue("2026-01-02");

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => match.Should().HaveValue("2026-01-03"));
        Assert.AreEqual("Expected match to match '2026-01-03', but found '2026-01-02'.", ex.Message);

        var failed = DigitsRegex().Match("nope");
        var failedEx = Assert.ThrowsExactly<AssertionFailedException>(() => failed.Should().HaveValue("1"));
        Assert.AreEqual("Expected failed to match '1', but the match failed.", failedEx.Message);
    }

    [TestMethod]
    public void HaveGroup_ByIndex_PassAndValueFailure() {
        var match = IndexedGroupsRegex().Match("2026-01");
        match.Should().HaveGroup(1, "2026").And.HaveGroup(2, "01");

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => match.Should().HaveGroup(2, "99"));
        Assert.AreEqual("Expected match to have group '2' with value '99', but found '01'.", ex.Message);
    }

    [TestMethod]
    public void HaveGroup_ByName_AndMissingListsAvailable() {
        var match = NamedGroupsRegex().Match("2026-01");
        match.Should().HaveGroup("year", "2026");
        match.Should().HaveGroup("month", "01");

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => match.Should().HaveGroup("day", "01"));
        Assert.AreEqual("Expected match to have group 'day', but it does not exist (available: 0, year, month).", ex.Message);

        var rangeEx = Assert.ThrowsExactly<AssertionFailedException>(() => match.Should().HaveGroup(5, "x"));
        Assert.AreEqual("Expected match to have group '5', but it does not exist (available: 0, year, month).", rangeEx.Message);
    }

    [TestMethod]
    public void HaveGroup_NonParticipatingOptionalGroup_IsEmpty() {
        var match = OptionalThenRequiredRegex().Match("b");
        match.Should().HaveGroup(1, "");

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => match.Should().HaveGroup(1, "a"));
        Assert.AreEqual("Expected match to have group '1' with value 'a', but found ''.", ex.Message);
    }

    [TestMethod]
    public void HaveGroup_OnFailedMatch_Fails() {
        var failed = NamedXRegex().Match("nope");

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => failed.Should().HaveGroup("x", "a"));
        Assert.AreEqual("Expected failed to have group 'x' with value 'a', but the match failed.", ex.Message);
    }

    [TestMethod]
    public void HaveGroupCount_IncludesGroupZero() {
        var match = TwoGroupsRegex().Match("ab");
        match.Should().HaveGroupCount(3);

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => match.Should().HaveGroupCount(2));
        Assert.AreEqual("Expected match to have 2 group(s), but found 3.", ex.Message);
    }

    [TestMethod]
    public void NullSubjects_FailWithTheDedicatedMessage() {
        Match? missing = null;

        var groupEx = Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().HaveGroup("x", "y"));
        Assert.AreEqual("Expected missing to be a successful match, but the match is null.", groupEx.Message);

        var countEx = Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().HaveGroupCount(1));
        Assert.AreEqual("Expected missing to be a successful match, but the match is null.", countEx.Message);
    }

    [TestMethod]
    public void NullArguments_ThrowArgumentNull() {
        var match = SingleGroupRegex().Match("a");

        var nameEx = Assert.ThrowsExactly<ArgumentNullException>(() => match.Should().HaveGroup((string)null!, "a"));
        Assert.AreEqual("name", nameEx.ParamName);

        var expectedEx = Assert.ThrowsExactly<ArgumentNullException>(() => match.Should().HaveGroup("1", null!));
        Assert.AreEqual("expected", expectedEx.ParamName);

        var valueEx = Assert.ThrowsExactly<ArgumentNullException>(() => match.Should().HaveValue(null!));
        Assert.AreEqual("expected", valueEx.ParamName);
    }

    [GeneratedRegex(@"\d{4}")]
    private static partial Regex FourDigitsRegex();

    [GeneratedRegex(@"\d{4}-\d{2}-\d{2}")]
    private static partial Regex IsoDateRegex();

    [GeneratedRegex(@"\d+")]
    private static partial Regex DigitsRegex();

    [GeneratedRegex(@"(\d{4})-(\d{2})")]
    private static partial Regex IndexedGroupsRegex();

    [GeneratedRegex(@"(?<year>\d{4})-(?<month>\d{2})")]
    private static partial Regex NamedGroupsRegex();

    [GeneratedRegex("(a)?(b)")]
    private static partial Regex OptionalThenRequiredRegex();

    [GeneratedRegex("(?<x>a)")]
    private static partial Regex NamedXRegex();

    [GeneratedRegex("(a)(b)")]
    private static partial Regex TwoGroupsRegex();

    [GeneratedRegex("(a)")]
    private static partial Regex SingleGroupRegex();
}

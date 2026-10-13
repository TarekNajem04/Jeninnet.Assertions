//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Tests;

[TestClass]
public partial class StringPatternTests {
    [GeneratedRegex(@"\d+")]
    private static partial Regex DigitsRegex();
    [TestMethod]
    public void StartWith_PassForPrefix() {
        const string subject = "hello";
        subject.Should().StartWith("he");
    }

    [TestMethod]
    public void StartWith_FailsForMissingPrefixAndNull() {
        const string subject = "hello";
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => subject.Should().StartWith("bye"));
        Assert.AreEqual("Expected subject to start with 'bye', but found 'hello'.", ex.Message);

        const string? missing = null;
        var nullEx = Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().StartWith("he"));
        Assert.AreEqual("Expected missing to start with 'he', but it was null.", nullEx.Message);
    }

    [TestMethod]
    public void NotStartWith_FailsForPrefix() {
        const string subject = "hello";
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => subject.Should().NotStartWith("he"));
        Assert.AreEqual("Expected subject not to start with 'he', but found 'hello'.", ex.Message);
    }

    [TestMethod]
    public void StartWithEquivalentOf_IgnoresCasing() {
        const string subject = "Hello";
        subject.Should().StartWithEquivalentOf("he");

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => subject.Should().StartWithEquivalentOf("bye"));
        Assert.AreEqual("Expected subject to start with equivalent of 'bye', but found 'Hello'.", ex.Message);
    }

    [TestMethod]
    public void NotStartWithEquivalentOf_FailsForPrefix() {
        const string subject = "Hello";
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => subject.Should().NotStartWithEquivalentOf("HE"));
        Assert.AreEqual("Expected subject not to start with equivalent of 'HE', but found 'Hello'.", ex.Message);
    }

    [TestMethod]
    public void EndWith_PassForSuffix() {
        const string subject = "file.cs";
        subject.Should().EndWith(".cs");
    }

    [TestMethod]
    public void EndWithEquivalentOf_IgnoresCasing() {
        const string subject = "file.CS";
        subject.Should().EndWithEquivalentOf(".cs");

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => subject.Should().EndWithEquivalentOf(".txt"));
        Assert.AreEqual("Expected subject to end with equivalent of '.txt', but found 'file.CS'.", ex.Message);
    }

    [TestMethod]
    public void NotEndWith_FailsForSuffix() {
        const string subject = "file.cs";
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => subject.Should().NotEndWith(".cs"));
        Assert.AreEqual("Expected subject not to end with '.cs', but found 'file.cs'.", ex.Message);
    }

    [TestMethod]
    public void Contain_WithOccurrence_PassForExactCount() {
        const string subject = "aaa";
        subject.Should().Contain("a", Exactly.Thrice());
    }

    [TestMethod]
    public void Contain_WithOccurrence_FailsReportingCounts() {
        const string subject = "aaa";
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => subject.Should().Contain("a", Exactly.Twice()));
        Assert.AreEqual("Expected subject to contain 'a' exactly twice, but found 3 occurrences.", ex.Message);
    }

    [TestMethod]
    public void Contain_WithOccurrenceSugar_AtLeast() {
        const string subject = "aa";
        subject.Should().Contain("a", 2.TimesOrMore());

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => subject.Should().Contain("a", AtLeast.Times(3)));
        Assert.AreEqual("Expected subject to contain 'a' at least thrice, but found 2 occurrences.", ex.Message);
    }

    [TestMethod]
    public void ContainAll_PassWhenAllPresent() {
        const string subject = "hello world";
        subject.Should().ContainAll("hello", "world");
    }

    [TestMethod]
    public void ContainAll_FailsNamingMissing() {
        const string subject = "hello world";
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => subject.Should().ContainAll("hello", "bye"));
        Assert.AreEqual("Expected subject to contain 'bye', but it was not found.", ex.Message);
    }

    [TestMethod]
    public void NotContainAll_FailsWhenAllPresent() {
        const string subject = "hello world";
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => subject.Should().NotContainAll("hello", "world"));
        Assert.AreEqual("Expected subject not to contain all of the given strings, but all of them were found.", ex.Message);
    }

    [TestMethod]
    public void ContainAny_PassForOneMatch() {
        const string subject = "hello";
        subject.Should().ContainAny("x", "ell");
    }

    [TestMethod]
    public void ContainAny_FailsWhenNoneMatches() {
        const string subject = "hello";
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => subject.Should().ContainAny("x", "y"));
        Assert.AreEqual("Expected subject to contain at least one of 'x', 'y', but none was found.", ex.Message);
    }

    [TestMethod]
    public void NotContainAny_FailsNamingFound() {
        const string subject = "hello";
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => subject.Should().NotContainAny("x", "ell"));
        Assert.AreEqual("Expected subject not to contain any of 'x', 'ell', but found 'ell'.", ex.Message);
    }

    [TestMethod]
    public void ContainEquivalentOf_IgnoresCasing() {
        const string subject = "Hello World";
        subject.Should().ContainEquivalentOf("WORLD");

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => subject.Should().ContainEquivalentOf("BYE"));
        Assert.AreEqual("Expected subject to contain equivalent of 'BYE', but found 'Hello World'.", ex.Message);
    }

    [TestMethod]
    public void ContainEquivalentOf_WithOccurrence_CountsIgnoreCase() {
        const string subject = "aAa";
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => subject.Should().ContainEquivalentOf("a", Exactly.Twice()));
        Assert.AreEqual("Expected subject to contain equivalent of 'a' exactly twice, but found 3 occurrences.", ex.Message);
    }

    [TestMethod]
    public void NotContainEquivalentOf_FailsForMatch() {
        const string subject = "Hello";
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => subject.Should().NotContainEquivalentOf("ELL"));
        Assert.AreEqual("Expected subject not to contain equivalent of 'ELL', but found 'Hello'.", ex.Message);
    }

    [TestMethod]
    public void BeEquivalentTo_IgnoresCaseByDefault() {
        const string subject = "Hello";
        subject.Should().BeEquivalentTo("HELLO");
    }

    [TestMethod]
    public void BeEquivalentTo_WithOptions_IgnoresWhitespaceAndNewlines() {
        const string subject = "  hello";
        subject.Should().BeEquivalentTo("hello", static o => o.IgnoringLeadingWhitespace());

        const string multiline = "a\r\nb";
        multiline.Should().BeEquivalentTo("a\nb", static o => o.IgnoringNewlineStyle());

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => subject.Should().BeEquivalentTo("HELLO", static o => o.Using(StringComparer.Ordinal)));
        Assert.AreEqual("Expected subject to be equivalent to 'HELLO', but found '  hello'.", ex.Message);
    }

    [TestMethod]
    public void NotBeEquivalentTo_FailsForEquivalent() {
        const string subject = "hello";
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => subject.Should().NotBeEquivalentTo("HELLO"));
        Assert.AreEqual("Expected subject not to be equivalent to 'HELLO', but found 'hello'.", ex.Message);
    }

    [TestMethod]
    public void Match_PassForWildcards() {
        const string email = "user@example.com";
        email.Should().Match("*@*.com");
        "hello".Should().Match("h?llo");
    }

    [TestMethod]
    public void Match_FailsForMismatch() {
        const string subject = "hello";
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => subject.Should().Match("h?llo!"));
        Assert.AreEqual("Expected subject to match 'h?llo!', but found 'hello'.", ex.Message);
    }

    [TestMethod]
    public void NotMatch_FailsForMatch() {
        const string subject = "hello";
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => subject.Should().NotMatch("h*"));
        Assert.AreEqual("Expected subject not to match 'h*', but found 'hello'.", ex.Message);
    }

    [TestMethod]
    public void MatchEquivalentOf_IgnoresCasing() {
        const string subject = "Hello";
        subject.Should().MatchEquivalentOf("h*");

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => subject.Should().MatchEquivalentOf("B*"));
        Assert.AreEqual("Expected subject to match equivalent of 'B*', but found 'Hello'.", ex.Message);
    }

    [TestMethod]
    public void NotMatchEquivalentOf_FailsForMatch() {
        const string subject = "Hello";
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => subject.Should().NotMatchEquivalentOf("H*"));
        Assert.AreEqual("Expected subject not to match equivalent of 'H*', but found 'Hello'.", ex.Message);
    }

    [TestMethod]
    public void MatchRegex_PassForPattern() {
        const string subject = "order 123 shipped";
        subject.Should().MatchRegex(@"\d+");
        subject.Should().MatchRegex(DigitsRegex());
    }

    [TestMethod]
    public void MatchRegex_FailsWithoutMatch() {
        const string subject = "hello";
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => subject.Should().MatchRegex(@"\d+"));
        Assert.AreEqual(@"Expected subject to match regex '\d+', but found 'hello'.", ex.Message);
    }

    [TestMethod]
    public void MatchRegex_WithOccurrence_CountsMatches() {
        const string subject = "a1b2c3";
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => subject.Should().MatchRegex(@"\d", Exactly.Twice()));
        Assert.AreEqual(@"Expected subject to match regex '\d' exactly twice, but found 3 matches.", ex.Message);
    }

    [TestMethod]
    public void NotMatchRegex_FailsForMatch() {
        const string subject = "abc123";
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => subject.Should().NotMatchRegex(@"\d+"));
        Assert.AreEqual(@"Expected subject not to match regex '\d+', but found 'abc123'.", ex.Message);
    }

    [TestMethod]
    public void NullPattern_ThrowsArgumentNull() {
        const string subject = "hello";
        Assert.ThrowsExactly<ArgumentNullException>(() => subject.Should().StartWith(null!));
        Assert.ThrowsExactly<ArgumentNullException>(() => subject.Should().Contain(null!));
        Assert.ThrowsExactly<ArgumentNullException>(() => subject.Should().Match(null!));
        Assert.ThrowsExactly<ArgumentNullException>(() => subject.Should().MatchRegex((string)null!));
    }

    [TestMethod]
    public void EquivalentOf_WithOptions_IgnoresWhitespace() {
        const string subject = "  Hello World  ";
        subject.Should().ContainEquivalentOf("hello", static o => o.IgnoringLeadingWhitespace().IgnoringTrailingWhitespace());
        subject.Should().StartWithEquivalentOf("hello", static o => o.IgnoringLeadingWhitespace());
        subject.Should().EndWithEquivalentOf("world", static o => o.IgnoringTrailingWhitespace());
        subject.Should().NotContainEquivalentOf("bye", static o => o.IgnoringLeadingWhitespace());
        subject.Should().MatchEquivalentOf("hello*", static o => o.IgnoringLeadingWhitespace());

        var ex = Assert.ThrowsExactly<AssertionFailedException>(
            () => subject.Should().StartWithEquivalentOf("hello", static o => o.IgnoringTrailingWhitespace()));
        Assert.AreEqual("Expected subject to start with equivalent of 'hello', but found '  Hello World  '.", ex.Message);
    }

    [TestMethod]
    public void EquivalentOf_WithOptions_IgnoresNewlineStyle() {
        const string subject = "a\r\nb";
        subject.Should().ContainEquivalentOf("a\nb", static o => o.IgnoringNewlineStyle());
        subject.Should().StartWithEquivalentOf("a\nb", static o => o.IgnoringNewlineStyle());

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => subject.Should().ContainEquivalentOf("a\nb"));
        Assert.AreEqual("Expected subject to contain equivalent of 'a\nb', but found 'a\r\nb'.", ex.Message);
    }

    [TestMethod]
    public void EquivalentOf_WithNullOptions_UsesDefaults() {
        const string subject = "Hello";
        subject.Should().ContainEquivalentOf("HELLO", null);
        subject.Should().StartWithEquivalentOf("HE", null);
        subject.Should().EndWithEquivalentOf("LO", null);
        subject.Should().MatchEquivalentOf("H*", null);
        subject.Should().NotContainEquivalentOf("BYE", null);
        subject.Should().NotStartWithEquivalentOf("BYE", null);
        subject.Should().NotEndWithEquivalentOf("BYE", null);
        subject.Should().ContainEquivalentOf("L", Exactly.Twice(), null);
    }

    [TestMethod]
    public void EquivalentOf_WithOptions_OccurrenceCounts() {
        const string subject = "  aA  ";
        subject.Should().ContainEquivalentOf("a", Exactly.Twice(), static o => o.IgnoringLeadingWhitespace().IgnoringTrailingWhitespace());

        var ex = Assert.ThrowsExactly<AssertionFailedException>(
            () => subject.Should().ContainEquivalentOf("a", Exactly.Times(4), static o => o.IgnoringLeadingWhitespace()));
        Assert.Contains("to contain equivalent of 'a' exactly 4 times", ex.Message);
    }
}

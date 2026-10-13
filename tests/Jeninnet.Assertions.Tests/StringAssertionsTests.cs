//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Tests;

[TestClass]
public class StringAssertionsTests {
    [TestMethod]
    public void Be_PassForEqualStrings() => "hello".Should().Be("hello");

    [TestMethod]
    public void Be_UsesOrdinalComparison() => Assert.ThrowsExactly<AssertionFailedException>(() => "Hello".Should().Be("hello"));

    [TestMethod]
    public void Be_FailsForDifferentValue() {
        const string subject = "actual";
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => subject.Should().Be("expected"));
        Assert.AreEqual("Expected subject to be 'expected', but found 'actual'.", ex.Message);
    }

    [TestMethod]
    public void Be_HandlesNullSubjectAndExpected() {
        const string? subject = null;
        subject.Should().Be(null);

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => subject.Should().Be("x"));
        Assert.AreEqual("Expected subject to be 'x', but found <null>.", ex.Message);
    }

    [TestMethod]
    public void BeNull_PassForNull() => ((string?)null).Should().BeNull();

    [TestMethod]
    public void BeNull_FailsForValue() {
        const string subject = "x";
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => subject.Should().BeNull());
        Assert.AreEqual("Expected subject to be null, but it was not null.", ex.Message);
    }

    [TestMethod]
    public void NotBeNull_PassForValue() => "x".Should().NotBeNull();

    [TestMethod]
    public void NotBeNull_FailsForNull() {
        const string? subject = null;
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => subject.Should().NotBeNull());
        Assert.AreEqual("Expected subject to not be null, but found null.", ex.Message);
    }

    [TestMethod]
    public void Contain_PassForSubstring() => "hello world".Should().Contain("world");

    [TestMethod]
    public void Contain_FailsForMissingSubstring() {
        const string subject = "hello";
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => subject.Should().Contain("bye"));
        Assert.AreEqual("Expected subject to contain 'bye', but found 'hello'.", ex.Message);
    }

    [TestMethod]
    public void Contain_FailsForNullSubject() {
        const string? subject = null;
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => subject.Should().Contain("x"));
        Assert.AreEqual("Expected subject to contain 'x', but it was null.", ex.Message);
    }

    [TestMethod]
    public void EndsWith_PassForSuffix() => "file.cs".Should().EndsWith(".cs");

    [TestMethod]
    public void EndsWith_Alias_MatchesEndWith() {
        const string subject = "file.cs";
        subject.Should().EndsWith(".cs").And.EndWith(".cs");
    }

    [TestMethod]
    public void EndsWith_FailsForMissingSuffix() {
        const string subject = "file.cs";
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => subject.Should().EndsWith(".txt"));
        Assert.AreEqual("Expected subject to end with '.txt', but found 'file.cs'.", ex.Message);
    }

    [TestMethod]
    public void BeNullOrEmpty_PassForNull() {
        const string? subject = null;
        subject.Should().BeNullOrEmpty();
    }

    [TestMethod]
    public void BeNullOrEmpty_PassForEmpty() => "".Should().BeNullOrEmpty();

    [TestMethod]
    public void BeNullOrEmpty_FailsForNonEmpty() {
        const string subject = "hello";
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => subject.Should().BeNullOrEmpty());
        Assert.AreEqual("Expected subject to be null or empty, but found 'hello'.", ex.Message);
    }

    [TestMethod]
    public void NotBeNullOrEmpty_PassForNonEmpty() => "hello".Should().NotBeNullOrEmpty();

    [TestMethod]
    public void NotBeNullOrEmpty_FailsForNull() {
        const string? subject = null;
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => subject.Should().NotBeNullOrEmpty());
        Assert.AreEqual("Expected subject not to be null or empty, but it was.", ex.Message);
    }

    [TestMethod]
    public void NotBeNullOrEmpty_FailsForEmpty() {
        const string subject = "";
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => subject.Should().NotBeNullOrEmpty());
        Assert.AreEqual("Expected subject not to be null or empty, but it was.", ex.Message);
    }

    [TestMethod]
    public void NotContainLine_PassForMissingLine() => "hello\nworld".Should().NotContainLine("foo");

    [TestMethod]
    public void NotContainLine_FailsForPresentLine() {
        const string subject = "hello\nworld";
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => subject.Should().NotContainLine("world"));
        Assert.AreEqual("Expected subject not to contain line 'world', but found it.", ex.Message);
    }

    [TestMethod]
    public void NotHaveLineCount_PassForDifferentCount() => "hello\nworld".Should().NotHaveLineCount(3);

    [TestMethod]
    public void NotHaveLineCount_FailsForMatchingCount() {
        const string subject = "hello\nworld";
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => subject.Should().NotHaveLineCount(2));
        Assert.AreEqual("Expected subject not to have 2 line(s), but found 2.", ex.Message);
    }

    [TestMethod]
    public void ContainLine_PassAndExposeMatch() {
        const string subject = "hello\nworld";
        subject.Should().ContainLine("world").Which.Should().Be("world");
    }

    [TestMethod]
    public void ContainLine_FailsForMissingAndPartialLines() {
        const string subject = "hello\nworld";

        var missing = Assert.ThrowsExactly<AssertionFailedException>(() => subject.Should().ContainLine("foo"));
        Assert.AreEqual("Expected subject to contain line 'foo', but it was not found.", missing.Message);

        var partial = Assert.ThrowsExactly<AssertionFailedException>(() => subject.Should().ContainLine("orl"));
        Assert.AreEqual("Expected subject to contain line 'orl', but it was not found.", partial.Message);
    }

    [TestMethod]
    public void ContainLine_MatchesEmptyAndSingleLines() {
        const string blank = "hello\n\nworld";
        blank.Should().ContainLine("").Which.Should().Be("");

        const string single = "hello";
        single.Should().ContainLine("hello");
    }

    [TestMethod]
    public void ContainLine_FailsForNullSubject() {
        const string? subject = null;
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => subject.Should().ContainLine("hello"));
        Assert.AreEqual("Expected subject to contain line 'hello', but it was not found.", ex.Message);
    }

    [TestMethod]
    public void HaveLineCount_PassAndFail() {
        "hello\nworld".Should().HaveLineCount(2);
        "hello".Should().HaveLineCount(1);
        "".Should().HaveLineCount(0);
        "hello\nworld\n".Should().HaveLineCount(3);

        const string subject = "hello\nworld";
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => subject.Should().HaveLineCount(3));
        Assert.AreEqual("Expected subject to have 3 line(s), but found 2.", ex.Message);
    }

    [TestMethod]
    public void HaveLineCount_FailsForNull() {
        const string? subject = null;
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => subject.Should().HaveLineCount(0));
        Assert.AreEqual("Expected subject to have 0 line(s), but found null.", ex.Message);
    }
}

//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Tests;

[TestClass]
public class StringContentTests {
    [TestMethod]
    public void NotBe_PassForDifferentValue() {
        const string subject = "a";
        subject.Should().NotBe("b");
    }

    [TestMethod]
    public void NotBe_FailsForEqualValue() {
        const string subject = "x";
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => subject.Should().NotBe("x"));
        Assert.AreEqual("Expected subject not to be 'x', but found 'x'.", ex.Message);
    }

    [TestMethod]
    public void BeEmpty_PassForEmpty() {
        const string subject = "";
        subject.Should().BeEmpty();
    }

    [TestMethod]
    public void BeEmpty_FailsForValueAndNull() {
        const string subject = "x";
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => subject.Should().BeEmpty());
        Assert.AreEqual("Expected subject to be empty, but found 'x'.", ex.Message);

        const string? missing = null;
        var nullEx = Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().BeEmpty());
        Assert.AreEqual("Expected missing to be empty, but it was null.", nullEx.Message);
    }

    [TestMethod]
    public void NotBeEmpty_PassForValueAndNull() {
        const string subject = "x";
        subject.Should().NotBeEmpty();
        const string? missing = null;
        missing.Should().NotBeEmpty();
    }

    [TestMethod]
    public void NotBeEmpty_FailsForEmpty() {
        const string subject = "";
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => subject.Should().NotBeEmpty());
        Assert.AreEqual("Expected subject not to be empty, but it was empty.", ex.Message);
    }

    [TestMethod]
    public void BeNullOrWhiteSpace_PassForNullEmptyAndWhitespace() {
        const string? missing = null;
        missing.Should().BeNullOrWhiteSpace();
        "".Should().BeNullOrWhiteSpace();
        "   ".Should().BeNullOrWhiteSpace();
    }

    [TestMethod]
    public void BeNullOrWhiteSpace_FailsForContent() {
        const string subject = "x";
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => subject.Should().BeNullOrWhiteSpace());
        Assert.AreEqual("Expected subject to be null or whitespace, but found 'x'.", ex.Message);
    }

    [TestMethod]
    public void NotBeNullOrWhiteSpace_FailsForNullAndWhitespace() {
        const string? missing = null;
        var nullEx = Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().NotBeNullOrWhiteSpace());
        Assert.AreEqual("Expected missing not to be null or whitespace, but found <null>.", nullEx.Message);

        const string subject = "   ";
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => subject.Should().NotBeNullOrWhiteSpace());
        Assert.AreEqual("Expected subject not to be null or whitespace, but found '   '.", ex.Message);
    }

    [TestMethod]
    public void HaveLength_PassForExactLength() {
        const string subject = "hey";
        subject.Should().HaveLength(3);
    }

    [TestMethod]
    public void HaveLength_FailsForDifferentLengthAndNull() {
        const string subject = "hi";
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => subject.Should().HaveLength(3));
        Assert.AreEqual("Expected subject to have length 3, but found 'hi' with length 2.", ex.Message);

        const string? missing = null;
        var nullEx = Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().HaveLength(3));
        Assert.AreEqual("Expected missing to have length 3, but it was null.", nullEx.Message);
    }

    [TestMethod]
    public void NotHaveLength_PassForDifferentLengthAndNull() {
        const string subject = "hi";
        subject.Should().NotHaveLength(3);
        const string? missing = null;
        missing.Should().NotHaveLength(3);
    }

    [TestMethod]
    public void NotHaveLength_FailsForSameLength() {
        const string subject = "hi";
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => subject.Should().NotHaveLength(2));
        Assert.AreEqual("Expected subject not to have length 2, but found 'hi' with length 2.", ex.Message);
    }

    [TestMethod]
    public void BeUpperCased_PassIgnoresCaselessCharacters() {
        const string subject = "ABC 123!";
        subject.Should().BeUpperCased();
    }

    [TestMethod]
    public void BeUpperCased_FailsForLowerCaseAndNull() {
        const string subject = "aBC";
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => subject.Should().BeUpperCased());
        Assert.AreEqual("Expected subject to be upper cased, but found 'aBC'.", ex.Message);

        const string? missing = null;
        var nullEx = Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().BeUpperCased());
        Assert.AreEqual("Expected missing to be upper cased, but found <null>.", nullEx.Message);
    }

    [TestMethod]
    public void NotBeUpperCased_PassForNullAndCaseless() {
        const string? missing = null;
        missing.Should().NotBeUpperCased();
        "123".Should().NotBeUpperCased();
        "aBC".Should().NotBeUpperCased();
    }

    [TestMethod]
    public void NotBeUpperCased_FailsForAllUpper() {
        const string subject = "ABC";
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => subject.Should().NotBeUpperCased());
        Assert.AreEqual("Expected subject not to be upper cased, but found 'ABC'.", ex.Message);
    }

    [TestMethod]
    public void BeLowerCased_PassIgnoresCaselessCharacters() {
        const string subject = "abc 123!";
        subject.Should().BeLowerCased();
    }

    [TestMethod]
    public void BeLowerCased_FailsForUpperCase() {
        const string subject = "aBC";
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => subject.Should().BeLowerCased());
        Assert.AreEqual("Expected subject to be lower cased, but found 'aBC'.", ex.Message);
    }

    [TestMethod]
    public void NotBeLowerCased_FailsForAllLower() {
        const string subject = "abc";
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => subject.Should().NotBeLowerCased());
        Assert.AreEqual("Expected subject not to be lower cased, but found 'abc'.", ex.Message);
    }

    [TestMethod]
    public void BeOneOf_PassForCandidate() {
        const string subject = "b";
        subject.Should().BeOneOf("a", "b", "c");
    }

    [TestMethod]
    public void BeOneOf_FailsListingCandidates() {
        const string subject = "z";
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => subject.Should().BeOneOf("a", "b"));
        Assert.AreEqual("Expected subject to be one of 'a', 'b', but found 'z'.", ex.Message);
    }
}

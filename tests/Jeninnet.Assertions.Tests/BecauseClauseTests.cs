//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Tests;

[TestClass]
public class BecauseClauseTests {
    [TestMethod]
    public void Because_IsAppendedToDefaultMessage() {
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => false.Should().BeTrue("the feature flag is enabled"));
        Assert.AreEqual("Expected false to be true, but found false because the feature flag is enabled.", ex.Message);
    }

    [TestMethod]
    public void Because_WithLeadingBecauseKeyword_IsNotDuplicated() {
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => false.Should().BeTrue("because the feature flag is enabled"));
        Assert.AreEqual("Expected false to be true, but found false because the feature flag is enabled.", ex.Message);
    }

    [TestMethod]
    public void BecauseArgs_AreFormattedIntoMessage() {
        int[] values = [1];
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => values.Should().HaveCount(2, "we inserted {0} rows", 2));
        Assert.AreEqual("Expected values to contain 2 item(s), but found 1 because we inserted 2 rows.", ex.Message);
    }

    [TestMethod]
    public void EmptyBecause_KeepsDefaultMessage() {
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => false.Should().BeTrue(""));
        Assert.AreEqual("Expected false to be true, but found false.", ex.Message);
    }

    [TestMethod]
    public void Because_NamesSubjectCapturedFromCaller() {
        const string username = "dennis";
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => username.Should().Be("jonas", "the user was renamed"));
        Assert.AreEqual("Expected username to be 'jonas', but found 'dennis' because the user was renamed.", ex.Message);
    }
}

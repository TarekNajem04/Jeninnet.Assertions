//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Tests;

[TestClass]
public class StringDiffTests {
    [TestMethod]
    public void Be_ShortStrings_KeepOneLiner() {
        const string subject = "hello";
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => subject.Should().Be("bye"));
        Assert.AreEqual("Expected subject to be 'bye', but found 'hello'.", ex.Message);
    }

    [TestMethod]
    public void Be_MultilineChange_ShowsDiff() {
        const string actualValue = "line1\nCHANGED\nline3";
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => actualValue.Should().Be("line1\nline2\nline3"));
        Assert.AreEqual(
            "Expected actualValue to be 'line1\nline2\nline3', but found 'line1\nCHANGED\nline3'.\n--- expected\n+++ actual\n@@ -1,3 +1,3 @@\n line1\n-line2\n+CHANGED\n line3",
            ex.Message);
    }

    [TestMethod]
    public void Be_Insertion_ShowsDiff() {
        const string actualValue = "a\nx\nb";
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => actualValue.Should().Be("a\nb"));
        Assert.AreEqual(
            "Expected actualValue to be 'a\nb', but found 'a\nx\nb'.\n--- expected\n+++ actual\n@@ -1,2 +1,3 @@\n a\n+x\n b",
            ex.Message);
    }

    [TestMethod]
    public void Be_Deletion_ShowsDiff() {
        const string actualValue = "a\nb";
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => actualValue.Should().Be("a\nx\nb"));
        Assert.AreEqual(
            "Expected actualValue to be 'a\nx\nb', but found 'a\nb'.\n--- expected\n+++ actual\n@@ -1,3 +1,2 @@\n a\n-x\n b",
            ex.Message);
    }

    [TestMethod]
    public void Be_EmptyExpected_ShowsAddedLines() {
        const string actualValue = "line1\nline2";
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => actualValue.Should().Be(""));
        Assert.AreEqual(
            "Expected actualValue to be '', but found 'line1\nline2'.\n--- expected\n+++ actual\n@@ -0,0 +1,2 @@\n+line1\n+line2",
            ex.Message);
    }

    [TestMethod]
    public void Be_CrLfDifference_ShowsChangedPair() {
        const string actualValue = "a\nb";
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => actualValue.Should().Be("a\r\nb"));
        Assert.AreEqual(
            "Expected actualValue to be 'a\r\nb', but found 'a\nb'.\n--- expected\n+++ actual\n@@ -1,2 +1,2 @@\n-a\r\n+a\n b",
            ex.Message);
    }

    [TestMethod]
    public void Be_LongSingleLine_TruncatesOneLiner() {
        var expectedLong = new string('a', 250);
        var actualLong = new string('a', 249) + "b";
        var preview = new string('a', 100) + "…";
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => actualLong.Should().Be(expectedLong));
        Assert.AreEqual(
            $"Expected actualLong to be '{preview}', but found '{preview}'.\n--- expected\n+++ actual\n@@ -1,1 +1,1 @@\n-{expectedLong}\n+{actualLong}",
            ex.Message);
    }

    [TestMethod]
    public void BeEquivalentTo_Multiline_ShowsDiff() {
        const string greeting = "Hello\nWorld";
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => greeting.Should().BeEquivalentTo("Hello\nThere"));
        Assert.AreEqual(
            "Expected greeting to be equivalent to 'Hello\nThere', but found 'Hello\nWorld'.\n--- expected\n+++ actual\n@@ -1,2 +1,2 @@\n Hello\n-There\n+World",
            ex.Message);
    }

    [TestMethod]
    public void Be_WithBecause_AppendsReasonBeforeDiff() {
        const string actualValue = "line1\nCHANGED\nline3";
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => actualValue.Should().Be("line1\nline2\nline3", "output mismatch"));
        Assert.AreEqual(
            "Expected actualValue to be 'line1\nline2\nline3', but found 'line1\nCHANGED\nline3' because output mismatch.\n--- expected\n+++ actual\n@@ -1,3 +1,3 @@\n line1\n-line2\n+CHANGED\n line3",
            ex.Message);
    }

    [TestMethod]
    public void Be_ArabicMessage_KeepsInvariantDiff() {
        var previous = AssertionCulture.Current;
        AssertionCulture.Current = new CultureInfo("ar");
        try {
            const string actualValue = "line1\nCHANGED\nline3";
            var ex = Assert.ThrowsExactly<AssertionFailedException>(() => actualValue.Should().Be("line1\nline2\nline3"));
            Assert.AreEqual(
                "المتوقع من actualValue أن يكون 'line1\nline2\nline3'، لكن وُجد 'line1\nCHANGED\nline3'.\n--- expected\n+++ actual\n@@ -1,3 +1,3 @@\n line1\n-line2\n+CHANGED\n line3",
                ex.Message);
        }
        finally {
            AssertionCulture.Current = previous;
        }
    }

    [TestMethod]
    public void Be_HugeInput_SkipsDiffGracefully() {
        var bigExpected = string.Join("\n", Enumerable.Range(0, 1100).Select(static i => "line" + i));
        var bigActual = bigExpected + "\nx";
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => bigActual.Should().Be(bigExpected));
        Assert.DoesNotContain("@@", ex.Message);
    }
}

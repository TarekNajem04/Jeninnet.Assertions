//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Tests;

[TestClass]
public class FormattingLimitsTests {
    [TestMethod]
    public void ValueFormatterOptions_Defaults_AreBackwardCompatible() {
        var options = new ValueFormatterOptions();

        Assert.AreEqual(10, options.MaxDepth);
        Assert.AreEqual(100, options.MaxLines);
        Assert.AreEqual(100, options.MaxEnumerables);
        Assert.AreEqual(8_000, options.MaxStringLength);
        Assert.IsTrue(options.UseLineBreaks);
        Assert.IsFalse(options.RespectRuntimeTypes);
    }

    [TestMethod]
    public void MaxLines_TruncatesWithHiddenLineCount() {
        using var scope = new AssertionScope();
        scope.Formatting.MaxLines = 2;
        scope.Formatting.AddFormatter(new BlobFormatter());

        new Blob("a\nb\nc\nd").Should().Be(new Blob("z"));

        var failures = scope.Discard();
        Assert.HasCount(1, failures);
        Assert.Contains("a\nb" + Environment.NewLine + "… (2 more line(s))", failures[0]);
    }

    [TestMethod]
    public void MaxStringLength_TruncatesWithHiddenCharacterCount() {
        using var scope = new AssertionScope();
        scope.Formatting.MaxStringLength = 5;
        scope.Formatting.AddFormatter(new BlobFormatter());

        new Blob("1234567890").Should().Be(new Blob("z"));

        var failures = scope.Discard();
        Assert.HasCount(1, failures);
        Assert.Contains("12345… (5 more char(s))", failures[0]);
    }

    [TestMethod]
    public void UseLineBreaksFalse_CollapsesWhitespaceToSingleLine() {
        using var scope = new AssertionScope();
        scope.Formatting.UseLineBreaks = false;
        scope.Formatting.AddFormatter(new BlobFormatter());

        new Blob("a\n  b\tc").Should().Be(new Blob("z"));

        var failures = scope.Discard();
        Assert.HasCount(1, failures);
        Assert.Contains("a b c", failures[0]);
        Assert.DoesNotContain("\n", failures[0]);
    }

    [TestMethod]
    public void RespectRuntimeTypes_FormatsCollectionsStructurally() {
        using var scope = new AssertionScope();
        scope.Formatting.RespectRuntimeTypes = true;

        object actual = new[] { 1, 2, 3 };
        object expected = new[] { 1, 2, 4 };
        actual.Should().Be(expected);

        var failures = scope.Discard();
        Assert.HasCount(1, failures);
        Assert.Contains("[1, 2, 3]", failures[0]);
        Assert.Contains("[1, 2, 4]", failures[0]);
        Assert.DoesNotContain("System.Int32[]", failures[0]);
    }

    [TestMethod]
    public void RespectRuntimeTypes_RespectsMaxEnumerables() {
        using var scope = new AssertionScope();
        scope.Formatting.RespectRuntimeTypes = true;
        scope.Formatting.MaxEnumerables = 3;

        object actual = new[] { 1, 2, 3, 4, 5 };
        object expected = new[] { 9 };
        actual.Should().Be(expected);

        var failures = scope.Discard();
        Assert.HasCount(1, failures);
        Assert.Contains("[1, 2, 3, … (truncated)]", failures[0]);
    }

    [TestMethod]
    public void RespectRuntimeTypes_RespectsMaxDepth() {
        using var scope = new AssertionScope();
        scope.Formatting.RespectRuntimeTypes = true;
        scope.Formatting.MaxDepth = 1;

        object actual = new object[] { new[] { 1, 2 }, 3 };
        object expected = new object[] { 9 };
        actual.Should().Be(expected);

        var failures = scope.Discard();
        Assert.HasCount(1, failures);
        Assert.Contains("[… (max depth), 3]", failures[0]);
    }

    [TestMethod]
    public void RespectRuntimeTypes_StopsAtReferenceCycles() {
        var cyclic = new List<object>();
        cyclic.Add(cyclic);

        using var scope = new AssertionScope();
        scope.Formatting.RespectRuntimeTypes = true;

        object actual = cyclic;
        object expected = new object[] { 1 };
        actual.Should().Be(expected);

        var failures = scope.Discard();
        Assert.HasCount(1, failures);
        Assert.Contains("[… (cycle)]", failures[0]);
    }

    [TestMethod]
    public void MaxLines_LeavesTextWithinLimitUntouched() {
        using var scope = new AssertionScope();
        scope.Formatting.MaxLines = 3;
        scope.Formatting.AddFormatter(new BlobFormatter());

        new Blob("a\nb").Should().Be(new Blob("z"));

        var failures = scope.Discard();
        Assert.HasCount(1, failures);
        Assert.Contains("'a\nb'", failures[0]);
        Assert.DoesNotContain("more line(s)", failures[0]);
    }

    [TestMethod]
    public void MaxLines_UsesLocalizedMarker() {
        var before = AssertionCulture.Current;
        try {
            AssertionCulture.Current = new CultureInfo("ar");
            using var scope = new AssertionScope();
            scope.Formatting.MaxLines = 1;
            scope.Formatting.AddFormatter(new BlobFormatter());

            new Blob("a\nb\nc").Should().Be(new Blob("z"));

            var failures = scope.Discard();
            Assert.HasCount(1, failures);
            Assert.Contains("… (2 سطر إضافي مخفي)", failures[0]);
        }
        finally {
            AssertionCulture.Current = before;
        }
    }

    private sealed class Blob(string text) {
        public string Text { get; } = text;
    }

    private sealed class BlobFormatter : IValueFormatter {
        public bool CanFormat(object? value) => value is Blob;

        public string? Format(object? value) => value is Blob blob ? blob.Text : null;
    }
}

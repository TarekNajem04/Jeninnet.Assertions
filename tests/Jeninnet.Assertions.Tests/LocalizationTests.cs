//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Tests;

[TestClass]
public partial class LocalizationTests {
    private static async IAsyncEnumerable<int> Counting(params int[] items) {
        foreach(var item in items) {
            await Task.Yield();
            yield return item;
        }
    }

    [TestMethod]
    public void Arabic_ScopeHeader() {
        var previous = AssertionCulture.Current;
        AssertionCulture.Current = new CultureInfo("ar");
        try {
            var ex = Assert.ThrowsExactly<AssertionFailedException>(FailTwice);
            Assert.Contains("2 من إخفاقات التأكيد:", ex.Message);
        }
        finally {
            AssertionCulture.Current = previous;
        }

        static void FailTwice() {
            using var scope = new AssertionScope();
            const bool flag = false;
            flag.Should().BeTrue();
            flag.Should().NotBe(false);
        }
    }

    [TestMethod]
    public void UnknownCulture_FallsBackToEnglish() {
        var previous = AssertionCulture.Current;
        try {
            AssertionCulture.Current = new CultureInfo("fi");
            const bool flag = false;
            var ex = Assert.ThrowsExactly<AssertionFailedException>(() => flag.Should().BeTrue());
            Assert.AreEqual("Expected flag to be true, but found false.", ex.Message);
        }
        finally {
            AssertionCulture.Current = previous;
        }
    }

    [TestMethod]
    public void Current_DefaultsToEnglish() => Assert.AreEqual("en", AssertionCulture.Current.Name);

    [TestMethod]
    public void Current_Null_ThrowsArgumentNull() => Assert.ThrowsExactly<ArgumentNullException>(() => AssertionCulture.Current = null!);

    [TestMethod]
    public void French_TranslatedMessages_NoEnglishFallback() {
        using var _ = new FrenchScope();

        const bool flag = false;
        var beTrue = Assert.ThrowsExactly<AssertionFailedException>(() => flag.Should().BeTrue());
        Assert.AreEqual("Attendu que flag soit true, mais trouvé false.", beTrue.Message);

        const int number = -2;
        var positive = Assert.ThrowsExactly<AssertionFailedException>(() => number.Should().BePositive());
        Assert.AreEqual("Attendu que number soit positif, mais trouvé -2.", positive.Message);

        const string subject = "actual";
        var be = Assert.ThrowsExactly<AssertionFailedException>(() => subject.Should().Be("expected"));
        Assert.AreEqual("Attendu que subject soit 'expected', mais trouvé 'actual'.", be.Message);
    }

    [TestMethod]
    public void GetMessage_ReturnsLocalizedTemplate_ForCurrentCulture() {
        using var _ = new FrenchScope();
        Assert.AreEqual("Attendu que {0} soit true, mais trouvé false.", AssertionCulture.GetMessage("BOOLEAN_BE_TRUE"));
    }

    [TestMethod]
    public void GetMessage_FallsBackToEnglish_ForUntranslatedKey() {
        using var _ = new FrenchScope();
        Assert.AreEqual(
            "Expected {0} to be within {1} of the current moment, but found {2}.",
            AssertionCulture.GetMessage("DATE_BE_WITHIN_NOW"));
    }

    [TestMethod]
    public void FormatMessage_SubstitutesInvariantly() {
        using var _ = new FrenchScope();
        Assert.AreEqual("Attendu que count soit positif, mais trouvé 5.", AssertionCulture.FormatMessage("NUMERIC_BE_POSITIVE", "count", "5"));
    }

    [TestMethod]
    public void GetMessage_NullKey_ThrowsArgumentNull() =>
        Assert.ThrowsExactly<ArgumentNullException>(() => AssertionCulture.GetMessage(null!));

    [TestMethod]
    public void FormatMessage_NullArgs_ThrowsArgumentNull() =>
        Assert.ThrowsExactly<ArgumentNullException>(() => AssertionCulture.FormatMessage("BOOLEAN_BE_TRUE", null!));

    private sealed class FrenchScope : IDisposable {
        private readonly CultureInfo _previous = AssertionCulture.Current;

        public FrenchScope() => AssertionCulture.Current = new CultureInfo("fr");

        public void Dispose() => AssertionCulture.Current = _previous;
    }

    private sealed class ArabicScope : IDisposable {
        private readonly CultureInfo _previous = AssertionCulture.Current;

        public ArabicScope() => AssertionCulture.Current = new CultureInfo("ar");

        public void Dispose() => AssertionCulture.Current = _previous;
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Constructor)]
    private sealed class GadgetAttribute : Attribute;

    // Fixture members are located via reflection by the assertion tests, never invoked directly.

#pragma warning disable S1144
    private sealed class SampleWidget {

        public SampleWidget() {
        }

        // Fixture located via reflection (Arabic_ConstructorInfo); never called directly.
#pragma warning disable IDE0051
        private SampleWidget(int _) { }
#pragma warning restore IDE0051

        public string Name { get; set; } = "";

        // Fixture must stay an instance write-only property for the BeReadable test.
#pragma warning disable CA1822
        // Fixture is deliberately write-only so BeReadable can fail against it (IDE0052: the
        // backing field is assigned but never read, by the same design).
#pragma warning disable S2376, IDE0052
        private string _writeOnly = string.Empty;
        public string WriteOnly { set => _writeOnly = value; }
#pragma warning restore CA1822, S2376, IDE0052

        public string ReadOnly { get; } = "";

        public int Add(int x, int y) => x + y + Name.Length;
    }

    [Gadget]
    private sealed class MarkedWidget : BaseWidget;

    // Fixture: intentionally unsealed and empty so BeSealed can fail against it.

#pragma warning disable S2094 // Fixture: intentionally empty base type for type-assertion tests.
    private class BaseWidget;

    [Flags]
    private enum Access {
        None = 0,
        Read = 1,
        Write = 2,
    }

    private sealed class Publisher {
        public event EventHandler? Changed;

        public void RaiseChanged() => Changed?.Invoke(this, EventArgs.Empty);
    }

    private sealed class NonSeekableStream : MemoryStream {
        public override bool CanSeek => false;

        public override bool CanWrite => false;
    }

    private static void ThrowArgument(string myParam) => throw new ArgumentException("bad", myParam);

    [GeneratedRegex(@"(?<year>\d{4})-(?<month>\d{2})")]
    private static partial Regex NamedYearMonthRegex();
}

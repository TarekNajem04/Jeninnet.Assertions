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
            var ex = Throws<AssertionFailedException>(FailTwice);
            ex.Message.Should().Contain("2 من إخفاقات التأكيد:");
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
            var ex = Throws<AssertionFailedException>(() => flag.Should().BeTrue());
            ex.Message.Should().Be("Expected flag to be true, but found false.");
        }
        finally {
            AssertionCulture.Current = previous;
        }
    }

    [TestMethod]
    public void Current_DefaultsToEnglish() => AssertionCulture.Current.Name.Should().Be("en");

    [TestMethod]
    public void Current_Null_ThrowsArgumentNull() => Throws<ArgumentNullException>(() => AssertionCulture.Current = null!);

    [TestMethod]
    public void French_TranslatedMessages_NoEnglishFallback() {
        using var _ = new FrenchScope();

        const bool flag = false;
        var beTrue = Throws<AssertionFailedException>(() => flag.Should().BeTrue());
        beTrue.Message.Should().Be("Attendu que flag soit true, mais trouvé false.");

        const int number = -2;
        var positive = Throws<AssertionFailedException>(() => number.Should().BePositive());
        positive.Message.Should().Be("Attendu que number soit positif, mais trouvé -2.");

        const string subject = "actual";
        var be = Throws<AssertionFailedException>(() => subject.Should().Be("expected"));
        be.Message.Should().Be("Attendu que subject soit 'expected', mais trouvé 'actual'.");
    }

    [TestMethod]
    public void GetMessage_ReturnsLocalizedTemplate_ForCurrentCulture() {
        using var _ = new FrenchScope();
        AssertionCulture.GetMessage("BOOLEAN_BE_TRUE").Should().Be("Attendu que {0} soit true, mais trouvé false.");
    }

    [TestMethod]
    public void GetMessage_FallsBackToEnglish_ForUntranslatedKey() {
        using var _ = new FrenchScope();
        AssertionCulture.GetMessage("DATE_BE_WITHIN_NOW").Should().Be(
            "Expected {0} to be within {1} of the current moment, but found {2}.");
    }

    [TestMethod]
    public void FormatMessage_SubstitutesInvariantly() {
        using var _ = new FrenchScope();
        AssertionCulture.FormatMessage("NUMERIC_BE_POSITIVE", "count", "5").Should().Be("Attendu que count soit positif, mais trouvé 5.");
    }

    [TestMethod]
    public void GetMessage_NullKey_ThrowsArgumentNull() =>
        Throws<ArgumentNullException>(() => AssertionCulture.GetMessage(null!));

    [TestMethod]
    public void FormatMessage_NullArgs_ThrowsArgumentNull() =>
        Throws<ArgumentNullException>(() => AssertionCulture.FormatMessage("BOOLEAN_BE_TRUE", null!));

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

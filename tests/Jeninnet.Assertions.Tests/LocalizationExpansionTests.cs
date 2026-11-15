//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Tests;

[TestClass]
public class LocalizationExpansionTests {
    [TestMethod]
    public void PseudoLocale_TransformsResourceMessages() {
        using var _ = new CultureScope("qps-ploc");

        const bool flag = false;
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => flag.Should().BeTrue());

        Assert.IsTrue(ex.Message.StartsWith("[!!", StringComparison.Ordinal));
        Assert.Contains("ƒ", ex.Message);
        Assert.DoesNotContain("Expected", ex.Message);
    }

    [TestMethod]
    public void PseudoLocale_RevealsHardcodedText() {
        using var _ = new CultureScope("qps-ploc");
        using var scope = new AssertionScope();

        new HardcodedAssertions().FailHardcoded();

        var failure = scope.Discard().Single();
        Assert.AreEqual("Hardcoded message.", failure);
    }

    [TestMethod]
    public void RegionalCultures_FallBackToParentSatellite() {
        using(var _ = new CultureScope("ar-EG")) {
            const bool flag = false;
            var ex = Assert.ThrowsExactly<AssertionFailedException>(() => flag.Should().BeTrue());
            Assert.Contains("المتوقع من flag", ex.Message);
        }

        using(var _ = new CultureScope("fr-CA")) {
            const bool flag = false;
            var ex = Assert.ThrowsExactly<AssertionFailedException>(() => flag.Should().BeTrue());
            Assert.AreEqual("Attendu que flag soit true, mais trouvé false.", ex.Message);
        }
    }

    [TestMethod]
    public void SpanishAndGerman_TranslateMessagesBecauseAndScopeHeader() {
        foreach(var culture in new[] { "es-ES", "de-DE" }) {
            using var _ = new CultureScope(culture);
            var expected = culture.StartsWith("es", StringComparison.Ordinal)
                ? "Se esperaba que flag fuera true, pero se encontró false."
                : "Erwartet wurde, dass flag true ist, aber false gefunden wurde.";
            var because = culture.StartsWith("es", StringComparison.Ordinal) ? "porque" : "weil";

            Assert.AreEqual(expected, AssertionCulture.FormatMessage("BOOLEAN_BE_TRUE", "flag"));
            Assert.AreNotEqual("Expected {0} to be true, but found false.", AssertionCulture.GetMessage("BOOLEAN_BE_TRUE"), culture);
            Assert.AreNotEqual("because ", AssertionCulture.GetMessage("MESSAGE_BECAUSE"), culture);
            Assert.AreNotEqual("{0} assertion failures:", AssertionCulture.GetMessage("SCOPE_ASSERTION_FAILURES"), culture);

            const bool flag = false;
            var ex = Assert.ThrowsExactly<AssertionFailedException>(() => flag.Should().BeTrue("a reason"));
            Assert.Contains(because + " a reason", ex.Message);
            Assert.DoesNotContain("Expected", ex.Message);
        }
    }

    [TestMethod]
    public void InvariantNumbers_AreStableAcrossCultures() {
        var previous = CultureInfo.CurrentCulture;
        try {
            foreach(var culture in new[] { "de-DE", "es-ES", "ar-EG", "fr-FR" }) {
                using var _ = new CultureScope(culture);
                CultureInfo.CurrentCulture = new CultureInfo(culture);

                const decimal actual = 1.5m;
                var ex = Assert.ThrowsExactly<AssertionFailedException>(() => actual.Should().Be(2m));
                Assert.Contains("1.5", ex.Message);
                Assert.DoesNotContain("1,5", ex.Message);
            }
        }
        finally {
            CultureInfo.CurrentCulture = previous;
        }
    }

    [TestMethod]
    public void ArabicMessage_StartsWithTranslatedPrefix() {
        using var _ = new CultureScope("ar");

        const bool flag = false;
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => flag.Should().BeTrue());

        Assert.IsTrue(ex.Message.StartsWith("المتوقع من flag", StringComparison.Ordinal));
        Assert.DoesNotContain("Expected", ex.Message);
    }

    private sealed class CultureScope : IDisposable {
        private readonly CultureInfo _previous = AssertionCulture.Current;

        internal CultureScope(string culture) => AssertionCulture.Current = new CultureInfo(culture);

        public void Dispose() => AssertionCulture.Current = _previous;
    }

    private sealed class HardcodedAssertions() : AssertionsBase(null, "value") {
        internal void FailHardcoded() => Fail("Hardcoded message.");
    }
}

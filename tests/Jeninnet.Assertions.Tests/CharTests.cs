//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Tests;

[TestClass]
public class CharTests {
    [TestMethod]
    public void Be_SucceedsForEqualCharacter() {
        const char subject = 'a';

        subject.Should().Be('a');
    }

    [TestMethod]
    public void Be_FailsWithExpectedAndActualCharacter() {
        const char subject = 'a';

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => subject.Should().Be('b'));

        Assert.AreEqual("Expected subject to be 'b', but found 'a'.", ex.Message);
    }

    [TestMethod]
    public void Be_FormatsControlCharactersAsEscapes() {
        const char subject = '\n';

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => subject.Should().Be('x'));

        Assert.AreEqual(@"Expected subject to be 'x', but found '\n'.", ex.Message);
    }

    [TestMethod]
    public void NotBe_SucceedsAndFails() {
        const char subject = 'a';

        subject.Should().NotBe('b');

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => subject.Should().NotBe('a'));
        Assert.AreEqual("Expected subject not to be 'a', but found it.", ex.Message);
    }

    [TestMethod]
    public void CategoryVerbs_AcceptMatchingCharacters() {
        'a'.Should().BeLetter();
        '5'.Should().BeDigit();
        'a'.Should().BeLetterOrDigit();
        ' '.Should().BeWhiteSpace();
        ','.Should().BePunctuation();
        '€'.Should().BeSymbol();
        '\u0001'.Should().BeControl();
        '\uD83D'.Should().BeSurrogate();
        'A'.Should().BeUpper();
        'a'.Should().BeLower();
        '~'.Should().BeAscii();
    }

    [TestMethod]
    public void CategoryVerbs_AcceptNonAsciiLetters() {
        '\u0639'.Should().BeLetter();
        '\uFF15'.Should().BeDigit();

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => '\u0639'.Should().BeAscii());
        Assert.AreEqual(@"Expected '\u0639' to be an ASCII character, but found 'ع'.", ex.Message);
    }

    [TestMethod]
    public void CategoryVerbs_RejectOtherCharacters() {
        (char Value, Func<char, Action> Create, string Category)[] cases = [
            ('1', subject => () => subject.Should().BeLetter(), "a letter"),
            ('a', subject => () => subject.Should().BeDigit(), "a digit"),
            ('!', subject => () => subject.Should().BeLetterOrDigit(), "a letter or digit"),
            ('a', subject => () => subject.Should().BeWhiteSpace(), "whitespace"),
            ('a', subject => () => subject.Should().BePunctuation(), "punctuation"),
            ('a', subject => () => subject.Should().BeSymbol(), "a symbol"),
            ('a', subject => () => subject.Should().BeControl(), "a control character"),
            ('a', subject => () => subject.Should().BeSurrogate(), "a surrogate"),
            ('a', subject => () => subject.Should().BeUpper(), "an uppercase letter"),
            ('A', subject => () => subject.Should().BeLower(), "a lowercase letter"),
            ('\u0639', subject => () => subject.Should().BeAscii(), "an ASCII character"),
        ];

        foreach(var (value, create, category) in cases) {
            var ex = Assert.ThrowsExactly<AssertionFailedException>(create(value));
            Assert.AreEqual($"Expected subject to be {category}, but found '{value}'.", ex.Message);
        }
    }

    [TestMethod]
    public void NegatedCategoryVerbs_AcceptOtherCharacters() {
        'a'.Should().NotBeDigit();
        'a'.Should().NotBeWhiteSpace();
        'a'.Should().NotBePunctuation();
        'a'.Should().NotBeSymbol();
        'a'.Should().NotBeControl();
        'a'.Should().NotBeSurrogate();
        'a'.Should().NotBeUpper();
        'A'.Should().NotBeLower();
        '\u0639'.Should().NotBeAscii();
    }

    [TestMethod]
    public void NegatedCategoryVerbs_RejectMatchingCharacters() {
        (char Value, Func<char, Action> Create, string Category)[] cases = [
            ('a', subject => () => subject.Should().NotBeLetter(), "a letter"),
            ('1', subject => () => subject.Should().NotBeDigit(), "a digit"),
            ('a', subject => () => subject.Should().NotBeLetterOrDigit(), "a letter or digit"),
            (' ', subject => () => subject.Should().NotBeWhiteSpace(), "whitespace"),
            (',', subject => () => subject.Should().NotBePunctuation(), "punctuation"),
            ('€', subject => () => subject.Should().NotBeSymbol(), "a symbol"),
            ('\u0001', subject => () => subject.Should().NotBeControl(), "a control character"),
            ('\uD83D', subject => () => subject.Should().NotBeSurrogate(), "a surrogate"),
            ('A', subject => () => subject.Should().NotBeUpper(), "an uppercase letter"),
            ('a', subject => () => subject.Should().NotBeLower(), "a lowercase letter"),
            ('~', subject => () => subject.Should().NotBeAscii(), "an ASCII character"),
        ];

        foreach(var (value, create, category) in cases) {
            var ex = Assert.ThrowsExactly<AssertionFailedException>(create(value));
            Assert.AreEqual($"Expected subject not to be {category}, but found it.", ex.Message);
        }
    }

    [TestMethod]
    public void CategoryVerbs_AppendBecauseReason() {
        const char subject = 'a';

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => subject.Should().BeDigit("because {0}", "the code is numeric"));

        Assert.AreEqual("Expected subject to be a digit, but found 'a' because the code is numeric.", ex.Message);
    }

    [TestMethod]
    public void CategoryVerbs_AreLocalizedInArabic() {
        using var _ = new CultureScope("ar");
        const char subject = 'a';

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => subject.Should().BeDigit());

        Assert.AreEqual("المتوقع أن يكون subject رقمًا، لكن وُجد 'a'.", ex.Message);
    }
}

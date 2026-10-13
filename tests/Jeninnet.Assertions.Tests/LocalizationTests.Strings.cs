//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Tests;

public partial class LocalizationTests {
    [TestMethod]
    public void Arabic_StringEquality() {
        using var _ = new ArabicScope();

        const string subject = "actual";
        var be = Assert.ThrowsExactly<AssertionFailedException>(() => subject.Should().Be("expected"));
        Assert.AreEqual("المتوقع من subject أن يكون 'expected'، لكن وُجد 'actual'.", be.Message);

        const string text = "x";
        var notBe = Assert.ThrowsExactly<AssertionFailedException>(() => text.Should().NotBe("x"));
        Assert.AreEqual("المتوقع من text ألا يكون 'x'، لكن وُجد 'x'.", notBe.Message);

        const string padded = "  hello";
        var equivalent = Assert.ThrowsExactly<AssertionFailedException>(() => padded.Should().BeEquivalentTo("HELLO"));
        Assert.AreEqual("المتوقع من padded أن يكافئ 'HELLO'، لكن وُجد '  hello'.", equivalent.Message);

        const string word = "hello";
        var notEquivalent = Assert.ThrowsExactly<AssertionFailedException>(() => word.Should().NotBeEquivalentTo("HELLO"));
        Assert.AreEqual("المتوقع من word ألا يكافئ 'HELLO'، لكن وُجد 'hello'.", notEquivalent.Message);

        const string letter = "z";
        var oneOf = Assert.ThrowsExactly<AssertionFailedException>(() => letter.Should().BeOneOf("a", "b"));
        Assert.AreEqual("المتوقع من letter أن يكون أحد 'a', 'b'، لكن وُجد 'z'.", oneOf.Message);

        const string present = "x";
        var beNull = Assert.ThrowsExactly<AssertionFailedException>(() => present.Should().BeNull());
        Assert.AreEqual("المتوقع من present أن يكون null، لكنه لم يكن null.", beNull.Message);

        const string? missing = null;
        var notBeNull = Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().NotBeNull());
        Assert.AreEqual("المتوقع من missing ألا يكون null، لكن وُجد null.", notBeNull.Message);
    }

    [TestMethod]
    public void Arabic_StringEmptinessAndLength() {
        using var _ = new ArabicScope();

        const string subject = "x";
        var beEmpty = Assert.ThrowsExactly<AssertionFailedException>(() => subject.Should().BeEmpty());
        Assert.AreEqual("المتوقع من subject أن يكون فارغاً، لكن وُجد 'x'.", beEmpty.Message);

        const string? missing = null;
        var beEmptyNull = Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().BeEmpty());
        Assert.AreEqual("المتوقع من missing أن يكون فارغاً، لكنه كان null.", beEmptyNull.Message);

        const string empty = "";
        var notBeEmpty = Assert.ThrowsExactly<AssertionFailedException>(() => empty.Should().NotBeEmpty());
        Assert.AreEqual("المتوقع من empty ألا يكون فارغاً، لكنه كان فارغاً.", notBeEmpty.Message);

        var blank = Assert.ThrowsExactly<AssertionFailedException>(() => subject.Should().BeNullOrWhiteSpace());
        Assert.AreEqual("المتوقع من subject أن يكون null أو مسافة بيضاء، لكن وُجد 'x'.", blank.Message);

        const string spaces = "   ";
        var notBlank = Assert.ThrowsExactly<AssertionFailedException>(() => spaces.Should().NotBeNullOrWhiteSpace());
        Assert.AreEqual("المتوقع من spaces ألا يكون null أو مسافة بيضاء، لكن وُجد '   '.", notBlank.Message);

        const string greeting = "hi";
        var length = Assert.ThrowsExactly<AssertionFailedException>(() => greeting.Should().HaveLength(3));
        Assert.AreEqual("المتوقع من greeting أن يكون طوله 3، لكن وُجد 'hi' بطول 2.", length.Message);

        var lengthNull = Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().HaveLength(3));
        Assert.AreEqual("المتوقع من missing أن يكون طوله 3، لكنه كان null.", lengthNull.Message);

        var notLength = Assert.ThrowsExactly<AssertionFailedException>(() => greeting.Should().NotHaveLength(2));
        Assert.AreEqual("المتوقع من greeting ألا يكون طوله 2، لكن وُجد 'hi' بطول 2.", notLength.Message);
    }

    [TestMethod]
    public void Arabic_StringCasing() {
        using var _ = new ArabicScope();

        const string mixed = "aBC";
        var upper = Assert.ThrowsExactly<AssertionFailedException>(() => mixed.Should().BeUpperCased());
        Assert.AreEqual("المتوقع من mixed أن يكون بأحرف كبيرة، لكن وُجد 'aBC'.", upper.Message);

        const string shout = "ABC";
        var notUpper = Assert.ThrowsExactly<AssertionFailedException>(() => shout.Should().NotBeUpperCased());
        Assert.AreEqual("المتوقع من shout ألا يكون بأحرف كبيرة، لكن وُجد 'ABC'.", notUpper.Message);

        var lower = Assert.ThrowsExactly<AssertionFailedException>(() => mixed.Should().BeLowerCased());
        Assert.AreEqual("المتوقع من mixed أن يكون بأحرف صغيرة، لكن وُجد 'aBC'.", lower.Message);

        const string quiet = "abc";
        var notLower = Assert.ThrowsExactly<AssertionFailedException>(() => quiet.Should().NotBeLowerCased());
        Assert.AreEqual("المتوقع من quiet ألا يكون بأحرف صغيرة، لكن وُجد 'abc'.", notLower.Message);
    }

    [TestMethod]
    public void Arabic_StringContain() {
        using var _ = new ArabicScope();

        const string? missing = null;
        var containNull = Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().Contain("x"));
        Assert.AreEqual("المتوقع من missing أن يحتوي 'x'، لكنه كان null.", containNull.Message);

        const string subject = "hello";
        var contain = Assert.ThrowsExactly<AssertionFailedException>(() => subject.Should().Contain("bye"));
        Assert.AreEqual("المتوقع من subject أن يحتوي 'bye'، لكن وُجد 'hello'.", contain.Message);

        var containTimesNull = Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().Contain("a", Exactly.Twice()));
        Assert.AreEqual("المتوقع من missing أن يحتوي 'a' exactly twice، لكنه كان null.", containTimesNull.Message);

        const string triple = "aaa";
        var containTimes = Assert.ThrowsExactly<AssertionFailedException>(() => triple.Should().Contain("a", Exactly.Twice()));
        Assert.AreEqual("المتوقع من triple أن يحتوي 'a' exactly twice، لكن وُجد 3 occurrences.", containTimes.Message);

        var notContain = Assert.ThrowsExactly<AssertionFailedException>(() => subject.Should().NotContain("ell"));
        Assert.AreEqual("المتوقع من subject ألا يحتوي 'ell'، لكن وُجد 'hello'.", notContain.Message);
    }

    [TestMethod]
    public void Arabic_StringContainSets() {
        using var _ = new ArabicScope();

        const string? missing = null;
        var allNull = Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().ContainAll("x"));
        Assert.AreEqual("المتوقع من missing أن يحتوي كل السلاسل المعطاة، لكنه كان null.", allNull.Message);

        const string subject = "hello";
        var allMissing = Assert.ThrowsExactly<AssertionFailedException>(() => subject.Should().ContainAll("bye"));
        Assert.AreEqual("المتوقع من subject أن يحتوي 'bye'، لكنه لم يُعثر عليه.", allMissing.Message);

        const string sentence = "hello world";
        var notAll = Assert.ThrowsExactly<AssertionFailedException>(() => sentence.Should().NotContainAll("hello", "world"));
        Assert.AreEqual("المتوقع من sentence ألا يحتوي كل السلاسل المعطاة، لكنها كلها وُجدت.", notAll.Message);

        var any = Assert.ThrowsExactly<AssertionFailedException>(() => subject.Should().ContainAny("x", "y"));
        Assert.AreEqual("المتوقع من subject أن يحتوي واحداً على الأقل من 'x', 'y'، but none was found.", any.Message);

        var notAny = Assert.ThrowsExactly<AssertionFailedException>(() => subject.Should().NotContainAny("x", "ell"));
        Assert.AreEqual("المتوقع من subject ألا يحتوي أياً من 'x', 'ell'، لكن وُجد 'ell'.", notAny.Message);
    }

    [TestMethod]
    public void Arabic_StringContainEquivalent() {
        using var _ = new ArabicScope();

        const string subject = "Hello World";
        var equivalent = Assert.ThrowsExactly<AssertionFailedException>(() => subject.Should().ContainEquivalentOf("BYE"));
        Assert.AreEqual("المتوقع من subject أن يحتوي ما يكافئ 'BYE'، لكن وُجد 'Hello World'.", equivalent.Message);

        const string? missing = null;
        var equivalentNull = Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().ContainEquivalentOf("BYE"));
        Assert.AreEqual("المتوقع من missing أن يحتوي ما يكافئ 'BYE'، لكنه كان null.", equivalentNull.Message);

        const string triple = "aaa";
        var equivalentTimes = Assert.ThrowsExactly<AssertionFailedException>(() => triple.Should().ContainEquivalentOf("a", Exactly.Twice()));
        Assert.AreEqual("المتوقع من triple أن يحتوي ما يكافئ 'a' exactly twice، لكن وُجد 3 occurrences.", equivalentTimes.Message);

        var equivalentTimesNull = Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().ContainEquivalentOf("a", Exactly.Twice()));
        Assert.AreEqual("المتوقع من missing أن يحتوي ما يكافئ 'a' exactly twice، لكنه كان null.", equivalentTimesNull.Message);

        const string greeting = "Hello";
        var notEquivalent = Assert.ThrowsExactly<AssertionFailedException>(() => greeting.Should().NotContainEquivalentOf("ELL"));
        Assert.AreEqual("المتوقع من greeting ألا يحتوي ما يكافئ 'ELL'، لكن وُجد 'Hello'.", notEquivalent.Message);
    }

    [TestMethod]
    public void Arabic_StringAffixes() {
        using var _ = new ArabicScope();

        const string subject = "hello";
        var start = Assert.ThrowsExactly<AssertionFailedException>(() => subject.Should().StartWith("bye"));
        Assert.AreEqual("المتوقع من subject أن يبدأ بـ 'bye'، لكن وُجد 'hello'.", start.Message);

        const string? missing = null;
        var startNull = Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().StartWith("he"));
        Assert.AreEqual("المتوقع من missing أن يبدأ بـ 'he'، لكنه كان null.", startNull.Message);

        var notStart = Assert.ThrowsExactly<AssertionFailedException>(() => subject.Should().NotStartWith("he"));
        Assert.AreEqual("المتوقع من subject ألا يبدأ بـ 'he'، لكن وُجد 'hello'.", notStart.Message);

        const string greeting = "Hello";
        var startEquivalent = Assert.ThrowsExactly<AssertionFailedException>(() => greeting.Should().StartWithEquivalentOf("bye"));
        Assert.AreEqual("المتوقع من greeting أن يبدأ بما يكافئ 'bye'، لكن وُجد 'Hello'.", startEquivalent.Message);

        var startEquivalentNull = Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().StartWithEquivalentOf("he"));
        Assert.AreEqual("المتوقع من missing أن يبدأ بما يكافئ 'he'، لكنه كان null.", startEquivalentNull.Message);

        var notStartEquivalent = Assert.ThrowsExactly<AssertionFailedException>(() => greeting.Should().NotStartWithEquivalentOf("HE"));
        Assert.AreEqual("المتوقع من greeting ألا يبدأ بما يكافئ 'HE'، لكن وُجد 'Hello'.", notStartEquivalent.Message);

        const string file = "file.cs";
        var end = Assert.ThrowsExactly<AssertionFailedException>(() => file.Should().EndWith(".txt"));
        Assert.AreEqual("المتوقع من file أن ينتهي بـ '.txt'، لكن وُجد 'file.cs'.", end.Message);

        var endNull = Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().EndWith(".txt"));
        Assert.AreEqual("المتوقع من missing أن ينتهي بـ '.txt'، لكنه كان null.", endNull.Message);

        var notEnd = Assert.ThrowsExactly<AssertionFailedException>(() => file.Should().NotEndWith(".cs"));
        Assert.AreEqual("المتوقع من file ألا ينتهي بـ '.cs'، لكن وُجد 'file.cs'.", notEnd.Message);

        const string upper = "file.CS";
        var endEquivalent = Assert.ThrowsExactly<AssertionFailedException>(() => upper.Should().EndWithEquivalentOf(".txt"));
        Assert.AreEqual("المتوقع من upper أن ينتهي بما يكافئ '.txt'، لكن وُجد 'file.CS'.", endEquivalent.Message);

        var endEquivalentNull = Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().EndWithEquivalentOf(".txt"));
        Assert.AreEqual("المتوقع من missing أن ينتهي بما يكافئ '.txt'، لكنه كان null.", endEquivalentNull.Message);

        var notEndEquivalent = Assert.ThrowsExactly<AssertionFailedException>(() => upper.Should().NotEndWithEquivalentOf(".CS"));
        Assert.AreEqual("المتوقع من upper ألا ينتهي بما يكافئ '.CS'، لكن وُجد 'file.CS'.", notEndEquivalent.Message);
    }

    [TestMethod]
    public void Arabic_StringMatching() {
        using var _ = new ArabicScope();

        const string subject = "hello";
        var match = Assert.ThrowsExactly<AssertionFailedException>(() => subject.Should().Match("h?llo!"));
        Assert.AreEqual("المتوقع من subject أن يطابق 'h?llo!'، but found 'hello'.", match.Message);

        const string? missing = null;
        var matchNull = Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().Match("H*"));
        Assert.AreEqual("المتوقع من missing أن يطابق 'H*'، but it was null.", matchNull.Message);

        var notMatch = Assert.ThrowsExactly<AssertionFailedException>(() => subject.Should().NotMatch("h*"));
        Assert.AreEqual("المتوقع من subject ألا يطابق 'h*'، لكن وُجد 'hello'.", notMatch.Message);

        var matchEquivalent = Assert.ThrowsExactly<AssertionFailedException>(() => subject.Should().MatchEquivalentOf("B*"));
        Assert.AreEqual("المتوقع من subject أن يطابق ما يكافئ 'B*'، but found 'hello'.", matchEquivalent.Message);

        var notMatchEquivalent = Assert.ThrowsExactly<AssertionFailedException>(() => subject.Should().NotMatchEquivalentOf("H*"));
        Assert.AreEqual("المتوقع من subject ألا يطابق ما يكافئ 'H*'، لكن وُجد 'hello'.", notMatchEquivalent.Message);

        var regexNull = Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().MatchRegex(@"\d+"));
        Assert.AreEqual(@"المتوقع من missing أن يطابق التعبير النمطي '\d+'، لكنه كان null.", regexNull.Message);

        var regex = Assert.ThrowsExactly<AssertionFailedException>(() => subject.Should().MatchRegex(@"\d+"));
        Assert.AreEqual(@"المتوقع من subject أن يطابق التعبير النمطي '\d+'، لكن وُجد 'hello'.", regex.Message);

        const string order = "order 123 shipped";
        var regexTimes = Assert.ThrowsExactly<AssertionFailedException>(() => order.Should().MatchRegex(@"\d", Exactly.Twice()));
        Assert.AreEqual(@"المتوقع من order أن يطابق التعبير النمطي '\d' exactly twice، لكن وُجد 3 matches.", regexTimes.Message);

        var regexTimesNull = Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().MatchRegex(@"\d", Exactly.Twice()));
        Assert.AreEqual(@"المتوقع من missing أن يطابق التعبير النمطي '\d' exactly twice، لكنه كان null.", regexTimesNull.Message);

        const string code = "abc123";
        var notRegex = Assert.ThrowsExactly<AssertionFailedException>(() => code.Should().NotMatchRegex(@"\d+"));
        Assert.AreEqual(@"المتوقع من code ألا يطابق التعبير النمطي '\d+'، لكن وُجد 'abc123'.", notRegex.Message);
    }

    [TestMethod]
    public void Arabic_StringCollectionVerbs() {
        using var _ = new ArabicScope();

        var words = new List<string> { "a", "b" };
        var allBe = Assert.ThrowsExactly<AssertionFailedException>(() => words.Should().AllBe("a"));
        Assert.AreEqual("المتوقع من words أن تكون كل عناصره 'a'، لكن وُجد 'b'.", allBe.Message);

        IEnumerable<string>? missing = null;
        var allBeNull = Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().AllBe("a"));
        Assert.AreEqual("المتوقع من missing أن تكون كل عناصره 'a'، لكن المجموعة null.", allBeNull.Message);

        var contain = Assert.ThrowsExactly<AssertionFailedException>(() => words.Should().ContainMatch("z*"));
        Assert.AreEqual("المتوقع من words أن يحتوي عنصرًا يطابق 'z*'، لكن لم يُعثر على أي.", contain.Message);

        var containNull = Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().ContainMatch("z*"));
        Assert.AreEqual("المتوقع من missing أن يحتوي عنصرًا يطابق 'z*'، لكن المجموعة null.", containNull.Message);

        var notContain = Assert.ThrowsExactly<AssertionFailedException>(() => words.Should().NotContainMatch("a"));
        Assert.AreEqual("المتوقع من words ألا يحتوي عنصرًا يطابق 'a'، لكن وُجد 'a'.", notContain.Message);

        var notContainNull = Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().NotContainMatch("z*"));
        Assert.AreEqual("المتوقع من missing ألا يحتوي عنصرًا يطابق 'z*'، لكن المجموعة null.", notContainNull.Message);
    }
}

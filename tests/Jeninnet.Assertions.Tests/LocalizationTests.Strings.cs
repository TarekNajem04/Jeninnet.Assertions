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
        var be = Throws<AssertionFailedException>(() => subject.Should().Be("expected"));
        be.Message.Should().Be("المتوقع من subject أن يكون 'expected'، لكن وُجد 'actual'.");

        const string text = "x";
        var notBe = Throws<AssertionFailedException>(() => text.Should().NotBe("x"));
        notBe.Message.Should().Be("المتوقع من text ألا يكون 'x'، لكن وُجد 'x'.");

        const string padded = "  hello";
        var equivalent = Throws<AssertionFailedException>(() => padded.Should().BeEquivalentTo("HELLO"));
        equivalent.Message.Should().Be("المتوقع من padded أن يكافئ 'HELLO'، لكن وُجد '  hello'.");

        const string word = "hello";
        var notEquivalent = Throws<AssertionFailedException>(() => word.Should().NotBeEquivalentTo("HELLO"));
        notEquivalent.Message.Should().Be("المتوقع من word ألا يكافئ 'HELLO'، لكن وُجد 'hello'.");

        const string letter = "z";
        var oneOf = Throws<AssertionFailedException>(() => letter.Should().BeOneOf("a", "b"));
        oneOf.Message.Should().Be("المتوقع من letter أن يكون أحد 'a', 'b'، لكن وُجد 'z'.");

        const string present = "x";
        var beNull = Throws<AssertionFailedException>(() => present.Should().BeNull());
        beNull.Message.Should().Be("المتوقع من present أن يكون null، لكنه لم يكن null.");

        const string? missing = null;
        var notBeNull = Throws<AssertionFailedException>(() => missing.Should().NotBeNull());
        notBeNull.Message.Should().Be("المتوقع من missing ألا يكون null، لكن وُجد null.");
    }

    [TestMethod]
    public void Arabic_StringEmptinessAndLength() {
        using var _ = new ArabicScope();

        const string subject = "x";
        var beEmpty = Throws<AssertionFailedException>(() => subject.Should().BeEmpty());
        beEmpty.Message.Should().Be("المتوقع من subject أن يكون فارغاً، لكن وُجد 'x'.");

        const string? missing = null;
        var beEmptyNull = Throws<AssertionFailedException>(() => missing.Should().BeEmpty());
        beEmptyNull.Message.Should().Be("المتوقع من missing أن يكون فارغاً، لكنه كان null.");

        const string empty = "";
        var notBeEmpty = Throws<AssertionFailedException>(() => empty.Should().NotBeEmpty());
        notBeEmpty.Message.Should().Be("المتوقع من empty ألا يكون فارغاً، لكنه كان فارغاً.");

        var blank = Throws<AssertionFailedException>(() => subject.Should().BeNullOrWhiteSpace());
        blank.Message.Should().Be("المتوقع من subject أن يكون null أو مسافة بيضاء، لكن وُجد 'x'.");

        const string spaces = "   ";
        var notBlank = Throws<AssertionFailedException>(() => spaces.Should().NotBeNullOrWhiteSpace());
        notBlank.Message.Should().Be("المتوقع من spaces ألا يكون null أو مسافة بيضاء، لكن وُجد '   '.");

        const string greeting = "hi";
        var length = Throws<AssertionFailedException>(() => greeting.Should().HaveLength(3));
        length.Message.Should().Be("المتوقع من greeting أن يكون طوله 3، لكن وُجد 'hi' بطول 2.");

        var lengthNull = Throws<AssertionFailedException>(() => missing.Should().HaveLength(3));
        lengthNull.Message.Should().Be("المتوقع من missing أن يكون طوله 3، لكنه كان null.");

        var notLength = Throws<AssertionFailedException>(() => greeting.Should().NotHaveLength(2));
        notLength.Message.Should().Be("المتوقع من greeting ألا يكون طوله 2، لكن وُجد 'hi' بطول 2.");
    }

    [TestMethod]
    public void Arabic_StringCasing() {
        using var _ = new ArabicScope();

        const string mixed = "aBC";
        var upper = Throws<AssertionFailedException>(() => mixed.Should().BeUpperCased());
        upper.Message.Should().Be("المتوقع من mixed أن يكون بأحرف كبيرة، لكن وُجد 'aBC'.");

        const string shout = "ABC";
        var notUpper = Throws<AssertionFailedException>(() => shout.Should().NotBeUpperCased());
        notUpper.Message.Should().Be("المتوقع من shout ألا يكون بأحرف كبيرة، لكن وُجد 'ABC'.");

        var lower = Throws<AssertionFailedException>(() => mixed.Should().BeLowerCased());
        lower.Message.Should().Be("المتوقع من mixed أن يكون بأحرف صغيرة، لكن وُجد 'aBC'.");

        const string quiet = "abc";
        var notLower = Throws<AssertionFailedException>(() => quiet.Should().NotBeLowerCased());
        notLower.Message.Should().Be("المتوقع من quiet ألا يكون بأحرف صغيرة، لكن وُجد 'abc'.");
    }

    [TestMethod]
    public void Arabic_StringContain() {
        using var _ = new ArabicScope();

        const string? missing = null;
        var containNull = Throws<AssertionFailedException>(() => missing.Should().Contain("x"));
        containNull.Message.Should().Be("المتوقع من missing أن يحتوي 'x'، لكنه كان null.");

        const string subject = "hello";
        var contain = Throws<AssertionFailedException>(() => subject.Should().Contain("bye"));
        contain.Message.Should().Be("المتوقع من subject أن يحتوي 'bye'، لكن وُجد 'hello'.");

        var containTimesNull = Throws<AssertionFailedException>(() => missing.Should().Contain("a", Exactly.Twice()));
        containTimesNull.Message.Should().Be("المتوقع من missing أن يحتوي 'a' exactly twice، لكنه كان null.");

        const string triple = "aaa";
        var containTimes = Throws<AssertionFailedException>(() => triple.Should().Contain("a", Exactly.Twice()));
        containTimes.Message.Should().Be("المتوقع من triple أن يحتوي 'a' exactly twice، لكن وُجد 3 occurrences.");

        var notContain = Throws<AssertionFailedException>(() => subject.Should().NotContain("ell"));
        notContain.Message.Should().Be("المتوقع من subject ألا يحتوي 'ell'، لكن وُجد 'hello'.");
    }

    [TestMethod]
    public void Arabic_StringContainSets() {
        using var _ = new ArabicScope();

        const string? missing = null;
        var allNull = Throws<AssertionFailedException>(() => missing.Should().ContainAll("x"));
        allNull.Message.Should().Be("المتوقع من missing أن يحتوي كل السلاسل المعطاة، لكنه كان null.");

        const string subject = "hello";
        var allMissing = Throws<AssertionFailedException>(() => subject.Should().ContainAll("bye"));
        allMissing.Message.Should().Be("المتوقع من subject أن يحتوي 'bye'، لكنه لم يُعثر عليه.");

        const string sentence = "hello world";
        var notAll = Throws<AssertionFailedException>(() => sentence.Should().NotContainAll("hello", "world"));
        notAll.Message.Should().Be("المتوقع من sentence ألا يحتوي كل السلاسل المعطاة، لكنها كلها وُجدت.");

        var any = Throws<AssertionFailedException>(() => subject.Should().ContainAny("x", "y"));
        any.Message.Should().Be("المتوقع من subject أن يحتوي واحداً على الأقل من 'x', 'y'، but none was found.");

        var notAny = Throws<AssertionFailedException>(() => subject.Should().NotContainAny("x", "ell"));
        notAny.Message.Should().Be("المتوقع من subject ألا يحتوي أياً من 'x', 'ell'، لكن وُجد 'ell'.");
    }

    [TestMethod]
    public void Arabic_StringContainEquivalent() {
        using var _ = new ArabicScope();

        const string subject = "Hello World";
        var equivalent = Throws<AssertionFailedException>(() => subject.Should().ContainEquivalentOf("BYE"));
        equivalent.Message.Should().Be("المتوقع من subject أن يحتوي ما يكافئ 'BYE'، لكن وُجد 'Hello World'.");

        const string? missing = null;
        var equivalentNull = Throws<AssertionFailedException>(() => missing.Should().ContainEquivalentOf("BYE"));
        equivalentNull.Message.Should().Be("المتوقع من missing أن يحتوي ما يكافئ 'BYE'، لكنه كان null.");

        const string triple = "aaa";
        var equivalentTimes = Throws<AssertionFailedException>(() => triple.Should().ContainEquivalentOf("a", Exactly.Twice()));
        equivalentTimes.Message.Should().Be("المتوقع من triple أن يحتوي ما يكافئ 'a' exactly twice، لكن وُجد 3 occurrences.");

        var equivalentTimesNull = Throws<AssertionFailedException>(() => missing.Should().ContainEquivalentOf("a", Exactly.Twice()));
        equivalentTimesNull.Message.Should().Be("المتوقع من missing أن يحتوي ما يكافئ 'a' exactly twice، لكنه كان null.");

        const string greeting = "Hello";
        var notEquivalent = Throws<AssertionFailedException>(() => greeting.Should().NotContainEquivalentOf("ELL"));
        notEquivalent.Message.Should().Be("المتوقع من greeting ألا يحتوي ما يكافئ 'ELL'، لكن وُجد 'Hello'.");
    }

    [TestMethod]
    public void Arabic_StringAffixes() {
        using var _ = new ArabicScope();

        const string subject = "hello";
        var start = Throws<AssertionFailedException>(() => subject.Should().StartWith("bye"));
        start.Message.Should().Be("المتوقع من subject أن يبدأ بـ 'bye'، لكن وُجد 'hello'.");

        const string? missing = null;
        var startNull = Throws<AssertionFailedException>(() => missing.Should().StartWith("he"));
        startNull.Message.Should().Be("المتوقع من missing أن يبدأ بـ 'he'، لكنه كان null.");

        var notStart = Throws<AssertionFailedException>(() => subject.Should().NotStartWith("he"));
        notStart.Message.Should().Be("المتوقع من subject ألا يبدأ بـ 'he'، لكن وُجد 'hello'.");

        const string greeting = "Hello";
        var startEquivalent = Throws<AssertionFailedException>(() => greeting.Should().StartWithEquivalentOf("bye"));
        startEquivalent.Message.Should().Be("المتوقع من greeting أن يبدأ بما يكافئ 'bye'، لكن وُجد 'Hello'.");

        var startEquivalentNull = Throws<AssertionFailedException>(() => missing.Should().StartWithEquivalentOf("he"));
        startEquivalentNull.Message.Should().Be("المتوقع من missing أن يبدأ بما يكافئ 'he'، لكنه كان null.");

        var notStartEquivalent = Throws<AssertionFailedException>(() => greeting.Should().NotStartWithEquivalentOf("HE"));
        notStartEquivalent.Message.Should().Be("المتوقع من greeting ألا يبدأ بما يكافئ 'HE'، لكن وُجد 'Hello'.");

        const string file = "file.cs";
        var end = Throws<AssertionFailedException>(() => file.Should().EndWith(".txt"));
        end.Message.Should().Be("المتوقع من file أن ينتهي بـ '.txt'، لكن وُجد 'file.cs'.");

        var endNull = Throws<AssertionFailedException>(() => missing.Should().EndWith(".txt"));
        endNull.Message.Should().Be("المتوقع من missing أن ينتهي بـ '.txt'، لكنه كان null.");

        var notEnd = Throws<AssertionFailedException>(() => file.Should().NotEndWith(".cs"));
        notEnd.Message.Should().Be("المتوقع من file ألا ينتهي بـ '.cs'، لكن وُجد 'file.cs'.");

        const string upper = "file.CS";
        var endEquivalent = Throws<AssertionFailedException>(() => upper.Should().EndWithEquivalentOf(".txt"));
        endEquivalent.Message.Should().Be("المتوقع من upper أن ينتهي بما يكافئ '.txt'، لكن وُجد 'file.CS'.");

        var endEquivalentNull = Throws<AssertionFailedException>(() => missing.Should().EndWithEquivalentOf(".txt"));
        endEquivalentNull.Message.Should().Be("المتوقع من missing أن ينتهي بما يكافئ '.txt'، لكنه كان null.");

        var notEndEquivalent = Throws<AssertionFailedException>(() => upper.Should().NotEndWithEquivalentOf(".CS"));
        notEndEquivalent.Message.Should().Be("المتوقع من upper ألا ينتهي بما يكافئ '.CS'، لكن وُجد 'file.CS'.");
    }

    [TestMethod]
    public void Arabic_StringMatching() {
        using var _ = new ArabicScope();

        const string subject = "hello";
        var match = Throws<AssertionFailedException>(() => subject.Should().Match("h?llo!"));
        match.Message.Should().Be("المتوقع من subject أن يطابق 'h?llo!'، but found 'hello'.");

        const string? missing = null;
        var matchNull = Throws<AssertionFailedException>(() => missing.Should().Match("H*"));
        matchNull.Message.Should().Be("المتوقع من missing أن يطابق 'H*'، but it was null.");

        var notMatch = Throws<AssertionFailedException>(() => subject.Should().NotMatch("h*"));
        notMatch.Message.Should().Be("المتوقع من subject ألا يطابق 'h*'، لكن وُجد 'hello'.");

        var matchEquivalent = Throws<AssertionFailedException>(() => subject.Should().MatchEquivalentOf("B*"));
        matchEquivalent.Message.Should().Be("المتوقع من subject أن يطابق ما يكافئ 'B*'، but found 'hello'.");

        var notMatchEquivalent = Throws<AssertionFailedException>(() => subject.Should().NotMatchEquivalentOf("H*"));
        notMatchEquivalent.Message.Should().Be("المتوقع من subject ألا يطابق ما يكافئ 'H*'، لكن وُجد 'hello'.");

        var regexNull = Throws<AssertionFailedException>(() => missing.Should().MatchRegex(@"\d+"));
        regexNull.Message.Should().Be(@"المتوقع من missing أن يطابق التعبير النمطي '\d+'، لكنه كان null.");

        var regex = Throws<AssertionFailedException>(() => subject.Should().MatchRegex(@"\d+"));
        regex.Message.Should().Be(@"المتوقع من subject أن يطابق التعبير النمطي '\d+'، لكن وُجد 'hello'.");

        const string order = "order 123 shipped";
        var regexTimes = Throws<AssertionFailedException>(() => order.Should().MatchRegex(@"\d", Exactly.Twice()));
        regexTimes.Message.Should().Be(@"المتوقع من order أن يطابق التعبير النمطي '\d' exactly twice، لكن وُجد 3 matches.");

        var regexTimesNull = Throws<AssertionFailedException>(() => missing.Should().MatchRegex(@"\d", Exactly.Twice()));
        regexTimesNull.Message.Should().Be(@"المتوقع من missing أن يطابق التعبير النمطي '\d' exactly twice، لكنه كان null.");

        const string code = "abc123";
        var notRegex = Throws<AssertionFailedException>(() => code.Should().NotMatchRegex(@"\d+"));
        notRegex.Message.Should().Be(@"المتوقع من code ألا يطابق التعبير النمطي '\d+'، لكن وُجد 'abc123'.");
    }

    [TestMethod]
    public void Arabic_StringCollectionVerbs() {
        using var _ = new ArabicScope();

        var words = new List<string> { "a", "b" };
        var allBe = Throws<AssertionFailedException>(() => words.Should().AllBe("a"));
        allBe.Message.Should().Be("المتوقع من words أن تكون كل عناصره 'a'، لكن وُجد 'b'.");

        IEnumerable<string>? missing = null;
        var allBeNull = Throws<AssertionFailedException>(() => missing.Should().AllBe("a"));
        allBeNull.Message.Should().Be("المتوقع من missing أن تكون كل عناصره 'a'، لكن المجموعة null.");

        var contain = Throws<AssertionFailedException>(() => words.Should().ContainMatch("z*"));
        contain.Message.Should().Be("المتوقع من words أن يحتوي عنصرًا يطابق 'z*'، لكن لم يُعثر على أي.");

        var containNull = Throws<AssertionFailedException>(() => missing.Should().ContainMatch("z*"));
        containNull.Message.Should().Be("المتوقع من missing أن يحتوي عنصرًا يطابق 'z*'، لكن المجموعة null.");

        var notContain = Throws<AssertionFailedException>(() => words.Should().NotContainMatch("a"));
        notContain.Message.Should().Be("المتوقع من words ألا يحتوي عنصرًا يطابق 'a'، لكن وُجد 'a'.");

        var notContainNull = Throws<AssertionFailedException>(() => missing.Should().NotContainMatch("z*"));
        notContainNull.Message.Should().Be("المتوقع من missing ألا يحتوي عنصرًا يطابق 'z*'، لكن المجموعة null.");
    }
}

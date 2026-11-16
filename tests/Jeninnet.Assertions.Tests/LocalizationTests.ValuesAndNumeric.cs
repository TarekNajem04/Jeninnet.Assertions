//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Tests;

public partial class LocalizationTests {
    [TestMethod]
    public void Arabic_BooleanMessages() {
        using var _ = new ArabicScope();

        const bool flag = false;
        var beTrue = Throws<AssertionFailedException>(() => flag.Should().BeTrue());
        beTrue.Message.Should().Be("المتوقع من flag أن يكون true، لكن وُجد false.");

        const bool other = true;
        var beFalse = Throws<AssertionFailedException>(() => other.Should().BeFalse());
        beFalse.Message.Should().Be("المتوقع من other أن يكون false، لكن وُجد true.");

        var notBe = Throws<AssertionFailedException>(() => flag.Should().NotBe(false));
        notBe.Message.Should().Be("المتوقع من flag ألا يكون false، لكن وُجد false.");

        var imply = Throws<AssertionFailedException>(() => other.Should().Imply(false));
        imply.Message.Should().Be("المتوقع من other أن يستلزم false، لكن true لا يستلزم false.");
    }

    [TestMethod]
    public void Arabic_NullableBooleanMessages() {
        using var _ = new ArabicScope();

        bool? missing = null;
        var be = Throws<AssertionFailedException>(() => missing.Should().Be(true));
        be.Message.Should().Be("المتوقع من missing أن يكون true، لكن وُجد <null>.");

        var beTrue = Throws<AssertionFailedException>(() => missing.Should().BeTrue());
        beTrue.Message.Should().Be("المتوقع من missing أن يكون true، لكن وُجد <null>.");

        bool? present = true;
        var beFalse = Throws<AssertionFailedException>(() => present.Should().BeFalse());
        beFalse.Message.Should().Be("المتوقع من present أن يكون false، لكن وُجد true.");

        var beNull = Throws<AssertionFailedException>(() => present.Should().BeNull());
        beNull.Message.Should().Be("المتوقع من present أن يكون null، لكن وُجد true.");

        var haveValue = Throws<AssertionFailedException>(() => missing.Should().HaveValue());
        haveValue.Message.Should().Be("المتوقع من missing ألا يكون null، لكن وُجد null.");
    }

    [TestMethod]
    public void Arabic_NumericComparisons() {
        using var _ = new ArabicScope();

        int? count = 5;
        var be = Throws<AssertionFailedException>(() => count.Should().Be(6));
        be.Message.Should().Be("المتوقع من count أن يكون 6، لكن وُجد 5.");

        var notBe = Throws<AssertionFailedException>(() => count.Should().NotBe(5));
        notBe.Message.Should().Be("المتوقع من count ألا يكون 5، لكن وُجد 5.");

        var greater = Throws<AssertionFailedException>(() => count.Should().BeGreaterThan(10));
        greater.Message.Should().Be("المتوقع من count أن يكون أكبر من 10، لكن وُجد 5.");

        var greaterOrEqual = Throws<AssertionFailedException>(() => count.Should().BeGreaterThanOrEqualTo(6));
        greaterOrEqual.Message.Should().Be("المتوقع من count أن يكون أكبر من أو يساوي 6، لكن وُجد 5.");

        var less = Throws<AssertionFailedException>(() => count.Should().BeLessThan(5));
        less.Message.Should().Be("المتوقع من count أن يكون أصغر من 5، لكن وُجد 5.");

        var lessOrEqual = Throws<AssertionFailedException>(() => count.Should().BeLessThanOrEqualTo(4));
        lessOrEqual.Message.Should().Be("المتوقع من count أن يكون أصغر من أو يساوي 4، لكن وُجد 5.");
    }

    [TestMethod]
    public void Arabic_NumericSignAndRange() {
        using var _ = new ArabicScope();

        const int number = -2;
        var positive = Throws<AssertionFailedException>(() => number.Should().BePositive());
        positive.Message.Should().Be("المتوقع من number أن يكون موجباً، لكن وُجد -2.");

        int? zero = 0;
        var negative = Throws<AssertionFailedException>(() => zero.Should().BeNegative());
        negative.Message.Should().Be("المتوقع من zero أن يكون سالباً، لكن وُجد 0.");

        int? count = 5;
        var between = Throws<AssertionFailedException>(() => count.Should().BeInRange(6, 10));
        between.Message.Should().Be("المتوقع من count أن يكون بين 6 و10، لكن وُجد 5.");

        var notBetween = Throws<AssertionFailedException>(() => count.Should().NotBeInRange(1, 10));
        notBetween.Message.Should().Be("المتوقع من count ألا يكون بين 1 و10، لكن وُجد 5.");
    }

    [TestMethod]
    public void Arabic_NumericApproximateAndNaN() {
        using var _ = new ArabicScope();

        const double ratio = 5.1;
        var approximate = Throws<AssertionFailedException>(() => ratio.Should().BeApproximately(5, 0.05));
        approximate.Message.Should().Be("المتوقع من ratio أن يقارب 5 (±0.05)، لكن وُجد 5.1.");

        const double value = 3.5;
        var notApproximate = Throws<AssertionFailedException>(() => value.Should().NotBeApproximately(3.4, 0.5));
        notApproximate.Message.Should().Be("المتوقع من value ألا يقارب 3.4 (±0.5)، لكن وُجد 3.5.");

        const double measurement = 1;
        var nanEx = Throws<AssertionFailedException>(() => measurement.Should().BeNaN());
        nanEx.Message.Should().Be("المتوقع من measurement أن يكون NaN، لكن وُجد 1.");

        const double nan = double.NaN;
        var notNan = Throws<AssertionFailedException>(() => nan.Should().NotBeNaN());
        notNan.Message.Should().Be("المتوقع من nan ألا يكون NaN، لكن وُجد NaN.");
    }

    [TestMethod]
    public void Arabic_NumericSetAndNull() {
        using var _ = new ArabicScope();

        int? count = 5;
        var oneOf = Throws<AssertionFailedException>(() => count.Should().BeOneOf(3, 6));
        oneOf.Message.Should().Be("المتوقع من count أن يكون أحد 3, 6، لكن وُجد 5.");

        Expression<Func<int, bool>> isEven = x => x % 2 == 0;
        var match = Throws<AssertionFailedException>(() => count.Should().Match(isEven));
        match.Message.Should().Be("المتوقع من count أن يطابق 'x => ((x % 2) == 0)'، لكنه لم يطابق.");

        int? present = 5;
        var beNull = Throws<AssertionFailedException>(() => present.Should().BeNull());
        beNull.Message.Should().Be("المتوقع من present أن يكون null، لكن وُجد 5.");

        int? missing = null;
        double? missingDouble = null;
        var haveValue = Throws<AssertionFailedException>(() => missing.Should().HaveValue());
        haveValue.Message.Should().Be("المتوقع من missing ألا يكون null، لكن وُجد null.");

        var positive = Throws<AssertionFailedException>(() => missing.Should().BePositive());
        positive.Message.Should().Be("المتوقع من missing أن يكون موجباً، لكن وُجد <null>.");

        var approximate = Throws<AssertionFailedException>(() => missingDouble.Should().BeApproximately(1.0, 0.1));
        approximate.Message.Should().Be("المتوقع من missingDouble أن يقارب 1 (±0.1)، لكن وُجد <null>.");
    }

    [TestMethod]
    public void Arabic_ComparableType() {
        using var _ = new ArabicScope();

        var number = new ComparableTypeAssertions<int>(5, "number");
        var be = Throws<AssertionFailedException>(() => number.Be(6));
        be.Message.Should().Be("المتوقع من number أن يكون 6، لكن وُجد 5.");

        var notBe = Throws<AssertionFailedException>(() => number.NotBe(5));
        notBe.Message.Should().Be("المتوقع من number ألا يكون 5، لكن وُجد 5.");

        var greater = Throws<AssertionFailedException>(() => number.BeGreaterThan(10));
        greater.Message.Should().Be("المتوقع من number أن يكون أكبر من 10، لكن وُجد 5.");

        var greaterOrEqual = Throws<AssertionFailedException>(() => number.BeGreaterThanOrEqualTo(6));
        greaterOrEqual.Message.Should().Be("المتوقع من number أن يكون أكبر من أو يساوي 6، لكن وُجد 5.");

        var less = Throws<AssertionFailedException>(() => number.BeLessThan(5));
        less.Message.Should().Be("المتوقع من number أن يكون أصغر من 5، لكن وُجد 5.");

        var lessOrEqual = Throws<AssertionFailedException>(() => number.BeLessThanOrEqualTo(4));
        lessOrEqual.Message.Should().Be("المتوقع من number أن يكون أصغر من أو يساوي 4، لكن وُجد 5.");

        var between = Throws<AssertionFailedException>(() => number.BeInRange(6, 10));
        between.Message.Should().Be("المتوقع من number أن يكون بين 6 و10، لكن وُجد 5.");

        var notBetween = Throws<AssertionFailedException>(() => number.NotBeInRange(1, 10));
        notBetween.Message.Should().Be("المتوقع من number ألا يكون بين 1 و10، لكن وُجد 5.");

        var oneOf = Throws<AssertionFailedException>(() => number.BeOneOf(3, 6));
        oneOf.Message.Should().Be("المتوقع من number أن يكون أحد 3, 6، لكن وُجد 5.");

        var match = Throws<AssertionFailedException>(() => number.Match(x => x > 10));
        match.Message.Should().Be("المتوقع من number أن يطابق 'x => (x > 10)'، لكنه لم يطابق.");
    }

    [TestMethod]
    public void Arabic_Enum() {
        using var _ = new ArabicScope();

        const DayOfWeek day = DayOfWeek.Friday;
        var be = Throws<AssertionFailedException>(() => day.Should().Be(DayOfWeek.Monday));
        be.Message.Should().Be("المتوقع من day أن يكون Monday، لكن وُجد Friday.");

        var notBe = Throws<AssertionFailedException>(() => day.Should().NotBe(DayOfWeek.Friday));
        notBe.Message.Should().Be("المتوقع من day ألا يكون Friday، لكن وُجد Friday.");

        var flag = Throws<AssertionFailedException>(() => Access.Read.Should().HaveFlag(Access.Write));
        flag.Message.Should().Be("المتوقع من Access.Read أن يحمل الراية Write، لكن وُجد Read.");

        var notFlag = Throws<AssertionFailedException>(() => Access.Read.Should().NotHaveFlag(Access.Read));
        notFlag.Message.Should().Be("المتوقع من Access.Read ألا يحمل الراية Read، لكن وُجد Read.");

        const DayOfWeek undefined = (DayOfWeek)42;
        var defined = Throws<AssertionFailedException>(() => undefined.Should().BeDefined());
        defined.Message.Should().Be("المتوقع من undefined أن يكون قيمة DayOfWeek معرّفة، لكن وُجد 42.");

        DayOfWeek? missing = null;
        var beNull = Throws<AssertionFailedException>(() => missing.Should().Be(DayOfWeek.Monday));
        beNull.Message.Should().Be("المتوقع من missing أن يكون Monday، لكن وُجد <null>.");

        var haveValue = Throws<AssertionFailedException>(() => missing.Should().HaveValue());
        haveValue.Message.Should().Be("المتوقع من missing ألا يكون null، لكن وُجد null.");
    }

    [TestMethod]
    public void Arabic_Guid() {
        using var _ = new ArabicScope();

        var id = new Guid("11111111-1111-1111-1111-111111111111");
        var be = Throws<AssertionFailedException>(() => id.Should().Be(Guid.Empty));
        be.Message.Should().Be("المتوقع من id أن يكون 00000000-0000-0000-0000-000000000000، لكن وُجد 11111111-1111-1111-1111-111111111111.");

        var notBe = Throws<AssertionFailedException>(() => id.Should().NotBe(id));
        notBe.Message.Should().Be("المتوقع من id ألا يكون 11111111-1111-1111-1111-111111111111، لكن وُجد 11111111-1111-1111-1111-111111111111.");

        var beEmpty = Throws<AssertionFailedException>(() => id.Should().BeEmpty());
        beEmpty.Message.Should().Be("المتوقع من id أن يكون فارغاً، لكن وُجد 11111111-1111-1111-1111-111111111111.");

        var notBeEmpty = Throws<AssertionFailedException>(() => Guid.Empty.Should().NotBeEmpty());
        notBeEmpty.Message.Should().Be("المتوقع من Guid.Empty ألا يكون فارغاً، لكن وُجد 00000000-0000-0000-0000-000000000000.");

        Guid? missing = null;
        var beNull = Throws<AssertionFailedException>(() => missing.Should().Be(Guid.Empty));
        beNull.Message.Should().Be("المتوقع من missing أن يكون 00000000-0000-0000-0000-000000000000، لكن وُجد <null>.");

        var haveValue = Throws<AssertionFailedException>(() => missing.Should().HaveValue());
        haveValue.Message.Should().Be("المتوقع من missing ألا يكون null، لكن وُجد null.");
    }

    [TestMethod]
    public void Arabic_Object() {
        using var _ = new ArabicScope();

        object subject = "actual";
        var be = Throws<AssertionFailedException>(() => subject.Should().Be("expected"));
        be.Message.Should().Be("المتوقع من subject أن يكون 'expected'، لكن كان 'actual'.");

        var notBe = Throws<AssertionFailedException>(() => subject.Should().NotBe("actual"));
        notBe.Message.Should().Be("المتوقع من subject ألا يكون 'actual'، لكن وُجد 'actual'.");

        var same = Throws<AssertionFailedException>(() => subject.Should().BeSameAs(new object()));
        same.Message.Should().Be("المتوقع من subject أن يكون نفس المثيل 'System.Object'، لكن وُجد مرجع مختلف.");

        var notSame = Throws<AssertionFailedException>(() => subject.Should().NotBeSameAs(subject));
        notSame.Message.Should().Be("المتوقع من subject ألا يكون نفس المثيل 'actual'، لكنه كان.");

        var beNull = Throws<AssertionFailedException>(() => subject.Should().BeNull());
        beNull.Message.Should().Be("المتوقع من subject أن يكون null، لكنه لم يكن null.");

        object? missing = null;
        var notBeNull = Throws<AssertionFailedException>(() => missing.Should().NotBeNull());
        notBeNull.Message.Should().Be("المتوقع من missing ألا يكون null، لكن وُجد null.");

        var ofType = Throws<AssertionFailedException>(() => subject.Should().BeOfType<int>());
        ofType.Message.Should().Be("المتوقع من subject أن يكون من النوع Int32، لكن وُجد String.");

        var notOfType = Throws<AssertionFailedException>(() => subject.Should().NotBeOfType<string>());
        notOfType.Message.Should().Be("المتوقع من subject ألا يكون من النوع String، لكن وُجد String.");

        var assignable = Throws<AssertionFailedException>(() => subject.Should().BeAssignableTo<int>());
        assignable.Message.Should().Be("المتوقع من subject أن يكون قابلاً للإسناد إلى Int32، لكن وُجد String.");

        var notAssignable = Throws<AssertionFailedException>(() => subject.Should().NotBeAssignableTo<string>());
        notAssignable.Message.Should().Be("المتوقع من subject ألا يكون قابلاً للإسناد إلى String، لكن وُجد String.");

        var match = Throws<AssertionFailedException>(() => subject.Should().Match(x => x.Equals("zzz")));
        match.Message.Should().Be("المتوقع من subject أن يطابق 'x => x.Equals(\"zzz\")'، لكنه لم يطابق.");

        object boxed = new { A = 1 };
        var equivalent = Throws<AssertionFailedException>(() => boxed.Should().BeEquivalentTo(new { A = 2 }));
        equivalent.Message.Should().Be("المتوقع من boxed أن يكون مكافئاً، لكن A was 1 instead of 2.");
    }

    [TestMethod]
    public void Arabic_ValueChecks() {
        using var _ = new ArabicScope();

        var matches = Throws<AssertionFailedException>(() => Value.ThatMatches(5, static x => x > 10));
        matches.Message.Should().Be("المتوقع من القيمة أن تطابق (x > 10)، لكن وُجد 5.");

        var satisfies = Throws<AssertionFailedException>(
            () => Value.ThatSatisfies(5, static _ => throw new InvalidOperationException("bang")));
        satisfies.Message.Should().Be("المتوقع من القيمة أن تحقق الفحص المعطى، لكن رُمي System.InvalidOperationException: bang.");
    }

    [TestMethod]
    public void Arabic_ComparableRanked() {
        using var _ = new ArabicScope();

        const int rank = 5;
        var ranked = Throws<AssertionFailedException>(() => rank.Should().BeRankedEquallyTo(6));
        ranked.Message.Should().Be("المتوقع من rank أن يكون مساوياً في الترتيب لـ 6، لكن وُجد 5.");

        var notRanked = Throws<AssertionFailedException>(() => rank.Should().NotBeRankedEquallyTo(5));
        notRanked.Message.Should().Be("المتوقع من rank ألا يكون مساوياً في الترتيب لـ 5، لكن وُجد 5.");
    }

    [TestMethod]
    public void Arabic_EnumCrossType() {
        using var _ = new ArabicScope();

        const DayOfWeek day = DayOfWeek.Friday;
        var sameName = Throws<AssertionFailedException>(() => day.Should().HaveSameNameAs(DayOfWeek.Monday));
        sameName.Message.Should().Be("المتوقع من day أن يحمل نفس اسم Monday، لكن وُجد Friday.");

        var notSameName = Throws<AssertionFailedException>(() => day.Should().NotHaveSameNameAs(DayOfWeek.Friday));
        notSameName.Message.Should().Be("المتوقع من day ألا يحمل نفس اسم Friday، لكن وُجد Friday.");

        var sameValue = Throws<AssertionFailedException>(() => day.Should().HaveSameValueAs(DayOfWeek.Monday));
        sameValue.Message.Should().Be("المتوقع من day أن يحمل نفس قيمة Monday، لكن وُجد Friday.");

        var notSameValue = Throws<AssertionFailedException>(() => day.Should().NotHaveSameValueAs(DayOfWeek.Friday));
        notSameValue.Message.Should().Be("المتوقع من day ألا يحمل نفس قيمة Friday، لكن وُجد Friday.");
    }
}

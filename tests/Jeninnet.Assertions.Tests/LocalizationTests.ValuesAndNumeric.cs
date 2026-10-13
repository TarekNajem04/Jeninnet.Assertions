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
        var beTrue = Assert.ThrowsExactly<AssertionFailedException>(() => flag.Should().BeTrue());
        Assert.AreEqual("المتوقع من flag أن يكون true، لكن وُجد false.", beTrue.Message);

        const bool other = true;
        var beFalse = Assert.ThrowsExactly<AssertionFailedException>(() => other.Should().BeFalse());
        Assert.AreEqual("المتوقع من other أن يكون false، لكن وُجد true.", beFalse.Message);

        var notBe = Assert.ThrowsExactly<AssertionFailedException>(() => flag.Should().NotBe(false));
        Assert.AreEqual("المتوقع من flag ألا يكون false، لكن وُجد false.", notBe.Message);

        var imply = Assert.ThrowsExactly<AssertionFailedException>(() => other.Should().Imply(false));
        Assert.AreEqual("المتوقع من other أن يستلزم false، لكن true لا يستلزم false.", imply.Message);
    }

    [TestMethod]
    public void Arabic_NullableBooleanMessages() {
        using var _ = new ArabicScope();

        bool? missing = null;
        var be = Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().Be(true));
        Assert.AreEqual("المتوقع من missing أن يكون true، لكن وُجد <null>.", be.Message);

        var beTrue = Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().BeTrue());
        Assert.AreEqual("المتوقع من missing أن يكون true، لكن وُجد <null>.", beTrue.Message);

        bool? present = true;
        var beFalse = Assert.ThrowsExactly<AssertionFailedException>(() => present.Should().BeFalse());
        Assert.AreEqual("المتوقع من present أن يكون false، لكن وُجد true.", beFalse.Message);

        var beNull = Assert.ThrowsExactly<AssertionFailedException>(() => present.Should().BeNull());
        Assert.AreEqual("المتوقع من present أن يكون null، لكن وُجد true.", beNull.Message);

        var haveValue = Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().HaveValue());
        Assert.AreEqual("المتوقع من missing ألا يكون null، لكن وُجد null.", haveValue.Message);
    }

    [TestMethod]
    public void Arabic_NumericComparisons() {
        using var _ = new ArabicScope();

        int? count = 5;
        var be = Assert.ThrowsExactly<AssertionFailedException>(() => count.Should().Be(6));
        Assert.AreEqual("المتوقع من count أن يكون 6، لكن وُجد 5.", be.Message);

        var notBe = Assert.ThrowsExactly<AssertionFailedException>(() => count.Should().NotBe(5));
        Assert.AreEqual("المتوقع من count ألا يكون 5، لكن وُجد 5.", notBe.Message);

        var greater = Assert.ThrowsExactly<AssertionFailedException>(() => count.Should().BeGreaterThan(10));
        Assert.AreEqual("المتوقع من count أن يكون أكبر من 10، لكن وُجد 5.", greater.Message);

        var greaterOrEqual = Assert.ThrowsExactly<AssertionFailedException>(() => count.Should().BeGreaterThanOrEqualTo(6));
        Assert.AreEqual("المتوقع من count أن يكون أكبر من أو يساوي 6، لكن وُجد 5.", greaterOrEqual.Message);

        var less = Assert.ThrowsExactly<AssertionFailedException>(() => count.Should().BeLessThan(5));
        Assert.AreEqual("المتوقع من count أن يكون أصغر من 5، لكن وُجد 5.", less.Message);

        var lessOrEqual = Assert.ThrowsExactly<AssertionFailedException>(() => count.Should().BeLessThanOrEqualTo(4));
        Assert.AreEqual("المتوقع من count أن يكون أصغر من أو يساوي 4، لكن وُجد 5.", lessOrEqual.Message);
    }

    [TestMethod]
    public void Arabic_NumericSignAndRange() {
        using var _ = new ArabicScope();

        const int number = -2;
        var positive = Assert.ThrowsExactly<AssertionFailedException>(() => number.Should().BePositive());
        Assert.AreEqual("المتوقع من number أن يكون موجباً، لكن وُجد -2.", positive.Message);

        int? zero = 0;
        var negative = Assert.ThrowsExactly<AssertionFailedException>(() => zero.Should().BeNegative());
        Assert.AreEqual("المتوقع من zero أن يكون سالباً، لكن وُجد 0.", negative.Message);

        int? count = 5;
        var between = Assert.ThrowsExactly<AssertionFailedException>(() => count.Should().BeInRange(6, 10));
        Assert.AreEqual("المتوقع من count أن يكون بين 6 و10، لكن وُجد 5.", between.Message);

        var notBetween = Assert.ThrowsExactly<AssertionFailedException>(() => count.Should().NotBeInRange(1, 10));
        Assert.AreEqual("المتوقع من count ألا يكون بين 1 و10، لكن وُجد 5.", notBetween.Message);
    }

    [TestMethod]
    public void Arabic_NumericApproximateAndNaN() {
        using var _ = new ArabicScope();

        const double ratio = 5.1;
        var approximate = Assert.ThrowsExactly<AssertionFailedException>(() => ratio.Should().BeApproximately(5, 0.05));
        Assert.AreEqual("المتوقع من ratio أن يقارب 5 (±0.05)، لكن وُجد 5.1.", approximate.Message);

        const double value = 3.5;
        var notApproximate = Assert.ThrowsExactly<AssertionFailedException>(() => value.Should().NotBeApproximately(3.4, 0.5));
        Assert.AreEqual("المتوقع من value ألا يقارب 3.4 (±0.5)، لكن وُجد 3.5.", notApproximate.Message);

        const double measurement = 1;
        var nanEx = Assert.ThrowsExactly<AssertionFailedException>(() => measurement.Should().BeNaN());
        Assert.AreEqual("المتوقع من measurement أن يكون NaN، لكن وُجد 1.", nanEx.Message);

        const double nan = double.NaN;
        var notNan = Assert.ThrowsExactly<AssertionFailedException>(() => nan.Should().NotBeNaN());
        Assert.AreEqual("المتوقع من nan ألا يكون NaN، لكن وُجد NaN.", notNan.Message);
    }

    [TestMethod]
    public void Arabic_NumericSetAndNull() {
        using var _ = new ArabicScope();

        int? count = 5;
        var oneOf = Assert.ThrowsExactly<AssertionFailedException>(() => count.Should().BeOneOf(3, 6));
        Assert.AreEqual("المتوقع من count أن يكون أحد 3, 6، لكن وُجد 5.", oneOf.Message);

        Expression<Func<int, bool>> isEven = x => x % 2 == 0;
        var match = Assert.ThrowsExactly<AssertionFailedException>(() => count.Should().Match(isEven));
        Assert.AreEqual("المتوقع من count أن يطابق 'x => ((x % 2) == 0)'، لكنه لم يطابق.", match.Message);

        int? present = 5;
        var beNull = Assert.ThrowsExactly<AssertionFailedException>(() => present.Should().BeNull());
        Assert.AreEqual("المتوقع من present أن يكون null، لكن وُجد 5.", beNull.Message);

        int? missing = null;
        double? missingDouble = null;
        var haveValue = Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().HaveValue());
        Assert.AreEqual("المتوقع من missing ألا يكون null، لكن وُجد null.", haveValue.Message);

        var positive = Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().BePositive());
        Assert.AreEqual("المتوقع من missing أن يكون موجباً، لكن وُجد <null>.", positive.Message);

        var approximate = Assert.ThrowsExactly<AssertionFailedException>(() => missingDouble.Should().BeApproximately(1.0, 0.1));
        Assert.AreEqual("المتوقع من missingDouble أن يقارب 1 (±0.1)، لكن وُجد <null>.", approximate.Message);
    }

    [TestMethod]
    public void Arabic_ComparableType() {
        using var _ = new ArabicScope();

        var number = new ComparableTypeAssertions<int>(5, "number");
        var be = Assert.ThrowsExactly<AssertionFailedException>(() => number.Be(6));
        Assert.AreEqual("المتوقع من number أن يكون 6، لكن وُجد 5.", be.Message);

        var notBe = Assert.ThrowsExactly<AssertionFailedException>(() => number.NotBe(5));
        Assert.AreEqual("المتوقع من number ألا يكون 5، لكن وُجد 5.", notBe.Message);

        var greater = Assert.ThrowsExactly<AssertionFailedException>(() => number.BeGreaterThan(10));
        Assert.AreEqual("المتوقع من number أن يكون أكبر من 10، لكن وُجد 5.", greater.Message);

        var greaterOrEqual = Assert.ThrowsExactly<AssertionFailedException>(() => number.BeGreaterThanOrEqualTo(6));
        Assert.AreEqual("المتوقع من number أن يكون أكبر من أو يساوي 6، لكن وُجد 5.", greaterOrEqual.Message);

        var less = Assert.ThrowsExactly<AssertionFailedException>(() => number.BeLessThan(5));
        Assert.AreEqual("المتوقع من number أن يكون أصغر من 5، لكن وُجد 5.", less.Message);

        var lessOrEqual = Assert.ThrowsExactly<AssertionFailedException>(() => number.BeLessThanOrEqualTo(4));
        Assert.AreEqual("المتوقع من number أن يكون أصغر من أو يساوي 4، لكن وُجد 5.", lessOrEqual.Message);

        var between = Assert.ThrowsExactly<AssertionFailedException>(() => number.BeInRange(6, 10));
        Assert.AreEqual("المتوقع من number أن يكون بين 6 و10، لكن وُجد 5.", between.Message);

        var notBetween = Assert.ThrowsExactly<AssertionFailedException>(() => number.NotBeInRange(1, 10));
        Assert.AreEqual("المتوقع من number ألا يكون بين 1 و10، لكن وُجد 5.", notBetween.Message);

        var oneOf = Assert.ThrowsExactly<AssertionFailedException>(() => number.BeOneOf(3, 6));
        Assert.AreEqual("المتوقع من number أن يكون أحد 3, 6، لكن وُجد 5.", oneOf.Message);

        var match = Assert.ThrowsExactly<AssertionFailedException>(() => number.Match(x => x > 10));
        Assert.AreEqual("المتوقع من number أن يطابق 'x => (x > 10)'، لكنه لم يطابق.", match.Message);
    }

    [TestMethod]
    public void Arabic_Enum() {
        using var _ = new ArabicScope();

        const DayOfWeek day = DayOfWeek.Friday;
        var be = Assert.ThrowsExactly<AssertionFailedException>(() => day.Should().Be(DayOfWeek.Monday));
        Assert.AreEqual("المتوقع من day أن يكون Monday، لكن وُجد Friday.", be.Message);

        var notBe = Assert.ThrowsExactly<AssertionFailedException>(() => day.Should().NotBe(DayOfWeek.Friday));
        Assert.AreEqual("المتوقع من day ألا يكون Friday، لكن وُجد Friday.", notBe.Message);

        var flag = Assert.ThrowsExactly<AssertionFailedException>(() => Access.Read.Should().HaveFlag(Access.Write));
        Assert.AreEqual("المتوقع من Access.Read أن يحمل الراية Write، لكن وُجد Read.", flag.Message);

        var notFlag = Assert.ThrowsExactly<AssertionFailedException>(() => Access.Read.Should().NotHaveFlag(Access.Read));
        Assert.AreEqual("المتوقع من Access.Read ألا يحمل الراية Read، لكن وُجد Read.", notFlag.Message);

        const DayOfWeek undefined = (DayOfWeek)42;
        var defined = Assert.ThrowsExactly<AssertionFailedException>(() => undefined.Should().BeDefined());
        Assert.AreEqual("المتوقع من undefined أن يكون قيمة DayOfWeek معرّفة، لكن وُجد 42.", defined.Message);

        DayOfWeek? missing = null;
        var beNull = Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().Be(DayOfWeek.Monday));
        Assert.AreEqual("المتوقع من missing أن يكون Monday، لكن وُجد <null>.", beNull.Message);

        var haveValue = Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().HaveValue());
        Assert.AreEqual("المتوقع من missing ألا يكون null، لكن وُجد null.", haveValue.Message);
    }

    [TestMethod]
    public void Arabic_Guid() {
        using var _ = new ArabicScope();

        var id = new Guid("11111111-1111-1111-1111-111111111111");
        var be = Assert.ThrowsExactly<AssertionFailedException>(() => id.Should().Be(Guid.Empty));
        Assert.AreEqual("المتوقع من id أن يكون 00000000-0000-0000-0000-000000000000، لكن وُجد 11111111-1111-1111-1111-111111111111.", be.Message);

        var notBe = Assert.ThrowsExactly<AssertionFailedException>(() => id.Should().NotBe(id));
        Assert.AreEqual("المتوقع من id ألا يكون 11111111-1111-1111-1111-111111111111، لكن وُجد 11111111-1111-1111-1111-111111111111.", notBe.Message);

        var beEmpty = Assert.ThrowsExactly<AssertionFailedException>(() => id.Should().BeEmpty());
        Assert.AreEqual("المتوقع من id أن يكون فارغاً، لكن وُجد 11111111-1111-1111-1111-111111111111.", beEmpty.Message);

        var notBeEmpty = Assert.ThrowsExactly<AssertionFailedException>(() => Guid.Empty.Should().NotBeEmpty());
        Assert.AreEqual("المتوقع من Guid.Empty ألا يكون فارغاً، لكن وُجد 00000000-0000-0000-0000-000000000000.", notBeEmpty.Message);

        Guid? missing = null;
        var beNull = Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().Be(Guid.Empty));
        Assert.AreEqual("المتوقع من missing أن يكون 00000000-0000-0000-0000-000000000000، لكن وُجد <null>.", beNull.Message);

        var haveValue = Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().HaveValue());
        Assert.AreEqual("المتوقع من missing ألا يكون null، لكن وُجد null.", haveValue.Message);
    }

    [TestMethod]
    public void Arabic_Object() {
        using var _ = new ArabicScope();

        object subject = "actual";
        var be = Assert.ThrowsExactly<AssertionFailedException>(() => subject.Should().Be("expected"));
        Assert.AreEqual("المتوقع من subject أن يكون 'expected'، لكن كان 'actual'.", be.Message);

        var notBe = Assert.ThrowsExactly<AssertionFailedException>(() => subject.Should().NotBe("actual"));
        Assert.AreEqual("المتوقع من subject ألا يكون 'actual'، لكن وُجد 'actual'.", notBe.Message);

        var same = Assert.ThrowsExactly<AssertionFailedException>(() => subject.Should().BeSameAs(new object()));
        Assert.AreEqual("المتوقع من subject أن يكون نفس المثيل 'System.Object'، لكن وُجد مرجع مختلف.", same.Message);

        var notSame = Assert.ThrowsExactly<AssertionFailedException>(() => subject.Should().NotBeSameAs(subject));
        Assert.AreEqual("المتوقع من subject ألا يكون نفس المثيل 'actual'، لكنه كان.", notSame.Message);

        var beNull = Assert.ThrowsExactly<AssertionFailedException>(() => subject.Should().BeNull());
        Assert.AreEqual("المتوقع من subject أن يكون null، لكنه لم يكن null.", beNull.Message);

        object? missing = null;
        var notBeNull = Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().NotBeNull());
        Assert.AreEqual("المتوقع من missing ألا يكون null، لكن وُجد null.", notBeNull.Message);

        var ofType = Assert.ThrowsExactly<AssertionFailedException>(() => subject.Should().BeOfType<int>());
        Assert.AreEqual("المتوقع من subject أن يكون من النوع Int32، لكن وُجد String.", ofType.Message);

        var notOfType = Assert.ThrowsExactly<AssertionFailedException>(() => subject.Should().NotBeOfType<string>());
        Assert.AreEqual("المتوقع من subject ألا يكون من النوع String، لكن وُجد String.", notOfType.Message);

        var assignable = Assert.ThrowsExactly<AssertionFailedException>(() => subject.Should().BeAssignableTo<int>());
        Assert.AreEqual("المتوقع من subject أن يكون قابلاً للإسناد إلى Int32، لكن وُجد String.", assignable.Message);

        var notAssignable = Assert.ThrowsExactly<AssertionFailedException>(() => subject.Should().NotBeAssignableTo<string>());
        Assert.AreEqual("المتوقع من subject ألا يكون قابلاً للإسناد إلى String، لكن وُجد String.", notAssignable.Message);

        var match = Assert.ThrowsExactly<AssertionFailedException>(() => subject.Should().Match(x => x.Equals("zzz")));
        Assert.AreEqual("المتوقع من subject أن يطابق 'x => x.Equals(\"zzz\")'، لكنه لم يطابق.", match.Message);

        object boxed = new { A = 1 };
        var equivalent = Assert.ThrowsExactly<AssertionFailedException>(() => boxed.Should().BeEquivalentTo(new { A = 2 }));
        Assert.AreEqual("المتوقع من boxed أن يكون مكافئاً، لكن A was 1 instead of 2.", equivalent.Message);
    }

    [TestMethod]
    public void Arabic_ValueChecks() {
        using var _ = new ArabicScope();

        var matches = Assert.ThrowsExactly<AssertionFailedException>(() => Value.ThatMatches(5, static x => x > 10));
        Assert.AreEqual("المتوقع من القيمة أن تطابق (x > 10)، لكن وُجد 5.", matches.Message);

        var satisfies = Assert.ThrowsExactly<AssertionFailedException>(
            () => Value.ThatSatisfies(5, static _ => throw new InvalidOperationException("bang")));
        Assert.AreEqual("المتوقع من القيمة أن تحقق الفحص المعطى، لكن رُمي System.InvalidOperationException: bang.", satisfies.Message);
    }

    [TestMethod]
    public void Arabic_ComparableRanked() {
        using var _ = new ArabicScope();

        const int rank = 5;
        var ranked = Assert.ThrowsExactly<AssertionFailedException>(() => rank.Should().BeRankedEquallyTo(6));
        Assert.AreEqual("المتوقع من rank أن يكون مساوياً في الترتيب لـ 6، لكن وُجد 5.", ranked.Message);

        var notRanked = Assert.ThrowsExactly<AssertionFailedException>(() => rank.Should().NotBeRankedEquallyTo(5));
        Assert.AreEqual("المتوقع من rank ألا يكون مساوياً في الترتيب لـ 5، لكن وُجد 5.", notRanked.Message);
    }

    [TestMethod]
    public void Arabic_EnumCrossType() {
        using var _ = new ArabicScope();

        const DayOfWeek day = DayOfWeek.Friday;
        var sameName = Assert.ThrowsExactly<AssertionFailedException>(() => day.Should().HaveSameNameAs(DayOfWeek.Monday));
        Assert.AreEqual("المتوقع من day أن يحمل نفس اسم Monday، لكن وُجد Friday.", sameName.Message);

        var notSameName = Assert.ThrowsExactly<AssertionFailedException>(() => day.Should().NotHaveSameNameAs(DayOfWeek.Friday));
        Assert.AreEqual("المتوقع من day ألا يحمل نفس اسم Friday، لكن وُجد Friday.", notSameName.Message);

        var sameValue = Assert.ThrowsExactly<AssertionFailedException>(() => day.Should().HaveSameValueAs(DayOfWeek.Monday));
        Assert.AreEqual("المتوقع من day أن يحمل نفس قيمة Monday، لكن وُجد Friday.", sameValue.Message);

        var notSameValue = Assert.ThrowsExactly<AssertionFailedException>(() => day.Should().NotHaveSameValueAs(DayOfWeek.Friday));
        Assert.AreEqual("المتوقع من day ألا يحمل نفس قيمة Friday، لكن وُجد Friday.", notSameValue.Message);
    }
}

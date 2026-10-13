//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Tests;

[TestClass]
public class EnumTests {
    [Flags]
    private enum Access {
        None = 0,
        Read = 1,
        Write = 1 << 1,
    }

    [TestMethod]
    public void Be_PassAndFail() {
        const DayOfWeek day = DayOfWeek.Friday;
        day.Should().Be(DayOfWeek.Friday);

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => day.Should().Be(DayOfWeek.Monday));
        Assert.AreEqual("Expected day to be Monday, but found Friday.", ex.Message);
    }

    [TestMethod]
    public void NotBe_FailsForEqual() {
        const Access access = Access.Read;
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => access.Should().NotBe(Access.Read));
        Assert.AreEqual("Expected access not to be Read, but found Read.", ex.Message);
    }

    [TestMethod]
    public void HaveFlag_PassAndFail() {
        const Access access = Access.Read | Access.Write;
        access.Should().HaveFlag(Access.Read);

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => Access.Read.Should().NotHaveFlag(Access.Read));
        Assert.AreEqual("Expected Access.Read not to have flag Read, but found Read.", ex.Message);
    }

    [TestMethod]
    public void BeDefined_PassAndFail() {
        DayOfWeek.Friday.Should().BeDefined();
        Access.None.Should().BeDefined();

        const DayOfWeek undefined = (DayOfWeek)42;
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => undefined.Should().BeDefined());
        Assert.AreEqual("Expected undefined to be a defined DayOfWeek value, but found 42.", ex.Message);
    }

    [TestMethod]
    public void Nullable_Behaviors() {
        DayOfWeek? present = DayOfWeek.Friday;
        present.Should().Be(DayOfWeek.Friday).And.BeDefined().And.HaveValue();

        DayOfWeek? missing = null;
        missing.Should().Be(null).And.BeNull();
        missing.Should().NotBe(DayOfWeek.Friday).And.NotHaveFlag(DayOfWeek.Friday);

        var flag = Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().HaveFlag(DayOfWeek.Friday));
        Assert.AreEqual("Expected missing to have flag Friday, but found <null>.", flag.Message);
    }

    [TestMethod]
    public void NullableHaveValue_PassAndFail() {
        DayOfWeek? present = DayOfWeek.Friday;
        present.Should().HaveValue();

        DayOfWeek? missing = null;
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().HaveValue());
        Assert.AreEqual("Expected missing to not be null, but found null.", ex.Message);
    }

    [TestMethod]
    public void NullableNotHaveValue_PassAndFail() {
        DayOfWeek? missing = null;
        missing.Should().NotHaveValue();

        DayOfWeek? present = DayOfWeek.Friday;
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => present.Should().NotHaveValue());
        Assert.AreEqual("Expected present to be null, but found Friday.", ex.Message);
    }

    [TestMethod]
    public void NotBeDefined_PassAndFail() {
        const DayOfWeek undefined = (DayOfWeek)42;
        undefined.Should().NotBeDefined();

        const DayOfWeek defined = DayOfWeek.Friday;
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => defined.Should().NotBeDefined());
        Assert.AreEqual("Expected defined not to be a defined DayOfWeek value, but found Friday.", ex.Message);

        DayOfWeek? missing = null;
        missing.Should().NotBeDefined();
    }

    [TestMethod]
    public void BeOneOf_PassFailAndNullGuard() {
        const DayOfWeek day = DayOfWeek.Friday;
        day.Should().BeOneOf(DayOfWeek.Monday, DayOfWeek.Friday);

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => day.Should().BeOneOf(DayOfWeek.Monday, DayOfWeek.Sunday));
        Assert.AreEqual("Expected day to be one of Monday, Sunday, but found Friday.", ex.Message);

        Assert.ThrowsExactly<ArgumentNullException>(() => day.Should().BeOneOf((DayOfWeek[])null!));

        DayOfWeek? missing = null;
        var nullEx = Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().BeOneOf(DayOfWeek.Friday));
        Assert.AreEqual("Expected missing to be one of Friday, but found <null>.", nullEx.Message);
    }

    [TestMethod]
    public void HaveValue_PassAndFail() {
        const DayOfWeek day = DayOfWeek.Friday;
        day.Should().HaveValue(5);
        day.Should().NotHaveValue(6);

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => day.Should().HaveValue(6));
        Assert.AreEqual("Expected day to have value 6, but found Friday.", ex.Message);

        var notEx = Assert.ThrowsExactly<AssertionFailedException>(() => day.Should().NotHaveValue(5));
        Assert.AreEqual("Expected day not to have value 5, but found Friday.", notEx.Message);
    }

    [TestMethod]
    public void Match_PassFailAndNullGuard() {
        const DayOfWeek day = DayOfWeek.Friday;
        day.Should().Match(static d => d == DayOfWeek.Friday);

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => day.Should().Match(static d => d == DayOfWeek.Monday));
        Assert.AreEqual("Expected day to match '(Convert(d, Nullable`1) == Convert(Monday, Nullable`1))', but it did not.", ex.Message);

        Assert.ThrowsExactly<ArgumentNullException>(() => day.Should().Match(null!));

        DayOfWeek? missing = null;
        Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().Match(static d => d == DayOfWeek.Friday));
    }

    [TestMethod]
    public void SameName_PassAndFail() {
        const Bird sparrow = Bird.Sparrow;
        sparrow.Should().HaveSameNameAs(Fish.Sparrow);
        sparrow.Should().NotHaveSameValueAs(Fish.Sparrow);

        var nameEx = Assert.ThrowsExactly<AssertionFailedException>(() => sparrow.Should().HaveSameNameAs(Fish.Shark));
        Assert.AreEqual("Expected sparrow to have the same name as Shark, but found Sparrow.", nameEx.Message);

        var notNameEx = Assert.ThrowsExactly<AssertionFailedException>(() => sparrow.Should().NotHaveSameNameAs(Fish.Sparrow));
        Assert.AreEqual("Expected sparrow not to have the same name as Sparrow, but found Sparrow.", notNameEx.Message);

        Bird? missing = null;
        var nullEx = Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().HaveSameNameAs(Fish.Sparrow));
        Assert.AreEqual("Expected missing to have the same name as Sparrow, but found <null>.", nullEx.Message);
        missing.Should().NotHaveSameNameAs(Fish.Sparrow);
    }

    [TestMethod]
    public void SameValue_PassAndFail() {
        const Bird sparrow = Bird.Sparrow;
        sparrow.Should().HaveSameValueAs(Fish.Shark);
        sparrow.Should().NotHaveSameNameAs(Fish.Shark);

        var valueEx = Assert.ThrowsExactly<AssertionFailedException>(() => sparrow.Should().HaveSameValueAs(Fish.Trout));
        Assert.AreEqual("Expected sparrow to have the same value as Trout, but found Sparrow.", valueEx.Message);

        var notValueEx = Assert.ThrowsExactly<AssertionFailedException>(() => sparrow.Should().NotHaveSameValueAs(Fish.Shark));
        Assert.AreEqual("Expected sparrow not to have the same value as Shark, but found Sparrow.", notValueEx.Message);

        Bird? missing = null;
        missing.Should().NotHaveSameValueAs(Fish.Shark);
    }

    private enum Bird {
        Sparrow = 1,
        Eagle = 2,
    }

    private enum Fish {
        Shark = 1,
        Trout = 3,
        Sparrow = 5,
    }
}

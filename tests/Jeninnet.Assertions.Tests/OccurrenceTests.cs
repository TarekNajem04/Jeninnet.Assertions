//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Tests;

[TestClass]
public class OccurrenceTests {
    [TestMethod]
    public void Exactly_Once_HasExpectedBounds() {
        var constraint = Exactly.Once();
        Assert.AreEqual(1, constraint.Minimum);
        Assert.AreEqual(1, constraint.Maximum);
        Assert.AreEqual("exactly once", constraint.Expectation);
        Assert.IsTrue(constraint.Matches(1));
        Assert.IsFalse(constraint.Matches(0));
        Assert.IsFalse(constraint.Matches(2));
    }

    [TestMethod]
    public void Exactly_Times_UsesWordsForSmallCounts() {
        Assert.AreEqual("exactly twice", Exactly.Twice().Expectation);
        Assert.AreEqual("exactly thrice", Exactly.Thrice().Expectation);
        Assert.AreEqual("exactly 4 times", Exactly.Times(4).Expectation);
    }

    [TestMethod]
    public void AtLeast_Twice_HasOpenUpperBound() {
        var constraint = AtLeast.Twice();
        Assert.AreEqual(2, constraint.Minimum);
        Assert.AreEqual(int.MaxValue, constraint.Maximum);
        Assert.AreEqual("at least twice", constraint.Expectation);
        Assert.IsTrue(constraint.Matches(100));
        Assert.IsFalse(constraint.Matches(1));
    }

    [TestMethod]
    public void AtMost_Times_HasZeroLowerBound() {
        var constraint = AtMost.Times(5);
        Assert.AreEqual(0, constraint.Minimum);
        Assert.AreEqual(5, constraint.Maximum);
        Assert.AreEqual("at most 5 times", constraint.Expectation);
    }

    [TestMethod]
    public void MoreThan_Once_StartsAfterOne() {
        var constraint = MoreThan.Once();
        Assert.AreEqual(2, constraint.Minimum);
        Assert.AreEqual("more than once", constraint.Expectation);
    }

    [TestMethod]
    public void LessThan_Times_EndsBeforeCount() {
        var constraint = LessThan.Times(3);
        Assert.AreEqual(0, constraint.Minimum);
        Assert.AreEqual(2, constraint.Maximum);
        Assert.AreEqual("less than thrice", constraint.Expectation);
    }

    [TestMethod]
    public void IntegerSugar_MatchesFactories() {
        Assert.AreEqual(Exactly.Twice(), 2.TimesExactly());
        Assert.AreEqual(AtLeast.Times(3), 3.TimesOrMore());
        Assert.AreEqual(AtMost.Once(), 1.TimesOrLess());
    }

    [TestMethod]
    public void NegativeCounts_ThrowArgumentOutOfRange() {
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => Exactly.Times(-1));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => AtLeast.Times(-1));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => AtMost.Times(-1));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => MoreThan.Times(-1));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => LessThan.Times(0));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new OccurrenceConstraint(-1, 2, "invalid"));
    }

    [TestMethod]
    public void InvertedBounds_ThrowArgumentException() => Assert.ThrowsExactly<ArgumentException>(() => new OccurrenceConstraint(5, 3, "invalid"));
}

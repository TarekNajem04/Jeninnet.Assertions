//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Tests;

[TestClass]
public class GivenChainTests {
    [TestMethod]
    public void Given_WhenConditionMet_DoesNotThrow() {
        using var scope = new AssertionScope();
        scope.Given(() => 5).ForCondition(value => value > 3).FailWith("Expected the value to be greater than 3.");
        Assert.IsFalse(scope.HasFailures());
    }

    [TestMethod]
    public void Given_WhenConditionMet_FailWithAfterwards_IsIneffective() {
        using var scope = new AssertionScope();
        scope.Given(() => 5).ForCondition(value => value > 3).FailWith("Expected the value to be greater than 3.");
        scope.Given(() => 1).ForCondition(value => value == 1).FailWith("Expected the value to differ.");
        Assert.IsFalse(scope.HasFailures());
    }

    [TestMethod]
    public void Given_WhenConditionNotMet_ThrowsFormattedMessage() {
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => {
            using var scope = new AssertionScope();
            scope.Given(() => 5).ForCondition(value => value > 10).FailWith("Expected a value greater than 10, but found {0}.", value => value);
        });
        Assert.AreEqual("Expected a value greater than 10, but found 5.", ex.Message);
    }

    [TestMethod]
    public void Given_FormattedArgs_ReceiveSelectedValue() {
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => {
            using var scope = new AssertionScope();
            scope.Given(() => "alpha").ForCondition(value => value.Length == 0).FailWith("Expected an empty string, but found {0}.", value => value);
        });
        Assert.AreEqual("Expected an empty string, but found alpha.", ex.Message);
    }

    [TestMethod]
    public void Given_FailureMessage_IsCapitalized() {
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => {
            using var scope = new AssertionScope();
            scope.Given(() => 1).ForCondition(value => value != 1).FailWith("expected the value to differ.");
        });
        Assert.AreEqual("Expected the value to differ.", ex.Message);
    }

    [TestMethod]
    public void Given_ProjectedValue_FeedsNextCondition() {
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => {
            using var scope = new AssertionScope();
            scope.Given(() => 21)
                .Given(value => value * 2)
                .ForCondition(value => value > 50)
                .FailWith("Expected the doubled value to exceed 50, but got {0}.", value => value);
        });
        Assert.AreEqual("Expected the doubled value to exceed 50, but got 42.", ex.Message);
    }

    [TestMethod]
    public void Given_AfterFailure_SelectorsAndFailWithsAreSkipped() {
        var evaluations = 0;
        Assert.ThrowsExactly<AssertionFailedException>(() => {
            using var scope = new AssertionScope();
            scope.Given(() => 5)
                .ForCondition(value => value > 10)
                .FailWith("first {0}", value => value)
                .Then
                .Given(value => {
                    evaluations++;
                    return value;
                })
                .FailWith("second {0}", value => value);
        });
        Assert.AreEqual(0, evaluations);
    }

    [TestMethod]
    public void Given_MultipleFailures_AreCollectedAndAggregated() {
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => {
            using var scope = new AssertionScope();
            scope.Given(() => 5).ForCondition(value => value > 10).FailWith("first failure");
            scope.Given(() => 2).ForCondition(value => value > 10).FailWith("second failure");
        });
        Assert.Contains("First failure", ex.Message);
        Assert.Contains("Second failure", ex.Message);
    }

    [TestMethod]
    public void Given_ThrowsForNullSelector() {
        using var scope = new AssertionScope();
        Assert.ThrowsExactly<ArgumentNullException>(() => scope.Given((Func<int>)(null!)));
    }

    [TestMethod]
    public void Given_ThrowsForNullPredicate() {
        using var scope = new AssertionScope();
        Assert.ThrowsExactly<ArgumentNullException>(() => scope.Given(() => 5).ForCondition((Func<int, bool>)(null!)));
    }
}

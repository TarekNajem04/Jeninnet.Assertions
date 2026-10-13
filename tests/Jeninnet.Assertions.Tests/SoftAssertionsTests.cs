//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Tests;

[TestClass]
public class SoftAssertionsTests {
    [TestMethod]
    public void Soft_PassingAssertions_DoNotThrowOnReport() {
        var soft = new SoftAssertions().Collect();

        true.Should().BeTrue();
        "a".Should().Be("a");

        soft.ThenReport();
    }

    [TestMethod]
    public void Soft_BatchesMultipleFailuresIntoOneReport() {
        var soft = new SoftAssertions().Collect();

        const bool flag = false;
        flag.Should().BeTrue();
        const string actual = "Actual";
        actual.Should().Be("Expected");
        var numbers = new int[] { 1, 2 };
        numbers.Should().Contain(3);

        var ex = Assert.ThrowsExactly<AssertionFailedException>(soft.ThenReport);

        Assert.Contains("3 assertion failures:", ex.Message);
        Assert.Contains("Expected flag to be true, but found false.", ex.Message);
        Assert.Contains("Expected actual to be 'Expected', but found 'Actual'.", ex.Message);
        Assert.Contains("Expected numbers to contain '3', but it was not found.", ex.Message);
    }

    [TestMethod]
    public void Soft_SingleFailure_ReportsOriginalMessage() {
        var soft = new SoftAssertions().Collect();

        const bool flag = false;
        flag.Should().BeTrue();

        var ex = Assert.ThrowsExactly<AssertionFailedException>(soft.ThenReport);

        Assert.AreEqual("Expected flag to be true, but found false.", ex.Message);
    }

    [TestMethod]
    public void Soft_ReportWithoutCollect_ThrowsInvalidOperation() {
        var soft = new SoftAssertions();

        var ex = Assert.ThrowsExactly<InvalidOperationException>(soft.ThenReport);

        Assert.Contains("Collect()", ex.Message);
    }

    [TestMethod]
    public void Soft_DefaultPathWithoutCollect_StillThrowsImmediately() {
        const bool flag = false;

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => flag.Should().BeTrue());

        Assert.AreEqual("Expected flag to be true, but found false.", ex.Message);
    }

    [TestMethod]
    public void Soft_AmbientScopeIsRestoredAfterReport() {
        var soft = new SoftAssertions().Collect();
        const bool flag = false;
        flag.Should().BeTrue();

        Assert.ThrowsExactly<AssertionFailedException>(soft.ThenReport);

        const bool other = true;
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => other.Should().BeFalse());
        Assert.AreEqual("Expected other to be false, but found true.", ex.Message);
    }

    [TestMethod]
    public void Soft_CanCollectReportCollectAgain() {
        var soft = new SoftAssertions();
        soft.Collect();
        soft.ThenReport();

        soft.Collect();
        const bool flag = false;
        flag.Should().BeTrue();
        var ex = Assert.ThrowsExactly<AssertionFailedException>(soft.ThenReport);
        Assert.Contains("Expected flag to be true, but found false.", ex.Message);
    }

    [TestMethod]
    public void Soft_CollectIsIdempotentWhileCollecting() {
        var soft = new SoftAssertions().Collect();
        var again = soft.Collect();

        Assert.AreSame(soft, again);
        const bool flag = false;
        flag.Should().BeTrue();

        var ex = Assert.ThrowsExactly<AssertionFailedException>(soft.ThenReport);
        Assert.Contains("Expected flag to be true, but found false.", ex.Message);
    }

    [TestMethod]
    public void Soft_DisposeWithoutExplicitReport_StillReports() {
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => {
            using var soft = new SoftAssertions();
            soft.Collect();
            const bool flag = false;
            flag.Should().BeTrue();
            const string actual = "Actual";
            actual.Should().Be("Expected");
        });

        Assert.Contains("2 assertion failures:", ex.Message);
        Assert.Contains("Expected flag to be true, but found false.", ex.Message);
        Assert.Contains("Expected actual to be 'Expected', but found 'Actual'.", ex.Message);
    }
}

//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Tests;

[TestClass]
public class BoolAssertionsTests {
    [TestMethod]
    public void BeTrue_PassForTrue() => true.Should().BeTrue();

    [TestMethod]
    public void BeTrue_FailsForFalse() {
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => false.Should().BeTrue());
        Assert.AreEqual("Expected false to be true, but found false.", ex.Message);
    }

    [TestMethod]
    public void BeFalse_PassForFalse() => false.Should().BeFalse();

    [TestMethod]
    public void BeFalse_FailsForTrue() {
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => true.Should().BeFalse());
        Assert.AreEqual("Expected true to be false, but found true.", ex.Message);
    }
}

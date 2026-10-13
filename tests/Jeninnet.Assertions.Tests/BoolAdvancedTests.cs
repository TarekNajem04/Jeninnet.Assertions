//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Tests;

[TestClass]
public class BoolAdvancedTests {
    [TestMethod]
    public void NotBe_PassForDifferentValue() {
        const bool flag = true;
        flag.Should().NotBe(false);
    }

    [TestMethod]
    public void NotBe_FailsForSameValue() {
        const bool flag = true;
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => flag.Should().NotBe(true));
        Assert.AreEqual("Expected flag not to be true, but found true.", ex.Message);
    }

    [TestMethod]
    public void Imply_PassForValidImplications() {
        const bool yes = true;
        const bool no = false;
        yes.Should().Imply(true);
        no.Should().Imply(false);
        no.Should().Imply(true);
    }

    [TestMethod]
    public void Imply_FailsForTrueImplyingFalse() {
        const bool flag = true;
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => flag.Should().Imply(false));
        Assert.AreEqual("Expected flag to imply false, but true does not imply false.", ex.Message);
    }

    [TestMethod]
    public void NullableBe_PassForEqualAndNull() {
        bool? flag = true;
        flag.Should().Be(true);
        flag.Should().BeTrue();
        bool? missing = null;
        missing.Should().Be(null);
        missing.Should().BeNull();
    }

    [TestMethod]
    public void NullableBeTrue_FailsForFalseAndNull() {
        bool? flag = false;
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => flag.Should().BeTrue());
        Assert.AreEqual("Expected flag to be true, but found false.", ex.Message);

        bool? missing = null;
        var nullEx = Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().BeTrue());
        Assert.AreEqual("Expected missing to be true, but found <null>.", nullEx.Message);
    }

    [TestMethod]
    public void NullableBeFalse_FailsForTrue() {
        bool? flag = true;
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => flag.Should().BeFalse());
        Assert.AreEqual("Expected flag to be false, but found true.", ex.Message);
    }

    [TestMethod]
    public void NullableNotBe_FailsForSameValue() {
        bool? flag = true;
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => flag.Should().NotBe(true));
        Assert.AreEqual("Expected flag not to be true, but found true.", ex.Message);
    }

    [TestMethod]
    public void NullableHaveValue_FailsForNull() {
        bool? missing = null;
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().HaveValue());
        Assert.AreEqual("Expected missing to not be null, but found null.", ex.Message);
    }

    [TestMethod]
    public void NullableHaveValue_PassForValue() {
        bool? flag = true;
        flag.Should().HaveValue();
    }

    [TestMethod]
    public void NullableNotHaveValue_PassAndFail() {
        bool? missing = null;
        missing.Should().NotHaveValue();

        bool? flag = true;
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => flag.Should().NotHaveValue());
        Assert.AreEqual("Expected flag to be null, but found true.", ex.Message);
    }

    [TestMethod]
    public void NullableNotBeTrue_PassAndFail() {
        bool? no = false;
        no.Should().NotBeTrue();

        bool? missing = null;
        missing.Should().NotBeTrue();

        bool? yes = true;
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => yes.Should().NotBeTrue());
        Assert.AreEqual("Expected yes not to be true, but found true.", ex.Message);
    }

    [TestMethod]
    public void NullableNotBeFalse_PassAndFail() {
        bool? yes = true;
        yes.Should().NotBeFalse();

        bool? missing = null;
        missing.Should().NotBeFalse();

        bool? no = false;
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => no.Should().NotBeFalse());
        Assert.AreEqual("Expected no not to be false, but found false.", ex.Message);
    }
}

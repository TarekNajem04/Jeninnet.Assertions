//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Tests;

[TestClass]
public class GuidTests {
    private static readonly Guid _fixed = new("11111111-1111-1111-1111-111111111111");

    [TestMethod]
    public void Be_PassAndFail() {
        _fixed.Should().Be(_fixed);

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => _fixed.Should().Be(Guid.Empty));
        Assert.AreEqual(
            "Expected _fixed to be 00000000-0000-0000-0000-000000000000, but found 11111111-1111-1111-1111-111111111111.",
            ex.Message);
    }

    [TestMethod]
    public void BeEmpty_NotBeEmpty() {
        Guid.Empty.Should().BeEmpty();
        _fixed.Should().NotBeEmpty();

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => Guid.Empty.Should().NotBeEmpty());
        Assert.AreEqual(
            "Expected Guid.Empty not to be empty, but found 00000000-0000-0000-0000-000000000000.",
            ex.Message);
    }

    [TestMethod]
    public void Nullable_Behaviors() {
        Guid? present = _fixed;
        present.Should().Be(_fixed).And.NotBeEmpty().And.HaveValue();

        Guid? missing = null;
        missing.Should().Be(null).And.BeNull();
        missing.Should().NotBe(_fixed).And.NotBeEmpty();

        var empty = Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().BeEmpty());
        Assert.AreEqual("Expected missing to be empty, but found <null>.", empty.Message);

        var notNull = Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().HaveValue());
        Assert.AreEqual("Expected missing to not be null, but found null.", notNull.Message);
    }

    [TestMethod]
    public void HaveValue_PassAndFail() {
        Guid? present = _fixed;
        present.Should().HaveValue();

        Guid? missing = null;
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().HaveValue());
        Assert.AreEqual("Expected missing to not be null, but found null.", ex.Message);
    }

    [TestMethod]
    public void NotHaveValue_PassAndFail() {
        Guid? missing = null;
        missing.Should().NotHaveValue();

        Guid? present = _fixed;
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => present.Should().NotHaveValue());
        Assert.AreEqual("Expected present to be null, but found 11111111-1111-1111-1111-111111111111.", ex.Message);
    }

    [TestMethod]
    public void BeFromText_PassAndFail() {
        _fixed.Should().Be("11111111-1111-1111-1111-111111111111");

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => _fixed.Should().Be("00000000-0000-0000-0000-000000000000"));
        Assert.AreEqual("Expected _fixed to be 00000000-0000-0000-0000-000000000000, but found 11111111-1111-1111-1111-111111111111.", ex.Message);
    }

    [TestMethod]
    public void NotBeFromText_PassAndFail() {
        _fixed.Should().NotBe("00000000-0000-0000-0000-000000000000");

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => _fixed.Should().NotBe("11111111-1111-1111-1111-111111111111"));
        Assert.AreEqual("Expected _fixed not to be 11111111-1111-1111-1111-111111111111, but found 11111111-1111-1111-1111-111111111111.", ex.Message);
    }

    [TestMethod]
    public void BeFromText_InvalidText_Throws() {
        Assert.ThrowsExactly<FormatException>(() => _fixed.Should().Be("not-a-guid"));
        Assert.ThrowsExactly<FormatException>(() => _fixed.Should().NotBe("not-a-guid"));

        const string? text = null;
        Assert.ThrowsExactly<ArgumentNullException>(() => _fixed.Should().Be(text));
        Assert.ThrowsExactly<ArgumentNullException>(() => _fixed.Should().NotBe(text));
    }
}

//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Tests;

public partial class CollectionAssertionsTests {
    [TestMethod]
    public void StringCollection_AllBe_PassAndFail() {
        IEnumerable<string> logs = ["ok", "ok"];
        logs.Should().AllBe("ok");

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => logs.Should().AllBe("fail"));
        Assert.AreEqual("Expected logs to have all items be 'fail', but found 'ok'.", ex.Message);

        IEnumerable<string>? missing = null;
        var nullEx = Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().AllBe("ok"));
        Assert.AreEqual("Expected missing to have all items be 'ok', but collection is null.", nullEx.Message);
    }

    [TestMethod]
    public void StringCollection_ContainMatch_PassFailAndWhich() {
        List<string> logs = ["build succeeded", "test failed"];
        logs.Should().ContainMatch("* failed").Which.Should().Be("test failed");
        logs.Should().NotContainMatch("* skipped");

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => logs.Should().ContainMatch("* skipped"));
        Assert.AreEqual("Expected logs to contain an item matching '* skipped', but none was found.", ex.Message);

        var notEx = Assert.ThrowsExactly<AssertionFailedException>(() => logs.Should().NotContainMatch("* failed"));
        Assert.AreEqual("Expected logs to contain no item matching '* failed', but found 'test failed'.", notEx.Message);

        Assert.ThrowsExactly<ArgumentNullException>(() => logs.Should().ContainMatch(null!));
    }

    [TestMethod]
    public void SingleElementAffixes_PassAndFail() {
        int[] values = [1, 2, 3];
        values.Should().StartWith(1);
        values.Should().EndWith(3);

        var startEx = Assert.ThrowsExactly<AssertionFailedException>(() => values.Should().StartWith(2));
        Assert.AreEqual("Expected values to start with ['2'], but item at index 0 was '1'.", startEx.Message);

        var endEx = Assert.ThrowsExactly<AssertionFailedException>(() => values.Should().EndWith(2));
        Assert.AreEqual("Expected values to end with ['2'], but item at index 2 was '3'.", endEx.Message);
    }

    [TestMethod]
    public void AffixesWithPredicate_CompareElementWise() {
        var actual = new[] { new DeliberatelyEqualItem("a"), new DeliberatelyEqualItem("b") };
        var head = new[] { "a" };
        var tail = new[] { "b" };
        actual.Should().StartWith(head, static (item, name) => item.Name == name);
        actual.Should().EndWith(tail, static (item, name) => item.Name == name);

        var missing = new[] { "z" };
        var ex = Assert.ThrowsExactly<AssertionFailedException>(
            () => actual.Should().StartWith(missing, static (item, name) => item.Name == name));
        Assert.AreEqual("Expected actual to start with ['z'], but item at index 0 was 'Jeninnet.Assertions.Tests.CollectionAssertionsTests+DeliberatelyEqualItem'.", ex.Message);
    }
}

//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.MSTest.Tests;

[TestClass]
public class AdapterTests {
    [TestMethod]
    public void That_PassThrough() {
        object subject = "actual";
        MSTestAssertions.That(() => subject.Should().Be("actual"));
    }

    [TestMethod]
    public void That_TranslatesFailure() {
        object subject = "actual";
        var ex = Assert.ThrowsExactly<AssertFailedException>(() => MSTestAssertions.That(() => subject.Should().Be("expected")));
        Assert.AreEqual("Expected subject to be 'expected', but got 'actual'.", ex.Message);
        Assert.IsNull(ex.InnerException);
    }

    [TestMethod]
    public void That_PreservesInnerException() {
        Action action = () => throw new InvalidOperationException("boom");
        var ex = Assert.ThrowsExactly<AssertFailedException>(() => MSTestAssertions.That(() => action.Should().NotThrow()));
        Assert.AreEqual("Expected action not to throw, but InvalidOperationException was thrown.", ex.Message);
        Assert.IsInstanceOfType<InvalidOperationException>(ex.InnerException);
    }

    [TestMethod]
    public void That_LeavesOtherExceptionsUntouched() {
        var ex = Assert.ThrowsExactly<InvalidOperationException>(() => MSTestAssertions.That(() => throw new InvalidOperationException("boom")));
        Assert.AreEqual("boom", ex.Message);
    }

    [TestMethod]
    public void That_NullAction_ThrowsArgumentNull() =>
        Assert.ThrowsExactly<ArgumentNullException>(() => MSTestAssertions.That(null!));

    [TestMethod]
    public async Task ThatAsync_PassThrough() {
        Func<Task> action = () => Task.CompletedTask;
        await MSTestAssertions.ThatAsync(() => action.Should().NotThrowAsync());
    }

    [TestMethod]
    public async Task ThatAsync_TranslatesFailure() {
        Func<Task> action = () => Task.FromException(new InvalidOperationException("boom"));
        var ex = await Assert.ThrowsExactlyAsync<AssertFailedException>(() => MSTestAssertions.ThatAsync(() => action.Should().NotThrowAsync()));
        Assert.AreEqual("Expected action not to throw, but InvalidOperationException was thrown.", ex.Message);
        Assert.IsInstanceOfType<InvalidOperationException>(ex.InnerException);
    }

    [TestMethod]
    public async Task ThatAsync_NullAction_ThrowsArgumentNull() =>
        await Assert.ThrowsExactlyAsync<ArgumentNullException>(() => MSTestAssertions.ThatAsync(null!));
}

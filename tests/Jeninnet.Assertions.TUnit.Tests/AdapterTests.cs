//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.TUnit.Tests;

[TestClass]
public class AdapterTests {
    [TestMethod]
    public void That_PassThrough() {
        object subject = "actual";
        TUnitAssertions.That(() => subject.Should().Be("actual"));
    }

    [TestMethod]
    public void That_TranslatesFailure() {
        object subject = "actual";
        var ex = MSTestAssert.ThrowsExactly<TUnitAssertionException>(() => TUnitAssertions.That(() => subject.Should().Be("expected")));
        MSTestAssert.AreEqual("Expected subject to be 'expected', but got 'actual'.", ex.Message);
        MSTestAssert.IsNull(ex.InnerException);
    }

    [TestMethod]
    public void That_PreservesInnerException() {
        Action action = () => throw new InvalidOperationException("boom");
        var ex = MSTestAssert.ThrowsExactly<TUnitAssertionException>(() => TUnitAssertions.That(() => action.Should().NotThrow()));
        MSTestAssert.AreEqual("Expected action not to throw, but InvalidOperationException was thrown.", ex.Message);
        MSTestAssert.IsInstanceOfType<InvalidOperationException>(ex.InnerException);
    }

    [TestMethod]
    public void That_LeavesOtherExceptionsUntouched() {
        var ex = MSTestAssert.ThrowsExactly<InvalidOperationException>(() => TUnitAssertions.That(() => throw new InvalidOperationException("boom")));
        MSTestAssert.AreEqual("boom", ex.Message);
    }

    [TestMethod]
    public void That_NullAction_ThrowsArgumentNull() =>
        MSTestAssert.ThrowsExactly<ArgumentNullException>(() => TUnitAssertions.That(null!));

    [TestMethod]
    public async Task ThatAsync_PassThrough() {
        Func<Task> action = () => Task.CompletedTask;
        await TUnitAssertions.ThatAsync(() => action.Should().NotThrowAsync());
    }

    [TestMethod]
    public async Task ThatAsync_TranslatesFailure() {
        Func<Task> action = () => Task.FromException(new InvalidOperationException("boom"));
        var ex = await MSTestAssert.ThrowsExactlyAsync<TUnitAssertionException>(() => TUnitAssertions.ThatAsync(() => action.Should().NotThrowAsync()));
        MSTestAssert.AreEqual("Expected action not to throw, but InvalidOperationException was thrown.", ex.Message);
        MSTestAssert.IsInstanceOfType<InvalidOperationException>(ex.InnerException);
    }

    [TestMethod]
    public async Task ThatAsync_NullAction_ThrowsArgumentNull() =>
        await MSTestAssert.ThrowsExactlyAsync<ArgumentNullException>(() => TUnitAssertions.ThatAsync(null!));
}

//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Tests;

[TestClass]
public class AssertionScopeTests {
    [TestMethod]
    public void Scope_PassingAssertions_DoNotThrow() {
        using(new AssertionScope()) {
            true.Should().BeTrue();
            "a".Should().Be("a");
        }
    }

    [TestMethod]
    public void Scope_BatchesMultipleFailures() {
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => {
            using(new AssertionScope()) {
                const bool flag = false;
                flag.Should().BeTrue();
                const string actual = "Actual";
                actual.Should().Be("Expected");
            }
        });

        Assert.Contains("2 assertion failures:", ex.Message);
        Assert.Contains("Expected flag to be true, but found false.", ex.Message);
        Assert.Contains("Expected actual to be 'Expected', but found 'Actual'.", ex.Message);
    }

    [TestMethod]
    public void Scope_SingleFailure_ThrowsOriginalMessage() {
        const bool flag = false;
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => {
            using(new AssertionScope()) {
                flag.Should().BeTrue();
            }
        });

        Assert.AreEqual("Expected flag to be true, but found false.", ex.Message);
    }

    [TestMethod]
    public void Scope_Discard_PreventsThrow() {
        string[] discarded;
        using(var scope = new AssertionScope()) {
            const bool flag = false;
            flag.Should().BeTrue();
            discarded = scope.Discard();
        }

        Assert.HasCount(1, discarded);
        Assert.AreEqual("Expected flag to be true, but found false.", discarded[0]);
    }

    [TestMethod]
    public void Scope_Nested_MergesWithContextPrefix() {
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => {
            using(new AssertionScope("Outer"))
            using(new AssertionScope("Inner")) {
                const bool flag = false;
                flag.Should().BeTrue();
            }
        });

        Assert.AreEqual("[Inner] Expected flag to be true, but found false.", ex.Message);
    }

    [TestMethod]
    public void Scope_ChainedAssertions_ContinueAfterFailure() {
        int[] values = [1];
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => {
            using(new AssertionScope()) {
                values.Should().HaveCount(5).And.Contain(1);
                values.Should().Contain(9);
            }
        });

        Assert.Contains("Expected values to contain 5 item(s), but found 1.", ex.Message);
        Assert.Contains("Expected values to contain '9', but it was not found.", ex.Message);
    }

    [TestMethod]
    public void Scope_NegativeAssertions_AreRecordedNotThrown() {
        int[] values = [1, 2, 3];
        const string empty = "";
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => {
            using(new AssertionScope()) {
                values.Should().NotHaveCount(3);
                values.Should().NotBeInAscendingOrder();
                empty.Should().NotBeNullOrEmpty();
            }
        });

        Assert.Contains("3 assertion failures:", ex.Message);
        Assert.Contains("Expected values not to contain 3 item(s), but found 3.", ex.Message);
        Assert.Contains("Expected values not to be in ascending order, but found 1 after 3 at index 0.", ex.Message);
        Assert.Contains("Expected empty not to be null or empty, but it was.", ex.Message);
    }

    [TestMethod]
    public void Scope_AddPreFormattedFailure_BatchesExternalFailure() {
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => {
            using var scope = new AssertionScope();
            scope.AddPreFormattedFailure("External failure.");
        });

        Assert.AreEqual("External failure.", ex.Message);
    }

    [TestMethod]
    public void Scope_AddPreFormattedFailure_Null_ThrowsArgumentNull() {
        using var scope = new AssertionScope();
        Assert.ThrowsExactly<ArgumentNullException>(() => scope.AddPreFormattedFailure(null!));
    }

    [TestMethod]
    public void Scope_AppendTracing_AppendedToSingleFailureMessage() {
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => {
            using var scope = new AssertionScope();
            scope.AppendTracing("trace line");
            const bool flag = false;
            flag.Should().BeTrue();
        });

        Assert.AreEqual("Expected flag to be true, but found false." + Environment.NewLine + "trace line", ex.Message);
    }

    [TestMethod]
    public void Scope_AppendTracing_WithoutFailures_DoesNotThrow() {
        using var scope = new AssertionScope();
        scope.AppendTracing("unused trace");
        Assert.IsFalse(scope.HasFailures());
    }

    [TestMethod]
    public void Scope_AppendTracing_Nested_MergesIntoTrace() {
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => {
            using(new AssertionScope())
            using(var inner = new AssertionScope("Inner")) {
                inner.AppendTracing("inner trace");
                const bool flag = false;
                flag.Should().BeTrue();
            }
        });

        Assert.AreEqual("[Inner] Expected flag to be true, but found false." + Environment.NewLine + "inner trace", ex.Message);
    }

    [TestMethod]
    public void Scope_AppendTracing_MultipleFailures_AppendsAfterList() {
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => {
            using var scope = new AssertionScope();
            scope.AppendTracing("dump");
            const bool flag = false;
            flag.Should().BeTrue();
            const string actual = "Actual";
            actual.Should().Be("Expected");
        });

        Assert.Contains("2 assertion failures:", ex.Message);
        Assert.IsTrue(ex.Message.EndsWith(Environment.NewLine + "dump", StringComparison.Ordinal));
    }

    [TestMethod]
    public void Scope_WithCustomStrategy_RoutesFailuresToIt() {
        var strategy = new RecordingAssertionStrategy();

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => {
            using(new AssertionScope(strategy)) {
                new List<int> { 1 }.Should().BeEmpty();
            }
        });

        Assert.HasCount(1, strategy.FailureMessages);
        Assert.Contains("Expected", strategy.FailureMessages.Single());
        Assert.Contains("to be empty, but found 1 item(s)", strategy.FailureMessages.Single());
        Assert.Contains(ex.Message, strategy.FailureMessages.Single());
    }

    [TestMethod]
    public void Scope_WithCustomStrategy_NestedScopes_MergeIntoStrategy() {
        var strategy = new RecordingAssertionStrategy();

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => {
            using(new AssertionScope(strategy)) {
                using(var inner = new AssertionScope("Inner")) {
                    const string actual = "Actual";
                    actual.Should().Be("Expected");
                }

                const bool flag = false;
                flag.Should().BeTrue();
            }
        });

        Assert.HasCount(2, strategy.FailureMessages);
        Assert.Contains(m => m.StartsWith("[Inner] Expected ", StringComparison.Ordinal), strategy.FailureMessages);
        Assert.Contains("Expected flag to be true", ex.Message);
    }

    [TestMethod]
    public void Scope_WithCustomStrategy_DiscardOnlyClearsThatStrategy() {
        var strategy = new RecordingAssertionStrategy();

        using(var scope = new AssertionScope(strategy)) {
            const bool flag = false;
            flag.Should().BeTrue();
            var discarded = scope.Discard();

            Assert.HasCount(1, discarded);
            Assert.IsFalse(scope.HasFailures());
        }

        Assert.IsEmpty(strategy.FailureMessages);
    }

    [TestMethod]
    public void Scope_WithNullStrategy_Throws() {
        IAssertionStrategy? strategy = null;
        AssertionScope act() => new(strategy!);

        Assert.ThrowsExactly<ArgumentNullException>(act);
    }

    private sealed class RecordingAssertionStrategy : IAssertionStrategy {
        private readonly List<string> _messages = [];

        public IEnumerable<string> FailureMessages => _messages;

        public void HandleFailure(string message) => _messages.Add(message);

        public IEnumerable<string> DiscardFailures() {
            var discarded = _messages.ToArray();
            _messages.Clear();
            return discarded;
        }

        public void ThrowIfAny(IDictionary<string, object> context) {
        }
    }
}

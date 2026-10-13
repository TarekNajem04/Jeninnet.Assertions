//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Tests;

[TestClass]
public class AssertionIsolationTests {
    [TestMethod]
    public void Culture_RestoresOnDispose() {
        var before = AssertionCulture.Current;
        try {
            AssertionCulture.Current = new CultureInfo("ar");

            using(AssertionIsolation.Isolate()) {
                AssertionCulture.Current = new CultureInfo("en");
                Assert.AreEqual("en", AssertionCulture.Current.Name);
            }

            Assert.AreEqual("ar", AssertionCulture.Current.Name);
        }
        finally {
            AssertionCulture.Current = before;
        }
    }

    [TestMethod]
    public void Clock_RestoresOnDispose() {
        var epoch = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);
        var original = new ManualClock(epoch);
        AssertionClock.Current = original;
        try {
            using(AssertionIsolation.Isolate()) {
                AssertionClock.Current = new ManualClock(epoch.AddDays(1));
                Assert.AreNotSame(original, AssertionClock.Current);
            }

            Assert.AreSame(original, AssertionClock.Current);
        }
        finally {
            AssertionClock.Current = SystemClock.Instance;
        }
    }

    /// <summary>
    /// Mutates the process-wide formatter registry; excluded from parallel execution so it cannot race other
    /// formatter tests (the registry itself is global by design).
    /// </summary>
    [TestMethod, DoNotParallelize]
    public void Formatters_RestoreTheProcessWideRegistry() {
        var first = new WidgetFormatter();
        AssertionConfiguration.Formatting.AddFormatter(first);
        try {
            var beforeEx = Assert.ThrowsExactly<AssertionFailedException>(() => new Widget("a").Should().Be(new Widget("b")));
            Assert.Contains("W[b]", beforeEx.Message);

            using(AssertionIsolation.Isolate()) {
                AssertionConfiguration.Formatting.AddFormatter(new GadgetFormatter());

                var insideEx = Assert.ThrowsExactly<AssertionFailedException>(() => new Gadget("a").Should().Be(new Gadget("b")));
                Assert.Contains("G[b]", insideEx.Message);

                var widgetEx = Assert.ThrowsExactly<AssertionFailedException>(() => new Widget("a").Should().Be(new Widget("b")));
                Assert.Contains("W[b]", widgetEx.Message);
            }

            var afterEx = Assert.ThrowsExactly<AssertionFailedException>(() => new Gadget("a").Should().Be(new Gadget("b")));
            Assert.IsFalse(afterEx.Message.Contains("G[b]", StringComparison.Ordinal));

            var keptEx = Assert.ThrowsExactly<AssertionFailedException>(() => new Widget("a").Should().Be(new Widget("b")));
            Assert.Contains("W[b]", keptEx.Message);
        }
        finally {
            AssertionConfiguration.Formatting.RemoveFormatter(first);
        }
    }

    [TestMethod]
    public void EquivalencyAmbient_RestoresOnDispose() {
        AssertionConfiguration.Equivalency.Modify(static o => o.StrictOrdering = true);
        try {
            var actual = new Basket(["b", "a"]);
            var expected = new Basket(["a", "b"]);
            Assert.ThrowsExactly<AssertionFailedException>(() => actual.Should().BeEquivalentTo(expected));

            using(AssertionIsolation.Isolate()) {
                EquivalencyDefaults.ResetToDefaults();
                actual.Should().BeEquivalentTo(expected);
            }

            Assert.ThrowsExactly<AssertionFailedException>(() => actual.Should().BeEquivalentTo(expected));
        }
        finally {
            EquivalencyDefaults.ResetToDefaults();
        }
    }

    [TestMethod]
    public void NestedScopes_RestoreTheirOwnSnapshots() {
        var epoch = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);
        var outer = new ManualClock(epoch.AddHours(1));
        var middle = new ManualClock(epoch.AddHours(2));
        var inner = new ManualClock(epoch.AddHours(3));
        AssertionClock.Current = outer;
        try {
            using(AssertionIsolation.Isolate()) {
                AssertionClock.Current = middle;

                using(AssertionIsolation.Isolate()) {
                    AssertionClock.Current = inner;
                    Assert.AreSame(inner, AssertionClock.Current);
                }

                Assert.AreSame(middle, AssertionClock.Current);
            }

            Assert.AreSame(outer, AssertionClock.Current);
        }
        finally {
            AssertionClock.Current = SystemClock.Instance;
        }
    }

    [TestMethod]
    public void ExceptionInsideTheScope_StillRestores() {
        var before = AssertionCulture.Current;
        try {
            AssertionCulture.Current = new CultureInfo("ar");

            Assert.ThrowsExactly<InvalidOperationException>(() => {
                using(AssertionIsolation.Isolate()) {
                    AssertionCulture.Current = new CultureInfo("en");
                    throw new InvalidOperationException("boom");
                }
            });

            Assert.AreEqual("ar", AssertionCulture.Current.Name);
        }
        finally {
            AssertionCulture.Current = before;
        }
    }

    [TestMethod]
    public async Task Isolation_IsPerAsyncFlow() {
        var before = AssertionCulture.Current;
        try {
            AssertionCulture.Current = new CultureInfo("ar");

            using(AssertionIsolation.Isolate()) {
                AssertionCulture.Current = new CultureInfo("en");

                var inherited = await Task.Run(() => AssertionCulture.Current);
                var replaced = await Task.Run(() => {
                    AssertionCulture.Current = new CultureInfo("fr");
                    return AssertionCulture.Current;
                });

                Assert.AreEqual("en", inherited.Name);
                Assert.AreEqual("fr", replaced.Name);
                Assert.AreEqual("en", AssertionCulture.Current.Name);
            }

            Assert.AreEqual("ar", AssertionCulture.Current.Name);
        }
        finally {
            AssertionCulture.Current = before;
        }
    }

    private sealed class Basket(List<string> tags) {
        public List<string> Tags { get; } = tags;
    }

    private sealed class Widget(string label) {
        public string Label { get; } = label;
    }

    private sealed class WidgetFormatter : IValueFormatter {
        public bool CanFormat(object? value) => value is Widget;

        public string? Format(object? value) => value is Widget widget ? $"W[{widget.Label}]" : null;
    }

    private sealed class Gadget(string label) {
        public string Label { get; } = label;
    }

    private sealed class GadgetFormatter : IValueFormatter {
        public bool CanFormat(object? value) => value is Gadget;

        public string? Format(object? value) => value is Gadget gadget ? $"G[{gadget.Label}]" : null;
    }
}

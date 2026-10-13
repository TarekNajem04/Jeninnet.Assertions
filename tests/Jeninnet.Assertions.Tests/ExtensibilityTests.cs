//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Tests;

[TestClass]
public class ExtensibilityTests {
    [TestMethod]
    public void ScopedFormatter_UsedInsideScope() {
        object subject = new Widget("a");
        object expected = new Widget("b");

        using(var scope = new AssertionScope()) {
            scope.Formatting.AddFormatter(new WidgetFormatter());
            subject.Should().Be(expected);
            var failures = scope.Discard();
            Assert.HasCount(1, failures);
            Assert.AreEqual("Expected subject to be 'W[b]', but got 'W[a]'.", failures[0]);

            using var inner = new AssertionScope();
            subject.Should().Be(expected);
            var nested = inner.Discard();
            Assert.HasCount(1, nested);
            Assert.AreEqual("Expected subject to be 'W[b]', but got 'W[a]'.", nested[0]);
        }

        var plain = Assert.ThrowsExactly<AssertionFailedException>(() => subject.Should().Be(expected));
        Assert.Contains("Widget", plain.Message);
    }

    [TestMethod]
    public void GlobalFormatter_UsedAndRemovable() {
        var formatter = new GadgetFormatter();
        AssertionConfiguration.Formatting.AddFormatter(formatter);
        try {
            object subject = new Gadget("a");
            object expected = new Gadget("b");
            var ex = Assert.ThrowsExactly<AssertionFailedException>(() => subject.Should().Be(expected));
            Assert.AreEqual("Expected subject to be 'G[b]', but got 'G[a]'.", ex.Message);
        }
        finally {
            Assert.IsTrue(AssertionConfiguration.Formatting.RemoveFormatter(formatter));
        }
    }

    /// <summary>
    /// Mutates process-wide configuration; excluded from parallel execution so it cannot race other formatter tests.
    /// </summary>
    [TestMethod, DoNotParallelize]
    public void GlobalFormatter_RemovedByType() {
        AssertionConfiguration.Formatting.AddFormatter(new DoohickeyFormatter());
        try {
            object subject = new Doohickey("a");
            object expected = new Doohickey("b");
            var active = Assert.ThrowsExactly<AssertionFailedException>(() => subject.Should().Be(expected));
            Assert.AreEqual("Expected subject to be 'D[b]', but got 'D[a]'.", active.Message);

            Assert.IsTrue(AssertionConfiguration.Formatting.RemoveFormatter<DoohickeyFormatter>());

            var plain = Assert.ThrowsExactly<AssertionFailedException>(() => subject.Should().Be(expected));
            Assert.Contains("Doohickey", plain.Message);
        }
        finally {
            AssertionConfiguration.Formatting.RemoveFormatter<DoohickeyFormatter>();
        }
    }

    [TestMethod]
    public void ValueFormatter_ContractIsExactlyCanFormatAndFormat() {
        var methods = typeof(IValueFormatter).GetMethods();
        Assert.HasCount(2, methods);

        var canFormat = typeof(IValueFormatter).GetMethod(nameof(IValueFormatter.CanFormat))!;
        Assert.AreEqual(typeof(bool), canFormat.ReturnType);
        var canFormatParameters = canFormat.GetParameters();
        Assert.HasCount(1, canFormatParameters);
        Assert.AreEqual(typeof(object), canFormatParameters[0].ParameterType);

        var format = typeof(IValueFormatter).GetMethod(nameof(IValueFormatter.Format))!;
        Assert.AreEqual(typeof(string), format.ReturnType);
        var formatParameters = format.GetParameters();
        Assert.HasCount(1, formatParameters);
        Assert.AreEqual(typeof(object), formatParameters[0].ParameterType);
    }

    [TestMethod]
    public void EngineInitializer_RunsBeforeAssertions() {
        Assert.ThrowsExactly<AssertionFailedException>(() => false.Should().BeTrue());
        Assert.IsTrue(TestInitializer.Ran);
    }

    /// <summary>
    /// Mutates process-wide configuration; excluded from parallel execution so it cannot race the default-value test below.
    /// </summary>
    [TestMethod, DoNotParallelize]
    public void StrictOrderingDefault_ChangesCollectionEquivalency() {
        AssertionConfiguration.Equivalency.StrictOrderingDefault = true;
        try {
            int[] values = [1, 2, 3];
            var ex = Assert.ThrowsExactly<AssertionFailedException>(() => values.Should().BeEquivalentTo(3, 2, 1));
            Assert.AreEqual("Expected values to be equivalent in the same order, but item at index 0 was '1' instead of '3'.", ex.Message);
        }
        finally {
            AssertionConfiguration.Equivalency.StrictOrderingDefault = false;
        }
    }

    /// <summary>
    /// Reads process-wide configuration; excluded from parallel execution so it cannot race the mutating test above.
    /// </summary>
    [TestMethod, DoNotParallelize]
    public void StrictOrderingDefault_IsFalseByDefault() =>
        Assert.IsFalse(AssertionConfiguration.Equivalency.StrictOrderingDefault);

    [TestMethod]
    public void CustomAssertion_MarksExtensionMethod() {
        var attribute = typeof(ExtensibilityTests).GetMethod(nameof(SampleExtension))!
            .GetCustomAttributes(typeof(CustomAssertionAttribute), inherit: false);
        Assert.HasCount(1, attribute);
    }

    [CustomAssertion]
    public static void SampleExtension() {
        // Marker-only fixture: the attribute presence is the assertion under test.
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

    private sealed class Doohickey(string label) {
        public string Label { get; } = label;
    }

    private sealed class DoohickeyFormatter : IValueFormatter {
        public bool CanFormat(object? value) => value is Doohickey;

        public string? Format(object? value) => value is Doohickey doohickey ? $"D[{doohickey.Label}]" : null;
    }
}

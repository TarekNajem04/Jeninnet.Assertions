//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Tests;

[TestClass]
public class IndependenceTests {
    [TestMethod]
    public void Library_DoesNotReferenceJeninnetLocalization() {
        var assembly = typeof(AssertionExtensions).Assembly;
        var referenced = assembly.GetReferencedAssemblies().Select(a => a.Name).ToArray();

        Assert.IsNotEmpty(referenced);
        Assert.DoesNotContain("Jeninnet.Localization", referenced);
        Assert.DoesNotContain("Jeninnet.Localization.Abstractions", referenced);
        Assert.DoesNotContain("Jeninnet.Localization.Core", referenced);
        Assert.DoesNotContain("Jeninnet.Localization.Storage", referenced);
    }

    [TestMethod]
    public void Library_IsStandaloneProductAssembly() {
        var assembly = typeof(AssertionExtensions).Assembly;
        var typeNames = assembly.GetTypes().Select(t => t.Name);

        Assert.AreEqual("Jeninnet.Assertions", assembly.GetName().Name);
        Assert.Contains(nameof(AssertionFailedException), typeNames);
        Assert.Contains(nameof(AssertionScope), typeNames);
        Assert.Contains("BoolAssertions", typeNames);
        Assert.Contains("StringAssertions", typeNames);
        Assert.Contains("CollectionAssertions`1", typeNames);
        Assert.Contains("ObjectAssertions`1", typeNames);
        Assert.Contains("ActionAssertions", typeNames);
        Assert.Contains("ActionAssertions`1", typeNames);
        Assert.Contains("AsyncActionAssertions", typeNames);
        Assert.Contains("ExceptionAssertions`1", typeNames);
        Assert.Contains("AndConstraint`1", typeNames);
        Assert.Contains("WhichConstraint`1", typeNames);
        Assert.Contains("NullableBooleanAssertions", typeNames);
        Assert.Contains("OccurrenceConstraint", typeNames);
        Assert.Contains(nameof(Exactly), typeNames);
        Assert.Contains(nameof(AtLeast), typeNames);
        Assert.Contains(nameof(AtMost), typeNames);
        Assert.Contains(nameof(MoreThan), typeNames);
        Assert.Contains(nameof(LessThan), typeNames);
        Assert.Contains("OccurrenceExtensions", typeNames);
        Assert.Contains("StringEquivalencyOptions", typeNames);
        Assert.Contains("ComparableTypeAssertions`1", typeNames);
        Assert.Contains("NumericAssertions`1", typeNames);
        Assert.Contains("NullableNumericAssertions`1", typeNames);
        Assert.Contains("GenericDictionaryAssertions`2", typeNames);
        Assert.Contains("SpanAssertions`1", typeNames);
        Assert.Contains("CollectionEquivalencyOptions`1", typeNames);
        Assert.Contains("SelfReferenceEquivalencyOptions`1", typeNames);
        Assert.Contains("DateTimeAssertions", typeNames);
        Assert.Contains("NullableDateTimeAssertions", typeNames);
        Assert.Contains("DateOnlyAssertions", typeNames);
        Assert.Contains("NullableDateOnlyAssertions", typeNames);
        Assert.Contains("TimeOnlyAssertions", typeNames);
        Assert.Contains("NullableTimeOnlyAssertions", typeNames);
        Assert.Contains("DateTimeOffsetAssertions", typeNames);
        Assert.Contains("NullableDateTimeOffsetAssertions", typeNames);
        Assert.Contains("TimeSpanAssertions", typeNames);
        Assert.Contains("NullableTimeSpanAssertions", typeNames);
        Assert.Contains("GuidAssertions", typeNames);
        Assert.Contains("NullableGuidAssertions", typeNames);
        Assert.Contains("EnumAssertions`1", typeNames);
        Assert.Contains("NullableEnumAssertions`1", typeNames);
        Assert.Contains("TypeAssertions", typeNames);
        Assert.Contains("AssemblyAssertions", typeNames);
        Assert.Contains("MethodInfoAssertions", typeNames);
        Assert.Contains("PropertyInfoAssertions", typeNames);
        Assert.Contains("ConstructorInfoAssertions", typeNames);
        Assert.Contains("AllTypes", typeNames);
        Assert.Contains("TypeSelector", typeNames);
        Assert.Contains("TypeSelectorAssertions", typeNames);
        Assert.Contains("StreamAssertions", typeNames);
        Assert.Contains("BufferedStreamAssertions", typeNames);
        Assert.Contains("XDocumentAssertions", typeNames);
        Assert.Contains("XElementAssertions", typeNames);
        Assert.Contains("XAttributeAssertions", typeNames);
        Assert.Contains("XmlNodeAssertions", typeNames);
        Assert.Contains("EventMonitor", typeNames);
        Assert.Contains("EventMonitorAssertions", typeNames);
        Assert.Contains("OccurredEvent", typeNames);
        Assert.Contains("IValueFormatter", typeNames);
        Assert.Contains("ValueFormatterOptions", typeNames);
        Assert.Contains("AssertionConfiguration", typeNames);
        Assert.Contains("EquivalencyDefaults", typeNames);
        Assert.Contains("SelfReferenceEquivalencyOptions", typeNames);
        Assert.Contains("Comparands", typeNames);
        Assert.Contains("IEquivalencyStep", typeNames);
        Assert.Contains("EquivalencyStep", typeNames);
        Assert.Contains("IObjectGraph", typeNames);
        Assert.Contains("IEquivalencyStepContext", typeNames);
        Assert.Contains("IEquivalencyValidationContext", typeNames);
        Assert.Contains("EquivalencyPlan", typeNames);
        Assert.Contains("IMemberSelectionRule", typeNames);
        Assert.Contains("IMemberMatchingRule", typeNames);
        Assert.Contains("IOrderingRule", typeNames);
        Assert.Contains("ITraceWriter", typeNames);
        Assert.Contains("StringBuilderTraceWriter", typeNames);
        Assert.Contains("MemberTypePreference", typeNames);
        Assert.Contains("EnumComparison", typeNames);
        Assert.Contains(nameof(Value), typeNames);
        Assert.Contains("CustomAssertionDiscovery", typeNames);
        Assert.Contains("AssertionEngineInitializerAttribute", typeNames);
        Assert.Contains("CustomAssertionAttribute", typeNames);
        Assert.Contains("CustomAssertionsAssemblyAttribute", typeNames);
        Assert.Contains("ByteSizeExtensions", typeNames);
        Assert.Contains("GenericAsyncFunctionAssertions`1", typeNames);
        Assert.Contains("TaskCompletionSourceAssertions", typeNames);
        Assert.Contains("TaskCompletionSourceAssertions`1", typeNames);
        Assert.Contains("ExecutionTimeAssertions", typeNames);
        Assert.Contains("JsonNodeAssertions", typeNames);
    }
}

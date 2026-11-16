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

        referenced.Should().NotBeEmpty();
        referenced.Should().NotContain("Jeninnet.Localization");
        referenced.Should().NotContain("Jeninnet.Localization.Abstractions");
        referenced.Should().NotContain("Jeninnet.Localization.Core");
        referenced.Should().NotContain("Jeninnet.Localization.Storage");
    }

    [TestMethod]
    public void Library_IsStandaloneProductAssembly() {
        var assembly = typeof(AssertionExtensions).Assembly;
        var typeNames = assembly.GetTypes().Select(t => t.Name);

        assembly.GetName().Name.Should().Be("Jeninnet.Assertions");
        typeNames.Should().Contain(nameof(AssertionFailedException));
        typeNames.Should().Contain(nameof(AssertionScope));
        typeNames.Should().Contain("BoolAssertions");
        typeNames.Should().Contain("StringAssertions");
        typeNames.Should().Contain("CollectionAssertions`1");
        typeNames.Should().Contain("ObjectAssertions`1");
        typeNames.Should().Contain("ActionAssertions");
        typeNames.Should().Contain("ActionAssertions`1");
        typeNames.Should().Contain("AsyncActionAssertions");
        typeNames.Should().Contain("ExceptionAssertions`1");
        typeNames.Should().Contain("AndConstraint`1");
        typeNames.Should().Contain("WhichConstraint`1");
        typeNames.Should().Contain("NullableBooleanAssertions");
        typeNames.Should().Contain("OccurrenceConstraint");
        typeNames.Should().Contain(nameof(Exactly));
        typeNames.Should().Contain(nameof(AtLeast));
        typeNames.Should().Contain(nameof(AtMost));
        typeNames.Should().Contain(nameof(MoreThan));
        typeNames.Should().Contain(nameof(LessThan));
        typeNames.Should().Contain("OccurrenceExtensions");
        typeNames.Should().Contain("StringEquivalencyOptions");
        typeNames.Should().Contain("ComparableTypeAssertions`1");
        typeNames.Should().Contain("NumericAssertions`1");
        typeNames.Should().Contain("NullableNumericAssertions`1");
        typeNames.Should().Contain("GenericDictionaryAssertions`2");
        typeNames.Should().Contain("SpanAssertions`1");
        typeNames.Should().Contain("CollectionEquivalencyOptions`1");
        typeNames.Should().Contain("SelfReferenceEquivalencyOptions`1");
        typeNames.Should().Contain("DateTimeAssertions");
        typeNames.Should().Contain("NullableDateTimeAssertions");
        typeNames.Should().Contain("DateOnlyAssertions");
        typeNames.Should().Contain("NullableDateOnlyAssertions");
        typeNames.Should().Contain("TimeOnlyAssertions");
        typeNames.Should().Contain("NullableTimeOnlyAssertions");
        typeNames.Should().Contain("DateTimeOffsetAssertions");
        typeNames.Should().Contain("NullableDateTimeOffsetAssertions");
        typeNames.Should().Contain("TimeSpanAssertions");
        typeNames.Should().Contain("NullableTimeSpanAssertions");
        typeNames.Should().Contain("GuidAssertions");
        typeNames.Should().Contain("NullableGuidAssertions");
        typeNames.Should().Contain("EnumAssertions`1");
        typeNames.Should().Contain("NullableEnumAssertions`1");
        typeNames.Should().Contain("TypeAssertions");
        typeNames.Should().Contain("AssemblyAssertions");
        typeNames.Should().Contain("MethodInfoAssertions");
        typeNames.Should().Contain("PropertyInfoAssertions");
        typeNames.Should().Contain("ConstructorInfoAssertions");
        typeNames.Should().Contain("AllTypes");
        typeNames.Should().Contain("TypeSelector");
        typeNames.Should().Contain("TypeSelectorAssertions");
        typeNames.Should().Contain("StreamAssertions");
        typeNames.Should().Contain("BufferedStreamAssertions");
        typeNames.Should().Contain("XDocumentAssertions");
        typeNames.Should().Contain("XElementAssertions");
        typeNames.Should().Contain("XAttributeAssertions");
        typeNames.Should().Contain("XmlNodeAssertions");
        typeNames.Should().Contain("EventMonitor");
        typeNames.Should().Contain("EventMonitorAssertions");
        typeNames.Should().Contain("OccurredEvent");
        typeNames.Should().Contain("IValueFormatter");
        typeNames.Should().Contain("ValueFormatterOptions");
        typeNames.Should().Contain("AssertionConfiguration");
        typeNames.Should().Contain("EquivalencyDefaults");
        typeNames.Should().Contain("SelfReferenceEquivalencyOptions");
        typeNames.Should().Contain("Comparands");
        typeNames.Should().Contain("IEquivalencyStep");
        typeNames.Should().Contain("EquivalencyStep");
        typeNames.Should().Contain("IObjectGraph");
        typeNames.Should().Contain("IEquivalencyStepContext");
        typeNames.Should().Contain("IEquivalencyValidationContext");
        typeNames.Should().Contain("EquivalencyPlan");
        typeNames.Should().Contain("IMemberSelectionRule");
        typeNames.Should().Contain("IMemberMatchingRule");
        typeNames.Should().Contain("IOrderingRule");
        typeNames.Should().Contain("ITraceWriter");
        typeNames.Should().Contain("StringBuilderTraceWriter");
        typeNames.Should().Contain("MemberTypePreference");
        typeNames.Should().Contain("EnumComparison");
        typeNames.Should().Contain(nameof(Value));
        typeNames.Should().Contain("CustomAssertionDiscovery");
        typeNames.Should().Contain("AssertionEngineInitializerAttribute");
        typeNames.Should().Contain("CustomAssertionAttribute");
        typeNames.Should().Contain("CustomAssertionsAssemblyAttribute");
        typeNames.Should().Contain("ByteSizeExtensions");
        typeNames.Should().Contain("GenericAsyncFunctionAssertions`1");
        typeNames.Should().Contain("TaskCompletionSourceAssertions");
        typeNames.Should().Contain("TaskCompletionSourceAssertions`1");
        typeNames.Should().Contain("ExecutionTimeAssertions");
        typeNames.Should().Contain("JsonNodeAssertions");
    }
}

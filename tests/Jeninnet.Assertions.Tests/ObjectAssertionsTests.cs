//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Tests;

[TestClass]
public class ObjectAssertionsTests {
    [TestMethod]
    public void Be_PassForEqualValue() {
        object value = 42;
        value.Should().Be(42);
    }

    [TestMethod]
    public void Be_FailsForDifferentValue() {
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => new object().Should().Be(new object()));
        Assert.IsFalse(string.IsNullOrEmpty(ex.Message));
    }

    [TestMethod]
    public void Be_UsesDefaultEqualityComparer() {
        object value = "a";
        value.Should().Be("a");
    }

    [TestMethod]
    public void Be_PassForNullSubjectAndNullExpected() => ((object?)null).Should().Be(null);

    [TestMethod]
    public void BeOfType_OnNullSubject_ReportsNull() {
        object? subject = null;
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => subject.Should().BeOfType<string>());
        Assert.AreEqual("Expected subject to be of type String, but found null.", ex.Message);
    }

    [TestMethod]
    public void BeOfType_PassForMatchingRuntimeType() => ((object)"text").Should().BeOfType<string>();

    [TestMethod]
    public void BeOfType_FailsForMismatchedRuntimeType() {
        object subject = 42;
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => subject.Should().BeOfType<string>());
        Assert.AreEqual("Expected subject to be of type String, but found Int32.", ex.Message);
    }

    [TestMethod]
    public void BeOfType_TypeSubject_MatchesOwnType() {
        object subject = typeof(CancellationToken);
        subject.Should().BeOfType<CancellationToken>();
    }

    [TestMethod]
    public void BeOfType_TypeSubject_FailsForDifferentType() {
        object subject = typeof(string);
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => subject.Should().BeOfType<int>());
        Assert.AreEqual("Expected subject to be of type Int32, but found String.", ex.Message);
    }

    [TestMethod]
    public void BeNull_PassForNull() => ((object?)null).Should().BeNull();

    [TestMethod]
    public void BeNull_FailsForValue() {
        object subject = new();
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => subject.Should().BeNull());
        Assert.AreEqual("Expected subject to be null, but it was not null.", ex.Message);
    }

    [TestMethod]
    public void NotBeNull_PassForValue() => new object().Should().NotBeNull();

    [TestMethod]
    public void NotBeNull_FailsForNull() {
        object? subject = null;
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => subject.Should().NotBeNull());
        Assert.AreEqual("Expected subject to not be null, but found null.", ex.Message);
    }

    [TestMethod]
    public void BeOfType_PassForExactType() {
        object subject = new MemoryStream();
        subject.Should().BeOfType<MemoryStream>();
    }

    [TestMethod]
    public void BeOfType_FailsForBaseType() {
        object subject = new MemoryStream();
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => subject.Should().BeOfType<Stream>());
        Assert.AreEqual("Expected subject to be of type Stream, but found MemoryStream.", ex.Message);
    }

    [TestMethod]
    public void BeOfType_PassForObject() {
        object subject = new();
        subject.Should().BeOfType<object>();
    }

    [TestMethod]
    public void BeOfType_FailsForUnrelatedType() {
        object subject = "text";
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => subject.Should().BeOfType<int>());
        Assert.AreEqual("Expected subject to be of type Int32, but found String.", ex.Message);
    }

    [TestMethod]
    public void BeOfType_FailsForNullSubject() {
        object? subject = null;
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => subject.Should().BeOfType<object>());
        Assert.AreEqual("Expected subject to be of type Object, but found null.", ex.Message);
    }

    [TestMethod]
    public void NotBeEquivalentTo_PassForDifferentGraphs() {
        var subject = new { Name = "a", Value = 1 };
        subject.Should().NotBeEquivalentTo(new { Name = "b", Value = 2 });
    }

    [TestMethod]
    public void NotBeEquivalentTo_FailsForEquivalentGraphs() {
        var subject = new { Name = "a", Value = 1 };
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => subject.Should().NotBeEquivalentTo(new { Name = "a", Value = 1 }));
        Assert.AreEqual("Expected subject not to be equivalent to { Name = a, Value = 1 }, but found differences.", ex.Message);
    }

    [TestMethod]
    public void As_PassForCompatibleCast() {
        object subject = "text";
        Assert.AreEqual("text", subject.As<string>());
    }

    [TestMethod]
    public void As_FailsForIncompatibleCast() {
        object subject = 42;
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => subject.As<string>());
        Assert.AreEqual("Expected subject to be castable to String, but found Int32.", ex.Message);
    }

    [TestMethod]
    public void Be_WithComparer_PassAndFail() {
        object subject = "a";
        subject.Should().Be("A", StringComparer.OrdinalIgnoreCase);

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => subject.Should().Be("b", StringComparer.OrdinalIgnoreCase));
        Assert.AreEqual("Expected subject to be 'b', but got 'a'.", ex.Message);
    }

    [TestMethod]
    public void Be_WithComparer_EqualsMatchButComparerMismatch_Fails() {
        // Default equality says equal, the comparer disagrees: the comparer wins.
        object subject = "a";
        var comparerEx = Assert.ThrowsExactly<AssertionFailedException>(() => subject.Should().Be("a", NeverEqualStrings.Instance));
        Assert.AreEqual("Expected subject to be 'a', but got 'a'.", comparerEx.Message);
    }

    [TestMethod]
    public void NotBe_WithComparer_PassAndFail() {
        object subject = "a";
        subject.Should().NotBe("b", StringComparer.OrdinalIgnoreCase);

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => subject.Should().NotBe("A", StringComparer.OrdinalIgnoreCase));
        Assert.AreEqual("Expected subject not to be 'A', but found 'a'.", ex.Message);
    }

    [TestMethod]
    public void BeOneOf_WithComparer_PassAndFail() {
        object subject = "a";
        subject.Should().BeOneOf(["x", "A"], StringComparer.OrdinalIgnoreCase);

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => subject.Should().BeOneOf(["x", "y"], StringComparer.OrdinalIgnoreCase));
        Assert.AreEqual("Expected subject to be one of x, y, but found a.", ex.Message);
    }

    [TestMethod]
    public void BeOneOf_WithoutComparer_UsesDefaultEquality() {
        object number = 42;
        number.Should().BeOneOf([41, 42, 43]);

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => number.Should().BeOneOf([1, 2]));
        Assert.AreEqual("Expected number to be one of 1, 2, but found 42.", ex.Message);

        Assert.ThrowsExactly<ArgumentNullException>(() => number.Should().BeOneOf((IEnumerable<object>)null!));
    }

    [TestMethod]
    public void ComparerParameters_AreAlwaysGeneric() {
        // Docs promise a non-generic IEqualityComparer is rejected at compile time: no public
        // signature may therefore mention the non-generic comparer interfaces at all.
        var offenders = new List<string>();
        foreach(var type in typeof(ObjectAssertions<object>).Assembly.GetTypes()) {
            if(!type.IsPublic && !type.IsNestedPublic) {
                continue;
            }

            offenders.AddRange(FindOffendingParameters(type));
            offenders.AddRange(FindOffendingProperties(type));
        }

        Assert.IsEmpty(offenders);
    }

    private const BindingFlags DECLARED_PUBLIC_FLAGS = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;

    private static IEnumerable<string> FindOffendingParameters(Type type) {
        foreach(var method in type.GetMethods(DECLARED_PUBLIC_FLAGS)) {
            foreach(var parameter in method.GetParameters()) {
                if(IsNonGenericComparer(parameter.ParameterType)) {
                    yield return $"{type.FullName}.{method.Name}({parameter.Name})";
                }
            }
        }
    }

    private static IEnumerable<string> FindOffendingProperties(Type type) {
        foreach(var property in type.GetProperties(DECLARED_PUBLIC_FLAGS)) {
            if(IsNonGenericComparer(property.PropertyType)) {
                yield return $"{type.FullName}.{property.Name}";
            }
        }
    }

    private static bool IsNonGenericComparer(Type type) =>
        type == typeof(System.Collections.IEqualityComparer) || type == typeof(System.Collections.IComparer);

    [TestMethod]
    public void Comparer_NullSelectsDefaultEquality() {
        object subject = "a";
        subject.Should().Be("a", null);
        subject.Should().NotBe("b", null);
        subject.Should().BeOneOf<string>(["a"], null);
    }

    [TestMethod]
    public void Be_WithComparer_CustomReferenceType() {
        var subject = new Product("a");
        subject.Should().Be(new Product("A"), SkuComparer.Instance);

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => subject.Should().Be(new Product("b"), SkuComparer.Instance));
        Assert.AreEqual("Expected subject to be 'b', but got 'a'.", ex.Message);
    }

    [TestMethod]
    public void BeXmlSerializable_PassFailAndUnserializable() {
        var point = new SerializablePoint { X = 1, Y = 2 };
        point.Should().BeXmlSerializable();

        var dictionary = new DictionaryPoint();
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => dictionary.Should().BeXmlSerializable());
        Assert.Contains("Expected dictionary to be XML-serializable, but serialization failed", ex.Message);
    }

    [TestMethod]
    public void BeJsonSerializable_PassAndLossyFail() {
        var point = new SerializablePoint { X = 1, Y = 2 };
        point.Should().BeJsonSerializable();

        // System.Text.Json deserializes object members as JsonElement, so type fidelity is lost.
        var lossy = new OpaquePoint { Anything = 5 };
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => lossy.Should().BeJsonSerializable());
        Assert.Contains("Expected lossy to be JSON-serializable, but the round-trip differed", ex.Message);
    }

    [TestMethod]
    public void BeDataContractSerializable_PassAndFail() {
        var contract = new DataPoint { X = 1 };
        contract.Should().BeDataContractSerializable();

        var opaque = new OpaquePoint { Anything = new object() };
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => opaque.Should().BeDataContractSerializable());
        Assert.Contains("Expected opaque to be data-contract-serializable", ex.Message);
    }

    public class SerializablePoint {
        public int X { get; set; }

        public int Y { get; set; }
    }

    public class OpaquePoint {
        public object? Anything { get; set; }
    }

    public class DictionaryPoint {
        public Dictionary<string, int> Map { get; set; } = new() { ["a"] = 1 };
    }

    [System.Runtime.Serialization.DataContract]
    public class DataPoint {
        [System.Runtime.Serialization.DataMember]
        public int X { get; set; }
    }

    private sealed class NeverEqualStrings : IEqualityComparer<string> {
        internal static readonly NeverEqualStrings Instance = new();

        public bool Equals(string? x, string? y) => false;

        public int GetHashCode(string obj) => 0;
    }

    private sealed class Product(string sku) {
        public string Sku { get; } = sku;

        public override string ToString() => Sku;
    }

    private sealed class SkuComparer : IEqualityComparer<Product> {
        internal static readonly SkuComparer Instance = new();

        public bool Equals(Product? x, Product? y) =>
            string.Equals(x?.Sku, y?.Sku, StringComparison.OrdinalIgnoreCase);

        public int GetHashCode(Product obj) => StringComparer.OrdinalIgnoreCase.GetHashCode(obj.Sku);
    }
}

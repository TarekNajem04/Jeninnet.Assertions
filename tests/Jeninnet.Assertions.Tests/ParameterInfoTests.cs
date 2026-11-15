//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Tests;

[TestClass]
public class ParameterInfoTests {
    [TestMethod]
    public void Should_ReturnsParameterAssertionsAndChains() {
        var parameter = GetParameter(nameof(PlainParameters), 0);

        parameter.Should().BeNamed("count").And.BeOfType<int>();
    }

    [TestMethod]
    public void BeNamed_FailsWithActualName() {
        var parameter = GetParameter(nameof(PlainParameters), 0);

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => parameter.Should().BeNamed("wrong"));

        Assert.AreEqual("Expected parameter to be named wrong, but found count.", ex.Message);
    }

    [TestMethod]
    public void NotBeNamed_SucceedsAndFails() {
        var parameter = GetParameter(nameof(PlainParameters), 0);

        parameter.Should().NotBeNamed("name");

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => parameter.Should().NotBeNamed("count"));
        Assert.AreEqual("Expected parameter not to be named count, but it is.", ex.Message);
    }

    [TestMethod]
    public void BeOfType_MatchesGenericAndRuntimeType() {
        var parameter = GetParameter(nameof(PlainParameters), 1);

        parameter.Should().BeOfType<string>();
#pragma warning disable CA2263 // The Type overload is part of the public surface under test.
        parameter.Should().BeOfType(typeof(string));
        parameter.Should().NotBeOfType(typeof(int));
#pragma warning restore CA2263
        parameter.Should().NotBeOfType<int>();
    }

    [TestMethod]
    public void BeOfType_FailsWithActualType() {
        var parameter = GetParameter(nameof(PlainParameters), 0);

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => parameter.Should().BeOfType<string>());

        Assert.AreEqual("Expected parameter to be of type String, but found Int32.", ex.Message);
    }

    [TestMethod]
    public void OptionalAndRequired_AreInverse() {
        var optional = GetParameter(nameof(OptionalParameters), 0);
        var required = GetParameter(nameof(PlainParameters), 0);

        optional.Should().BeOptional();
        required.Should().BeRequired();

        var optionalFailure = Assert.ThrowsExactly<AssertionFailedException>(() => required.Should().BeOptional());
        Assert.AreEqual("Expected required to be optional, but it is required.", optionalFailure.Message);

        var requiredFailure = Assert.ThrowsExactly<AssertionFailedException>(() => optional.Should().BeRequired());
        Assert.AreEqual("Expected optional to be required, but it is optional.", requiredFailure.Message);
    }

    [TestMethod]
    public void HaveDefaultValue_MatchesValueAndNull() {
        var optionalCount = GetParameter(nameof(OptionalParameters), 0);
        var optionalName = GetParameter(nameof(OptionalParameters), 1);

        optionalCount.Should().HaveDefaultValue(42);
        optionalName.Should().HaveDefaultValue(null);
    }

    [TestMethod]
    public void HaveDefaultValue_DistinguishesMissingFromNull() {
        var required = GetParameter(nameof(PlainParameters), 1);

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => required.Should().HaveDefaultValue(null));

        Assert.AreEqual("Expected required to have default value <null>, but it has no default value.", ex.Message);
    }

    [TestMethod]
    public void HaveDefaultValue_FailsWithDifferentValue() {
        var optional = GetParameter(nameof(OptionalParameters), 0);

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => optional.Should().HaveDefaultValue(7));

        Assert.AreEqual("Expected optional to have default value 7, but found 42.", ex.Message);
    }

    [TestMethod]
    public void NotHaveDefaultValue_SucceedsAndFails() {
        var required = GetParameter(nameof(PlainParameters), 0);
        var optional = GetParameter(nameof(OptionalParameters), 0);

        required.Should().NotHaveDefaultValue();

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => optional.Should().NotHaveDefaultValue());
        Assert.AreEqual("Expected optional not to have a default value, but it has 42.", ex.Message);
    }

    [TestMethod]
    public void Modifiers_AreDetectedFromMetadata() {
        var output = GetParameter(nameof(ModifierParameters), 0);
        var reference = GetParameter(nameof(ModifierParameters), 1);
        var input = GetParameter(nameof(ModifierParameters), 2);
        var values = GetParameter(nameof(ModifierParameters), 3);

        output.Should().BeOut().And.NotBeIn().And.NotBeRef().And.NotBeParams();
        reference.Should().BeRef().And.NotBeOut().And.NotBeIn();
        input.Should().BeIn().And.NotBeOut().And.NotBeRef();
        values.Should().BeParams();
    }

    [TestMethod]
    public void Modifiers_FailWithTheExpectedModifier() {
        var plain = GetParameter(nameof(PlainParameters), 0);
        var output = GetParameter(nameof(ModifierParameters), 0);

        var outFailure = Assert.ThrowsExactly<AssertionFailedException>(() => plain.Should().BeOut());
        Assert.AreEqual("Expected plain to be a parameter with the out modifier, but it is not.", outFailure.Message);

        var notOutFailure = Assert.ThrowsExactly<AssertionFailedException>(() => output.Should().NotBeOut());
        Assert.AreEqual("Expected output not to be a parameter with the out modifier, but it is.", notOutFailure.Message);
    }

    [TestMethod]
    public void HavePosition_MatchesAndFails() {
        var count = GetParameter(nameof(PlainParameters), 0);
        var name = GetParameter(nameof(PlainParameters), 1);

        count.Should().HavePosition(0);
        name.Should().HavePosition(1);
        count.Should().NotHavePosition(1);

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => count.Should().HavePosition(1));
        Assert.AreEqual("Expected count to be at position 1, but found position 0.", ex.Message);
    }

    [TestMethod]
    public void Attributes_AreDetectedFromMetadata() {
        var decorated = GetParameter(nameof(DecoratedParameters), 0);
        var plain = GetParameter(nameof(PlainParameters), 0);

        decorated.Should().BeDecoratedWith<MarkerAttribute>();
        decorated.Should().BeDecoratedWith<MarkerAttribute>().Which.Should().BeOfType<MarkerAttribute>();
        plain.Should().NotBeDecoratedWith<MarkerAttribute>();

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => plain.Should().BeDecoratedWith<MarkerAttribute>());
        Assert.AreEqual("Expected plain to be decorated with MarkerAttribute, but it is not.", ex.Message);
    }

    [TestMethod]
    public void NullParameter_FailsWithClearMessage() {
        ParameterInfo? parameter = null;

        parameter.Should().BeNull();

        var nullFailure = Assert.ThrowsExactly<AssertionFailedException>(() => parameter.Should().NotBeNull());
        Assert.AreEqual("Expected parameter not to be null.", nullFailure.Message);

        var metadataFailure = Assert.ThrowsExactly<AssertionFailedException>(() => parameter.Should().BeNamed("count"));
        Assert.AreEqual("Expected parameter to refer to a parameter, but found <null>.", metadataFailure.Message);
    }

    [TestMethod]
    public void BeNull_FailsForNonNullParameter() {
        var parameter = GetParameter(nameof(PlainParameters), 0);

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => parameter.Should().BeNull());

        Assert.AreEqual("Expected parameter to be null, but found Int32 count.", ex.Message);
    }

    [TestMethod]
    public void Because_IsAppendedToMetadataFailures() {
        var parameter = GetParameter(nameof(PlainParameters), 0);

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => parameter.Should().BeNamed("wrong", "because {0}", "the contract"));

        Assert.AreEqual("Expected parameter to be named wrong, but found count because the contract.", ex.Message);
    }

    [TestMethod]
    public void MetadataFailures_AreLocalizedInArabic() {
        using var _ = new CultureScope("ar");
        var parameter = GetParameter(nameof(PlainParameters), 0);

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => parameter.Should().BeNamed("wrong"));

        Assert.AreEqual("المتوقع أن يكون parameter باسم wrong، لكن وُجد count.", ex.Message);
    }

    private static ParameterInfo GetParameter(string methodName, int index) {
        var method = typeof(ParameterInfoTests).GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Static);
        Assert.IsNotNull(method);
        return method.GetParameters()[index];
    }

    private static void PlainParameters(int count, string name) => _ = count + name.Length;

    private static void ModifierParameters(out int output, ref int reference, in int input, params int[] values) {
        output = 0;
        reference++;
        _ = input + values.Length;
    }

    private static void OptionalParameters(int count = 42, string? name = null) => _ = count + (name?.Length ?? 0);

    private static void DecoratedParameters([Marker] int value) => _ = value;

    [AttributeUsage(AttributeTargets.Parameter)]
    private sealed class MarkerAttribute : Attribute;
}

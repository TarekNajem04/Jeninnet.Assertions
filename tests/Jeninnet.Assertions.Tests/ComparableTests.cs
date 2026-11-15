//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Tests;

[TestClass]
public class ComparableTests {
    private readonly record struct Money(decimal Amount) : IComparable<Money> {
        public int CompareTo(Money other) => Amount.CompareTo(other.Amount);
    }

    private sealed class Version(string text) : IComparable<Version> {
        public string Text { get; } = text;

        public int CompareTo(Version? other) =>
            other is null ? 1 : string.CompareOrdinal(Text, other.Text);

        public override string ToString() => Text;
    }

    private class AmountBase(decimal amount) : IComparable<AmountBase> {
        public decimal Amount { get; } = amount;

        public int CompareTo(AmountBase? other) =>
            other is null ? 1 : Amount.CompareTo(other.Amount);
    }

    private sealed class AmountDerived(decimal amount) : AmountBase(amount);

    private sealed class ComparableProxy : IComparable<Money> {
        public int CompareTo(Money other) => 0;
    }

    [TestMethod]
    public void Should_KeepsConcreteNumericAndValueOverloads() {
        var intAssertions = 1.Should();
        var longAssertions = 1L.Should();
        var decimalAssertions = 1m.Should();
        var doubleAssertions = 1.0.Should();
        var stringAssertions = "text".Should();
        var dateTimeAssertions = DateTime.UnixEpoch.Should();
        var guidAssertions = Guid.Empty.Should();

        intAssertions.Be(1);
        longAssertions.Be(1L);
        decimalAssertions.Be(1m);
        doubleAssertions.Be(1.0);
        stringAssertions.Be("text");
        dateTimeAssertions.Be(DateTime.UnixEpoch);
        guidAssertions.Be(Guid.Empty);
    }

    [TestMethod]
    public void Should_ReturnsComparableAssertionsForDomainStruct() {
        var assertions = new Money(10m).Should();

        assertions.BeGreaterThan(new Money(5m));
        assertions.Be(new Money(10m));
        assertions.NotBe(new Money(11m));
        assertions.BeInRange(new Money(1m), new Money(20m));
        assertions.BeRankedEquallyTo(new Money(10m));
        assertions.BeOneOf(new Money(1m), new Money(10m));
        assertions.Match(static money => money.Amount > 0);
    }

    [TestMethod]
    public void Should_ReturnsComparableAssertionsForDomainClass() {
        var assertions = new Version("1.2.3").Should();

        assertions.BeGreaterThan(new Version("1.2.2"));
        assertions.BeLessThan(new Version("1.3.0"));
        assertions.BeInRange(new Version("1.0.0"), new Version("2.0.0"));
    }

    [TestMethod]
    public void Should_UsesTheComparableInterfaceOfADerivedType() {
        var derived = new AmountDerived(20m);

        derived.Should().BeGreaterThan(new AmountBase(10m));
        derived.Should().BeInRange(new AmountBase(1m), new AmountBase(30m));
    }

    [TestMethod]
    public void ComparableFailure_NamesTheSubject() {
        var money = new Money(10m);
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => money.Should().BeGreaterThan(new Money(20m)));

        Assert.Contains("Expected money to be greater than", ex.Message);
        Assert.Contains("Money", ex.Message);
    }

    [TestMethod]
    public void NullableDomainStruct_FallsBackToObjectAssertions() {
        Money? missing = null;
        missing.Should().BeNull();

        Money? value = new Money(5m);
        value.Should().Be(new Money(5m));
    }

    [TestMethod]
    public void NullComparable_ThrowsArgumentNullException() {
        IComparable<Money>? comparable = null;
        Assert.ThrowsExactly<ArgumentNullException>(() => comparable!.Should());
    }

    [TestMethod]
    public void Should_RejectsAComparableThatIsNotTheTargetType() {
        IComparable<Money> proxy = new ComparableProxy();
        Assert.ThrowsExactly<ArgumentException>(() => proxy.Should());
    }
}

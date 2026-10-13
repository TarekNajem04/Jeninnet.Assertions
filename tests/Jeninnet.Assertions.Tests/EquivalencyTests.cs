//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Tests;

[TestClass]
public class EquivalencyTests {
    [TestMethod]
    public void EqualGraphs_Pass() {
        var actual = new Person("a", 1, new Address("x", "1"), ["t1", "t2"]);
        var expected = new Person("a", 1, new Address("x", "1"), ["t2", "t1"]);
        actual.Should().BeEquivalentTo(expected);
    }

    [TestMethod]
    public void PropertyMismatch_ReportsPath() {
        var actual = new Person("a", 1, new Address("x", "1"), []);
        var expected = new Person("b", 1, new Address("x", "1"), []);
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => actual.Should().BeEquivalentTo(expected));
        Assert.AreEqual("Expected actual to be equivalent, but Name was 'a' instead of 'b'.", ex.Message);
    }

    [TestMethod]
    public void NestedMismatch_ReportsFullPath() {
        var actual = new Person("a", 1, new Address("x", "1"), []);
        var expected = new Person("a", 1, new Address("y", "1"), []);
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => actual.Should().BeEquivalentTo(expected));
        Assert.AreEqual("Expected actual to be equivalent, but Address.City was 'x' instead of 'y'.", ex.Message);
    }

    [TestMethod]
    public void MultipleMismatches_AreNumbered() {
        var actual = new Person("a", 1, new Address("x", "1"), []);
        var expected = new Person("b", 2, new Address("x", "1"), []);
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => actual.Should().BeEquivalentTo(expected));
        Assert.AreEqual(
            string.Join(
                Environment.NewLine,
                "Expected actual to be equivalent, but found 2 difference(s):",
                "1. Age was 1 instead of 2.",
                "2. Name was 'a' instead of 'b'."),
            ex.Message);
    }

    [TestMethod]
    public void BothNull_Pass() {
        Person? actual = null;
        actual.Should().BeEquivalentTo((Person?)null);
    }

    [TestMethod]
    public void NullSubject_FailsAgainstValue() {
        Person? actual = null;
        var expected = new Person("a", 1, new Address("x", "1"), []);
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => actual.Should().BeEquivalentTo(expected));
        Assert.Contains("but found <null> instead of", ex.Message);
    }

    [TestMethod]
    public void MissingMember_Fails() {
        var actual = new { A = 1 };

        var expected = new { A = 1, B = 2 };

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => actual.Should().BeEquivalentTo(expected));
        Assert.AreEqual("Expected actual to be equivalent, but B was not found on the subject.", ex.Message);
    }

    [TestMethod]
    public void ExtraSubjectMember_IsIgnored() {
        var actual = new { A = 1, B = 2 };

        var expected = new { A = 1 };

        actual.Should().BeEquivalentTo(expected);
    }

    [TestMethod]
    public void Excluding_TopLevelMember_Passes() {
        var actual = new Person("a", 1, new Address("x", "1"), []);
        var expected = new Person("a", 99, new Address("x", "1"), []);
        actual.Should().BeEquivalentTo(expected, static o => o.Excluding(static x => x.Age));
    }

    [TestMethod]
    public void Excluding_NestedMember_Passes() {
        var actual = new Person("a", 1, new Address("x", "1"), []);
        var expected = new Person("a", 1, new Address("y", "1"), []);
        actual.Should().BeEquivalentTo(expected, static o => o.Excluding(static x => x.Address.City));
        actual.Should().BeEquivalentTo(expected, static o => o.Excluding(static x => x.Address));
    }

    [TestMethod]
    public void Excluding_CollectionIndex_Passes() {
        var actual = new Person("a", 1, new Address("x", "1"), ["a"]);
        var expected = new Person("a", 1, new Address("x", "1"), ["b"]);
        actual.Should().BeEquivalentTo(
            expected,
            static o => o.WithStrictOrdering().Excluding(static x => x.Tags[0]));
    }

    [TestMethod]
    public void Including_ReIncludesExcludedLeaf() {
        var actual = new Person("a", 1, new Address("x", "1"), []);
        var expected = new Person("a", 1, new Address("x", "2"), []);
        actual.Should().BeEquivalentTo(expected, static o => o.Excluding(static x => x.Address).Including(static x => x.Address.City));

        var cityMismatch = new Person("a", 1, new Address("y", "1"), []);
        var ex = Assert.ThrowsExactly<AssertionFailedException>(
            () => cityMismatch.Should().BeEquivalentTo(expected, static o => o.Excluding(static x => x.Address).Including(static x => x.Address.City)));
        Assert.AreEqual("Expected cityMismatch to be equivalent, but Address.City was 'y' instead of 'x'.", ex.Message);
    }

    [TestMethod]
    public void ComparingByValue_UsesEquals() {
        var actual = new IdOnly(1, "a");
        var expected = new IdOnly(1, "b");

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => actual.Should().BeEquivalentTo(expected));
        Assert.AreEqual("Expected actual to be equivalent, but Name was 'a' instead of 'b'.", ex.Message);

        actual.Should().BeEquivalentTo(expected, static o => o.ComparingByValue<IdOnly>());
    }

    [TestMethod]
    public void StrictOrdering_RespectsCollectionOrder() {
        var actual = new Person("a", 1, new Address("x", "1"), ["t1", "t2"]);
        var expected = new Person("a", 1, new Address("x", "1"), ["t2", "t1"]);
        actual.Should().BeEquivalentTo(expected);

        var ex = Assert.ThrowsExactly<AssertionFailedException>(
            () => actual.Should().BeEquivalentTo(expected, static o => o.WithStrictOrdering()));
        Assert.AreEqual(
            string.Join(
                Environment.NewLine,
                "Expected actual to be equivalent, but found 2 difference(s):",
                "1. Tags[0] was 't1' instead of 't2'.",
                "2. Tags[1] was 't2' instead of 't1'."),
            ex.Message);
    }

    [TestMethod]
    public void Dictionaries_CompareByKey() {
        var actual = new Dictionary<string, int> { ["a"] = 1 };

        actual.Should().BeEquivalentTo(new Dictionary<string, int> { ["a"] = 1 });

        var wrongValue = Assert.ThrowsExactly<AssertionFailedException>(
            () => actual.Should().BeEquivalentTo(new Dictionary<string, int> { ["a"] = 2 }));
        Assert.AreEqual("Expected actual to be equivalent, but [\"a\"] was 1 instead of 2.", wrongValue.Message);

        var missingKey = Assert.ThrowsExactly<AssertionFailedException>(
            () => actual.Should().BeEquivalentTo(new Dictionary<string, int> { ["a"] = 1, ["b"] = 2 }));
        Assert.AreEqual("Expected actual to be equivalent, but [\"b\"] was not found in the subject dictionary.", missingKey.Message);
    }

    [TestMethod]
    public void RootCollections_CompareUnorderedByDefault() {
        object subject = new[] { 1, 2, 3 };

        int[] reordered = [3, 2, 1];
        subject.Should().BeEquivalentTo(reordered);

        var strict = Assert.ThrowsExactly<AssertionFailedException>(
            () => subject.Should().BeEquivalentTo(reordered, static o => o.WithStrictOrdering()));
        Assert.AreEqual(
            string.Join(
                Environment.NewLine,
                "Expected subject to be equivalent, but found 2 difference(s):",
                "1. [0] was 1 instead of 3.",
                "2. [2] was 3 instead of 1."),
            strict.Message);

        int[] shorter = [1, 2];
        var count = Assert.ThrowsExactly<AssertionFailedException>(() => subject.Should().BeEquivalentTo(shorter));
        Assert.AreEqual("Expected subject to be equivalent, but found 3 item(s) instead of 2.", count.Message);
    }

    [TestMethod]
    public void TypeMismatch_Fails() {
        object subject = 5;
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => subject.Should().BeEquivalentTo("5"));
        Assert.AreEqual("Expected subject to be equivalent, but found an instance of type System.Int32 instead of System.String.", ex.Message);
    }

    [TestMethod]
    public void DerivedSubject_AgainstBaseExpectation_Passes() {
        object subject = new Dog("rex", true);
        subject.Should().BeEquivalentTo<Animal>(new Animal("rex"));
    }

    [TestMethod]
    public void CyclicGraphs_FailByDefault() {
        var first = new Node("a") { Next = new Node("b") };

        first.Next.Next = first;
        var second = new Node("a") { Next = new Node("b") };

        second.Next.Next = second;

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => first.Should().BeEquivalentTo(second));
        Assert.AreEqual("Expected first to be equivalent, but a cyclic reference was detected at Next.Next.", ex.Message);

        first.Should().BeEquivalentTo(second, static o => o.AllowingInfiniteRecursion());
    }

    [TestMethod]
    public void BecauseClause_IsAppended() {
        var actual = new Person("a", 1, new Address("x", "1"), []);
        var expected = new Person("b", 1, new Address("x", "1"), []);
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => actual.Should().BeEquivalentTo(expected, null, "the names were merged"));
        Assert.AreEqual("Expected actual to be equivalent, but Name was 'a' instead of 'b' because the names were merged.", ex.Message);
    }

    [TestMethod]
    public void InvalidSelector_ThrowsArgumentException() {
        var actual = new Person("a", 1, new Address("x", "1"), []);
        var expected = new Person("a", 1, new Address("x", "1"), []);
        Assert.ThrowsExactly<ArgumentException>(() => actual.Should().BeEquivalentTo(expected, static o => o.Excluding(static _ => 42)));
    }

    private sealed class Address(string city, string? zip) {
        public string City { get; } = city;
        public string? Zip { get; } = zip;
    }

    private sealed class Person(string name, int age, Address address, List<string> tags) {
        public string Name { get; } = name;
        public int Age { get; } = age;
        public Address Address { get; } = address;
        public List<string> Tags { get; } = tags;
    }

    private sealed class IdOnly(int id, string name) {
        public int Id { get; } = id;
        public string Name { get; } = name;

        public override bool Equals(object? obj) => obj is IdOnly other && Id == other.Id;

        public override int GetHashCode() => Id.GetHashCode();
    }

    private class Animal(string name) {
        public string Name { get; } = name;
    }

    private sealed class Dog(string name, bool barks) : Animal(name) {
        public bool Barks { get; } = barks;
    }

    private sealed class Node(string name) {
        public string Name { get; } = name;
        public Node? Next { get; set; }
    }
}

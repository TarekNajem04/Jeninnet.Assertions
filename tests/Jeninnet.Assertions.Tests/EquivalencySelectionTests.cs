//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Tests;

/// <summary>Covers the equivalency member-selection and member-matching options.</summary>
[TestClass]
public class EquivalencySelectionTests {
    [TestMethod]
    public void ExcludingByType_SkipsMembersOfThatType() {
        var actual = new Sample { Name = "a", Age = 1, Code = 1, Shape = new Shape { Sides = 4 } };
        var expected = new Sample { Name = "a", Age = 1, Code = 1, Shape = new Shape { Sides = 3 } };

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => actual.Should().BeEquivalentTo(expected));
        Assert.AreEqual("Expected actual to be equivalent, but Shape.Sides was 4 instead of 3.", ex.Message);

        actual.Should().BeEquivalentTo(expected, static o => o.Excluding<Shape>());
    }

    [TestMethod]
    public void ExcludingByType_ExcludesDerivedTypes() {
        var actual = new Sample { Name = "a", Age = 1, Code = 1, Shape = new Circle { Sides = 4, Radius = 2 } };
        var expected = new Sample { Name = "a", Age = 1, Code = 1, Shape = new Circle { Sides = 4, Radius = 9 } };

        actual.Should().BeEquivalentTo(expected, static o => o.Excluding<Shape>());
    }

    [TestMethod]
    public void ExcludingByOpenGenericType_SkipsClosedGenerics() {
        var actual = new Sample { Name = "a", Age = 1, Code = 1, Items = [new Inner { Rank = 1 }] };
        var expected = new Sample { Name = "a", Age = 1, Code = 1, Items = [new Inner { Rank = 9 }] };

        actual.Should().BeEquivalentTo(expected, static o => o.Excluding(typeof(List<>)));
    }

    [TestMethod]
    public void ExcludingByType_ValidatesNullArgument() {
        var actual = new Sample { Name = "a", Age = 1, Code = 1 };
        var expected = new Sample { Name = "a", Age = 1, Code = 1 };
        Assert.ThrowsExactly<ArgumentNullException>(() => actual.Should().BeEquivalentTo(expected, static o => o.Excluding((Type)null!)));
    }

    [TestMethod]
    public void ExcludingMembersNamed_SkipsMembersAtAnyDepth() {
        var actual = new Sample { Name = "a", Age = 1, Code = 1, Inner = new Inner { City = "x", Rank = 1 } };
        var expected = new Sample { Name = "a", Age = 1, Code = 1, Inner = new Inner { City = "y", Rank = 1 } };

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => actual.Should().BeEquivalentTo(expected));
        Assert.AreEqual("Expected actual to be equivalent, but Inner.City was 'x' instead of 'y'.", ex.Message);

        actual.Should().BeEquivalentTo(expected, static o => o.ExcludingMembersNamed("City"));
    }

    [TestMethod]
    public void ExcludingMembersNamed_ValidatesArguments() {
        var actual = new Sample { Name = "a", Age = 1, Code = 1 };
        var expected = new Sample { Name = "a", Age = 1, Code = 1 };
        Assert.ThrowsExactly<ArgumentNullException>(() => actual.Should().BeEquivalentTo(expected, static o => o.ExcludingMembersNamed(null!)));
        Assert.ThrowsExactly<ArgumentException>(() => actual.Should().BeEquivalentTo(expected, static o => o.ExcludingMembersNamed()));
        Assert.ThrowsExactly<ArgumentException>(() => actual.Should().BeEquivalentTo(expected, static o => o.ExcludingMembersNamed("Age", "")));
    }

    [TestMethod]
    public void ExcludingPredicate_SkipsMatchingMembers() {
        var actual = new Sample { Name = "a", Age = 1, Code = 1 };
        var expected = new Sample { Name = "a", Age = 9, Code = 1 };

        actual.Should().BeEquivalentTo(expected, static o => o.Excluding(static (MemberInfo member) => member.Name == "Age"));
    }

    [TestMethod]
    public void IncludingPredicate_ComparesOnlyMatchingMembers() {
        var actual = new Sample { Name = "a", Age = 1, Code = 1 };
        var expected = new Sample { Name = "b", Age = 1, Code = 9 };

        actual.Should().BeEquivalentTo(expected, static o => o.Including(static (MemberInfo member) => member.Name == "Age"));
    }

    [TestMethod]
    public void ExcludingPredicate_ValidatesNullArgument() {
        var actual = new Sample { Name = "a", Age = 1, Code = 1 };
        var expected = new Sample { Name = "a", Age = 1, Code = 1 };
        Assert.ThrowsExactly<ArgumentNullException>(() => actual.Should().BeEquivalentTo(expected, static o => o.Excluding((Func<MemberInfo, bool>)null!)));
        Assert.ThrowsExactly<ArgumentNullException>(() => actual.Should().BeEquivalentTo(expected, static o => o.Including((Func<MemberInfo, bool>)null!)));
    }

    [TestMethod]
    public void IncludingProperties_SkipsFields() {
        var actual = new Sample { Name = "a", Age = 1, Code = 1 };
        var expected = new Sample { Name = "a", Age = 1, Code = 9 };

        actual.Should().BeEquivalentTo(expected, static o => o.IncludingProperties());
    }

    [TestMethod]
    public void IncludingFields_SkipsProperties() {
        var actual = new Sample { Name = "a", Age = 1, Code = 1 };
        var expected = new Sample { Name = "b", Age = 9, Code = 1 };

        actual.Should().BeEquivalentTo(expected, static o => o.IncludingFields());
    }

    [TestMethod]
    public void ExcludingProperties_StopsComparingProperties() {
        var actual = new Sample { Name = "a", Age = 1, Code = 1 };
        var expected = new Sample { Name = "b", Age = 9, Code = 1 };

        actual.Should().BeEquivalentTo(expected, static o => o.ExcludingProperties());
    }

    [TestMethod]
    public void ExcludingFields_StopsComparingFields() {
        var actual = new Sample { Name = "a", Age = 1, Code = 1 };
        var expected = new Sample { Name = "a", Age = 1, Code = 9 };

        actual.Should().BeEquivalentTo(expected, static o => o.ExcludingFields());
    }

    [TestMethod]
    public void VisibilityOptions_LastCallWins() {
        var actual = new Sample { Name = "a", Age = 1, Code = 1 };
        var expected = new Sample { Name = "b", Age = 1, Code = 1 };

        actual.Should().BeEquivalentTo(expected, static o => o.IncludingProperties().IncludingFields());

        var ex = Assert.ThrowsExactly<AssertionFailedException>(
            () => actual.Should().BeEquivalentTo(expected, static o => o.IncludingFields().IncludingProperties()));
        Assert.AreEqual("Expected actual to be equivalent, but Name was 'a' instead of 'b'.", ex.Message);
    }

    [TestMethod]
    public void IncludingInternalProperties_ComparesNonPublicProperties() {
        var actual = new WithInternal { Value = 1, InternalValue = 1, InternalField = 1 };
        var expected = new WithInternal { Value = 1, InternalValue = 9, InternalField = 9 };

        actual.Should().BeEquivalentTo(expected);

        var propertyEx = Assert.ThrowsExactly<AssertionFailedException>(
            () => actual.Should().BeEquivalentTo(expected, static o => o.IncludingInternalProperties()));
        Assert.AreEqual("Expected actual to be equivalent, but InternalValue was 1 instead of 9.", propertyEx.Message);

        var fieldEx = Assert.ThrowsExactly<AssertionFailedException>(
            () => actual.Should().BeEquivalentTo(expected, static o => o.IncludingInternalFields()));
        Assert.AreEqual("Expected actual to be equivalent, but InternalField was 1 instead of 9.", fieldEx.Message);
    }

    [TestMethod]
    public void IncludingAllDeclaredProperties_IgnoresInheritedMembers() {
        var actual = new Derived { Inherited = "a", Own = 1 };
        var expected = new Derived { Inherited = "b", Own = 1 };

        actual.Should().BeEquivalentTo(expected, static o => o.IncludingAllDeclaredProperties());

        var ex = Assert.ThrowsExactly<AssertionFailedException>(
            () => actual.Should().BeEquivalentTo(expected, static o => o.IncludingAllRuntimeProperties()));
        Assert.AreEqual("Expected actual to be equivalent, but Inherited was 'a' instead of 'b'.", ex.Message);
    }

    [TestMethod]
    public void IncludingAllDeclaredProperties_ClearsSelectionRules() {
        var actual = new Derived { Inherited = "a", Own = 1 };
        var expected = new Derived { Inherited = "a", Own = 9 };

        actual.Should().BeEquivalentTo(expected, static o => o.Excluding(static x => x.Own));

        var ex = Assert.ThrowsExactly<AssertionFailedException>(
            () => actual.Should().BeEquivalentTo(expected, static o => o.Excluding(static x => x.Own).IncludingAllDeclaredProperties()));
        Assert.AreEqual("Expected actual to be equivalent, but Own was 1 instead of 9.", ex.Message);
    }

    [TestMethod]
    public void ExcludingExplicitlyImplementedProperties_SkipsDottedMembers() {
        var actual = new Explicitly("a");
        var expected = new Explicitly("b");

        var ex = Assert.ThrowsExactly<AssertionFailedException>(
            () => actual.Should().BeEquivalentTo(expected, static o => o.IncludingInternalProperties()));
        Assert.Contains("ILabeled.Label was 'a' instead of 'b'.", ex.Message);

        actual.Should().BeEquivalentTo(expected, static o => o.IncludingInternalProperties().ExcludingExplicitlyImplementedProperties());
    }

    [TestMethod]
    public void ExcludingNonBrowsableMembers_SkipsExpectationMember() {
        var actual = new Hidden { Value = 1, Secret = 1 };
        var expected = new Hidden { Value = 1, Secret = 9 };

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => actual.Should().BeEquivalentTo(expected));
        Assert.AreEqual("Expected actual to be equivalent, but Secret was 1 instead of 9.", ex.Message);

        actual.Should().BeEquivalentTo(expected, static o => o.ExcludingNonBrowsableMembers());
    }

    [TestMethod]
    public void IgnoringNonBrowsableMembersOnSubject_TreatsMemberAsMissing() {
        var actual = new Hidden { Value = 1, Secret = 1 };
        var expected = new Hidden { Value = 1, Secret = 9 };

        var ex = Assert.ThrowsExactly<AssertionFailedException>(
            () => actual.Should().BeEquivalentTo(expected, static o => o.IgnoringNonBrowsableMembersOnSubject()));
        Assert.AreEqual("Expected actual to be equivalent, but Secret was not found on the subject.", ex.Message);

        actual.Should().BeEquivalentTo(expected, static o => o.IgnoringNonBrowsableMembersOnSubject().ExcludingMissingMembers());
    }

    [TestMethod]
    public void ExcludingMissingMembers_IgnoresAbsentMembers() {
        var actual = new WithoutZip { City = "x" };
        var expected = new WithZip { City = "x", Zip = "1" };

        actual.Should().BeEquivalentTo(expected, static o => o.ExcludingMissingMembers());
    }

    [TestMethod]
    public void ThrowingOnMissingMembers_LastCallWins() {
        var actual = new WithoutZip { City = "x" };
        var expected = new WithZip { City = "x", Zip = "1" };

        var ex = Assert.ThrowsExactly<AssertionFailedException>(
            () => actual.Should().BeEquivalentTo(expected, static o => o.ExcludingMissingMembers().ThrowingOnMissingMembers()));
        Assert.AreEqual("Expected actual to be equivalent, but Zip was not found on the subject.", ex.Message);
    }

    [TestMethod]
    public void WithMapping_RenamesDirectMembers() {
        var actual = new TownRoot { Town = "x" };
        var expected = new CityRoot { City = "x" };

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => actual.Should().BeEquivalentTo(expected));
        Assert.AreEqual("Expected actual to be equivalent, but City was not found on the subject.", ex.Message);

        actual.Should().BeEquivalentTo(expected, static o => o.WithMapping("City", "Town"));
    }

    [TestMethod]
    public void WithMapping_RenamesNestedMembersByPath() {
        var actual = new TownGraph { Inner = new TownSubject { Town = "x" } };
        var expected = new CityGraph { Inner = new CityExpect { City = "x" } };

        actual.Should().BeEquivalentTo(expected, static o => o.WithMapping("Inner.City", "Inner.Town"));
    }

    [TestMethod]
    public void WithMapping_RenamesMembersFromExpressions() {
        var actual = new TownGraph { Inner = new TownSubject { Town = "x" } };
        var expected = new CityGraph { Inner = new CityExpect { City = "x" } };

        actual.Should().BeEquivalentTo(expected, static o => o.WithMapping<TownGraph>(static e => e.Inner.City, static s => s.Inner.Town));
    }

    [TestMethod]
    public void WithMapping_ScopedToNestedTypes() {
        var actual = new TownGraph { Inner = new TownSubject { Town = "x" } };
        var expected = new CityGraph { Inner = new CityExpect { City = "x" } };

        actual.Should().BeEquivalentTo(expected, static o => o.WithMapping<CityExpect, TownSubject>("City", "Town"));
        actual.Should().BeEquivalentTo(expected, static o => o.WithMapping(static (CityExpect e) => e.City, static (TownSubject s) => s.Town));
    }

    [TestMethod]
    public void WithMapping_ScopedRuleDoesNotApplyToOtherTypes() {
        var actual = new TownRoot { Town = "x" };
        var expected = new CityRoot { City = "x" };

        var ex = Assert.ThrowsExactly<AssertionFailedException>(
            () => actual.Should().BeEquivalentTo(expected, static o => o.WithMapping<CityRoot, TownRoot>("Other", "Town")));
        Assert.AreEqual("Expected actual to be equivalent, but City was not found on the subject.", ex.Message);
    }

    [TestMethod]
    public void WithMapping_LastCallWins() {
        var actual = new TownGraph { Inner = new TownSubject { Town = "x", County = "c" } };
        var expected = new CityGraph { Inner = new CityExpect { City = "x" } };

        actual.Should().BeEquivalentTo(expected, static o => o.WithMapping("Inner.City", "Inner.Town"));

        var ex = Assert.ThrowsExactly<AssertionFailedException>(
            () => actual.Should().BeEquivalentTo(expected, static o => o.WithMapping("Inner.City", "Inner.Town").WithMapping("Inner.City", "Inner.County")));
        Assert.AreEqual("Expected actual to be equivalent, but Inner.City was 'c' instead of 'x'.", ex.Message);
    }

    [TestMethod]
    public void WithMapping_ValidatesArguments() {
        var options = new SelfReferenceEquivalencyOptions<CityGraph>();

        Assert.ThrowsExactly<ArgumentException>(() => options.WithMapping(" ", "Town"));
        Assert.ThrowsExactly<ArgumentException>(() => options.WithMapping<CityGraph, TownGraph>("Inner.City", "Town"));
        Assert.ThrowsExactly<ArgumentException>(
            () => options.WithMapping<CityGraph, TownGraph>(static e => e.Inner.City, static s => s.Inner.Town));
        Assert.ThrowsExactly<ArgumentNullException>(() => options.WithMapping(null!, static (TownGraph s) => s.Inner.Town));
    }

    [TestMethod]
    public void For_Exclude_AppliesToSelectedCollectionOnly() {
        var actual = new Sample { Name = "a", Age = 1, Code = 1, Items = [new Inner { Rank = 1 }], Others = [new Inner { Rank = 1 }] };
        var expected = new Sample { Name = "a", Age = 1, Code = 1, Items = [new Inner { Rank = 9 }], Others = [new Inner { Rank = 9 }] };

        var ex = Assert.ThrowsExactly<AssertionFailedException>(
            () => actual.Should().BeEquivalentTo(expected, static o => o.WithStrictOrdering().For(static x => x.Items).Exclude(static x => x.Rank)));
        Assert.AreEqual("Expected actual to be equivalent, but Others[0].Rank was 1 instead of 9.", ex.Message);
    }

    [TestMethod]
    public void For_ExcludeAll_SkipsTheWholeCollection() {
        var actual = new Sample { Name = "a", Age = 1, Code = 1, Items = [new Inner { Rank = 1, City = "x" }] };
        var expected = new Sample { Name = "a", Age = 1, Code = 1, Items = [new Inner { Rank = 9, City = "y" }] };

        actual.Should().BeEquivalentTo(expected, static o => o.For(static x => x.Items).ExcludeAll());
    }

    [TestMethod]
    public void For_ChainsIntoNestedCollections() {
        var actual = new Grouped {
            Name = "a",
            Groups = [new Section { Items = [new Inner { City = "x", Rank = 1 }] }],
        };

        var expected = new Grouped {
            Name = "a",
            Groups = [new Section { Items = [new Inner { City = "y", Rank = 1 }] }],
        };

        actual.Should().BeEquivalentTo(expected, static o => o.For(static x => x.Groups).For(static x => x.Items).Exclude(static x => x.City));

        expected.Groups[0].Items[0].Rank = 9;
        var ex = Assert.ThrowsExactly<AssertionFailedException>(
            () => actual.Should().BeEquivalentTo(expected, static o => o.WithStrictOrdering().For(static x => x.Groups).For(static x => x.Items).Exclude(static x => x.City)));
        Assert.AreEqual("Expected actual to be equivalent, but Groups[0].Items[0].Rank was 1 instead of 9.", ex.Message);
    }

    [TestMethod]
    public void For_ValidatesArguments() {
        var options = new SelfReferenceEquivalencyOptions<Sample>();
        var selectsNoMember = Expression.Lambda<Func<Sample, IEnumerable<Inner>>>(
            Expression.Default(typeof(IEnumerable<Inner>)),
            Expression.Parameter(typeof(Sample)));

        Assert.ThrowsExactly<ArgumentNullException>(() => options.For<List<Inner>>(null!));
        Assert.ThrowsExactly<ArgumentException>(() => options.For(selectsNoMember));
    }

    [TestMethod]
    public void ComparingByValue_ByType_UsesEquals() {
        var actual = new Keyed(1, "a");
        var expected = new Keyed(1, "b");

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => actual.Should().BeEquivalentTo(expected));
        Assert.AreEqual("Expected actual to be equivalent, but Name was 'a' instead of 'b'.", ex.Message);

        var byValue = typeof(Keyed);
        actual.Should().BeEquivalentTo(expected, o => o.ComparingByValue(byValue));
    }

    [TestMethod]
    public void TopLevelExclusion_DoesNotLeakIntoNestedMembers() {
        var actual = new Sample { Name = "a", Age = 1, Code = 1, Inner = new Inner { Name = "a", Rank = 1 } };
        var expected = new Sample { Name = "b", Age = 1, Code = 1, Inner = new Inner { Name = "z", Rank = 1 } };

        var ex = Assert.ThrowsExactly<AssertionFailedException>(
            () => actual.Should().BeEquivalentTo(expected, static o => o.Excluding(static x => x.Name)));
        Assert.AreEqual("Expected actual to be equivalent, but Inner.Name was 'a' instead of 'z'.", ex.Message);
    }

    [TestMethod]
    public void RootLeafWithoutComparableMembers_StaysComparedByEquals() {
        var actual = new Opaque(1);
        var expected = new Opaque(2);

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => actual.Should().BeEquivalentTo(expected));
        Assert.Contains("instead of", ex.Message);
    }
}

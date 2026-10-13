//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Tests;

[TestClass]
public class EquivalencySemanticsTests {
    [TestMethod]
    public void StrictTyping_DerivedSubjectFails() {
        var actual = new Circle { Sides = 3, Radius = 1.5 };
        var expected = new Shape { Sides = 3 };
        actual.Should().BeEquivalentTo(expected);

        var ex = Assert.ThrowsExactly<AssertionFailedException>(
            () => actual.Should().BeEquivalentTo(expected, static o => o.WithStrictTyping()));
        Assert.Contains("but found an instance of type", ex.Message);
        Assert.Contains("Circle instead of", ex.Message);
    }

    [TestMethod]
    public void StrictTyping_LastCallWins() {
        var actual = new Circle { Sides = 3, Radius = 1.5 };
        var expected = new Shape { Sides = 3 };
        actual.Should().BeEquivalentTo(expected, static o => o.WithStrictTyping().WithoutStrictTyping());

        var ex = Assert.ThrowsExactly<AssertionFailedException>(
            () => actual.Should().BeEquivalentTo(expected, static o => o.WithoutStrictTyping().WithStrictTyping()));
        Assert.Contains("instead of", ex.Message);
    }

    [TestMethod]
    public void StrictTypingFor_ScopesOneMember() {
        var actual = new Exhibit { Item = new Circle { Sides = 3, Radius = 1 }, Label = "a" };
        var expected = new Exhibit { Item = new Shape { Sides = 3 }, Label = "a" };
        actual.Should().BeEquivalentTo(expected);

        var scoped = Assert.ThrowsExactly<AssertionFailedException>(
            () => actual.Should().BeEquivalentTo(expected, static o => o.WithStrictTypingFor(static m => m.Name == nameof(Exhibit.Item))));
        Assert.AreEqual(
            "Expected actual to be equivalent, but Item was of type Jeninnet.Assertions.Tests.EquivalencyFixtures.Circle instead of Jeninnet.Assertions.Tests.EquivalencyFixtures.Shape.",
            scoped.Message);

        actual.Should().BeEquivalentTo(expected, static o => o.WithStrictTypingFor(static m => m.Name == nameof(Exhibit.Label)));
    }

    [TestMethod]
    public void StrictExcept_SparesOneMember() {
        var actual = new Exhibit { Item = new Circle { Sides = 3, Radius = 1 }, Label = "a" };
        var expected = new Exhibit { Item = new Shape { Sides = 3 }, Label = "a" };

        var strict = Assert.ThrowsExactly<AssertionFailedException>(
            () => actual.Should().BeEquivalentTo(expected, static o => o.WithStrictTyping()));
        Assert.Contains("Item was of type", strict.Message);

        actual.Should().BeEquivalentTo(
            expected,
            static o => o.WithStrictTyping().WithoutStrictTypingFor(static m => m.Name == nameof(Exhibit.Item)));
    }

    [TestMethod]
    public void AutoConversion_PassFailAndLastWins() {
        var actual = new Flex { Count = 1, Total = 2, Label = "a" };
        var expected = new Flex { Count = 1L, Total = 2L, Label = "a" };

        var plain = Assert.ThrowsExactly<AssertionFailedException>(() => actual.Should().BeEquivalentTo(expected));
        Assert.Contains("2 difference(s)", plain.Message);
        Assert.Contains("Count was of type System.Int32 instead of System.Int64.", plain.Message);

        actual.Should().BeEquivalentTo(expected, static o => o.WithAutoConversion());

        var off = Assert.ThrowsExactly<AssertionFailedException>(
            () => actual.Should().BeEquivalentTo(expected, static o => o.WithAutoConversion().WithoutAutoConversion()));
        Assert.Contains("Count was of type", off.Message);
    }

    [TestMethod]
    public void AutoConversionFor_ScopesOneMember() {
        var actual = new Flex { Count = 1, Total = 2, Label = "a" };
        var expected = new Flex { Count = 1L, Total = 2L, Label = "a" };
        actual.Should().BeEquivalentTo(expected, static o => o.WithAutoConversion());

        var scoped = Assert.ThrowsExactly<AssertionFailedException>(
            () => actual.Should().BeEquivalentTo(expected, static o => o.WithAutoConversionFor(static m => m.Name == nameof(Flex.Total))));
        Assert.AreEqual("Expected actual to be equivalent, but Count was of type System.Int32 instead of System.Int64.", scoped.Message);

        var exempt = Assert.ThrowsExactly<AssertionFailedException>(
            () => actual.Should().BeEquivalentTo(
                expected,
                static o => o.WithAutoConversion().WithoutAutoConversionFor(static m => m.Name == nameof(Flex.Count))));
        Assert.AreEqual("Expected actual to be equivalent, but Count was of type System.Int32 instead of System.Int64.", exempt.Message);
    }

    [TestMethod]
    public void StrictBeatsConversion_InBothOrders() {
        var actual = new Flex { Count = 1, Total = 2, Label = "a" };
        var expected = new Flex { Count = 1L, Total = 2L, Label = "a" };

        var strictFirst = Assert.ThrowsExactly<AssertionFailedException>(
            () => actual.Should().BeEquivalentTo(expected, static o => o.WithStrictTyping().WithAutoConversion()));
        Assert.Contains("Count was of type", strictFirst.Message);

        var autoFirst = Assert.ThrowsExactly<AssertionFailedException>(
            () => actual.Should().BeEquivalentTo(expected, static o => o.WithAutoConversion().WithStrictTyping()));
        Assert.Contains("Count was of type", autoFirst.Message);

        actual.Should().BeEquivalentTo(expected, static o => o.WithStrictTyping().WithoutStrictTyping().WithAutoConversion());
    }

    [TestMethod]
    public void UnconvertiblePair_KeepsValueFailure() {
        var actual = new Flex { Count = 1, Total = 2, Label = "a" };
        var expected = new Flex { Count = 1L, Total = 2L, Label = "Friday" };

        var ex = Assert.ThrowsExactly<AssertionFailedException>(
            () => actual.Should().BeEquivalentTo(expected, static o => o.WithAutoConversion()));
        Assert.AreEqual("Expected actual to be equivalent, but Label was 'a' instead of 'Friday'.", ex.Message);
    }

    [TestMethod]
    public void DeclaredPreference_HidesRuntimeMembers() {
        var actual = new Derived { Inherited = "x", Own = 1 };
        Base expectation = new Derived { Inherited = "x", Own = 99 };

        var runtime = Assert.ThrowsExactly<AssertionFailedException>(() => actual.Should().BeEquivalentTo<Base>(expectation));
        Assert.Contains("Own was 1 instead of 99.", runtime.Message);

        actual.Should().BeEquivalentTo<Base>(expectation, static o => o.PreferringDeclaredMemberTypes());
        actual.Should().BeEquivalentTo<Base>(expectation, static o => o.PreferringRuntimeMemberTypes().PreferringDeclaredMemberTypes());

        var restored = Assert.ThrowsExactly<AssertionFailedException>(
            () => actual.Should().BeEquivalentTo<Base>(expectation, static o => o.PreferringDeclaredMemberTypes().PreferringDefaultMemberTypes()));
        Assert.Contains("Own was 1 instead of 99.", restored.Message);

        var explicitRuntime = Assert.ThrowsExactly<AssertionFailedException>(
            () => actual.Should().BeEquivalentTo<Base>(expectation, static o => o.PreferringRuntimeMemberTypes()));
        Assert.Contains("Own was 1 instead of 99.", explicitRuntime.Message);
    }

    [TestMethod]
    public void EnumsByValue_MatchesAcrossTypes() {
        var actual = new Flex { Flag = DayKind.Tue };
        var expected = new Flex { Flag = OtherKind.Second };

        var plain = Assert.ThrowsExactly<AssertionFailedException>(() => actual.Should().BeEquivalentTo(expected));
        Assert.Contains("Flag was of type", plain.Message);

        actual.Should().BeEquivalentTo(expected, static o => o.ComparingEnumsByValue());
    }

    [TestMethod]
    public void EnumsByName_SameValueDifferentNamesFails() {
        var actual = new Flex { Flag = DayKind.Tue };
        var expected = new Flex { Flag = OtherKind.Second };

        actual.Should().BeEquivalentTo(expected, static o => o.ComparingEnumsByValue());

        var byName = Assert.ThrowsExactly<AssertionFailedException>(
            () => actual.Should().BeEquivalentTo(expected, static o => o.ComparingEnumsByValue().ComparingEnumsByName()));
        Assert.AreEqual("Expected actual to be equivalent, but Flag was Tue instead of Second.", byName.Message);

        var sameName = new Flex { Flag = DayKind.Mon };
        var sameNameExpected = new Flex { Flag = OtherKind.Mon };
        sameName.Should().BeEquivalentTo(sameNameExpected, static o => o.ComparingEnumsByName());
    }

    [TestMethod]
    public void ByMembers_ComparesStringLengthOnly() {
        object actual = "ab";
        const string expected = "ba";

        var plain = Assert.ThrowsExactly<AssertionFailedException>(() => actual.Should().BeEquivalentTo(expected));
        Assert.AreEqual("Expected actual to be equivalent, but found 'ab' instead of 'ba'.", plain.Message);

        actual.Should().BeEquivalentTo(expected, static o => o.ComparingByMembers<string>());
        actual.Should().BeEquivalentTo(expected, o => o.ComparingByMembers(expected.GetType()));

        var longer = Assert.ThrowsExactly<AssertionFailedException>(
            () => actual.Should().BeEquivalentTo("abc", static o => o.ComparingByMembers<string>()));
        Assert.AreEqual("Expected actual to be equivalent, but Length was 2 instead of 3.", longer.Message);
    }

    [TestMethod]
    public void RecordsByValue_UsesEquality() {
        var actual = new PointRec(1, 2);
        var expected = new PointRec(1, 3);

        var members = Assert.ThrowsExactly<AssertionFailedException>(() => actual.Should().BeEquivalentTo(expected));
        Assert.AreEqual("Expected actual to be equivalent, but Y was 2 instead of 3.", members.Message);

        var byValue = Assert.ThrowsExactly<AssertionFailedException>(
            () => actual.Should().BeEquivalentTo(expected, static o => o.ComparingRecordsByValue()));
        Assert.Contains("to be equivalent, but found PointRec", byValue.Message);
        Assert.Contains("instead of PointRec", byValue.Message);

        actual.Should().BeEquivalentTo(new PointRec(1, 2), static o => o.ComparingRecordsByValue());

        var restored = Assert.ThrowsExactly<AssertionFailedException>(
            () => actual.Should().BeEquivalentTo(expected, static o => o.ComparingRecordsByValue().ComparingRecordsByMembers()));
        Assert.AreEqual("Expected actual to be equivalent, but Y was 2 instead of 3.", restored.Message);
    }

    [TestMethod]
    public void NullStrings_AllCombinations() {
        static Box nullBox(string? label) => new() { Label = label };
        var none = nullBox(null);
        var blank = nullBox("");
        none.Should().BeEquivalentTo(blank, static o => o.ComparingNullStringsAsEmpty());
        blank.Should().BeEquivalentTo(none, static o => o.ComparingNullStringsAsEmpty());
        blank.Should().BeEquivalentTo(nullBox(""));
        none.Should().BeEquivalentTo(nullBox(null));

        var value = nullBox("x");
        var nullVsValue = Assert.ThrowsExactly<AssertionFailedException>(
            () => none.Should().BeEquivalentTo(value, static o => o.ComparingNullStringsAsEmpty()));
        Assert.AreEqual("Expected none to be equivalent, but Label was <null> instead of 'x'.", nullVsValue.Message);

        var valueVsNull = Assert.ThrowsExactly<AssertionFailedException>(
            () => value.Should().BeEquivalentTo(none, static o => o.ComparingNullStringsAsEmpty()));
        Assert.AreEqual("Expected value to be equivalent, but Label was 'x' instead of <null>.", valueVsNull.Message);

        var defaultFails = Assert.ThrowsExactly<AssertionFailedException>(() => none.Should().BeEquivalentTo(blank));
        Assert.AreEqual("Expected none to be equivalent, but Label was <null> instead of ''.", defaultFails.Message);
    }

    [TestMethod]
    public void NullCollections_AllCombinations() {
        static Basket nullBasket(List<string>? lines) => new() { Lines = lines };
        var none = nullBasket(null);
        var empty = nullBasket([]);
        none.Should().BeEquivalentTo(empty, static o => o.ComparingNullCollectionsAsEmpty());
        empty.Should().BeEquivalentTo(none, static o => o.ComparingNullCollectionsAsEmpty());

        var one = nullBasket(["a"]);
        var nullVsItems = Assert.ThrowsExactly<AssertionFailedException>(
            () => none.Should().BeEquivalentTo(one, static o => o.ComparingNullCollectionsAsEmpty()));
        Assert.Contains("Lines was <null> instead of", nullVsItems.Message);

        var itemsVsNull = Assert.ThrowsExactly<AssertionFailedException>(
            () => one.Should().BeEquivalentTo(none, static o => o.ComparingNullCollectionsAsEmpty()));
        Assert.Contains("instead of <null>.", itemsVsNull.Message);

        var defaultFails = Assert.ThrowsExactly<AssertionFailedException>(() => none.Should().BeEquivalentTo(empty));
        Assert.Contains("Lines was <null> instead of System.Collections.Generic.List", defaultFails.Message);
    }

    [TestMethod]
    public void CyclicReferences_FailByDefault() {
        var actual = new Node { Value = 1 };
        actual.Next = actual;
        var expected = new Node { Value = 1 };
        expected.Next = expected;

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => actual.Should().BeEquivalentTo(expected));
        Assert.AreEqual("Expected actual to be equivalent, but a cyclic reference was detected at Next.", ex.Message);

        actual.Should().BeEquivalentTo(expected, static o => o.AllowingInfiniteRecursion());
        actual.Should().BeEquivalentTo(expected, static o => o.IgnoringCyclicReferences());
        actual.Should().BeEquivalentTo(expected, static o => o.AllowingInfiniteRecursion().IgnoringCyclicReferences());
        actual.Should().BeEquivalentTo(expected, static o => o.IgnoringCyclicReferences().AllowingInfiniteRecursion());
    }

    [TestMethod]
    public void CyclicCutoff_StillComparesLeaves() {
        var actual = new Node { Value = 1, Next = new Node { Value = 2 } };
        actual.Next.Next = actual;
        var expected = new Node { Value = 1, Next = new Node { Value = 9 } };
        expected.Next.Next = expected;

        var ex = Assert.ThrowsExactly<AssertionFailedException>(
            () => actual.Should().BeEquivalentTo(expected, static o => o.IgnoringCyclicReferences()));
        Assert.AreEqual("Expected actual to be equivalent, but Next.Value was 2 instead of 9.", ex.Message);
    }

    [TestMethod]
    public void WithoutRecursing_StopsAtSurface() {
        var actual = new Box { Label = "a", Inner = new Box { Label = "x" } };
        var expected = new Box { Label = "a", Inner = new Box { Label = "y" } };

        var deep = Assert.ThrowsExactly<AssertionFailedException>(() => actual.Should().BeEquivalentTo(expected));
        Assert.AreEqual("Expected actual to be equivalent, but Inner.Label was 'x' instead of 'y'.", deep.Message);

        var shallow = Assert.ThrowsExactly<AssertionFailedException>(
            () => actual.Should().BeEquivalentTo(expected, static o => o.WithoutRecursing()));
        Assert.Contains("Inner was ", shallow.Message);
        Assert.Contains("instead of", shallow.Message);

        var restored = Assert.ThrowsExactly<AssertionFailedException>(
            () => actual.Should().BeEquivalentTo(expected, static o => o.WithoutRecursing().IncludingNestedObjects()));
        Assert.AreEqual("Expected actual to be equivalent, but Inner.Label was 'x' instead of 'y'.", restored.Message);

        var shared = new Box { Label = "s" };
        new Box { Label = "a", Inner = shared }.Should().BeEquivalentTo(
            new Box { Label = "a", Inner = shared },
            static o => o.WithoutRecursing());
    }

    [TestMethod]
    public void StrictOrderingFor_ScopesOneCollection() {
        var actual = new Basket { Tags = ["b", "a"], Codes = ["y", "x"] };
        var expected = new Basket { Tags = ["a", "b"], Codes = ["x", "y"] };
        actual.Should().BeEquivalentTo(expected);

        var scoped = Assert.ThrowsExactly<AssertionFailedException>(
            () => actual.Should().BeEquivalentTo(expected, static o => o.WithStrictOrderingFor(static x => x.Tags)));
        Assert.Contains("Tags[0] was 'b' instead of 'a'.", scoped.Message);

        actual.Should().BeEquivalentTo(
            expected,
            static o => o.WithStrictOrderingFor(static x => x.Tags).WithoutStrictOrderingFor(static x => x.Tags));

        var mixed = Assert.ThrowsExactly<AssertionFailedException>(
            () => actual.Should().BeEquivalentTo(
                expected,
                static o => o.WithStrictOrdering().WithoutStrictOrderingFor(static x => x.Tags)));
        Assert.Contains("2 difference(s)", mixed.Message);
        Assert.Contains("Codes[0] was 'y' instead of 'x'.", mixed.Message);
    }

    [TestMethod]
    public void JsonPropertyCasing_IgnoresKeyCase() {
        var actual = new Basket { Map = new Dictionary<string, int> { ["Key"] = 1 } };
        var expected = new Basket { Map = new Dictionary<string, int> { ["key"] = 1 } };

        var plain = Assert.ThrowsExactly<AssertionFailedException>(() => actual.Should().BeEquivalentTo(expected));
        Assert.Contains("was not found in the subject dictionary.", plain.Message);

        actual.Should().BeEquivalentTo(expected, static o => o.IgnoringJsonPropertyCasing());
    }

    [TestMethod]
    public void GraphStringModes_PassAndFail() {
        static Box textBox(string? label) => new() { Label = label };
        var lower = textBox("a");
        var casing = Assert.ThrowsExactly<AssertionFailedException>(() => lower.Should().BeEquivalentTo(textBox("A")));
        Assert.AreEqual("Expected lower to be equivalent, but Label was 'a' instead of 'A'.", casing.Message);
        lower.Should().BeEquivalentTo(textBox("A"), static o => o.IgnoringCase());

        var padded = textBox(" x");
        var leading = Assert.ThrowsExactly<AssertionFailedException>(() => padded.Should().BeEquivalentTo(textBox("x")));
        Assert.AreEqual("Expected padded to be equivalent, but Label was ' x' instead of 'x'.", leading.Message);
        padded.Should().BeEquivalentTo(textBox("x"), static o => o.IgnoringLeadingWhitespace());

        var spaced = textBox("x ");
        var trailing = Assert.ThrowsExactly<AssertionFailedException>(() => spaced.Should().BeEquivalentTo(textBox("x")));
        Assert.AreEqual("Expected spaced to be equivalent, but Label was 'x ' instead of 'x'.", trailing.Message);
        spaced.Should().BeEquivalentTo(textBox("x"), static o => o.IgnoringTrailingWhitespace());

        var windows = textBox("a\r\nb");
        var newline = Assert.ThrowsExactly<AssertionFailedException>(() => windows.Should().BeEquivalentTo(textBox("a\nb")));
        Assert.AreEqual("Expected windows to be equivalent, but Label was 'a\r\nb' instead of 'a\nb'.", newline.Message);
        windows.Should().BeEquivalentTo(textBox("a\nb"), static o => o.IgnoringNewlineStyle());
    }

    [TestMethod]
    public void LongStringMismatch_TruncatesValuesByDefault() {
        const string subjectTail = "SUBJECT_TAIL_MARKER";
        const string expectedTail = "EXPECTED_TAIL_MARKER";
        var actual = new Box { Label = new string('a', 250) + subjectTail };
        var expected = new Box { Label = new string('b', 250) + expectedTail };

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => actual.Should().BeEquivalentTo(expected));
        Assert.Contains("...(truncated)", ex.Message);
        Assert.DoesNotContain(subjectTail, ex.Message);
        Assert.DoesNotContain(expectedTail, ex.Message);
    }

    [TestMethod]
    public void LongStringMismatch_WithIncludingFullStrings_ShowsFullValues() {
        const string subjectTail = "SUBJECT_TAIL_MARKER";
        const string expectedTail = "EXPECTED_TAIL_MARKER";
        var actual = new Box { Label = new string('a', 250) + subjectTail };
        var expected = new Box { Label = new string('b', 250) + expectedTail };

        var ex = Assert.ThrowsExactly<AssertionFailedException>(
            () => actual.Should().BeEquivalentTo(
                expected,
                static o => o.IncludingFullStringsInDifference().WithFullDump()));
        Assert.Contains(subjectTail, ex.Message);
        Assert.Contains(expectedTail, ex.Message);
        Assert.Contains("include-full-strings-in-difference", ex.Message);
    }

    private sealed class Flex {
        public object Count { get; set; } = 0;

        public object Total { get; set; } = 0;

        public string Label { get; set; } = "";

        public object Flag { get; set; } = 0;
    }

    private sealed class Exhibit {
        public Shape Item { get; set; } = new Shape();

        public string Label { get; set; } = "";
    }

    private enum DayKind {
        Mon = 1,
        Tue = 2,
    }

    private enum OtherKind {
        Mon = 1,
        Second = 2,
    }

    private sealed record PointRec(int X, int Y);

    private sealed class Box {
        public string? Label { get; set; }

        public Box? Inner { get; set; }
    }

    private sealed class Basket {
        public List<string> Tags { get; set; } = [];

        public List<string> Codes { get; set; } = [];

        public List<string>? Lines { get; set; }

        public Dictionary<string, int> Map { get; set; } = [];
    }

    private sealed class Node {
        public int Value { get; set; }

        public Node? Next { get; set; }
    }
}

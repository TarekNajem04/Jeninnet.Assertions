//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Tests;

[TestClass]
public class SelectorTests {
    private abstract class ShapeBase {
        public abstract double Area();
    }

    private sealed class Circle : ShapeBase {
        public override double Area() => 3.14;
    }

    private sealed class Square : ShapeBase {
        public override double Area() => 4.0;
    }

    [TestMethod]
    public void ThatDeriveFrom_SelectsSubclasses() {
        var assembly = typeof(Circle).Assembly;
        AllTypes.From(assembly).ThatDeriveFrom<ShapeBase>().Should().HaveCount(2).And.BeSealed().And.HaveMethod("Area");
        ShapeBase shape = new Circle();
        Assert.AreEqual(3.14, shape.Area());
        Assert.AreEqual(4.0, new Square().Area());
    }

    [TestMethod]
    public void HaveMethod_FailsNamingOffender() {
        var assembly = typeof(Circle).Assembly;
        var ex = Assert.ThrowsExactly<AssertionFailedException>(
            () => AllTypes.From(assembly).ThatDeriveFrom<ShapeBase>().Should().HaveMethod("Missing"));
        Assert.Contains("to have method 'Missing'", ex.Message);
        Assert.IsTrue(ex.Message.Contains("Circle") || ex.Message.Contains("Square"));
    }

    [TestMethod]
    public void EmptySelection_PassesVacuously() {
        var assembly = typeof(Circle).Assembly;
        AllTypes.From(assembly).ThatDeriveFrom<string>().Should().HaveCount(0).And.BeSealed();
    }

    [TestMethod]
    public void ThatAreInNamespace_SelectsTypes() {
        var assembly = typeof(Circle).Assembly;
        AllTypes.From(assembly).ThatAreInNamespace(typeof(Circle).Namespace!).Should().HaveMethod("ToString");
    }

    [TestMethod]
    public void From_RequiresAssemblies() {
        Assert.ThrowsExactly<ArgumentException>(() => AllTypes.From());
        Assert.ThrowsExactly<ArgumentNullException>(() => AllTypes.From(null!));
    }

    [TestMethod]
    public void ThatDoNotDeriveFrom_SelectsComplement() {
        new TypeSelector([typeof(ShapeBase), typeof(Circle), typeof(Widget)]).ThatDoNotDeriveFrom<ShapeBase>()
            .Should().HaveCount(2);
        new TypeSelector([typeof(ShapeBase), typeof(Circle), typeof(Widget)]).ThatDoNotDeriveFrom<ShapeBase>()
            .ThatAreClasses().Should().HaveCount(2);
        new TypeSelector([typeof(Circle), typeof(Widget)]).ThatDeriveFrom<ShapeBase>().ToArray().Should().HaveCount(1);
    }

    [TestMethod]
    public void ThatDoNotImplement_SelectsComplement() {
        new TypeSelector([typeof(Widget), typeof(StructWidget), typeof(IWorker)]).ThatDoNotImplement<IWidget>()
            .Should().HaveCount(1);
        new TypeSelector([typeof(IWidget), typeof(IWorker)]).ThatDoNotImplement<IWidget>().Should().HaveCount(1);
        new TypeSelector([typeof(IWorker), typeof(Widget)]).ThatDoNotImplement<IWidget>().ToArray()
            .Should().ContainSingle().Which.Should().Be(typeof(IWorker));
    }

    [TestMethod]
    public void ThatAreNotInNamespace_SelectsComplement() {
        var ns = typeof(Widget).Namespace!;
        new TypeSelector([typeof(Widget), typeof(string)]).ThatAreNotInNamespace(ns).Should().HaveCount(1);
        new TypeSelector([typeof(Widget), typeof(string)]).ThatAreInNamespace(ns).Should().HaveCount(1);
        Assert.ThrowsExactly<ArgumentNullException>(() => new TypeSelector([typeof(Widget)]).ThatAreNotInNamespace(null!));
    }

    [TestMethod]
    public void ThatAreUnderNamespace_SelectsNested() {
        new TypeSelector([typeof(Widget), typeof(string)]).ThatAreUnderNamespace("Jeninnet.Assertions.Tests")
            .Should().HaveCount(1);
        new TypeSelector([typeof(Widget), typeof(string)]).ThatAreUnderNamespace("Jeninnet.Assertions.Tests.SelectorFixtures")
            .Should().HaveCount(1);
        new TypeSelector([typeof(Widget), typeof(string)]).ThatAreUnderNamespace("Jeninnet.Assert")
            .Should().HaveCount(0);
        Assert.ThrowsExactly<ArgumentNullException>(() => new TypeSelector([typeof(Widget)]).ThatAreUnderNamespace(null!));
    }

    [TestMethod]
    public void ThatAreNotUnderNamespace_SelectsComplement() {
        new TypeSelector([typeof(Widget), typeof(string)]).ThatAreNotUnderNamespace("Jeninnet.Assertions")
            .Should().HaveCount(1);
        new TypeSelector([typeof(Widget), typeof(string)]).ThatAreNotUnderNamespace("Nope")
            .Should().HaveCount(2);
        Assert.ThrowsExactly<ArgumentNullException>(() => new TypeSelector([typeof(Widget)]).ThatAreNotUnderNamespace(null!));
    }

    [TestMethod]
    public void ToArray_AndEnumeration_RoundTrip() {
        TypeSelector selector = new([typeof(Circle), typeof(Square)]);
        var types = selector.ToArray();
        Assert.HasCount(2, types);
        Assert.AreEqual(typeof(Circle), types[0]);

        List<Type> visited = [.. selector];

        Assert.HasCount(2, visited);
        Assert.IsTrue(selector.SequenceEqual(types));
    }

    [TestMethod]
    public void SelectorAssertions_ExposeSelectedTypes() {
        var selector = new TypeSelector([typeof(Circle), typeof(Square)]);
        Assert.IsTrue(selector.SequenceEqual(selector.Should().Subject));
    }

    [TestMethod]
    public void ThatAreClasses_Filters() {
        var assembly = typeof(Circle).Assembly;
        AllTypes.From(assembly).ThatImplement<IWidget>().ThatAreClasses().Should().HaveCount(4);
        AllTypes.From(assembly).ThatImplement<IWidget>().ThatAreNotClasses().Should().HaveCount(1);
        Assert.AreEqual("static", WidgetStatic.Name);
    }

    [TestMethod]
    public void ThatAreInterfaces_Filters() {
        new TypeSelector([typeof(Widget), typeof(IWidget)]).ThatAreInterfaces().Should().HaveCount(1);
        new TypeSelector([typeof(Widget), typeof(IWidget)]).ThatAreNotInterfaces().Should().HaveCount(1);
    }

    [TestMethod]
    public void ThatAreValueTypes_Filters() {
        var assembly = typeof(Circle).Assembly;
        AllTypes.From(assembly).ThatImplement<IWidget>().ThatAreValueTypes().Should().HaveCount(1);
        AllTypes.From(assembly).ThatImplement<IWidget>().ThatAreNotValueTypes().Should().HaveCount(4);
        new TypeSelector([typeof(StructWidget), typeof(Widget)]).ThatAreValueTypes().Should().HaveCount(1);
    }

    [TestMethod]
    public void ThatAreStatic_Filters() {
        new TypeSelector([typeof(WidgetStatic), typeof(Widget)]).ThatAreStatic().Should().HaveCount(1);
        new TypeSelector([typeof(WidgetStatic), typeof(Widget)]).ThatAreNotStatic().Should().HaveCount(1);
    }

    [TestMethod]
    public void ThatAreAbstract_Filters() {
        var assembly = typeof(Circle).Assembly;
        AllTypes.From(assembly).ThatImplement<IWidget>().ThatAreAbstract().Should().HaveCount(1);
        AllTypes.From(assembly).ThatImplement<IWidget>().ThatAreNotAbstract().Should().HaveCount(4);
        new TypeSelector([typeof(WidgetBase), typeof(Widget)]).ThatAreAbstract().Should().HaveCount(1);
    }

    [TestMethod]
    public void ThatAreSealed_Filters() {
        var assembly = typeof(Circle).Assembly;
        AllTypes.From(assembly).ThatImplement<IWidget>().ThatAreSealed().Should().HaveCount(2);
        AllTypes.From(assembly).ThatImplement<IWidget>().ThatAreNotSealed().Should().HaveCount(3);
    }

    [TestMethod]
    public void ThatAreDecoratedWith_Filters() {
        new TypeSelector([typeof(TaggedWidget), typeof(Widget)]).ThatAreDecoratedWith<TagAttribute>().Should().HaveCount(1);
        new TypeSelector([typeof(TaggedWidget), typeof(Widget)]).ThatAreNotDecoratedWith<TagAttribute>().Should().HaveCount(1);
    }

    [TestMethod]
    public void ThatAreDecoratedWithOrInherit_Filters() {
        new TypeSelector([typeof(ChildWidget), typeof(Widget)]).ThatAreDecoratedWithOrInherit<TagAttribute>().Should().HaveCount(1);
        new TypeSelector([typeof(ChildWidget), typeof(Widget)]).ThatAreDecoratedWith<TagAttribute>().Should().HaveCount(0);
        new TypeSelector([typeof(ChildWidget), typeof(Widget)]).ThatAreNotDecoratedWithOrInherit<TagAttribute>().Should().HaveCount(1);
    }

    [TestMethod]
    public void ThatSatisfy_Filters() {
        var assembly = typeof(Circle).Assembly;
        AllTypes.From(assembly).ThatDeriveFrom<ShapeBase>().ThatSatisfy(static type => type.Name.StartsWith('C')).Should().HaveCount(1);
        Assert.ThrowsExactly<ArgumentNullException>(() => AllTypes.From(assembly).ThatSatisfy(null!));
    }

    [TestMethod]
    public void ChainedFilters_Intersect() {
        var assembly = typeof(Circle).Assembly;
        AllTypes.From(assembly).ThatImplement<IWidget>().ThatAreClasses().ThatAreNotSealed().Should().HaveCount(3);
    }

    [TestMethod]
    public void UnwrapTaskTypes_Unwraps() {
        new TypeSelector([typeof(Task<int>)]).UnwrapTaskTypes().Should().HaveMethod("Parse");
        new TypeSelector([typeof(Task)]).UnwrapTaskTypes().ThatSatisfy(static type => type == typeof(void)).Should().HaveCount(1);
        new TypeSelector([typeof(string)]).UnwrapTaskTypes().Should().HaveMethod("Substring");
    }

    [TestMethod]
    public void UnwrapEnumerableTypes_Unwraps() {
        new TypeSelector([typeof(List<int>)]).UnwrapEnumerableTypes().Should().HaveMethod("Parse");
        new TypeSelector([typeof(string)]).UnwrapEnumerableTypes().Should().HaveMethod("Substring");
        new TypeSelector([typeof(int[])]).UnwrapEnumerableTypes().Should().HaveMethod("Parse");
    }

    [TestMethod]
    public void Selector_NotBeSealed_PassAndFail() {
        new TypeSelector([typeof(Widget)]).Should().NotBeSealed();

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => new TypeSelector([typeof(ChildWidget)]).Should().NotBeSealed());
        Assert.AreEqual("Expected Jeninnet.Assertions.Tests.SelectorFixtures.ChildWidget not to be sealed, but it was.", ex.Message);
    }

    [TestMethod]
    public void Selector_BeDecoratedWith_PassAndFail() {
        new TypeSelector([typeof(TaggedWidget)]).Should().BeDecoratedWith<TagAttribute>();
        new TypeSelector([typeof(Widget)]).Should().NotBeDecoratedWith<TagAttribute>();

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => new TypeSelector([typeof(Widget)]).Should().BeDecoratedWith<TagAttribute>());
        Assert.AreEqual("Expected Jeninnet.Assertions.Tests.SelectorFixtures.Widget to be decorated with TagAttribute, but it was not.", ex.Message);

        var notEx = Assert.ThrowsExactly<AssertionFailedException>(() => new TypeSelector([typeof(TaggedWidget)]).Should().NotBeDecoratedWith<TagAttribute>());
        Assert.AreEqual("Expected Jeninnet.Assertions.Tests.SelectorFixtures.TaggedWidget not to be decorated with TagAttribute, but it was.", notEx.Message);
    }

    [TestMethod]
    public void Selector_BeDecoratedWithOrInherit_PassAndFail() {
        new TypeSelector([typeof(ChildWidget)]).Should().BeDecoratedWithOrInherit<TagAttribute>();
        new TypeSelector([typeof(Widget)]).Should().NotBeDecoratedWithOrInherit<TagAttribute>();

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => new TypeSelector([typeof(Widget)]).Should().BeDecoratedWithOrInherit<TagAttribute>());
        Assert.AreEqual("Expected Jeninnet.Assertions.Tests.SelectorFixtures.Widget to be decorated with TagAttribute or inherit it, but it was not.", ex.Message);

        var notEx = Assert.ThrowsExactly<AssertionFailedException>(() => new TypeSelector([typeof(ChildWidget)]).Should().NotBeDecoratedWithOrInherit<TagAttribute>());
        Assert.AreEqual("Expected Jeninnet.Assertions.Tests.SelectorFixtures.ChildWidget not to be decorated with TagAttribute or inherit it, but it was.", notEx.Message);
    }

    [TestMethod]
    public void Selector_BeInNamespace_PassAndFail() {
        new TypeSelector([typeof(Widget)]).Should().BeInNamespace("Jeninnet.Assertions.Tests.SelectorFixtures");
        new TypeSelector([typeof(Widget)]).Should().NotBeInNamespace("Nope");

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => new TypeSelector([typeof(Widget)]).Should().BeInNamespace("Nope"));
        Assert.AreEqual("Expected Jeninnet.Assertions.Tests.SelectorFixtures.Widget to be in namespace 'Nope', but it was not.", ex.Message);

        var notEx = Assert.ThrowsExactly<AssertionFailedException>(() => new TypeSelector([typeof(Widget)]).Should().NotBeInNamespace("Jeninnet.Assertions.Tests.SelectorFixtures"));
        Assert.AreEqual("Expected Jeninnet.Assertions.Tests.SelectorFixtures.Widget not to be in namespace 'Jeninnet.Assertions.Tests.SelectorFixtures', but it was.", notEx.Message);

        Assert.ThrowsExactly<ArgumentNullException>(() => new TypeSelector([typeof(Widget)]).Should().BeInNamespace(null!));
        Assert.ThrowsExactly<ArgumentException>(() => new TypeSelector([typeof(Widget)]).Should().BeInNamespace("  "));
    }

    [TestMethod]
    public void Selector_BeUnderNamespace_PassAndFail() {
        new TypeSelector([typeof(Widget)]).Should().BeUnderNamespace("Jeninnet.Assertions");
        new TypeSelector([typeof(Widget)]).Should().NotBeUnderNamespace("Nope");

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => new TypeSelector([typeof(Widget)]).Should().BeUnderNamespace("Nope"));
        Assert.AreEqual("Expected Jeninnet.Assertions.Tests.SelectorFixtures.Widget to be under namespace 'Nope', but it was not.", ex.Message);

        var notEx = Assert.ThrowsExactly<AssertionFailedException>(() => new TypeSelector([typeof(Widget)]).Should().NotBeUnderNamespace("Jeninnet.Assertions.Tests"));
        Assert.AreEqual("Expected Jeninnet.Assertions.Tests.SelectorFixtures.Widget not to be under namespace 'Jeninnet.Assertions.Tests', but it was.", notEx.Message);
    }
}

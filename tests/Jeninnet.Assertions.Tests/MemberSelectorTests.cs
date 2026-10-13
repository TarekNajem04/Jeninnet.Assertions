//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Tests;

[TestClass]
public class MemberSelectorTests {
    [TestMethod]
    public void Methods_PublicVoidChain_HasTwo() =>
        typeof(TinyService).Methods().ThatArePublic().ThatReturnVoid().Should().HaveCount(2);

    [TestMethod]
    public void Methods_AccessibilityFilters_Select() {
        typeof(OrderService).Methods().ThatArePublic().Should().HaveCount(8);
        typeof(OrderService).Methods().ThatArePrivate().Should().HaveCount(1);
        typeof(OrderService).Methods().ThatAreProtected().Should().HaveCount(0);
        typeof(OrderService).Methods().ThatAreInternal().Should().HaveCount(0);
        typeof(OrderService).Methods().ThatArePublicOrInternal().Should().HaveCount(8);
        typeof(OrderService).Methods().ThatAreProtectedOrInternal().Should().HaveCount(0);
    }

    [TestMethod]
    public void Methods_ReturnFilters_Select() {
        typeof(OrderService).Methods().ThatReturnVoid().Should().HaveCount(6);
        typeof(OrderService).Methods().ThatDoNotReturnVoid().Should().HaveCount(3);
        typeof(OrderService).Methods().ThatReturn<int>().Should().HaveCount(1);
        typeof(OrderService).Methods().ThatDoNotReturn<int>().Should().HaveCount(8);
        typeof(OrderService).Methods().ThatReturn<Task>().Should().HaveCount(1);
    }

    [TestMethod]
    public void Methods_ModifierFilters_Select() {
        typeof(OrderService).Methods().ThatAreStatic().Should().HaveCount(1);
        typeof(OrderService).Methods().ThatAreNotStatic().Should().HaveCount(8);
        typeof(OrderService).Methods().ThatAreVirtual().Should().HaveCount(1);
        typeof(OrderService).Methods().ThatAreNotVirtual().Should().HaveCount(8);
        typeof(OrderService).Methods().ThatAreAbstract().Should().HaveCount(0);
        typeof(OrderService).Methods().ThatAreNotAbstract().Should().HaveCount(9);
        typeof(OrderService).Methods().ThatAreAsync().Should().HaveCount(1);
        typeof(OrderService).Methods().ThatAreNotAsync().Should().HaveCount(8);
    }

    [TestMethod]
    public void Methods_AttributeFilters_Select() {
        typeof(OrderService).Methods().ThatAreDecoratedWith<MarkerAttribute>().Should().HaveCount(1);
        typeof(OrderService).Methods().ThatAreNotDecoratedWith<MarkerAttribute>().Should().HaveCount(8);
        typeof(OrderService).Methods().ThatSatisfy(static m => m.Name.StartsWith("Count", StringComparison.Ordinal)).Should().HaveCount(1);
    }

    [TestMethod]
    public void Methods_ReturnTypes_TransitionsToTypes() {
        typeof(OrderService).Methods().ThatAreAsync().ReturnTypes().Should().HaveCount(1);
        typeof(OrderService).Methods().ThatReturnVoid().ReturnTypes().ThatAreClasses().Should().HaveCount(0);
    }

    [TestMethod]
    public void Methods_Assertions_Pass() {
        typeof(OrderService).Methods().ThatAreDecoratedWith<MarkerAttribute>().Should().HaveAttribute<MarkerAttribute>();
        typeof(OrderService).Methods().ThatReturnVoid().Should().ReturnVoid();
        typeof(OrderService).Methods().ThatAreAsync().Should().BeAsync();
        typeof(OrderService).Methods().ThatAreVirtual().Should().BeVirtual();
        typeof(OrderService).Methods().ThatAreStatic().Should().BeStatic();
        typeof(OrderService).Methods().ThatArePublic().Should().HaveAccessModifier(AccessModifier.Public);
        typeof(OrderService).Methods().ThatArePublic().Should().HaveDeclaringType<OrderService>();
        var declaring = typeof(OrderService);
        typeof(OrderService).Methods().ThatArePublic().Should().HaveDeclaringType(declaring);
    }

    [TestMethod]
    public void Methods_Assertions_ReportFirstOffender() {
        var attribute = Assert.ThrowsExactly<AssertionFailedException>(
            () => typeof(OrderService).Methods().Should().HaveAttribute<MarkerAttribute>());
        Assert.Contains("to be decorated with MarkerAttribute", attribute.Message);

        var access = Assert.ThrowsExactly<AssertionFailedException>(
            () => typeof(OrderService).Methods().ThatArePublic().Should().HaveAccessModifier(AccessModifier.Private));
        Assert.Contains("to have access modifier Private", access.Message);

        var other = typeof(string);
        var declaring = Assert.ThrowsExactly<AssertionFailedException>(
            () => typeof(OrderService).Methods().ThatArePublic().Should().HaveDeclaringType(other));
        Assert.Contains("to be declared in System.String", declaring.Message);

        var count = Assert.ThrowsExactly<AssertionFailedException>(
            () => typeof(OrderService).Methods().ThatArePublic().Should().HaveCount(2));
        Assert.AreEqual("Expected selected members to contain 2 item(s), but found 8.", count.Message);
    }

    [TestMethod]
    public void Methods_NullGuards_Throw() {
        Assert.ThrowsExactly<ArgumentNullException>(() => ((Type)null!).Methods());
        Assert.ThrowsExactly<ArgumentNullException>(() => new MethodInfoSelector((Type)null!));
        Assert.ThrowsExactly<ArgumentNullException>(() => new MethodInfoSelector((IEnumerable<Type>)null!));
        Assert.ThrowsExactly<ArgumentException>(() => new MethodInfoSelector([typeof(OrderService), null!]));
        Assert.ThrowsExactly<ArgumentNullException>(
            () => typeof(OrderService).Methods().ThatSatisfy(null!));
        Assert.ThrowsExactly<ArgumentNullException>(
            () => typeof(OrderService).Methods().Should().HaveDeclaringType((Type)null!));
    }

    [TestMethod]
    public void Properties_AccessibilityFilters_Select() {
        typeof(OrderService).Properties().ThatArePublic().Should().HaveCount(3);
        typeof(OrderService).Properties().ThatArePrivate().Should().HaveCount(1);
        typeof(OrderService).Properties().ThatAreProtected().Should().HaveCount(0);
        typeof(OrderService).Properties().ThatArePublicOrInternal().Should().HaveCount(3);
    }

    [TestMethod]
    public void Properties_KindFilters_Select() {
        typeof(OrderService).Properties().ThatAreStatic().Should().HaveCount(1);
        typeof(OrderService).Properties().ThatAreNotStatic().Should().HaveCount(3);
        typeof(OrderService).Properties().ThatAreReadOnly().Should().HaveCount(1);
        typeof(OrderService).Properties().ThatAreWriteOnly().Should().HaveCount(0);
        typeof(OrderService).Properties().ThatAreWritable().Should().HaveCount(3);
        typeof(OrderService).Properties().OfType<string>().Should().HaveCount(2);
        typeof(OrderService).Properties().NotOfType<string>().Should().HaveCount(2);
        typeof(OrderService).Properties().OfType<string>().ReturnTypes().Should().HaveCount(2);
    }

    [TestMethod]
    public void Properties_Assertions_Pass() {
        typeof(OrderService).Properties().ThatAreReadOnly().Should().BeReadOnly();
        typeof(OrderService).Properties().ThatAreWritable().Should().BeWritable();
        typeof(OrderService).Properties().ThatArePublic().Should().HaveGetter();
        typeof(OrderService).Properties().ThatAreWritable().Should().HaveSetter();
        typeof(OrderService).Properties().ThatArePublic().Should().HaveAccessModifier(AccessModifier.Public);
        typeof(OrderService).Properties().ThatArePublic().Should().HaveDeclaringType<OrderService>();
    }

    [TestMethod]
    public void Properties_Assertions_ReportFirstOffender() {
        var readOnly = Assert.ThrowsExactly<AssertionFailedException>(
            () => typeof(OrderService).Properties().ThatArePublic().Should().BeReadOnly());
        Assert.Contains("to be read-only", readOnly.Message);

        var setter = Assert.ThrowsExactly<AssertionFailedException>(
            () => typeof(OrderService).Properties().ThatAreReadOnly().Should().HaveSetter());
        Assert.AreEqual("Expected property 'Total' to have a setter, but it does not.", setter.Message);
    }

    [TestMethod]
    public void Methods_FACompatVerbs_Pass() {
        typeof(OrderService).Methods().ThatArePublic().Should().Be(AccessModifier.Public);
        typeof(OrderService).Methods().ThatArePublic().Should().NotBe(AccessModifier.Private);
        typeof(OrderService).Methods().ThatAreDecoratedWith<MarkerAttribute>().Should().BeDecoratedWith<MarkerAttribute>();
        typeof(OrderService).Methods().ThatAreDecoratedWith<MarkerAttribute>().Should().BeDecoratedWith<MarkerAttribute>(static _ => true);
        typeof(OrderService).Methods().ThatAreNotDecoratedWith<MarkerAttribute>().Should().NotBeDecoratedWith<MarkerAttribute>();
        typeof(OrderService).Methods().ThatAreNotDecoratedWith<MarkerAttribute>().Should().NotBeDecoratedWith<MarkerAttribute>(static _ => false);
        typeof(OrderService).Methods().ThatAreNotAsync().Should().NotBeAsync();
        typeof(OrderService).Methods().ThatAreNotVirtual().Should().NotBeVirtual();
    }

    [TestMethod]
    public void Methods_FACompatVerbs_ReportOffender() {
        var access = Assert.ThrowsExactly<AssertionFailedException>(
            () => typeof(OrderService).Methods().ThatSatisfy(static m => m.Name == "PlaceOrder").Should().NotBe(AccessModifier.Public));
        Assert.AreEqual("Expected member 'PlaceOrder' not to have access modifier Public, but it does.", access.Message);

        var notAsyncEx = Assert.ThrowsExactly<AssertionFailedException>(
            () => typeof(OrderService).Methods().ThatSatisfy(static m => m.Name == "SaveAsync").Should().NotBeAsync());
        Assert.AreEqual("Expected method 'SaveAsync' not to be async, but it was.", notAsyncEx.Message);

        var virtualEx = Assert.ThrowsExactly<AssertionFailedException>(
            () => typeof(OrderService).Methods().ThatSatisfy(static m => m.Name == "Validate").Should().NotBeVirtual());
        Assert.AreEqual("Expected member 'Validate' not to be virtual, but it was.", virtualEx.Message);

        var decorated = Assert.ThrowsExactly<AssertionFailedException>(
            () => typeof(OrderService).Methods().ThatAreDecoratedWith<MarkerAttribute>().Should().NotBeDecoratedWith<MarkerAttribute>());
        Assert.AreEqual("Expected member 'Marked' not to be decorated with MarkerAttribute, but it was.", decorated.Message);

        var predicate = Assert.ThrowsExactly<AssertionFailedException>(
            () => typeof(OrderService).Methods().ThatAreDecoratedWith<MarkerAttribute>().Should().BeDecoratedWith<MarkerAttribute>(static _ => false));
        Assert.AreEqual("Expected member 'Marked' to be decorated with MarkerAttribute matching the predicate, but it was not.", predicate.Message);

        var notPredicate = Assert.ThrowsExactly<AssertionFailedException>(
            () => typeof(OrderService).Methods().ThatAreDecoratedWith<MarkerAttribute>().Should().NotBeDecoratedWith<MarkerAttribute>(static _ => true));
        Assert.AreEqual("Expected member 'Marked' not to be decorated with MarkerAttribute matching the predicate, but it was.", notPredicate.Message);

        Assert.ThrowsExactly<ArgumentNullException>(
            () => typeof(OrderService).Methods().Should().BeDecoratedWith<MarkerAttribute>((Expression<Func<MarkerAttribute, bool>>)null!));
        Assert.ThrowsExactly<ArgumentNullException>(
            () => typeof(OrderService).Methods().Should().NotBeDecoratedWith<MarkerAttribute>((Expression<Func<MarkerAttribute, bool>>)null!));
    }

    [TestMethod]
    public void Properties_FACompatVerbs_Pass() {
        typeof(VirtualService).Properties().ThatAreDecoratedWith<PropertyMarkerAttribute>().Should().BeDecoratedWith<PropertyMarkerAttribute>();
        typeof(VirtualService).Properties().ThatAreNotDecoratedWith<PropertyMarkerAttribute>().Should().NotBeDecoratedWith<PropertyMarkerAttribute>();
        typeof(VirtualService).Properties().ThatAreVirtual().Should().BeVirtual();
        typeof(VirtualService).Properties().ThatAreNotVirtual().Should().NotBeVirtual();
        typeof(OrderService).Properties().ThatAreReadOnly().Should().NotBeWritable();
    }

    [TestMethod]
    public void Properties_FACompatVerbs_ReportOffender() {
        var virtualEx = Assert.ThrowsExactly<AssertionFailedException>(
            () => typeof(VirtualService).Properties().ThatSatisfy(static p => p.Name == "Plain").Should().BeVirtual());
        Assert.AreEqual("Expected member 'Plain' to be virtual, but it was not.", virtualEx.Message);

        var notVirtual = Assert.ThrowsExactly<AssertionFailedException>(
            () => typeof(VirtualService).Properties().ThatSatisfy(static p => p.Name == "Label").Should().NotBeVirtual());
        Assert.AreEqual("Expected member 'Label' not to be virtual, but it was.", notVirtual.Message);

        var writable = Assert.ThrowsExactly<AssertionFailedException>(
            () => typeof(VirtualService).Properties().ThatSatisfy(static p => p.Name == "Plain").Should().NotBeWritable());
        Assert.AreEqual("Expected property 'Plain' not to be writable, but it was.", writable.Message);

        var decorated = Assert.ThrowsExactly<AssertionFailedException>(
            () => typeof(VirtualService).Properties().ThatAreDecoratedWith<PropertyMarkerAttribute>().Should().NotBeDecoratedWith<PropertyMarkerAttribute>());
        Assert.AreEqual("Expected member 'Label' not to be decorated with PropertyMarkerAttribute, but it was.", decorated.Message);

        var missing = Assert.ThrowsExactly<AssertionFailedException>(
            () => typeof(VirtualService).Properties().ThatAreNotDecoratedWith<PropertyMarkerAttribute>().Should().BeDecoratedWith<PropertyMarkerAttribute>());
        Assert.AreEqual("Expected member 'Plain' to be decorated with PropertyMarkerAttribute, but it was not.", missing.Message);
    }

    [TestMethod]
    public void Types_NestedTypes_Select() {
        typeof(OrderService).Types().Should().HaveCount(2);
        typeof(TinyService).Types().Should().HaveCount(0);
        Assert.ThrowsExactly<ArgumentNullException>(() => ((Type)null!).Types());
        Assert.ThrowsExactly<ArgumentNullException>(() => ((Type)null!).Properties());
    }

    [TestMethod]
    public void MethodSelector_CanBeEnumerated() {
        var selector = typeof(OrderService).Methods().ThatArePublic();
        List<MethodInfo> visited = [.. selector];

        Assert.IsTrue(visited.SequenceEqual(selector));
    }

    [TestMethod]
    public void SelectorAssertions_ExposeSelectedMembers() {
        var methods = typeof(OrderService).Methods();
        Assert.IsTrue(methods.SequenceEqual(methods.Should().SubjectMethods));

        var properties = typeof(OrderService).Properties();
        Assert.IsTrue(properties.SequenceEqual(properties.Should().SubjectProperties));
    }
}

//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Tests;

#pragma warning disable S2094, CA1812 // Reflection fixture: referenced by typeof only.
internal sealed class InternalReflectionFixture;
#pragma warning restore S2094, CA1812

internal static class ReflectionWorkerExtensions {
    public static string Twice(this string value) => value + value;
}

#pragma warning disable S1144, S2094, S2325, S1186, S1104, CA1812, CA1852, CA1822, IDE0051, RCS1213, RCS1170 // Reflection fixtures: shapes only for metadata assertions; empty bodies, public fields, and uninstantiated types are intentional.
[TestClass]
public class ReflectionAssertionTests {
    [AttributeUsage(AttributeTargets.All)]
    private sealed class MarkerAttribute(string label) : Attribute {
        public string Label { get; } = label;
    }

    private sealed class Worker : IWorker {
        public int Load => 1;

        public void Work() {
        }
    }

    private abstract class MemberBase {
        public abstract int AbstractProperty { get; }

        public virtual int VirtualProperty { get; set; }

        public abstract void AbstractMethod();

        public virtual void VirtualMethod() {
        }

        public virtual string SealedText() => "base";
    }

    private class MemberDerived : MemberBase {
        [Marker("property")]
        public string MarkedProperty { get; set; } = "";

        public required string RequiredProperty { get; init; }

        public string InitOnlyProperty { get; init; } = "";

        public static string StaticProperty { get; set; } = "";

        public string NormalProperty { get; set; } = "";

        public int this[int index] => index;

        public string this[string key, int index] => key + index;

        public override int AbstractProperty => 1;

        public override int VirtualProperty { get; set; }

        public override void AbstractMethod() {
        }

        public override void VirtualMethod() {
        }

        public sealed override string SealedText() => "derived";

        [Marker("method")]
        public void MarkedMethod() {
        }

        public void NormalMethod() {
        }

        public void Multi(int number, string text) => _ = number + text.Length;

        public T Echo<T>(T value) => value;

        [Marker("ctor")]
        public MemberDerived(double value) => _ = value;

        private MemberDerived() {
        }

        protected MemberDerived(int value) => _ = value;

        internal MemberDerived(string value) => _ = value;

        static MemberDerived() {
        }
    }

    private sealed record Person(string Name);

    private readonly record struct Point(int X, int Y);

    private sealed class ImmutableClass {
        public ImmutableClass(string name) => Name = name;

        public string Name { get; }
    }

    private sealed class MutableClass {
        public string Name { get; set; } = "";
    }

    private struct MutableStruct {
        public int Value;
    }

    private enum Color {
        Red,
    }

    private delegate void Callback();

    private static class StaticHolder;

    [Marker("type")]
    private sealed class MarkedType;

    private sealed class GenericHolder<T>(T value) {
        public T Value { get; } = value;
    }

    private class PrivateNested;

    protected class ProtectedNested;

    private static MethodInfo Method(Type type, string name) =>
        type.GetMethod(name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly)!;

    private static PropertyInfo Property(Type type, string name) =>
        type.GetProperty(name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly)!;

    private static ConstructorInfo Constructor(Type type, BindingFlags flags, params Type[] parameterTypes) =>
        type.GetConstructor(flags, binder: null, parameterTypes, modifiers: null)!;

    [TestMethod]
    public void TypeKinds_PassAndFail() {
        typeof(Person).Should().BeClass().And.BeRecord().And.NotBeInterface().And.NotBeEnum().And.NotBeDelegate().And.NotBeValueType();
        typeof(Point).Should().BeValueType().And.BeRecord().And.NotBeClass();
        typeof(IWorker).Should().BeInterface().And.NotBeClass();
        typeof(Color).Should().BeEnum().And.BeValueType().And.NotBeClass();
        typeof(Callback).Should().BeDelegate().And.NotBeRecord();
        typeof(string).Should().BeClass().And.NotBeRecord().And.NotBeEnum().And.NotBeDelegate();

        var wrongKind = Assert.ThrowsExactly<AssertionFailedException>(() => typeof(IWorker).Should().BeClass());
        Assert.AreEqual("Expected typeof(IWorker) to have kind class, but found interface.", wrongKind.Message);

        var notKind = Assert.ThrowsExactly<AssertionFailedException>(() => typeof(string).Should().NotBeClass());
        Assert.AreEqual("Expected typeof(string) not to have kind class, but it was.", notKind.Message);

        Type? missing = null;
        missing.Should().NotBeClass().And.NotBeInterface().And.NotBeEnum().And.NotBeDelegate().And.NotBeValueType();

        var nullKind = Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().BeClass());
        Assert.AreEqual("Expected missing to have kind class, but found <null>.", nullKind.Message);
    }

    [TestMethod]
    public void TypeGenericNestedAndImmutable() {
        typeof(GenericHolder<>).Should().BeGeneric().And.BeNested();
        typeof(GenericHolder<int>).Should().BeGeneric();
        typeof(string).Should().NotBeGeneric().And.NotBeNested();

        typeof(Person).Should().BeImmutable();
        typeof(Point).Should().BeImmutable();
        typeof(ImmutableClass).Should().BeImmutable();
        typeof(Color).Should().BeImmutable();
        typeof(MutableClass).Should().NotBeImmutable();
        typeof(MutableStruct).Should().NotBeImmutable();
        var mutableStruct = new MutableStruct { Value = 1 };
        Assert.AreEqual(1, mutableStruct.Value);
        typeof(IWorker).Should().NotBeImmutable();
        typeof(Callback).Should().NotBeImmutable();
        typeof(StaticHolder).Should().NotBeImmutable();

        var mutable = Assert.ThrowsExactly<AssertionFailedException>(() => typeof(MutableClass).Should().BeImmutable());
        Assert.AreEqual("Expected typeof(MutableClass) to have kind immutable type, but found class.", mutable.Message);

        var immutable = Assert.ThrowsExactly<AssertionFailedException>(() => typeof(Person).Should().NotBeImmutable());
        Assert.AreEqual("Expected typeof(Person) not to have kind immutable type, but it was.", immutable.Message);
    }

    [TestMethod]
    public void TypeAccessModifiers_PassAndFail() {
        typeof(string).Should().BePublic();
        typeof(InternalReflectionFixture).Should().BeInternal();
        typeof(PrivateNested).Should().BePrivate();
        typeof(ProtectedNested).Should().BeProtected();

        var publicFailure = Assert.ThrowsExactly<AssertionFailedException>(() => typeof(InternalReflectionFixture).Should().BePublic());
        Assert.AreEqual("Expected typeof(InternalReflectionFixture) to have Public accessibility, but found Internal.", publicFailure.Message);

        var privateFailure = Assert.ThrowsExactly<AssertionFailedException>(() => typeof(ProtectedNested).Should().BePrivate());
        Assert.AreEqual("Expected typeof(ProtectedNested) to have Private accessibility, but found Protected.", privateFailure.Message);
    }

    [TestMethod]
    public void TypeIdentity_PassAndFail() {
        typeof(Person).Should().BeSameAs(typeof(Person)).And.NotBeSameAs(typeof(Point));
        typeof(Person).Should().HaveName("Person");
        typeof(Person).Should().HaveFullName(typeof(Person).FullName!);
        typeof(Person).Should().HaveNamespace(typeof(Person).Namespace!);

        var sameAs = Assert.ThrowsExactly<AssertionFailedException>(() => typeof(Person).Should().BeSameAs(typeof(Point)));
        Assert.Contains("to be the same instance as", sameAs.Message);
        Assert.Contains("Point", sameAs.Message);

        var notSameAs = Assert.ThrowsExactly<AssertionFailedException>(() => typeof(Person).Should().NotBeSameAs(typeof(Person)));
        Assert.Contains("not to be the same instance as", notSameAs.Message);

        var name = Assert.ThrowsExactly<AssertionFailedException>(() => typeof(Person).Should().HaveName("Other"));
        Assert.AreEqual("Expected typeof(Person) to have name 'Other', but found 'Person'.", name.Message);

        var fullName = Assert.ThrowsExactly<AssertionFailedException>(() => typeof(Person).Should().HaveFullName("Other"));
        Assert.Contains("to have full name 'Other'", fullName.Message);

        var @namespace = Assert.ThrowsExactly<AssertionFailedException>(() => typeof(Person).Should().HaveNamespace("Other"));
        Assert.Contains("to be in namespace 'Other'", @namespace.Message);

        Assert.ThrowsExactly<ArgumentNullException>(() => typeof(Person).Should().HaveName(null!));
        Assert.ThrowsExactly<ArgumentNullException>(() => typeof(Person).Should().HaveFullName(null!));
    }

    [TestMethod]
    public void TypeHierarchy_PassAndFail() {
        typeof(object).Should().BeAssignableFrom<string>();
        typeof(IWorker).Should().BeAssignableFrom<Worker>();
        typeof(MemberDerived).Should().BeSubclassOf<MemberBase>();
        typeof(string).Should().BeSubclassOf<object>();
        typeof(MemberBase).Should().BeBaseOf<MemberDerived>();
        typeof(IWorker).Should().BeBaseOf<Worker>();
        typeof(object).Should().BeBaseOf<string>();
        typeof(MemberDerived).Should().HaveBaseType(typeof(MemberBase));

        var notAssignable = Assert.ThrowsExactly<AssertionFailedException>(() => typeof(Worker).Should().BeAssignableFrom<IWorker>());
        Assert.Contains("to be assignable from", notAssignable.Message);
        Assert.Contains("IWorker", notAssignable.Message);

        var sameClass = Assert.ThrowsExactly<AssertionFailedException>(() => typeof(MemberBase).Should().BeSubclassOf<MemberBase>());
        Assert.Contains("to be a subclass of", sameClass.Message);

        var interfaceSubclass = Assert.ThrowsExactly<AssertionFailedException>(() => typeof(Worker).Should().BeSubclassOf<IWorker>());
        Assert.Contains("to be a subclass of", interfaceSubclass.Message);

        var sameBase = Assert.ThrowsExactly<AssertionFailedException>(() => typeof(MemberBase).Should().BeBaseOf<MemberBase>());
        Assert.Contains("to be a base type of", sameBase.Message);

        var wrongBase = Assert.ThrowsExactly<AssertionFailedException>(() => typeof(MemberDerived).Should().HaveBaseType(typeof(object)));
        Assert.Contains("to have base type System.Object", wrongBase.Message);

        Assert.ThrowsExactly<ArgumentNullException>(() => typeof(MemberDerived).Should().HaveBaseType(null!));

        Type? missing = null;
        Assert.Contains("but found null", Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().BeAssignableFrom<string>()).Message);
        Assert.Contains("but found null", Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().BeSubclassOf<MemberBase>()).Message);
        Assert.Contains("but found null", Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().BeBaseOf<MemberDerived>()).Message);
    }

    [TestMethod]
    public void TypeGenericParameters_PassAndFail() {
        typeof(Dictionary<,>).Should().HaveGenericParameter(2).And.HaveGenericParameter("TKey").And.HaveGenericParameter("TValue");
        typeof(string).Should().HaveGenericParameter(0);

        var count = Assert.ThrowsExactly<AssertionFailedException>(() => typeof(Dictionary<,>).Should().HaveGenericParameter(3));
        Assert.Contains("to have 3 generic parameter(s)", count.Message);
        Assert.Contains("but found 2", count.Message);

        var name = Assert.ThrowsExactly<AssertionFailedException>(() => typeof(Dictionary<,>).Should().HaveGenericParameter("TMissing"));
        Assert.Contains("to declare a generic parameter named 'TMissing'", name.Message);

        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => typeof(Dictionary<,>).Should().HaveGenericParameter(-1));
        Assert.ThrowsExactly<ArgumentNullException>(() => typeof(Dictionary<,>).Should().HaveGenericParameter((string)null!));
    }

    [TestMethod]
    public void TypeDecoratorPredicate_PassAndFail() {
        typeof(MarkedType).Should().BeDecoratedWith<MarkerAttribute>(static attribute => attribute.Label == "type");
        typeof(Person).Should().NotBeDecoratedWith<MarkerAttribute>(static attribute => attribute.Label == "type");
        typeof(MarkedType).Should().NotBeDecoratedWith<MarkerAttribute>(static attribute => attribute.Label == "other");

        var notDecorated = Assert.ThrowsExactly<AssertionFailedException>(() => typeof(Person).Should().BeDecoratedWith<MarkerAttribute>(static attribute => attribute.Label == "type"));
        Assert.AreEqual("Expected typeof(Person) to be decorated with MarkerAttribute, but found Jeninnet.Assertions.Tests.ReflectionAssertionTests+Person.", notDecorated.Message);

        var decorated = Assert.ThrowsExactly<AssertionFailedException>(() => typeof(MarkedType).Should().NotBeDecoratedWith<MarkerAttribute>(static attribute => attribute.Label == "type"));
        Assert.AreEqual("Expected typeof(MarkedType) not to be decorated with MarkerAttribute matching the predicate, but it was.", decorated.Message);

        Assert.ThrowsExactly<ArgumentNullException>(() => typeof(MarkedType).Should().NotBeDecoratedWith<MarkerAttribute>((Expression<Func<MarkerAttribute, bool>>)null!));
    }

    [TestMethod]
    public void MethodModifiers_PassAndFail() {
        var normal = Method(typeof(MemberDerived), nameof(MemberDerived.NormalMethod));
        var baseAbstract = Method(typeof(MemberBase), nameof(MemberBase.AbstractMethod));
        var derivedVirtual = Method(typeof(MemberDerived), nameof(MemberBase.VirtualMethod));
        var baseVirtual = Method(typeof(MemberBase), nameof(MemberBase.VirtualMethod));
        var final = Method(typeof(MemberDerived), nameof(MemberDerived.SealedText));
        var echo = Method(typeof(MemberDerived), nameof(MemberDerived.Echo));
        var extension = typeof(ReflectionWorkerExtensions).GetMethod(nameof(ReflectionWorkerExtensions.Twice), BindingFlags.Public | BindingFlags.Static)!;

        baseAbstract.Should().BeAbstract().And.NotBeOverride().And.NotBeFinal().And.NotBeGeneric().And.NotBeExtension();
        derivedVirtual.Should().BeOverride().And.NotBeAbstract();
        baseVirtual.Should().NotBeOverride();
        final.Should().BeFinal().And.NotBeAbstract();
        echo.Should().BeGeneric().And.NotBeExtension();
        extension.Should().BeExtension().And.NotBeGeneric();
        normal.Should().NotBeAbstract().And.NotBeOverride().And.NotBeFinal().And.NotBeGeneric().And.NotBeExtension();

        Assert.AreEqual("Expected normal to be abstract, but it was not.", Assert.ThrowsExactly<AssertionFailedException>(() => normal.Should().BeAbstract()).Message);
        Assert.AreEqual("Expected baseAbstract not to be abstract, but it was.", Assert.ThrowsExactly<AssertionFailedException>(() => baseAbstract.Should().NotBeAbstract()).Message);
        Assert.AreEqual("Expected normal to be an override, but it was not.", Assert.ThrowsExactly<AssertionFailedException>(() => normal.Should().BeOverride()).Message);
        Assert.AreEqual("Expected derivedVirtual not to be an override, but it was.", Assert.ThrowsExactly<AssertionFailedException>(() => derivedVirtual.Should().NotBeOverride()).Message);
        Assert.AreEqual("Expected normal to be final (a sealed override), but it was not.", Assert.ThrowsExactly<AssertionFailedException>(() => normal.Should().BeFinal()).Message);
        Assert.AreEqual("Expected final not to be final, but it was.", Assert.ThrowsExactly<AssertionFailedException>(() => final.Should().NotBeFinal()).Message);
        Assert.AreEqual("Expected normal to be generic, but it was not.", Assert.ThrowsExactly<AssertionFailedException>(() => normal.Should().BeGeneric()).Message);
        Assert.AreEqual("Expected echo not to be generic, but it was.", Assert.ThrowsExactly<AssertionFailedException>(() => echo.Should().NotBeGeneric()).Message);
        Assert.AreEqual("Expected normal to be an extension method, but it was not.", Assert.ThrowsExactly<AssertionFailedException>(() => normal.Should().BeExtension()).Message);
        Assert.AreEqual("Expected extension not to be an extension method, but it was.", Assert.ThrowsExactly<AssertionFailedException>(() => extension.Should().NotBeExtension()).Message);

        MethodInfo? missing = null;
        missing.Should().NotBeAbstract().And.NotBeOverride().And.NotBeFinal().And.NotBeGeneric().And.NotBeExtension();
        Assert.AreEqual("Expected missing to be abstract, but it was not.", Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().BeAbstract()).Message);
    }

    [TestMethod]
    public void MethodParametersAndDecorators_PassAndFail() {
        var multi = Method(typeof(MemberDerived), nameof(MemberDerived.Multi));
        var normal = Method(typeof(MemberDerived), nameof(MemberDerived.NormalMethod));
        var marked = Method(typeof(MemberDerived), nameof(MemberDerived.MarkedMethod));

        multi.Should().HaveParameter("number", typeof(int)).And.HaveParameters([typeof(int), typeof(string)]);
        normal.Should().HaveParameters([]);

        var parameters = Assert.ThrowsExactly<AssertionFailedException>(() => multi.Should().HaveParameters([typeof(string)]));
        Assert.AreEqual("Expected multi to have parameters (String), but found (Int32, String).", parameters.Message);

        var missingParameters = Assert.ThrowsExactly<AssertionFailedException>(() => normal.Should().HaveParameters([typeof(int)]));
        Assert.AreEqual("Expected normal to have parameters (Int32), but found (none).", missingParameters.Message);

        marked.Should().BeDecoratedWith<MarkerAttribute>(static attribute => attribute.Label == "method");
        normal.Should().NotBeDecoratedWith<MarkerAttribute>(static attribute => attribute.Label == "method");
        marked.Should().NotBeDecoratedWith<MarkerAttribute>(static attribute => attribute.Label == "other");

        var notDecorated = Assert.ThrowsExactly<AssertionFailedException>(() => normal.Should().BeDecoratedWith<MarkerAttribute>(static attribute => attribute.Label == "method"));
        Assert.AreEqual("Expected normal to be decorated with MarkerAttribute matching the predicate, but it did not.", notDecorated.Message);

        var decorated = Assert.ThrowsExactly<AssertionFailedException>(() => marked.Should().NotBeDecoratedWith<MarkerAttribute>(static attribute => attribute.Label == "method"));
        Assert.AreEqual("Expected marked not to be decorated with MarkerAttribute matching the predicate, but it did.", decorated.Message);

        Assert.ThrowsExactly<ArgumentNullException>(() => multi.Should().HaveParameters(null!));
        Assert.ThrowsExactly<ArgumentNullException>(() => marked.Should().BeDecoratedWith<MarkerAttribute>((Expression<Func<MarkerAttribute, bool>>)null!));
        Assert.ThrowsExactly<ArgumentNullException>(() => marked.Should().NotBeDecoratedWith<MarkerAttribute>((Expression<Func<MarkerAttribute, bool>>)null!));
    }

    [TestMethod]
    public void MethodIdentity_PassAndFail() {
        var marked = Method(typeof(MemberDerived), nameof(MemberDerived.MarkedMethod));
        var normal = Method(typeof(MemberDerived), nameof(MemberDerived.NormalMethod));

        marked.Should().BeSameAs(marked).And.NotBeSameAs(normal);

        var sameAs = Assert.ThrowsExactly<AssertionFailedException>(() => marked.Should().BeSameAs(normal));
        Assert.AreEqual("Expected marked to be the same instance as NormalMethod, but it was not.", sameAs.Message);

        var notSameAs = Assert.ThrowsExactly<AssertionFailedException>(() => marked.Should().NotBeSameAs(marked));
        Assert.AreEqual("Expected marked not to be the same instance as MarkedMethod, but it was.", notSameAs.Message);
    }

    [TestMethod]
    public void PropertyModifiers_PassAndFail() {
        var staticProperty = Property(typeof(MemberDerived), nameof(MemberDerived.StaticProperty));
        var normal = Property(typeof(MemberDerived), nameof(MemberDerived.NormalProperty));
        var abstractProperty = Property(typeof(MemberBase), nameof(MemberBase.AbstractProperty));
        var virtualProperty = Property(typeof(MemberDerived), nameof(MemberBase.VirtualProperty));
        var baseVirtual = Property(typeof(MemberBase), nameof(MemberBase.VirtualProperty));

        staticProperty.Should().BeStatic().And.NotBeAbstract().And.NotBeOverride();
        normal.Should().NotBeStatic().And.NotBeAbstract().And.NotBeOverride();
        abstractProperty.Should().BeAbstract().And.NotBeOverride();
        virtualProperty.Should().BeOverride().And.NotBeAbstract();
        baseVirtual.Should().NotBeOverride();

        Assert.AreEqual("Expected normal to be static, but it was not.", Assert.ThrowsExactly<AssertionFailedException>(() => normal.Should().BeStatic()).Message);
        Assert.AreEqual("Expected staticProperty not to be static, but it was.", Assert.ThrowsExactly<AssertionFailedException>(() => staticProperty.Should().NotBeStatic()).Message);
        Assert.AreEqual("Expected normal to be abstract, but it was not.", Assert.ThrowsExactly<AssertionFailedException>(() => normal.Should().BeAbstract()).Message);
        Assert.AreEqual("Expected abstractProperty not to be abstract, but it was.", Assert.ThrowsExactly<AssertionFailedException>(() => abstractProperty.Should().NotBeAbstract()).Message);
        Assert.AreEqual("Expected normal to be an override, but it was not.", Assert.ThrowsExactly<AssertionFailedException>(() => normal.Should().BeOverride()).Message);
        Assert.AreEqual("Expected virtualProperty not to be an override, but it was.", Assert.ThrowsExactly<AssertionFailedException>(() => virtualProperty.Should().NotBeOverride()).Message);

        PropertyInfo? missing = null;
        missing.Should().NotBeStatic().And.NotBeAbstract().And.NotBeOverride();
        Assert.AreEqual("Expected missing to be static, but it was not.", Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().BeStatic()).Message);
    }

    [TestMethod]
    public void PropertyRequiredAndInitOnly_PassAndFail() {
        var required = Property(typeof(MemberDerived), nameof(MemberDerived.RequiredProperty));
        var initOnly = Property(typeof(MemberDerived), nameof(MemberDerived.InitOnlyProperty));
        var normal = Property(typeof(MemberDerived), nameof(MemberDerived.NormalProperty));

        required.Should().BeRequired().And.BeInitOnly();
        initOnly.Should().NotBeRequired().And.BeInitOnly();
        normal.Should().NotBeRequired().And.NotBeInitOnly();

        Assert.AreEqual("Expected normal to be required, but it was not.", Assert.ThrowsExactly<AssertionFailedException>(() => normal.Should().BeRequired()).Message);
        Assert.AreEqual("Expected required not to be required, but it was.", Assert.ThrowsExactly<AssertionFailedException>(() => required.Should().NotBeRequired()).Message);
        Assert.AreEqual("Expected normal to be init-only, but it was not.", Assert.ThrowsExactly<AssertionFailedException>(() => normal.Should().BeInitOnly()).Message);
        Assert.AreEqual("Expected initOnly not to be init-only, but it was.", Assert.ThrowsExactly<AssertionFailedException>(() => initOnly.Should().NotBeInitOnly()).Message);

        PropertyInfo? missing = null;
        Assert.AreEqual("Expected missing to be required, but it was not.", Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().BeRequired()).Message);
        missing.Should().NotBeRequired().And.NotBeInitOnly();
    }

    [TestMethod]
    public void PropertyIndexParameters_PassAndFail() {
        var intIndexer = typeof(MemberDerived).GetProperty("Item", [typeof(int)])!;
        var stringIndexer = typeof(MemberDerived).GetProperty("Item", [typeof(string), typeof(int)])!;
        var normal = Property(typeof(MemberDerived), nameof(MemberDerived.NormalProperty));

        intIndexer.Should().HaveIndexParameters([typeof(int)]);
        stringIndexer.Should().HaveIndexParameters([typeof(string), typeof(int)]);
        normal.Should().HaveIndexParameters([]);

        var indexParameters = Assert.ThrowsExactly<AssertionFailedException>(() => intIndexer.Should().HaveIndexParameters([typeof(string)]));
        Assert.AreEqual("Expected intIndexer to have index parameters (String), but found (Int32).", indexParameters.Message);

        var noIndexParameters = Assert.ThrowsExactly<AssertionFailedException>(() => normal.Should().HaveIndexParameters([typeof(int)]));
        Assert.AreEqual("Expected normal to have index parameters (Int32), but found (none).", noIndexParameters.Message);

        Assert.ThrowsExactly<ArgumentNullException>(() => intIndexer.Should().HaveIndexParameters(null!));
    }

    [TestMethod]
    public void PropertyDecoratorsAndIdentity_PassAndFail() {
        var marked = Property(typeof(MemberDerived), nameof(MemberDerived.MarkedProperty));
        var normal = Property(typeof(MemberDerived), nameof(MemberDerived.NormalProperty));

        marked.Should().BeDecoratedWith<MarkerAttribute>(static attribute => attribute.Label == "property");
        normal.Should().NotBeDecoratedWith<MarkerAttribute>(static attribute => attribute.Label == "property");

        var notDecorated = Assert.ThrowsExactly<AssertionFailedException>(() => normal.Should().BeDecoratedWith<MarkerAttribute>(static attribute => attribute.Label == "property"));
        Assert.AreEqual("Expected normal to be decorated with MarkerAttribute matching the predicate, but it did not.", notDecorated.Message);

        var decorated = Assert.ThrowsExactly<AssertionFailedException>(() => marked.Should().NotBeDecoratedWith<MarkerAttribute>(static attribute => attribute.Label == "property"));
        Assert.AreEqual("Expected marked not to be decorated with MarkerAttribute matching the predicate, but it did.", decorated.Message);

        marked.Should().BeSameAs(marked).And.NotBeSameAs(normal);
        Assert.AreEqual("Expected marked to be the same instance as NormalProperty, but it was not.", Assert.ThrowsExactly<AssertionFailedException>(() => marked.Should().BeSameAs(normal)).Message);
        Assert.AreEqual("Expected marked not to be the same instance as MarkedProperty, but it was.", Assert.ThrowsExactly<AssertionFailedException>(() => marked.Should().NotBeSameAs(marked)).Message);

        Assert.ThrowsExactly<ArgumentNullException>(() => normal.Should().BeDecoratedWith<MarkerAttribute>((Expression<Func<MarkerAttribute, bool>>)null!));
        Assert.ThrowsExactly<ArgumentNullException>(() => normal.Should().NotBeDecoratedWith<MarkerAttribute>((Expression<Func<MarkerAttribute, bool>>)null!));
    }

    [TestMethod]
    public void ConstructorModifiers_PassAndFail() {
        var privateCtor = Constructor(typeof(MemberDerived), BindingFlags.NonPublic | BindingFlags.Instance);
        var protectedCtor = Constructor(typeof(MemberDerived), BindingFlags.NonPublic | BindingFlags.Instance, typeof(int));
        var internalCtor = Constructor(typeof(MemberDerived), BindingFlags.NonPublic | BindingFlags.Instance, typeof(string));
        var publicCtor = Constructor(typeof(MemberDerived), BindingFlags.Public | BindingFlags.Instance, typeof(double));
        var staticCtor = typeof(MemberDerived).TypeInitializer!;

        privateCtor.Should().BePrivate().And.NotBePublic();
        protectedCtor.Should().BeProtected().And.NotBePublic();
        internalCtor.Should().BeInternal().And.NotBePublic();
        publicCtor.Should().BePublic();
        staticCtor.Should().BeStatic();

        Assert.AreEqual("Expected publicCtor to be private, but it was Public.", Assert.ThrowsExactly<AssertionFailedException>(() => publicCtor.Should().BePrivate()).Message);
        Assert.AreEqual("Expected privateCtor to be protected, but it was Private.", Assert.ThrowsExactly<AssertionFailedException>(() => privateCtor.Should().BeProtected()).Message);
        Assert.AreEqual("Expected publicCtor to be internal, but it was Public.", Assert.ThrowsExactly<AssertionFailedException>(() => publicCtor.Should().BeInternal()).Message);
        Assert.AreEqual("Expected publicCtor not to be public, but it was.", Assert.ThrowsExactly<AssertionFailedException>(() => publicCtor.Should().NotBePublic()).Message);
        Assert.AreEqual("Expected publicCtor to be a static constructor, but it was not.", Assert.ThrowsExactly<AssertionFailedException>(() => publicCtor.Should().BeStatic()).Message);

        ConstructorInfo? missing = null;
        missing.Should().NotBePublic();
        Assert.AreEqual("Expected missing to be private, but found null.", Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().BePrivate()).Message);
        Assert.AreEqual("Expected missing to be a static constructor, but found null.", Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().BeStatic()).Message);
    }

    [TestMethod]
    public void ConstructorParametersDecoratorsAndIdentity_PassAndFail() {
        var privateCtor = Constructor(typeof(MemberDerived), BindingFlags.NonPublic | BindingFlags.Instance);
        var publicCtor = Constructor(typeof(MemberDerived), BindingFlags.Public | BindingFlags.Instance, typeof(double));

        publicCtor.Should().HaveParameters([typeof(double)]).And.HaveParameter("value", typeof(double));
        privateCtor.Should().HaveParameters([]);

        var parameters = Assert.ThrowsExactly<AssertionFailedException>(() => privateCtor.Should().HaveParameters([typeof(double)]));
        Assert.AreEqual("Expected privateCtor to have parameters (Double), but found (none).", parameters.Message);

        publicCtor.Should().BeDecoratedWith<MarkerAttribute>(static attribute => attribute.Label == "ctor");
        privateCtor.Should().NotBeDecoratedWith<MarkerAttribute>(static attribute => attribute.Label == "ctor");

        var notDecorated = Assert.ThrowsExactly<AssertionFailedException>(() => privateCtor.Should().BeDecoratedWith<MarkerAttribute>(static attribute => attribute.Label == "ctor"));
        Assert.AreEqual("Expected privateCtor to be decorated with MarkerAttribute matching the predicate, but it did not.", notDecorated.Message);

        var decorated = Assert.ThrowsExactly<AssertionFailedException>(() => publicCtor.Should().NotBeDecoratedWith<MarkerAttribute>(static attribute => attribute.Label == "ctor"));
        Assert.AreEqual("Expected publicCtor not to be decorated with MarkerAttribute matching the predicate, but it did.", decorated.Message);

        publicCtor.Should().BeSameAs(publicCtor).And.NotBeSameAs(privateCtor);
        Assert.AreEqual("Expected publicCtor to be the same instance as .ctor, but it was not.", Assert.ThrowsExactly<AssertionFailedException>(() => publicCtor.Should().BeSameAs(privateCtor)).Message);
        Assert.AreEqual("Expected publicCtor not to be the same instance as .ctor, but it was.", Assert.ThrowsExactly<AssertionFailedException>(() => publicCtor.Should().NotBeSameAs(publicCtor)).Message);

        Assert.ThrowsExactly<ArgumentNullException>(() => publicCtor.Should().HaveParameters(null!));
        Assert.ThrowsExactly<ArgumentNullException>(() => publicCtor.Should().BeDecoratedWith<MarkerAttribute>((Expression<Func<MarkerAttribute, bool>>)null!));
        Assert.ThrowsExactly<ArgumentNullException>(() => publicCtor.Should().NotBeDecoratedWith<MarkerAttribute>((Expression<Func<MarkerAttribute, bool>>)null!));
    }
}
#pragma warning restore S1144, S2094, S2325, S1186, S1104, CA1812, CA1852, CA1822, IDE0051, RCS1213, RCS1170

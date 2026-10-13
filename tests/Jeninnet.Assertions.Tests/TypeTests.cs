//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Tests;

[TestClass]
public class TypeTests {
    private sealed class Sample(string name) {
        public Sample() : this("default") {
        }

        public string Name { get; } = name;

        public int Add(int x, int y) => x + y + Name.Length;

        [Step]
        public string Greet() => $"hi {Name}";
    }

    [AttributeUsage(AttributeTargets.Class)]
    private sealed class MarkerAttribute : Attribute;

    [AttributeUsage(AttributeTargets.Method)]
    private sealed class StepAttribute : Attribute;

    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    private sealed class FinalAttribute : Attribute;

    [Marker]
    private sealed class Marked;

    [Final]
    internal class FinalBase;

#pragma warning disable S2094 // Empty by design: provides an undecorated subclass for BeDecoratedWithOrInherit tests.
    internal sealed class FinalDerived : FinalBase;
#pragma warning restore S2094

#pragma warning disable S2094 // Empty by design: accessibility-mapping fixtures for HaveAccessModifier tests.
    protected internal class FamilyOrAssemblySample;

    private protected sealed class FamilyAndAssemblySample;
#pragma warning restore S2094

    private class SampleBase {
        protected SampleBase() {
        }

        protected internal static int Helper() => Secret();

        private protected static int PHelper() => 2;

        private static int Secret() => 1;
    }

    private sealed class DerivedSample : SampleBase {
        public static int UseHelpers() => Helper() + PHelper();
    }

    [AttributeUsage(AttributeTargets.Class)]
    private sealed class TierAttribute(string level) : Attribute {
        public string Level { get; } = level;
    }

    [Tier("Gold")]
#pragma warning disable CA1852 // Unsealed by design: hosts virtual members for BeVirtual tests.
    private class Vault : IWorker {
        public Vault() => Guarded = 7;

        public int this[int index] => index;

        public int Counter { get; set; }

#pragma warning disable S2325, CA1822, S2376 // Write-only by design: fixture for NotBeReadable tests; no instance state to touch and no getter by definition.
        public string Secret { set => _ = value; }
#pragma warning restore S2325, CA1822, S2376

        public int Guarded { get; private set; }

        public virtual int Level { get; }

        public virtual string Label() => "vault";

        public async Task<string> FetchAsync() {
            await Task.Delay(1);
            return Label();
        }

        public static implicit operator int(Vault vault) => vault.Counter;

        public static explicit operator string(Vault vault) => vault.Label();

        void IWorker.Work() {
        }

        int IWorker.Load => 1;
    }
#pragma warning restore CA1852

    private sealed class NoDefault(int value) {
        public int Value { get; } = value;
    }

    private static class StaticVault {
        public static int Answer() => 42;
    }

    [TestMethod]
    public void HaveMethod_ExposesWhich() {
        var method = typeof(Sample).Should().HaveMethod("Add").Which;
        method.Should().Return<int>().And.HaveParameter("x", typeof(int));
        Assert.AreEqual(1 + 2 + "default".Length, new Sample("default").Add(1, 2));
    }

    [TestMethod]
    public void HaveMethod_WithSignature_PassAndFail() {
        typeof(Sample).Should().HaveMethod("Add", [typeof(int), typeof(int)]);

        var ex = Assert.ThrowsExactly<AssertionFailedException>(
            () => typeof(Sample).Should().HaveMethod("Add", [typeof(string)]));
        Assert.AreEqual("Expected typeof(Sample) to have method 'Add(String)', but it was not found.", ex.Message);
    }

    [TestMethod]
    public void NotHaveMethod_PassAndFail() {
        typeof(Sample).Should().NotHaveMethod("Missing");

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => typeof(Sample).Should().NotHaveMethod("Add"));
        Assert.AreEqual("Expected typeof(Sample) not to have method 'Add', but it was found.", ex.Message);
    }

    [TestMethod]
    public void HaveProperty_ExposesWhich() {
        var property = typeof(Sample).Should().HaveProperty("Name").Which;
        property.Should().Return<string>().And.BeReadable();
    }

    [TestMethod]
    public void BeSealed_BeAbstract() {
        typeof(Sample).Should().BeSealed().And.NotBeAbstract();

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => typeof(Sample).Should().BeAbstract());
        Assert.AreEqual("Expected typeof(Sample) to be abstract, but it was not.", ex.Message);
    }

    [TestMethod]
    public void BeAssignableTo_PassAndFail() {
        typeof(Sample).Should().BeAssignableTo<object>().And.NotBeAssignableTo<int>();

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => typeof(Sample).Should().BeAssignableTo<int>());
        Assert.Contains("to be assignable to Int32", ex.Message);
        Assert.Contains("Sample", ex.Message);
    }

    [TestMethod]
    public void BeDecoratedWith_PassAndFail() {
        typeof(Marked).Should().BeDecoratedWith<MarkerAttribute>().And.NotBeDecoratedWith<StepAttribute>();

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => typeof(Sample).Should().BeDecoratedWith<MarkerAttribute>());
        Assert.Contains("to be decorated with MarkerAttribute", ex.Message);
    }

    [TestMethod]
    public void HaveConstructor_PassAndFail() {
        typeof(Sample).Should().HaveConstructor().Which.Should().BePublic();
        typeof(Sample).Should().HaveConstructor(typeof(string)).Which.Should().HaveParameter("name", typeof(string));

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => typeof(Sample).Should().HaveConstructor(typeof(int)));
        Assert.Contains("to have constructor", ex.Message);
    }

    [TestMethod]
    public void MethodInfo_Details() {
        var method = typeof(Sample).Should().HaveMethod("Add").Which;
        method.Should().Return<int>();

        var wrongReturn = Assert.ThrowsExactly<AssertionFailedException>(() => method.Should().Return<string>());
        Assert.AreEqual("Expected method to have return type String, but found Int32.", wrongReturn.Message);

        var missingParameter = Assert.ThrowsExactly<AssertionFailedException>(() => method.Should().HaveParameter("z", typeof(int)));
        Assert.AreEqual("Expected method to have parameter 'z' of type Int32, but it was not found.", missingParameter.Message);
    }

    [TestMethod]
    public void MethodInfo_VirtualStaticDecorated() {
        var greet = typeof(Sample).Should().HaveMethod("Greet").Which;
        greet.Should().BeDecoratedWith<StepAttribute>();
        Assert.AreEqual("hi x", new Sample("x").Greet());

        var add = typeof(Sample).Should().HaveMethod("Add").Which;
        var notVirtual = Assert.ThrowsExactly<AssertionFailedException>(() => add.Should().BeVirtual());
        Assert.AreEqual("Expected add to be virtual, but it was not.", notVirtual.Message);

        var notStatic = Assert.ThrowsExactly<AssertionFailedException>(() => add.Should().BeStatic());
        Assert.AreEqual("Expected add to be static, but it was not.", notStatic.Message);
    }

    [TestMethod]
    public void PropertyInfo_Writable_FailsForGetterOnly() {
        var property = typeof(Sample).Should().HaveProperty("Name").Which;
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => property.Should().BeWritable());
        Assert.AreEqual("Expected property to be writable, but it was not.", ex.Message);
    }

    [TestMethod]
    public void NullType_FailsPositives_PassesNegatives() {
        Type? missing = null;
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().HaveMethod("Add"));
        Assert.AreEqual("Expected missing to have method 'Add', but it was not found.", ex.Message);

        missing.Should().NotHaveMethod("Add").And.NotHaveProperty("Name");
        var notNull = Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().NotBeNull());
        Assert.AreEqual("Expected missing not to be null, but found null.", notNull.Message);
    }

    [TestMethod]
    public void Assembly_DefineType_Reference() {
        var assembly = typeof(Sample).Assembly;
        assembly.Should().DefineType(typeof(Sample).FullName!).Which.FullName.Should().Be(typeof(Sample).FullName);
        assembly.Should().NotDefineType("No.Such.Type");
        assembly.Should().Reference(typeof(Assert).Assembly);
    }

    [TestMethod]
    public void Assembly_NotReference_PassAndFail() {
        var library = typeof(AssertionFailedException).Assembly;
        var tests = typeof(Sample).Assembly;
        library.Should().NotReference(tests);

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => tests.Should().NotReference(library));
        Assert.AreEqual("Expected tests not to reference assembly 'Jeninnet.Assertions', but it was found.", ex.Message);

        Assembly? missing = null;
        var nullEx = Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().NotReference(library));
        Assert.AreEqual("Expected missing not to reference assembly 'Jeninnet.Assertions', but found null.", nullEx.Message);

        Assert.ThrowsExactly<ArgumentNullException>(() => tests.Should().NotReference(null!));
    }

    [TestMethod]
    public void HaveAccessModifier_Type_PassAndFail() {
        typeof(Sample).Should().HaveAccessModifier(AccessModifier.Private);
        typeof(string).Should().HaveAccessModifier(AccessModifier.Public);
        typeof(FinalDerived).Should().HaveAccessModifier(AccessModifier.Internal);
        typeof(FamilyOrAssemblySample).Should().HaveAccessModifier(AccessModifier.ProtectedInternal);
        typeof(FamilyAndAssemblySample).Should().HaveAccessModifier(AccessModifier.PrivateProtected);

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => typeof(Sample).Should().HaveAccessModifier(AccessModifier.Public));
        Assert.AreEqual("Expected typeof(Sample) to have Public accessibility, but found Private.", ex.Message);

        Type? missing = null;
        var nullEx = Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().HaveAccessModifier(AccessModifier.Public));
        Assert.AreEqual("Expected missing to have Public accessibility, but found <null>.", nullEx.Message);
    }

    [TestMethod]
    public void HaveAccessModifier_MethodAndProperty() {
        var add = typeof(Sample).Should().HaveMethod("Add").Which;
        add.Should().HaveAccessModifier(AccessModifier.Public);

        var secret = typeof(SampleBase).GetMethod("Secret", BindingFlags.NonPublic | BindingFlags.Static)!;
        secret.Should().HaveAccessModifier(AccessModifier.Private);

        var helper = typeof(SampleBase).GetMethod("Helper", BindingFlags.NonPublic | BindingFlags.Static)!;
        helper.Should().HaveAccessModifier(AccessModifier.ProtectedInternal);

        var phelper = typeof(SampleBase).GetMethod("PHelper", BindingFlags.NonPublic | BindingFlags.Static)!;
        phelper.Should().HaveAccessModifier(AccessModifier.PrivateProtected);

        var finalize = typeof(object).GetMethod("Finalize", BindingFlags.NonPublic | BindingFlags.Instance)!;
        finalize.Should().HaveAccessModifier(AccessModifier.Protected);

        Assert.AreEqual(3, DerivedSample.UseHelpers());

        var wrong = Assert.ThrowsExactly<AssertionFailedException>(() => add.Should().HaveAccessModifier(AccessModifier.Private));
        Assert.AreEqual("Expected add to have Private accessibility, but found Public.", wrong.Message);

        var property = typeof(Sample).Should().HaveProperty("Name").Which;
        property.Should().HaveAccessModifier(AccessModifier.Public);
    }

    [TestMethod]
    public void BeDecoratedWithOrInherit_FindsBaseAttribute() {
        typeof(Marked).Should().BeDecoratedWithOrInherit<MarkerAttribute>();
        typeof(FinalDerived).Should().BeDecoratedWithOrInherit<FinalAttribute>();

        var missing = Assert.ThrowsExactly<AssertionFailedException>(() => typeof(FinalDerived).Should().BeDecoratedWith<FinalAttribute>());
        Assert.AreEqual("Expected typeof(FinalDerived) to be decorated with FinalAttribute, but found Jeninnet.Assertions.Tests.TypeTests+FinalDerived.", missing.Message);

        var absent = Assert.ThrowsExactly<AssertionFailedException>(() => typeof(Sample).Should().BeDecoratedWithOrInherit<FinalAttribute>());
        Assert.AreEqual("Expected typeof(Sample) to be decorated with FinalAttribute or inherit it, but found Jeninnet.Assertions.Tests.TypeTests+Sample.", absent.Message);
    }

    [TestMethod]
    public void Implement_PassAndFail() {
        typeof(Vault).Should().Implement<IWorker>().And.NotImplement<IDisposable>();

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => typeof(Vault).Should().Implement<IDisposable>());
        Assert.AreEqual("Expected typeof(Vault) to implement System.IDisposable, but found Jeninnet.Assertions.Tests.TypeTests+Vault.", ex.Message);

        var notEx = Assert.ThrowsExactly<AssertionFailedException>(() => typeof(Vault).Should().NotImplement<IWorker>());
        Assert.AreEqual("Expected typeof(Vault) not to implement Jeninnet.Assertions.Tests.SelectorFixtures.IWorker, but it does.", notEx.Message);

#pragma warning disable CA2263 // Intentional: exercises the non-generic overload; generics covered above.
        typeof(Vault).Should().Implement(typeof(IWorker));
#pragma warning restore CA2263
        Assert.ThrowsExactly<ArgumentNullException>(() => typeof(Vault).Should().Implement((Type)null!));
        Assert.ThrowsExactly<ArgumentNullException>(() => typeof(Vault).Should().NotImplement((Type)null!));
    }

    [TestMethod]
    public void BeDerivedFrom_PassAndFail() {
        typeof(FinalDerived).Should().BeDerivedFrom<FinalBase>();
        typeof(FinalBase).Should().NotBeDerivedFrom<FinalBase>();
        typeof(Sample).Should().NotBeDerivedFrom<FinalBase>();

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => typeof(Sample).Should().BeDerivedFrom<FinalBase>());
        Assert.AreEqual("Expected typeof(Sample) to derive from Jeninnet.Assertions.Tests.TypeTests+FinalBase, but found Jeninnet.Assertions.Tests.TypeTests+Sample.", ex.Message);

        var notEx = Assert.ThrowsExactly<AssertionFailedException>(() => typeof(FinalDerived).Should().NotBeDerivedFrom<FinalBase>());
        Assert.AreEqual("Expected typeof(FinalDerived) not to derive from Jeninnet.Assertions.Tests.TypeTests+FinalBase, but it does.", notEx.Message);

#pragma warning disable CA2263 // Intentional: exercises the non-generic overload; generics covered above.
        typeof(FinalDerived).Should().BeDerivedFrom(typeof(FinalBase));
#pragma warning restore CA2263
        Assert.ThrowsExactly<ArgumentNullException>(() => typeof(Sample).Should().BeDerivedFrom((Type)null!));
    }

    [TestMethod]
    public void BeStatic_PassAndFail() {
        typeof(StaticVault).Should().BeStatic();
        Assert.AreEqual(42, StaticVault.Answer());
        typeof(Vault).Should().NotBeStatic();

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => typeof(Vault).Should().BeStatic());
        Assert.AreEqual("Expected typeof(Vault) to be static, but it was not.", ex.Message);

        var notEx = Assert.ThrowsExactly<AssertionFailedException>(() => typeof(StaticVault).Should().NotBeStatic());
        Assert.AreEqual("Expected typeof(StaticVault) not to be static, but it was.", notEx.Message);

        Type? missing = null;
        var nullEx = Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().BeStatic());
        Assert.AreEqual("Expected missing to be static, but found null.", nullEx.Message);
        missing.Should().NotBeStatic();
    }

    [TestMethod]
    public void DefaultConstructor_PassAndFail() {
        typeof(Sample).Should().HaveDefaultConstructor();
        typeof(NoDefault).Should().NotHaveDefaultConstructor();
        typeof(Sample).Should().NotHaveConstructor(typeof(int));
        Assert.AreEqual(0, new NoDefault(0).Value);

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => typeof(NoDefault).Should().HaveDefaultConstructor());
        Assert.AreEqual("Expected typeof(NoDefault) to have a default constructor, but found Jeninnet.Assertions.Tests.TypeTests+NoDefault.", ex.Message);

        var notEx = Assert.ThrowsExactly<AssertionFailedException>(() => typeof(Sample).Should().NotHaveDefaultConstructor());
        Assert.AreEqual("Expected typeof(Sample) not to have a default constructor, but it does.", notEx.Message);

        var ctor = Assert.ThrowsExactly<AssertionFailedException>(() => typeof(Sample).Should().NotHaveConstructor());
        Assert.AreEqual("Expected typeof(Sample) not to have constructor (ctor()), but it was found.", ctor.Message);

        Assert.ThrowsExactly<ArgumentNullException>(() => typeof(Sample).Should().NotHaveConstructor(null!));
    }

    [TestMethod]
    public void HaveIndexer_PassAndFail() {
        typeof(Vault).Should().HaveIndexer(typeof(int), [typeof(int)]);
        typeof(Sample).Should().NotHaveIndexer([typeof(int)]);

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => typeof(Vault).Should().HaveIndexer(typeof(string), [typeof(int)]));
        Assert.AreEqual("Expected typeof(Vault) to have indexer of type String with parameters (Int32), but it was not found.", ex.Message);

        var notEx = Assert.ThrowsExactly<AssertionFailedException>(() => typeof(Vault).Should().NotHaveIndexer([typeof(int)]));
        Assert.AreEqual("Expected typeof(Vault) not to have indexer with parameters (Int32), but it was found.", notEx.Message);

        Assert.ThrowsExactly<ArgumentNullException>(() => typeof(Vault).Should().HaveIndexer(null!, [typeof(int)]));
        var vault = new Vault {
            Counter = 3,
            Secret = "s",
        };
        Assert.AreEqual(5, vault[5]);
        Assert.AreEqual(3, vault.Counter);
        Assert.AreEqual(7, vault.Guarded);
        Assert.AreEqual(3, (int)vault);
    }

    [TestMethod]
    public void ConversionOperators_PassAndFail() {
        typeof(Vault).Should().HaveImplicitConversionOperator(typeof(int));
        typeof(Vault).Should().HaveExplicitConversionOperator(typeof(string));
        typeof(Vault).Should().NotHaveImplicitConversionOperator(typeof(string));
        typeof(Vault).Should().NotHaveExplicitConversionOperator(typeof(int));
        Assert.AreEqual("vault", (string)new Vault());

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => typeof(Vault).Should().HaveImplicitConversionOperator(typeof(string)));
        Assert.AreEqual("Expected typeof(Vault) to have an implicit conversion operator for System.String, but it was not found.", ex.Message);

        var notEx = Assert.ThrowsExactly<AssertionFailedException>(() => typeof(Vault).Should().NotHaveExplicitConversionOperator(typeof(string)));
        Assert.AreEqual("Expected typeof(Vault) not to have an explicit conversion operator for System.String, but it was found.", notEx.Message);

        Assert.ThrowsExactly<ArgumentNullException>(() => typeof(Vault).Should().HaveImplicitConversionOperator(null!));
    }

    [TestMethod]
    public void ExplicitMembers_PassAndFail() {
        typeof(Vault).Should().HaveExplicitMethod("IWorker.Work").And.HaveExplicitMethod("Work");
        typeof(Vault).Should().HaveExplicitProperty("IWorker.Load").And.HaveExplicitProperty("Load");
        typeof(Sample).Should().NotHaveExplicitMethod("Add").And.NotHaveExplicitProperty("Name");

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => typeof(Sample).Should().HaveExplicitMethod("Missing"));
        Assert.AreEqual("Expected typeof(Sample) to have explicit method 'Missing', but it was not found.", ex.Message);

        var notEx = Assert.ThrowsExactly<AssertionFailedException>(() => typeof(Vault).Should().NotHaveExplicitProperty("Load"));
        Assert.AreEqual("Expected typeof(Vault) not to have explicit property 'Load', but it was found.", notEx.Message);

        Assert.ThrowsExactly<ArgumentNullException>(() => typeof(Vault).Should().HaveExplicitMethod(null!));

        IWorker worker = new Vault();
        worker.Work();
        Assert.AreEqual(1, worker.Load);
    }

    [TestMethod]
    public void BeDecoratedWithPredicate_PassAndFail() {
        typeof(Vault).Should().BeDecoratedWith<TierAttribute>(static attribute => attribute.Level == "Gold");

        var mismatch = Assert.ThrowsExactly<AssertionFailedException>(() => typeof(Vault).Should().BeDecoratedWith<TierAttribute>(static attribute => attribute.Level == "Silver"));
        Assert.AreEqual("Expected typeof(Vault) to be decorated with TierAttribute, but it did not match the predicate.", mismatch.Message);

        var absent = Assert.ThrowsExactly<AssertionFailedException>(() => typeof(Sample).Should().BeDecoratedWith<TierAttribute>(static attribute => attribute.Level == "Gold"));
        Assert.AreEqual("Expected typeof(Sample) to be decorated with TierAttribute, but found Jeninnet.Assertions.Tests.TypeTests+Sample.", absent.Message);

        Assert.ThrowsExactly<ArgumentNullException>(() => typeof(Vault).Should().BeDecoratedWith<TierAttribute>((Expression<Func<TierAttribute, bool>>)null!));
    }

    [TestMethod]
    public void NotBeDecoratedWithOrInherit_PassAndFail() {
        typeof(Sample).Should().NotBeDecoratedWithOrInherit<FinalAttribute>();

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => typeof(FinalDerived).Should().NotBeDecoratedWithOrInherit<FinalAttribute>());
        Assert.AreEqual("Expected typeof(FinalDerived) not to be decorated with FinalAttribute or inherit it, but it was found.", ex.Message);
    }

    [TestMethod]
    public void NotHaveAccessModifier_PassAndFail() {
        typeof(Vault).Should().NotHaveAccessModifier(AccessModifier.Public);

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => typeof(Vault).Should().NotHaveAccessModifier(AccessModifier.Private));
        Assert.AreEqual("Expected typeof(Vault) not to have Private accessibility, but found Private.", ex.Message);

        Type? missing = null;
        var nullEx = Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().NotHaveAccessModifier(AccessModifier.Public));
        Assert.AreEqual("Expected missing not to have Public accessibility, but found <null>.", nullEx.Message);
    }

    [TestMethod]
    public void BeType_PassAndFail() {
        typeof(Sample).Should().Be(typeof(Sample));
        typeof(Sample).Should().NotBe(typeof(int));

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => typeof(Sample).Should().Be(typeof(int)));
        Assert.AreEqual("Expected typeof(Sample) to be System.Int32, but found Jeninnet.Assertions.Tests.TypeTests+Sample.", ex.Message);

        var notEx = Assert.ThrowsExactly<AssertionFailedException>(() => typeof(Sample).Should().NotBe(typeof(Sample)));
        Assert.AreEqual("Expected typeof(Sample) not to be Jeninnet.Assertions.Tests.TypeTests+Sample, but found Jeninnet.Assertions.Tests.TypeTests+Sample.", notEx.Message);

        Assert.ThrowsExactly<ArgumentNullException>(() => typeof(Sample).Should().Be(null!));
        Assert.ThrowsExactly<ArgumentNullException>(() => typeof(Sample).Should().NotBe(null!));
    }

    [TestMethod]
    public void MethodInfo_Return_Virtual_Async() {
        var add = typeof(Sample).Should().HaveMethod("Add").Which;
        add.Should().Return<int>().And.NotReturn<string>().And.NotBeVirtual().And.NotBeAsync();

        var greet = typeof(Sample).Should().HaveMethod("Greet").Which;
        greet.Should().Return<string>();

        var fetch = typeof(Vault).Should().HaveMethod("FetchAsync").Which;
        fetch.Should().BeAsync().And.Return<Task<string>>();

        var label = typeof(Vault).Should().HaveMethod("Label").Which;
        label.Should().BeVirtual();

        var notVirtualLabel = Assert.ThrowsExactly<AssertionFailedException>(() => label.Should().NotBeVirtual());
        Assert.AreEqual("Expected label not to be virtual, but it was.", notVirtualLabel.Message);

        var work = typeof(Vault).GetMethod("Jeninnet.Assertions.Tests.SelectorFixtures.IWorker.Work", BindingFlags.NonPublic | BindingFlags.Instance)!;
        work.Should().ReturnVoid();

        var vault = new Vault();
        Assert.AreEqual("vault", vault.Label());
        Assert.AreEqual("vault", vault.FetchAsync().GetAwaiter().GetResult());

        var voidReturn = Assert.ThrowsExactly<AssertionFailedException>(() => add.Should().ReturnVoid());
        Assert.AreEqual("Expected add to return void, but found Int32.", voidReturn.Message);

        var notVoid = Assert.ThrowsExactly<AssertionFailedException>(() => add.Should().NotReturn<int>());
        Assert.AreEqual("Expected add not to have return type Int32, but found Int32.", notVoid.Message);

        var notAsync = Assert.ThrowsExactly<AssertionFailedException>(() => fetch.Should().NotBeAsync());
        Assert.AreEqual("Expected fetch not to be async, but it was.", notAsync.Message);

        var syncAsync = Assert.ThrowsExactly<AssertionFailedException>(() => add.Should().BeAsync());
        Assert.AreEqual("Expected add to be async, but it was not.", syncAsync.Message);

        add.Should().NotBeStatic();
        var answer = typeof(StaticVault).Should().HaveMethod("Answer").Which;
        var staticMethod = Assert.ThrowsExactly<AssertionFailedException>(() => answer.Should().NotBeStatic());
        Assert.AreEqual("Expected answer not to be static, but it was.", staticMethod.Message);

        Assert.ThrowsExactly<ArgumentNullException>(() => add.Should().Return((Type)null!));
    }

    [TestMethod]
    public void PropertyInfo_Return_Virtual_Access() {
        var name = typeof(Sample).Should().HaveProperty("Name").Which;
        name.Should().Return<string>().And.NotReturn<int>().And.NotBeVirtual().And.BeReadable(AccessModifier.Public);

        var counter = typeof(Vault).Should().HaveProperty("Counter").Which;
        counter.Should().Return<int>();

        var guarded = typeof(Vault).Should().HaveProperty("Guarded").Which;
        guarded.Should().BeWritable(AccessModifier.Private);

        var notReadable = Assert.ThrowsExactly<AssertionFailedException>(() => counter.Should().NotBeReadable());
        Assert.AreEqual("Expected counter not to be readable, but it was.", notReadable.Message);

        typeof(Vault).Should().HaveProperty("Secret").Which.Should().NotBeReadable();

        var virtualProp = Assert.ThrowsExactly<AssertionFailedException>(() => counter.Should().BeVirtual());
        Assert.AreEqual("Expected counter to be virtual, but it was not.", virtualProp.Message);

        var level = typeof(Vault).Should().HaveProperty("Level").Which;
        level.Should().BeVirtual();
        Assert.AreEqual(0, new Vault().Level);

        var notVirtualLevel = Assert.ThrowsExactly<AssertionFailedException>(() => level.Should().NotBeVirtual());
        Assert.AreEqual("Expected level not to be virtual, but it was.", notVirtualLevel.Message);

        var wrongAccess = Assert.ThrowsExactly<AssertionFailedException>(() => counter.Should().BeReadable(AccessModifier.Private));
        Assert.AreEqual("Expected counter to be readable with Private access, but found Public.", wrongAccess.Message);

        var wrongReturn = Assert.ThrowsExactly<AssertionFailedException>(() => counter.Should().Return<string>());
        Assert.AreEqual("Expected counter to have return type String, but found Int32.", wrongReturn.Message);

        Assert.ThrowsExactly<ArgumentNullException>(() => counter.Should().Return((Type)null!));
        Assert.ThrowsExactly<ArgumentNullException>(() => counter.Should().NotReturn((Type)null!));
    }

    [TestMethod]
    public void Assembly_Unsigned_SignedWithPublicKey() {
        var probe = new AssemblyName("UnsignedProbe");
        var unsigned = AssemblyBuilder.DefineDynamicAssembly(probe, AssemblyBuilderAccess.Run);
        unsigned.Should().BeUnsigned();

        var signed = typeof(object).Assembly;
        var expectedHex = Convert.ToHexString(signed.GetName().GetPublicKey()!);
        signed.Should().BeSignedWithPublicKey(expectedHex);
        signed.Should().BeSignedWithPublicKey(expectedHex.ToLowerInvariant());

        var unsignedKey = Assert.ThrowsExactly<AssertionFailedException>(() => unsigned.Should().BeSignedWithPublicKey("00"));
        Assert.AreEqual("Expected unsigned to be signed with public key '00', but found <unsigned>.", unsignedKey.Message);

        var wrongKey = Assert.ThrowsExactly<AssertionFailedException>(() => signed.Should().BeSignedWithPublicKey("00"));
        Assert.Contains("Expected signed to be signed with public key '00', but found ", wrongKey.Message);

        var signedAsUnsigned = Assert.ThrowsExactly<AssertionFailedException>(() => signed.Should().BeUnsigned());
        Assert.AreEqual("Expected signed to be unsigned, but it was signed.", signedAsUnsigned.Message);

        Assert.ThrowsExactly<ArgumentNullException>(() => signed.Should().BeSignedWithPublicKey(null!));
        Assert.ThrowsExactly<ArgumentException>(() => signed.Should().BeSignedWithPublicKey("ZZ"));
        Assert.ThrowsExactly<ArgumentException>(() => signed.Should().BeSignedWithPublicKey("0"));

        Assembly? missing = null;
        var nullUnsigned = Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().BeUnsigned());
        Assert.AreEqual("Expected missing to be unsigned, but found null.", nullUnsigned.Message);
    }
}

//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Tests;

public partial class LocalizationTests {
    [TestMethod]
    public void Arabic_TypeBasics() {
        using var _ = new ArabicScope();

        var beNull = Assert.ThrowsExactly<AssertionFailedException>(() => typeof(SampleWidget).Should().BeNull());
        Assert.AreEqual("المتوقع من typeof(SampleWidget) أن يكون null، لكن وُجد Jeninnet.Assertions.Tests.LocalizationTests+SampleWidget.", beNull.Message);

        Type? missingType = null;
        var notBeNull = Assert.ThrowsExactly<AssertionFailedException>(() => missingType.Should().NotBeNull());
        Assert.AreEqual("المتوقع من missingType ألا يكون null، لكن وُجد null.", notBeNull.Message);

        var signature = Assert.ThrowsExactly<AssertionFailedException>(() => typeof(SampleWidget).Should().HaveMethod("Add", [typeof(string)]));
        Assert.AreEqual("المتوقع من typeof(SampleWidget) أن يحتوي الدالة 'Add(String)'، لكنه لم يُعثر عليها.", signature.Message);

        var notMethod = Assert.ThrowsExactly<AssertionFailedException>(() => typeof(SampleWidget).Should().NotHaveMethod("Add"));
        Assert.AreEqual("المتوقع من typeof(SampleWidget) ألا يحتوي الدالة 'Add'، لكنها وُجدت.", notMethod.Message);

        var property = Assert.ThrowsExactly<AssertionFailedException>(() => typeof(SampleWidget).Should().HaveProperty("Missing"));
        Assert.AreEqual("المتوقع من typeof(SampleWidget) أن يحتوي الخاصية 'Missing'، لكنه لم يُعثر عليها.", property.Message);

        var notProperty = Assert.ThrowsExactly<AssertionFailedException>(() => typeof(SampleWidget).Should().NotHaveProperty("Name"));
        Assert.AreEqual("المتوقع من typeof(SampleWidget) ألا يحتوي الخاصية 'Name'، لكنها وُجدت.", notProperty.Message);

        var sealedType = Assert.ThrowsExactly<AssertionFailedException>(() => typeof(BaseWidget).Should().BeSealed());
        Assert.AreEqual("المتوقع من typeof(BaseWidget) أن يكون مختوماً، لكن it was not.", sealedType.Message);

        var notSealed = Assert.ThrowsExactly<AssertionFailedException>(() => typeof(string).Should().NotBeSealed());
        Assert.AreEqual("المتوقع من typeof(string) ألا يكون مختوماً، لكنه كان.", notSealed.Message);

        var abstractType = Assert.ThrowsExactly<AssertionFailedException>(() => typeof(SampleWidget).Should().BeAbstract());
        Assert.AreEqual("المتوقع من typeof(SampleWidget) أن يكون مجرداً، لكن it was not.", abstractType.Message);

        var notAbstract = Assert.ThrowsExactly<AssertionFailedException>(() => typeof(System.IO.Stream).Should().NotBeAbstract());
        Assert.AreEqual("المتوقع من typeof(System.IO.Stream) ألا يكون مجرداً، لكنه كان.", notAbstract.Message);
    }

    [TestMethod]
    public void Arabic_TypeAttributes() {
        using var _ = new ArabicScope();

        var assignable = Assert.ThrowsExactly<AssertionFailedException>(() => typeof(SampleWidget).Should().BeAssignableTo<int>());
        Assert.AreEqual("المتوقع من typeof(SampleWidget) أن يكون قابلاً للإسناد إلى Int32، لكن found Jeninnet.Assertions.Tests.LocalizationTests+SampleWidget.", assignable.Message);

        var notAssignable = Assert.ThrowsExactly<AssertionFailedException>(() => typeof(SampleWidget).Should().NotBeAssignableTo<object>());
        Assert.AreEqual("المتوقع من typeof(SampleWidget) ألا يكون قابلاً للإسناد إلى Object، لكنه كان.", notAssignable.Message);

        var decorated = Assert.ThrowsExactly<AssertionFailedException>(() => typeof(SampleWidget).Should().BeDecoratedWith<GadgetAttribute>());
        Assert.AreEqual("المتوقع من typeof(SampleWidget) أن يكون موسوماً بـ GadgetAttribute، لكن found Jeninnet.Assertions.Tests.LocalizationTests+SampleWidget.", decorated.Message);

        var notDecorated = Assert.ThrowsExactly<AssertionFailedException>(() => typeof(MarkedWidget).Should().NotBeDecoratedWith<GadgetAttribute>());
        Assert.AreEqual("المتوقع من typeof(MarkedWidget) ألا يكون موسوماً بـ GadgetAttribute، لكنه كان.", notDecorated.Message);
    }

    [TestMethod]
    public void Arabic_MethodInfo() {
        using var _ = new ArabicScope();

        var method = typeof(SampleWidget).Should().HaveMethod("Add").Which;
        var beNull = Assert.ThrowsExactly<AssertionFailedException>(() => method.Should().BeNull());
        Assert.AreEqual("المتوقع من method أن يكون null، لكن وُجد Add.", beNull.Message);

        var returns = Assert.ThrowsExactly<AssertionFailedException>(() => method.Should().Return<string>());
        Assert.AreEqual("المتوقع من method أن يكون نوع إرجاعه String، لكن وُجد Int32.", returns.Message);

        var parameter = Assert.ThrowsExactly<AssertionFailedException>(() => method.Should().HaveParameter("z", typeof(int)));
        Assert.AreEqual("المتوقع من method أن يحتوي المعامل 'z' من النوع Int32، لكنه لم يُعثر عليه.", parameter.Message);

        var nonVirtual = Assert.ThrowsExactly<AssertionFailedException>(() => method.Should().BeVirtual());
        Assert.AreEqual("المتوقع من method أن يكون افتراضياً، لكن it was not.", nonVirtual.Message);

        var staticMethod = Assert.ThrowsExactly<AssertionFailedException>(() => method.Should().BeStatic());
        Assert.AreEqual("المتوقع من method أن يكون ساكناً، لكن it was not.", staticMethod.Message);

        var decorated = Assert.ThrowsExactly<AssertionFailedException>(() => method.Should().BeDecoratedWith<GadgetAttribute>());
        Assert.AreEqual("المتوقع من method أن يكون موسوماً بـ GadgetAttribute، لكن it was not.", decorated.Message);

        MethodInfo? missingMethod = null;
        var notBeNull = Assert.ThrowsExactly<AssertionFailedException>(() => missingMethod.Should().NotBeNull());
        Assert.AreEqual("المتوقع من missingMethod ألا يكون null، لكن وُجد null.", notBeNull.Message);
    }

    [TestMethod]
    public void Arabic_PropertyInfo() {
        using var _ = new ArabicScope();

        var property = typeof(SampleWidget).Should().HaveProperty("Name").Which;
        var beNull = Assert.ThrowsExactly<AssertionFailedException>(() => property.Should().BeNull());
        Assert.AreEqual("المتوقع من property أن يكون null، لكن وُجد Name.", beNull.Message);

        var ofType = Assert.ThrowsExactly<AssertionFailedException>(() => property.Should().Return<int>());
        Assert.AreEqual("المتوقع من property أن يكون نوع إرجاعه Int32، لكن وُجد String.", ofType.Message);

        var writeOnly = typeof(SampleWidget).Should().HaveProperty("WriteOnly").Which;
        var readable = Assert.ThrowsExactly<AssertionFailedException>(() => writeOnly.Should().BeReadable());
        Assert.AreEqual("المتوقع من writeOnly أن يكون قابلاً للقراءة، لكن it was not.", readable.Message);

        var readOnly = typeof(SampleWidget).Should().HaveProperty("ReadOnly").Which;
        var writable = Assert.ThrowsExactly<AssertionFailedException>(() => readOnly.Should().BeWritable());
        Assert.AreEqual("المتوقع من readOnly أن يكون قابلاً للكتابة، لكن it was not.", writable.Message);

        var decorated = Assert.ThrowsExactly<AssertionFailedException>(() => property.Should().BeDecoratedWith<GadgetAttribute>());
        Assert.AreEqual("المتوقع من property أن يكون موسوماً بـ GadgetAttribute، لكن it was not.", decorated.Message);

        PropertyInfo? missingProperty = null;
        var notBeNull = Assert.ThrowsExactly<AssertionFailedException>(() => missingProperty.Should().NotBeNull());
        Assert.AreEqual("المتوقع من missingProperty ألا يكون null، لكن وُجد null.", notBeNull.Message);
    }

    [TestMethod]
    public void Arabic_ConstructorInfo() {
        using var _ = new ArabicScope();

        var constructor = typeof(SampleWidget).Should().HaveConstructor().Which;
        var beNull = Assert.ThrowsExactly<AssertionFailedException>(() => constructor.Should().BeNull());
        Assert.AreEqual("المتوقع من constructor أن يكون null، لكنه لم يكن null.", beNull.Message);

        var parameter = Assert.ThrowsExactly<AssertionFailedException>(() => constructor.Should().HaveParameter("zzz", typeof(int)));
        Assert.AreEqual("المتوقع من constructor أن يحتوي المعامل 'zzz' من النوع Int32، لكنه لم يُعثر عليه.", parameter.Message);

        var decorated = Assert.ThrowsExactly<AssertionFailedException>(() => constructor.Should().BeDecoratedWith<GadgetAttribute>());
        Assert.AreEqual("المتوقع من constructor أن يكون موسوماً بـ GadgetAttribute، لكن it was not.", decorated.Message);

        var missing = Assert.ThrowsExactly<AssertionFailedException>(() => typeof(SampleWidget).Should().HaveConstructor(typeof(int)));
        Assert.AreEqual("المتوقع من typeof(SampleWidget) أن يحتوي المنشئ '(Int32)'، لكنه لم يُعثر عليه.", missing.Message);

        var hidden = typeof(SampleWidget).GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, binder: null, [typeof(int)], modifiers: null)!;
        var bePublic = Assert.ThrowsExactly<AssertionFailedException>(() => hidden.Should().BePublic());
        Assert.AreEqual("المتوقع من hidden أن يكون عاماً، لكن it was not.", bePublic.Message);

        ConstructorInfo? missingConstructor = null;
        var notBeNull = Assert.ThrowsExactly<AssertionFailedException>(() => missingConstructor.Should().NotBeNull());
        Assert.AreEqual("المتوقع من missingConstructor ألا يكون null، لكن وُجد null.", notBeNull.Message);
    }

    [TestMethod]
    public void Arabic_Assembly() {
        using var _ = new ArabicScope();

        var assembly = typeof(SampleWidget).Assembly;
        var define = Assert.ThrowsExactly<AssertionFailedException>(() => assembly.Should().DefineType("No.Such.Type"));
        Assert.AreEqual("المتوقع من assembly أن يعرّف النوع 'No.Such.Type'، لكنه لم يُعثر عليه.", define.Message);

        var notDefine = Assert.ThrowsExactly<AssertionFailedException>(() => assembly.Should().NotDefineType("Jeninnet.Assertions.Tests.LocalizationTests+SampleWidget"));
        Assert.AreEqual("المتوقع من assembly ألا يعرّف النوع 'Jeninnet.Assertions.Tests.LocalizationTests+SampleWidget'، لكنه وُجد.", notDefine.Message);

        var reference = Assert.ThrowsExactly<AssertionFailedException>(() => typeof(string).Assembly.Should().Reference(assembly));
        Assert.AreEqual("المتوقع من typeof(string).Assembly أن يشير إلى التجميع 'Jeninnet.Assertions.Tests'، لكنه لم يُعثر عليه.", reference.Message);

        Assembly? missingAssembly = null;
        var referenceNull = Assert.ThrowsExactly<AssertionFailedException>(() => missingAssembly.Should().Reference(assembly));
        Assert.AreEqual("المتوقع من missingAssembly أن يشير إلى التجميع 'Jeninnet.Assertions.Tests'، لكن وُجد null.", referenceNull.Message);

        var beNull = Assert.ThrowsExactly<AssertionFailedException>(() => assembly.Should().BeNull());
        Assert.Contains("المتوقع من assembly أن يكون null، لكن وُجد Jeninnet.Assertions.Tests,", beNull.Message);

        var notBeNull = Assert.ThrowsExactly<AssertionFailedException>(() => missingAssembly.Should().NotBeNull());
        Assert.AreEqual("المتوقع من missingAssembly ألا يكون null، لكن وُجد null.", notBeNull.Message);
    }

    [TestMethod]
    public void Arabic_TypeSelector() {
        using var _ = new ArabicScope();

        var count = Assert.ThrowsExactly<AssertionFailedException>(
            () => AllTypes.From(typeof(SampleWidget).Assembly).ThatAreInNamespace("No.Such.Namespace").Should().HaveCount(2));
        Assert.AreEqual("المتوقع من الأنواع المحددة أن تحتوي 2 من العناصر، لكن وُجد 0.", count.Message);

        var lonely = AllTypes.From(typeof(LonelyWidget).Assembly).ThatAreInNamespace("Jeninnet.Assertions.Tests.LocalizationFixtures");
        var method = Assert.ThrowsExactly<AssertionFailedException>(() => lonely.Should().HaveMethod("Missing"));
        Assert.AreEqual("المتوقع من Jeninnet.Assertions.Tests.LocalizationFixtures.LonelyWidget أن يحتوي الدالة 'Missing'، لكنه لم يُعثر عليها.", method.Message);

        var property = Assert.ThrowsExactly<AssertionFailedException>(() => lonely.Should().HaveProperty("Missing"));
        Assert.AreEqual("المتوقع من Jeninnet.Assertions.Tests.LocalizationFixtures.LonelyWidget أن يحتوي الخاصية 'Missing'، لكنه لم يُعثر عليها.", property.Message);

        var sealedType = Assert.ThrowsExactly<AssertionFailedException>(() => lonely.Should().BeSealed());
        Assert.AreEqual("المتوقع من Jeninnet.Assertions.Tests.LocalizationFixtures.LonelyWidget أن يكون مختوماً، لكنه لم يكن.", sealedType.Message);

        var abstractType = Assert.ThrowsExactly<AssertionFailedException>(() => lonely.Should().BeAbstract());
        Assert.AreEqual("المتوقع من Jeninnet.Assertions.Tests.LocalizationFixtures.LonelyWidget أن يكون مجرداً، لكنه لم يكن.", abstractType.Message);
    }

    [TestMethod]
    public void Arabic_SelectorCompatVerbs() {
        using var _ = new ArabicScope();

        var notAccess = Assert.ThrowsExactly<AssertionFailedException>(
            () => typeof(OrderService).Methods().ThatSatisfy(static m => m.Name == "PlaceOrder").Should().NotBe(AccessModifier.Public));
        Assert.AreEqual("المتوقع من العضو 'PlaceOrder' ألا يكون معدّل الوصول Public، لكنه كذلك.", notAccess.Message);

        var notAsync = Assert.ThrowsExactly<AssertionFailedException>(
            () => typeof(OrderService).Methods().ThatSatisfy(static m => m.Name == "SaveAsync").Should().NotBeAsync());
        Assert.AreEqual("المتوقع من الدالة 'SaveAsync' ألا تكون async، لكنها كانت كذلك.", notAsync.Message);

        var notVirtual = Assert.ThrowsExactly<AssertionFailedException>(
            () => typeof(OrderService).Methods().ThatSatisfy(static m => m.Name == "Validate").Should().NotBeVirtual());
        Assert.AreEqual("المتوقع من العضو 'Validate' ألا يكون virtual، لكنه كان كذلك.", notVirtual.Message);

        var decoratedPredicate = Assert.ThrowsExactly<AssertionFailedException>(
            () => typeof(OrderService).Methods().ThatAreDecoratedWith<MarkerAttribute>().Should().BeDecoratedWith<MarkerAttribute>(static _ => false));
        Assert.AreEqual("المتوقع من العضو 'Marked' أن يكون مزيّناً بـ MarkerAttribute مطابقاً للشرط، لكنه لم يكن كذلك.", decoratedPredicate.Message);

        var notDecoratedPredicate = Assert.ThrowsExactly<AssertionFailedException>(
            () => typeof(OrderService).Methods().ThatAreDecoratedWith<MarkerAttribute>().Should().NotBeDecoratedWith<MarkerAttribute>(static _ => true));
        Assert.AreEqual("المتوقع من العضو 'Marked' ألا يكون مزيّناً بـ MarkerAttribute مطابقاً للشرط، لكنه كان كذلك.", notDecoratedPredicate.Message);

        var notWritable = Assert.ThrowsExactly<AssertionFailedException>(
            () => typeof(OrderService).Properties().ThatSatisfy(static p => p.Name == "Name").Should().NotBeWritable());
        Assert.AreEqual("المتوقع من الخاصية 'Name' ألا تكون قابلة للكتابة، لكنها كانت كذلك.", notWritable.Message);
    }
}

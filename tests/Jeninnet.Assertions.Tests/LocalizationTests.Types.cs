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

        var beNull = Throws<AssertionFailedException>(() => typeof(SampleWidget).Should().BeNull());
        beNull.Message.Should().Be("المتوقع من typeof(SampleWidget) أن يكون null، لكن وُجد Jeninnet.Assertions.Tests.LocalizationTests+SampleWidget.");

        Type? missingType = null;
        var notBeNull = Throws<AssertionFailedException>(() => missingType.Should().NotBeNull());
        notBeNull.Message.Should().Be("المتوقع من missingType ألا يكون null، لكن وُجد null.");

        var signature = Throws<AssertionFailedException>(() => typeof(SampleWidget).Should().HaveMethod("Add", [typeof(string)]));
        signature.Message.Should().Be("المتوقع من typeof(SampleWidget) أن يحتوي الدالة 'Add(String)'، لكنه لم يُعثر عليها.");

        var notMethod = Throws<AssertionFailedException>(() => typeof(SampleWidget).Should().NotHaveMethod("Add"));
        notMethod.Message.Should().Be("المتوقع من typeof(SampleWidget) ألا يحتوي الدالة 'Add'، لكنها وُجدت.");

        var property = Throws<AssertionFailedException>(() => typeof(SampleWidget).Should().HaveProperty("Missing"));
        property.Message.Should().Be("المتوقع من typeof(SampleWidget) أن يحتوي الخاصية 'Missing'، لكنه لم يُعثر عليها.");

        var notProperty = Throws<AssertionFailedException>(() => typeof(SampleWidget).Should().NotHaveProperty("Name"));
        notProperty.Message.Should().Be("المتوقع من typeof(SampleWidget) ألا يحتوي الخاصية 'Name'، لكنها وُجدت.");

        var sealedType = Throws<AssertionFailedException>(() => typeof(BaseWidget).Should().BeSealed());
        sealedType.Message.Should().Be("المتوقع من typeof(BaseWidget) أن يكون مختوماً، لكن it was not.");

        var notSealed = Throws<AssertionFailedException>(() => typeof(string).Should().NotBeSealed());
        notSealed.Message.Should().Be("المتوقع من typeof(string) ألا يكون مختوماً، لكنه كان.");

        var abstractType = Throws<AssertionFailedException>(() => typeof(SampleWidget).Should().BeAbstract());
        abstractType.Message.Should().Be("المتوقع من typeof(SampleWidget) أن يكون مجرداً، لكن it was not.");

        var notAbstract = Throws<AssertionFailedException>(() => typeof(System.IO.Stream).Should().NotBeAbstract());
        notAbstract.Message.Should().Be("المتوقع من typeof(System.IO.Stream) ألا يكون مجرداً، لكنه كان.");
    }

    [TestMethod]
    public void Arabic_TypeAttributes() {
        using var _ = new ArabicScope();

        var assignable = Throws<AssertionFailedException>(() => typeof(SampleWidget).Should().BeAssignableTo<int>());
        assignable.Message.Should().Be("المتوقع من typeof(SampleWidget) أن يكون قابلاً للإسناد إلى Int32، لكن found Jeninnet.Assertions.Tests.LocalizationTests+SampleWidget.");

        var notAssignable = Throws<AssertionFailedException>(() => typeof(SampleWidget).Should().NotBeAssignableTo<object>());
        notAssignable.Message.Should().Be("المتوقع من typeof(SampleWidget) ألا يكون قابلاً للإسناد إلى Object، لكنه كان.");

        var decorated = Throws<AssertionFailedException>(() => typeof(SampleWidget).Should().BeDecoratedWith<GadgetAttribute>());
        decorated.Message.Should().Be("المتوقع من typeof(SampleWidget) أن يكون موسوماً بـ GadgetAttribute، لكن found Jeninnet.Assertions.Tests.LocalizationTests+SampleWidget.");

        var notDecorated = Throws<AssertionFailedException>(() => typeof(MarkedWidget).Should().NotBeDecoratedWith<GadgetAttribute>());
        notDecorated.Message.Should().Be("المتوقع من typeof(MarkedWidget) ألا يكون موسوماً بـ GadgetAttribute، لكنه كان.");
    }

    [TestMethod]
    public void Arabic_MethodInfo() {
        using var _ = new ArabicScope();

        var method = typeof(SampleWidget).Should().HaveMethod("Add").Which;
        var beNull = Throws<AssertionFailedException>(() => method.Should().BeNull());
        beNull.Message.Should().Be("المتوقع من method أن يكون null، لكن وُجد Add.");

        var returns = Throws<AssertionFailedException>(() => method.Should().Return<string>());
        returns.Message.Should().Be("المتوقع من method أن يكون نوع إرجاعه String، لكن وُجد Int32.");

        var parameter = Throws<AssertionFailedException>(() => method.Should().HaveParameter("z", typeof(int)));
        parameter.Message.Should().Be("المتوقع من method أن يحتوي المعامل 'z' من النوع Int32، لكنه لم يُعثر عليه.");

        var nonVirtual = Throws<AssertionFailedException>(() => method.Should().BeVirtual());
        nonVirtual.Message.Should().Be("المتوقع من method أن يكون افتراضياً، لكن it was not.");

        var staticMethod = Throws<AssertionFailedException>(() => method.Should().BeStatic());
        staticMethod.Message.Should().Be("المتوقع من method أن يكون ساكناً، لكن it was not.");

        var decorated = Throws<AssertionFailedException>(() => method.Should().BeDecoratedWith<GadgetAttribute>());
        decorated.Message.Should().Be("المتوقع من method أن يكون موسوماً بـ GadgetAttribute، لكن it was not.");

        MethodInfo? missingMethod = null;
        var notBeNull = Throws<AssertionFailedException>(() => missingMethod.Should().NotBeNull());
        notBeNull.Message.Should().Be("المتوقع من missingMethod ألا يكون null، لكن وُجد null.");
    }

    [TestMethod]
    public void Arabic_PropertyInfo() {
        using var _ = new ArabicScope();

        var property = typeof(SampleWidget).Should().HaveProperty("Name").Which;
        var beNull = Throws<AssertionFailedException>(() => property.Should().BeNull());
        beNull.Message.Should().Be("المتوقع من property أن يكون null، لكن وُجد Name.");

        var ofType = Throws<AssertionFailedException>(() => property.Should().Return<int>());
        ofType.Message.Should().Be("المتوقع من property أن يكون نوع إرجاعه Int32، لكن وُجد String.");

        var writeOnly = typeof(SampleWidget).Should().HaveProperty("WriteOnly").Which;
        var readable = Throws<AssertionFailedException>(() => writeOnly.Should().BeReadable());
        readable.Message.Should().Be("المتوقع من writeOnly أن يكون قابلاً للقراءة، لكن it was not.");

        var readOnly = typeof(SampleWidget).Should().HaveProperty("ReadOnly").Which;
        var writable = Throws<AssertionFailedException>(() => readOnly.Should().BeWritable());
        writable.Message.Should().Be("المتوقع من readOnly أن يكون قابلاً للكتابة، لكن it was not.");

        var decorated = Throws<AssertionFailedException>(() => property.Should().BeDecoratedWith<GadgetAttribute>());
        decorated.Message.Should().Be("المتوقع من property أن يكون موسوماً بـ GadgetAttribute، لكن it was not.");

        PropertyInfo? missingProperty = null;
        var notBeNull = Throws<AssertionFailedException>(() => missingProperty.Should().NotBeNull());
        notBeNull.Message.Should().Be("المتوقع من missingProperty ألا يكون null، لكن وُجد null.");
    }

    [TestMethod]
    public void Arabic_ConstructorInfo() {
        using var _ = new ArabicScope();

        var constructor = typeof(SampleWidget).Should().HaveConstructor().Which;
        var beNull = Throws<AssertionFailedException>(() => constructor.Should().BeNull());
        beNull.Message.Should().Be("المتوقع من constructor أن يكون null، لكنه لم يكن null.");

        var parameter = Throws<AssertionFailedException>(() => constructor.Should().HaveParameter("zzz", typeof(int)));
        parameter.Message.Should().Be("المتوقع من constructor أن يحتوي المعامل 'zzz' من النوع Int32، لكنه لم يُعثر عليه.");

        var decorated = Throws<AssertionFailedException>(() => constructor.Should().BeDecoratedWith<GadgetAttribute>());
        decorated.Message.Should().Be("المتوقع من constructor أن يكون موسوماً بـ GadgetAttribute، لكن it was not.");

        var missing = Throws<AssertionFailedException>(() => typeof(SampleWidget).Should().HaveConstructor(typeof(int)));
        missing.Message.Should().Be("المتوقع من typeof(SampleWidget) أن يحتوي المنشئ '(Int32)'، لكنه لم يُعثر عليه.");

        var hidden = typeof(SampleWidget).GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, binder: null, [typeof(int)], modifiers: null)!;
        var bePublic = Throws<AssertionFailedException>(() => hidden.Should().BePublic());
        bePublic.Message.Should().Be("المتوقع من hidden أن يكون عاماً، لكن it was not.");

        ConstructorInfo? missingConstructor = null;
        var notBeNull = Throws<AssertionFailedException>(() => missingConstructor.Should().NotBeNull());
        notBeNull.Message.Should().Be("المتوقع من missingConstructor ألا يكون null، لكن وُجد null.");
    }

    [TestMethod]
    public void Arabic_Assembly() {
        using var _ = new ArabicScope();

        var assembly = typeof(SampleWidget).Assembly;
        var define = Throws<AssertionFailedException>(() => assembly.Should().DefineType("No.Such.Type"));
        define.Message.Should().Be("المتوقع من assembly أن يعرّف النوع 'No.Such.Type'، لكنه لم يُعثر عليه.");

        var notDefine = Throws<AssertionFailedException>(() => assembly.Should().NotDefineType("Jeninnet.Assertions.Tests.LocalizationTests+SampleWidget"));
        notDefine.Message.Should().Be("المتوقع من assembly ألا يعرّف النوع 'Jeninnet.Assertions.Tests.LocalizationTests+SampleWidget'، لكنه وُجد.");

        var reference = Throws<AssertionFailedException>(() => typeof(string).Assembly.Should().Reference(assembly));
        reference.Message.Should().Be("المتوقع من typeof(string).Assembly أن يشير إلى التجميع 'Jeninnet.Assertions.Tests'، لكنه لم يُعثر عليه.");

        Assembly? missingAssembly = null;
        var referenceNull = Throws<AssertionFailedException>(() => missingAssembly.Should().Reference(assembly));
        referenceNull.Message.Should().Be("المتوقع من missingAssembly أن يشير إلى التجميع 'Jeninnet.Assertions.Tests'، لكن وُجد null.");

        var beNull = Throws<AssertionFailedException>(() => assembly.Should().BeNull());
        beNull.Message.Should().Contain("المتوقع من assembly أن يكون null، لكن وُجد Jeninnet.Assertions.Tests,");

        var notBeNull = Throws<AssertionFailedException>(() => missingAssembly.Should().NotBeNull());
        notBeNull.Message.Should().Be("المتوقع من missingAssembly ألا يكون null، لكن وُجد null.");
    }

    [TestMethod]
    public void Arabic_ReflectionMetadataMessages() {
        using var _ = new ArabicScope();

        var kind = Throws<AssertionFailedException>(() => typeof(IWorker).Should().BeClass());
        kind.Message.Should().Be("المتوقع من typeof(IWorker) أن يكون من النوع class، لكن وُجد interface.");

        var name = Throws<AssertionFailedException>(() => typeof(SampleWidget).Should().HaveName("Other"));
        name.Message.Should().Be("المتوقع من typeof(SampleWidget) أن يكون اسمه 'Other'، لكن وُجد 'SampleWidget'.");

        var sameAs = Throws<AssertionFailedException>(() => typeof(SampleWidget).Should().NotBeSameAs(typeof(SampleWidget)));
        sameAs.Message.Should().Contain("المتوقع من typeof(SampleWidget) ألا يكون نفس النسخة مثل");

        var method = typeof(SampleWidget).Should().HaveMethod("Add").Which;
        var abstractMethod = Throws<AssertionFailedException>(() => method.Should().BeAbstract());
        abstractMethod.Message.Should().Be("المتوقع من method أن يكون مجرداً، لكنه لم يكن.");

        var property = typeof(SampleWidget).Should().HaveProperty("Name").Which;
        var required = Throws<AssertionFailedException>(() => property.Should().BeRequired());
        required.Message.Should().Be("المتوقع من property أن يكون مطلوباً (required)، لكنه لم يكن.");

        var constructor = typeof(SampleWidget).Should().HaveConstructor().Which;
        var privateCtor = Throws<AssertionFailedException>(() => constructor.Should().BePrivate());
        privateCtor.Message.Should().Be("المتوقع من constructor أن يكون خاصاً، لكن it was Public.");

        var staticCtor = Throws<AssertionFailedException>(() => constructor.Should().BeStatic());
        staticCtor.Message.Should().Be("المتوقع من constructor أن يكون منشئاً ساكناً، لكن it was not.");

        var decorated = Throws<AssertionFailedException>(
            () => method.Should().BeDecoratedWith<GadgetAttribute>(static _ => true));
        decorated.Message.Should().Be("المتوقع من method أن يكون موسوماً بـ GadgetAttribute مطابقاً للشرط، لكنه لم يكن.");
    }

    [TestMethod]
    public void Arabic_ReflectionMetadataKeysAreTranslated() {
        string[] keys = [
            "TYPE_BE_KIND",
            "TYPE_NOT_BE_KIND",
            "TYPE_BE_SAME_AS",
            "TYPE_NOT_BE_SAME_AS",
            "TYPE_BE_ASSIGNABLE_FROM",
            "TYPE_BE_SUBCLASS_OF",
            "TYPE_BE_BASE_OF",
            "TYPE_HAVE_NAME",
            "TYPE_HAVE_FULL_NAME",
            "TYPE_HAVE_NAMESPACE",
            "TYPE_HAVE_BASE_TYPE",
            "TYPE_HAVE_GENERIC_PARAMETER",
            "TYPE_HAVE_GENERIC_PARAMETER_NAME",
            "TYPE_NOT_BE_DECORATED_PREDICATE",
            "MEMBER_BE_ABSTRACT",
            "MEMBER_NOT_BE_ABSTRACT",
            "MEMBER_BE_OVERRIDE",
            "MEMBER_NOT_BE_OVERRIDE",
            "MEMBER_BE_FINAL",
            "MEMBER_NOT_BE_FINAL",
            "MEMBER_BE_GENERIC",
            "MEMBER_NOT_BE_GENERIC",
            "MEMBER_BE_EXTENSION",
            "MEMBER_NOT_BE_EXTENSION",
            "MEMBER_HAVE_PARAMETERS",
            "MEMBER_BE_SAME_AS",
            "MEMBER_NOT_BE_SAME_AS",
            "MEMBER_BE_DECORATED_MATCH",
            "MEMBER_NOT_BE_DECORATED_MATCH",
            "PROPERTY_BE_STATIC",
            "PROPERTY_NOT_BE_STATIC",
            "PROPERTY_BE_REQUIRED",
            "PROPERTY_NOT_BE_REQUIRED",
            "PROPERTY_BE_INIT_ONLY",
            "PROPERTY_NOT_BE_INIT_ONLY",
            "PROPERTY_HAVE_INDEX_PARAMETERS",
            "CTOR_NOT_BE_PUBLIC",
            "CTOR_BE_PRIVATE",
            "CTOR_BE_PROTECTED",
            "CTOR_BE_INTERNAL",
            "CTOR_BE_STATIC",
        ];

        var previous = AssertionCulture.Current;
        try {
            AssertionCulture.Current = new CultureInfo("en");
            var english = keys.ToDictionary(static key => key, AssertionCulture.GetMessage);

            AssertionCulture.Current = new CultureInfo("ar");
            foreach(var key in keys) {
                AssertionCulture.GetMessage(key).Should().NotBe(english[key], because: key);
            }
        }
        finally {
            AssertionCulture.Current = previous;
        }
    }

    [TestMethod]
    public void Arabic_TypeSelector() {
        using var _ = new ArabicScope();

        var count = Throws<AssertionFailedException>(
            () => AllTypes.From(typeof(SampleWidget).Assembly).ThatAreInNamespace("No.Such.Namespace").Should().HaveCount(2));
        count.Message.Should().Be("المتوقع من الأنواع المحددة أن تحتوي 2 من العناصر، لكن وُجد 0.");

        var lonely = AllTypes.From(typeof(LonelyWidget).Assembly).ThatAreInNamespace("Jeninnet.Assertions.Tests.LocalizationFixtures");
        var method = Throws<AssertionFailedException>(() => lonely.Should().HaveMethod("Missing"));
        method.Message.Should().Be("المتوقع من Jeninnet.Assertions.Tests.LocalizationFixtures.LonelyWidget أن يحتوي الدالة 'Missing'، لكنه لم يُعثر عليها.");

        var property = Throws<AssertionFailedException>(() => lonely.Should().HaveProperty("Missing"));
        property.Message.Should().Be("المتوقع من Jeninnet.Assertions.Tests.LocalizationFixtures.LonelyWidget أن يحتوي الخاصية 'Missing'، لكنه لم يُعثر عليها.");

        var sealedType = Throws<AssertionFailedException>(() => lonely.Should().BeSealed());
        sealedType.Message.Should().Be("المتوقع من Jeninnet.Assertions.Tests.LocalizationFixtures.LonelyWidget أن يكون مختوماً، لكنه لم يكن.");

        var abstractType = Throws<AssertionFailedException>(() => lonely.Should().BeAbstract());
        abstractType.Message.Should().Be("المتوقع من Jeninnet.Assertions.Tests.LocalizationFixtures.LonelyWidget أن يكون مجرداً، لكنه لم يكن.");
    }

    [TestMethod]
    public void Arabic_SelectorCompatVerbs() {
        using var _ = new ArabicScope();

        var notAccess = Throws<AssertionFailedException>(
            () => typeof(OrderService).Methods().ThatSatisfy(static m => m.Name == "PlaceOrder").Should().NotBe(AccessModifier.Public));
        notAccess.Message.Should().Be("المتوقع من العضو 'PlaceOrder' ألا يكون معدّل الوصول Public، لكنه كذلك.");

        var notAsync = Throws<AssertionFailedException>(
            () => typeof(OrderService).Methods().ThatSatisfy(static m => m.Name == "SaveAsync").Should().NotBeAsync());
        notAsync.Message.Should().Be("المتوقع من الدالة 'SaveAsync' ألا تكون async، لكنها كانت كذلك.");

        var notVirtual = Throws<AssertionFailedException>(
            () => typeof(OrderService).Methods().ThatSatisfy(static m => m.Name == "Validate").Should().NotBeVirtual());
        notVirtual.Message.Should().Be("المتوقع من العضو 'Validate' ألا يكون virtual، لكنه كان كذلك.");

        var decoratedPredicate = Throws<AssertionFailedException>(
            () => typeof(OrderService).Methods().ThatAreDecoratedWith<MarkerAttribute>().Should().BeDecoratedWith<MarkerAttribute>(static _ => false));
        decoratedPredicate.Message.Should().Be("المتوقع من العضو 'Marked' أن يكون مزيّناً بـ MarkerAttribute مطابقاً للشرط، لكنه لم يكن كذلك.");

        var notDecoratedPredicate = Throws<AssertionFailedException>(
            () => typeof(OrderService).Methods().ThatAreDecoratedWith<MarkerAttribute>().Should().NotBeDecoratedWith<MarkerAttribute>(static _ => true));
        notDecoratedPredicate.Message.Should().Be("المتوقع من العضو 'Marked' ألا يكون مزيّناً بـ MarkerAttribute مطابقاً للشرط، لكنه كان كذلك.");

        var notWritable = Throws<AssertionFailedException>(
            () => typeof(OrderService).Properties().ThatSatisfy(static p => p.Name == "Name").Should().NotBeWritable());
        notWritable.Message.Should().Be("المتوقع من الخاصية 'Name' ألا تكون قابلة للكتابة، لكنها كانت كذلك.");
    }
}

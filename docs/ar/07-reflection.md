<div dir="rtl">

# الفصل ٧ — الانعكاس: الأنواع والأعضاء وقواعد البنية

كثيرًا ما تمرُّ قرارات بنيوية **دون أن تُكتب في شكل توكيد**. مثال شائع: يضيف أحدهم دالة عامة مؤقتة <code dir="ltr">Delete()</code> إلى كيان، ويُفترض أنها مؤقتة — وبعد أشهر يستدعيها كود آخر ظنًا منها جزءًا من العقد. لا يكسر هذا الأمر أي توكيد وظيفي، لذا يمرُّ مرور الكرام.

الحل هو تحويل القواعد الشفهية إلى **توكيدات قابلة للتنفيذ**. تصبح قواعد "كل كيان مختوم" أو "لا توجد دوال <code dir="ltr">Delete</code> عامة" جزءًا من مجموعة الاختبارات، فتُمسك بالتغيير في لحظة حدوثه داخل <code dir="ltr">CI</code>.

وقيمة هذا الضرب من التوكيدات أنه يفحص ما لا تفحصه الاختبارات الوظيفية بطبيعتها. الاختبار الوظيفي يسأل: هل يؤدي الكود عمله؟ أما القاعدة البنيوية فتسأل: هل بقي التصميم على الشكل الذي اتفقنا عليه؟ وقد تنجح كل السيناريوهات الوظيفية في نظام أفسدت بنيته ببطء، لأن التآكل البنيوي لا يُظهر فشلًا لحظيًا؛ يُظهر فقط أن الكود صار أصعب على الفهم والتعديل بعد عام. والاختبار البنيوي هو الذي يحوِّل هذا الانحراف البطيء إلى خط أحمر يظهر في اللحظة التي يُرتكب فيه.

## ٧.١ توكيدات الأنواع

نبدأ بالتحقُّق من خصائص النوع نفسه مباشرةً:

<div dir="ltr">

```csharp
typeof(Order).Should().BeSealed().And.NotBeAbstract();
typeof(Order).Should().BeAssignableTo<Entity>().And.NotBeAssignableTo<string>();
typeof(Order).Should().BeDecoratedWith<AuditedAttribute>();
typeof(Order).Should().NotBeDecoratedWith<ObsoleteAttribute>();
typeof(Order).Should().BeDecoratedWithOrInherit<AuditedAttribute>();
typeof(Order).Should().HaveAccessModifier(AccessModifier.Public);
```

</div>

كل قاعدة بنيوية كانت تُقال شفهيًا تحوَّلت إلى سطر واحد يُنفَّذ.

فرق دقيق بين طريقتين للسمات: <code dir="ltr">BeDecoratedWith()</code> يبحث عن سمة على النوع الحالي (مع مراعاة سلوك الوراثة المعتاد للسمات). أما <code dir="ltr">BeDecoratedWithOrInherit()</code> فيتحرَّى سلسلة الوراثة صراحةً ليجد السمات حتى لو وُضعت على صنف أساسي ولم تُعلَّم بأنها موروثة. والفرق ليس تفصيلًا أكاديميًا: كثير من سمات الأطر تُعلَن <code dir="ltr">Inherited = false</code> لأسباب محسوبة، فيصبح السؤال «هل نفحص ما يراه المستهلك فعلًا أم ما كُتب على الصنف بعينه؟» سؤال تصميم، والاختيار بين الفعلين هو الجواب عنه.

يتحدَّد مستوى الوصول عبر تعداد صريح: <code dir="ltr">Public</code>، <code dir="ltr">Internal</code>، <code dir="ltr">Protected</code>، <code dir="ltr">Private</code>، <code dir="ltr">ProtectedInternal</code>، <code dir="ltr">PrivateProtected</code> — ما يمنع الأخطاء الإملائية مقارنةً بالسلاسل النصية. والقيمة العملية أن إعادة التسمية أو تفكيك مستوى الوصول لا تمران بصمت: يقرر المصرّف على العضو بعينه لا على نص لا يتحقق منه أحد.

وكثيرًا ما يكون فحص الختم — <code dir="ltr">BeSealed()</code> — أهم سطر في الملف: النوع غير المختوم يُعلن ضمنا أن الوراثة منه مسموحة، وأن أي سلوك افتراضي فيه قابل للتبديل. فإن لم يكن ذلك مقصودًا معلنًا في التصميم، فيجب أن يحرسه توكيد، لأن السماح غير المقصود بالوراثة يُنشئ عقدًا لا يرضاه أحد.

## ٧.٢ توكيدات الأعضاء: الدوال والخصائص والمنشئات

عندما نريد التحقُّق من عضو بعينه، نستخدم <code dir="ltr">HaveMethod()</code> أو <code dir="ltr">HaveProperty()</code> أو <code dir="ltr">HaveConstructor()</code>، ثم نواصل التفاصيل عبر <code dir="ltr">.Which</code>:

<div dir="ltr">

```csharp
var method = typeof(Order).Should().HaveMethod("Total").Which;
method.Should().Return<decimal>()
    .And.HaveParameter("currency", typeof(string))
    .And.BeVirtual()
    .And.NotBeStatic()
    .And.HaveAccessModifier(AccessModifier.Public);

var property = typeof(Order).Should().HaveProperty("Id").Which;
property.Should().Return<Guid>()
    .And.BeReadable()
    .And.NotBeWritable();

typeof(Order).Should().HaveConstructor(typeof(Guid), typeof(string));
typeof(Order).Should().HaveConstructor().Which.Should().BePublic();
```

</div>

بهذا نُمسك بتفاصيل التوقيع بدقة: اسم المعامل ونوعه، ونوع الإرجاع، وهل هو افتراضي أم ثابت، ومستوى الوصول. أي تغيير في إعادة الهيكلة سيُكسر الاختبار فورًا بدلًا من الظهور كخطأ في وقت التشغيل.

القيمة الحقيقية لهذا الأسلوب في إعادة الهيكلة بوجه خاص. النوع العام للدالة لا يكفي عقدًا: نعرف أن <code dir="ltr">Total</code> موجودة، لكن هل معاملها ما زال <code dir="ltr">currency</code> من نوع <code dir="ltr">string</code>؟ هل ما زالت قابلة للتبديل في وراثة مخصَّصة، أم صارت مختومة صامتةً؟ القواعد التي لم تُكتب هي التي تُنسى عند أول ضغط تسليم، والاختبار البنيوي هو المكان الذي تُكتب فيه مرة واحدة وتُنسى بأمان.

### منع الأعضاء المحظورة

أقوى ما في هذا القسم هو **النفي**:

<div dir="ltr">

```csharp
typeof(Order).Should().NotHaveMethod("Delete");
typeof(Order).Should().NotHaveProperty("PasswordHash");
```

</div>

هذا بالضبط ما يمنع "الدالة المؤقتة" من البقاء خفية. يحمي العقد المقصود قبل أن يبدأ أحدهم باستدعائها. والمشكلة في العضو المؤقت أنه لا يتقادم بالسكوت؛ يتقادم بالاستعمال: كل مستدعٍ جديد يمثّل تصويتًا على إبقائه، وبعد فترة لا يعرف أحد كُتب ولماذا. التوكيد السالب يقطع هذا المسار في أوله.

قاعدة سلوك: لو كان النوع أو <code dir="ltr">MethodInfo</code> أو <code dir="ltr">PropertyInfo</code> أو <code dir="ltr">ConstructorInfo</code> أو <code dir="ltr">Assembly</code> الناتج <code dir="ltr">null</code>، فإن التوكيد الموجب يفشل، أما التوكيد السالب فينجح. أي: غياب العضو يعني أن <code dir="ltr">NotHave*</code> يمرّ بأمان — وهو السلوك الصحيح تمامًا: لا يمكنك أن تثبت عدم قابلية تغيير عنصر غير موجود، ولا معنى لرفض نفيٍ يخصُّ غيابًا هو الحالة المطلوبة أصلًا.

## ٧.٣ قواعد البنية على مستوى التجميع

ننتقل الآن من نوع واحد إلى **كل الأنواع** في التجميع. هذا مثالي لحفظ قواعد التصميم على مستوى النطاق.

### توكيدات التجميع

<div dir="ltr">

```csharp
var assembly = typeof(Order).Assembly;

assembly.Should().DefineType("Store.Orders.Order");
assembly.Should().NotDefineType("Store.Orders.LegacyOrder");
assembly.Should().Reference(typeof(System.Text.Json.JsonSerializer).Assembly);
assembly.Should().NotReference(legacyAssembly);
```

</div>

نُتحقَّق من وجود أنواع محدَّدة، أو غياب أنواع عفا عليها الزمن، أو وجود/غياب مراجع تجميعات. وأما فحص المراجع فقيمته في اتجاهين يتكاملان: يضمن ألا تسحب الوحدة مكتبة ثقيلة أو مهجورة، ويضمن في الوقت نفسه أن اعتمادا مطلوبا لا يُحذف من المشروع فينكسر التجميع في لحظة أقل توقعًا — عند النشر، لا عند البناء.

### توكيدات على كل الأنواع

الأكثر فاعلية هو تصفية الأنواع ثم توكيد قاعدة جماعية:

<div dir="ltr">

```csharp
AllTypes.From(typeof(Order).Assembly)
    .ThatDeriveFrom<Entity>()
    .Should().BeSealed();

AllTypes.From(typeof(Order).Assembly)
    .ThatAreInNamespace("Store.Orders")
    .Should().HaveCount(12);

AllTypes.From(typeof(Order).Assembly)
    .ThatImplement<IHandler>()
    .Should().HaveMethod("Handle");
```

</div>

تُركَّب المُرشِّحات معًا بطبيعية: <code dir="ltr">ThatDeriveFrom</code>، <code dir="ltr">ThatImplement</code>، <code dir="ltr">ThatAreInNamespace</code>، <code dir="ltr">ThatAreDecoratedWith</code> وغيرها. والنتيجة تستخدم نفس أفعال توكيد النوع المفرد: <code dir="ltr">BeSealed()</code>، <code dir="ltr">HaveCount()</code>، <code dir="ltr">HaveMethod()</code>، <code dir="ltr">HaveProperty()</code>...

بهذا نحصل على **اختبارات بنيوية حقيقية** تعمل داخل <code dir="ltr">CI</code> كأي اختبار آخر. لا تُقرأ في المراجعة اليومية، وتظهر الحاجة إليها عند أول انحراف بنيوي: يتحول التغيير غير المقصود إلى فشل مرئي في اللحظة التي يُرتكب فيها بدل أن يتراكم بلا ملاحظة. ولاحظ أن عدّ الأنواع في مساحة أسماء ليس تحكُّمًا ميكانيكيًا: كل رقم زائد أو ناقص لهذا العدد هو قرار تصميم حدث في مكان ما، وإجباره على المرور عبر مراجعة واحدة هو وظيفة هذا التوكيد.

### محددات الأعضاء: التوابع والخصائص

تختار الأنواع أعضاءها بالطريقة نفسها التي يختار بها التجميع أنواعه. نبدأ من <code dir="ltr">Type</code>، ثم نرشّح، ثم نؤكّد — والفشل يسمّي أول عضو مخالف:

<div dir="ltr">

```csharp
typeof(OrderService).Methods().ThatArePublic().ThatReturnVoid().Should().HaveCount(2);

typeof(OrderService).Methods().ThatArePublic().Should().HaveAccessModifier(AccessModifier.Public);
typeof(OrderService).Properties().ThatAreReadOnly().Should().BeReadOnly();
typeof(OrderService).Methods().ThatAreDecoratedWith<MarkerAttribute>().Should().HaveAttribute<MarkerAttribute>();
```

</div>

والرسالة التي تسمّي أول عضو مخالف ذات قيمة تشخيصية عالية: بدل أن تسأل «أي من المئتين خالف القاعدة؟» تجيبك الرسالة بالمخالف الأول، وعند إصلاحه يظهر التالي إن وُجد — والمحصلة أن عملية الإصلاح تتدفق كما يتدفق إصلاح سلسلة فحوص، لا كما تُقرأ لائحة أخطاء طويلة.

وتنتقل التحديدات بين الأصناف: تعود أنواع الإرجاع إلى تحديد الأنواع عبر <code dir="ltr">ReturnTypes()</code>، وتُختار الأنواع المتداخلة عبر <code dir="ltr">Types()</code> (وتُستبعد الأنواع المولّدة من المصرّف):

<div dir="ltr">

```csharp
typeof(OrderService).Methods().ThatAreAsync().ReturnTypes().Should().HaveCount(1);
typeof(OrderService).Types().Should().HaveCount(2);
```

</div>

تغطي مرشّحات الخصائص إمكانية الوصول (يكفي أي موصِل)، وstatic/virtual/abstract، والقراءة فقط/الكتابة فقط/القابلة للكتابة، و<code dir="ltr">OfType&lt;T&gt;</code>، والسمات، ومسند <code dir="ltr">ThatSatisfy()</code> وحيد — ولا يوجد <code dir="ltr">Where()</code> منفصل عمداً. والمرشّحات توابع (لا خصائص)، تماشياً مع أسلوب <code dir="ltr">TypeSelector</code> في هذه المكتبة لا الأسلوب المختلط (خصائص وتوابع).

وقاعدة «المرشّح تابع لا خاصية» اختيار اتساق لا تفصيل أسلوبي: سطح واحد الشكل يُسهل التعلُّم والاكتشاف، ويُبقي الأخطاء المتفاوتة بعيدة عن بنية الـAPI نفسها.

## ٧.٤ خلاصة وتمرين

**الخلاصة:**

- **حوِّل القواعد الشفهية إلى توكيدات.** كل اتفاقية بنيوية تستحق سطرًا تنفيذيًا.
- **انتقل من النوع إلى العضو عبر <code dir="ltr">.Which</code>.** لتفاصيل التوقيع ومستوى الوصول والسكونية.
- **استخدم النفي لحماية العقد.** <code dir="ltr">NotHaveMethod()</code> يمنع الأعضاء "المؤقتة" قبل أن تُصبح دائمة بالاستعمال.
- **ارفع المستوى إلى التجميع.** <code dir="ltr">AllTypes.From()</code> يفرض قواعد على كل الأنواع دفعة واحدة.
- **افحص ما يراه المستهلك فعلًا.** <code dir="ltr">BeDecoratedWithOrInherit()</code> و<code dir="ltr">DefineType()</code> يقرآن الحقيقة البنيوية، لا نية الكاتب.

**تمرين:**

1. اشترط أن **جميع كيانات النطاق** مختومة (<code dir="ltr">BeSealed()</code>).
2. امنع ظهور أي دالة باسم <code dir="ltr">Delete</code> في مساحة أسماء النطاق.
3. ثبِّت عدد الأنواع داخل مساحة أسماء محدَّدة.
4. أضف عضوًا مخالفًا عمدًا وشاهِد الاختبار يمسكه فورًا.

</div>

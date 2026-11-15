<div dir="rtl">

# الفصل ٩ — غرفة المحرِّك: النطاقات والثقافة وتنسيق القيم والإعدادات

التوكيدات نفسها نصف المكتبة فقط. النصف الآخر هو **ما يحدث حولها**: هل يتوقَّف عند أول فشل، أم تُجمَع الإخفاقات كلها؟ هل تتسرَّب ثقافة أحد الاختبارات إلى اختبار آخر يجري بالتوازي؟ وكيف تُعرض أنواعنا المخصَّصة في رسالة الفشل؟

هذا الفصل يشرح الآليات التي تجعل مئات التوكيدات تتصرَّف كنظام واحد متناسق. والمشكلة التي يعالجها ليست في توكيد بعينه، بل في السلوك الجَماعي: نظام من مئات الاختبارات المتوازية لا يكفي فيه أن يكون كل توكيد صحيحًا بمفرده، بل يجب أن تبقى الحالة المشتركة — الثقافة والساعة والمنسّقات والافتراضات — معزولة بحيث لا يفسد أحدها الآخر. وهذه هي «غرفة المحرِّك»: ما لا يُكتب في الاختبار عادةً، لكنه يحدد هل يبقى الاختبار صادقًا تحت كل ظروف التشغيل.

## ٩.١ النطاق: تقرير شامل بدل رمي أول فشل

خارج نطاق التوكيد (<code dir="ltr">AssertionScope</code>)، يُرمى الاستثناء عند **أول فشل** مباشرةً. داخل النطاق، تُجمَع **جميع الإخفاقات** ثم تُرمى معًا في تقرير واحد.

<div dir="ltr">

```csharp
using (new AssertionScope()) {
    total.Should().BePositive();
    lines.Should().NotBeEmpty();
    customer.Should().NotBeNull();
}
```

</div>

لو فشل توكيدان معًا داخل النطاق، سترى كلا الإخفاقين في رسالة واحدة — ما يوفر دورات تصحيح كثيرة ويُظهر الصورة الكاملة للمشكلة دفعة واحدة.

**قاعدة مهمة:** يُجمَع فقط ما هو **مستقل منطقيًا**. الفحوص التابعة — أي التي لا معنى لها إن فشل ما قبلها — يُفضَّل إبقاؤها خارج النطاق أو متسلسلة. جمع الإخفاقات بعد شرط مسبق مكسور يولِّد ضجيجًا بدل التشخيص الواضح. والاختبار العملي بسيط: اسأل عن كل فحص «لو فشل الذي قبله، هل تبقى نتيجة هذا ذات معنى؟» فإن كان الجواب لا، فالفحص تابع مهما بدا مستقبلًا في الكتابة، ومكانه خارج الجمْع.

### تداخل النطاقات والتسمية

تتداخل النطاقات: إخفاقات النطاق الداخلي تُمرَّر إلى النطاق الخارجي وتُضمَّن في تقريره. ويمكننا تسميته لتصنيف التقرير:

<div dir="ltr">

```csharp
using (new AssertionScope("Pricing")) {
    // ...
}
```

</div>

والاسم ليس زخرفة: حين يضم التقرير إخفاقات من أكثر من مرحلة — تسعير ثم خصم ثم ضريبة — يكون العنوان هو الفاصل الذي يجعل قراءة التقرير مسحًا لأقسام لا كومةً من الجمل. والتسمية تظهر في الرسالة النهائية كما كتبتها، فتختار لها اسمًا يفهمه من سيقرأ الأحمر في الليل لا من كتبه اليوم.

### التخلص من التجميع دون الرمي

أحيانًا نريد **التحقُّق من أن الإخفاقات تُجمَع** دون أن يُفشل الاختبار. هنا نستخدم <code dir="ltr">Discard()</code>:

<div dir="ltr">

```csharp
var failures = scope.Discard();
failures.Should().Contain(f => f.Contains("total"));
```

</div>

يُعيد <code dir="ltr">Discard()</code> مصفوفة من نصوص الإخفاقات المجمَّعة. مفيد لاختبارات المكتبة نفسها أو للتقارير المخصَّصة. وهذه الأداة تحديدًا هي ما يجعل «تجميع الإخفاقات» نفسه قابلًا للاختبار: أنت لا تفحص نتيجةً خرجت من الكود، بل تفحص **آلية الفحص** — وهو مستوى من الاختبار لا تُتيحه إلا مكتبة تعرِّض حالتها التشخيصية بوضوح.

ويمكن للأطر الخارجية والأدوات مراقبة النطاق المحيط دون امتلاكه: <code dir="ltr">AssertionScope.Current</code> يُعيد النطاق النشط (أو <code dir="ltr">null</code> خارجه)، و<code dir="ltr">scope.Name</code> يُعيد اسم الإنشاء، و<code dir="ltr">scope.HasFailures()</code> يبلِّغ عن إخفاقات معلَّقة. كلها تشخيصات قراءة فقط؛ ويبقى التجميع عبر التوكيدات العادية وحدها. والقاعدة هنا مقصودة: من يستطيع الكتابة يستطيع الإفساد؛ فتظل القناة العامة للقراءة، وتظل الكتابة الطريق الوحيد المعتاد — التوكيدات.

## ٩.٢ الثقافة: لكل تدفق ثقافته دون تسريب

تدعم المكتبة ترجمة رسائل الفشل إلى لغات متعددة. افتراضيًا الإنجليزية، ويمكن تغييره لكل تدفق (<code dir="ltr">AsyncLocal</code>):

<div dir="ltr">

```csharp
AssertionCulture.Current = new CultureInfo("ar");
flag.Should().BeTrue();
// الرسالة بالعربية عند الفشل
```

</div>

يضمن العزل بين التدفقات غير المتزامنة أن اختبارين يجريان بالتوازي بثقافتين مختلفتين **لا يتسرَّبان** إلى بعضهما. وأساس العزل أن الحالة محفوظة في سياق التدفق لا في خيط أو عملية: اختبارات المشغِّل الحديثة تعمل بالتوازي افتراضيًا، ولو كانت الثقافة حالة عامة لصار نتاج كل اختبار يعتمد على ترتيب تشغيله — وهو أسوأ أنواع عدم الحتمية لأنه يظهر ويتوارى بحسب ازدحام الجهاز.

### ضمانات مهمة

1. **القيم تبقى ثابتة**: الأرقام، التواريخ، الأحجام، والقيم المُنسَّقة تُنسَّق دائمًا بتنسيق ثابت لا يتأثر بالثقافة (<code dir="ltr">InvariantCulture</code>). فقط النثر يُترجم. هذا يحافظ على رسائل الفشل **قابلة للمقارنة النصية** داخل الاختبارات.
2. **اللغات غير المعروفة تسقط إلى الإنجليزية**: آمنة ولا تفشل عند إضافة لغات جديدة.
3. **<code dir="ltr">because</code> وعناوين النطاق تُترجم أيضًا**: التكامل كامل.

ولماذا كان ثبات القيم خاصية لا تفصيل؟ لأن اختبارات المكتبة نفسها تقارن رسائل الفشل سطرًا بسطر، ولو تغيّر شكل الرقم أو التاريخ بتغيّر لغة التشغيل لصار كل اختبار رسالة يتقلب مع بيئة من ينسخ المستودع. فالرسالة المغربية: النثر مترجم والقيم ثابتة، فيبقى معنى الرسالة مرنًا ويبقى نصها حجّة يمكن الحكم عليها.

### أضف لغة في ١٠ دقائق

التوطين نشاط على ملفات الموارد، لا على الكود:

1. انسخ الملف المحايد وأعِد تسميته حسب الثقافة:

<div dir="ltr">

```text
src/Jeninnet.Assertions/Localization/Strings.resx
→ src/Jeninnet.Assertions/Localization/Strings.<culture>.resx
```

</div>

2. ترجم قيم <code dir="ltr">&lt;value&gt;</code>. اللغات المشحونة يجب أن تكون مكتملة المفاتيح؛ وتفرض <code dir="ltr">check-localization.ps1</code> عدم وجود مفتاح ناقص/زائد/فارغ (وقمر الفرنسية المرجعي مسموح به صراحةً كجزئي). حافظ على العناصر النائبة <code dir="ltr">{0}</code> و<code dir="ltr">{1}</code>، واترك القيم المنسَّقة (<code dir="ltr">true</code> والأرقام والتواريخ) كما هي لتبقى الرسائل قابلة للمقارنة بايتيًّا.
3. ابنِ. يولِّد SDK تجميع القمر الصناعي (<code dir="ltr">Strings.&lt;culture&gt;.resources.dll</code>) بلا أي ربط إضافي.

واقرأ القوالب المترجمة لأي تقارير مخصَّصة عبر القارئ العام:

<div dir="ltr">

```csharp
AssertionCulture.Current = new CultureInfo("ar");
var template = AssertionCulture.GetMessage("BOOLEAN_BE_TRUE");         // قالب مترجم
var message = AssertionCulture.FormatMessage("BOOLEAN_BE_TRUE", "flag"); // تنسيق ثابت
```

</div>

الإنجليزية هي المورد المحايد؛ والعربية (<code dir="ltr">ar</code>) والإسبانية (<code dir="ltr">es</code>) والألمانية (<code dir="ltr">de</code>) تُشحن كاملة. والفرنسية (<code dir="ltr">fr</code>) قمر مرجعي جزئي يوضِّح الرجوع للنص الإنجليزي لكل مفتاح غير مترجم. و<code dir="ltr">qps-ploc</code> توطين شبهي مُلتزَم لكشف النصوص المضمنة. بوابة الاكتمال هي <code dir="ltr">pwsh scripts/docs/check-localization.ps1</code>؛ تفشل عند مفتاح ناقص/زائد/فارغ (وقمر الفرنسية مسموح به صراحةً كجزئي)، ودليل المترجم في <a href="../../CONTRIBUTING.md#translation-guide">CONTRIBUTING.md</a>.

## ٩.٣ تنسيق القيم: علِّم المكتبة كيف تُظهر أنواعك

لو طبع الفشل <code dir="ltr">Store.Money</code> بدلًا من <code dir="ltr">$42.00</code> أو <code dir="ltr">USD 42.00</code>، فالمشكلة في **العرض**، وليس في التوكيد. الحل هو **منسِّق قيم** (<code dir="ltr">IValueFormatter</code>) لا توكيد جديد في كل اختبار.

<div dir="ltr">

```csharp
public sealed class MoneyFormatter : IValueFormatter {
    public bool CanFormat(object? value) => value is Money;
    public string Format(object? value) => ((Money)value!).ToDisplayString();
}
```

</div>

والمسألة ليست تجميلًا للرسالة، بل حفاظ على قيمتها التشخيصية. رسالة تقول <code dir="ltr">Expected price to be Store.Money, but found Store.Money</code> لا تقول شيئًا ولا تُقارن شيئًا؛ ورسالة تقول <code dir="ltr">Expected price to be USD 42.00, but found USD 41.99</code> تجيب عن السؤال قبل أن يُطرح. وكل دقيقة تُوفَّر على قارئ الفشل هي مردود مباشر من هذه السطور الثلاثة.

### تسجيل المنسِّقات

يمكن تسجيله **على مستوى النطاق**:

<div dir="ltr">

```csharp
using (var scope = new AssertionScope()) {
    scope.Formatting.AddFormatter(new MoneyFormatter());
    // ...
}
```

</div>

أو **على مستوى عام** يشمل كل الاختبارات:

<div dir="ltr">

```csharp
AssertionConfiguration.Formatting.AddFormatter(new MoneyFormatter());
AssertionConfiguration.Formatting.RemoveFormatter<MoneyFormatter>();
AssertionConfiguration.Formatting.Clear();
```

</div>

**قاعدة صارمة:** المنسِّقات تغيِّر **العرض فقط**، ولا تغيِّر **المقارنة** أبدًا. تقارن التوكيدات القيم الحقيقية دائمًا. عرض نوع النطاق (<code dir="ltr">Domain Type</code>) يخصُّه أن يكون له منسِّقه الخاص بدل الاعتماد على <code dir="ltr">ToString()</code> الافتراضي الذي غالبًا ما يكون غير واضح أو غير مفيد لرسالة الفشل. وهذه القاعدة هي الفرق بين أداة عرض وأداة تشويه: لو أثر المنسِّق في المقارنة لصار تغييره — وهو كود تشخيصي بحت — قادرًا على تغيير نتيجة اختبار، وذلك نوع من الاقتران لا يُغتفر في طبقة توكيدات.

### حدود التنسيق

يحدّ <code dir="ltr">ValueFormatterOptions</code> أيضًا من حجم النص الذي تحمله رسالة الفشل، حتى لا يغرق رسم بياني عميق أو نص ضخم الرسالة. تُطبَّق الحدود بعد تشغيل المنسِّقات، وتُضبط لكل نطاق أو على مستوى العملية:

<div dir="ltr">

```csharp
using (var scope = new AssertionScope()) {
    scope.Formatting.MaxDepth = 10;           // عمق التداخل البنيوي
    scope.Formatting.MaxLines = 100;          // الأسطر المعروضة
    scope.Formatting.MaxEnumerables = 100;    // العناصر لكل تعداد
    scope.Formatting.MaxStringLength = 8_000; // الأحرف
    scope.Formatting.UseLineBreaks = true;    // false يطوي النص في سطر واحد
    scope.Formatting.RespectRuntimeTypes = true; // عرض بنيوي اختياري
}
```

</div>

تحافظ الافتراضات على المخرجات التاريخية: <code dir="ltr">MaxDepth</code> ‏10، و<code dir="ltr">MaxLines</code> ‏100، و<code dir="ltr">MaxEnumerables</code> ‏100، و<code dir="ltr">MaxStringLength</code> ‏8,000، و<code dir="ltr">UseLineBreaks</code> مفعّل، و<code dir="ltr">RespectRuntimeTypes</code> معطّل. والاقتطاع ليس صامتًا أبدًا: ينتهي النص بعلامة <code dir="ltr">…</code> وعدد الأسطر أو الأحرف المخفية. وتُحفظ نهايات الأسطر افتراضيًا؛ فقط <code dir="ltr">UseLineBreaks = false</code> يطوي المسافات.

<code dir="ltr">RespectRuntimeTypes</code> هو المفتاح الذي يغيّر عرض المجموعات: عند تفعيله تُعرض المصفوفة كـ<code dir="ltr">[1, 2, 3]</code> (تعاوديًا، حتى <code dir="ltr">MaxDepth</code> و<code dir="ltr">MaxEnumerables</code>، مع علامات <code dir="ltr">… (truncated)</code> و<code dir="ltr">… (max depth)</code> و<code dir="ltr">… (cycle)</code>). وعند تعطيله يبقى المحرك يستدعي <code dir="ltr">ToString()</code> كما كان تمامًا. وتنقل <code dir="ltr">Copy()</code> و<code dir="ltr">RestoreFrom()</code> المنسِّقات **والحدود** معًا، لذا يعيد <code dir="ltr">AssertionIsolation.Isolate()</code> إعداد التنسيق كاملًا.

هدف مستبعد عمدًا: عقد <code dir="ltr">IValueFormatter</code> هذا (<code dir="ltr">CanFormat</code> + <code dir="ltr">Format</code>) **غير متوافق** مع الواجهة التي تحمل الاسم نفسه في مكتبات التأكيدات الأخرى. توحيدهما سيكسر كل منسِّق موجود دون قدرة جديدة.

## ٩.٤ الإعدادات والمهيئات: افتراضات مرة واحدة

لتفادي التكرار في كل اختبار، نرفع الافتراضات إلى مستوى التكوين العام.

<code dir="ltr">AssertionConfiguration</code> يحتوي على مقبضين رئيسيين:

- <code dir="ltr">Formatting</code> — منسِّقات القيم (كما أعلاه)
- <code dir="ltr">Equivalency</code> — افتراضات <code dir="ltr">BeEquivalentTo()</code>: <code dir="ltr">StrictOrderingDefault</code>، وخطة خطوات مخصَّصة مشتركة <code dir="ltr">Plan</code>، وقالب محيطي يُعدَّل بـ<code dir="ltr">Modify(o =&gt; { o.StrictOrdering = true; })</code> ويُنسخ بـ<code dir="ltr">CloneDefaults()</code> ويُصفَّر بـ<code dir="ltr">EquivalencyDefaults.ResetToDefaults()</code>

تجاوزات <code dir="ltr">Modify</code> معزولة لكل تدفق (<code dir="ltr">AsyncLocal</code>) تمامًا كالنطاقات والثقافة: الاختبارات المتوازية لا ترى تجاوزات بعضها. وتوكيد كل استدعاء يغلب القالب المحيطي، الذي يغلب الافتراضات العامة. توجيه الفشل قابل للتركيب عبر الواجهة العلنية <code dir="ltr">IAssertionStrategy</code> (<code dir="ltr">AssertionScope(IAssertionStrategy)</code>) مع استراتيجية تجميع افتراضية تجمع الإخفاقات حتى يُصفَّى النطاق. وهدفان مستبعدان عمدًا ومسجَّلان هنا: لا <code dir="ltr">AssertionChain</code> علنية (سلسلة <code dir="ltr">GivenSelector</code> تغطي الفحص الشرطي متعدد الخطوات؛ وإلا فالتوسعة عبر <code dir="ltr">IEquivalencyStep</code> والقواعد و<code dir="ltr">AssertionScope</code> والمنسِّقات بدل خط تأكيد ثانٍ)، ولا خيار إصلاح تلقائي في أي مكان (إعادة كتابة الكائن داخل التوكيد ستعدّل بيانات المستخدم).

وسلسلة الأولوية — توكيد ثم محيط ثم عام — هي ما يجعل التخصيص ممكنًا بلا فوضى: التفضيل الجماعي للفريق يُكتب مرة في التكوين، والاستثناء لاختبار بعينه يُكتب في موضعه، ولا يمحو أحدهما الآخر. وأما استثناء الإصلاح التلقائي فقرار مبدئي: مكتبة التوكيدات وسيلة للملاحظة لا للتعديل، والفرق بينهما حد لا يجوز أن يُمحى من أجل راحة لحظية.

### عزل الحالة المحيطية

ما تغيّره دورة اختبار لا يجوز أن يتسرب إلى التي تليها. سطر واحد يعزل المقابض المحيطية — الثقافة والساعة والتنسيق (المنسّقات والحدود) وexpectation النطاق النشط وقالب التكافؤ المحيطي — ويعيدها عند الخروج، حتى لو خرج النطاق عبر استثناء:

<div dir="ltr">

```csharp
using(AssertionIsolation.Isolate()) {
    AssertionCulture.Current = new CultureInfo("ar");
    AssertionClock.Current = new ManualClock(epoch);
    AssertionConfiguration.Formatting.AddFormatter(new MoneyFormatter());
    AssertionConfiguration.Formatting.MaxLines = 20;
    AssertionScope.Current?.WithExpectation("Expected the invoice");
    AssertionConfiguration.Equivalency.Modify(o => o.StrictOrdering = true);

    // ... اختبار يتوقع كل ما سبق ...
}
// تعود الثقافة والساعة والتنسيق وexpectation النطاق والتكافؤ إلى قيمها قبل النطاق.
```

</div>

والنطاقات المتداخلة تعيد كلٌّ لقطته، وأجزاء الثقافة والساعة وexpectation والتكافؤ لكل تدفق غير متزامن (<code dir="ltr">AsyncLocal</code>) فتظل الاختبارات المتوازية معزولة. وثمة حدّان مقصودان: سجل المنسّقات عام على مستوى العملية (يمنع النطاق التسرب المتسلسل، أما الاختبارات المتوازية التي تعدّله فيلزمها تنسيق خاص)، والحالة خارج مكتبتنا — كـ<code dir="ltr">CultureInfo.CurrentCulture</code> للخيط — لا تُلمَس.

وترك الحالة الخارجية دون لمس ليس تقصيرًا بل حدّ مسؤولية: المكتبة تحفظ ما تملكه، ولا تدّعي ملكية ما ليس لها؛ فمن يعزل <code dir="ltr">CurrentCulture</code> ثم يفاجأ بأن مكتبة توكيدات أعادتها إلى وضع آخر، سيفقد الثقة بكل ما تعزله. والاستثناء الوحيد المعلن — سجل المنسّقات المشترك — مسجَّل بصراحة في هذا الفصل، ويُعالج بالتزام المؤلِّف: اختبار يعدّل السجل عليه أن يعيده أو يتوقع عزلًا متسلسلًا فقط.

### مهيئات على مستوى التجميع

للإعداد لمرة واحدة قبل أي اختبار، نستخدم سمة <code dir="ltr">AssertionEngineInitializer</code>:

<div dir="ltr">

```csharp
[assembly: AssertionEngineInitializer(typeof(TestSetup), nameof(TestSetup.Initialize))]

public static class TestSetup {
    public static void Initialize() {
        AssertionConfiguration.Formatting.AddFormatter(new MoneyFormatter());
        // إعدادات أخرى لمرة واحدة
    }
}
```

</div>

تُنفَّذ هذه المهيئات **مرة واحدة** عند تحميل محرِّك التوكيدات، وقبل أن يبدأ أي اختبار. لذا يجب أن تكون **سريعة وبلا آثار جانبية**. والتكلفة المحسوبة هنا زمانية: أي عمل ثقيل في المهيئة يدفعه كل تشغيل للمشغِّل، وأي أثر جانبي فيها يلوّث كل اختبار بعدها بلا استئذان — فحدّها العملي: إعداد لا يُقاس، وبلا حالة عابرة.

### التوكيدات المخصَّصة

لكتَّاب التوكيدات المخصَّصة، تستخدم المكتبة <code dir="ltr">[CustomAssertion]</code> (و<code dir="ltr">[CustomAssertionsAssembly]</code> للتجميعات) لتحديد حدود التوكيد بوضوح. وتستهلكها المضيفات والأدوات عبر <code dir="ltr">CustomAssertionDiscovery</code>: <code dir="ltr">FindMarkedAssemblies()</code> يحدِّد تجميعات التوسعة، و<code dir="ltr">FindCustomAssertions(assembly)</code> يتحقَّق من أعضائها الموسومة (يجب أن تكون static علنية)، و<code dir="ltr">RunInitializers(assembly)</code> ينفِّذ إعدادات محرِّكها — بما فيها التجميعات المحمَّلة بعد أول توكيد، التي يفوِّتها الفحص التلقائي. أما التوسعة اليومية فتبقى عبر <code dir="ltr">AssertionScope</code> وحده؛ وواجهة الاكتشاف لتركيب تلك التوسعات.

والفكرة أن التوسعة تُصنِّف نفسها: تجميعة تحتوي توكيدات مخصَّصة تعلن ذلك، فيستطيع مضيف أن يجدها ويفحص عقدها ويشغّل مهيئتها دون أن يعرف أسماءها مسبقًا. وهذا ما يجعل مكتبة التوكيدات قابلة للامتداد من خارج ملحقها دون فتح باب الاكتشاف العشوائي في كل مكان.

<code dir="ltr">AssertionsBase</code> هو الأساس المدعوم لصنف توكيد مخصَّص. اشتقّ منه، واستخدم <code dir="ltr">Expression</code> للتعبير الملتقط للموضوع، ونادِ <code dir="ltr">Fail</code> بالرسالة الكاملة مع سبب <code dir="ltr">because</code> الاختياري؛ يضيف <code dir="ltr">Fail</code> السبب ويوجّه الفشل إلى النطاق النشط (وتحتفظ الإخفاقات المهيكلة بالتعبير والقيمتين الفعلية والمتوقعة من أجل <code dir="ltr">SoftAssertions</code>):

<div dir="ltr">

```csharp
public sealed class OrderAssertions(Order order, string? expression = null)
    : AssertionsBase(expression, "order") {
    public AndConstraint<OrderAssertions> BePaid(string? because = null, params object?[] becauseArgs) {
        if(order.Status != "Paid") {
            Fail($"Expected {Expression} to be paid, but found {FormatValue(order.Status)}.", because, becauseArgs);
        }

        return new(this);
    }
}
```

</div>

يُنسِّق <code dir="ltr">FormatValue</code> بالثقافة الثابتة وبحدود التنسيق النشطة، فتبقى الرسائل المخصَّصة مستقرة بايتًا ببايت. ويُلاحظ <code dir="ltr">AssertionScope.Current</code> المجمِّع المحيط عندما يحتاج مساعد إلى فحص الإخفاقات أو إضافة رسالة منسَّقة مسبقًا. ولإعادة تسمية الموضوع داخل كتلة، اضبط expectation على النطاق؛ ترثه النطاقات المتداخلة دون تعديل الأب، ويعيد <code dir="ltr">ClearExpectation()</code> الافتراضي:

<div dir="ltr">

```csharp
using (new AssertionScope().WithExpectation("Expected order #42")) {
    order.Should().BePaid(); // Expected order #42 to be paid, but found Pending.
}
```

</div>

## ٩.٥ ساعات حتمية: اختبر الوقت بلا نوم

تقرأ الأفعال الزمنية (<code dir="ltr">BeWithin</code> و<code dir="ltr">BeLessThan</code> و<code dir="ltr">BeMoreThan</code> و<code dir="ltr">BeAtLeast</code>) اللحظة الحالية من مصدر واحد قابل للحقن: <code dir="ltr">AssertionClock.Current</code>، وهو <code dir="ltr">IClock</code> افتراضه <code dir="ltr">SystemClock</code> (ساعة الجهاز). وحقن ساعة ثابتة يجعل اختبارات الحدود حتمية — بلا <code dir="ltr">Thread.Sleep</code> وبلا سباقات زمن الحائط:

<div dir="ltr">

```csharp
AssertionClock.Current = new ManualClock(new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero));
try {
    var value = new DateTime(2026, 1, 1, 0, 5, 0, DateTimeKind.Utc);
    value.Should().BeWithin(TimeSpan.FromMinutes(5));      // تنجح عند الحد تمامًا
    value.Should().BeWithin(TimeSpan.FromMinutes(3));      // تفشل حتميًا
}
finally {
    AssertionClock.Current = SystemClock.Instance;
}
```

</div>

والقيمة العملية لهذا الحقن أنه يجعل اختبار الحدود ممكنًا أصلًا. بدون ساعة قابلة للحقن، إما تنتظر حقيقةً — فتتحول دورة الاختبارات إلى سلسلة انتظارات — أو تختار قيمة بعيدة عن الحد، فلا تفحص الحد بل تفحص ما حوله. ومع <code dir="ltr">ManualClock</code> يقف الاختبار على الحد بالضبط: يثبت أن القاعدة تشمل اللحظة وتفصلها في آن، وهي بالضبط اللحظة التي تظهر فيها أخطاء المقارنة في الإنتاج.

تُشحن <code dir="ltr">ManualClock</code> مع المكتبة: ابدأها من لحظة ثابتة، ونادِ <code dir="ltr">Advance</code>/<code dir="ltr">Set</code> في المواضع التي يعيش فيها الكود الإنتاجي مرور الوقت — بلا <code dir="ltr">Thread.Sleep</code>. وعلى .NET 8+ يصل <code dir="ltr">TimeProviderClock</code> أي <code dir="ltr">TimeProvider</code> (بما فيه المزوِّدات الوهمية) إلى نقطة الحقن ذاتها، فتُوصل الأدوات الوهمية القائمة مباشرة؛ وأعد الضبط في التنظيف دائمًا، كما أعلاه.

ويعرض <code dir="ltr">IClock</code> الخصيصتين <code dir="ltr">Now</code> (محلية) و<code dir="ltr">UtcNow</code>؛ وتختار توكيدات <code dir="ltr">DateTime</code> قيمة <code dir="ltr">UtcNow</code> للقيم ذات <code dir="ltr">DateTimeKind.Utc</code> و<code dir="ltr">Now</code> فيما عدا ذلك، بينما تستخدم توكيدات <code dir="ltr">DateTimeOffset</code> الخصيصة <code dir="ltr">Now</code>. وكالنطاقات والثقافة، يُعزل <code dir="ltr">AssertionClock.Current</code> لكل تدفق غير متزامن (<code dir="ltr">AsyncLocal</code>)، فلا ترى الاختبارات المتوازية ساعة غيرها. ويُستخدم <code dir="ltr">SystemClock.Instance</code> الافتراضي عند عدم حقن شيء.

وقاعدة اختيار <code dir="ltr">UtcNow</code>/<code dir="ltr">Now</code> تتبع نوع القيمة لا مزاج الكاتب: القيمة المحلية تحمل دلالة محلية فتقارن بساعة محلية، والقيمة المؤطَّرة بـUTC تقارن بساعة UTC، فلا تبتلع منطقة زمنية يظن أحدهم أنها فُحصت وهي لم تُفحص.

## ٩.٦ التشذيب وNativeAOT

النواة الخالية من الانعكاس آمنة تحت التشذيب وNativeAOT. يشحن المستودع عينة <code dir="ltr">samples/Jeninnet.Assertions.Aot</code>، وهي تطبيق وحدة تحكم يمارس <code dir="ltr">Be</code> و<code dir="ltr">BeTrue</code> و<code dir="ltr">Contain</code> و<code dir="ltr">HaveCount</code> و<code dir="ltr">Throw</code>، ويُنشر مشذَّبًا وNativeAOT ويعمل بنجاح:

<div dir="ltr">

```powershell
dotnet publish samples/Jeninnet.Assertions.Aot/Jeninnet.Assertions.Aot.csproj -c Release -p:TrimSample=true
dotnet publish samples/Jeninnet.Assertions.Aot/Jeninnet.Assertions.Aot.csproj -c Release -p:AotSample=true
```

</div>

| السطح | التشذيب/NativeAOT | ملاحظات |
|---|---|---|
| القيم والنصوص والمجموعات والقواميس والأرقام والوقت والأفعال/async وHTTP وIO والتدفقات | آمن | لا انعكاس على بيانات أنواع عشوائية. |
| النطاقات والثقافة والمنسّقات والمدققات وتقارير CI | آمن | التنسيق ثابت وحتمي. |
| توكيدات الأنواع/الأعضاء/التجميعات/المعاملات و<code dir="ltr">AllTypes</code> و<code dir="ltr">TypeSelector</code> | <code dir="ltr">RequiresUnreferencedCode</code> | حافظ على الأنواع/الأعضاء المفحوصة أو استخدم توكيدات بلا انعكاس. |
| التكافؤ (<code dir="ltr">BeEquivalentTo</code> و<code dir="ltr">EquivalencyEvaluation.Evaluate</code>) | <code dir="ltr">RequiresUnreferencedCode</code> | مرور انعكاسي على الأعضاء؛ بلا توليد كود ديناميكي. |
| مراقبة الأحداث | <code dir="ltr">RequiresUnreferencedCode</code> + <code dir="ltr">RequiresDynamicCode</code> | انعكاس على الأحداث وترجمة accessors. |
| اللقطات (<code dir="ltr">MatchSnapshot</code>) | <code dir="ltr">RequiresUnreferencedCode</code> | بيانات إطار الاستدعاء؛ ولقطات الكائنات تحتاج أيضًا تسلسل JSON ديناميكيًا. |
| <code dir="ltr">BeJsonSerializable</code> / <code dir="ltr">BeDataContractSerializable</code> / لقطة الكائن | <code dir="ltr">RequiresUnreferencedCode</code> + <code dir="ltr">RequiresDynamicCode</code> | استخدم عقود JSON المولَّدة للمصدر مع NativeAOT. |
| <code dir="ltr">BeXmlSerializable</code> | <code dir="ltr">RequiresUnreferencedCode</code> | يحتاج <code dir="ltr">XmlSerializer</code> بيانات الأعضاء. |
| <code dir="ltr">JsonNode.HaveValue&lt;T&gt;</code> | <code dir="ltr">RequiresUnreferencedCode</code> + <code dir="ltr">RequiresDynamicCode</code> | تحويل قيمي عشوائي. |
| <code dir="ltr">CustomAssertionDiscovery</code> | <code dir="ltr">RequiresUnreferencedCode</code> | يفحص التجميعات والأعضاء. |

المكتبة **لا** تضبط <code dir="ltr">IsAotCompatible=true</code> عمدًا: فعل ذلك سيحوّل اقتراحات الرابط المتبقية للعائلات الانعكاسية إلى أخطاء بناء. والسمات موضوعة على نقاط الدخول العامة، فيرى مستخدم AOT تحذيرًا صريحًا ويستطيع إما حفظ البيانات (<code dir="ltr">DynamicallyAccessedMembers</code>) أو الكبت في موقع الاستدعاء بسبب موثَّق. أما العينة نفسها فتضبط <code dir="ltr">IsAotCompatible=true</code> لإثبات أن السطح الآمن يبقى بلا تحذيرات.

## ٩.٧ الأداء والتحقق من الإصدار

مسار النجاح مصمَّم ليبقى رخيصًا: الأفعال الساخنة تلتقط تعبير الموضوع وتقارن، لكنها لا تنسّق رسالة إلا عند الفشل. يثبّت <code dir="ltr">PerformanceInvariantTests</code> ذلك بحدود تخصيص (لا بحدود زمن الجدار)، وينشر مشروع <code dir="ltr">benchmarks/Jeninnet.Assertions.Benchmarks</code> قياسات قابلة للتكرار:

<div dir="ltr">

```powershell
dotnet run --project benchmarks/Jeninnet.Assertions.Benchmarks/Jeninnet.Assertions.Benchmarks.csproj -c Release -- --filter '*' --exporters github
```

</div>

يشغّل سير عمل <code dir="ltr">Benchmarks</code> الأسبوعي الأمر نفسه على Linux وWindows ويرفع <code dir="ltr">BenchmarkDotNet.Artifacts</code>؛ وهو استشاري ولا يمنع دمج أي طلب.

قيس على Windows 11، معالج Intel Core i7-8850H (2.60 GHz، 6 أنوية فعلية/12 منطقية)، .NET SDK 10.0.404، .NET 10.0.15، X64 RyuJIT، BenchmarkDotNet 0.15.8، 3 تسخينات + 5 تكرارات. تعامل مع الأرقام كرتبة حجم لا كمقارنة بين أجهزة:

| السيناريو | المتوسط | المخصَّص |
|---|---:|---:|
| <code dir="ltr">Be</code> (int) نجاح / فشل | 4.3 ns / 2.8 μs | 32 B / 664 B |
| <code dir="ltr">Be</code> (string) نجاح / فشل | 5.0 ns / 3.2 μs | 32 B / 1,160 B |
| <code dir="ltr">BeTrue</code> نجاح / فشل | 4.0 ns / 2.5 μs | 32 B / 592 B |
| <code dir="ltr">Contain</code> (string) نجاح | 14.7 ns | 32 B |
| <code dir="ltr">Contain</code> (collection) نجاح | 10.9 ns | 32 B |
| <code dir="ltr">HaveCount</code> نجاح | 24.4 ns | 104 B |
| <code dir="ltr">Throw</code> نجاح | 2.4 μs | 416 B |
| <code dir="ltr">AssertionScope</code> نجاح | 116–120 ns | 488 B |
| <code dir="ltr">AssertionScope</code> مع 1 / 10 إخفاقات | 522 ns / 3.4 μs | 1,056 B / 5,496 B |
| <code dir="ltr">BeEquivalentTo</code> رسم صغير | 1.9 μs | 3.66 KB |
| <code dir="ltr">BeEquivalentTo</code> 500 كائنًا | 1.1 ms | 2.25 MB |
| <code dir="ltr">MatchSnapshot</code> + scrubber (IO ملفات) | 330 μs | 14.9 KB |

قيست مقارنة النظراء في الجلسة نفسها. كل خلية تعني **المتوسط / المخصَّص**:

| السيناريو | Jeninnet | AwesomeAssertions 9.6 | Shouldly 4.3 | NFluent 3.1 |
|---|---:|---:|---:|---:|
| <code dir="ltr">Be</code> (int) نجاح | **4.3 ns / 32 B** | 139.7 ns / 808 B | 60.5 ns / 304 B | 155.1 ns / 800 B |
| <code dir="ltr">Be</code> (int) فشل | **2.8 μs / 664 B** | 139.4 μs / 38.9 KB | 170.2 μs / 41.7 KB | 5.8 μs / 5.4 KB |
| <code dir="ltr">Be</code> (string) نجاح | **5.0 ns / 32 B** | 154.0 ns / 888 B | 719.0 ns / 992 B | 947.5 ns / 1,856 B |
| <code dir="ltr">Be</code> (string) فشل | **3.2 μs / 1.16 KB** | 175.5 μs / 42.2 KB | 185.6 μs / 67.0 KB | 7.4 μs / 8.6 KB |
| <code dir="ltr">BeTrue</code> نجاح | 4.0 ns / 32 B | 121.0 ns / 664 B | **0.9 ns / 0 B** | 89.9 ns / 568 B |
| <code dir="ltr">Contain</code> (string) نجاح | **14.7 ns / 32 B** | 114.3 ns / 624 B | 31.2 ns / 96 B | 359.5 ns / 1,112 B |
| <code dir="ltr">HaveCount</code> نجاح | **24.4 ns / 104 B** | 186.9 ns / 1,048 B | 61.8 ns / 304 B | 262.4 ns / 896 B |
| <code dir="ltr">AssertionScope</code> نجاح | **116 ns / 488 B** | 360 ns / 1,480 B | 125 ns / 496 B¹ | 471 ns / 1,880 B² |
| <code dir="ltr">AssertionScope</code> فشل | **522 ns / 1.06 KB** | 128.5 μs / 44.9 KB | 515.2 μs / 136.2 KB | 6.4 μs / 5.4 KB² |

¹ لا يملك Shouldly نطاق تجميع؛ يستخدم الصف <code dir="ltr">ShouldSatisfyAllConditions</code> الذي ينفّذ الشرطين ويجمع الإخفاقات.
² لا يملك NFluent نطاق تجميع؛ يقيس الصف استدعاءَي <code dir="ltr">Check.That</code> متتاليين بفشل سريع، فليس مطابقًا دلاليًا مباشرًا.

في هذا التشغيل كان Jeninnet الأقل متوسطًا وتخصيصًا في معظم السيناريوهات. الاستثناء هو <code dir="ltr">ShouldBeTrue</code> لدى Shouldly (0.9 ns بلا تخصيص) مقابل 4.0 ns لدينا. الفشل يهيمن عليه تنسيق الرسالة في كل المكتبات؛ والأرقام رتبة حجم لا حكم بين أجهزة. صفوف <code dir="ltr">Throw</code> والتكافؤ واللقطات من تشغيل المحرك السابق؛ جدول النظراء من تشغيل 2026-11-15.

زمن اللقطة مذكور سطرًا منفصلًا عمدًا: فهي تنفّذ IO ملفات وليست فعلًا ساخنًا.

ويُتحقق من مسار الإصدار دون نشر:

<div dir="ltr">

```powershell
dotnet pack src/Jeninnet.Assertions/Jeninnet.Assertions.csproj -c Release --output artifacts/packages
pwsh scripts/release/verify-package.ps1
```

</div>

يفحص السكربت تجميعات <code dir="ltr">lib/&lt;tfm&gt;</code> الأربعة مع وثائق XML، وREADME/الأيقونة، وترخيص MIT وبصمة commit في nuspec، وغياب اعتماديات أطر الاختبار، ووجود PDBs في <code dir="ltr">.snupkg</code> المرافق، وعدم تسرّب أي ملفات مصدر أو اختبار أو private إلى أي من الأرشيفين. ويسرد <code dir="ltr">docs/release-checklist.md</code> تسلسل ما قبل الإصدار الكامل؛ وتشغّل مهمة CI <code dir="ltr">package-validation</code> التحقق نفسه.

## ٩.٨ خلاصة وتمرين

**الخلاصة:**

- **داخل <code dir="ltr">AssertionScope</code> تُجمَع الإخفاقات المستقلة**، وخارجه يُرمى أول فشل. استخدم <code dir="ltr">Discard()</code> للتحقُّق من التجميع دون فشل الاختبار.
- **الثقافة لكل تدفق** مع عزل تام (<code dir="ltr">AsyncLocal</code>). القيم تبقى <code dir="ltr">InvariantCulture</code> دائمًا، واللغات تُضاف عبر ملفات <code dir="ltr">.resx</code> فقط.
- **المنسِّقات تغيِّر العرض لا المقارنة.** سجِّلها عامًّا أو ضمن النطاق لعرض أنواع النطاق بوضوح.
- **ارفع الافتراضات إلى التكوين.** استخدم <code dir="ltr">AssertionConfiguration</code> والمهيئات على مستوى التجميع (<code dir="ltr">AssertionEngineInitializer</code>) بدل التكرار في كل اختبار.
- **احقن ساعة للتوكيدات الزمنية.** <code dir="ltr">AssertionClock.Current</code> (معزولة لكل تدفق) تجعل حدود النوافذ حتمية بلا نوم.
- **اقِس التخصيص لا زمن الجدار في الاختبارات.** مسار النجاح لا ينسّق شيئًا؛ والقياسات تنشر رتبة الحجم، والحزمة تُفحص قبل الإصدار.

**تمرين:**

1. ضع ثلاثة توكيدات مستقلة داخل <code dir="ltr">AssertionScope</code> وجعل اثنين منها يفشلان — شاهِد التقرير الشامل.
2. جرِّب عزل الثقافة: ضع <code dir="ltr">AssertionCulture.Current = new CultureInfo("ar")</code> داخل اختبار وشاهِد رسالة الفشل بالعربية (إن وُجدت الترجمة).
3. أنشئ منسِّقًا لنوع مخصَّص <code dir="ltr">Money</code> وسجِّله عامًّا. اجعل توكيدًا يفشل وشاهِد كيف يظهر النوع المُنسَّق.
4. حوِّل إعداد المنسِّق إلى <code dir="ltr">AssertionEngineInitializer</code> على مستوى التجميع.
5. لفّ اختبارك في <code dir="ltr">AssertionIsolation.Isolate()</code> وغيّر الثقافة والساعة داخله — وتأكد من عودتهما إلى وضعهما بعد الخروج حتى عند الفشل.

</div>

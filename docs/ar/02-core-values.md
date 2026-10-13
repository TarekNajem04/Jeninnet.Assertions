<div dir="rtl">

# الفصل ٢ — القيم الأساسية: <code dir="ltr">bool</code> و<code dir="ltr">object</code> والأرقام والنصوص

بعد أن تعرَّفنا على قراءة رسالة الفشل في الفصل الأول، ننتقل إلى التوكيدات الأكثر شيوعًا على القيم اليومية: القيم المنطقية، والكائنات، والأرقام، والنصوص. وهي القيم التي نكتب عليها أغلب اختباراتنا، ومنها تأتي أغلب رسائل الفشل التي نقرؤها لاحقًا. ولهذا يستحق كل نوع من هذه الأنواع وقفة قصيرة: أي توكيد يعبِّر عن العقد المقصود، وأي صياغة تُوصل الرسالة بأقل غموض.

هدفنا هنا بسيط: كتابة توكيد يُفهم كجملة طبيعية، ويُخرج رسالة فشل واضحة حين يخفق.

## ٢.١ القيم المنطقية: <code dir="ltr">BeTrue()</code> و<code dir="ltr">BeFalse()</code>

نبدأ بالشرط الأوضح:

<div dir="ltr">

```csharp
isReady.Should().BeTrue();
hasErrors.Should().BeFalse();
```

</div>

عند الفشل:

<div dir="ltr">

```text
Expected isReady to be true, but found false.
```

</div>

قد يبدو هذا تكرارًا لـ<code dir="ltr">Be(true)</code>، لكن الفرق يظهر في الرسالة والقصد: <code dir="ltr">BeTrue()</code> صيغة تُعلن أن الفحص عن شرط منطقي بعينه، فيقرؤها من يمر على الكود كسؤال «هل هذا جاهز؟» لا كسؤال مقارنة بين قيمتين. وحين تفشل، تقول الرسالة <code dir="ltr">to be true</code> مباشرة بدل تلخيص قيمة مبهمة. ولمن يُنشئ رسائل مخصَّصة أو يترجمها، يبقى القالب أنظف حين يكون الفعل الفعلي هو المقصود لا الشكل العام للمقارنة.

بالنسبة إلى <code dir="ltr">bool?</code>، ينطبق الأمر ذاته. إذا كانت القيمة <code dir="ltr">null</code>، فإن <code dir="ltr">BeTrue()</code> أو <code dir="ltr">BeFalse()</code> ستفشل — فلا تقبل <code dir="ltr">null</code> صوابًا ولا خطأً. وهذا امتداد طبيعي لقاعدة الفصل الأول: النوع القابل لـ<code dir="ltr">null</code> لا يُطلب منه إثبات قيمة قبل أن يُبرهن وجودها، فإن أردت قبول الغياب كحالة مشروعة، أضِف <code dir="ltr">BeNull()</code> في المسار المناسب صراحةً.

## ٢.٢ الكائنات: <code dir="ltr">Be()</code> و<code dir="ltr">BeSameAs()</code> و<code dir="ltr">BeOfType()</code>

حين نُقارن بقيمة متوقَّعة، نستخدم <code dir="ltr">Be()</code>:

<div dir="ltr">

```csharp
name.Should().Be("alice");
id.Should().Be(42);
```

</div>

يستخدم <code dir="ltr">Be()</code> <code dir="ltr">EqualityComparer&lt;T&gt;.Default</code>، أي أنه يُقارن **بالقيمة**، لا بالهوية المرجعية. وهي المقارنة التي تريدها في أغلب الحالات: كائن <code dir="ltr">User</code> منقول من قاعدة بيانات لا يكون نفس مثيل الذاكرة، لكن قيمته هي القيمة.

أما حين نريد التحقُّق من **نفس المثيل بالضبط**، فنستخدم <code dir="ltr">BeSameAs()</code>:

<div dir="ltr">

```csharp
var x = new User("alice");
var y = x;

y.Should().BeSameAs(x); // نفس المرجع
```

</div>

لو كان لدينا <code dir="ltr">var z = new User("alice")</code> بقيم متطابقة، فـ<code dir="ltr">BeSameAs(z)</code> سيفشل رغم تكافؤ القيم. هذا الفرق مهم جدًا لرسالة الفشل، لأن الخطأ الشائع في الفرق بين «نسخة من نفس البيانات» و«نفس النسخة» يظهر مرتين بمعنى مختلف تمامًا: مرة في كاش لا يريد أن يعيد بناء الكائن، ومرة في اختبار يريد أن يثبت أن البيانات نُسخت لا أنها أُشير إليها. ولا تُغني <code dir="ltr">Be()</code> هنا: هي تنجح في الحالتين، فيمر الاختبار الذي يقصد الأولى على سلوك الثانية.

حيث تريد صرامة أكثر من القيمة وأقل من المثيل نفسه، تعمل المقارنة المخصَّصة (<code dir="ltr">custom comparer</code>). حين لا تُناسبنا المساواة الافتراضية، نُمرِّر مقارنة مخصَّصة، ويبقى الكائن على نوعه والمقارنة هي من يحدِّد معنى «التساوي»:

<div dir="ltr">

```csharp
object name = "Alice";
name.Should().Be("ALICE", StringComparer.OrdinalIgnoreCase);
name.Should().NotBe("Bob", StringComparer.OrdinalIgnoreCase);
name.Should().BeOneOf(["alice", "bob"], StringComparer.OrdinalIgnoreCase);
```

</div>

تمرير <code dir="ltr">null</code> يُبقي <code dir="ltr">EqualityComparer&lt;T&gt;.Default</code>. أما <code dir="ltr">IEqualityComparer</code> غير العامة فيرفضها المترجم لأنها لا تقارن الكائن بأمان — وهي صرامة مقصودة: بدل أن نقبل مقارنة قد تُحوِّل الأنواع بصمت، نُبقي سطح API نظيفًا بلا مسار سريع للخطأ.

وللتأكُّد من النوع في وقت التشغيل:

<div dir="ltr">

```csharp
result.Should().BeOfType<Success>();
result.Should().NotBeOfType<Failure>();
```

</div>

أحيانًا نحتاج إلى التحقُّق من أن الكائن يُمكن تعيينه إلى نوع أساسي، فنتجه إلى <code dir="ltr">BeAssignableTo&lt;T&gt;()</code> بدلًا من ذلك. الفرق بين الفعلين هو نفسه الفرق بين <code dir="ltr">Throw</code> و<code dir="ltr">ThrowExactly</code> الذي سيأتي في فصل الأفعال: أحدهما يطلب النوع بالضبط، والآخر يقبل ما يشتق منه، والقاعدة أن تُختار الصيغة الأضيق التي تعبِّر عن العقد.

## ٢.٣ الأرقام والأنواع القابلة لأن تكون <code dir="ltr">null</code>

للأنواع القابلة لـ<code dir="ltr">null</code> مثل <code dir="ltr">int?</code>، يظهر صف توكيدات يجمع بين الوجود والقيمة:

<div dir="ltr">

```csharp
age.Should().NotBeNull().And.BeGreaterThan(18);
discount.Should().BeInRange(0, 10);
```

</div>

ترتيب الفحص هنا مقصود: نثبت الوجود أولًا ثم القيمة، فيقرؤها القارئ كجملة واحدة واضحة الترتيب. ومن التوكيدات الشائعة <code dir="ltr">BeGreaterThan()</code> و<code dir="ltr">BeLessThan()</code>، وصيغتاهما غير الحادَّتين <code dir="ltr">BeGreaterOrEqualTo()</code> و<code dir="ltr">BeLessOrEqualTo()</code>، والنطاق الشامل للطرفين <code dir="ltr">BeInRange(min, max)</code>، والإشارة <code dir="ltr">BePositive()</code> و<code dir="ltr">BeNegative()</code>، والقرب <code dir="ltr">BeApproximately(precision)</code> للقيم العشرية أو الفاصلة العائمة، والعضوية <code dir="ltr">BeOneOf(...)</code> للتحقُّق من انتماء القيمة إلى مجموعة محدَّدة.

<div dir="ltr">

```csharp
price.Should().BeApproximately(9.99m, precision: 0.01m);
score.Should().BeOneOf(0, 1, 2, 3);
```

</div>

ملاحظة عملية: <code dir="ltr">BeApproximately()</code> مناسب للقياسات أو الحسابات التقريبية، أما الأموال فالأفضل التحقُّق منها بنطاق واضح <code dir="ltr">BeInRange()</code> لتفادي الغموض. المنطق أن السؤال «هل السعر صحيح؟» سؤال نطاق لا سؤال تقريب: الخطأ المسموح في القياس فيزيائي بطبيعته، أما في المال فقرار تجاري يجب أن يُكتب بحدوده. وحين يظهر <code dir="ltr">BeApproximately</code> على حساب مالي، فهي إشارة إلى أن العقد لم يُصَغ بعد، لا إلى أن الاختبار متقدِّم.

## ٢.٤ النصوص: مساواة، واحتواء، ونمط، وطول

نُطوِّر فحص النص خطوةً بخطوة حتى نحصل على ما نريد بالضبط:

<div dir="ltr">

```csharp
name.Should().Be("ada");
name.Should().HaveLength(3);
code.Should().StartWith("USR");
code.Should().EndWith("01");
email.Should().Contain("@");
email.Should().MatchRegex(@"^[^@]+@[^@]+\.[^@]+$");
```

</div>

يمكن ربطها جميعًا:

<div dir="ltr">

```csharp
email.Should().Contain("@").And.MatchRegex(@"^[^@]+@[^@]+\.[^@]+$");
```

</div>

السلم هنا مقصود: كل ترقية صعودًا تضيف قيدًا. <code dir="ltr">Be</code> هو الأصل، و<code dir="ltr">StartWith</code>/<code dir="ltr">EndWith</code> يعبِّران عن جزء من الشكل، و<code dir="ltr">Contain</code> عن الاحتواء، و<code dir="ltr">MatchRegex</code> عن قاعدة كاملة. القاعدة المهنية أن تختار درجة القيد التي يعبِّر عنها العقد وتقف عندها؛ فحين يكون المطلوب أن البريد يحتوي <code dir="ltr">@</code> لا أن يطابق تعبيرًا كاملًا، فإن <code dir="ltr">MatchRegex</code> يُدخل إلى الاختبار تفاصيل ليست من العقد، ويكسر نفسه مع أي تعديل في صيغة البريد.

للتحقُّق من حالة الأحرف أو الفراغ:

<div dir="ltr">

```csharp
code.Should().BeUpperCased();
name.Should().NotBeNullOrWhiteSpace();
comment.Should().BeNullOrWhiteSpace();
```

</div>

عندما يفشل توكيد نص طويل، تُظهر المكتبة فرقًا موحَّدًا (<code dir="ltr">unified diff</code>) بين ما كان متوقَّعًا وما وُجد، بدلًا من لصق السلسلتين كاملتين بلا تمييز. قيمة هذا لا تظهر في نصوص قصيرة، بل حيث تقارن حمولة تحتوي على مئات الأحرف: يقرؤك الفرق مباشرة إلى الحرف الذي اختلف، لا إلى السلسلتين.

يُفحَص النص متعدد الأسطر سطرًا سطرًا — والمطابقة للسطر الكامل أبدًا، لا جزئية:

<div dir="ltr">

```csharp
report.Should().ContainLine("total: 42").Which.Should().Be("total: 42");
report.Should().NotContainLine("total: 43");
report.Should().HaveLineCount(3).And.NotHaveLineCount(4);
```

</div>

تُقسَّم الأسطر على <code dir="ltr">\n</code>: <code dir="ltr">"a\nb"</code> سطران، و<code dir="ltr">"a\nb\n"</code> ثلاثة أسطر، و<code dir="ltr">""</code> صفر. اللافت هنا أن سطرًا فارغًا في النهاية يُحتسب سطرًا، وهذا مقصود: كثير من أدوات توليد التقارير تُنهي مخرجاتها بسطر جديد، فلو تجاهلنا السطر الأخير لتساوت الحالتان، وضاعت علامة على أن المخرج تغيَّر. حين يظهر <code dir="ltr">HaveLineCount(4)</code> بدل <code dir="ltr">HaveLineCount(3)</code> فالمعنى عادةً أن المخرج اكتسب سطرًا جديدًا — إما فراغ زائد أو سطر بيانات أُضيف.

وعائلة <code dir="ltr">*EquivalentOf</code> تقارن متجاهلة حالة الأحرف افتراضيًا؛ ويقبل كل فعل خيارات للمسافات ونمط السطر (وضبط <code dir="ltr">null</code> يعني الافتراضي):

<div dir="ltr">

```csharp
name.Should().ContainEquivalentOf("ada", static o => o.IgnoringLeadingWhitespace());
name.Should().StartWithEquivalentOf("a", static o => o.IgnoringNewlineStyle());
name.Should().MatchEquivalentOf("a*", static o => o.IgnoringTrailingWhitespace());
```

</div>

هذه العائلة مكانها الطبيعي حيث يقارن الاختبار نصًا كتبه إنسان أو نسّقه مُحرِّر: بريد إلكتروني، أو ترويسة HTTP، أو مخرَج CLI. أما حيث يكون النص عقدًا بايتيًا — توقيع أو بصمة — فلا مكان للتسامح، و<code dir="ltr">Be</code> الحدث حينئذ.

### مجموعات Regex

يُوكَّد كائن <code dir="ltr">Match</code> مباشرة — النجاح، ونص المطابقة كاملًا، والمجموعات المسمّاة أو المرقّمة، وعدد المجموعات (المجموعة 0 هي المطابقة كاملة؛ يتطلب net6.0+):

<div dir="ltr">

```csharp
var logLine = Regex.Match("2026-01-02 ERROR disk full", @"(?<date>\d{4}-\d{2}-\d{2}) (?<level>\w+)");
logLine.Should().Succeed();
logLine.Should().HaveGroup("date", "2026-01-02");
logLine.Should().HaveGroup("level", "ERROR");
logLine.Should().HaveGroupCount(3);
```

</div>

المطابقة الفاشلة أو <code dir="ltr">null</code> تفشل هذه الأفعال برسالة مخصَّصة، والمجموعة التي لم تشارك (مجموعة اختيارية غير مطابَقة) تُحتسب نصًا فارغًا، والاسم غير الموجود يسرد أسماء المجموعات المتاحة. هذه التفاصيل الثلاثة تحسم أكثر ما يُربك عند فحص السجلات: لا سؤال «هل طابق النمط؟» وحده، بل أي مجموعة التقطت أي قيمة. فحين تتوقَّع <code dir="ltr">date</code> وتحصل على نص فارغ، فذلك يعني أن الجزء الاختياري لم يُطابَق أصلًا، وأن التعبير النمطي نفسه يحتاج مراجعة، لا أن النظام أعاد قيمة خاطئة.

## ٢.٥ التحقُّق من عدة شروط مستقلة

أحيانًا يلزم أن تُرضي قيمة واحدة عدة قواعد مستقلة. بدلًا من التوقُّف عند أول فشل، يقرأ <code dir="ltr">SatisfyAllConditions()</code> كل الإخفاقات دفعة واحدة:

<div dir="ltr">

```csharp
order.SatisfyAllConditions(
    static o => o.Total.Should().BePositive(),
    static o => o.Lines.Should().NotBeEmpty(),
    static o => o.Customer.Should().NotBeNull());
```

</div>

مخرَج الفشل:

<div dir="ltr">

```text
Expected value to satisfy all conditions, but the following failed:
1. Expected o.Total to be positive, but found -5.
2. Expected o.Lines not to be empty, but it was empty.
```

</div>

مفيد خصوصًا عند تشخيص شروط مسبقة معقَّدة في مسار واحد: بدل أن تُصلح الشرط الأول لتكتشف الثاني ثم الثالث، ترى الشروط الثلاثة في تقرير واحد مرقَّم بالترتيب. ويجدر التمييز بينه وبين <code dir="ltr">AssertionScope</code> الذي سنراه لاحقًا: هنا الشروط تُكتب في استدعاء واحد على قيمة واحدة، وهناك نطاق مستقل يجمع ما يُكتب داخله على قيم مختلفة. وأيًّا كان الأسلوب، احصر الاستخدام في الفحوص **المستقلة** — أما الفحوص التابعة فيجب أن تبقى متسلسلة حتى لا يتحوَّل التقرير إلى سلسلة أصداء لفشل واحد.

## ٢.٦ خلاصة وتمرين

**الخلاصة:**

- <code dir="ltr">BeTrue()</code> و<code dir="ltr">BeFalse()</code> للشرط المنطقي. القيم <code dir="ltr">null</code> تفشل كليهما.
- <code dir="ltr">Be()</code> يقارن بالقيمة، <code dir="ltr">BeSameAs()</code> يقارن بالهوية المرجعية. <code dir="ltr">BeOfType()</code> يضمن النوع الفعلي.
- للأنواع القابلة لـ<code dir="ltr">null</code> نبدأ بـ<code dir="ltr">BeNull()</code>/<code dir="ltr">NotBeNull()</code> قبل توكيدات القيمة.
- للنصوص نستخدم سلمًا واضحًا: المساواة، والطول، وبداية/نهاية، واحتواء، ومطابقة نمط.
- <code dir="ltr">SatisfyAllConditions()</code> يجمع كل الإخفاقات المستقلة بدل التوقُّف عند الأول.

**تمرين:**

1. أنشئ متغيِّرات من أنواع <code dir="ltr">bool</code> و<code dir="ltr">string</code> و<code dir="ltr">int?</code>.
2. اكتب توكيدات تغطِّي حالات صواب وخطأ وقيمة و<code dir="ltr">null</code>.
3. اربط توكيدين على الأقل باستخدام <code dir="ltr">.And</code> مع إضافة <code dir="ltr">because</code> موجزة.
4. اجعل أحد التوكيدات يفشل وشاهِد رسالة الفشل: هل هي واضحة بذاتها؟

</div>

<div dir="rtl">

# الفصل ٦ — الشكل لا الهوية: التكافؤ واللقطات

يحدث كثيرًا أن تمرَّ قيمة عبر سلسلة تحويلات: كيان قاعدة بيانات ← نموذج النطاق ← <code dir="ltr">DTO</code> للاستجابة. الأنواع مختلفة، والمراجع مختلفة، لكن **البيانات نفسها**. لو استخدمنا <code dir="ltr">Be()</code> فسيقارن الهوية أو النوع وسيُفشل رغم صحة البيانات.

هنا ننتقل من "متساويان" إلى "متكافئان في البنية". نقارن **الشكل** لا الهوية، ونستبعد ما يختلف بطبيعته (مثل معرفات قاعدة البيانات أو الطوابع الزمنية)، ثم نُثبِّت المخرجات الكبيرة **بلقطات** بدلًا من كتابة عشرات التوكيدات.

وقوة هذا الانتقال خطرُه في الوقت نفسه. المقارنة البنيوية قادرة على أن تمرّ بفرق حقيقي إن أفرطت في التسامح — استبعاد حقل لمجرد الخوف من فشله، أو تخطي نوع لأن التحويل «يعمل» — فتصبح الأداة التي وُجدت لكشف الانحراف ساترًا له. ولهذا كان انضباط هذا الفصل كله في سؤالين لا ثالث لهما: أي الأعضاء تُقارَن فعلًا، وكيف تُقارَن؛ وكل خيار فيه يجيب عن أحدهما صراحةً.

## ٦.١ <code dir="ltr">BeEquivalentTo()</code>: مقارنة بالبنية

يُتيح لنا <code dir="ltr">BeEquivalentTo()</code> مقارنة كائنين بناءً على أعضائهما، بغض النظر عن النوع أو المرجع:

<div dir="ltr">

```csharp
order.Should().BeEquivalentTo(expected);
order.Should().BeEquivalentTo(expected, options => options.Excluding(x => x.Id));
order.Should().BeEquivalentTo(expected, options => options.Excluding(x => x.Address.City));
order.Should().BeEquivalentTo(expected, options =>
    options.Excluding(x => x.Address).Including(x => x.Address.City));
```

</div>

القاعدة العملية: **استبعد ما يختلف بطبيعته، وليس ما يُخشى أن يفشل**. أمثلة شائعة: <code dir="ltr">Id</code>، و<code dir="ltr">CreatedAt</code>، و<code dir="ltr">UpdatedAt</code>، وأرقام التسلسل. وما تبقَّى يُقارَن بصرامة.

واعلم أن صرامة الافتراضي ليست تفصيلًا خفيًا في التنفيذ، بل تعريف الأداة نفسها: كل عضو لم تستبعده ولم تخففه يُقارن ويُفشل عند أول فرق. فكل سطر استبعاد تكتبه إنما يسجّل قرارًا هندسيًا بأن هذا الفرق غير ذي دلالة، ومن يقرأ الخيارات بعدك يقرأ هذه القرارات مجمَّعة.

يتضمن تكوين المقارنة خيارات مفيدة:

- <code dir="ltr">Excluding()</code> / <code dir="ltr">Including()</code> لمسارات الأعضاء (تدعم التداخل).
- <code dir="ltr">WithStrictOrdering()</code> / <code dir="ltr">WithoutStrictOrdering()</code> للتحكُّم في ترتيب المجموعات الفرعية.
- <code dir="ltr">ComparingByValue&lt;T&gt;()</code> لأنواع تُقصد مقارنتها بقيمتها الدلالية.
- <code dir="ltr">AllowingInfiniteRecursion()</code> للمخططات الحلقية النادرة (التعامل الافتراضي آمن ويمنع الحلقات غير المنتهية).

يمكن أيضًا تعيين افتراضات عامة عبر <code dir="ltr">AssertionConfiguration.Equivalency</code> (مثل <code dir="ltr">StrictOrderingDefault</code>) لتوحيد السياسة في المشروع.

ملاحظة: تمرير تعبير ليس مسار عضو مثل <code dir="ltr">x =&gt; 42</code> سيُطلق <code dir="ltr">ArgumentException</code>. الرسالة واضحة، لكن يجدر الانتباه له.

### ٦.١.١ اختيار الأعضاء المشاركة في المقارنة

<div dir="ltr">

```csharp
// بالاسم، في أي موضع من الرسم البياني
dto.Should().BeEquivalentTo(entity, o => o.ExcludingMembersNamed("RowVersion", "ETag"));
// بنوع قيمة العضو (تشمل الأنواع المشتقة، وتعمل الأنواع العامة المفتوحة)
dto.Should().BeEquivalentTo(entity, o => o.Excluding<Money>().Excluding(typeof(ImmutableArray<>)));
// بمسند على عضو الانعكاس نفسه
dto.Should().BeEquivalentTo(entity, o => o.Excluding((MemberInfo m) => m.Name.StartsWith("Internal")));
dto.Should().BeEquivalentTo(entity, o => o.Including((MemberInfo m) => m.Name is "Id" or "Total"));
```

</div>

| الخيار | الأثر |
| --- | --- |
| <code dir="ltr">IncludingProperties()</code> / <code dir="ltr">IncludingFields()</code> | مقارنة الخصائص العامة / الحقول العامة فقط |
| <code dir="ltr">IncludingInternalProperties()</code> / <code dir="ltr">IncludingInternalFields()</code> | مقارنة الخصائص / الحقول غير العامة أيضًا |
| <code dir="ltr">ExcludingProperties()</code> / <code dir="ltr">ExcludingFields()</code> | إيقاف مقارنة الخصائص / الحقول |
| <code dir="ltr">IncludingAllDeclaredProperties()</code> | الخصائص المُعلَنة على النوع وقت التشغيل فقط (تُتجاهل الموروثة) |
| <code dir="ltr">IncludingAllRuntimeProperties()</code> | الخصائص الظاهرة على النوع وقت التشغيل، بما فيها الموروثة |
| <code dir="ltr">ExcludingExplicitlyImplementedProperties()</code> | تخطّي تنفيذات الواجهات الصريحة (يكشفها وقت التشغيل كأعضاء غير عامة بأسماء منقّطة) |
| <code dir="ltr">ExcludingNonBrowsableMembers()</code> | تخطّي أعضاء المتوقَّع المعلَّمين بـ <code dir="ltr">[EditorBrowsable(Never)]</code> |
| <code dir="ltr">IgnoringNonBrowsableMembersOnSubject()</code> | اعتبار عضو الموضوع غير القابل للتصفّح غير موجود |
| <code dir="ltr">ExcludingMissingMembers()</code> | تجاهل أعضاء المتوقَّع غير المعلَنة في الموضوع |
| <code dir="ltr">ThrowingOnMissingMembers()</code> | إعادة اشتراطها (الافتراضي) |

**الأولوية: الاستدعاء الأخير هو الفائز.** فـ<code dir="ltr">IncludingProperties().IncludingFields()</code> يقارن الحقول فقط، والعكس يقارن الخصائص فقط. أما قواعد الاختيار (<code dir="ltr">Excluding</code>/<code dir="ltr">Including</code> بالمسار أو الاسم أو النوع أو المسند) **تتراكم** — لذا <code dir="ltr">IncludingAllDeclaredProperties()</code> و<code dir="ltr">IncludingAllRuntimeProperties()</code> **تمسحها** لاستعادة اختيار قائم على الظهور فقط. وقواعد المطابقة (Mapping) تتراكم بمعزل عن قواعد الاختيار.

وتفسير هذا التفريق أن النوعين من القواعد يجيبان عن سؤالين مختلفين: قواعد الظهور تقول *صنفًا* أي الأعضاء يُقارَن، فمن الطبيعي أن يحل الأخير محل الأول؛ وقواعد التصفية تقول *استثناءات* محدَّدة، فمن الطبيعي أن تتجمع. ولو عُومِلتا بالمبدأ ذاته لصار في كل ترتيب استدعاءات مفاجأة، ولهذا كُتبت القاعدة هنا صراحةً قبل أن يكتشفها أحد بالمصادفة.

### ٦.١.٢ إعادة تسمية الأعضاء عبر <code dir="ltr">WithMapping</code>

قد يسمّي المتوقَّع والموضوع الشيء نفسه باسمين مختلفين:

<div dir="ltr">

```csharp
dto.Should().BeEquivalentTo(entity, o => o.WithMapping("Total", "Amount"));   // بمسار
dto.Should().BeEquivalentTo(entity, o => o.WithMapping("Address.City", "Address.Town"));
dto.Should().BeEquivalentTo(entity, o => o.WithMapping(e => e.Id, s => s.OrderId));   // بتعبير
// مقصور على زوج من الأنواع المتداخلة، أينما ظهر في الرسم البياني
dto.Should().BeEquivalentTo(entity, o => o.WithMapping<MoneyDto, Money>(o => o.Amount, m => m.Value));
```

</div>

تتجاهل قواعد المطابقة مقاطع الفهارس داخل المجموعات، فقاعدة واحدة تغطّي كل عنصر. والاسمان مستقلان — لا يُفرض تطابق حرفي بينهما.

### ٦.١.٣ المجموعات المتداخلة عبر <code dir="ltr">For(...)</code>

يختار <code dir="ltr">For(...)</code> مجموعة ويُعيد واجهة بناء، فتبقى قواعد العناصر المتداخلة عميقة في تعبير واحد متسلسل:

<div dir="ltr">

```csharp
order.Should().BeEquivalentTo(expected, o => o
    .For(x => x.Lines)
    .Exclude(x => x.UnitPrice));                    // عضو واحد من كل سطر

order.Should().BeEquivalentTo(expected, o => o
    .For(x => x.Lines)
    .For(x => x.Options)
    .Exclude(x => x.Code));                         // تداخل في مجموعة أعمق

order.Should().BeEquivalentTo(expected, o => o
    .For(x => x.Lines)
    .ExcludeAll());                                 // المجموعة وكل ما تحتها
```

</div>

تنطبق القاعدة على المسار المختار فقط: استبعاد <code dir="ltr">Lines.UnitPrice</code> لا يمسّ <code dir="ltr">LinesDiscount</code> الشقيق، ولا يتسرّب إلى عضو بالاسم نفسه في مستوى آخر.

### ٦.١.٤ خيارات المعنى: الصرامة والتحويل والشكل

الاختيار يحدِّد *أي* الأعضاء تُقارَن؛ وخيارات المعنى تحدد *كيف*. آخر استدعاء يفوز، مع أولوية موثقة واحدة: **الصرامة النوعية تغلب التحويل التلقائي** على العضو نفسه بأي ترتيب للاستدعاء.

<div dir="ltr">

```csharp
// أعضاء int مقابل long تفشل افتراضيًا؛ نختار التحويل...
actual.Should().BeEquivalentTo(expected, o => o.WithAutoConversion());
// ...أو نشترط تطابق الأنواع الفعلية تمامًا (بلا تحايل نوعي).
actual.Should().BeEquivalentTo(expected, o => o.WithStrictTyping());
// تقييد أي من القاعدتين على حافة عضو واحد:
actual.Should().BeEquivalentTo(expected, o => o.WithAutoConversionFor(m => m.Name == "Total"));
```

</div>

الأزواج المرتبطة: <code dir="ltr">WithoutStrictTyping[For]</code> / <code dir="ltr">WithoutAutoConversion[For]</code> تعيد إطفاء القواعد؛ <code dir="ltr">PreferringDeclaredMemberTypes()</code> يقارن جذر الرسم بالنوع المعلن وقت الترجمة (<code dir="ltr">PreferringRuntimeMemberTypes()</code> / <code dir="ltr">PreferringDefaultMemberTypes()</code> يعيدان الافتراضي)؛ <code dir="ltr">ComparingEnumsByValue()</code> يطابق الأرقام المتساوية عبر أنواع التعداد بينما <code dir="ltr">ComparingEnumsByName()</code> يطابق المعرفات؛ <code dir="ltr">ComparingByMembers&lt;T&gt;()</code> يفرض المقارنة بالأعضاء على الأنواع ذات المقارنة القيمية؛ <code dir="ltr">ComparingRecordsByValue()</code> / <code dir="ltr">ComparingRecordsByMembers()</code> يبدّلان مقارنة السجلات؛ <code dir="ltr">ComparingNullStringsAsEmpty()</code> / <code dir="ltr">ComparingNullCollectionsAsEmpty()</code> يساويان الفارغ مع null؛ <code dir="ltr">IgnoringCyclicReferences()</code> يقطع الدورات مثل <code dir="ltr">AllowingInfiniteRecursion()</code> (آخر استدعاء يفوز)؛ <code dir="ltr">WithoutRecursing()</code> يقارن القيم المتداخلة بـ<code dir="ltr">Equals</code> (<code dir="ltr">IncludingNestedObjects()</code> يعيد العمق)؛ <code dir="ltr">WithStrictOrderingFor(x =&gt; x.Tags)</code> يقيّد الترتيب على مجموعة واحدة؛ <code dir="ltr">IgnoringJsonPropertyCasing()</code> يطابق مفاتيح القواميس النصية متجاهلاً الحالة. لا يوجد خيار إصلاح تلقائي عمدًا: إعادة كتابة الكائن داخل التوكيد ستعدّل بيانات المستخدم.

ووراء أولوية «الصرامة النوعية تغلب التحويل» سبب منطقي: التحويل يوسّع ما يُقبل، والصرامة تضيّق؛ وحين يتعارضان على العضو نفسه، فالأحوط أن يفوز التضييق، وإلا صار الاختيار الحاد خاضعًا لترتيب كتابة غير مقصود. أما تقييد القاعدة على حافة عضو واحد بـ<code dir="ltr">...For</code> فهو الترجمة العملية للفكرة ذاتها: التسامح يُمنَح لموضع يعرفه الكاتب، لا يُنشَر على الرسم كله احتياطًا.

### ٦.١.٥ توسيع المقارنة: الخطوات والخطط والقواعد

حين لا تكفي الخيارات، نضيف قاعدتنا الخاصة. خطوة <code dir="ltr">IEquivalencyStep</code> مخصَّصة ترى كل عقدة قبل المقارنة المدمجة:

<div dir="ltr">

```csharp
public sealed class MoneyStep : EquivalencyStep {
    public override bool Handles(Comparands comparands) => comparands.Expectation is Money;
    public override bool TryHandle(Comparands comparands, IObjectGraph graph, IEquivalencyStepContext context) {
        var subject = (Money?)comparands.Subject;
        var expectation = (Money)comparands.Expectation!;
        if(subject is null || subject.Amount != expectation.Amount) {
            context.Fail($"{comparands.FormattedPath} was {subject?.Amount} instead of {expectation.Amount}.");
        }
        return true; // امتلكنا العقدة: نتخطى المقارنة المدمجة هنا
    }
}

order.Should().BeEquivalentTo(expected, o => o.Using(new MoneyStep()));
```

</div>

دورة الحياة: <code dir="ltr">Handles</code> ثم <code dir="ltr">TryHandle</code> (إرجاع <code dir="ltr">false</code> يسقط إلى <code dir="ltr">AssertEquivalency</code> لخطوات نمط التحقُّق). تعيش الخطوات في <code dir="ltr">EquivalencyPlan</code> مع <code dir="ltr">Add</code> و<code dir="ltr">Insert</code> و<code dir="ltr">AddBefore</code>/<code dir="ltr">AddAfter</code> و<code dir="ltr">Remove</code> و<code dir="ltr">Reset</code> و<code dir="ltr">Clear</code>؛ تُسجَّل لكل توكيد عبر <code dir="ltr">Using(...)</code> أو عمومًا عبر <code dir="ltr">AssertionConfiguration.Equivalency.Plan</code>. وهناك خطافات أدق: <code dir="ltr">UsingSelectionRule</code> (<code dir="ltr">IMemberSelectionRule</code>) و<code dir="ltr">UsingMatchingRule</code> (<code dir="ltr">IMemberMatchingRule</code>) و<code dir="ltr">UsingOrderingRule</code> (<code dir="ltr">IOrderingRule</code>)، لكل منها مفتاح إطفاء <code dir="ltr">Without*Rules()</code>. ولفحوص القيم المفردة دون رسم، نستخدم <code dir="ltr">Value.ThatMatches(value, x =&gt; x &gt; 0)</code> أو <code dir="ltr">Value.ThatSatisfies(value, v =&gt; v.Should().BePositive())</code>.

وخيار «الخطوة» هو حدّ التوسعة الحقيقي في المحرِّك: كل ما لم تلتقطه الخيارات الجاهزة — مقارنة نطاق مالي، أو استخراج قيمة من صيغة خاصة — تجده في مكان واحد تراه كل عقدة قبل المقارنة المدمجة. وإرجاع <code dir="ltr">false</code> من <code dir="ltr">TryHandle</code> ليس فشلًا بل تفويضًا: يعني أن الخطوة فحصت العقدة ولم تمتلكها، فتمر للقاعدة التالية. بهذا يمكن بناء خطوات تحقّق خفيفة تشارك في الرسم دون أن تحجب أي مقارنة أخرى.

تشخيصان يعينان حين يبقى الرسم مختلفًا. <code dir="ltr">WithTracing(new StringBuilderTraceWriter())</code> يسجِّل كل عقدة وقرار خطوة؛ و<code dir="ltr">WithFullDump()</code> يُلحق بالرسالة شجرتي الكائنين مع ملخص <code dir="ltr">With configuration:</code>. التتبُّع والتفريغ نص تشخيصي ثابت لا يُترجَم. **تحذير أمني:** التفريغ يطبع كل قيم الأعضاء، فلا تفعِّله إلا لبيانات غير حساسة ولا تحفظ مخرجاته.

### ٦.١.٦ التفاوتات: انحراف الساعات والتقريب

قلّما تُعيد الأنظمة الحقيقية الطوابع الزمنية والقيم العشرية مطابقة تمامًا. خياران يمتصّان هذا الضجيج على مستوى الرسم كله: <code dir="ltr">WithDateTimeTolerance(TimeSpan)</code> يقارن كل <code dir="ltr">DateTime</code>/<code dir="ltr">DateTimeOffset</code> كلحظة ضمن نافذة معطاة، و<code dir="ltr">WithNumericTolerance(epsilon)</code> يفعل الشيء نفسه للقيم <code dir="ltr">float</code>/<code dir="ltr">double</code>/<code dir="ltr">decimal</code> بالفرق المطلق. أما الأعداد الصحيحة فلا تصبح تقريبية أبدًا:

<div dir="ltr">

```csharp
actual.Should().BeEquivalentTo(expected, o => o
    .WithDateTimeTolerance(TimeSpan.FromSeconds(2))
    .WithNumericTolerance(0.01));
```

</div>

ولتضييق النافذة أو توسيعها لعضو واحد استخدم صيغ <code dir="ltr">...For</code> (والقاعدة المقيَّدة تتجاوز العامة على مسارها):

<div dir="ltr">

```csharp
actual.Should().BeEquivalentTo(expected, o => o
    .WithDateTimeTolerance(TimeSpan.FromSeconds(1))
    .WithDateTimeToleranceFor(TimeSpan.FromMinutes(5), x => x.SessionExpiresAt));
```

</div>

والقيمة الخارجة عن تفاوتها تفشل بالفرق المعتاد مع ملاحظة تسمّي الفرق الفعلي والتفاوت المسموح (<code dir="ltr">The difference 00:00:05 exceeds the allowed tolerance 00:00:01.</code>). وتُشغَّل الخطوات المخصصة قبل فحص التفاوت، فيحتفظ <code dir="ltr">Using(...)</code> بالملكية حيث يعالج عقدة.

وثبات «الأعداد الصحيحة لا تصبح تقريبية أبدًا» قرار جوهري: العدّ والمعرّفات وكميات الأصناف قيم منفصلة بطبيعتها، وتسامحها بكسور يعني اختبارًا لا يستطيع أن يثبت حتى عدد عناصر. ولهذا يقتصر التقويم على ما هو متصل في الأصل — الأزمنة والعشرية — حيث المنطق نفسه يحتمل التقريب. وأما القاعدة المقيَّدة على مسار بعينها فتتجاوز العامة على مسارها لأن التخصيص الأضيق هو القرار الأحدث والأنسب للموضع، والنظام لا يجمع النافذتين ولا يخلط بينهما.

## ٦.٢ المجموعات والنصوص في التكافؤ

للمجموعات، السلوك الافتراضي في <code dir="ltr">BeEquivalentTo()</code> هو: **لا يُشترط الترتيب، ويُحتسَب التكرار**. هذا يتوافق مع أغلب حالات العمل (الفصل ٣):

<div dir="ltr">

```csharp
codes.Should().BeEquivalentTo(["A", "B"], options => options.IgnoringCase());
codes.Should().BeEquivalentTo([1, 2, 3], options => options.WithStrictOrdering());
```

</div>

عند المقارنة مع النصوص، تتوفَّر خيارات إضافية على مستوى الرسم كله:

<div dir="ltr">

```csharp
order.Should().BeEquivalentTo(expected, o => o.IgnoringCase().IgnoringNewlineStyle());
order.Should().BeEquivalentTo(expected, o => o.IgnoringLeadingWhitespace().IgnoringTrailingWhitespace());
```

</div>

وخيارات النصوص تتوزّع على مستويين: <code dir="ltr">IgnoringCase()</code> قرار حالة أحرف يُطبَّق على الرسم كله حيث ظهر نص، وتحته تبقى قرارات متخصصة أخرى. والقاعدة العامة في الاختيار بينهما أن كل تسامح يُكتب حيث يعنيه القرار: النص الذي يُقارَن من مصدر بشري يتحمّل حالة الأحرف، والنص الذي يُقارَن كعقد تقني — بصمة أو معرّف — لا يتحملها.

## ٦.٣ اللقطات: تثبيت المخرجات الكبيرة

عندما يكون الإخراج كبيرًا جدًا (فاتورة، استجابة JSON كاملة، تقرير طويل...)، يصبح وصف كل سطر توكيدًا عملاً مضنيًا وهشًا. أي تعديل طفيف في التنسيق سيُكسر عشرات التوكيدات. البديل الأذكى: **لقطة** (<code dir="ltr">Snapshot</code>).

نُراجع الناتج بصريًا **مرة واحدة**، ثم نثبِّته. كل تغيير لاحق سيُظهر فرقًا واضحًا ويطلب مراجعة واعية.

<div dir="ltr">

```csharp
invoiceText.Should().MatchSnapshot();                    // ملف invoice.verified.txt بجوار الاختبار
invoiceText.Should().MatchSnapshot("feb-07");            // اسم صريح للملف
invoiceText.Should().MatchSnapshot("id", directory, options =>
    options.Scrub(s => s.Replace("42", "<id>")));
```

</div>

### آلية العمل الصارمة

لا تُقبل اللقطة عنك أبدًا. فحين تغيب النسخة المعتمدة — أو يختلف المخرج المُنتَج عنها — يكتب التوكيد ملف <code dir="ltr">.received</code> ويفشل، عارضًا المحتوى المستلم أو فرقًا موحَّدًا. أما القبول فهو فعل تنفّذه أنت عن قصد: راجع الدليل، ثم أعد تسمية الملف إلى <code dir="ltr">.verified</code>. وعند التطابق التام يمرّ الاختبار ويُحذف أي <code dir="ltr">.received</code> قديم، فيبقى الزوج معبِّرًا عن قرار اتخذه إنسان لا عن آخر ما أنتجه الكود. والمحتوى المتطاير بين التشغيلات — معرفات <code dir="ltr">Guid</code> وأرقام الطلبات والطوابع الزمنية — يُنقَّى بقواعد <code dir="ltr">Scrub()</code> صريحة قبل تثبيت المرجع، وتُلتزم المراجع بنهايات أسطر <code dir="ltr">LF</code> ليبقى التطابق بايتًا ببايت على أي نظام. ويتوسّع [الفصل ١٢](12-snapshots.md) في التفاصيل التشغيلية، بما فيها المفتاح الوحيد لإعادة التأسيس دفعةً واحدة.

## ٦.٤ خلاصة وتمرين

**الخلاصة:**

- **الشكل لا الهوية.** استخدم <code dir="ltr">BeEquivalentTo()</code> لمقارنة كائنات من أنواع مختلفة ذات بيانات متطابقة.
- **استبعد ما يختلف بطبيعته.** ركِّز على ما يهم القاعدة، واستبعد <code dir="ltr">Id</code> والطوابع الزمنية وما شابهها.
- **المجموعات افتراضية بلا ترتيب.** جرِّد الأمر بصراحة بـ <code dir="ltr">WithStrictOrdering()</code> فقط حين يكون الترتيب جزءًا من العقد.
- **اللقطات لمخرجات كبيرة.** راجع مرة واحدة، ثبِّت، ثم دع الفرق يُظهر التغييرات المستقبلية بوضوح.

**تمرين:**

1. خذ كيانًا و<code dir="ltr">DTO</code> له نفس القيم، وقارنهما بـ <code dir="ltr">BeEquivalentTo()</code> مستبعدًا حقلًا واحدًا على الأقل.
2. أنشئ لقطة لنص طويل (أو JSON مصغَّر). شغِّل الاختبار أول مرة، راجع <code dir="ltr">.received</code>، ثم قبِّلها بتحويلها إلى <code dir="ltr">.verified</code>.
3. عدِّل جزءًا طفيفًا في النص وشغِّل الاختبار مرة أخرى — شاهِد الفرق الموحد يظهر بوضوح.

</div>

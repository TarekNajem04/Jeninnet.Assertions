<div dir="rtl">

# الفصل ٥ — الأفعال وغير المتزامن: الاستثناءات والانتظار والميزانيات

أحد أسرع الحلول الخاطئة في الاختبارات هو <code dir="ltr">Thread.Sleep(2000)</code>. نستخدمه حين لا يظهر النتيجة فورًا: رصيد يصل بعد بضع مئات من الميلي ثوانٍ، أو حدث يُطلق لاحقًا. المشكلة أن النوم **يُخمن الوقت**، لا **ينتظر الشرط**. يصبح الاختبار أبطأ بلا داعٍ، والفشل أقل وضوحًا، ولا يُخبرنا رسالة الفشل بما كان يحدث فعلاً.

خلف هذه العادة ثلاثة أسئلة يخلط بينها من يكتب الاختبار على عجل: كيف نفحص ما **يجب أن يفشل**، وكيف ننتظر ما **سيحدث لاحقًا**، وكيف نضبط ما **يجب أن يبقى سريعًا**؟ الأول يُعالج بمعاملة الاستثناء كقيمة تُفحص لا كحادث يُلتقط، والثاني بالانتظار للشرط لا للساعة، والثالث بعتبات تعثر سخية لا بحدود أداء ضيقة. وهذا الفصل يعالج الأسئلة الثلاثة بهذا الترتيب، مع فرع كامل لما يفترضه غير المتزامن من عقود الإلغاء والسلامة.

## ٥.١ الاستثناءات: النوع أولًا، ثم المحتوى

عندما يُفترض أن يرمي الأسلوب استثناءً، نُغلِّف الاستدعاء في مفوَّض، ثم نُستجوب ما رُمِي.

<div dir="ltr">

```csharp
Action act = () => cart.Checkout();
act.Should().Throw<InvalidOperationException>();
act.Should().ThrowExactly<InvalidOperationException>();
act.Should().NotThrow();
```

</div>

الفرق مهم: <code dir="ltr">Throw&lt;T&gt;()</code> تقبل الاستثناءات من النوع المشتق (<code dir="ltr">T</code> أو أي صنف يرثه). أما <code dir="ltr">ThrowExactly&lt;T&gt;()</code> فتشترط **نوع التشغيل الدقيق** فقط. نستخدم <code dir="ltr">ThrowExactly&lt;T&gt;()</code> حين يعتمد <code dir="ltr">catch</code> على النوع بالضبط. والسؤال الذي يحسم اختيار الصيغة: لو غيّر المنفِّذ لاحقًا رميًا من نوع مشتق، فهل يبقى السلوك صحيحًا من وجهة نظر العقد؟ إن نعم، فالأعم أصدق تعبيرًا؛ وإن كان المستهلك يفرِّق بين النوعين، فالصيغة الدقيقة هي التي تحمي ذلك التفريق من الانزلاق الصامت.

بعد التحقُّق من النوع، نتابع المحتوى عبر الكائن المُعاد:

<div dir="ltr">

```csharp
act.Should().Throw<ArgumentException>()
    .WithMessage("Quantity *")
    .WithoutMessage("*success*")
    .WithParameterName("qty")
    .WithInnerException<FormatException>()
    .WithInnerExceptionExactly(typeof(FormatException))
    .Where(static ex => ex.ParamName == "qty");
```

</div>

<code dir="ltr">WithMessage()</code> تدعم بدل <code dir="ltr">*</code>، ما يسمح لنا بالتركيز على جوهر الرسالة دون تقييد صياغتها حرفيًا. هذا يحافظ على الاختبار مرنًا دون أن يفقد قدرته على الكشف عن الخطأ. أما <code dir="ltr">WithoutMessage()</code> فتتحقَّق من العكس: يجب ألا تطابق الرسالة النمط. و<code dir="ltr">WithInnerException&lt;T&gt;()</code> تقبل الأنواع المشتقة، بينما تشترط <code dir="ltr">WithInnerExceptionExactly&lt;T&gt;()</code> نوع التشغيل الدقيق. والاستثناء الملتقَط متاح أيضًا عبر <code dir="ltr">.Which</code> لمواصلة التوكيدات. ودقة الصياغة هنا تتوزَّع طبقتين: طبقة تُثبت **نوع ما رمى**، وطبقة تُثبت **مضمونه** — ولا تغني إحداهما عن الأخرى، لأن استثناءً من النوع الصحيح برسالة فاسدة يظهر للمستخدم كعطب حقيقي، تمامًا كنوع خاطئ برسالة سليمة.

يمكن كتابة الاختبارات أحادية الجملة عبر <code dir="ltr">FluentActions</code>، بما فيها المتسلسلات المؤجلة:

<div dir="ltr">

```csharp
FluentActions.Invoking(() => cart.Checkout(null)).Should().Throw<ArgumentNullException>();
await FluentActions.Awaiting(() => gateway.ChargeAsync(order)).Should().ThrowAsync<PaymentException>();

Func<IEnumerable<int>> rows = () => report.Rows(account);
FluentActions.Enumerating(rows).Should().Throw<InvalidOperationException>();
```

</div>

يفرض <code dir="ltr">Enumerating()</code> التكرار الكامل حين لا يظهر الاستثناء إلا أثناء استهلاك متسلسلة <code dir="ltr">yield</code>. وهذه نقطة تُوقع كثيرين: الاستثناء في المُكرَّرات الكسولة لا يُرمى عند الاستدعاء، بل عند أول عنصر يُطلب؛ فلو فحص الاختبار الاستدعاء وحده لمرّ أخضر وهو لا يعرف بالعطب بعد. وقد تدعم المكررات المؤجلة التعداد مرة واحدة فقط، لذا استدعِ الإجراء المُعاد مرة واحدة. ويتوفَّر تحويل متحقق منه عبر <code dir="ltr">value.As&lt;string&gt;()</code>؛ فعند فشل التحويل نحصل على رسالة توكيد بدل <code dir="ltr">InvalidCastException</code>.

للمفوَّضات التي تُعيد قيمة <code dir="ltr">Func&lt;T&gt;</code>، نفس الواجهة متاحة — ويمكننا متابعة التوكيد حين لا يُرمى استثناء. أي أن الحالة الناجحة ليست فراغًا في الفحص، بل طريق يعود بقيمة تُواصَل التوكيدات عليها.

## ٥.٢ العمليات غير المتزامنة

لكل توكيد استثناء متزامن توأمه غير المتزامن:

<div dir="ltr">

```csharp
Func<Task> call = () => gateway.ChargeAsync(order);
await call.Should().ThrowAsync<PaymentException>();
await call.Should().ThrowExactlyAsync<PaymentException>();
await call.Should().NotThrowAsync();
```

</div>

قاعدة مهمة هنا: **الإلغاء هو نتيجة يجب تفسيرها، وليس نجاحًا يُتجاوز**. مهمة أُلغيَت لا "تنجح دون رمي استثناء"، بل تُنتج حالة إلغاء — لذا يفشل <code dir="ltr">NotThrowAsync()</code> عند الإلغاء بدلًا من النجاح. هذا يُعبِّر بوضوح عن السلوك المقصود، ويخالف ما قد يتوقعه من تعوَّد على اعتبار «لا استثناء» مرادفًا لـ«نجاح». في عمليات الدفع والشحن والقوائم انتظارٍ، الفرق بين «تمّت» و«أُلغيت» هو الفرق بين رصيد مسحوب ورصيد لم يُلمس، ولا يصح أن يضيع في مرونة زائدة.

حين يُفترض أن يرمي الإجراء قريبًا بدل أن ينجح eventually، نستخدم ميزانية زمنية بدل الاستطلاع:

<div dir="ltr">

```csharp
await call.Should().ThrowWithinAsync(TimeSpan.FromSeconds(5));
await call.Should().ThrowWithinAsync<PaymentException>(TimeSpan.FromSeconds(5));
```

</div>

يستدعي <code dir="ltr">ThrowWithinAsync()</code> المفوَّض مرة واحدة وينتظر استقرار ذلك الاستدعاء فقط. فالاستثناء المختلف، أو الاكتمال الهادئ، أو تجاوز الميزانية، كلها تفشل مع التشخيص المناسب. والاستدعاء مرة واحدة مقصود: هذا توكيد على **سلوك الاستدعاء نفسه**، وليس إعادة محاولة حتى يتحقق ما تريد؛ فمن يحتاج إعادة المحاولة فله توكيدات الاستطلاع في آخر الفصل، والمزج بين العقدين خطأ يرتكبه من يظن أن الاثنين وجهان لحاجة واحدة.

لـ<code dir="ltr">Task&lt;T&gt;</code> تتوفَّر نفس التوكيدات، مع إمكانية دمجها مع توكيدات الانتظار لاحقًا.

تستخدم المهام البادئة ومصادر إكمال المهام توكيدات إكمال بدل توكيدات المفوَّضات:

<div dir="ltr">

```csharp
await task.Should().CompleteWithinAsync(TimeSpan.FromSeconds(5)).WithResultAsync(42);
await source.Should().CompleteWithinAsync(TimeSpan.FromSeconds(5)).WithResultAsync(42);
await pending.Should().NotCompleteWithinAsync(TimeSpan.FromMilliseconds(50));
```

</div>

يعني الاكتمال أن المهمة استقرت في أي حالة نهائية: فالمهمة الفاشلة أو الملغاة تُحتسب مكتملة زمنيًا، ثم تفشل مع الاحتفاظ بالاستثناء الأصلي. ويتابع <code dir="ltr">WithResultAsync()</code> من القيمة المكتملة دون إعادة انتظار العملية. وهذا التقسيم إلى سؤالين — «هل استقرت في الميزانية؟» ثم «وبأي نتيجة استقرت؟» — يمنحك تشخيصًا دقيقًا حين يتأخر شيء: هل المشكلة في زمن الوصول، أم في النتيجة التي وصلت؟ وأما <code dir="ltr">NotCompleteWithinAsync</code> فهي الأداة الصحيحة لفحص أن شيئًا **لم يكن يجب أن ينتهي بعد** — مهمة طويلة الأجل، أو عملية عُلِّقت عمدًا — فتحوِّل الغياب إلى نتيجة مُثبَتة لا مجرد مصادفة.

### احترام الإلغاء

المفوَّض الذي يقبل <code dir="ltr">CancellationToken</code> يعقد وعدًا: إذا كان الرمز ملغىً مسبقًا فليتوقف قبل أداء العمل. وكِّد هذا الوعد مباشرة — يمرر الفعل رمزًا ملغىً ويعتبر النجاح رمي <code dir="ltr">OperationCanceledException</code> حاملًا الرمز نفسه، بلا أي انتظار زمني:

<div dir="ltr">

```csharp
Func<CancellationToken, Task> shipment = token => gateway.ShipAsync(order, token);
await shipment.Should().HonorCancellationAsync();

Action<CancellationToken> load = token => cache.Load(token);
load.Should().HonorCancellationAsync();
```

</div>

واستخدام رمز **ملغىً مسبقًا** هو مفتاح الفحص بلا ساعة: لا وقت انتظار ولا مسابقة، فإما أن الكود يحترم الإلغاء فيتوقف عند أول بوابة، وإما أن يواصل تنفيذ العمل فيكشفه الفشل فورًا. والتحقُّق من أن الاستثناء يحمل **الرمز نفسه** يمنع نوعًا خفيًا من الاحتيال: رمي <code dir="ltr">OperationCanceledException</code> من رمز آخر، أو من لغز داخلي، يعطي انطباعًا بالاحترام دون أن يمرر الطلب فعلًا.

واستخدم <code dir="ltr">CompleteDespiteCancellationAsync()</code> للعقد المعاكس — الكتل الحرجة والتنظيف التي يجب أن تكتمل حتى مع طلب الإلغاء:

<div dir="ltr">

```csharp
Func<CancellationToken, Task> flush = token => audit.FlushAsync(token);
await flush.Should().CompleteDespiteCancellationAsync();
```

</div>

المفوَّض الذي يعود بلا رمي يفشل برسالة «لم يحترم الإلغاء»، والذي يرمي استثناءً آخر (أو <code dir="ltr">OperationCanceledException</code> برمز غير ذي صلة) يفشل بتسمية المخالف. تتحقق هاتان الصيغتان من عقد الرمي فقط — ولا تحكمان على مدى سرعة ملاحظة الإلغاء.

### التدفقات غير المتزامنة

تُوكَّد تسلسلات <code dir="ltr">IAsyncEnumerable&lt;T&gt;</code> كالمجموعات، لكن غير متزامنة ومع مراعاة الإلغاء — كل فعل يقبل رمزًا يُمرَّر إلى مُعدِّد المصدر، وتسمّي الإخفاقات الفهرس الذي ظهر فيه الانحراف:

<div dir="ltr">

```csharp
await stream.Should().BeEmptyAsync();
await stream.Should().NotBeEmptyAsync();
await stream.Should().HaveCountAsync(3);
await stream.Should().ContainAsync(item);
await stream.Should().NotContainAsync(item);                 // يسمّي الإخفاق الفهرس
await stream.Should().ContainInOrderAsync([first, third]);   // تسمح بفجوات؛ ويسمّي الإخفاق نافذة البحث
await stream.Should().BeEquivalentToAsync([a, b, a]);        // مجموعة متعددة: يُتجاهل الترتيب وتُحتسب التكرارات
var items = (await stream.Should().CompleteAsync()).Which;   // استنزاف مرة واحدة وكشف العناصر
```

</div>

رمز ملغى قبل التوكيد يرمي <code dir="ltr">OperationCanceledException</code> فورًا — فالإلغاء لا يُبتلع في إخفاق أبدًا. وفحوص العدّ والتكافؤ تستنزف التسلسل كاملًا، لذا المصادر اللانهائية خارج النطاق (قرار مقصود). والأهم أن دلالات هذه العائلة تحفظ نفس درجات الصرامة التي رأيتها في فصل المجموعات — الترتيب لا يُشترط في التكافؤ، والتكرار يُحتسب، و<code dir="ltr">ContainInOrder</code> تسمح بالفجوات — فقط بوعي غير متزامن: كل فحص يمرر الرمز إلى المنبع حتى يتوقف السحب عند أول طلب إلغاء، ويسمّي موضع الانحراف بالفهرس كي لا تتحول تسلسلة بعشرات العناصر إلى مقارنة عمياء.

### سلامة التزامن تحت الحمل

تختفي علل التزامن خلف أول استثناء. يبدأ <code dir="ltr">BeThreadSafeAsync()</code> الإجراء في وقت واحد على عدة نسخ في مجمّع الخيوط خلف حاجز واحد، ويبلّغ فشلًا واحدًا مجمَّعًا — مرتَّبًا حسب نوع الاستثناء ورسالته مع أرقام النسخ الفاشلة — أو يسمّي عدد النسخ التي لم تنتهِ قط:

<div dir="ltr">

```csharp
Action refresh = () => cache.Refresh();
await refresh.Should().BeThreadSafeAsync(copies: 8, timeout: TimeSpan.FromSeconds(5));
```

</div>

النسخ التي لا تكتمل خلال المهلة تُبلَّغ كجمود محتمل؛ لاحظ أن خيوطها لا يمكن مقاطعتها، فالإجراء المتجمّد عن قصد يبقيها حتى خروج العملية. هذا فحص تشخيصي تحت الضغط، وليس إثباتًا لسلامة التزامن. والقيمة العملية للحاجز أنه يرفع فرصة التصادم: البدء المتزامن للنسخ يجعل نافذة المنافسة على المورد المشترك أوسع وأدوم، فيظهر الانهيار في اختبار يعمل في ثوانٍ بدل أن يظهر في الإنتاج بعد أسابيع. وما تسجله الرسالة المجمَّعة — نوع الخطأ ورسالته وأرقام النسخ — يحوِّل ما قد يبدو ضجيجًا عشوائيًا إلى نمط يمكن قراءته: خطأ واحد تكرر في سبع نسخ شيء آخر تمامًا من سبعة أخطاء مختلفة.

## ٥.٣ الانتظار بالاستطلاع، لا بالنوم: <code dir="ltr">SucceedWithin()</code> و<code dir="ltr">EventuallySatisfy()</code>

"ليس بعد" ليس خطأً. الفرق بينهما أن النتيجة **ستظهر لاحقًا** خلال مهلة معقولة. الحل الصحيح هو **الاستطلاع** (Polling): نتحقَّق من الشرط مرارًا حتى يتحقَّق أو تنتهي المهلة.

<div dir="ltr">

```csharp
Action sync = () => stock.Reserve(line);
sync.Should().SucceedWithin(TimeSpan.FromSeconds(5));
```

</div>

<code dir="ltr">SucceedWithin()</code> يتحقَّق من أن الإجراء **ينجح قبل انتهاء المهلة**. لو فشل، تُظهر رسالة الفشل **آخر استثناء** و**الوضع في لحظة انتهاء المهلة** — تشخيص واضح بدل "انتهت المهلة فقط". وهذه هي الفجوة التي يخلقها <code dir="ltr">Thread.Sleep</code> بلا علاج: النوم يجعل الاختبار ينتظر مدة ثابتة ثم يفحص مرة واحدة، فإن كانت البنية تحت الفحص قد وصلت بسرعة كان الوقت ضائعًا، وإن لم تكن قد وصلت كان الاختبار يفشل دون أن يعرف أحد ما آخر حالة رآها النظام. أما الاستطلاع فيقلب المعادلة: الوقت حد أعلى لا موعدًا، والتشخيص يُجمع أثناء المحاولات.

يتوفَّر <code dir="ltr">NotThrowAfter()</code> للتوافق فقط مع واجهات الاستطلاع التي تتوقَّع سلوكًا هادئًا. فهو يُعيد المحاولة حتى يتوقَّف الإجراء عن الرمي، لكنه لا يميِّز الفشل العابر عن العطل الدائم بوضوح <code dir="ltr">SucceedWithin()</code>. لذا يُفضَّل <code dir="ltr">SucceedWithin()</code>، ويُستخدم <code dir="ltr">NotThrowAfter()</code> عند مطابقة عقد استطلاع خارجي.

<div dir="ltr">

```csharp
sync.Should().NotThrowAfter(TimeSpan.FromSeconds(5), TimeSpan.FromMilliseconds(100));
await call.Should().NotThrowAfterAsync(TimeSpan.FromSeconds(5), TimeSpan.FromMilliseconds(100));
```

</div>

للتحقُّق من قيمة تتغيَّر تدريجيًا:

<div dir="ltr">

```csharp
Func<int> read = () => ledger.Balance(account);
read.Should().EventuallySatisfy(static b => b == 100, TimeSpan.FromSeconds(5));
```

</div>

وللعمليات غير المتزامنة:

<div dir="ltr">

```csharp
Func<Task<int>> readAsync = () => ledger.BalanceAsync(account);
await readAsync.Should().EventuallySatisfyAsync(
    static b => b == 100,
    TimeSpan.FromSeconds(5),
    cancellationToken);
```

</div>

الفرق الجوهري عن <code dir="ltr">Thread.Sleep</code>: نحن ننتظر **الشرط**، لا **الوقت**. لو تحقَّق الشرط بعد ٢٠٠ ميلي ثانية، ينجح الاختبار فورًا ويُوفِّر الوقت. لو لم يتحقَّق، نحصل على تشخيص مفيد (آخر قيمة أو آخر خطأ) بدل رسالة غامضة. ولاحظ أن الصيغة تعبِّر عن القصد بذاتها: <code dir="ltr">EventuallySatisfy</code> تقول لكل قارئ إن الحالة غير المستقرة جزء من السيناريو المشروع، لا عيبٌ يُنتظر أن يزول بالسكوت.

### التدريج المنحني عبر <code dir="ltr">PollingBackoff</code>

افتراضيًا تُعيد توكيدات الاستطلاع المحاولة بفاصل ثابت. وللانتظارات الطويلة، مرِّر <code dir="ltr">PollingBackoff</code> لتتزايد فواصل الانتظار بدل إرهاق الموضوع:

<div dir="ltr">

```csharp
var backoff = PollingBackoff.Exponential(
    TimeSpan.FromMilliseconds(50),
    factor: 2.0,
    maxInterval: TimeSpan.FromSeconds(1));

read.Should().EventuallySatisfy(static b => b == 100, backoff, TimeSpan.FromSeconds(30));
sync.Should().SucceedWithin(backoff, TimeSpan.FromSeconds(30));
```

</div>

<code dir="ltr">PollingBackoff.Constant(interval)</code> يطابق الفاصل الثابت الافتراضي؛ و<code dir="ltr">PollingBackoff.Exponential</code> يبدأ سريعًا ويضرب فاصل الانتظار في <code dir="ltr">factor</code> بعد كل محاولة بحدٍّ أقصى <code dir="ltr">maxInterval</code>. ويُقيَّد التأخير دائمًا بالمهلة المتبقية، مع الإبلاغ عن آخر قيمة وآخر خطأ عند الفشل. والفائدة مضاعفة: الانتظار القصير يكتشف الحالة العابرة سريعًا كما كان، والانتظار الطويل لا يستهلك المعالج بمطاردة مورد لم يجهز بعد — وحين يجمع النظام مؤشرات الأداء، يظهر أثر هذا الفرق في أرقامه لا في مشاعر الفريق فقط.

## ٥.٤ الميزانيات: أسلاك تعثر، لا مقاييس أداء

توكيدات التخصيص والوقت هنا مخصصة **لاكتشاف الانحدار المفاجئ** (Regression Smoke)، وليس لقياس الأداء الدقيق.

<div dir="ltr">

```csharp
Action price = () => catalog.PriceAll(lines);
price.ExecutionTime().Elapsed.Should().BeLessThan(TimeSpan.FromSeconds(1));

Action hot = () => pricing.Total(lines);
hot.Should().AllocateAtMost(64.Kilobytes());
hot.Should().AllocateLessThan(1.Megabytes());
await hot.Should().AllocateAtMostAsync(64.Kilobytes());
```

</div>

<code dir="ltr">ExecutionTime()</code> تقيس التنفيذ مرة واحدة وتُرجع قيمة <code dir="ltr">ExecutionTime</code> يمكن التوكيد عليها مباشرة، بما في ذلك القرب بتفاوت مسموح:

<div dir="ltr">

```csharp
price.ExecutionTime().Should().BeLessThan(TimeSpan.FromSeconds(1));
price.ExecutionTime().Should().BeCloseTo(TimeSpan.FromMilliseconds(200), TimeSpan.FromMilliseconds(50));
```

</div>

ويقبل حملان زائدان أيضًا مفوَّضًا غير متزامن (<code dir="ltr">ExecutionTime(async () =&gt; await work())</code>) وتعبير عضو (<code dir="ltr">ExecutionTimeOf(obj, x =&gt; x.Method())</code>) لقياس استدعاء واحد باسم فاعل مقروء. توكيدات التخصيص تقيس بايتات الخيط الحالي.

القاعدة الذهبية: **استخدم أسقفًا سخية جدًا** تكشف الانحدار الخطير فقط. الأداء يُقاس في بيئات مخصَّصة (Benchmarks) على أجهزة مخصَّصة، لا في اختبارات الوحدة التي تجري في بيئات متنوعة وغير متوقَّعة. والسقف السخي ليس ضعفًا في الفحص، بل دقة في الوظيفة: وظيفة هذا الاختبار أن يصرخ حين يتحول مسار ساخن من كيلوبايتات إلى ميغابايتات، لا أن يراقب انحرافًا بعشرات البايتات يتغير مع كل تحديث لمنصة التشغيل.

ملاحظة: تتبُّع التخصيص غير متاح على <code dir="ltr">netstandard2.0</code> — الدوال موجودة لكن الرصد لا يعمل في ذلك الهدف. يُفضَّل إبقاء اختبارات التخصيص على الأهداف الحديثة (<code dir="ltr">net6.0+</code>).

## ٥.٥ خلاصة وتمرين

**الخلاصة:**

- **الاستثناء قيمة.** تحقَّق من النوع أولًا (<code dir="ltr">Throw</code> مقابل <code dir="ltr">ThrowExactly</code>)، ثم من المحتوى (<code dir="ltr">WithMessage</code>، <code dir="ltr">WithParameterName</code>، <code dir="ltr">WithInnerException</code>).
- **الإلغاء يُفسَّر.** <code dir="ltr">NotThrowAsync()</code> يفشل عند الإلغاء — لأن الإلغاء نتيجة تحتاج تفسيرًا.
- **انتظر الشرط لا الوقت.** استخدم <code dir="ltr">SucceedWithin()</code> و<code dir="ltr">EventuallySatisfy()</code> بدل <code dir="ltr">Thread.Sleep</code>. تحصل على تشخيص أفضل وتختبار أسرع في المتوسط.
- **الميزانيات لسلك التعثر.** ضع أسقفًا سخية جدًا لاكتشاف الانحدارات، ولا تستخدمها لقياس الأداء.
- **السلامة تحت الحمل فحص لا برهان.** <code dir="ltr">BeThreadSafeAsync()</code> يوسّع نافذة المنافسة ويلخّص الأنماط، لكنه لا يُثبت غياب العلل.

**تمرين:**

1. اكتب اختبارًا يُفترض أن يرمي <code dir="ltr">ArgumentException</code> بمعامل <code dir="ltr">qty</code> واستثناء داخلي <code dir="ltr">FormatException</code>. تحقَّق من الثلاثة معًا.
2. حوِّل اختبارًا يستخدم <code dir="ltr">Thread.Sleep()</code> ليستخدم <code dir="ltr">EventuallySatisfy()</code> بدلًا منه.
3. أضِف توكيد تخصيص سخيفًا بشكل مقصود (<code dir="ltr">AllocateAtMost(1.Byte())</code>) وشاهِد رسالة الفشل — ثم أعده إلى قيمة معقولة.
4. خذ عملية تقبل <code dir="ltr">CancellationToken</code> واكتب لها فحصين: واحد يثبت احترامها للإلغاء، وآخر يثبت أن تنظيفها يكتمل رغم الإلغاء.

</div>

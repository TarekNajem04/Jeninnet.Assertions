<div dir="rtl">

# الفصل ٨ — العالم الخارجي: التدفقات وXML وJSON وHTTP والأحداث وملفات الإدخال/الإخراج

كل ما يقع خارج ذاكرة العملية هو **حد** محفوف بالخصوم: الشبكة تنقطع، والملفات تُقفل، والتدفقات تُغلق، والأحداث قد لا تُثار أو تُثار في توقيت خاطئ. اختبار هذه الحدود مباشرةً يعني غالبًا اختبار "الطريق السيئ" فيزيائيًا، لا منطق الكود.

القاعدة هنا بسيطة: **ادفع الحدود إلى الداخل**. زيِّف النقل، اعزل نظام الملفات، وأكِّد **القدرة** قبل **المحتوى**، وراقب ما أُثير **وما لم يُثر** معًا.

ومشكلة اختبار الحدود مباشرة ليست في الصعوبة وحدها، بل في أن كل فشل فيها يخلط بين سؤالين: هل منطقنا خاطئ، أم أن العالم الخارجي لم يلعب دوره اليوم؟ الشبكة التي تأخرت، والملف الذي كان مشغولًا، والحدث الذي تأخر ترتيبه — كلها تنتج اختبارًا أحمر لا يخبرك بشيء عن كودك. و«دفع الحدود إلى الداخل» هو حل هذه الاختلاطة: نستبدل الطرف الخارجي ببديل يسيطر عليه الاختبار، فيصبح كل أحمر محسوبًا على الكود وحده، ويصبح الجزء الحقيقي من التعامل مع الحد — عقود الفشل — قابلًا للفحص في عده قرارات لا في عده مصادفات.

## ٨.١ التدفقات: القدرة قبل المحتوى

قبل القراءة أو الكتابة في تدفق، نتحقَّق من قدراته:

<div dir="ltr">

```csharp
stream.Should().BeReadable();
stream.Should().BeWritable();
stream.Should().NotBeNull();
stream.Should().BeSeekable();
stream.Should().HaveLength(1024);
```

</div>

القراءة من تدفق غير قابل للقراءة سترمي استثناءً — لكن توكيدًا صريحًا يُخرج رسالة فشل واضحة ويُحدِّد المشكلة قبل التنفيذ. هذا أنفع بكثير في تشخيص الفشل.

وذلك أن مبدأ «القدرة قبل المحتوى» يحرّك موضع الكشف إلى أبكر نقطة ممكنة. لو تُرك الأمر للاستثناء، انفجر الخطأ عند أول عملية قراءة، وقد يكون موضع الانفجار أبعد بسطور من موضع السبب؛ أما التوكيد الصريح على القدرة فيضع الفشل عند الحد نفسه ويسمّيه باسمه: هذا التدفق لا يُقرأ، أو هذا التدفق لا يُكتب. والقدرة أيضًا عقد يُفحَص وحده كما يُفحَص السلوك: تدفق للقراءة فقط هو قرار تصميمي، وتوكيده يوثّق القرار لا يعيد اختبار القراءة.

وتُكمل السالبات وأنماط الوصول والمواضع السطح:

<div dir="ltr">

```csharp
stream.Should().NotBeReadable().And.NotBeWritable().And.NotBeSeekable();
readOnly.Should().BeReadOnly();
writeOnly.Should().BeWriteOnly();
stream.Should().NotHaveLength(0).And.HavePosition(2).And.NotHavePosition(0);
buffered.Should().NotHaveBufferSize(1024);
```

</div>

يقرأ <code dir="ltr">HavePosition()</code> موضع التموضع القابل للقراءة (وتبدأ المواضع من الصفر). أما التوكيدات السالبة فتنجح مع <code dir="ltr">null</code> أو التدفقات غير المقروءة: فالغياب لا يمكن دحضه بلا قيمة مقروءة.

بالنسبة إلى <code dir="ltr">BufferedStream</code>، يمكن التحقُّق من حجم المخزن المؤقت:

<div dir="ltr">

```csharp
buffered.Should().HaveBufferSize(8192);
```

</div>

## ٨.٢ XML وJSON: تنقَّل ثم تحقَّق

### XML

لدعم كلًا من <code dir="ltr">System.Xml.XmlNode</code> وLINQ-to-XML (<code dir="ltr">XDocument</code>، <code dir="ltr">XElement</code>، <code dir="ltr">XAttribute</code>)، تتوفَّر توكيدات متناسقة:

<div dir="ltr">

```csharp
doc.Should().HaveRoot("invoice").Which
    .Should().HaveElement("total").Which
    .Should().HaveValue("42.00");

element.Should().HaveAttribute("currency").Which
    .Should().HaveValue("USD");

element.Should().HaveAttributeWithValue("currency", "USD");
```

</div>

يُرجع <code dir="ltr">HaveElement()</code> و<code dir="ltr">HaveAttribute()</code> <code dir="ltr">.Which</code> لتمكين التنقل والتوكيد في جملة واحدة، بدلًا من البحث ثم الفحص المنفصل. وتطابق حمولات <code dir="ltr">XName</code> المؤهلة مساحات الأسماء بدقة:

<div dir="ltr">

```csharp
XNamespace ns = "urn:example";
element.Should().HaveElement(ns + "total", Exactly.Once());
element.Should().HaveElementWithValue("total", "42.00");
element.Should().HaveElementWithNamespace("total", "urn:example");
element.Should().HaveAttributeWithNamespace("currency", "urn:example");
element.Should().NotHaveElement("legacy");
element.Should().NotHaveAttribute("legacy");
element.Should().HaveInnerText("42.00");
element.Should().NotHaveInnerText("0.00");
```

</div>

للمستندات الكاملة، الأفضل مقارنة البنية لا النص:

<div dir="ltr">

```csharp
doc.Should().BeEquivalentTo(expectedDoc);
```

</div>

هذا يتجاهل الفروق غير ذات الصلة مثل ترتيب السمات أو فراغات التنسيق.

وهذا هو الفرق بين قراءة XML كنص وقراءته كبنية: النص يتأثر بأشياء لا تعني أحدًا — ترتيب السمات، وفراغات السطر، وتغليف العنصر في ذاته — فتتساقط الاختبارات مع كل إعادة تنسيق. أما المقارنة البنيوية فتفحص القرابة الدلالية؛ ومعها يصبح <code dir="ltr">Which</code> على <code dir="ltr">HaveElement()</code> و<code dir="ltr">HaveAttribute()</code> هو التنقل الطبيعي: تُمسك العقدة مرةً، ثم تُوكَّد قيمتها، فيبقى الاختبار جملة واحدة تقرؤها من أولها إلى آخرها.

### JSON

لـ<code dir="ltr">System.Text.Json.JsonNode</code> (وأشباهه)، نفس المنطق:

<div dir="ltr">

```csharp
node.Should().HaveProperty("total").Which
    .Should().HaveValue<decimal>(42.00m);

node.Should().HaveCount(3);
node.Should().BeEquivalentTo(expectedNode);
node.Should().NotBeEquivalentTo(otherNode);
node.Should().BeString().Which.Should().Be("total");
node.Should().BeNumeric().Which.Should().Be("42.00");
node.Should().BeBool().Which.Should().BeTrue();
node.Should().BeAnArray().Which.Should().HaveCount(3);
node.Should().BeUtcDate();
node.Should().BeLocalDate();
```

</div>

يعامل <code dir="ltr">JsonArray.Should()</code> المصفوفة كمجموعة، بينما يحافظ <code dir="ltr">Should&lt;T&gt;()</code> على نوع عقدة JSON المغلق للتوكيدات العامة.

واختيار الواجهة بحسب السؤال: هل تسأل عن *شكل* العقدة أم عن *نوعها الدقيق*؟ فعبارات النوع (<code dir="ltr">BeString</code>، <code dir="ltr">BeNumeric</code>، <code dir="ltr">BeBool</code>، <code dir="ltr">BeAnArray</code>) تتحقق من الحالة ثم تتيح لك مواصلة التوكيد عبر <code dir="ltr">Which</code> على القيمة المستخرجة، فيبقى الفحص على مسارين متصلين: هل هذا نص؟ وما النص؟ — بدل أن يفكك الاختبار العقدة يدويًّا ثم يقارن قطعًا معدودة.

للمخرجات الكبيرة جدًا، يُفضَّل **لقطات JSON** بدل تفكيك كل عقدة (راجع الفصل ٦).

### مسارات JSON

الوصول داخل مستندات متداخلة بمسار مصغَّر أوفر من تسلسل <code dir="ltr">HaveProperty</code>: <code dir="ltr">$</code> هو الجذر، و<code dir="ltr">.name</code> خاصية كائن (حساسة لحالة الأحرف)، و<code dir="ltr">[index]</code> عنصر مصفوفة — وتعود العقدة المُنجَزة عبر <code dir="ltr">Which</code>:

<div dir="ltr">

```csharp
node.Should().HaveJsonPath("$.users[0].name").Which.Should().BeString().Which.Should().Be("ada");
node.Should().HaveJsonPath("$.user.middle").Which.Should().BeNull();   // قيمة null ما زالت «موجودة»
node.Should().NotHaveJsonPath("$.user.deleted");
```

</div>

اختبار API نموذجي يحلِّل الحمولة مرة واحدة (<code dir="ltr">JsonNode.Parse</code> بعد <code dir="ltr">BeJsonSerializable</code>)، ثم يثبّت الحقول المهمة بالمسارات. وعند غياب مقطع، يسمّي الفشل المقطع المكسور ويسرد الخصائص الشقيقة الموجودة فعلًا (<code dir="ltr">(available: …)</code> بحدّ عشرة)، مع تلميح أقرب اسم عندما يشبه الخطأ خطأً كتابيًا (<code dir="ltr">هل تقصد 'name'؟</code>)؛ والمسارات المشوَّهة (بلا <code dir="ltr">$</code> أو مقاطع فارغة أو فهارس غير رقمية) ترمي <code dir="ltr">ArgumentException</code>. البدائل والفلاتر والأسماء المقتبسة والنزول التكراري مستبعدة عمدًا.

وقيمة هذا التشخيص تظهر في أكثر أخطاء تكامل الـAPI شيوعًا: تسمية حقل تغيّرت، أو بنية سرية («name» بدل «user.name»). رسالة تقول «المقطع user.name غير موجود، والمتاح user: name, id» تختصر عليك فتح الحمولة ومقارنتها؛ وتلميح «هل تقصد name؟» ينهي الحيرة إذا كان الفرق حرفًا واحدًا. ولهذا سقطت البدائل والفلاتر من النطاق: قوّتها الصيغية تجعل التشخيص يدور في احتمالات، والصيغة المحدودة تفشل بجملة لا تحتمل الاحتمالات.

### التسلسل ذهابًا وإيابًا

للتحقُّق من أن النوع ينجو من دورة تسلسل/استرجاع، نوكِّد الذهاب والإياب مباشرة. يُسلسَل الكائن **بنوعه الفعلي** ثم يُسترجَع ويُقارَن بمحرك التكافؤ، فأي فقد (أعضاء ضائعة أو تغيُّر الدقة أو خاصية ترمي أثناء المقارنة) يُفشل برسالة واضحة:

<div dir="ltr">

```csharp
order.Should().BeXmlSerializable();
order.Should().BeJsonSerializable();
order.Should().BeDataContractSerializable();
```

</div>

وقيمة فحص الذهاب والإياب أنه يكشف الخسائر الصامتة التي لا يراها بناء ولا يمسكها فحص عادي: عضوًا لا يُسلسَل فسقط، أو دقة تقلصت، أو خاصية ترمي عند القراءة فيفشل التسلسل في موضع بعيد عن سببها. ولأن المقارنة تمرّ بمحرك التكافؤ نفسه، فأي فارق يظهر باسم العضو الذي انكسر ومعناه — لا برسالة عامة تقول إن الاسترجاع لم ينجح.

## ٨.٣ HTTP: زيِّف النقل، لا تختبر الشبكة

توكيدات <code dir="ltr">HttpResponseMessage</code> متاحة للأهداف <code dir="ltr">net6.0</code> فأحدث:

<div dir="ltr">

```csharp
response.Should().HaveStatusCode(HttpStatusCode.OK);
response.Should().BeSuccessful(); // أي حالة 2xx
response.Should().HaveHeader("X-Request-Id");
response.Should().HaveHeader("Content-Type", "application/json");
```

</div>

**قاعدة صارمة:** اختبر دائمًا ضد <code dir="ltr">HttpMessageHandler</code> مزيَّف، لا ضد شبكة حقيقية. الشبكة غير حتمية، بطيئة، وعرضة للتغير — ما يجعل الاختبار هشًا ويصعب تكراره.

لو كانت <code dir="ltr">response</code> <code dir="ltr">null</code>، فستفشل كل توكيد برسالة واضحة عن <code dir="ltr">null</code> بدلًا من رمي <code dir="ltr">NullReferenceException</code>. هذا يحافظ على تشخيص واضح.

والقاعدة «زيِّف النقل» ليست فقط لأن الشبكة غير حتمية، بل لأنها تجعل الدلالة الحقيقية للاختبار قابلة للتحكم: تريد أن تفحص كيف يتعامل كودك مع ٤٠٣ و٥٠٠ وترويسة ناقصة؟ البديل المزيَّف يعطيك ذلك بلا عنوان حقيقي ولا مهلة انتظار؛ ومعالج مزيَّف واحد يعيد ما تشاء متى تشاء. أما استدعاء الخدمة الفعلية فيحوّل اختبارًا عن سلوك كودك إلى اختبار عن توفر خدمة لا تملكها.

## ٨.٤ الأحداث: تحقَّق مما أُثير وما لم يُثر

لمراقبة أحداث <code dir="ltr">event</code>، نستخدم <code dir="ltr">Monitor()</code> الذي يلتصق بالأحداث العامة ويجمع وقوعاتها:

<div dir="ltr">

```csharp
using var monitor = publisher.Monitor();
publisher.Ship(order);

monitor.Should().Raise("OrderShipped")
    .Which.Sender.Should().BeSameAs(publisher);
```

</div>

يُتيح لنا <code dir="ltr">.Which</code> الوصول إلى <code dir="ltr">OccurredEvent</code>: <code dir="ltr">EventName</code>، <code dir="ltr">Sender</code>، <code dir="ltr">Args</code>، <code dir="ltr">Timestamp</code>. هذا يمكِّننا من التحقُّق الدقيق — مثل التأكُّد من أن الحدث أُثير من المرسِل الصحيح بالضبط. وتتوفر توكيدات الوقوع مباشرة بالتسلسل:

<div dir="ltr">

```csharp
monitor.Should().Raise("OrderShipped")
    .WithSender(publisher)
    .And.WithArgs<OrderShippedArgs>(static args => args.OrderId == 42);
```

</div>

يقبل <code dir="ltr">WithArgs()</code> أيضًا مسندًا لكل وسيط (<code dir="ltr">WithArgs(p1, p2)</code>)؛ ويجب أن يطابق عدد المسندات عدد الوسائط. ولمصادر <code dir="ltr">INotifyPropertyChanged</code> يوجد اختصار (المطابقة حرفية وحساسة لحالة الأحرف):

<div dir="ltr">

```csharp
monitor.Should().RaisePropertyChangeFor("Total");
monitor.Should().NotRaisePropertyChangeFor("Discount");
```

</div>

والمراقِب نفسه قابل للفحص عبر العقد العام <code dir="ltr">IMonitor&lt;OccurredEvent&gt;</code> (وهي <code dir="ltr">Subject</code> و<code dir="ltr">MonitoredEvents</code> و<code dir="ltr">GetRecordingFor(name)</code> و<code dir="ltr">Clear()</code>)، ويسمّي <code dir="ltr">Monitor&lt;T&gt;()</code> المصدر سكونياً. أما المصادر التي ترمي موصِّلات اشتراكها فتفشل سريعًا افتراضيًا؛ واختر <code dir="ltr">EventMonitorOptions.IgnoringEventAccessorExceptions()</code> لتخطّي تلك الأحداث بدلًا من ذلك:

<div dir="ltr">

```csharp
using var monitor = publisher.Monitor(new EventMonitorOptions().IgnoringEventAccessorExceptions());
```

</div>

الأهم من تأكيد ما أُثير هو تأكيد **ما لم يُثر**:

<div dir="ltr">

```csharp
monitor.Should().NotRaise("OrderCancelled");
```

</div>

الأخطاء الصامتة غالبًا ما تختبئ هنا: حدث غير مرغوب أُثير دون أن ننتبه، أو حدث قديم بقي في مسار التنفيذ. تأكيد الصمت يحمي من هذه الحالات.

وللحدث ثلاثة أسئلة لا يكفي أحدها: هل أُثير؟ ومن المرسِل؟ وبأي حمولة؟ رسالة أن الحدث أُثير من مصدر آخر لا تكشفه قائمة العدّ وحدها، ولذلك تُسلَّم الوقوعة كاملةً مع المرسِل والحماولة عبر <code dir="ltr">WithSender</code> و<code dir="ltr">WithArgs</code>. وأما تأكيد الصمت فهو الحارس الأخير: أن يُعدم حدث ما ليس عيبًا وظيفيًا لأن أحدًا لا يفحص غيابه، وأن تبقى آثار مسار قديم تظهر في شكل أحداث «ما زالت تُثار» بعد أن ظن الجميع أنهم أزالوها.

## ٨.٥ نظام الملفات: بيئة معزولة لكل اختبار

للتعامل مع نظام الملفات دون التأثير على مستودع العمل، توفر المكتبة <code dir="ltr">TestEnvironment</code>. وهو قابل للتصرف، ويُنشئ شجرة مؤقتة كاملة لكل اختبار:

<div dir="ltr">

```csharp
using var env = new TestEnvironment();
var dir = env.CreateDirectory("inbox");
env.CreateFile("inbox/order.json", "{\"id\":1}");
env.CreateTree(new Dictionary<string, string?> {
    ["a/b.txt"] = "x",
    ["a/c.txt"] = null  // إنشاء مجلدات فقط إن لزم الأمر
});
env.CreateFiles("one.txt", "two.txt");
```

</div>

كل شيء يُنشأ تحت <code dir="ltr">env.Root</code>، وكل شيء يُحذف تلقائيًا عند التخلص من <code dir="ltr">env</code>.

### محاكاة حالات الفشل

نحتاج أحيانًا لاختبار ما يحدث حين يكون الملف مقفلًا أو المجلد غير قابل للوصول. هنا تساعد أدوات المحاكاة:

<div dir="ltr">

```csharp
using var locked = env.CreateLockedFile("busy", "data.txt");
env.SetInaccessible("busy\\data.txt");
env.CreateInaccessibleDirectory("vault");
env.AssertDirectoryInaccessible("vault");
```

</div>

تُغطي هذه الأدوات الفروق بين المنصات (ACLs في Windows مقابل صلاحيات Unix) عبر <code dir="ltr">InaccessibleDirectorySimulator.CreateLockedDir</code> و<code dir="ltr">CreatePseudoInaccessibleDir</code>، ما يسمح باختبار مسارات الفشل باستمرار عبر بيئات التطوير المختلفة.

ووراء هذا السعي إلى بيئة موحَّدة أعطاب لا تظهر إلا حيث تختلف المنصات: ملف مفتوح في Windows يقفل ما يقفل، وصلاحية مجلد في Unix تمنع ما تمنع، واختبار يعمل على جهازك يفشل على خط الأنابيب لأن الفرق في نظام الملفات لا في الكود. ولذلك تُنشأ حالات الرفض هذه بأدوات تحاكيها على النظامين معًا، فتُفحَص مسارات الفشل نفسها — رسالة مقروءة، وسلوك متوقَّع — بدل أن تبقى تلك المسارات بلا اختبار لأن إنتاجها يدويًّا يتفاوت بين الأجهزة.

## ٨.٦ خلاصة وتمرين

**الخلاصة:**

- **ادفع الحدود إلى الداخل.** زيِّف النقل، واعزل نظام الملفات. لا تختبر الشبكة ولا نظام الملفات الفعلي في اختبارات الوحدة.
- **القدرة قبل المحتوى.** تحقَّق من <code dir="ltr">BeReadable()</code>، <code dir="ltr">BeWritable()</code>، <code dir="ltr">BeSeekable()</code> قبل القراءة/الكتابة.
- **تنقَّل ثم تحقَّق.** استخدم <code dir="ltr">.Which</code> مع <code dir="ltr">HaveElement()</code> و<code dir="ltr">HaveAttribute()</code> و<code dir="ltr">HaveProperty()</code>.
- **أكِّد الحضور والغياب معًا.** استخدم <code dir="ltr">Raise()</code> لما يجب أن يُثار، و<code dir="ltr">NotRaise()</code> لما يجب ألا يُثار.
- **بيئة معزولة لكل اختبار.** استخدم <code dir="ltr">TestEnvironment</code> وحرِّر الموارد بـ <code dir="ltr">using</code>.

**تمرين:**

1. أنشئ <code dir="ltr">TestEnvironment</code>، واكتب ملفًا، ثم تحقَّق من أنه قابل للقراءة.
2. حلِّل مستند XML بسيط باستخدام <code dir="ltr">HaveRoot()</code> و<code dir="ltr">HaveElement()</code> و<code dir="ltr">HaveValue()</code>.
3. راقب حدثًا باستخدام <code dir="ltr">Monitor()</code>، وأكِّد أنه أُثير من المرسِل الصحيح، وأن حدثًا آخر **لم يُثر**.
4. جرِّب إنشاء ملف "مقفل" باستخدام <code dir="ltr">CreateLockedFile()</code> أو <code dir="ltr">SetInaccessible()</code>.

</div>

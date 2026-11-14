<div dir="rtl">

# دليل الهجرة — من التوكيدات الكلاسيكية إلى <code dir="ltr">Should()</code>

هذا الدليل رفيق [الفصل 10](10-ecosystem.md). يشرح محلِّل Roslyn الاختياري (<code dir="ltr">Jeninnet.Assertions.Analyzer</code>) عبر كل إطار يفهمه، ويُظهر إعادة الكتابة التي ينتجها، ويسرد ما يتركه للتحويل اليدوي. والمقصود هجرة تُنفَّذ اقتراحًا اقتراحًا ويمكن مراجعة كل فرق فيها، لا استبدالًا جماعيًّا يتعذّر تتبّعه.

وشرطا كل اقتراح واضحان: أن يترجم الكود الناتج، وأن تكون دلالته هي دلالة الأصل. والشرط الأول وحده لا يكفي؛ فالتحويل الذي يترجم ويغيّر الدلالة يعبر فحص البناء ثم يظهر أثره بعد الالتزام: توكيد ينجح لأنه يقارن أوسع مما كان الأصل يقارنه، أو يفشل عند أول قيمة حدّية كانت القاعدة القديمة تعالجها علاجًا آخر. ولهذا اقتصرت الاقتراحات على صيغ محدَّدة يمكن إثبات تكافؤها واحدةً واحدة، وما لا يمكن إثباته لا يُقترح فيه شيء ويحوَّل يدويًّا مع مراجعة.

## 1. تفعيل المحلِّل

يُشحن المحلِّل كحزمة مستقلة على <code dir="ltr">netstandard2.0</code> تُحمَّل في كل مضيف Roslyn (Visual Studio وVS Code مع C# وJetBrains Rider و<code dir="ltr">dotnet build</code>):

<div dir="ltr">

```shell
dotnet add package Jeninnet.Assertions.Analyzer
```

</div>

يبلِّغ عن سبعة تشخيصات بمستوى <code dir="ltr">Info</code> ويقدّم إصلاحًا لكلٍّ منها (البادئة <code dir="ltr">JNA</code> تعني Jeninnet Assertions):

- **<code dir="ltr">JNA1001</code>** — المساواة: <code dir="ltr">Assert.AreEqual</code> / <code dir="ltr">AreNotEqual</code>، وxUnit <code dir="ltr">Assert.Equal</code> / <code dir="ltr">NotEqual</code>، وNUnit <code dir="ltr">Assert.That(actual, Is.EqualTo(...))</code>.
- **<code dir="ltr">JNA1002</code>** — الشروط: <code dir="ltr">Assert.IsTrue</code> / <code dir="ltr">IsFalse</code> / <code dir="ltr">IsNull</code> / <code dir="ltr">IsNotNull</code>، وxUnit <code dir="ltr">Assert.True</code> / <code dir="ltr">False</code> / <code dir="ltr">Null</code> / <code dir="ltr">NotNull</code>، وNUnit <code dir="ltr">Assert.That(actual, Is.True / False / Null / Not.Null)</code>.
- **<code dir="ltr">JNA1003</code>** — النصوص: <code dir="ltr">StringAssert.Contains</code> / <code dir="ltr">StartsWith</code> / <code dir="ltr">EndsWith</code>، وxUnit <code dir="ltr">Assert.Contains</code> (نص) / <code dir="ltr">StartsWith</code> / <code dir="ltr">EndsWith</code>.
- **<code dir="ltr">JNA1004</code>** — المجموعات: <code dir="ltr">Assert.Empty</code> / <code dir="ltr">NotEmpty</code> / <code dir="ltr">Single</code> / <code dir="ltr">Contains</code> / <code dir="ltr">DoesNotContain</code>، و<code dir="ltr">Assert.IsEmpty</code> / <code dir="ltr">IsNotEmpty</code>، و<code dir="ltr">CollectionAssert.Contains</code>.
- **<code dir="ltr">JNA1005</code>** — الاستثناءات: <code dir="ltr">Assert.Throws</code> / <code dir="ltr">ThrowsAsync</code> / <code dir="ltr">ThrowsException</code> / <code dir="ltr">ThrowsExceptionAsync</code> في موضع العبارة.
- **<code dir="ltr">JNA1006</code>** — الأنواع: <code dir="ltr">Assert.IsInstanceOfType</code> / <code dir="ltr">IsType</code> / <code dir="ltr">IsAssignableFrom</code> في موضع العبارة.
- **<code dir="ltr">JNA1007</code>** — حالة المهمة: <code dir="ltr">Assert.True(task.IsCompleted)</code> / <code dir="ltr">Assert.False(task.IsCompleted)</code>.

المستوى <code dir="ltr">Info</code> لا يُفشل البناء أبدًا، فالهجرة قرارك دائمًا.

## 2. تطبيق الإصلاح

- **استدعاء واحد في كل مرة:** ضع المؤشر على الاستدعاء واقبل إصلاح المصباح (<code dir="ltr">Convert to 'Be'</code>). يصبح المستقبِل <code dir="ltr">Should()</code> ويُنقل سبب الرسالة إلى <code dir="ltr">because</code>.
- **ملف أو مشروع كامل:** استخدم **إصلاح الكل** في قائمة النطاق (مستند / مشروع / حل). تُطبَّق الإصلاحات دفعةً واحدة.
- **تُضاف <code dir="ltr">using</code> نيابةً عنك:** يُدرِج الإصلاح <code dir="ltr">using Jeninnet.Assertions;</code> عند غيابها.

## 3. الجدول المرجعي

### MSTest (<code dir="ltr">Microsoft.VisualStudio.TestTools.UnitTesting.Assert</code>)

| قبل | بعد |
|---|---|
| <code dir="ltr">Assert.AreEqual(expected, actual)</code> | <code dir="ltr">actual.Should().Be(expected)</code> |
| <code dir="ltr">Assert.AreNotEqual(expected, actual)</code> | <code dir="ltr">actual.Should().NotBe(expected)</code> |
| <code dir="ltr">Assert.AreEqual(true, flag)</code> | <code dir="ltr">flag.Should().BeTrue()</code> |
| <code dir="ltr">Assert.IsTrue(flag)</code> | <code dir="ltr">flag.Should().BeTrue()</code> |
| <code dir="ltr">Assert.IsFalse(flag)</code> | <code dir="ltr">flag.Should().BeFalse()</code> |
| <code dir="ltr">Assert.IsNull(value)</code> | <code dir="ltr">value.Should().BeNull()</code> |
| <code dir="ltr">Assert.IsNotNull(value)</code> | <code dir="ltr">value.Should().NotBeNull()</code> |
| <code dir="ltr">StringAssert.Contains(actual, "frag")</code> | <code dir="ltr">actual.Should().Contain("frag")</code> |
| <code dir="ltr">StringAssert.StartsWith(actual, "start")</code> | <code dir="ltr">actual.Should().StartWith("start")</code> |
| <code dir="ltr">StringAssert.EndsWith(actual, "end")</code> | <code dir="ltr">actual.Should().EndWith("end")</code> |
| <code dir="ltr">CollectionAssert.Contains(items, item)</code> | <code dir="ltr">items.Should().Contain(item)</code> |
| <code dir="ltr">Assert.IsInstanceOfType(value, typeof(Order))</code> | <code dir="ltr">value.Should().BeAssignableTo&lt;Order&gt;()</code> |
| <code dir="ltr">Assert.IsExactInstanceOfType(value, typeof(Order))</code> | <code dir="ltr">value.Should().BeOfType&lt;Order&gt;()</code> |
| <code dir="ltr">Assert.Throws&lt;T&gt;(() =&gt; Act())</code> | <code dir="ltr">new Action(() =&gt; Act()).Should().Throw&lt;T&gt;()</code> |
| <code dir="ltr">Assert.IsTrue(task.IsCompleted)</code> | <code dir="ltr">task.Should().BeCompleted()</code> |

### xUnit (<code dir="ltr">Xunit.Assert</code>)

| قبل | بعد |
|---|---|
| <code dir="ltr">Assert.Equal(expected, actual)</code> | <code dir="ltr">actual.Should().Be(expected)</code> |
| <code dir="ltr">Assert.NotEqual(expected, actual)</code> | <code dir="ltr">actual.Should().NotBe(expected)</code> |
| <code dir="ltr">Assert.True(flag)</code> | <code dir="ltr">flag.Should().BeTrue()</code> |
| <code dir="ltr">Assert.False(flag)</code> | <code dir="ltr">flag.Should().BeFalse()</code> |
| <code dir="ltr">Assert.Null(value)</code> | <code dir="ltr">value.Should().BeNull()</code> |
| <code dir="ltr">Assert.NotNull(value)</code> | <code dir="ltr">value.Should().NotBeNull()</code> |
| <code dir="ltr">Assert.Contains(fragment, actual)</code> | <code dir="ltr">actual.Should().Contain(fragment)</code> |
| <code dir="ltr">Assert.StartsWith(prefix, actual)</code> | <code dir="ltr">actual.Should().StartWith(prefix)</code> |
| <code dir="ltr">Assert.EndsWith(suffix, actual)</code> | <code dir="ltr">actual.Should().EndWith(suffix)</code> |
| <code dir="ltr">Assert.Empty(items)</code> | <code dir="ltr">items.Should().BeEmpty()</code> |
| <code dir="ltr">Assert.NotEmpty(items)</code> | <code dir="ltr">items.Should().NotBeEmpty()</code> |
| <code dir="ltr">Assert.Single(items)</code> | <code dir="ltr">items.Should().ContainSingle()</code> |
| <code dir="ltr">Assert.Contains(item, items)</code> | <code dir="ltr">items.Should().Contain(item)</code> |
| <code dir="ltr">Assert.IsType&lt;Order&gt;(value)</code> | <code dir="ltr">value.Should().BeOfType&lt;Order&gt;()</code> |
| <code dir="ltr">Assert.IsAssignableFrom&lt;Entity&gt;(value)</code> | <code dir="ltr">value.Should().BeAssignableTo&lt;Entity&gt;()</code> |
| <code dir="ltr">Assert.Throws&lt;T&gt;(() =&gt; Act())</code> | <code dir="ltr">new Action(() =&gt; Act()).Should().ThrowExactly&lt;T&gt;()</code> |
| <code dir="ltr">Assert.True(task.IsCompleted)</code> | <code dir="ltr">task.Should().BeCompleted()</code> |

### NUnit الكلاسيكي (<code dir="ltr">NUnit.Framework.Assert</code>)

تُحوَّل <code dir="ltr">AreEqual</code> و<code dir="ltr">AreNotEqual</code> و<code dir="ltr">IsTrue</code> و<code dir="ltr">IsFalse</code> و<code dir="ltr">IsNull</code> و<code dir="ltr">IsNotNull</code> تمامًا كجدول MSTest أعلاه. وتستخدم <code dir="ltr">StringAssert.Contains</code>/<code dir="ltr">StartsWith</code>/<code dir="ltr">EndsWith</code> الأهداف نفسها <code dir="ltr">Contain</code>/<code dir="ltr">StartWith</code>/<code dir="ltr">EndWith</code>، وتُحوَّل <code dir="ltr">Assert.IsEmpty</code>/<code dir="ltr">IsNotEmpty</code>/<code dir="ltr">Contains</code>/<code dir="ltr">DoesNotContain</code> إلى أفعال المجموعات، و<code dir="ltr">Assert.IsInstanceOf</code>/<code dir="ltr">IsAssignableFrom</code> إلى <code dir="ltr">BeAssignableTo</code> (مع السماح بالأنواع المشتقة)، و<code dir="ltr">Assert.Throws&lt;T&gt;</code> إلى <code dir="ltr">ThrowExactly&lt;T&gt;</code> (يشترط NUnit النوع الدقيق للاستثناء).

### نموذج القيود في NUnit (<code dir="ltr">Assert.That</code>)

| قبل | بعد |
|---|---|
| <code dir="ltr">Assert.That(actual, Is.EqualTo(expected))</code> | <code dir="ltr">actual.Should().Be(expected)</code> |
| <code dir="ltr">Assert.That(actual, Is.Not.EqualTo(expected))</code> | <code dir="ltr">actual.Should().NotBe(expected)</code> |
| <code dir="ltr">Assert.That(flag, Is.True)</code> | <code dir="ltr">flag.Should().BeTrue()</code> |
| <code dir="ltr">Assert.That(flag, Is.False)</code> | <code dir="ltr">flag.Should().BeFalse()</code> |
| <code dir="ltr">Assert.That(value, Is.Null)</code> | <code dir="ltr">value.Should().BeNull()</code> |
| <code dir="ltr">Assert.That(value, Is.Not.Null)</code> | <code dir="ltr">value.Should().NotBeNull()</code> |

تُحفظ الوسائط المُسمّاة (<code dir="ltr">expected:</code> و<code dir="ltr">actual:</code> و<code dir="ltr">message:</code>)، وتصبح الرسالة <code dir="ltr">because</code>.

## 4. ما يُترك عن قصد

لا يقدّم المحلِّل إصلاحًا إلا حين يكون الاستدعاء الناتج مضمون الترجمة ومطابقًا في المعنى. ويمتنع عن الاقتراح في الحالات التالية:

- **المساواة بسماحية/فارق** — <code dir="ltr">Assert.AreEqual(5.0, value, 0.1)</code> وحمولات الدقة في xUnit.
- **المساواة الحساسة لحالة الأحرف أو الثقافة** — رايات <code dir="ltr">ignoreCase</code> ووسائط <code dir="ltr">CultureInfo</code>.
- **المقارنات المخصّصة** — حمولات <code dir="ltr">IEqualityComparer</code>.
- **مساواة الأرقام والتعدادات** — تُترك <code dir="ltr">Assert.AreEqual</code> على الأرقام أو التعدادات لتختار الفعل الرقمي المناسب (<code dir="ltr">BeApproximately</code> و<code dir="ltr">BeGreaterThan</code> …).
- **قيود NUnit المعقّدة** — أي شيء يتجاوز <code dir="ltr">Is.EqualTo</code> و<code dir="ltr">Is.True/False</code> و<code dir="ltr">Is.Null</code> وصيغ <code dir="ltr">Is.Not</code> (مثل <code dir="ltr">Is.GreaterThan</code> و<code dir="ltr">Is.InRange</code> و<code dir="ltr">Has.Count</code>).
- **حمولات حالة الأحرف/الثقافة للنصوص** — تبقى <code dir="ltr">StringAssert.Contains(actual, text, StringComparison...)</code> يدوية.
- **النتائج المستهلكة** — لا تُحوَّل <code dir="ltr">Assert.Single</code> و<code dir="ltr">Assert.IsType</code>/<code dir="ltr">IsAssignableFrom</code> وتوكيدات الاستثناء إلا عندما تُهمَل النتيجة؛ فالأفعال الانسيابية تعيد كائن توكيد لا العنصر أو الاستثناء أو القيمة النوعية.
- **<code dir="ltr">Exception</code> المجرّد** — يبقى <code dir="ltr">Assert.Throws&lt;Exception&gt;</code> يدويًّا لأن عقد الاستثناء المقصود غامض.
- **احتواء القواميس** — يحتاج <code dir="ltr">Contains</code> في القاموس إلى <code dir="ltr">ContainKey</code>/<code dir="ltr">ContainValue</code>، لذا لا يُحوَّل إلى <code dir="ltr">Contain</code>.
- **أدوات اللقطات** — لـ<code dir="ltr">ShouldMatchApproved</code>/<code dir="ltr">Verify</code> دلالات اعتماد مختلفة ولا تُحوَّل أبدًا.
- **المستقبِلات الحرفية** — <code dir="ltr">Assert.AreEqual("a", "b")</code> لا تملك تعبير موضوع لربط <code dir="ltr">Should()</code> به.
- **التوكيدات مُرجِعة القيمة** — لا تُقترح تحويلات تقتضي تغيير نوع قيمة.

أما هذه فحوّلها يدويًّا حتى تبقى الدلالة المقصودة معلومةً في موضع التحويل.

والقائمة السابقة ليست ثغرات في المحلِّل، بل تعيين لحدود الأمان فيه. كل بند منها حالة يبدو فيها التحويل المباشر صحيحًا وهو يغيّر الدلالة: <code dir="ltr">Assert.AreEqual(5.0, value, 0.1)</code> يقارن بسماحية معلنة، ونقله إلى <code dir="ltr">Be</code> يحوّله إلى مقارنة صارمة تفشل عند أول انحراف كان الأصل يقبله. و<code dir="ltr">ignoreCase</code> يثبت قرارًا لغويًّا لا يستطيع المحلِّل استنتاجه من الوسيطين، فيمتنع عن الاقتراح وإن كان النقل الحرفي ممكنًا. وفي الحالتين يكون التحويل اليدوي هو الموضع الذي تبقى فيه الدلالة الأصلية موثقة وقابلة للمراجعة.

## 5. بعد إعادة الكتابة

1. **ابنِ** — لا يقترح الإصلاح إلا ما يُترجَم، لكن <code dir="ltr">usings</code> والحمولات المحلية هي المرجع.
2. **شغِّل الاختبارات** — الرسالة الانسيابية أغنى وليست مطابقة؛ والاختبارات الخالية من اللقطات تبقى خضراء.
3. **اقرأ الرسائل الجديدة** — إن غمض فشلٌ، أضِف سببًا <code dir="ltr">because</code>: <code dir="ltr">actual.Should().Be(expected, "the total must match the order")</code>.
4. **احذف مرجع الإطار القديم** متى خلا مشروع من استدعاءات <code dir="ltr">Assert</code> المتبقية.

أما الهجرة من مكتبات تأكيدات أخرى (نفس الأفعال مع أسماء قليلة متغيّرة) فمغطّاة على حدة في [الفصل 10](10-ecosystem.md).

وبعد اكتمال التحويل لا يُغلق الملف عند آخر استبدال: راجع رسالة فشل واحدة على الأقل من الاختبارات المحوَّلة، وقارن ما تقوله بما كانت تقوله الرسالة القديمة؛ فإن كانت أقل تفصيلًا في موضع مهم، أضِف <code dir="ltr">because</code> هناك مرة واحدة فيبقى تفسير التوقّع موثقًا في الرسالة نفسها. وأجّل حذف مرجع الإطار القديم إلى أن يخلو المشروع من استدعاءات <code dir="ltr">Assert</code> المتبقية، حتى تبقى كل مرحلة قابلة للرجوع إليها إن ظهر ما يستدعي المراجعة.

</div>

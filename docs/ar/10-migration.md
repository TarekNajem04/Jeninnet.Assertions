<div dir="rtl">

# دليل الهجرة — من التوكيدات الكلاسيكية إلى <code dir="ltr">Should()</code>

هذا الدليل رفيق [الفصل 10](10-ecosystem.md). يشرح محلِّل Roslyn الاختياري (<code dir="ltr">Jeninnet.Assertions.Analyzer</code>) عبر كل إطار يفهمه، ويُظهر إعادة الكتابة التي ينتجها، ويسرد ما يتركه للتحويل اليدوي. والمقصود هجرة تُنفَّذ اقتراحًا اقتراحًا ويمكن مراجعة كل فرق فيها، لا استبدالًا جماعيًّا يتعذّر تتبّعه. أما الأقسام اللاحقة فتعيّن Shouldly وNFluent وAwesomeAssertions/FluentAssertions يدويًّا، بما في ذلك الأشكال التي لا يجوز ترجمتها واحدًا لواحد.

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

## 5. القدوم من Shouldly

يعرض Shouldly توكيداته كامتدادات على الموضوع (<code dir="ltr">ShouldBe</code> و<code dir="ltr">ShouldThrow</code> …)، فمعظم الهجرة إعادة تسمية إلى <code dir="ltr">Should().*</code>. وShouldly بترخيص BSD-3-Clause؛ والجدول يعيّن القصد لا كل حمولة.

| Shouldly | Jeninnet | ملاحظات |
|---|---|---|
| <code dir="ltr">value.ShouldBe(x)</code> | <code dir="ltr">value.Should().Be(x)</code> | مساواة القيمة والمرجع تتبع نوع الموضوع. |
| <code dir="ltr">value.ShouldNotBe(x)</code> | <code dir="ltr">value.Should().NotBe(x)</code> | |
| <code dir="ltr">value.ShouldBeNull()</code> / <code dir="ltr">ShouldNotBeNull()</code> | <code dir="ltr">value.Should().BeNull()</code> / <code dir="ltr">NotBeNull()</code> | |
| <code dir="ltr">value.ShouldBeTrue()</code> / <code dir="ltr">ShouldBeFalse()</code> | <code dir="ltr">value.Should().BeTrue()</code> / <code dir="ltr">BeFalse()</code> | |
| <code dir="ltr">value.ShouldBeOfType&lt;T&gt;()</code> | <code dir="ltr">value.Should().BeOfType&lt;T&gt;()</code> | استخدم <code dir="ltr">BeAssignableTo&lt;T&gt;()</code> عندما تُقبل الأنواع المشتقة. |
| <code dir="ltr">value.ShouldBeAssignableTo&lt;T&gt;()</code> | <code dir="ltr">value.Should().BeAssignableTo&lt;T&gt;()</code> | |
| <code dir="ltr">value.ShouldBeOneOf(a, b)</code> | <code dir="ltr">value.Should().BeOneOf([a, b])</code> | <code dir="ltr">ObjectAssertions.BeOneOf</code> يأخذ تسلسلًا؛ ولتوكيدات النصوص حمولة <code dir="ltr">params</code> أيضًا. |
| <code dir="ltr">value.ShouldBeInRange(a, b)</code> | <code dir="ltr">value.Should().BeInRange(a, b)</code> | متاح حيث يكون الموضوع قابلًا للمقارنة. |
| <code dir="ltr">value.ShouldBeCloseTo(x, tolerance)</code> | <code dir="ltr">value.Should().BeApproximately(x, tolerance)</code> | <code dir="ltr">tolerance</code> هو أقصى فرق مطلق، لا عدد منازل عشرية. |
| <code dir="ltr">value.ShouldMatch(regex)</code> | <code dir="ltr">value.Should().MatchRegex(regex)</code> | Shouldly يطابق تعبيرًا نمطيًّا؛ و<code dir="ltr">Match</code> مطابقة wildcard، فلا تستبدله بها. |
| <code dir="ltr">action.ShouldThrow&lt;T&gt;()</code> | <code dir="ltr">action.Should().Throw&lt;T&gt;()</code> | <code dir="ltr">Throw</code> يقبل الاستثناءات المشتقة؛ استخدم <code dir="ltr">ThrowExactly&lt;T&gt;()</code> للنوع الدقيق. |
| <code dir="ltr">action.ShouldNotThrow()</code> | <code dir="ltr">action.Should().NotThrow()</code> | |
| <code dir="ltr">task.ShouldCompleteIn(timeout)</code> | <code dir="ltr">await task.Should().CompleteWithinAsync(timeout)</code> | الفعل في Jeninnet غير متزامن. |
| <code dir="ltr">value.ShouldMatchApproved()</code> | <code dir="ltr">value.Should().MatchSnapshot()</code> | دلالات الاعتماد مختلفة؛ راجع قسم اللقطات بدل ترجمة واحد لواحد. |
| <code dir="ltr">DynamicShould...</code> | لا مقابل | وكِّد على قيم <code dir="ltr">dynamic</code> صراحةً حتى يظهر النوع المتوقع. |

## 6. القدوم من NFluent

يتمركز NFluent حول <code dir="ltr">Check.That(subject).*</code>؛ استبدل الجذر بـ<code dir="ltr">subject.Should()</code> واحتفظ بالفعل. وNFluent بترخيص Apache-2.0.

| NFluent | Jeninnet | ملاحظات |
|---|---|---|
| <code dir="ltr">Check.That(x).IsEqualTo(y)</code> | <code dir="ltr">x.Should().Be(y)</code> | |
| <code dir="ltr">Check.That(x).IsNotEqualTo(y)</code> | <code dir="ltr">x.Should().NotBe(y)</code> | |
| <code dir="ltr">Check.That(x).IsNull()</code> | <code dir="ltr">x.Should().BeNull()</code> | |
| <code dir="ltr">Check.That(x).IsInstanceOf&lt;T&gt;()</code> | <code dir="ltr">x.Should().BeOfType&lt;T&gt;()</code> | استخدم <code dir="ltr">BeAssignableTo&lt;T&gt;()</code> عندما تُقبل الأنواع المشتقة. |
| <code dir="ltr">Check.That(x).IsGreaterThan(y)</code> | <code dir="ltr">x.Should().BeGreaterThan(y)</code> | |
| <code dir="ltr">Check.That(x).IsInRange(a, b)</code> | <code dir="ltr">x.Should().BeInRange(a, b)</code> | |
| <code dir="ltr">Check.That(x).Contains(...)</code> | <code dir="ltr">x.Should().Contain(...)</code> | يعمل للنصوص والمجموعات؛ والقواميس تستخدم <code dir="ltr">ContainKey</code>/<code dir="ltr">ContainValue</code>. |
| <code dir="ltr">Check.That(x).ContainsExactly(...)</code> | <code dir="ltr">x.Should().Equal(...)</code> | <code dir="ltr">Equal</code> يحافظ على الترتيب؛ استخدم <code dir="ltr">BeEquivalentTo</code> فقط عندما لا يهم الترتيب. |
| <code dir="ltr">Check.That(x).IsOnlyMadeOf(...)</code> | <code dir="ltr">x.Should().OnlyContain(item =&gt; allowed.Contains(item))</code> | إن كان التعدد الدقيق هو العقد فاستخدم <code dir="ltr">BeEquivalentTo</code>؛ و<code dir="ltr">OnlyContain</code> يحفظ معنى العضوية. |
| <code dir="ltr">Check.That(x).IsInAscendingOrder()</code> | <code dir="ltr">x.Should().BeInAscendingOrder()</code> | |
| <code dir="ltr">Check.That(x).HasSize(n)</code> | <code dir="ltr">x.Should().HaveCount(n)</code> | |
| <code dir="ltr">Check.That(x).IsEmpty()</code> | <code dir="ltr">x.Should().BeEmpty()</code> | |
| <code dir="ltr">Check.That(code).Throws&lt;T&gt;()</code> | <code dir="ltr">code.Should().Throw&lt;T&gt;()</code> | استخدم <code dir="ltr">ThrowExactly&lt;T&gt;()</code> عندما يكون النوع الدقيق هو العقد. |
| <code dir="ltr">Check.That(code).LastsLessThan(ms)</code> | <code dir="ltr">Action action = code; action.ExecutionTime().Should().BeLessThan(...)</code> | قِس الفعل صراحةً؛ لا زمن مخفيًّا داخل فعل. |
| <code dir="ltr">Check.That(x).Which</code> | <code dir="ltr">.Which</code> حيث يعيده الفعل، وإلا <code dir="ltr">.And</code> | شكل التسلسل يعتمد على نوع إرجاع الفعل. |
| <code dir="ltr">Check.ThatEnum(e)</code> | <code dir="ltr">e.Should()</code> | توكيدات التعداد تعرض أفعاله المخصصة. |
| <code dir="ltr">Check.That(x).Extracting(...)</code> | <code dir="ltr">x.Select(...).Should()</code> | أو استخدم <code dir="ltr">OnlyHaveUniqueItems(selector)</code> بالمفتاح عندما يكون القصد التفرّد. |

## 7. القدوم من AwesomeAssertions أو FluentAssertions

AwesomeAssertions (AA) شوكة مجتمعية من FluentAssertions (FA). وAA بترخيص Apache-2.0. أما FA 7.x وما قبلها فبترخيص Apache-2.0 أيضًا؛ وFA 8 وما بعدها تستخدم Xceed Community License — مجانية للاستخدام غير التجاري، ويتطلب الاستخدام التجاري ترخيصًا مدفوعًا من Xceed. وJeninnet بترخيص MIT ومستقلة عن كلتيهما.

تتبع واجهة AA إصدار FA 7.x عن قرب، فتنطبق مادة الهجرة الموجّهة إلى FA في [الفصل 10](10-ecosystem.md). والفروق الميكانيكية هي:

- **الحزمة ومساحة الأسماء.** كان AA 8.x بديلًا مباشرًا يحتفظ بمساحة اسم <code dir="ltr">FluentAssertions</code> واسم التجميع نفسه. أما AA 9 فغيّر كليهما إلى <code dir="ltr">AwesomeAssertions</code>. في الحالتين استبدل <code dir="ltr">using</code> بـ<code dir="ltr">using Jeninnet.Assertions;</code>.
- **بيانات الامتداد.** يملك AA <code dir="ltr">[CustomAssertion]</code> على الطرق، و<code dir="ltr">[CustomAssertions]</code> على الأصناف، و<code dir="ltr">[CustomAssertionsAssembly]</code> على التجميعات. ويملك Jeninnet <code dir="ltr">[CustomAssertion]</code> و<code dir="ltr">[CustomAssertionsAssembly]</code>؛ ولا يوجد مقابل على مستوى الصنف بعد، فوسم كل طريقة مخصصة.
- **بيانات المعاملات.** يعرض Jeninnet <code dir="ltr">ParameterInfo.Should()</code> بأفعال الاسم والنوع الدقيق والاختياري/الإلزامي والقيمة الافتراضية و<code dir="ltr">out</code>/<code dir="ltr">ref</code>/<code dir="ltr">in</code>/<code dir="ltr">params</code> والموضع والسمات. وتُقابل <code dir="ltr">ParameterInfoAssertions</code> في AA (التي تقتصر على السمات) مباشرةً لفحوص التزيين، أما أفعال البيانات الأوسع فهي خاصة بـJeninnet.
- **مناطق خاصة بـJeninnet.** التوطين والساعة الحتمية وبنية IO وتوكيدات التخصيص ولقطات v2 لا مقابل لها في FA/AA للهجرة منها.
- **الترخيص.** احفظ ترخيص المكتبة التي تزيلها في ملاحظات المراجعة: Shouldly BSD-3-Clause، وNFluent وAA Apache-2.0، وFA 7.x وما قبلها Apache-2.0، وFA 8+ Xceed Community/تجاري، وJeninnet MIT.

## 8. ما لا يُحوَّل واحدًا لواحد أبدًا

بعض الأشكال تبدو ميكانيكية لكنها تغيّر المعنى عند ترجمتها. حوّلها يدويًّا واحفظ القصد الأصلي في الاختبار.

| النمط | لماذا لا يُحوَّل تلقائيًّا | البديل |
|---|---|---|
| <code dir="ltr">ShouldMatchApproved</code> / <code dir="ltr">Verify</code> | أدوات الاعتماد تُسمّي الملفات وتلتقطها بطريقة مختلفة، وغالبًا تقبل أول تشغيل تلقائيًّا. | <code dir="ltr">MatchSnapshot</code> بملف verified صريح، مع مراجعة الفرق. |
| <code dir="ltr">DynamicShould...</code> | استدعاءات <code dir="ltr">dynamic</code> تُربط وقت التشغيل، فلا يمكن إثبات تحويل ساكن. | وكِّد على متغيرات محلية نوعية أو تحويلات <code dir="ltr">object</code> صريحة. |
| حمولات السماحية والمقارن والثقافة و<code dir="ltr">StringComparison</code> | قد يختار الفعل الانسيابي مقارنة مختلفة. | اختر <code dir="ltr">BeApproximately</code> أو <code dir="ltr">Contain(..., StringComparison...)</code> أو مقارنًا صريحًا، ثم راجع النتيجة. |
| سلاسل <code dir="ltr">Which</code> والقيود المركبة في NFluent | نوع النتيجة وسلوك الاختصار مختلفان. | استخدم <code dir="ltr">.Which</code> فقط عندما يعيده فعل Jeninnet؛ وإلا فقسّم التوكيدات. |
| سلاسل قيود NUnit (<code dir="ltr">Is.GreaterThan</code> و<code dir="ltr">Has.Count</code> …) | القيود تُركَّب بطريقة مختلفة عن الاستدعاءات الانسيابية. | اكتب تسلسل الأفعال المكافئ صراحةً. |
| <code dir="ltr">[CustomAssertions]</code> على مستوى الصنف في AA | لا يملك Jeninnet سمة على مستوى الصنف. | ضع <code dir="ltr">[CustomAssertion]</code> على كل طريقة مخصصة، أو <code dir="ltr">[CustomAssertionsAssembly]</code> على التجميع. |
| حزم FA 8+ | الترخيص ليس فرق كود. | احسم الترخيص قبل الهجرة؛ ولا تنسخ مصدر FA 8 إلى مشروع MIT. |
| <code dir="ltr">Assert.That(..., Is.Unique)</code> والعضوية المبنية على المجموعات | دلالات المجموعة والتعدد يسهل الخلط بينهما. | <code dir="ltr">OnlyHaveUniqueItems()</code> للتفرّد، و<code dir="ltr">OnlyContain</code> للعضوية، و<code dir="ltr">BeEquivalentTo</code> للتعدد الدقيق. |

## 9. بعد إعادة الكتابة

1. **ابنِ** — لا يقترح الإصلاح إلا ما يُترجَم، لكن <code dir="ltr">usings</code> والحمولات المحلية هي المرجع.
2. **شغِّل الاختبارات** — الرسالة الانسيابية أغنى وليست مطابقة؛ والاختبارات الخالية من اللقطات تبقى خضراء.
3. **اقرأ الرسائل الجديدة** — إن غمض فشلٌ، أضِف سببًا <code dir="ltr">because</code>: <code dir="ltr">actual.Should().Be(expected, "the total must match the order")</code>.
4. **احذف مرجع الإطار القديم** متى خلا مشروع من استدعاءات <code dir="ltr">Assert</code> المتبقية.

وبعد اكتمال التحويل لا يُغلق الملف عند آخر استبدال: راجع رسالة فشل واحدة على الأقل من الاختبارات المحوَّلة، وقارن ما تقوله بما كانت تقوله الرسالة القديمة؛ فإن كانت أقل تفصيلًا في موضع مهم، أضِف <code dir="ltr">because</code> هناك مرة واحدة فيبقى تفسير التوقّع موثقًا في الرسالة نفسها. وأجّل حذف مرجع الإطار القديم إلى أن يخلو المشروع من استدعاءات <code dir="ltr">Assert</code> المتبقية، حتى تبقى كل مرحلة قابلة للرجوع إليها إن ظهر ما يستدعي المراجعة.

</div>

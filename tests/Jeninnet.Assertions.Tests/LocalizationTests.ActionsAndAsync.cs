//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Tests;

public partial class LocalizationTests {
    [TestMethod]
    public void Arabic_ActionThrow() {
        using var _ = new ArabicScope();

        Action action = () => { };
        var none = Assert.ThrowsExactly<AssertionFailedException>(() => action.Should().Throw<InvalidOperationException>());
        Assert.AreEqual("المتوقع من action أن يرمي InvalidOperationException، لكن لم يُرمَ أي استثناء.", none.Message);

        Action wrong = () => throw new ArgumentException("wrong");
        var wrongType = Assert.ThrowsExactly<AssertionFailedException>(() => wrong.Should().Throw<InvalidOperationException>());
        Assert.AreEqual("المتوقع من wrong أن يرمي InvalidOperationException، لكن رُمي ArgumentException.", wrongType.Message);

        var exactlyWrong = Assert.ThrowsExactly<AssertionFailedException>(() => wrong.Should().ThrowExactly<InvalidOperationException>());
        Assert.AreEqual("المتوقع من wrong أن يرمي InvalidOperationException بالضبط، لكن رُمي ArgumentException.", exactlyWrong.Message);

        var exactlyNone = Assert.ThrowsExactly<AssertionFailedException>(() => action.Should().ThrowExactly<InvalidOperationException>());
        Assert.AreEqual("المتوقع من action أن يرمي InvalidOperationException بالضبط، لكن لم يُرمَ أي استثناء.", exactlyNone.Message);

        var notThrow = Assert.ThrowsExactly<AssertionFailedException>(() => wrong.Should().NotThrow());
        Assert.AreEqual("المتوقع من wrong ألا يرمي، لكن رُمي ArgumentException.", notThrow.Message);
    }

    [TestMethod]
    public async Task Arabic_ActionAsyncThrowAsync() {
        using var _ = new ArabicScope();

        Func<Task> action = () => Task.CompletedTask;
        var none = await Assert.ThrowsExactlyAsync<AssertionFailedException>(() => action.Should().ThrowAsync<InvalidOperationException>());
        Assert.AreEqual("المتوقع من action أن يرمي InvalidOperationException، لكن لم يُرمَ أي استثناء.", none.Message);

        Func<Task> wrong = () => Task.FromException(new ArgumentException("wrong"));
        var notThrow = await Assert.ThrowsExactlyAsync<AssertionFailedException>(() => wrong.Should().NotThrowAsync());
        Assert.AreEqual("المتوقع من wrong ألا يرمي، لكن رُمي ArgumentException.", notThrow.Message);
    }

    [TestMethod]
    public void Arabic_ActionPolling() {
        using var _ = new ArabicScope();

        Action always = () => throw new InvalidOperationException("always");
        var ex = Assert.ThrowsExactly<AssertionFailedException>(
            () => always.Should().SucceedWithin(TimeSpan.FromMilliseconds(100), TimeSpan.FromMilliseconds(10)));
        Assert.AreEqual("المتوقع من always أن ينجح خلال 00:00:00.1000000، لكنه ظل يفشل: always.", ex.Message);
    }

    [TestMethod]
    public void Arabic_ActionAllocation() {
        using var _ = new ArabicScope();

        Action action = () => throw new InvalidOperationException("boom");
        var threw = Assert.ThrowsExactly<AssertionFailedException>(() => action.Should().AllocateAtMost(1.Megabytes()));
        Assert.AreEqual("المتوقع من action أن يخصص 1 MB على الأكثر، لكنه رمى InvalidOperationException.", threw.Message);

        var lessThrew = Assert.ThrowsExactly<AssertionFailedException>(() => action.Should().AllocateLessThan(1.Megabytes()));
        Assert.AreEqual("المتوقع من action أن يخصص أقل من 1 MB، لكنه رمى InvalidOperationException.", lessThrew.Message);

        Action alloc = () => {
            var buffer = new byte[100_000];
            GC.KeepAlive(buffer);
        };

        var atMost = Assert.ThrowsExactly<AssertionFailedException>(() => alloc.Should().AllocateAtMost(1.Kilobytes()));
        Assert.Contains("المتوقع من alloc أن يخصص 1 KB على الأكثر، لكن خصص", atMost.Message);

        var less = Assert.ThrowsExactly<AssertionFailedException>(() => alloc.Should().AllocateLessThan(1.Kilobytes()));
        Assert.Contains("المتوقع من alloc أن يخصص أقل من 1 KB، لكن خصص", less.Message);
    }

    [TestMethod]
    public async Task Arabic_Cancellation() {
        using var _ = new ArabicScope();

        Func<CancellationToken, Task> work = static _ => Task.CompletedTask;
        var honored = await Assert.ThrowsExactlyAsync<AssertionFailedException>(() => work.Should().HonorCancellationAsync());
        Assert.AreEqual("المتوقع من work أن يحترم الإلغاء، لكنه اكتمل دون رمي OperationCanceledException.", honored.Message);

        Func<CancellationToken, Task> flush = static token => Task.FromCanceled(token);
        var completed = await Assert.ThrowsExactlyAsync<AssertionFailedException>(() => flush.Should().CompleteDespiteCancellationAsync());
        Assert.AreEqual("المتوقع من flush أن يكتمل رغم الإلغاء، لكنه أُلغي.", completed.Message);
    }

    [TestMethod]
    public async Task Arabic_AsyncEnumerable() {
        using var _ = new ArabicScope();

        var items = Counting(1, 2);
        var empty = await Assert.ThrowsExactlyAsync<AssertionFailedException>(() => items.Should().BeEmptyAsync());
        Assert.AreEqual("المتوقع من items أن يكون فارغاً، لكن وُجد 2 من العناصر.", empty.Message);

        var words = Counting(1, 2, 3);
        var contain = await Assert.ThrowsExactlyAsync<AssertionFailedException>(() => words.Should().NotContainAsync(2));
        Assert.AreEqual("المتوقع من words ألا يحتوي '2'، لكن وُجد عند الفهرس 1.", contain.Message);
    }

    [TestMethod]
    public void Arabic_RegexGroups() {
        using var _ = new ArabicScope();

        var m = NamedYearMonthRegex().Match("2026-01");
        var value = Assert.ThrowsExactly<AssertionFailedException>(() => m.Should().HaveGroup("month", "99"));
        Assert.AreEqual("المتوقع من m أن يحتوي المجموعة 'month' بقيمة '99'، لكن وُجد '01'.", value.Message);

        var missing = Assert.ThrowsExactly<AssertionFailedException>(() => m.Should().HaveGroup("day", "01"));
        Assert.AreEqual("المتوقع من m أن يحتوي المجموعة 'day'، لكنها غير موجودة (المتاح: 0, year, month).", missing.Message);
    }

    [TestMethod]
    public async Task Arabic_ThreadSafety() {
        using var _ = new ArabicScope();

        Action action = static () => throw new InvalidOperationException("boom");
        var ex = await Assert.ThrowsExactlyAsync<AssertionFailedException>(() => action.Should().BeThreadSafeAsync(copies: 2));
        Assert.AreEqual("المتوقع من action أن يكون آمناً للتزامن، لكن فشلت 2 من 2 نسخ:\n1. InvalidOperationException: boom (النسخ: 1, 2).", ex.Message);
    }

    [TestMethod]
    public void Arabic_Task15_Additions() {
        using var _ = new ArabicScope();

        var number = JsonNode.Parse("\"a\"")!;
        var numeric = Assert.ThrowsExactly<AssertionFailedException>(() => number.Should().BeNumeric());
        Assert.AreEqual("المتوقع من number أن يكون رقم JSON، لكن وُجد \"a\".", numeric.Message);

        var item = XElement.Parse("<item>text</item>");
        var innerText = Assert.ThrowsExactly<AssertionFailedException>(() => item.Should().HaveInnerText("other"));
        Assert.AreEqual("المتوقع من item أن يكون نصه الداخلي 'other'، لكن وُجد 'text'.", innerText.Message);

        Action action = () => throw new InvalidOperationException("boom happened");
        var withoutMessage = Assert.ThrowsExactly<AssertionFailedException>(
            () => action.Should().Throw<InvalidOperationException>().WithoutMessage("boom*"));
        Assert.AreEqual("المتوقع من رسالة الاستثناء ألا تطابق 'boom*'، لكن وُجد 'boom happened'.", withoutMessage.Message);

        object subject = 42;
        var cast = Assert.ThrowsExactly<AssertionFailedException>(() => subject.As<string>());
        Assert.AreEqual("المتوقع من subject أن يكون قابلاً للتحويل إلى String، لكن وُجد Int32.", cast.Message);

        Action noisy = () => throw new InvalidOperationException("always");
        var notQuiet = Assert.ThrowsExactly<AssertionFailedException>(
            () => noisy.Should().NotThrowAfter(TimeSpan.FromMilliseconds(100), TimeSpan.FromMilliseconds(10)));
        Assert.AreEqual("المتوقع من noisy ألا يرمي خلال 00:00:00.1000000، لكن رُمي InvalidOperationException.", notQuiet.Message);
    }

    [TestMethod]
    public async Task Arabic_Task15_Additions_Async() {
        using var _ = new ArabicScope();

        var pending = new TaskCompletionSource<int>();
        await pending.Should().NotCompleteWithinAsync(TimeSpan.FromMilliseconds(50));

        var completed = new TaskCompletionSource<int>();
        completed.SetResult(1);
        var notCompleted = await Assert.ThrowsExactlyAsync<AssertionFailedException>(
            () => completed.Should().NotCompleteWithinAsync(TimeSpan.FromMilliseconds(50)));
        Assert.AreEqual("المتوقع من completed ألا يكتمل خلال 00:00:00.0500000، لكنه اكتمل.", notCompleted.Message);

        Func<Task> quiet = static () => Task.CompletedTask;
        var noThrow = await Assert.ThrowsExactlyAsync<AssertionFailedException>(
            () => quiet.Should().ThrowWithinAsync(TimeSpan.FromSeconds(5)));
        Assert.AreEqual("المتوقع من quiet أن يرمي Exception خلال 00:00:05، لكن لم يُرمَ أي استثناء.", noThrow.Message);

        var wrong = Task.FromResult(7);
        var result = await Assert.ThrowsExactlyAsync<AssertionFailedException>(
            () => wrong.Should().CompleteWithinAsync(TimeSpan.FromSeconds(5)).WithResultAsync(8));
        Assert.AreEqual("المتوقع من wrong أن تكون نتيجته 8، لكن وُجد 7.", result.Message);
    }

    [TestMethod]
    public void Arabic_ExecutionTime() {
        using var _ = new ArabicScope();

        Action slow = static () => SpinWait.SpinUntil(static () => false, 150);
        var less = Assert.ThrowsExactly<AssertionFailedException>(() => slow.ExecutionTime().Should().BeLessThan(TimeSpan.FromMilliseconds(20)));
        Assert.Contains("المتوقع من زمن التنفيذ أن يكون أقل من 00:00:00.0200000، لكن وُجد", less.Message);

        var lessOrEqual = Assert.ThrowsExactly<AssertionFailedException>(() => slow.ExecutionTime().Should().BeLessThanOrEqualTo(TimeSpan.FromMilliseconds(5)));
        Assert.Contains("المتوقع من زمن التنفيذ أن يكون أقل من أو يساوي 00:00:00.0050000، لكن وُجد", lessOrEqual.Message);

        Action fast = () => { };
        var greater = Assert.ThrowsExactly<AssertionFailedException>(() => fast.ExecutionTime().Should().BeGreaterThan(TimeSpan.FromMinutes(1)));
        Assert.Contains("المتوقع من زمن التنفيذ أن يكون أكبر من 00:01:00، لكن وُجد", greater.Message);

        var greaterOrEqual = Assert.ThrowsExactly<AssertionFailedException>(() => fast.ExecutionTime().Should().BeGreaterThanOrEqualTo(TimeSpan.FromMinutes(1)));
        Assert.Contains("المتوقع من زمن التنفيذ أن يكون أكبر من أو يساوي 00:01:00، لكن وُجد", greaterOrEqual.Message);
    }

    [TestMethod]
    public void Arabic_ExceptionDetails() {
        using var _ = new ArabicScope();

        Action action = () => throw new InvalidOperationException("boom happened");
        var message = Assert.ThrowsExactly<AssertionFailedException>(() => action.Should().Throw<InvalidOperationException>().WithMessage("bam*"));
        Assert.AreEqual("المتوقع من رسالة الاستثناء أن تطابق 'bam*'، لكن وُجد 'boom happened'.", message.Message);

        Action nested = () => throw new InvalidOperationException("outer", new ArgumentException("inner"));
        var inner = Assert.ThrowsExactly<AssertionFailedException>(() => nested.Should().Throw<InvalidOperationException>().WithInnerException<TimeoutException>());
        Assert.AreEqual("المتوقع من الاستثناء الداخلي أن يكون TimeoutException، لكن وُجد ArgumentException.", inner.Message);

        var match = Assert.ThrowsExactly<AssertionFailedException>(() => action.Should().Throw<InvalidOperationException>().Where(static e => e.Message.Contains("zzz")));
        Assert.Contains("المتوقع من الاستثناء أن يطابق", match.Message);

        Action param = static () => ThrowArgument("myParam");
        var name = Assert.ThrowsExactly<AssertionFailedException>(() => param.Should().Throw<ArgumentException>().WithParameterName("other"));
        Assert.AreEqual("المتوقع من اسم معامل الاستثناء أن يكون 'other'، لكن وُجد 'myParam'.", name.Message);
    }

    [TestMethod]
    public async Task Arabic_TaskCompletion() {
        using var _ = new ArabicScope();

        var pending = new TaskCompletionSource();
        var within = await Assert.ThrowsExactlyAsync<AssertionFailedException>(() => pending.Should().CompleteWithinAsync(TimeSpan.FromMilliseconds(50)));
        Assert.AreEqual("المتوقع من pending أن يكتمل خلال 00:00:00.0500000، لكنه لم يكتمل.", within.Message);

        var source = new TaskCompletionSource();
        source.SetException(new InvalidOperationException("boom"));
        var successfully = await Assert.ThrowsExactlyAsync<AssertionFailedException>(() => source.Should().CompleteWithinAsync(TimeSpan.FromSeconds(5)));
        Assert.AreEqual("المتوقع من source أن يكتمل بنجاح خلال 00:00:05، لكن رُمي InvalidOperationException.", successfully.Message);
    }

    [TestMethod]
    public void Arabic_ExecutionTimeBecause() {
        using var _ = new ArabicScope();

        const bool flag = false;
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => flag.Should().BeTrue("الاختبار"));
        Assert.AreEqual("المتوقع من flag أن يكون true، لكن وُجد false لأن الاختبار.", ex.Message);
    }
}

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
        var none = Throws<AssertionFailedException>(() => action.Should().Throw<InvalidOperationException>());
        none.Message.Should().Be("المتوقع من action أن يرمي InvalidOperationException، لكن لم يُرمَ أي استثناء.");

        Action wrong = () => throw new ArgumentException("wrong");
        var wrongType = Throws<AssertionFailedException>(() => wrong.Should().Throw<InvalidOperationException>());
        wrongType.Message.Should().Be("المتوقع من wrong أن يرمي InvalidOperationException، لكن رُمي ArgumentException.");

        var exactlyWrong = Throws<AssertionFailedException>(() => wrong.Should().ThrowExactly<InvalidOperationException>());
        exactlyWrong.Message.Should().Be("المتوقع من wrong أن يرمي InvalidOperationException بالضبط، لكن رُمي ArgumentException.");

        var exactlyNone = Throws<AssertionFailedException>(() => action.Should().ThrowExactly<InvalidOperationException>());
        exactlyNone.Message.Should().Be("المتوقع من action أن يرمي InvalidOperationException بالضبط، لكن لم يُرمَ أي استثناء.");

        var notThrow = Throws<AssertionFailedException>(() => wrong.Should().NotThrow());
        notThrow.Message.Should().Be("المتوقع من wrong ألا يرمي، لكن رُمي ArgumentException.");
    }

    [TestMethod]
    public async Task Arabic_ActionAsyncThrowAsync() {
        using var _ = new ArabicScope();

        Func<Task> action = () => Task.CompletedTask;
        var none = await ThrowsAsync<AssertionFailedException>(() => action.Should().ThrowAsync<InvalidOperationException>());
        none.Message.Should().Be("المتوقع من action أن يرمي InvalidOperationException، لكن لم يُرمَ أي استثناء.");

        Func<Task> wrong = () => Task.FromException(new ArgumentException("wrong"));
        var notThrow = await ThrowsAsync<AssertionFailedException>(() => wrong.Should().NotThrowAsync());
        notThrow.Message.Should().Be("المتوقع من wrong ألا يرمي، لكن رُمي ArgumentException.");
    }

    [TestMethod]
    public void Arabic_ActionPolling() {
        using var _ = new ArabicScope();

        Action always = () => throw new InvalidOperationException("always");
        var ex = Throws<AssertionFailedException>(
            () => always.Should().SucceedWithin(TimeSpan.FromMilliseconds(100), TimeSpan.FromMilliseconds(10)));
        ex.Message.Should().Be("المتوقع من always أن ينجح خلال 00:00:00.1000000، لكنه ظل يفشل: always.");
    }

    [TestMethod]
    public void Arabic_ActionAllocation() {
        using var _ = new ArabicScope();

        Action action = () => throw new InvalidOperationException("boom");
        var threw = Throws<AssertionFailedException>(() => action.Should().AllocateAtMost(1.Megabytes()));
        threw.Message.Should().Be("المتوقع من action أن يخصص 1 MB على الأكثر، لكنه رمى InvalidOperationException.");

        var lessThrew = Throws<AssertionFailedException>(() => action.Should().AllocateLessThan(1.Megabytes()));
        lessThrew.Message.Should().Be("المتوقع من action أن يخصص أقل من 1 MB، لكنه رمى InvalidOperationException.");

        Action alloc = () => {
            var buffer = new byte[100_000];
            GC.KeepAlive(buffer);
        };

        var atMost = Throws<AssertionFailedException>(() => alloc.Should().AllocateAtMost(1.Kilobytes()));
        atMost.Message.Should().Contain("المتوقع من alloc أن يخصص 1 KB على الأكثر، لكن خصص");

        var less = Throws<AssertionFailedException>(() => alloc.Should().AllocateLessThan(1.Kilobytes()));
        less.Message.Should().Contain("المتوقع من alloc أن يخصص أقل من 1 KB، لكن خصص");
    }

    [TestMethod]
    public async Task Arabic_Cancellation() {
        using var _ = new ArabicScope();

        Func<CancellationToken, Task> work = static _ => Task.CompletedTask;
        var honored = await ThrowsAsync<AssertionFailedException>(() => work.Should().HonorCancellationAsync());
        honored.Message.Should().Be("المتوقع من work أن يحترم الإلغاء، لكنه اكتمل دون رمي OperationCanceledException.");

        Func<CancellationToken, Task> flush = static token => Task.FromCanceled(token);
        var completed = await ThrowsAsync<AssertionFailedException>(() => flush.Should().CompleteDespiteCancellationAsync());
        completed.Message.Should().Be("المتوقع من flush أن يكتمل رغم الإلغاء، لكنه أُلغي.");
    }

    [TestMethod]
    public async Task Arabic_AsyncEnumerable() {
        using var _ = new ArabicScope();

        var items = Counting(1, 2);
        var empty = await ThrowsAsync<AssertionFailedException>(() => items.Should().BeEmptyAsync());
        empty.Message.Should().Be("المتوقع من items أن يكون فارغاً، لكن وُجد 2 من العناصر.");

        var words = Counting(1, 2, 3);
        var contain = await ThrowsAsync<AssertionFailedException>(() => words.Should().NotContainAsync(2));
        contain.Message.Should().Be("المتوقع من words ألا يحتوي '2'، لكن وُجد عند الفهرس 1.");
    }

    [TestMethod]
    public void Arabic_RegexGroups() {
        using var _ = new ArabicScope();

        var m = NamedYearMonthRegex().Match("2026-01");
        var value = Throws<AssertionFailedException>(() => m.Should().HaveGroup("month", "99"));
        value.Message.Should().Be("المتوقع من m أن يحتوي المجموعة 'month' بقيمة '99'، لكن وُجد '01'.");

        var missing = Throws<AssertionFailedException>(() => m.Should().HaveGroup("day", "01"));
        missing.Message.Should().Be("المتوقع من m أن يحتوي المجموعة 'day'، لكنها غير موجودة (المتاح: 0, year, month).");
    }

    [TestMethod]
    public async Task Arabic_ThreadSafety() {
        using var _ = new ArabicScope();

        Action action = static () => throw new InvalidOperationException("boom");
        var ex = await ThrowsAsync<AssertionFailedException>(() => action.Should().BeThreadSafeAsync(copies: 2));
        ex.Message.Should().Be("المتوقع من action أن يكون آمناً للتزامن، لكن فشلت 2 من 2 نسخ:\n1. InvalidOperationException: boom (النسخ: 1, 2).");

        Func<Task> asyncAction = static () => Task.FromException(new InvalidOperationException("boom"));
        var asyncEx = await ThrowsAsync<AssertionFailedException>(() => asyncAction.Should().BeThreadSafeAsync(copies: 2));
        asyncEx.Message.Should().Be("المتوقع من asyncAction أن يكون آمناً للتزامن، لكن فشلت 2 من 2 نسخ:\n1. InvalidOperationException: boom (النسخ: 1, 2).");
    }

    [TestMethod]
    public void Arabic_TaskStates() {
        using var _ = new ArabicScope();

        var pending = new TaskCompletionSource().Task;
        var completed = Throws<AssertionFailedException>(() => pending.Should().BeCompleted());
        completed.Message.Should().Be("المتوقع من pending أن يكون مكتملاً، لكنه كان WaitingForActivation.");

        var faulted = Task.FromException(new InvalidOperationException("boom"));
        var successful = Throws<AssertionFailedException>(() => faulted.Should().BeCompletedSuccessfully());
        successful.Message.Should().Be("المتوقع من faulted أن يكتمل بنجاح، لكنه كان Faulted (InvalidOperationException).");

        var result = Task.FromResult(7);
        var mismatch = Throws<AssertionFailedException>(() => result.Should().HaveResult(8));
        mismatch.Message.Should().Be("المتوقع من result أن تكون نتيجته 8، لكن وُجد 7.");

        var done = Task.CompletedTask;
        var canceled = Throws<AssertionFailedException>(() => done.Should().BeCanceled());
        canceled.Message.Should().Be("المتوقع من done أن يكون ملغى، لكنه كان RanToCompletion.");
    }

    [TestMethod]
    public void Arabic_Task15_Additions() {
        using var _ = new ArabicScope();

        var number = JsonNode.Parse("\"a\"")!;
        var numeric = Throws<AssertionFailedException>(() => number.Should().BeNumeric());
        numeric.Message.Should().Be("المتوقع من number أن يكون رقم JSON، لكن وُجد \"a\".");

        var item = XElement.Parse("<item>text</item>");
        var innerText = Throws<AssertionFailedException>(() => item.Should().HaveInnerText("other"));
        innerText.Message.Should().Be("المتوقع من item أن يكون نصه الداخلي 'other'، لكن وُجد 'text'.");

        Action action = () => throw new InvalidOperationException("boom happened");
        var withoutMessage = Throws<AssertionFailedException>(
            () => action.Should().Throw<InvalidOperationException>().WithoutMessage("boom*"));
        withoutMessage.Message.Should().Be("المتوقع من رسالة الاستثناء ألا تطابق 'boom*'، لكن وُجد 'boom happened'.");

        object subject = 42;
        var cast = Throws<AssertionFailedException>(() => subject.As<string>());
        cast.Message.Should().Be("المتوقع من subject أن يكون قابلاً للتحويل إلى String، لكن وُجد Int32.");

        Action noisy = () => throw new InvalidOperationException("always");
        var notQuiet = Throws<AssertionFailedException>(
            () => noisy.Should().NotThrowAfter(TimeSpan.FromMilliseconds(100), TimeSpan.FromMilliseconds(10)));
        notQuiet.Message.Should().Be("المتوقع من noisy ألا يرمي خلال 00:00:00.1000000، لكن رُمي InvalidOperationException.");
    }

    [TestMethod]
    public async Task Arabic_Task15_Additions_Async() {
        using var _ = new ArabicScope();

        var pending = new TaskCompletionSource<int>();
        await pending.Should().NotCompleteWithinAsync(TimeSpan.FromMilliseconds(50));

        var completed = new TaskCompletionSource<int>();
        completed.SetResult(1);
        var notCompleted = await ThrowsAsync<AssertionFailedException>(
            () => completed.Should().NotCompleteWithinAsync(TimeSpan.FromMilliseconds(50)));
        notCompleted.Message.Should().Be("المتوقع من completed ألا يكتمل خلال 00:00:00.0500000، لكنه اكتمل.");

        Func<Task> quiet = static () => Task.CompletedTask;
        var noThrow = await ThrowsAsync<AssertionFailedException>(
            () => quiet.Should().ThrowWithinAsync(TimeSpan.FromSeconds(5)));
        noThrow.Message.Should().Be("المتوقع من quiet أن يرمي Exception خلال 00:00:05، لكن لم يُرمَ أي استثناء.");

        var wrong = Task.FromResult(7);
        var result = await ThrowsAsync<AssertionFailedException>(
            () => wrong.Should().CompleteWithinAsync(TimeSpan.FromSeconds(5)).WithResultAsync(8));
        result.Message.Should().Be("المتوقع من wrong أن تكون نتيجته 8، لكن وُجد 7.");
    }

    [TestMethod]
    public void Arabic_ExecutionTime() {
        using var _ = new ArabicScope();

        Action slow = static () => SpinWait.SpinUntil(static () => false, 150);
        var less = Throws<AssertionFailedException>(() => slow.ExecutionTime().Should().BeLessThan(TimeSpan.FromMilliseconds(20)));
        less.Message.Should().Contain("المتوقع من زمن التنفيذ أن يكون أقل من 00:00:00.0200000، لكن وُجد");

        var lessOrEqual = Throws<AssertionFailedException>(() => slow.ExecutionTime().Should().BeLessThanOrEqualTo(TimeSpan.FromMilliseconds(5)));
        lessOrEqual.Message.Should().Contain("المتوقع من زمن التنفيذ أن يكون أقل من أو يساوي 00:00:00.0050000، لكن وُجد");

        Action fast = () => { };
        var greater = Throws<AssertionFailedException>(() => fast.ExecutionTime().Should().BeGreaterThan(TimeSpan.FromMinutes(1)));
        greater.Message.Should().Contain("المتوقع من زمن التنفيذ أن يكون أكبر من 00:01:00، لكن وُجد");

        var greaterOrEqual = Throws<AssertionFailedException>(() => fast.ExecutionTime().Should().BeGreaterThanOrEqualTo(TimeSpan.FromMinutes(1)));
        greaterOrEqual.Message.Should().Contain("المتوقع من زمن التنفيذ أن يكون أكبر من أو يساوي 00:01:00، لكن وُجد");
    }

    [TestMethod]
    public void Arabic_ExceptionDetails() {
        using var _ = new ArabicScope();

        Action action = () => throw new InvalidOperationException("boom happened");
        var message = Throws<AssertionFailedException>(() => action.Should().Throw<InvalidOperationException>().WithMessage("bam*"));
        message.Message.Should().Be("المتوقع من رسالة الاستثناء أن تطابق 'bam*'، لكن وُجد 'boom happened'.");

        Action nested = () => throw new InvalidOperationException("outer", new ArgumentException("inner"));
        var inner = Throws<AssertionFailedException>(() => nested.Should().Throw<InvalidOperationException>().WithInnerException<TimeoutException>());
        inner.Message.Should().Be("المتوقع من الاستثناء الداخلي أن يكون TimeoutException، لكن وُجد ArgumentException.");

        var match = Throws<AssertionFailedException>(() => action.Should().Throw<InvalidOperationException>().Where(static e => e.Message.Contains("zzz")));
        match.Message.Should().Contain("المتوقع من الاستثناء أن يطابق");

        Action param = static () => ThrowArgument("myParam");
        var name = Throws<AssertionFailedException>(() => param.Should().Throw<ArgumentException>().WithParameterName("other"));
        name.Message.Should().Be("المتوقع من اسم معامل الاستثناء أن يكون 'other'، لكن وُجد 'myParam'.");
    }

    [TestMethod]
    public async Task Arabic_TaskCompletion() {
        using var _ = new ArabicScope();

        var pending = new TaskCompletionSource();
        var within = await ThrowsAsync<AssertionFailedException>(() => pending.Should().CompleteWithinAsync(TimeSpan.FromMilliseconds(50)));
        within.Message.Should().Be("المتوقع من pending أن يكتمل خلال 00:00:00.0500000، لكنه لم يكتمل.");

        var source = new TaskCompletionSource();
        source.SetException(new InvalidOperationException("boom"));
        var successfully = await ThrowsAsync<AssertionFailedException>(() => source.Should().CompleteWithinAsync(TimeSpan.FromSeconds(5)));
        successfully.Message.Should().Be("المتوقع من source أن يكتمل بنجاح خلال 00:00:05، لكن رُمي InvalidOperationException.");
    }

    [TestMethod]
    public void Arabic_ExecutionTimeBecause() {
        using var _ = new ArabicScope();

        const bool flag = false;
        var ex = Throws<AssertionFailedException>(() => flag.Should().BeTrue("الاختبار"));
        ex.Message.Should().Be("المتوقع من flag أن يكون true، لكن وُجد false لأن الاختبار.");
    }
}

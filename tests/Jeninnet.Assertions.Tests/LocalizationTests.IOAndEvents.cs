//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Tests;

public partial class LocalizationTests {
    [TestMethod]
    public void Arabic_FileSystem() {
        using var _ = new ArabicScope();
        using var environment = new TestEnvironment();

        var file = new FileInfo(environment.CreateFile("a.txt", "abc"));
        var length = Throws<AssertionFailedException>(() => file.Should().HaveLength(5));
        length.Message.Should().Be("المتوقع من file أن يكون طوله 5، لكن وُجد 3.");

        var directory = new DirectoryInfo(environment.CreateDirectory("dir"));
        environment.CreateFile("dir/a.txt", "a");
        var empty = Throws<AssertionFailedException>(() => directory.Should().BeEmpty());
        empty.Message.Should().Be("المتوقع من directory أن يكون فارغاً، لكن وُجد 1 عنصراً.");

        var contain = Throws<AssertionFailedException>(() => directory.Should().ContainFile("missing.txt"));
        contain.Message.Should().Be("المتوقع من directory أن يحتوي الملف 'missing.txt'، لكنه لم يُعثر عليه.");

        var missing = new FileInfo(environment.Abs("missing.txt"));
        var exist = Throws<AssertionFailedException>(() => missing.Should().Exist());
        exist.Message.Should().Be("المتوقع من missing أن يكون موجوداً، لكن it was not found.");
    }

    [TestMethod]
    public async Task Arabic_Http() {
        using var _ = new ArabicScope();
        using var response = new HttpResponseMessage(HttpStatusCode.OK) {
            Content = new StringContent("abc", Encoding.UTF8, "text/plain"),
        };

        var contentType = Throws<AssertionFailedException>(() => response.Should().HaveContentType("application/json"));
        contentType.Message.Should().Be("المتوقع من response أن يكون نوع المحتوى 'application/json'، لكن وُجد 'text/plain; charset=utf-8'.");

        var length = Throws<AssertionFailedException>(() => response.Should().HaveContentLength(5));
        length.Message.Should().Be("المتوقع من response أن يكون طول المحتوى 5، لكن وُجد 3.");

        var clientError = Throws<AssertionFailedException>(() => response.Should().BeClientError());
        clientError.Message.Should().Be("المتوقع من response أن يكون خطأ عميل (4xx)، لكن وُجد OK.");

        var cookie = Throws<AssertionFailedException>(() => response.Should().HaveCookie("session"));
        cookie.Message.Should().Be("المتوقع من response أن يضبط الكوكي 'session'، لكنه لم يُعثر عليه.");

        using var problem = new HttpResponseMessage(HttpStatusCode.BadRequest) {
            Content = new StringContent("{}", Encoding.UTF8, "text/plain"),
        };
        var problemDetails = await ThrowsAsync<AssertionFailedException>(() => problem.Should().BeProblemDetailsAsync());
        problemDetails.Message.Should().Be("المتوقع من problem أن يكون ProblemDetails بنوع محتوى 'application/problem+json' أو 'application/json'، لكن وُجد 'text/plain'.");
    }

    [TestMethod]
    public void Arabic_Stream() {
        using var _ = new ArabicScope();

        using var stream = new MemoryStream([1, 2, 3]);
        var length = Throws<AssertionFailedException>(() => stream.Should().HaveLength(5));
        length.Message.Should().Be("المتوقع من stream أن يكون طوله 5، لكن وُجد 3.");

        Stream? missing = null;
        var missingReadable = Throws<AssertionFailedException>(() => missing.Should().BeReadable());
        missingReadable.Message.Should().Be("المتوقع من missing أن يكون قابلاً للقراءة، لكن found null.");

        var beNull = Throws<AssertionFailedException>(() => stream.Should().BeNull());
        beNull.Message.Should().Be("المتوقع من stream أن يكون null، لكنه لم يكن null.");

        var notBeNull = Throws<AssertionFailedException>(() => missing.Should().NotBeNull());
        notBeNull.Message.Should().Be("المتوقع من missing ألا يكون null، لكن وُجد null.");

        using var fixedStream = new NonSeekableStream();
        var seekable = Throws<AssertionFailedException>(() => fixedStream.Should().BeSeekable());
        seekable.Message.Should().Be("المتوقع من fixedStream أن يكون قابلاً للتموضع، لكن it was not.");

        var writable = Throws<AssertionFailedException>(() => fixedStream.Should().BeWritable());
        writable.Message.Should().Be("المتوقع من fixedStream أن يكون قابلاً للكتابة، لكن it was not.");

        var unseekable = Throws<AssertionFailedException>(() => fixedStream.Should().HaveLength(3));
        unseekable.Message.Should().Be("المتوقع من fixedStream أن يكون طوله 3، لكن الدفق لا يدعم التموضع.");

        using var positioned = new MemoryStream([1, 2, 3]);
        positioned.Position = 2;
        var position = Throws<AssertionFailedException>(() => positioned.Should().HavePosition(1));
        position.Message.Should().Be("المتوقع من positioned أن يكون موضعه 1، لكن وُجد 2.");

        var notPosition = Throws<AssertionFailedException>(() => positioned.Should().NotHavePosition(2));
        notPosition.Message.Should().Be("المتوقع من positioned ألا يكون موضعه 2، لكنه وُجد.");

        var notReadable = Throws<AssertionFailedException>(() => positioned.Should().NotBeReadable());
        notReadable.Message.Should().Be("المتوقع من positioned ألا يكون قابلاً للقراءة، لكنه كان.");

        using var readOnly = new MemoryStream([1, 2, 3], writable: false);
        var notReadOnly = Throws<AssertionFailedException>(() => readOnly.Should().NotBeReadOnly());
        notReadOnly.Message.Should().Be("المتوقع من readOnly ألا يكون للقراءة فقط، لكنه كان.");

        var notLength = Throws<AssertionFailedException>(() => positioned.Should().NotHaveLength(3));
        notLength.Message.Should().Be("المتوقع من positioned ألا يكون طوله 3، لكنه وُجد.");
    }

    [TestMethod]
    public void Arabic_BufferedStream() {
        using var _ = new ArabicScope();

        using var inner = new MemoryStream();
        using var stream = new BufferedStream(inner);
        var size = Throws<AssertionFailedException>(() => stream.Should().HaveBufferSize(1024));
        size.Message.Should().Be("المتوقع من stream أن يكون حجم المخزن المؤقت 1024، لكن وُجد 4096.");

        BufferedStream? missingBuffer = null;
        var sizeNull = Throws<AssertionFailedException>(() => missingBuffer.Should().HaveBufferSize(1024));
        sizeNull.Message.Should().Be("المتوقع من missingBuffer أن يكون حجم المخزن المؤقت 1024، لكن وُجد null.");

        var notSize = Throws<AssertionFailedException>(() => stream.Should().NotHaveBufferSize(4096));
        notSize.Message.Should().Be("المتوقع من stream ألا يكون حجم المخزن المؤقت 4096، لكنه وُجد.");
    }

    [TestMethod]
    public void Arabic_EventMonitor() {
        using var _ = new ArabicScope();

        var publisher = new Publisher();
        using var monitor = publisher.Monitor();
        var raise = Throws<AssertionFailedException>(() => monitor.Should().Raise("Changed"));
        raise.Message.Should().Be("المتوقع من monitor أن يثير الحدث 'Changed'، لكنه لم يُثر.");

        publisher.RaiseChanged();
        var notRaise = Throws<AssertionFailedException>(() => monitor.Should().NotRaise("Changed"));
        notRaise.Message.Should().Be("المتوقع من monitor ألا يثير الحدث 'Changed'، لكنه أُثير 1 من المرات.");

        var sender = Throws<AssertionFailedException>(
            () => monitor.Should().Raise("Changed").WithSender(new Publisher()));
        sender.Message.Should().Contain("أن يكون مُرسِله");
    }

    /// <summary>Asserts the exact exception type asynchronously and returns the caught exception.</summary>
    /// <typeparam name="TException">The expected exception type.</typeparam>
    /// <param name="action">The asynchronous action expected to throw.</param>
    /// <returns>The caught exception.</returns>
    private static async Task<TException> ThrowsAsync<TException>(Func<Task> action) where TException : Exception =>
        (await action.Should().ThrowExactlyAsync<TException>()).Which;
}

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
        var length = Assert.ThrowsExactly<AssertionFailedException>(() => file.Should().HaveLength(5));
        Assert.AreEqual("المتوقع من file أن يكون طوله 5، لكن وُجد 3.", length.Message);

        var directory = new DirectoryInfo(environment.CreateDirectory("dir"));
        environment.CreateFile("dir/a.txt", "a");
        var empty = Assert.ThrowsExactly<AssertionFailedException>(() => directory.Should().BeEmpty());
        Assert.AreEqual("المتوقع من directory أن يكون فارغاً، لكن وُجد 1 عنصراً.", empty.Message);

        var contain = Assert.ThrowsExactly<AssertionFailedException>(() => directory.Should().ContainFile("missing.txt"));
        Assert.AreEqual("المتوقع من directory أن يحتوي الملف 'missing.txt'، لكنه لم يُعثر عليه.", contain.Message);

        var missing = new FileInfo(environment.Abs("missing.txt"));
        var exist = Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().Exist());
        Assert.AreEqual("المتوقع من missing أن يكون موجوداً، لكن it was not found.", exist.Message);
    }

    [TestMethod]
    public async Task Arabic_Http() {
        using var _ = new ArabicScope();
        using var response = new HttpResponseMessage(HttpStatusCode.OK) {
            Content = new StringContent("abc", Encoding.UTF8, "text/plain"),
        };

        var contentType = Assert.ThrowsExactly<AssertionFailedException>(() => response.Should().HaveContentType("application/json"));
        Assert.AreEqual("المتوقع من response أن يكون نوع المحتوى 'application/json'، لكن وُجد 'text/plain; charset=utf-8'.", contentType.Message);

        var length = Assert.ThrowsExactly<AssertionFailedException>(() => response.Should().HaveContentLength(5));
        Assert.AreEqual("المتوقع من response أن يكون طول المحتوى 5، لكن وُجد 3.", length.Message);

        var clientError = Assert.ThrowsExactly<AssertionFailedException>(() => response.Should().BeClientError());
        Assert.AreEqual("المتوقع من response أن يكون خطأ عميل (4xx)، لكن وُجد OK.", clientError.Message);

        var cookie = Assert.ThrowsExactly<AssertionFailedException>(() => response.Should().HaveCookie("session"));
        Assert.AreEqual("المتوقع من response أن يضبط الكوكي 'session'، لكنه لم يُعثر عليه.", cookie.Message);

        using var problem = new HttpResponseMessage(HttpStatusCode.BadRequest) {
            Content = new StringContent("{}", Encoding.UTF8, "text/plain"),
        };
        var problemDetails = await Assert.ThrowsExactlyAsync<AssertionFailedException>(() => problem.Should().BeProblemDetailsAsync());
        Assert.AreEqual("المتوقع من problem أن يكون ProblemDetails بنوع محتوى 'application/problem+json' أو 'application/json'، لكن وُجد 'text/plain'.", problemDetails.Message);
    }

    [TestMethod]
    public void Arabic_Stream() {
        using var _ = new ArabicScope();

        using var stream = new MemoryStream([1, 2, 3]);
        var length = Assert.ThrowsExactly<AssertionFailedException>(() => stream.Should().HaveLength(5));
        Assert.AreEqual("المتوقع من stream أن يكون طوله 5، لكن وُجد 3.", length.Message);

        Stream? missing = null;
        var missingReadable = Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().BeReadable());
        Assert.AreEqual("المتوقع من missing أن يكون قابلاً للقراءة، لكن found null.", missingReadable.Message);

        var beNull = Assert.ThrowsExactly<AssertionFailedException>(() => stream.Should().BeNull());
        Assert.AreEqual("المتوقع من stream أن يكون null، لكنه لم يكن null.", beNull.Message);

        var notBeNull = Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().NotBeNull());
        Assert.AreEqual("المتوقع من missing ألا يكون null، لكن وُجد null.", notBeNull.Message);

        using var fixedStream = new NonSeekableStream();
        var seekable = Assert.ThrowsExactly<AssertionFailedException>(() => fixedStream.Should().BeSeekable());
        Assert.AreEqual("المتوقع من fixedStream أن يكون قابلاً للتموضع، لكن it was not.", seekable.Message);

        var writable = Assert.ThrowsExactly<AssertionFailedException>(() => fixedStream.Should().BeWritable());
        Assert.AreEqual("المتوقع من fixedStream أن يكون قابلاً للكتابة، لكن it was not.", writable.Message);

        var unseekable = Assert.ThrowsExactly<AssertionFailedException>(() => fixedStream.Should().HaveLength(3));
        Assert.AreEqual("المتوقع من fixedStream أن يكون طوله 3، لكن الدفق لا يدعم التموضع.", unseekable.Message);

        using var positioned = new MemoryStream([1, 2, 3]);
        positioned.Position = 2;
        var position = Assert.ThrowsExactly<AssertionFailedException>(() => positioned.Should().HavePosition(1));
        Assert.AreEqual("المتوقع من positioned أن يكون موضعه 1، لكن وُجد 2.", position.Message);

        var notPosition = Assert.ThrowsExactly<AssertionFailedException>(() => positioned.Should().NotHavePosition(2));
        Assert.AreEqual("المتوقع من positioned ألا يكون موضعه 2، لكنه وُجد.", notPosition.Message);

        var notReadable = Assert.ThrowsExactly<AssertionFailedException>(() => positioned.Should().NotBeReadable());
        Assert.AreEqual("المتوقع من positioned ألا يكون قابلاً للقراءة، لكنه كان.", notReadable.Message);

        using var readOnly = new MemoryStream([1, 2, 3], writable: false);
        var notReadOnly = Assert.ThrowsExactly<AssertionFailedException>(() => readOnly.Should().NotBeReadOnly());
        Assert.AreEqual("المتوقع من readOnly ألا يكون للقراءة فقط، لكنه كان.", notReadOnly.Message);

        var notLength = Assert.ThrowsExactly<AssertionFailedException>(() => positioned.Should().NotHaveLength(3));
        Assert.AreEqual("المتوقع من positioned ألا يكون طوله 3، لكنه وُجد.", notLength.Message);
    }

    [TestMethod]
    public void Arabic_BufferedStream() {
        using var _ = new ArabicScope();

        using var inner = new MemoryStream();
        using var stream = new BufferedStream(inner);
        var size = Assert.ThrowsExactly<AssertionFailedException>(() => stream.Should().HaveBufferSize(1024));
        Assert.AreEqual("المتوقع من stream أن يكون حجم المخزن المؤقت 1024، لكن وُجد 4096.", size.Message);

        BufferedStream? missingBuffer = null;
        var sizeNull = Assert.ThrowsExactly<AssertionFailedException>(() => missingBuffer.Should().HaveBufferSize(1024));
        Assert.AreEqual("المتوقع من missingBuffer أن يكون حجم المخزن المؤقت 1024، لكن وُجد null.", sizeNull.Message);

        var notSize = Assert.ThrowsExactly<AssertionFailedException>(() => stream.Should().NotHaveBufferSize(4096));
        Assert.AreEqual("المتوقع من stream ألا يكون حجم المخزن المؤقت 4096، لكنه وُجد.", notSize.Message);
    }

    [TestMethod]
    public void Arabic_EventMonitor() {
        using var _ = new ArabicScope();

        var publisher = new Publisher();
        using var monitor = publisher.Monitor();
        var raise = Assert.ThrowsExactly<AssertionFailedException>(() => monitor.Should().Raise("Changed"));
        Assert.AreEqual("المتوقع من monitor أن يثير الحدث 'Changed'، لكنه لم يُثر.", raise.Message);

        publisher.RaiseChanged();
        var notRaise = Assert.ThrowsExactly<AssertionFailedException>(() => monitor.Should().NotRaise("Changed"));
        Assert.AreEqual("المتوقع من monitor ألا يثير الحدث 'Changed'، لكنه أُثير 1 من المرات.", notRaise.Message);

        var sender = Assert.ThrowsExactly<AssertionFailedException>(
            () => monitor.Should().Raise("Changed").WithSender(new Publisher()));
        Assert.Contains("أن يكون مُرسِله", sender.Message);
    }
}

//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Tests;

public partial class LocalizationTests {
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

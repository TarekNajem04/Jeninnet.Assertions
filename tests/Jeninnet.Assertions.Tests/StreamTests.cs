//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Tests;

[TestClass]
public class StreamTests {
    private sealed class NonSeekableStream(MemoryStream inner) : MemoryStream {
        private readonly MemoryStream _inner = inner;

        public override bool CanSeek => false;

        public override long Length => _inner.Length;
    }

    [TestMethod]
    public void Capabilities_PassForMemoryStream() {
        using var stream = new MemoryStream();
        stream.Should().BeReadable().And.BeWritable().And.BeSeekable().And.HaveLength(0).And.NotBeNull();
    }

    [TestMethod]
    public void HaveLength_FailsReportingLengths() {
        using var stream = new MemoryStream();
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => stream.Should().HaveLength(5));
        Assert.AreEqual("Expected stream to have length 5, but found 0.", ex.Message);
    }

    [TestMethod]
    public void HaveLength_FailsForNonSeekable() {
        using var inner = new MemoryStream([1, 2, 3]);
        using var stream = new NonSeekableStream(inner);
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => stream.Should().HaveLength(3));
        Assert.AreEqual("Expected stream to have length 3, but the stream does not support seeking.", ex.Message);

        var seekable = Assert.ThrowsExactly<AssertionFailedException>(() => stream.Should().BeSeekable());
        Assert.AreEqual("Expected stream to be seekable, but it was not.", seekable.Message);
    }

    [TestMethod]
    public void BufferedStream_BufferSize() {
        using var inner = new MemoryStream();
        using var stream = new BufferedStream(inner);
        stream.Should().HaveBufferSize(4096);

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => stream.Should().HaveBufferSize(1024));
        Assert.AreEqual("Expected stream to have buffer size 1024, but found 4096.", ex.Message);
    }

    [TestMethod]
    public void NullStream_FailsPositives() {
        Stream? missing = null;
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().BeReadable());
        Assert.AreEqual("Expected missing to be readable, but found null.", ex.Message);

        missing.Should().BeNull();
    }

    [TestMethod]
    public void CapabilityNegatives_PassAndFail() {
        using var stream = new MemoryStream([1, 2, 3]);
        var readable = Assert.ThrowsExactly<AssertionFailedException>(() => stream.Should().NotBeReadable());
        Assert.AreEqual("Expected stream not to be readable, but it was.", readable.Message);

        var writable = Assert.ThrowsExactly<AssertionFailedException>(() => stream.Should().NotBeWritable());
        Assert.AreEqual("Expected stream not to be writable, but it was.", writable.Message);

        var seekable = Assert.ThrowsExactly<AssertionFailedException>(() => stream.Should().NotBeSeekable());
        Assert.AreEqual("Expected stream not to be seekable, but it was.", seekable.Message);

        using var inner = new MemoryStream([1, 2, 3]);
        using var unseekable = new NonSeekableStream(inner);
        unseekable.Should().NotBeSeekable();

        Stream? missing = null;
        missing.Should().NotBeReadable().And.NotBeWritable().And.NotBeSeekable();
    }

    [TestMethod]
    public void ReadOnly_PassAndFail() {
        using var readOnly = new MemoryStream([1, 2, 3], writable: false);
        readOnly.Should().BeReadOnly();
        readOnly.Should().NotBeWriteOnly();

        using var readWrite = new MemoryStream([1, 2, 3]);
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => readWrite.Should().BeReadOnly());
        Assert.AreEqual("Expected readWrite to be read-only, but it was not.", ex.Message);

        readWrite.Should().NotBeReadOnly();
        var notEx = Assert.ThrowsExactly<AssertionFailedException>(() => readOnly.Should().NotBeReadOnly());
        Assert.AreEqual("Expected readOnly not to be read-only, but it was.", notEx.Message);
    }

    [TestMethod]
    public void WriteOnly_PassAndFail() {
        using var writeOnly = new WriteOnlyStream(new MemoryStream());
        writeOnly.Should().BeWriteOnly();
        writeOnly.Should().NotBeReadOnly();

        using var readWrite = new MemoryStream([1, 2, 3]);
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => readWrite.Should().BeWriteOnly());
        Assert.AreEqual("Expected readWrite to be write-only, but it was not.", ex.Message);

        readWrite.Should().NotBeWriteOnly();
        var notEx = Assert.ThrowsExactly<AssertionFailedException>(() => writeOnly.Should().NotBeWriteOnly());
        Assert.AreEqual("Expected writeOnly not to be write-only, but it was.", notEx.Message);
    }

    [TestMethod]
    public void NotHaveLength_PassAndFail() {
        using var stream = new MemoryStream([1, 2, 3]);
        stream.Should().NotHaveLength(5);

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => stream.Should().NotHaveLength(3));
        Assert.AreEqual("Expected stream not to have length 3, but found it.", ex.Message);

        Stream? missing = null;
        missing.Should().NotHaveLength(3);

        using var inner = new MemoryStream([1, 2, 3]);
        using var unseekable = new NonSeekableStream(inner);
        unseekable.Should().NotHaveLength(3);
    }

    [TestMethod]
    public void HavePosition_PassAndFail() {
        using var stream = new MemoryStream([1, 2, 3]);
        stream.Position = 2;
        stream.Should().HavePosition(2);
        stream.Should().NotHavePosition(1);

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => stream.Should().HavePosition(1));
        Assert.AreEqual("Expected stream to have position 1, but found 2.", ex.Message);

        var notEx = Assert.ThrowsExactly<AssertionFailedException>(() => stream.Should().NotHavePosition(2));
        Assert.AreEqual("Expected stream not to have position 2, but found it.", notEx.Message);

        Stream? missing = null;
        var nullEx = Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().HavePosition(0));
        Assert.AreEqual("Expected missing to have position 0, but found null.", nullEx.Message);
        missing.Should().NotHavePosition(0);

        using var inner = new MemoryStream([1, 2, 3]);
        using var unseekable = new NonSeekableStream(inner);
        var seekEx = Assert.ThrowsExactly<AssertionFailedException>(() => unseekable.Should().HavePosition(0));
        Assert.AreEqual("Expected unseekable to have position 0, but the stream does not support seeking.", seekEx.Message);
        unseekable.Should().NotHavePosition(0);
    }

    [TestMethod]
    public void BufferedStream_NotHaveBufferSize() {
        using var inner = new MemoryStream();
        using var stream = new BufferedStream(inner);
        stream.Should().NotHaveBufferSize(1024);

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => stream.Should().NotHaveBufferSize(4096));
        Assert.AreEqual("Expected stream not to have buffer size 4096, but found it.", ex.Message);
    }

    private sealed class WriteOnlyStream(MemoryStream inner) : MemoryStream {
        private readonly MemoryStream _inner = inner;

        public override bool CanRead => false;

        public override long Length => _inner.Length;
    }
}

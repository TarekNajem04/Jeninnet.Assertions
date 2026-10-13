//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Tests;

[TestClass]
public class TraceWriterTests {
    [TestMethod]
    public void AddSingle_RecordsIndentedLines() {
        var writer = new StringBuilderTraceWriter();
        writer.AddSingle("top");
        writer.AddSingle("a" + Environment.NewLine + "b");

        var expected = string.Join(Environment.NewLine, "  top", "  a", "  b") + Environment.NewLine;
        Assert.AreEqual(expected, writer.ToString());
    }

    [TestMethod]
    public void AddSingle_Null_ThrowsArgumentNull() {
        var writer = new StringBuilderTraceWriter();
        Assert.ThrowsExactly<ArgumentNullException>(() => writer.AddSingle(null!));
    }

    [TestMethod]
    public void AddBlock_IndentsUntilDisposed() {
        var writer = new StringBuilderTraceWriter();
        writer.AddSingle("top");
        using(writer.AddBlock("block")) {
            writer.AddSingle("inner");
        }

        writer.AddSingle("after");

        var expected = string.Join(Environment.NewLine, "  top", "  block", "  {", "    inner", "  }", "  after") + Environment.NewLine;
        Assert.AreEqual(expected, writer.ToString());
    }

    [TestMethod]
    public void Clear_RemovesRecordedLines() {
        var writer = new StringBuilderTraceWriter();
        writer.AddSingle("first");
        writer.Clear();
        writer.AddSingle("second");

        var expected = "  second" + Environment.NewLine;
        Assert.AreEqual(expected, writer.ToString());
    }
}

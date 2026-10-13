//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Equivalency;

/// <summary>Receives equivalency diagnostics lines while a comparison runs.</summary>
/// <remarks>Attach with <c>WithTracing</c>; tracing is off unless a writer is attached.</remarks>
public interface ITraceWriter {
    /// <summary>Writes a single diagnostics line.</summary>
    /// <param name="trace">The message to record.</param>
    void AddSingle(string trace);

    /// <summary>Starts a scoped diagnostics block; lines written until the returned handle is disposed are indented one level.</summary>
    /// <param name="trace">The block title to record.</param>
    /// <returns>A handle that closes the block when disposed.</returns>
    IDisposable AddBlock(string trace);

    /// <summary>Returns a copy of the recorded trace.</summary>
    /// <returns>The recorded trace.</returns>
    string ToString();
}

/// <summary>An <see cref="ITraceWriter"/> accumulating lines in memory for later inspection.</summary>
/// <remarks>Every recorded line is indented two spaces per nesting level.</remarks>
public sealed class StringBuilderTraceWriter : ITraceWriter {
    private readonly StringBuilder _builder = new();
    private readonly object _sync = new();
    private int _depth = 1;

    /// <inheritdoc/>
    public void AddSingle(string trace) {
        if(trace is null) {
            throw new ArgumentNullException(nameof(trace));
        }

        lock(_sync) {
            WriteLine(trace);
        }
    }

    /// <inheritdoc/>
    public IDisposable AddBlock(string trace) {
        if(trace is null) {
            throw new ArgumentNullException(nameof(trace));
        }

        lock(_sync) {
            WriteLine(trace);
            WriteLine("{");
        }

        _depth++;
        return new Block(this);
    }

    /// <summary>Removes all recorded lines.</summary>
    public void Clear() {
        lock(_sync) {
            _builder.Clear();
        }
    }

    /// <inheritdoc/>
    public override string ToString() {
        lock(_sync) {
            return _builder.ToString();
        }
    }

    /// <summary>Appends a trace message, indenting each line by the current block depth.</summary>
    /// <param name="trace">The trace text (may contain line breaks).</param>
    private void WriteLine(string trace) {
        var separators = new[] { Environment.NewLine, "\n" };
        foreach(var traceLine in trace.Split(separators, StringSplitOptions.RemoveEmptyEntries)) {
            _builder.Append(' ', _depth * 2).AppendLine(traceLine);
        }
    }

    /// <summary>Closes the current trace block, decrementing the depth and emitting a closing brace.</summary>
    private void ExitBlock() {
        lock(_sync) {
            _depth--;
            WriteLine("}");
        }
    }

    private sealed class Block : IDisposable {
        private readonly StringBuilderTraceWriter _writer;
        private bool _disposed;

        internal Block(StringBuilderTraceWriter writer) => _writer = writer;

        public void Dispose() {
            if(_disposed) {
                return;
            }

            _disposed = true;
            _writer.ExitBlock();
        }
    }
}

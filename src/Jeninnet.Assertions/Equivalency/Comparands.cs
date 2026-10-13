//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Equivalency;

/// <summary>The pair of values a custom <see cref="IEquivalencyStep"/> is asked to compare.</summary>
/// <remarks>Created by the engine for every compared node, from the root down to each member, key, and index.</remarks>
public sealed class Comparands {
    private readonly string[] _path;

    /// <summary>Creates the compared pair for one graph node.</summary>
    /// <param name="subject">The actual value.</param>
    /// <param name="expectation">The expected value.</param>
    /// <param name="path">The member path segments from the root (empty at the root).</param>
    /// <exception cref="ArgumentNullException"><paramref name="path"/> is <see langword="null"/>.</exception>
    public Comparands(object? subject, object? expectation, IReadOnlyList<string> path) {
        if(path is null) {
            throw new ArgumentNullException(nameof(path));
        }

        Subject = subject;
        Expectation = expectation;
        _path = [.. path];
    }

    /// <summary>Gets the actual value.</summary>
    public object? Subject { get; }

    /// <summary>Gets the expected value.</summary>
    public object? Expectation { get; }

    /// <summary>Gets the runtime type of the subject, or <see langword="null"/> when it is <see langword="null"/>.</summary>
    public Type? SubjectType => Subject?.GetType();

    /// <summary>Gets the runtime type of the expectation, or <see langword="null"/> when it is <see langword="null"/>.</summary>
    public Type? ExpectationType => Expectation?.GetType();

    /// <summary>Gets the member path segments from the root (empty at the root).</summary>
    public IReadOnlyList<string> Path => _path;

    /// <summary>Gets the dotted/bracketed path, or <c>&lt;root&gt;</c> at the root.</summary>
    /// <returns>The formatted path.</returns>
    public string FormattedPath => _path.Length == 0 ? "<root>" : MemberPath.Format(_path);
}

//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Snapshots;

/// <summary>Configures snapshot matching (explicit, opt-in text transformations).</summary>
/// <remarks>Scrubbers run on the received text before comparison and before writing the <c>.received</c>
/// file, so committed <c>.verified</c> files hold scrubbed content. Nothing is transformed by default.</remarks>
public sealed class SnapshotOptions {
    private readonly List<Func<string, string>> _scrubbers = [];

    /// <summary>Adds a transformation applied to received text (e.g. masking dates or identifiers).</summary>
    /// <param name="scrubber">The transformation (must not be <see langword="null"/>).</param>
    /// <returns>This instance for chaining.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="scrubber"/> is <see langword="null"/>.</exception>
    public SnapshotOptions Scrub(Func<string, string> scrubber) {
        if(scrubber is null) {
            throw new ArgumentNullException(nameof(scrubber));
        }

        _scrubbers.Add(scrubber);
        return this;
    }

    /// <summary>Gets the registered transformations in registration order.</summary>
    internal IReadOnlyList<Func<string, string>> Scrubbers => _scrubbers;
}

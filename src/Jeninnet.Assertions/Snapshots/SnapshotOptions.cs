//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Snapshots;

/// <summary>Configures snapshot matching (explicit, opt-in naming, normalization, and review behavior).</summary>
/// <remarks>
/// Every option is opt-in. Scrubbers run before comparison and before writing <c>.received</c>, so committed
/// <c>.verified</c> files hold scrubbed content. Naming options compose in this order: type, method, base name,
/// parameter text, then unique suffixes. Nothing is transformed or auto-accepted by default.
/// </remarks>
public sealed class SnapshotOptions {
    private static readonly Regex _guidPattern = new(
        "[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}",
        RegexOptions.CultureInvariant,
        TimeSpan.FromSeconds(1));

    private readonly List<Func<string, string>> _scrubbers = [];
    private readonly List<(Type Type, Func<string, string> Scrubber)> _typeScrubbers = [];
    private readonly List<(string Member, Func<string, string> Scrubber)> _memberScrubbers = [];
    private readonly List<string[]> _ignoredJsonPaths = [];
    private readonly HashSet<string> _ignoredMemberNames = new(StringComparer.Ordinal); // NOSONAR: a collection expression would drop the ordinal comparer.

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

    /// <summary>Adds a transformation applied to received text (alias of <see cref="Scrub"/>).</summary>
    /// <param name="scrubber">The transformation (must not be <see langword="null"/>).</param>
    /// <returns>This instance for chaining.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="scrubber"/> is <see langword="null"/>.</exception>
    public SnapshotOptions AddScrubber(Func<string, string> scrubber) => Scrub(scrubber);

    /// <summary>Replaces every occurrence of <see cref="Environment.MachineName"/> with <c>&lt;machine&gt;</c>.</summary>
    /// <remarks>Convenience scrubbers run after explicitly registered scrubbers, in the order machine, user, GUID.</remarks>
    /// <param name="enabled">Whether the scrubber is enabled.</param>
    /// <returns>This instance for chaining.</returns>
    public SnapshotOptions ScrubMachineName(bool enabled = true) {
        ScrubMachineNameEnabled = enabled;
        return this;
    }

    /// <summary>Replaces every occurrence of <see cref="Environment.UserName"/> with <c>&lt;user&gt;</c>.</summary>
    /// <remarks>Convenience scrubbers run after explicitly registered scrubbers, in the order machine, user, GUID.</remarks>
    /// <param name="enabled">Whether the scrubber is enabled.</param>
    /// <returns>This instance for chaining.</returns>
    public SnapshotOptions ScrubUserName(bool enabled = true) {
        ScrubUserNameEnabled = enabled;
        return this;
    }

    /// <summary>Replaces every hyphenated GUID with <c>&lt;guid&gt;</c>.</summary>
    /// <remarks>Convenience scrubbers run after explicitly registered scrubbers, in the order machine, user, GUID.</remarks>
    /// <param name="enabled">Whether the scrubber is enabled.</param>
    /// <returns>This instance for chaining.</returns>
    public SnapshotOptions ScrubGuid(bool enabled = true) {
        ScrubGuidEnabled = enabled;
        return this;
    }

    /// <summary>Adds a scrubber applied when the snapshotted object is an instance of <paramref name="type"/>.</summary>
    /// <remarks>The scrubber runs on the serialized text; it is not a per-node graph transform.</remarks>
    /// <param name="type">The root runtime type that activates the scrubber.</param>
    /// <param name="scrubber">The transformation (must not be <see langword="null"/>).</param>
    /// <returns>This instance for chaining.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="type"/> or <paramref name="scrubber"/> is <see langword="null"/>.</exception>
    public SnapshotOptions AddScrubberByType(Type type, Func<string, string> scrubber) {
        if(type is null) {
            throw new ArgumentNullException(nameof(type));
        }

        if(scrubber is null) {
            throw new ArgumentNullException(nameof(scrubber));
        }

        _typeScrubbers.Add((type, scrubber));
        return this;
    }

    /// <summary>Adds a scrubber applied when the snapshotted object exposes a member named <paramref name="memberName"/>.</summary>
    /// <remarks>The scrubber runs on the serialized text; it is not a per-member graph transform.</remarks>
    /// <param name="memberName">The member name that activates the scrubber.</param>
    /// <param name="scrubber">The transformation (must not be <see langword="null"/>).</param>
    /// <returns>This instance for chaining.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="memberName"/> or <paramref name="scrubber"/> is <see langword="null"/>.</exception>
    public SnapshotOptions AddScrubberByMember(string memberName, Func<string, string> scrubber) {
        if(memberName is null) {
            throw new ArgumentNullException(nameof(memberName));
        }

        if(scrubber is null) {
            throw new ArgumentNullException(nameof(scrubber));
        }

        _memberScrubbers.Add((memberName, scrubber));
        return this;
    }

    /// <summary>Uses a relative subdirectory under the resolved snapshots directory.</summary>
    /// <param name="directory">The relative subdirectory, or <see langword="null"/> to clear it.</param>
    /// <returns>This instance for chaining.</returns>
    public SnapshotOptions UseDirectory(string? directory) {
        Directory = directory;
        return this;
    }

    /// <summary>Overrides the snapshot base file name; it must remain a bare file name.</summary>
    /// <param name="fileName">The base file name, or <see langword="null"/> to use the subject expression.</param>
    /// <returns>This instance for chaining.</returns>
    public SnapshotOptions UseFileName(string? fileName) {
        FileName = fileName;
        return this;
    }

    /// <summary>Prefixes the snapshot name with the calling test method name.</summary>
    /// <param name="enabled">Whether the prefix is enabled.</param>
    /// <returns>This instance for chaining.</returns>
    public SnapshotOptions UseMethodName(bool enabled = true) {
        UseMethodNameEnabled = enabled;
        return this;
    }

    /// <summary>Prefixes the snapshot name with the calling test type name.</summary>
    /// <param name="enabled">Whether the prefix is enabled.</param>
    /// <returns>This instance for chaining.</returns>
    public SnapshotOptions UseTypeName(bool enabled = true) {
        UseTypeNameEnabled = enabled;
        return this;
    }

    /// <summary>Includes the explicit <see cref="UseTextForParameters"/> text in the snapshot name.</summary>
    /// <remarks>Without a test-framework dependency the library cannot infer test parameters; supply their text explicitly.</remarks>
    /// <param name="enabled">Whether the parameter text is appended.</param>
    /// <returns>This instance for chaining.</returns>
    public SnapshotOptions UseParameters(bool enabled = true) {
        UseParametersEnabled = enabled;
        return this;
    }

    /// <summary>Sets the text appended when parameter naming is enabled (for example, <c>with-discount</c>).</summary>
    /// <param name="text">The parameter text, or <see langword="null"/> to append nothing.</param>
    /// <returns>This instance for chaining.</returns>
    public SnapshotOptions UseTextForParameters(string? text) {
        ParametersText = text;
        return this;
    }

    /// <summary>Adds the caller assembly name suffix.</summary>
    /// <param name="enabled">Whether the suffix is enabled.</param>
    /// <returns>This instance for chaining.</returns>
    public SnapshotOptions UniqueForAssembly(bool enabled = true) {
        UniqueForAssemblyEnabled = enabled;
        return this;
    }

    /// <summary>Adds a stable OS suffix (<c>_win</c>, <c>_linux</c>, or <c>_macos</c>).</summary>
    /// <param name="enabled">Whether the suffix is enabled.</param>
    /// <returns>This instance for chaining.</returns>
    public SnapshotOptions UniqueForOSPlatform(bool enabled = true) {
        UniqueForOSPlatformEnabled = enabled;
        return this;
    }

    /// <summary>Adds a stable runtime suffix derived from <see cref="RuntimeInformation.FrameworkDescription"/>.</summary>
    /// <param name="enabled">Whether the suffix is enabled.</param>
    /// <returns>This instance for chaining.</returns>
    public SnapshotOptions UniqueForRuntime(bool enabled = true) {
        UniqueForRuntimeEnabled = enabled;
        return this;
    }

    /// <summary>Adds the target framework suffix (<c>_net6.0</c>, <c>_net8.0</c>, <c>_net10.0</c>, <c>_net11.0</c>, or <c>_netstandard2.0</c>).</summary>
    /// <param name="enabled">Whether the suffix is enabled.</param>
    /// <returns>This instance for chaining.</returns>
    public SnapshotOptions UniqueForTargetFramework(bool enabled = true) {
        UniqueForTargetFrameworkEnabled = enabled;
        return this;
    }

    /// <summary>Adds a stable process architecture suffix (<c>_x64</c>, <c>_x86</c>, <c>_arm64</c>, or <c>_arm</c>).</summary>
    /// <param name="enabled">Whether the suffix is enabled.</param>
    /// <returns>This instance for chaining.</returns>
    public SnapshotOptions UniqueForArchitecture(bool enabled = true) {
        UniqueForArchitectureEnabled = enabled;
        return this;
    }

    /// <summary>Ignores CRLF versus LF differences during comparison.</summary>
    /// <param name="enabled">Whether line-ending differences are ignored.</param>
    /// <returns>This instance for chaining.</returns>
    public SnapshotOptions IgnoreLineEndingDifferences(bool enabled = true) {
        IgnoreLineEndingDifferencesEnabled = enabled;
        return this;
    }

    /// <summary>Collapses whitespace runs during comparison.</summary>
    /// <param name="enabled">Whether whitespace-only differences are ignored.</param>
    /// <returns>This instance for chaining.</returns>
    public SnapshotOptions IgnoreWhiteSpaceDifferences(bool enabled = true) {
        IgnoreWhiteSpaceDifferencesEnabled = enabled;
        return this;
    }

    /// <summary>Sorts object keys recursively for JSON snapshots during comparison.</summary>
    /// <param name="enabled">Whether JSON object keys are sorted.</param>
    /// <returns>This instance for chaining.</returns>
    public SnapshotOptions SortProperties(bool enabled = true) {
        SortPropertiesEnabled = enabled;
        return this;
    }

    /// <summary>Removes a property path from JSON snapshots before scrubbing and comparison.</summary>
    /// <remarks>
    /// Only property paths are supported (for example <c>$.metadata.timestamp</c> or <c>metadata.timestamp</c>);
    /// array indexes and wildcards are rejected. A path that does not exist is a no-op. Invalid JSON is left
    /// unchanged. Projection runs after serialization and before scrubbers.
    /// </remarks>
    /// <param name="jsonPath">The property path to remove.</param>
    /// <returns>This instance for chaining.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="jsonPath"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="jsonPath"/> is blank, targets no property, or contains array syntax.</exception>
    public SnapshotOptions IgnoreJsonProperty(string jsonPath) {
        _ignoredJsonPaths.Add(ParseJsonPath(jsonPath));
        return this;
    }

    /// <summary>Removes every JSON property with one of the given names at any depth.</summary>
    /// <remarks>Names are matched ordinally; paths and nested arrays are traversed. Missing names are a no-op.</remarks>
    /// <param name="names">The property names to remove.</param>
    /// <returns>This instance for chaining.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="names"/> or one of its entries is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="names"/> is empty or contains a blank name.</exception>
    public SnapshotOptions IgnoreMembersNamed(params string[] names) {
        if(names is null) {
            throw new ArgumentNullException(nameof(names));
        }

        if(names.Length == 0) {
            throw new ArgumentException("At least one member name must be provided.", nameof(names));
        }

        foreach(var name in names) {
            if(name is null) {
                throw new ArgumentNullException(nameof(names));
            }

            var trimmed = name.Trim();
            if(trimmed.Length == 0) {
                throw new ArgumentException("Member names must not be empty.", nameof(names));
            }

            _ignoredMemberNames.Add(trimmed);
        }

        return this;
    }

    /// <summary>Resolves the default snapshots directory relative to the nearest project file instead of the caller file.</summary>
    /// <remarks>
    /// When enabled and no explicit snapshots directory is passed, the library searches upward from the caller file
    /// for the first <c>*.csproj</c> and uses <c>&lt;projectDir&gt;/__snapshots__</c>. If no project file is found,
    /// the caller-relative default is used. An explicit directory always wins. Results are cached per caller directory.
    /// </remarks>
    /// <param name="enabled">Whether project-relative resolution is enabled.</param>
    /// <returns>This instance for chaining.</returns>
    public SnapshotOptions UseProjectRelativeDirectory(bool enabled = true) {
        UseProjectRelativeDirectoryEnabled = enabled;
        return this;
    }

    /// <summary>Sets the indentation width used for JSON/XML snapshot content.</summary>
    /// <param name="spaces">The number of leading spaces per level (zero or greater).</param>
    /// <returns>This instance for chaining.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="spaces"/> is negative.</exception>
    public SnapshotOptions IndentSize(int spaces) {
        if(spaces < 0) {
            throw new ArgumentOutOfRangeException(nameof(spaces), "Indent size must not be negative.");
        }

        IndentSizeValue = spaces;
        return this;
    }

    /// <summary>Accepts the received content automatically for this snapshot only when explicitly enabled.</summary>
    /// <remarks>This is per-call convenience equivalent to the process-wide <c>JENINNET_SNAPSHOT_UPDATE=1</c>; it is never on by default.</remarks>
    /// <param name="enabled">Whether automatic acceptance is enabled.</param>
    /// <returns>This instance for chaining.</returns>
    public SnapshotOptions AutoVerify(bool enabled = true) {
        AutoVerifyEnabled = enabled;
        return this;
    }

    /// <summary>Starts the given diff executable on mismatch (verified and received paths are passed as arguments).</summary>
    /// <remarks>Nothing is launched unless this option is set. Launch failures never hide the assertion failure.</remarks>
    /// <param name="executablePath">The diff tool executable path, or <see langword="null"/> to disable it.</param>
    /// <returns>This instance for chaining.</returns>
    public SnapshotOptions DiffTool(string? executablePath) {
        DiffToolPath = executablePath;
        return this;
    }

    /// <summary>Suppresses the unified diff in mismatch messages.</summary>
    /// <param name="enabled">Whether the diff is suppressed.</param>
    /// <returns>This instance for chaining.</returns>
    public SnapshotOptions DisableDiff(bool enabled = true) {
        DisableDiffEnabled = enabled;
        return this;
    }

    /// <summary>Gets the registered global transformations in registration order.</summary>
    internal IReadOnlyList<Func<string, string>> Scrubbers => _scrubbers;

    /// <summary>Gets whether the machine-name convenience scrubber is enabled.</summary>
    internal bool ScrubMachineNameEnabled { get; private set; }

    /// <summary>Gets whether the user-name convenience scrubber is enabled.</summary>
    internal bool ScrubUserNameEnabled { get; private set; }

    /// <summary>Gets whether the GUID convenience scrubber is enabled.</summary>
    internal bool ScrubGuidEnabled { get; private set; }

    /// <summary>Gets the configured JSON property paths to remove.</summary>
    internal IReadOnlyList<string[]> IgnoredJsonPaths => _ignoredJsonPaths;

    /// <summary>Gets the JSON property names to remove at any depth.</summary>
    internal IReadOnlyCollection<string> IgnoredMemberNames => _ignoredMemberNames;

    /// <summary>Gets whether the default snapshots directory is resolved relative to the nearest project file.</summary>
    internal bool UseProjectRelativeDirectoryEnabled { get; private set; }

    /// <summary>Gets the type-activated transformations.</summary>
    internal IReadOnlyList<(Type Type, Func<string, string> Scrubber)> TypeScrubbers => _typeScrubbers;

    /// <summary>Gets the member-activated transformations.</summary>
    internal IReadOnlyList<(string Member, Func<string, string> Scrubber)> MemberScrubbers => _memberScrubbers;

    /// <summary>Gets the relative snapshot subdirectory, when configured.</summary>
    internal string? Directory { get; private set; }

    /// <summary>Gets the explicit base file name, when configured.</summary>
    internal string? FileName { get; private set; }

    /// <summary>Gets whether the caller method name prefixes the snapshot name.</summary>
    internal bool UseMethodNameEnabled { get; private set; }

    /// <summary>Gets whether the caller type name prefixes the snapshot name.</summary>
    internal bool UseTypeNameEnabled { get; private set; }

    /// <summary>Gets whether explicit parameter text is appended to the snapshot name.</summary>
    internal bool UseParametersEnabled { get; private set; }

    /// <summary>Gets the explicit parameter text.</summary>
    internal string? ParametersText { get; private set; }

    /// <summary>Gets whether the caller assembly suffix is enabled.</summary>
    internal bool UniqueForAssemblyEnabled { get; private set; }

    /// <summary>Gets whether the OS suffix is enabled.</summary>
    internal bool UniqueForOSPlatformEnabled { get; private set; }

    /// <summary>Gets whether the runtime suffix is enabled.</summary>
    internal bool UniqueForRuntimeEnabled { get; private set; }

    /// <summary>Gets whether the target-framework suffix is enabled.</summary>
    internal bool UniqueForTargetFrameworkEnabled { get; private set; }

    /// <summary>Gets whether the architecture suffix is enabled.</summary>
    internal bool UniqueForArchitectureEnabled { get; private set; }

    /// <summary>Gets whether CRLF/LF differences are ignored.</summary>
    internal bool IgnoreLineEndingDifferencesEnabled { get; private set; }

    /// <summary>Gets whether whitespace runs are collapsed for comparison.</summary>
    internal bool IgnoreWhiteSpaceDifferencesEnabled { get; private set; }

    /// <summary>Gets whether JSON object keys are sorted.</summary>
    internal bool SortPropertiesEnabled { get; private set; }

    /// <summary>Gets the configured indentation width.</summary>
    internal int IndentSizeValue { get; private set; } = 2;

    /// <summary>Gets whether this call auto-accepts the received content.</summary>
    internal bool AutoVerifyEnabled { get; private set; }

    /// <summary>Gets the configured diff tool path.</summary>
    internal string? DiffToolPath { get; private set; }

    /// <summary>Gets whether the unified diff is suppressed.</summary>
    internal bool DisableDiffEnabled { get; private set; }

    /// <summary>Replaces every hyphenated GUID with <c>&lt;guid&gt;</c>.</summary>
    /// <param name="text">The text to scrub.</param>
    /// <returns>The scrubbed text.</returns>
    internal static string ScrubGuids(string text) => _guidPattern.Replace(text, "<guid>");

    /// <summary>Parses a property-only JSON path into its segments.</summary>
    /// <param name="jsonPath">The path to parse.</param>
    /// <returns>The property segments.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="jsonPath"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="jsonPath"/> is blank, targets no property, or contains array syntax.</exception>
    private static string[] ParseJsonPath(string jsonPath) {
        if(jsonPath is null) {
            throw new ArgumentNullException(nameof(jsonPath));
        }

        var trimmed = jsonPath.Trim();
        if(trimmed.Length > 0 && trimmed[0] == '$') {
            trimmed = trimmed.Substring(1).Trim();
            if(trimmed.Length > 0 && trimmed[0] == '.') {
                trimmed = trimmed.Substring(1).Trim();
            }
        }

        if(trimmed.Length == 0) {
            throw new ArgumentException("The JSON path must target at least one property.", nameof(jsonPath));
        }

        if(trimmed.IndexOf('[') >= 0 || trimmed.IndexOf(']') >= 0) {
            throw new ArgumentException("Only property paths are supported; array indexes and wildcards are not.", nameof(jsonPath));
        }

        var segments = trimmed.Split('.').Select(static segment => segment.Trim()).ToArray();
        if(segments.Any(static segment => segment.Length == 0)) {
            throw new ArgumentException("The JSON path contains an empty property segment.", nameof(jsonPath));
        }

        return segments;
    }
}

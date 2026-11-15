//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Snapshots;

/// <summary>Resolves snapshot file names and directories from caller context and options.</summary>
internal static class SnapshotNaming {
    private static readonly Dictionary<string, string> _projectDirectories = new(PathComparer);
    private static readonly object _projectDirectoriesLock = new();
    /// <summary>Resolves the snapshot base name and appends configured prefixes/suffixes.</summary>
    /// <param name="snapshotName">The explicit name supplied to the assertion, when any.</param>
    /// <param name="options">The snapshot options.</param>
    /// <param name="context">The caller context.</param>
    /// <returns>The resolved bare snapshot name.</returns>
    /// <exception cref="ArgumentException">The resolved name is empty or unsafe.</exception>
    internal static string ResolveName(string? snapshotName, SnapshotOptions options, SnapshotContext context) {
        var caller = FindCaller();
        var baseName = options.FileName ?? snapshotName ?? Sanitize(context.Expression);
        ValidateName(baseName);

        var builder = new StringBuilder();
        if(options.UseTypeNameEnabled && caller.TypeName is not null) {
            builder.Append(SanitizeFragment(caller.TypeName)).Append('_');
        }

        if(options.UseMethodNameEnabled && caller.MethodName is not null) {
            builder.Append(SanitizeFragment(caller.MethodName)).Append('_');
        }

        builder.Append(baseName);
        if(options.UseParametersEnabled && options.ParametersText is { Length: > 0 } parameters) {
            builder.Append('_').Append(SanitizeFragment(parameters));
        }

        AppendUniqueSuffixes(builder, options, caller);
        var resolved = builder.ToString();
        ValidateName(resolved);
        return resolved;
    }

    /// <summary>Resolves the snapshots directory, applying a contained relative subdirectory when configured.</summary>
    /// <param name="snapshotsDirectory">The explicit directory supplied to the assertion, when any.</param>
    /// <param name="callerFilePath">The caller source file, used for the default <c>__snapshots__</c> folder.</param>
    /// <param name="options">The snapshot options.</param>
    /// <returns>The resolved directory.</returns>
    /// <exception cref="ArgumentException">The caller path is unavailable or the configured subdirectory escapes its base.</exception>
    internal static string ResolveDirectory(string? snapshotsDirectory, string? callerFilePath, SnapshotOptions options) {
        var baseDirectory = ResolveBaseDirectory(snapshotsDirectory, callerFilePath, options);
        if(options.Directory is null) {
            return baseDirectory;
        }

        if(Path.IsPathRooted(options.Directory)) {
            throw new ArgumentException("The snapshot directory must be relative.", nameof(options));
        }

        var fullBase = Path.GetFullPath(baseDirectory);
        var combined = Path.GetFullPath(Path.Combine(fullBase, options.Directory));
        var comparison = PathComparison;
        var prefix = fullBase.EndsWith(Path.DirectorySeparatorChar.ToString(), StringComparison.Ordinal)
            ? fullBase
            : fullBase + Path.DirectorySeparatorChar;
        if(!string.Equals(combined, fullBase, comparison) && !combined.StartsWith(prefix, comparison)) {
            throw new ArgumentException("The snapshot directory must stay inside its base directory.", nameof(options));
        }

        return combined;
    }

    /// <summary>Resolves the base directory: the explicit value, the nearest project folder, or <c>__snapshots__</c> beside the caller file.</summary>
    /// <param name="snapshotsDirectory">The explicit directory, when supplied.</param>
    /// <param name="callerFilePath">The caller source file.</param>
    /// <param name="options">The snapshot options.</param>
    /// <returns>The base directory.</returns>
    /// <exception cref="ArgumentException"><paramref name="snapshotsDirectory"/> is absent and the caller path is unavailable.</exception>
    private static string ResolveBaseDirectory(string? snapshotsDirectory, string? callerFilePath, SnapshotOptions options) {
        if(snapshotsDirectory is not null) {
            return snapshotsDirectory;
        }

        var baseDirectory = !string.IsNullOrEmpty(callerFilePath) ? Path.GetDirectoryName(callerFilePath) : null;
        if(baseDirectory is null || baseDirectory.Length == 0) {
            throw new ArgumentException("Pass snapshotsDirectory explicitly when the caller file path is unavailable.", nameof(callerFilePath));
        }

        if(options.UseProjectRelativeDirectoryEnabled) {
            var projectDirectory = FindProjectDirectory(baseDirectory);
            if(projectDirectory is not null) {
                return Path.Combine(projectDirectory, "__snapshots__");
            }
        }

        return Path.Combine(baseDirectory, "__snapshots__");
    }

    /// <summary>Finds the nearest directory containing a C# project file, caching the result per start directory.</summary>
    /// <param name="startDirectory">The directory to start the upward search from.</param>
    /// <returns>The project directory, or <see langword="null"/> when none was found.</returns>
    private static string? FindProjectDirectory(string startDirectory) {
        var start = Path.GetFullPath(startDirectory);
        lock(_projectDirectoriesLock) {
            if(_projectDirectories.TryGetValue(start, out var cached)) {
                return cached.Length == 0 ? null : cached;
            }

            var directory = new DirectoryInfo(start);
            while(directory is not null) {
                try {
                    if(directory.EnumerateFiles("*.csproj", SearchOption.TopDirectoryOnly).Any()) {
                        _projectDirectories[start] = directory.FullName;
                        return directory.FullName;
                    }
                }
                catch(IOException) {
                    // An unreadable directory cannot contain the project; keep walking upward.
                }
                catch(UnauthorizedAccessException) {
                    // The same fallback applies when the search hits an inaccessible directory.
                }

                directory = directory.Parent;
            }

            _projectDirectories[start] = string.Empty;
            return null;
        }
    }

    /// <summary>Validates a caller-supplied snapshot name as a bare, valid file name.</summary>
    /// <param name="snapshotName">The snapshot name to validate.</param>
    /// <exception cref="ArgumentException">The name is empty, a parent reference, or contains path characters.</exception>
    internal static void ValidateName(string snapshotName) {
        if(snapshotName.Length == 0) {
            throw new ArgumentException("Snapshot name must not be empty.", nameof(snapshotName));
        }

        if(snapshotName == ".."
            || snapshotName.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0
            || snapshotName.IndexOf('/') >= 0
            || snapshotName.IndexOf('\\') >= 0) {
            throw new ArgumentException($"Snapshot name '{snapshotName}' must be a bare file name.", nameof(snapshotName));
        }
    }

    /// <summary>Converts arbitrary text into a safe file-name fragment.</summary>
    /// <param name="text">The text to sanitize.</param>
    /// <returns>The text with path characters replaced by underscores; <c>snapshot</c> when nothing remains.</returns>
    internal static string Sanitize(string text) {
        var invalid = Path.GetInvalidFileNameChars();
        var builder = new StringBuilder(text.Length);
        foreach(var character in text) {
            var replacement = character == '/' || character == '\\' || Array.IndexOf(invalid, character) >= 0 || char.IsWhiteSpace(character)
                ? '_'
                : character;
            builder.Append(replacement);
        }

        var sanitized = builder.ToString().Trim();
        return sanitized.Length == 0 ? "snapshot" : sanitized;
    }

    /// <summary>Converts a naming fragment into a compact file-name fragment, replacing whitespace with underscores.</summary>
    /// <param name="text">The fragment to sanitize.</param>
    /// <returns>The sanitized fragment.</returns>
    private static string SanitizeFragment(string text) {
        var sanitized = Sanitize(text);
        var builder = new StringBuilder(sanitized.Length);
        foreach(var character in sanitized) {
            builder.Append(char.IsWhiteSpace(character) ? '_' : character);
        }

        return builder.ToString();
    }

    /// <summary>Appends configured unique suffixes in a fixed order.</summary>
    /// <param name="builder">The name builder.</param>
    /// <param name="options">The snapshot options.</param>
    /// <param name="caller">The caller identity.</param>
    private static void AppendUniqueSuffixes(StringBuilder builder, SnapshotOptions options, (string? TypeName, string? MethodName, string? AssemblyName) caller) {
        if(options.UniqueForAssemblyEnabled) {
            var assembly = caller.AssemblyName ?? Assembly.GetEntryAssembly()?.GetName().Name ?? "assembly";
            builder.Append('_').Append(SanitizeFragment(assembly).ToLowerInvariant());
        }

        if(options.UniqueForOSPlatformEnabled) {
            builder.Append('_').Append(OperatingSystemSuffix());
        }

        if(options.UniqueForRuntimeEnabled) {
            builder.Append('_').Append(RuntimeSuffix());
        }

        if(options.UniqueForTargetFrameworkEnabled) {
            builder.Append('_').Append(TargetFrameworkSuffix());
        }

        if(options.UniqueForArchitectureEnabled) {
            builder.Append('_').Append(ArchitectureSuffix());
        }
    }

    /// <summary>Finds the first stack frame outside the assertion assembly.</summary>
    /// <returns>The caller type/method/assembly names, or empty values when unavailable.</returns>
    private static (string? TypeName, string? MethodName, string? AssemblyName) FindCaller() {
        var assertionAssembly = typeof(SnapshotNaming).Assembly;
        foreach(var frame in new StackTrace().GetFrames()) {
            var method = frame.GetMethod();
            var type = method?.DeclaringType;
            if(type is null || type.Assembly == assertionAssembly) {
                continue;
            }

            var typeName = type.Name;
            var genericMarker = typeName.IndexOf('`');
            if(genericMarker >= 0) {
                typeName = typeName.Substring(0, genericMarker);
            }

            return (typeName, method!.Name, type.Assembly.GetName().Name);
        }

        return (null, null, null);
    }

    /// <summary>Gets the platform comparison for paths.</summary>
    private static StringComparison PathComparison =>
        RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;

    /// <summary>Gets the platform comparer for cached project paths.</summary>
    private static StringComparer PathComparer =>
        RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal;

    /// <summary>Returns the stable OS suffix.</summary>
    /// <returns><c>win</c>, <c>linux</c>, <c>macos</c>, or a sanitized description.</returns>
    private static string OperatingSystemSuffix() {
        if(RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
            return "win";
        }

        if(RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) {
            return "linux";
        }

        if(RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) {
            return "macos";
        }

        return SanitizeFragment(RuntimeInformation.OSDescription).ToLowerInvariant();
    }

    /// <summary>Returns the stable runtime suffix derived from the framework description.</summary>
    /// <returns>A lower-case suffix such as <c>net10.0.0</c>.</returns>
    private static string RuntimeSuffix() {
        var description = RuntimeInformation.FrameworkDescription;
        if(description.StartsWith(".NET Framework", StringComparison.Ordinal)) {
            return SanitizeFragment("netfx" + description.Substring(".NET Framework".Length).Trim()).ToLowerInvariant();
        }

        if(description.StartsWith(".NET", StringComparison.Ordinal)) {
            return SanitizeFragment("net" + description.Substring(".NET".Length).Trim()).ToLowerInvariant();
        }

        return description.StartsWith("Mono", StringComparison.Ordinal)
            ? SanitizeFragment("mono" + description.Substring("Mono".Length).Trim()).ToLowerInvariant()
            : SanitizeFragment(description).ToLowerInvariant();
    }

    /// <summary>Returns the compile-time target framework suffix.</summary>
    /// <returns><c>netstandard2.0</c>, <c>net6.0</c>, <c>net8.0</c>, or <c>net10.0</c>.</returns>
    private static string TargetFrameworkSuffix() {
#if NET10_0
        return "net10.0";
#elif NET8_0
        return "net8.0";
#elif NET6_0
        return "net6.0";
#else
        return "netstandard2.0";
#endif
    }

    /// <summary>Returns the process architecture suffix.</summary>
    /// <returns><c>x64</c>, <c>x86</c>, <c>arm64</c>, <c>arm</c>, or a sanitized architecture name.</returns>
    private static string ArchitectureSuffix() =>
        RuntimeInformation.ProcessArchitecture switch {
            Architecture.X64 => "x64",
            Architecture.X86 => "x86",
            Architecture.Arm64 => "arm64",
            Architecture.Arm => "arm",
            _ => SanitizeFragment(RuntimeInformation.ProcessArchitecture.ToString()).ToLowerInvariant(),
        };
}

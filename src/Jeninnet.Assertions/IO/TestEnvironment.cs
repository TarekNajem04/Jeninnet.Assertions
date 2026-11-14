//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.IO;

/// <summary>
/// Represents a temporary test environment for file system operations, providing methods to create files and directories, including inaccessible ones, and ensuring cleanup after tests.
/// </summary>
/// <remarks>
/// Every path accepted by this type is relative to <see cref="Root"/> and is normalized before use.
/// Absolute paths, and relative paths that traverse outside the root, are rejected with
/// <see cref="ArgumentException"/> before any file-system operation. Path comparison is
/// case-insensitive on Windows and ordinal elsewhere (conservative on case-insensitive
/// non-Windows volumes). Symbolic-link escapes are outside this check and are not resolved here.
/// </remarks>
public sealed class TestEnvironment : IDisposable {
    private readonly List<string> _createdFiles = [];
    private readonly List<string> _createdDirs = [];

    /// <summary>
    /// Gets the root directory of the test environment, which is a temporary directory created for testing purposes.
    /// </summary>
    public string Root { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="TestEnvironment"/> class, creating a temporary root directory for testing purposes.
    /// </summary>
    public TestEnvironment() {
        Root = Path.GetFullPath(Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("n")));
        Directory.CreateDirectory(Root);
        _createdDirs.Add(Root);
    }

    /// <summary>Resolves a relative path below the environment root and rejects escapes.</summary>
    /// <param name="relativePath">The relative path to resolve.</param>
    /// <param name="parameterName">The caller parameter name used in guard exceptions.</param>
    /// <returns>The normalized absolute path below <see cref="Root"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="relativePath"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="relativePath"/> is absolute or escapes the root.</exception>
    private string ResolveInsideRoot(string relativePath, string parameterName) {
        if(relativePath is null) {
            throw new ArgumentNullException(parameterName);
        }

        if(Path.IsPathRooted(relativePath)) {
            throw new ArgumentException("Path must be relative to the test environment root.", parameterName);
        }

        var root = Path.GetFullPath(Root);
        var fullPath = Path.GetFullPath(Path.Combine(root, relativePath));
        var comparison = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
            ? StringComparison.OrdinalIgnoreCase
            : StringComparison.Ordinal;
        var rootPrefix = root[root.Length - 1] == Path.DirectorySeparatorChar
            ? root
            : root + Path.DirectorySeparatorChar;

        if(!string.Equals(fullPath, root, comparison) && !fullPath.StartsWith(rootPrefix, comparison)) {
            throw new ArgumentException("Path must stay inside the test environment root.", parameterName);
        }

        return fullPath;
    }

    /// <summary>
    /// Creates a directory at the specified relative path within the test environment's root directory.
    /// </summary>
    /// <param name="relative">The relative path of the directory to create.</param>
    /// <returns>Return the full path of the created directory.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="relative"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="relative"/> is absolute or escapes the root.</exception>
    public string CreateDirectory(string relative) {
        var full = ResolveInsideRoot(relative, nameof(relative));
        Directory.CreateDirectory(full);
        _createdDirs.Add(full);
        return full;
    }

    /// <summary>
    /// Creates a file at the specified relative path within the test environment's root directory, optionally with specified contents.
    /// </summary>
    /// <param name="relativePath">The relative path of the file to create.</param>
    /// <param name="contents">The contents to write to the file.</param>
    /// <returns>Return the full path of the created file.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="relativePath"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="relativePath"/> is absolute or escapes the root.</exception>
    public string CreateFile(string relativePath, string? contents = "") {
        var fullPath = ResolveInsideRoot(relativePath, nameof(relativePath));
        // Combined paths always have a directory; the operator is redundant on netstandard2.0
        // (unannotated reference assemblies) while modern targets ignore it.
        Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);
        File.WriteAllText(fullPath, contents ?? "");
        _createdFiles.Add(fullPath);
        return fullPath;
    }

    /// <summary>
    /// Creates multiple files at the specified relative paths within the test environment's root directory, optionally with specified contents.
    /// </summary>
    /// <param name="relativePaths">The relative paths of the files to create.</param>
    /// <exception cref="ArgumentNullException"><paramref name="relativePaths"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">Any path is absolute or escapes the root; no file is created in that case.</exception>
    public void CreateFiles(params string[] relativePaths) {
        if(relativePaths is null) {
            throw new ArgumentNullException(nameof(relativePaths));
        }

        // Validate every path before creating the first file, so an escaping path cannot
        // leave a partially created set behind.
        foreach(var relativePath in relativePaths) {
            _ = ResolveInsideRoot(relativePath, nameof(relativePaths));
        }

        foreach(var relativePath in relativePaths) {
            CreateFile(relativePath);
        }
    }

    /// <summary>
    /// Returns the absolute path by combining the test environment's root directory with the specified relative paths.
    /// </summary>
    /// <param name="relative">The relative path parts to combine with the root directory.</param>
    /// <returns>The normalized absolute path below <see cref="Root"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="relative"/> or one of its parts is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">A part is absolute or the combined path escapes the root.</exception>
    public string Abs(params string[] relative) {
        if(relative is null) {
            throw new ArgumentNullException(nameof(relative));
        }

        if(relative.Any(static part => part is null)) {
            throw new ArgumentNullException(nameof(relative));
        }

        var combined = relative.Length == 0 ? string.Empty : Path.Combine(relative);
        return ResolveInsideRoot(combined, nameof(relative));
    }

    /// <summary>
    /// Creates a directory at the specified relative path and sets it to be inaccessible by setting its attributes to ReadOnly and System.
    /// </summary>
    /// <param name="relativePath">The relative path of the directory to make inaccessible.</param>
    /// <returns>Return the full path of the created directory.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="relativePath"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="relativePath"/> is absolute or escapes the root.</exception>
    public string CreateInaccessibleDirectory(string relativePath) {
        var path = CreateDirectory(relativePath);
        SetInaccessibleDirectory(relativePath);
        return path;
    }

    /// <summary>
    /// Sets the specified directory to be inaccessible by setting its attributes to ReadOnly and System.
    /// </summary>
    /// <param name="relativePath">The relative path of the directory to make inaccessible.</param>
    /// <exception cref="ArgumentNullException"><paramref name="relativePath"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="relativePath"/> is absolute or escapes the root.</exception>
    public void SetInaccessible(string relativePath) {
        var target = ResolveInsideRoot(relativePath, nameof(relativePath));

        if(!Directory.Exists(target)) {
            Directory.CreateDirectory(target);
        }

        try {
            var di = new DirectoryInfo(target);
            di.Attributes |= FileAttributes.ReadOnly | FileAttributes.System;
        }
        catch {
            /* Ignore failures to set attributes, as this is just a test environment setup. */
        }
    }

    /// <summary>
    /// Sets the specified directory to be inaccessible by setting its attributes to ReadOnly and System.
    /// </summary>
    /// <param name="relativePath">The relative path of the directory to make inaccessible.</param>
    /// <exception cref="ArgumentNullException"><paramref name="relativePath"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="relativePath"/> is absolute or escapes the root.</exception>
    public void SetInaccessibleDirectory(string relativePath) {
        var targetPath = ResolveInsideRoot(relativePath, nameof(relativePath));

        if(!Directory.Exists(targetPath)) {
            Directory.CreateDirectory(targetPath);
        }

        if(!RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
            try {
                var process = Process.Start("/usr/bin/chmod", $"000 \"{targetPath}\"");
                process?.WaitForExit();
                if(process?.ExitCode == 0) {
                    return;
                }
            }
            catch {
                /* Ignore failures to set attributes, as this is just a test environment setup. */
            }
        }

        try {
            Directory.Delete(targetPath, recursive: true);
#if !NETSTANDARD2_0
            CreateInaccessibleLink(targetPath);
#endif
        }
        catch {
            try {
                if(!Directory.Exists(targetPath)) {
                    Directory.CreateDirectory(targetPath);
                }

                var di = new DirectoryInfo(targetPath);
                di.Attributes |= FileAttributes.ReadOnly | FileAttributes.System;
            }
            catch {
                /* Ignore failures to set attributes, as this is just a test environment setup. */
            }
        }
    }

#if !NETSTANDARD2_0
    /// <summary>Replaces a directory with a symbolic link pointing at a path that cannot be resolved.</summary>
    /// <param name="targetPath">The directory path to make inaccessible.</param>
    /// <remarks>Excluded on .NET Standard 2.0, where the caller falls back to file attributes instead.</remarks>
    private static void CreateInaccessibleLink(string targetPath) {
        if(RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
            Directory.CreateSymbolicLink(targetPath, @$"{Path.GetPathRoot(targetPath)}\this-path-does-not-exist-xyz");
        } else {
            Directory.CreateSymbolicLink(targetPath, "/nonexistent/path/xyz");
        }
    }
#endif

    /// <summary>
    /// Creates a file at the specified relative path within the test environment's root directory,
    /// opens it with exclusive access (no sharing), and returns a FileStream for the file.
    /// This allows for testing scenarios where the file is locked by another process.
    /// </summary>
    /// <param name="relativePath">The relative path of the file to create.</param>
    /// <param name="contents">The contents to write to the file.</param>
    /// <returns>A FileStream for the created file.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="relativePath"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="relativePath"/> is absolute or escapes the root.</exception>
    public FileStream CreateLockedFile(string relativePath, string? contents = "") {
        var fullPath = ResolveInsideRoot(relativePath, nameof(relativePath));
        // Combined paths always have a directory; the operator is redundant on netstandard2.0
        // (unannotated reference assemblies) while modern targets ignore it.
        Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);

        File.WriteAllText(fullPath, contents ?? "");

        var stream = new FileStream(
            fullPath,
            FileMode.Open,
            FileAccess.ReadWrite,
            FileShare.None
        );

        _createdFiles.Add(fullPath);
        return stream;
    }

    /// <summary>
    /// Creates a directory tree based on the provided dictionary, where keys represent relative paths and values represent file contents.
    /// </summary>
    /// <param name="files">A dictionary where keys are relative paths and values are the contents for each file.</param>
    /// <returns>The root path of the created tree.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="files"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">Any key is absolute or escapes the root; no entry is created in that case.</exception>
    /// <remarks>All keys are validated before the first entry is created. Cleanup is handled by deleting the root recursively.</remarks>
    public string CreateTree(Dictionary<string, string?> files) {
        if(files is null) {
            throw new ArgumentNullException(nameof(files));
        }

        var resolved = new List<KeyValuePair<string, string?>>(files.Count);
        foreach(var pair in files) {
            resolved.Add(new KeyValuePair<string, string?>(ResolveInsideRoot(pair.Key, nameof(files)), pair.Value));
        }

        foreach(var pair in resolved) {
            var full = pair.Key;
            // Combined paths always have a directory; the operator is redundant on netstandard2.0
            // (unannotated reference assemblies) while modern targets ignore it.
            var dir = Path.GetDirectoryName(full)!;
            Directory.CreateDirectory(dir);

            if(pair.Value is not null) {
                File.WriteAllText(full, pair.Value);
            } else {
                Directory.CreateDirectory(full);
            }
        }

        return Root;
    }

    /// <summary>
    /// Asserts that the specified directory is inaccessible by attempting to enumerate its contents.
    /// </summary>
    /// <param name="relativePath">The relative path of the directory to inspect.</param>
    /// <exception cref="ArgumentNullException"><paramref name="relativePath"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="relativePath"/> is absolute or escapes the root.</exception>
    /// <exception cref="InvalidOperationException">The directory is still accessible.</exception>
    public void AssertDirectoryInaccessible(string relativePath) {
        var target = ResolveInsideRoot(relativePath, nameof(relativePath));

        try {
            _ = Directory.EnumerateFileSystemEntries(target).FirstOrDefault();
            throw new InvalidOperationException($"Directory '{target}' is still accessible; test setup failed.");
        }
        catch(IOException) {
            /* Expected exception, directory is inaccessible. */
        }
        catch(UnauthorizedAccessException) {
            /* Expected exception, directory is inaccessible. */
        }
    }

    /// <summary>
    /// Disposes of the test environment by attempting to delete the root directory and all its contents.
    /// </summary>
    public void Dispose() {
        try {
            if(!Directory.Exists(Root)) {
                return;
            }

            try {
                Directory.Delete(Root, recursive: true);
            }
            catch(IOException) {
                ResetAttributesRecursive(Root);
                Directory.Delete(Root, recursive: true);
            }
        }
        catch {
            /* Ignore failures to delete the test environment, as this is just a cleanup operation. */
        }
    }

    /// <summary>Clears read-only and hidden attributes across a directory tree so cleanup can delete it.</summary>
    /// <param name="path">The root path whose entries are reset (best effort; failures are ignored).</param>
    /// <remarks>On non-Windows platforms it also restores <c>755</c> permissions via <c>chmod</c>.</remarks>
    private static void ResetAttributesRecursive(string path) {
        foreach(var entry in Directory.EnumerateFileSystemEntries(path, "*", SearchOption.AllDirectories)) {
            try {
                if(!RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
                    var process = Process.Start("/usr/bin/chmod", $"755 \"{entry}\"");
                    process?.WaitForExit();
                }

                File.SetAttributes(entry, FileAttributes.Normal);
            }
            catch {
                /* Ignore failures to reset attributes, as this is just a cleanup operation. */
            }
        }

        try {
            if(!RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
                var process = Process.Start("/usr/bin/chmod", $"755 \"{path}\"");
                process?.WaitForExit();
            }

            File.SetAttributes(path, FileAttributes.Normal);
        }
        catch {
            /* Ignore failures to reset attributes, as this is just a cleanup operation. */
        }
    }
}

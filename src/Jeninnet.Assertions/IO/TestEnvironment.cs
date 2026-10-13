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
        Root = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("n"));
        Directory.CreateDirectory(Root);
        _createdDirs.Add(Root);
    }

    /// <summary>
    /// Creates a directory at the specified relative path within the test environment's root directory.
    /// </summary>
    /// <param name="relative">The relative path of the directory to create.</param>
    /// <returns>Return the full path of the created directory.</returns>
    public string CreateDirectory(string relative) {
        var full = Path.Combine(Root, relative);
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
    public string CreateFile(string relativePath, string? contents = "") {
        if(relativePath is null) {
            throw new ArgumentNullException(nameof(relativePath));
        }

        var fullPath = Path.Combine(Root, relativePath.Replace('/', Path.DirectorySeparatorChar));
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
    public void CreateFiles(params string[] relativePaths) {
        if(relativePaths is null) {
            throw new ArgumentNullException(nameof(relativePaths));
        }

        foreach(var p in relativePaths) {
            CreateFile(p);
        }
    }

    /// <summary>
    /// Returns the absolute path by combining the test environment's root directory with the specified relative paths.
    /// </summary>
    /// <param name="relative">The relative paths to combine with the root directory.</param>
    /// <returns>The absolute path.</returns>
    public string Abs(params string[] relative) => Path.Combine([Root, .. relative]);

    /// <summary>
    /// Creates a directory at the specified relative path and sets it to be inaccessible by setting its attributes to ReadOnly and System.
    /// </summary>
    /// <param name="relativePath">The relative path of the directory to make inaccessible.</param>
    /// <returns>Return the full path of the created directory.</returns>
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
    public void SetInaccessible(string relativePath) {
        if(relativePath is null) {
            throw new ArgumentNullException(nameof(relativePath));
        }

        var target = Path.Combine(Root, relativePath.Replace('/', Path.DirectorySeparatorChar));

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
    public void SetInaccessibleDirectory(string relativePath) {
        var targetPath = Abs(relativePath);

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
    public FileStream CreateLockedFile(string relativePath, string? contents = "") {
        if(relativePath is null) {
            throw new ArgumentNullException(nameof(relativePath));
        }

        var fullPath = Path.Combine(Root, relativePath.Replace('/', Path.DirectorySeparatorChar));
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
    public string CreateTree(Dictionary<string, string?> files) {
        if(files is null) {
            throw new ArgumentNullException(nameof(files));
        }

        foreach(var pair in files) {
            var full = Path.Combine(Root, pair.Key);
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
    /// <param name="relativePath">The relative path of the file to create.</param>
    /// <exception cref="InvalidOperationException"></exception>
    public void AssertDirectoryInaccessible(string relativePath) {
        var target = Abs(relativePath);

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

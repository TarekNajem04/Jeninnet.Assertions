//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Tests;

[TestClass]
public class TestEnvironmentTests {
    [TestMethod]
    public void Root_IsAbsoluteExistingDirectory() {
        using var env = new TestEnvironment();
        Assert.IsTrue(Path.IsPathRooted(env.Root));
        Assert.IsTrue(Directory.Exists(env.Root));
    }

    [TestMethod]
    public void CreateDirectory_CreatesAndReturnsFullPath() {
        using var env = new TestEnvironment();
        var path = env.CreateDirectory("sub/dir");
        Assert.IsTrue(Directory.Exists(path));
        Assert.IsTrue(PathHelper.Equivalent(path, env.Abs("sub", "dir")));
    }

    [TestMethod]
    public void CreateFile_WritesContentsAndReturnsFullPath() {
        using var env = new TestEnvironment();
        var path = env.CreateFile("sub/file.txt", "hello");
        Assert.IsTrue(File.Exists(path));
        Assert.AreEqual("hello", File.ReadAllText(path));
    }

    [TestMethod]
    public void CreateFile_DefaultsToEmptyContents() {
        using var env = new TestEnvironment();
        var path = env.CreateFile("file.txt");
        Assert.IsTrue(File.Exists(path));
        Assert.AreEqual("", File.ReadAllText(path));
    }

    [TestMethod]
    public void CreateFile_RejectsNullRelativePath() {
        using var env = new TestEnvironment();
        Assert.ThrowsExactly<ArgumentNullException>(() => env.CreateFile(null!));
    }

    [TestMethod]
    public void CreateFiles_CreatesAllFiles() {
        using var env = new TestEnvironment();
        env.CreateFiles("a.txt", "b/c.txt", "d.txt");
        Assert.IsTrue(File.Exists(env.Abs("a.txt")));
        Assert.IsTrue(File.Exists(env.Abs("b", "c.txt")));
        Assert.IsTrue(File.Exists(env.Abs("d.txt")));
    }

    [TestMethod]
    public void Abs_CombinesRootWithSegments() {
        using var env = new TestEnvironment();
        Assert.AreEqual(Path.Combine(env.Root, "x", "y"), env.Abs("x", "y"));
    }

    [TestMethod]
    public void CreateTree_CreatesFilesAndNestedDirectories() {
        using var env = new TestEnvironment();
        var files = new Dictionary<string, string?> {
            ["root.txt"] = "root",
            ["dir/nested.txt"] = "nested",
            ["emptydir"] = null
        };

        var root = env.CreateTree(files);
        Assert.AreEqual(env.Root, root);
        Assert.AreEqual("root", File.ReadAllText(Path.Combine(root, "root.txt")));
        Assert.AreEqual("nested", File.ReadAllText(Path.Combine(root, "dir", "nested.txt")));
        Assert.IsTrue(Directory.Exists(Path.Combine(root, "emptydir")));
    }

    [TestMethod]
    public void CreateTree_RejectsNullDictionary() {
        using var env = new TestEnvironment();
        Assert.ThrowsExactly<ArgumentNullException>(() => env.CreateTree(null!));
    }

    [TestMethod]
    public void SetInaccessible_DoesNotThrowForExistingDirectory() {
        using var env = new TestEnvironment();
        env.CreateDirectory("target");
        env.SetInaccessible("target");
        Assert.IsTrue(Directory.Exists(env.Abs("target")));
    }

    [TestMethod]
    public void Dispose_RemovesRootDirectory() {
        var env = new TestEnvironment();
        var root = env.Root;
        env.CreateFile("file.txt");
        env.Dispose();
        Assert.IsFalse(Directory.Exists(root));
    }
}

[TestClass]
public class InaccessibleDirectorySimulatorTests {
    [TestMethod]
    public void CreatePseudoInaccessibleDir_CreatesFileEntry() {
        using var env = new TestEnvironment();
        var path = InaccessibleDirectorySimulator.CreatePseudoInaccessibleDir(env, "pseudo");
        Assert.IsTrue(File.Exists(path));
        Assert.IsFalse(Directory.Exists(path));
    }

    [TestMethod]
    public void CreatePseudoInaccessibleDir_RejectsNullArguments() {
        using var env = new TestEnvironment();
        Assert.ThrowsExactly<ArgumentNullException>(() => InaccessibleDirectorySimulator.CreatePseudoInaccessibleDir(null!, "x"));
        Assert.ThrowsExactly<ArgumentNullException>(() => InaccessibleDirectorySimulator.CreatePseudoInaccessibleDir(env, null!));
    }

    [TestMethod]
    public void CreateLockedDir_CreatesDirectoryAndLockHandle() {
        using var env = new TestEnvironment();
        var path = InaccessibleDirectorySimulator.CreateLockedDir(env, "locked", out var stream);
        try {
            Assert.IsTrue(Directory.Exists(path));
            Assert.IsTrue(File.Exists(Path.Combine(path, ".lock")));
            Assert.IsTrue(stream.CanRead);
        }
        finally {
            stream.Dispose();
        }
    }

    [TestMethod]
    public void CreateLockedDir_RejectsNullArguments() {
        using var env = new TestEnvironment();
        FileStream? stream = null;
        Assert.ThrowsExactly<ArgumentNullException>(() => InaccessibleDirectorySimulator.CreateLockedDir(null!, "x", out stream));
        Assert.ThrowsExactly<ArgumentNullException>(() => InaccessibleDirectorySimulator.CreateLockedDir(env, null!, out stream));
    }

    [TestMethod]
    public void AssertDirectoryInaccessible_PassForPseudoInaccessibleEntry() {
        using var env = new TestEnvironment();
        InaccessibleDirectorySimulator.CreatePseudoInaccessibleDir(env, "pseudo");
        env.AssertDirectoryInaccessible("pseudo");
    }

    [TestMethod]
    public void AssertDirectoryInaccessible_ThrowsWhenDirectoryIsAccessible() {
        using var env = new TestEnvironment();
        env.CreateDirectory("open");
        var ex = Assert.ThrowsExactly<InvalidOperationException>(() => env.AssertDirectoryInaccessible("open"));
        Assert.Contains("still accessible", ex.Message);
    }
}

[TestClass]
public class PathHelperTests {
    [TestMethod]
    public void Join_UsesDirectorySeparator() => Assert.AreEqual(
            string.Join(Path.DirectorySeparatorChar, "a", "b", "c"),
            PathHelper.Join("a", "b", "c"));

    [TestMethod]
    public void Join_RejectsNullSegments() => Assert.ThrowsExactly<ArgumentNullException>(() => PathHelper.Join(null!));

    [TestMethod]
    public void Normalize_ConvertsForwardSlashes() {
        var normalized = PathHelper.Normalize("a/b/c");
        Assert.AreEqual(string.Join(Path.DirectorySeparatorChar, "a", "b", "c"), normalized);
    }

    [TestMethod]
    public void Normalize_RejectsNull() => Assert.ThrowsExactly<ArgumentNullException>(() => PathHelper.Normalize(null!));

    [TestMethod]
    public void Equivalent_IgnoresSeparatorsCaseAndTrailingSlash() => Assert.IsTrue(PathHelper.Equivalent("a/b/", "A" + Path.DirectorySeparatorChar + "b"));

    [TestMethod]
    public void Equivalent_DifferentiatesDistinctPaths() => Assert.IsFalse(PathHelper.Equivalent("a/b", "a/c"));

    [TestMethod]
    public void Equivalent_RejectsNullArguments() {
        Assert.ThrowsExactly<ArgumentNullException>(() => PathHelper.Equivalent(null!, "x"));
        Assert.ThrowsExactly<ArgumentNullException>(() => PathHelper.Equivalent("x", null!));
    }
}

[TestClass]
public class PathExtensionsTests {
    [TestMethod]
    public void EndWithNormalized_MatchesNormalizedSuffix() {
        var full = Path.Combine("root", "sub", "file.txt");
        Assert.IsTrue(full.EndWithNormalized("sub/file.txt"));
    }

    [TestMethod]
    public void EndWithNormalized_DoesNotMatchWrongSuffix() {
        var full = Path.Combine("root", "file.txt");
        Assert.IsFalse(full.EndWithNormalized("other/file.txt"));
    }

    [TestMethod]
    public void EndWithNormalized_RejectsNullArguments() => Assert.ThrowsExactly<ArgumentNullException>(() => "x".EndWithNormalized(null!));

    [TestMethod]
    public void EndWithPath_MatchesRelativeSuffix() {
        var full = Path.Combine("root", "a", "b");
        Assert.IsTrue(full.EndWithPath(Path.Combine("a", "b")));
    }

    [TestMethod]
    public void EndWithPath_UsesOrdinalComparison() {
        var full = Path.Combine("root", "A");
        Assert.IsFalse(full.EndWithPath("a"));
    }

    [TestMethod]
    public void EndWithPath_RejectsNullArguments() => Assert.ThrowsExactly<ArgumentNullException>(() => "x".EndWithPath(null!));
}

[TestClass]
public class TestEnvironmentExtensionsTests {
    [TestMethod]
    public void CreateDeepDirectoryTree_CreatesNestedDirectoriesAndFile() {
        using var env = new TestEnvironment();
        env.CreateDeepDirectoryTree(levels: 3);
        Assert.IsTrue(Directory.Exists(env.Abs("dir0", "dir1", "dir2")));
        Assert.IsTrue(File.Exists(env.Abs("dir0", "dir1", "dir2", "file.txt")));
    }

    [TestMethod]
    public void CreateDeepDirectoryTree_CreatesMultipleLeafFiles() {
        using var env = new TestEnvironment();
        env.CreateDeepDirectoryTree(levels: 2, fileCount: 3);
        for(var i = 0; i < 3; i++) {
            Assert.IsTrue(File.Exists(env.Abs("dir0", "dir1", $"file_{i}.txt")));
        }
    }

    [TestMethod]
    public void CreateDeepDirectoryTree_UsesCustomFileExtension() {
        using var env = new TestEnvironment();
        env.CreateDeepDirectoryTree(levels: 1, fileName: "config", fileExt: "json");
        Assert.IsTrue(File.Exists(env.Abs("dir0", "config.json")));
    }

    [TestMethod]
    public void CreateDeepDirectoryTree_NoOpForNonPositiveLevels() {
        using var env = new TestEnvironment();
        env.CreateDeepDirectoryTree(levels: 0);
        env.CreateDeepDirectoryTree(levels: -1);
        Assert.IsFalse(Directory.Exists(env.Abs("dir0")));
    }
}

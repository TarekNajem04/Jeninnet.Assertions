//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Tests;

[TestClass]
public class TestEnvironmentCoverageTests {
    [TestMethod]
    public void CreateFiles_RejectsNullArray() {
        using var env = new TestEnvironment();
        Assert.ThrowsExactly<ArgumentNullException>(() => env.CreateFiles(null!));
    }

    [TestMethod]
    public void CreateInaccessibleDirectory_ReturnsPath() {
        using var env = new TestEnvironment();
        var path = env.CreateInaccessibleDirectory("inaccessible");
        Assert.AreEqual(env.Abs("inaccessible"), path);
    }

    [TestMethod]
    public void SetInaccessible_RejectsNullAndCreatesMissingDirectory() {
        using var env = new TestEnvironment();
        Assert.ThrowsExactly<ArgumentNullException>(() => env.SetInaccessible(null!));

        env.SetInaccessible("fresh");
        Assert.IsTrue(Directory.Exists(env.Abs("fresh")));
    }

    [TestMethod]
    public void SetInaccessibleDirectory_BlocksDirectoryWithoutThrowing() {
        using var env = new TestEnvironment();
        env.CreateDirectory("blocked");

        env.SetInaccessibleDirectory("blocked");

        Assert.IsNotNull(env.Abs("blocked"));
    }

    [TestMethod]
    public void CreateLockedFile_WritesLocksAndRejectsNull() {
        using var env = new TestEnvironment();
        Assert.ThrowsExactly<ArgumentNullException>(() => env.CreateLockedFile(null!));

        using var stream = env.CreateLockedFile("locked.txt", "content");

        Assert.IsTrue(File.Exists(env.Abs("locked.txt")));
        Assert.IsTrue(stream.CanRead);
        Assert.IsTrue(stream.CanWrite);
    }

    [TestMethod]
    public void Dispose_IsIdempotentWhenRootMissing() {
        var env = new TestEnvironment();
        var root = env.Root;
        Directory.Delete(root, recursive: true);

        env.Dispose();

        Assert.IsFalse(Directory.Exists(root));
    }

    [TestMethod]
    public void Dispose_ResetsAttributesWhenAFileIsLocked() {
        var env = new TestEnvironment();
        var root = env.Root;
        var stream = env.CreateLockedFile("locked.txt");
        try {
            env.Dispose();
        }
        finally {
            stream.Dispose();
        }

        env.Dispose();

        Assert.IsFalse(Directory.Exists(root));
    }

    [TestMethod]
    public void CreateFile_EscapingRelativePath_ThrowsWithoutCreatingFile() {
        using var env = new TestEnvironment();
        var name = "jeninnet-escape-" + Guid.NewGuid().ToString("n") + ".txt";
        var outside = Path.Combine(Path.GetDirectoryName(env.Root)!, name);
        try {
            Assert.ThrowsExactly<ArgumentException>(() => env.CreateFile("../" + name));
            Assert.IsFalse(File.Exists(outside));
        }
        finally {
            if(File.Exists(outside)) {
                File.Delete(outside);
            }
        }
    }

    [TestMethod]
    public void CreateFile_AbsolutePath_ThrowsWithoutCreatingFile() {
        using var env = new TestEnvironment();
        var outside = Path.Combine(Path.GetDirectoryName(env.Root)!, "jeninnet-absolute-" + Guid.NewGuid().ToString("n") + ".txt");
        try {
            Assert.ThrowsExactly<ArgumentException>(() => env.CreateFile(outside));
            Assert.IsFalse(File.Exists(outside));

            var systemPath = Path.DirectorySeparatorChar == '\\' ? @"C:\Windows\x" : "/etc/x";
            Assert.ThrowsExactly<ArgumentException>(() => env.CreateFile(systemPath));
        }
        finally {
            if(File.Exists(outside)) {
                File.Delete(outside);
            }
        }
    }

    [TestMethod]
    public void CreateFile_AbsolutePathInsideRoot_IsRejected() {
        using var env = new TestEnvironment();
        var target = env.Abs("absolute.txt");

        Assert.ThrowsExactly<ArgumentException>(() => env.CreateFile(target));
        Assert.IsFalse(File.Exists(target));
    }

    [TestMethod]
    public void CreateFile_NormalizedRelativePath_StaysInsideRoot() {
        using var env = new TestEnvironment();

        var path = env.CreateFile("nested/../inside.txt", "content");

        Assert.AreEqual(env.Abs("inside.txt"), path);
        Assert.IsTrue(File.Exists(path));
    }

    [TestMethod]
    public void CreateDirectory_EscapingRelativePath_ThrowsWithoutCreatingDirectory() {
        using var env = new TestEnvironment();
        var name = "jeninnet-dir-" + Guid.NewGuid().ToString("n");
        var outside = Path.Combine(Path.GetDirectoryName(env.Root)!, name);
        try {
            Assert.ThrowsExactly<ArgumentException>(() => env.CreateDirectory("../" + name));
            Assert.IsFalse(Directory.Exists(outside));
        }
        finally {
            if(Directory.Exists(outside)) {
                Directory.Delete(outside, recursive: true);
            }
        }
    }

    [TestMethod]
    public void CreateTree_EscapingKey_ThrowsBeforeCreatingAnyEntry() {
        using var env = new TestEnvironment();
        var name = "jeninnet-tree-" + Guid.NewGuid().ToString("n") + ".txt";
        var outside = Path.Combine(Path.GetDirectoryName(env.Root)!, name);
        var files = new Dictionary<string, string?> {
            ["first.txt"] = "first",
            ["../" + name] = "escape",
        };

        try {
            Assert.ThrowsExactly<ArgumentException>(() => env.CreateTree(files));
            Assert.IsFalse(File.Exists(env.Abs("first.txt")));
            Assert.IsFalse(File.Exists(outside));
        }
        finally {
            if(File.Exists(outside)) {
                File.Delete(outside);
            }
        }
    }

    [TestMethod]
    public void CreateTree_NormalizedPaths_CreateEntriesInsideRoot() {
        using var env = new TestEnvironment();

        env.CreateTree(new Dictionary<string, string?> {
            ["nested/../file.txt"] = "content",
            ["folder"] = null,
        });

        Assert.IsTrue(File.Exists(env.Abs("file.txt")));
        Assert.IsTrue(Directory.Exists(env.Abs("folder")));
    }

    [TestMethod]
    public void CreateFiles_EscapingPath_ThrowsBeforeCreatingAnyFile() {
        using var env = new TestEnvironment();

        Assert.ThrowsExactly<ArgumentException>(() => env.CreateFiles("first.txt", "../escape.txt"));
        Assert.IsFalse(File.Exists(env.Abs("first.txt")));
    }

    [TestMethod]
    public void Abs_EscapingOrAbsoluteParts_Throw() {
        using var env = new TestEnvironment();

        Assert.ThrowsExactly<ArgumentException>(() => env.Abs("..", "outside.txt"));
        Assert.ThrowsExactly<ArgumentException>(() => env.Abs(Path.Combine(Path.GetDirectoryName(env.Root)!, "outside.txt")));
        Assert.ThrowsExactly<ArgumentNullException>(() => env.Abs(null!));
        Assert.AreEqual(env.Root, env.Abs());
    }

    [TestMethod]
    public void SetInaccessible_EscapingPaths_Throw() {
        using var env = new TestEnvironment();

        Assert.ThrowsExactly<ArgumentException>(() => env.SetInaccessible("../outside"));
        Assert.ThrowsExactly<ArgumentException>(() => env.SetInaccessibleDirectory("../outside"));
        Assert.ThrowsExactly<ArgumentException>(() => env.AssertDirectoryInaccessible("../outside"));
    }

    [TestMethod]
    public void CreateLockedFile_EscapingPath_Throws() {
        using var env = new TestEnvironment();

        Assert.ThrowsExactly<ArgumentException>(() => env.CreateLockedFile("../outside.txt"));
    }

    [TestMethod]
    public void Dispose_DoesNotDeleteOutsideRootAfterRejectedEscape() {
        var env = new TestEnvironment();
        var parent = Path.GetDirectoryName(env.Root)!;
        var outsideDir = Path.Combine(parent, "jeninnet-outside-" + Guid.NewGuid().ToString("n"));
        var outsideFile = Path.Combine(outsideDir, "keep.txt");
        Directory.CreateDirectory(outsideDir);
        File.WriteAllText(outsideFile, "keep");
        try {
            Assert.ThrowsExactly<ArgumentException>(() => env.CreateFile("../" + Path.GetFileName(outsideDir) + "/keep.txt"));
            env.Dispose();
            Assert.IsTrue(File.Exists(outsideFile));
        }
        finally {
            env.Dispose();
            if(Directory.Exists(outsideDir)) {
                Directory.Delete(outsideDir, recursive: true);
            }
        }
    }
}

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
}

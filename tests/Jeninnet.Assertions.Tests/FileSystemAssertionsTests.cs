//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Tests;

[TestClass]
public class FileSystemAssertionsTests {
    [TestMethod]
    public void File_ExistAndMetadata() {
        using var environment = new TestEnvironment();
        var path = environment.CreateFile("a.txt", "abc");
        var file = new FileInfo(path);

        file.Should().Exist()
            .And.HaveName("a.txt")
            .And.HaveExtension(".txt")
            .And.HaveExtension("txt")
            .And.HaveFullName(path)
            .And.HaveDirectory(environment.Root)
            .And.HaveLength(3)
            .And.HaveLengthGreaterThan(2)
            .And.HaveLengthLessThan(4)
            .And.NotHaveLength(4)
            .And.NotBeEmpty();

        var missing = new FileInfo(environment.Abs("missing.txt"));
        missing.Should().NotExist();

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().Exist());
        Assert.AreEqual("Expected missing to exist, but it was not found.", ex.Message);
    }

    [TestMethod]
    public void File_EmptyAndLengthFailures() {
        using var environment = new TestEnvironment();
        var empty = new FileInfo(environment.CreateFile("empty.txt"));
        empty.Should().BeEmpty().And.HaveLength(0);

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => empty.Should().NotBeEmpty());
        Assert.AreEqual("Expected empty not to be empty, but it was.", ex.Message);

        var file = new FileInfo(environment.CreateFile("data.txt", "abc"));
        var length = Assert.ThrowsExactly<AssertionFailedException>(() => file.Should().HaveLength(5));
        Assert.AreEqual("Expected file to have length 5, but found 3.", length.Message);

        var greater = Assert.ThrowsExactly<AssertionFailedException>(() => file.Should().HaveLengthGreaterThan(3));
        Assert.AreEqual("Expected file to have length greater than 3, but found 3.", greater.Message);

        var less = Assert.ThrowsExactly<AssertionFailedException>(() => file.Should().HaveLengthLessThan(3));
        Assert.AreEqual("Expected file to have length less than 3, but found 3.", less.Message);

        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => file.Should().HaveLength(-1));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => file.Should().NotHaveLength(-1));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => file.Should().HaveLengthGreaterThan(-1));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => file.Should().HaveLengthLessThan(-1));
    }

    [TestMethod]
    public void File_MissingMetadataFailsWithExistenceMessage() {
        using var environment = new TestEnvironment();
        var missing = new FileInfo(environment.Abs("missing.txt"));

        var name = Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().HaveName("missing.txt"));
        Assert.AreEqual("Expected missing to exist, but it was not found.", name.Message);

        var length = Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().HaveLength(1));
        Assert.AreEqual("Expected missing to exist, but it was not found.", length.Message);
    }

    [TestMethod]
    public void File_Attributes() {
        if(!OperatingSystem.IsWindows()) {
            Assert.Inconclusive("FileAttributes semantics are platform-specific; this suite covers Windows.");
        }

        using var environment = new TestEnvironment();
        var path = environment.CreateFile("attributes.txt", "x");
        var file = new FileInfo(path);
        file.Should().NotBeReadOnly().And.NotBeHidden();

        File.SetAttributes(path, File.GetAttributes(path) | FileAttributes.ReadOnly | FileAttributes.Hidden);
        file.Refresh();
        try {
            file.Should().BeReadOnly().And.BeHidden()
                .And.HaveAttribute(FileAttributes.ReadOnly | FileAttributes.Hidden)
                .And.NotHaveAttribute(FileAttributes.System);

            var readOnly = Assert.ThrowsExactly<AssertionFailedException>(() => file.Should().NotBeReadOnly());
            Assert.AreEqual("Expected file not to be read-only, but it was.", readOnly.Message);

            var hidden = Assert.ThrowsExactly<AssertionFailedException>(() => file.Should().NotBeHidden());
            Assert.AreEqual("Expected file not to be hidden, but it was.", hidden.Message);
        }
        finally {
            File.SetAttributes(path, FileAttributes.Normal);
        }
    }

    [TestMethod]
    public void File_TimesUseTolerance() {
        using var environment = new TestEnvironment();
        var file = new FileInfo(environment.CreateFile("time.txt", "x"));
        var creation = file.CreationTimeUtc;
        var lastWrite = file.LastWriteTimeUtc;

        file.Should().HaveCreationTime(creation, TimeSpan.FromSeconds(2))
            .And.HaveLastWriteTime(lastWrite, TimeSpan.FromSeconds(2));

        var creationFailure = Assert.ThrowsExactly<AssertionFailedException>(
            () => file.Should().HaveCreationTime(creation.AddDays(-1), TimeSpan.FromSeconds(1)));
        Assert.Contains("creation time within", creationFailure.Message);

        var writeFailure = Assert.ThrowsExactly<AssertionFailedException>(
            () => file.Should().HaveLastWriteTime(lastWrite.AddDays(-1), TimeSpan.FromSeconds(1)));
        Assert.Contains("last write time within", writeFailure.Message);

        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => file.Should().HaveCreationTime(creation, TimeSpan.FromSeconds(-1)));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => file.Should().HaveLastWriteTime(lastWrite, TimeSpan.FromSeconds(-1)));
    }

    [TestMethod]
    public void Directory_ContentsAndCounts() {
        using var environment = new TestEnvironment();
        var root = new DirectoryInfo(environment.CreateDirectory("root"));
        var child = new DirectoryInfo(environment.CreateDirectory("root/child"));
        environment.CreateFile("root/a.txt", "a");
        environment.CreateFile("root/b.txt", "bb");

        root.Should().Exist()
            .And.HaveName("root")
            .And.BeParentOf(child)
            .And.BeParentOf(new FileInfo(environment.Abs("root/a.txt")))
            .And.ContainFile("a.txt")
            .And.ContainDirectory("child")
            .And.HaveFileCount(2)
            .And.HaveDirectoryCount(1)
            .And.NotBeEmpty();

        child.Should().BeEmpty();

        var empty = Assert.ThrowsExactly<AssertionFailedException>(() => root.Should().BeEmpty());
        Assert.AreEqual("Expected root to be empty, but found 3 item(s).", empty.Message);

        var fileCount = Assert.ThrowsExactly<AssertionFailedException>(() => root.Should().HaveFileCount(3));
        Assert.AreEqual("Expected root to contain 3 file(s), but found 2.", fileCount.Message);

        var directoryCount = Assert.ThrowsExactly<AssertionFailedException>(() => root.Should().HaveDirectoryCount(0));
        Assert.AreEqual("Expected root to contain 0 directory(ies), but found 1.", directoryCount.Message);

        var missingFile = Assert.ThrowsExactly<AssertionFailedException>(() => root.Should().ContainFile("missing.txt"));
        Assert.AreEqual("Expected root to contain file 'missing.txt', but it was not found.", missingFile.Message);

        var missingDirectory = Assert.ThrowsExactly<AssertionFailedException>(() => root.Should().ContainDirectory("missing"));
        Assert.AreEqual("Expected root to contain directory 'missing', but it was not found.", missingDirectory.Message);
    }

    [TestMethod]
    public void Directory_ParentBoundariesAndNulls() {
        using var environment = new TestEnvironment();
        var root = new DirectoryInfo(environment.CreateDirectory("root"));
        var sibling = new DirectoryInfo(environment.CreateDirectory("root-other"));

        var siblingEx = Assert.ThrowsExactly<AssertionFailedException>(() => root.Should().BeParentOf(sibling));
        Assert.Contains("to be a parent directory of", siblingEx.Message);

        var sameEx = Assert.ThrowsExactly<AssertionFailedException>(() => root.Should().BeParentOf(root));
        Assert.Contains("to be a parent directory of", sameEx.Message);

        FileInfo? missingFile = null;
        missingFile.Should().NotExist();
        var fileEx = Assert.ThrowsExactly<AssertionFailedException>(() => missingFile.Should().Exist());
        Assert.AreEqual("Expected missingFile to exist, but it was null.", fileEx.Message);

        DirectoryInfo? missingDirectory = null;
        missingDirectory.Should().NotExist();
        var directoryEx = Assert.ThrowsExactly<AssertionFailedException>(() => missingDirectory.Should().Exist());
        Assert.AreEqual("Expected missingDirectory to exist, but it was null.", directoryEx.Message);

        var absentDirectory = new DirectoryInfo(environment.Abs("absent"));
        var contentsEx = Assert.ThrowsExactly<AssertionFailedException>(() => absentDirectory.Should().BeEmpty());
        Assert.AreEqual("Expected absentDirectory to exist, but it was not found.", contentsEx.Message);

        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => root.Should().HaveFileCount(-1));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => root.Should().HaveDirectoryCount(-1));
        Assert.ThrowsExactly<ArgumentNullException>(() => root.Should().BeParentOf(null!));
        Assert.ThrowsExactly<ArgumentNullException>(() => root.Should().ContainFile(null!));
        Assert.ThrowsExactly<ArgumentNullException>(() => root.Should().ContainDirectory(null!));
    }
}

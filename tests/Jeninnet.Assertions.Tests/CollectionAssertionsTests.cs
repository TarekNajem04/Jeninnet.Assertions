//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Tests;

[TestClass]
public partial class CollectionAssertionsTests {
    private sealed class DeliberatelyEqualItem(string name) {
        public string Name { get; } = name;

        public override bool Equals(object? obj) => obj is DeliberatelyEqualItem other && string.Equals(Name, other.Name, StringComparison.Ordinal);

        public override int GetHashCode() => StringComparer.Ordinal.GetHashCode(Name);
    }

    private sealed class BoxedSize(int size) {
        public int Size { get; } = size;
    }
}

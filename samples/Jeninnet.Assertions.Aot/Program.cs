//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
using Jeninnet.Assertions;

// Smoke test for the trimming/AOT-safe assertion surface. Every verb below is
// reflection-free; equivalency, type/member assertions, event monitoring, and
// object snapshots are deliberately excluded because they need reflection.
var subject = "actual";
subject.Should().Be("actual");
true.Should().BeTrue();
subject.Should().Contain("tua");
int[] numbers = [1, 2, 3];
numbers.Should().HaveCount(3);

Action action = () => throw new InvalidOperationException("boom");
action.Should().Throw<InvalidOperationException>();

Console.WriteLine("Jeninnet AOT smoke test passed.");

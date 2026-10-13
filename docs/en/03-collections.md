# Chapter 3 — Collections, Dictionaries, and Spans

When checking a collection, the most common mistake is choosing the wrong level of strictness. Insisting on an exact order when order doesn't matter produces brittle tests. Stopping at the count when you need to verify values lets bugs slip through.

The solution is to move **layer by layer**: start with count, then membership, then order, then set relationships. Each layer answers a different question—and mixing questions is what makes collection tests brittle.

## 3.1 Layer 1: Count before content

Most collection failures show up first in the **count**: a missing element, an extra one, or an unintended duplicate. Check the count before digging into individual elements.

```csharp
lines.Should().HaveCount(3)
    .And.NotBeEmpty()
    .And.HaveCountGreaterThan(2)
    .And.HaveSameCount(expected);
```

A wrong count makes all subsequent checks misleading, so it makes sense to assert it first. The failure message clearly states the count difference, which often pinpoints the issue immediately.

## 3.2 Layer 2: Membership—from existence to correctness

Membership asks two distinct questions: **does the element exist?** and **is this the right element?**

Answer the first with `Contain()`:

```csharp
lines.Should().Contain(first).And.NotContain(cancelled);
lines.Should().Contain(l => l.IsGift).And.NotContain(l => l.IsVoid);
lines.Should().NotContainNulls();
```

`NotContainNulls()` is especially helpful—it fails on the first `null` with its index, instead of letting a `NullReferenceException` bubble up later from unrelated code.

To answer the second question—verifying the specific element that matched—use `.Which` with `ContainSingle()`:

```csharp
var gift = lines.Should().ContainSingle(l => l.IsGift).Which;
gift.Should().BeGreaterThan(0);
gift.Description.Should().Be("Gift");
```

`ContainSingle(predicate).Which` returns the single matching element, letting you continue asserting against it. Existence alone is only half the test.

## 3.3 Layer 3: Order—choose the right strength

Not every sequence requires exact ordering. Pick the **weakest constraint that matches the actual rule**:

```csharp
lines.Should().Equal(a, b, c);                    // exact sequence, same elements, same order, same length
lines.Should().ContainInOrder(a, c);              // a appears before c, gaps allowed
lines.Should().ContainInConsecutiveOrder(b, c);   // adjacent in that exact order
lines.Should().StartWith(a).And.EndWith(c);       // prefix and suffix only
lines.Should().BeInAscendingOrder();              // ascending order
lines.Should().HaveElementAt(0, a);               // specific index and value
```

Rule of thumb: **start with the weakest constraint that fits, and only tighten it when order is truly part of the contract.** Forcing `Equal` when display order is incidental creates brittle tests.

You can pass a custom comparer for ordering: `BeInAscendingOrder(StringComparer.OrdinalIgnoreCase)`.

## 3.4 When order doesn't matter: `BeEquivalentTo()`

Most of the time, two lists containing the same elements with the same frequencies are **equivalent**, regardless of order:

```csharp
codes.Should().BeEquivalentTo(["A", "B"], options => options.IgnoringCase());
codes.Should().BeEquivalentTo([1, 2, 3], options => options.WithStrictOrdering());
```

By default, `BeEquivalentTo()` **does not require order and does count duplicates**. This is the right choice for most real-world cases like carts, results, or batches.

If order is actually part of the definition, make it explicit with `WithStrictOrdering()`. Tweak equality with `IgnoringCase()` or a custom comparer as needed.

## 3.5 Set relationships

The library provides direct assertions for common set relationships, avoiding manual loops:

```csharp
lines.Should().BeSubsetOf(catalog);       // every element is in catalog (no extras outside allowed set)
lines.Should().ContainSubset(required);   // every required element is present (no missing required items)
lines.Should().IntersectWith(promotions); // there is at least one overlap
lines.Should().NotIntersectWith(discontinued); // no overlap
```

`BeSubsetOf` guards against **unexpected extras**, while `ContainSubset` guards against **missing requirements**. You'll often need both in the same test.

## 3.6 Dictionaries

Dictionaries are checked with clear questions about keys and values:

```csharp
prices.Should().ContainKey("apple").And.NotContainKey("durian");
prices.Should().ContainKeys("apple", "pear");
prices.Should().Contain("apple", 1.20m); // key and value together
var price = prices.Should().ContainValue(1.20m).Which;
```

`.Which` also works here, returning the matching value so you can continue asserting.

When a lookup fails because of a typo, the message suggests the nearest existing names (`Did you mean 'apple'?`, ordered by edit distance and capped at three) — dictionary keys, JSON properties and paths, and string collections all diagnose themselves.

## 3.7 Spans: `Span<T>` and `ReadOnlySpan<T>`

To assert on memory spans without unnecessary allocations, use `Should()` directly. A dedicated array overload keeps `T[]` resolving to collection assertions, so spans and arrays never collide:

```csharp
Span<int> data = stackalloc[] { 1, 2, 3 };
data.Should().HaveCount(3).And.Contain(2);

var buffer = new Memory<int>(new[] { 1, 2, 3 });
buffer.Should().HaveCount(3);
```

Character spans can be compared to a whole string at once:

```csharp
"abc".AsSpan().Should().Be("abc");
```

On `netstandard2.0` the `Should()` span overloads are unavailable, so use `AsAssertions()` there instead.

## 3.8 Recap and exercise

**Recap:**

- Move layer by layer: **count → membership → order → set relationships**.
- `ContainSingle(predicate).Which` turns "exists and is unique" into a concrete value you can assert further.
- `BeEquivalentTo()` is the safe default when order doesn't matter. Use `Equal()` only when order is an explicit part of the contract.
- `Should()` works directly on spans and memory blocks (`AsAssertions()` remains for `netstandard2.0`).

**Exercise:**

1. Create a list: `[Book, Pen, Book]`.
2. Assert it's equivalent to a set of the same items regardless of order using `BeEquivalentTo()`—note how duplicates are handled.
3. Assert the exact sequence with `Equal()` using the same order, then change the order and observe the brittleness.
4. Extract the single item matching a condition with `ContainSingle(...).Which` and continue asserting against it.

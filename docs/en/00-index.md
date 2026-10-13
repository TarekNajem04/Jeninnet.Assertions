# Jeninnet.Assertions — The Book

Most assertion libraries begin with a single question: *What are we checking?*

This library begins with a different question: *When a check fails, what do we read?*

The failure message is the real interface of any assertion library. We read failure messages far more often than we write assertions. That moment comes at midnight when a previously green build turns red, during code review when we're trying to understand the intent behind a test, or when we're debugging a test we didn't write ourselves.

Jeninnet.Assertions is built around that moment. Every assertion starts with `Should()`, and every failure message states what was checked and what was actually found.

This book is the companion to `README.md`. The README is a concise reference that answers *what* exists. This book answers *how* to think with the library — chapter by chapter, from your first `dotnet add package` command all the way to the Roslyn analyzer that rewrites legacy assertion calls automatically.

## How to read this book

This book is not meant to be read in one sitting. Each chapter stands on its own and ends with a recap and a practical exercise.

- **Chapters 1–2:** Installation, your first assertion, and understanding failure messages. Then the core values: booleans, objects, numbers, and strings.
- **Chapters 3–5:** Everyday work: collections, time, async actions, and waiting with timeouts.
- **Chapter 6:** Structural assertions: when to compare by shape rather than identity, and when to freeze outputs with snapshots.
- **Chapters 7–8:** Beyond the code: reflection and architectural rules, then streams, formats, HTTP, events, and I/O.
- **Chapter 9:** The engine room: assertion scopes, culture, value formatting, and configuration.
- **Chapter 10:** The ecosystem: the MSTest adapter, Roslyn analyzer, the Consumer template, and migration from other assertion libraries.
- **Chapter 11:** Production validators: the same `Should()` verbs, collecting a result instead of throwing.
- **Chapter 12:** Snapshots in depth: the `.verified`/`.received` workflow, the evidence in each failure, and the deliberate update switch.

Code samples target `net10.0`. The library multi-targets `netstandard2.0;net6.0;net8.0;net10.0` (HTTP assertions require `net6.0+` only).

## Contents

| # | Chapter | File |
|---|---|---|
| 1 | First Steps: Installation, Your First Assertion, and Reading Failures | [01-first-steps.md](01-first-steps.md) |
| 2 | Core Values: `bool`, `object`, Numbers, and Strings | [02-core-values.md](02-core-values.md) |
| 3 | Collections, Dictionaries, and Spans | [03-collections.md](03-collections.md) |
| 4 | Time and Identity: Dates, `Guid`, and Enums | [04-time-and-identity.md](04-time-and-identity.md) |
| 5 | Actions and Async: Exceptions, Waiting, and Budgets | [05-actions-async.md](05-actions-async.md) |
| 6 | Shape, Not Identity: Equivalency and Snapshots | [06-equivalency-snapshots.md](06-equivalency-snapshots.md) |
| 7 | Reflection: Types, Members, and Architectural Rules | [07-reflection.md](07-reflection.md) |
| 8 | The Outside World: Streams, XML, JSON, HTTP, Events, and I/O | [08-io-formats-events.md](08-io-formats-events.md) |
| 9 | The Engine Room: Scopes, Culture, Formatting, and Configuration | [09-engine-room.md](09-engine-room.md) |
| 10 | The Ecosystem: MSTest Adapter, Analyzer, Consumer Template, and Migration | [10-ecosystem.md](10-ecosystem.md) |
| 11 | Validators: Assertions Without Throwing | [11-validators.md](11-validators.md) |
| 12 | Snapshots: Frozen Outputs You Review On Purpose | [12-snapshots.md](12-snapshots.md) |

> **Playbook:** [Migration Playbook — From Classic Asserts to `Should()`](10-migration.md) (MSTest, xUnit, and NUnit before/after, plus what the analyzer deliberately leaves manual).

Three conventions hold throughout this book:

1. Every assertion starts with `Should()`.
2. `.And` chains the next assertion to the previous one.
3. `because` explains *why* the expectation holds, not what it asserts.

Additionally, nullable types (`bool?`, `int?`, `DateTime?`, `Guid?`, nullable enums, `TimeSpan?`, ...) get their own assertion class that includes `BeNull()` and `NotBeNull()` alongside value assertions.

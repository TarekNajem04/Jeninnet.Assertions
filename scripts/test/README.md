# MSTest Assert meta allowlist

This folder contains the guardrail that keeps MSTest `Assert` confined to the tests that need an independent oracle.

## Policy

- `src/` never references a test framework.
- MSTest `Assert` is allowed only in the **engine** and **adapter** meta tests.
- Every other test file uses `Should()`.
- The allowlist is explicit and reviewed. Adding a file is a policy decision, not a mechanical baseline update.

## Categories

- **engine**: `tests/Jeninnet.Assertions.Tests/` — tests that verify the assertion engine, failure messages, scopes, reports, snapshots, equivalency, reflection, IO, and HTTP assertions.
- **adapter**: `tests/Jeninnet.Assertions.MSTest.Tests/`, `tests/Jeninnet.Assertions.NUnit.Tests/`, `tests/Jeninnet.Assertions.xUnit.Tests/`, and `tests/Jeninnet.Assertions.xUnit.v3.Tests/` — tests of the framework adapters.

## Usage

```pwsh
pwsh scripts/test/check-mstest-assert-usage.ps1
```

The check fails when a file outside the allowlist contains `Assert.*`.

## Exclusions

- Scans `tests/**/*.cs` and `consumers/**/*.cs`.
- Excludes `bin/`, `obj/`, and `tests/Jeninnet.Assertions.Analyzer.Tests/` because the latter contains `Assert.*` snippets that are inputs to the analyzer, not assertions executed by the test runner.
- Counts are textual occurrences of `Assert.`; keep comments and source snippets free of accidental matches.

## Adding a meta file

1. Explain why the test cannot use the library (normally because it asserts the library itself).
2. Add the path to `meta.engine` or `meta.adapter` in `mstest-assert-meta.json`.
3. Run the check and include the justification in the pull request.

## Why the engine keeps MSTest

The engine tests need an assertion mechanism that is independent of the code under test. Using `Should()` to verify `Should()` would make failures circular. High-level tests (architecture, independence, localization, and the consumer template) use the library directly.

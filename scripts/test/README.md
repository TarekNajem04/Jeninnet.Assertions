# MSTest Assert ratchet

This folder contains the guardrail that keeps the MSTest `Assert` migration from growing.

## Policy

- `src/` never references a test framework.
- MSTest `Assert` remains the independent oracle for the assertion engine and the adapter tests.
- Every other test file migrates to `Should()` incrementally.
- No new `Assert.*` usage is allowed, and an existing file cannot increase its count.
- The baseline is debt, not an allowlist for new code. Lower or remove entries as files migrate.

## Usage

```pwsh
pwsh scripts/test/check-mstest-assert-usage.ps1
```

The check fails when:

- a file outside `mstest-assert-baseline.json` contains `Assert.*`;
- a baseline file contains more `Assert.*` calls than the recorded count.

After an intentional migration, regenerate the baseline and commit it with the migration:

```pwsh
pwsh scripts/test/check-mstest-assert-usage.ps1 -UpdateBaseline
```

## Scope and exclusions

- Scans `tests/**/*.cs` and `consumers/**/*.cs`.
- Excludes `bin/`, `obj/`, and `tests/Jeninnet.Assertions.Analyzer.Tests/` because the latter contains `Assert.*` snippets that are inputs to the analyzer, not assertions executed by the test runner.
- Counts are textual occurrences of `Assert.`; keep comments and source snippets free of accidental matches.
- The baseline file is `mstest-assert-baseline.json` beside this README.

## Migration pattern

1. Replace `Assert.*` with the equivalent `Should()` assertion.
2. Add a small test-local helper when an exception instance is needed (`Throws<T>` backed by `ThrowExactly`).
3. Run the full test suite.
4. Run the ratchet with `-UpdateBaseline`.
5. Commit the migration and the baseline together.

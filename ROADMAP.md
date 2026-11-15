# Roadmap

> Living document. Priorities may shift as the library stabilizes; nothing here is a dated commitment.

## Now — release readiness (unreleased)

- Keep the public-API baseline green and the quality gate green (zero open issues, new-code coverage ≥ 80%, duplication ≤ 3%).
- Keep the sample suites (xUnit, NUnit, MSTest, Fixie, Expecto, TUnit), the consumer template, and the playground passing on every change.
- Run the release checklist before any publish: four-TFM build, full tests, package verification, trimmed/NativeAOT smoke tests, and the advisory benchmarks.
- Publish from a `v*` tag only after explicit approval; the tag must match `Directory.Build.props`.

## Next — 1.x hardening

- Raise the coverage floor in the remaining weak spots (nullable date/time components, XML/JSON diagnostics) and grow branch coverage.
- Equivalency and analyzer improvements driven by real-world feedback, while keeping the "guaranteed to compile" contract.
- Snapshots: more scrubbers and guidance for larger suites.
- Keep the published benchmarks current for the hot verbs as the API grows.

## Later — ecosystem

- Editor experience: richer code fixes, rule documentation, and a per-framework migration guide.
- Localization: additional satellite languages and a simple contribution flow for translators.
- Performance: extend the published benchmark suite to new assertion families as they grow.
- Community: open source-code contributions with coding standards, a review process, and a contributor program.

## Explicitly out of scope

- **`DataSet` / `DataTable` / `DataRow` / `DataColumn` assertions.** This is a schema-sized surface
  (constraints, relations, row states, `DBNull`, culture, and four TFMs) and it is reflection/AOT-heavy.
  It is not part of the core package. If concrete demand appears, an optional `Jeninnet.Assertions.Data`
  package can be considered, starting with table/column names, row count, and cell access. Until then,
  assert the `DataSet`/`DataTable` APIs directly or project rows into typed objects and use the core
  assertions.

## Principles

- No breaking changes without a major version and a migration note.
- Zero third-party runtime dependencies in the core package.
- Framework-agnostic by design; adapters remain optional packages.
- Failure messages stay deterministic: localized prose, invariant values.

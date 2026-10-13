# Roadmap

> Living document. Priorities may shift as the library stabilizes; nothing here is a dated commitment.

## Now — stabilization (1.0.0.0)

- Keep the public-API baseline green and the quality gate green (zero open issues, new-code coverage ≥ 80%, duplication ≤ 3%).
- Keep the sample suites (xUnit, NUnit, MSTest, Fixie, Expecto, TUnit), the consumer template, and the playground passing on every change.
- Publish the first NuGet release from a `v*` tag once the repository is public: core package, MSTest adapter, and analyzer.

## Next — 1.x hardening

- Raise the coverage floor in the remaining weak spots (nullable date/time components, XML/JSON diagnostics) and grow branch coverage.
- Add optional xUnit and NUnit adapters on demand, following the same explicit opt-in pattern as the MSTest adapter.
- Snapshots: per-directory defaults, more scrubbers, and guidance for Verify-style workflows.
- Equivalency: more diagnostics and mapping conveniences driven by real-world feedback.
- Analyzer: extend the rules beyond equality and condition calls (collection and string migrations) while keeping the "guaranteed to compile" contract.

## Later — ecosystem

- Editor experience: richer code fixes, rule documentation, and a per-framework migration guide.
- Localization: additional satellite languages and a simple contribution flow for translators.
- Performance: publish allocation and throughput benchmarks for the hot verbs and keep the success path allocation-free.
- Community: open source-code contributions with coding standards, a review process, and a contributor program.

## Principles

- No breaking changes without a major version and a migration note.
- Zero third-party runtime dependencies in the core package.
- Framework-agnostic by design; adapters remain optional packages.
- Failure messages stay deterministic: localized prose, invariant values.

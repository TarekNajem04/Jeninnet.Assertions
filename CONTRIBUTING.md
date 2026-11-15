# Contributing to Jeninnet.Assertions

Thank you for your interest in Jeninnet.Assertions.

The project aims to provide a framework-agnostic, fluent assertion library for .NET with a strong focus on correctness, expressive failure messages, performance, maintainability, and long-term API stability.

## Current Project Status

Jeninnet.Assertions is currently in an API stabilization phase.

During this stage, the project's internal architecture, CI/CD pipelines, quality gates, release process, and public APIs are still being refined and validated.

To maintain consistency and avoid unnecessary breaking changes, source-code contributions are temporarily limited to project maintainers.

## How You Can Contribute Today

Community participation is highly appreciated.

The most valuable contributions at this stage are:

* Bug reports
* Feature requests
* Design discussions
* API design feedback
* Documentation improvements
* Performance observations
* Security observations
* Real-world usage feedback

Please use GitHub Issues or Discussions to share ideas and suggestions.

## Pull Requests

At the current stage of the project:

* Pull requests containing source-code changes may not be accepted.
* Pull requests that improve documentation may be considered on a case-by-case basis.
* Large design proposals should begin as an Issue or Discussion before implementation.

Once the project reaches a stable baseline, external code contributions will be opened publicly.

## Project Structure

```text
src/
    Jeninnet.Assertions                 # core package
    Jeninnet.Assertions.MSTest          # optional adapter
    Jeninnet.Assertions.NUnit           # optional adapter
    Jeninnet.Assertions.TUnit           # optional adapter
    Jeninnet.Assertions.xUnit           # optional adapter (v2)
    Jeninnet.Assertions.xUnit.v3        # optional adapter (v3)
    Jeninnet.Assertions.Analyzer        # optional Roslyn analyzer

tests/
    Jeninnet.Assertions.Tests
    Jeninnet.Assertions.MSTest.Tests
    Jeninnet.Assertions.NUnit.Tests
    Jeninnet.Assertions.TUnit.Tests
    Jeninnet.Assertions.xUnit.Tests
    Jeninnet.Assertions.xUnit.v3.Tests
    Jeninnet.Assertions.Analyzer.Tests

benchmarks/
    Jeninnet.Assertions.Benchmarks      # advisory BenchmarkDotNet suite

consumers/
    Jeninnet.Assertions.Consumer
    Jeninnet.Assertions.Consumer.Tests

samples/
    Jeninnet.Assertions.Playground
    Jeninnet.Assertions.FrameworkZoo
    Jeninnet.Assertions.Aot             # trimmed/NativeAOT smoke test

docs/
```

## Design Philosophy

The architecture emphasizes:

* Framework independence: the core package never depends on MSTest, xUnit, NUnit, or TUnit; adapters are separate opt-in packages
* A fluent, discoverable assertion surface with consistent verbs, overloads, and chaining (`And`/`Which`)
* Deterministic failure messages: localized prose with invariant value formatting, and a clock that can be pinned per test
* Extensibility through value formatters, equivalency steps, custom assertions, and engine initializers
* Measured performance, with an allocation-stable success path for hot verbs and an advisory benchmark suite
* NativeAOT/trimming honesty: reflection-heavy families are annotated; the reflection-free surface is covered by a trimmed/NativeAOT smoke sample
* Long-term API stability enforced by a committed public-API baseline and package validation

When evaluating proposals, maintainability and long-term API stability are generally preferred over short-term convenience. Do not add DI containers, database layers, or application hosting to a library that does not need them.

## Development Standards

The project follows modern .NET engineering practices including:

* Automated CI validation across Windows, Linux, and macOS
* Static analysis and analyzer triage with zero unexplained warnings
* Deterministic formatting (`dotnet format --verify-no-changes`)
* Package validation and artifact inspection (assets, symbols, metadata, dependency hygiene)
* Public API surface baselines and intentional-change review
* Documentation parity, README claims, and localization gates
* Trimmed/NativeAOT smoke tests and advisory benchmarks
* Release verification procedures in `docs/release-checklist.md`

Contributions are expected to satisfy these requirements; a green build alone is not sufficient evidence.

## Translation Guide

Failure messages live in `src/Jeninnet.Assertions/Localization/`. Adding a language requires **no code change**:

1. Copy `Strings.resx` to `Strings.<culture>.resx` (for example `Strings.es.resx`).
2. Translate only the `<value>` text. Keep every `<data name="...">` key exactly as-is.
3. Preserve every placeholder exactly: `{0}`, `{1}`, … including order and braces.
4. Do not translate parameter names, member/type names, or technical tokens inside placeholders. Code-like literals such as `null`, `true`/`false`, `NaN`, `JSON`, `XML`, `HTTP`, and `URI` normally stay unchanged.
5. Run the completion gate:

   ```powershell
   pwsh scripts/docs/check-localization.ps1
   ```

   It reports `culture, missing, extra, empty` and fails on missing/extra keys or empty values. The French reference satellite is intentionally partial and is allowlisted; every other language must be complete.
6. Run the full test suite. Fallback is automatic: `es-ES` → `es` → English, `ar-EG` → `ar` → English, and so on.

### Terminology

| English | Arabic | Spanish | German |
|---|---|---|---|
| assertion | توكيد | aserción | Assertion |
| expected | المتوقع | esperado | erwartet |
| actual | الفعلي | real | tatsächlich |
| because | لأن | porque | weil |
| scope | نطاق | ámbito | Bereich |

### Rules

- **No unreviewed machine translation.** A native or fluent reviewer should approve the file before release.
- Translate the `because` glue (`MESSAGE_BECAUSE`) and the scope header (`SCOPE_ASSERTION_FAILURES`); they are user-facing text.
- Guard exceptions (`ArgumentNullException` and friends) stay in English by design.
- `Strings.qps-ploc.resx` is a commit-time pseudo-localization used to detect hardcoded text; it is shipped as a testing aid and is not a user language.

## Future Contributor Program

External source-code contributions are planned for a future phase of the project.

Once the architecture, release workflow, and public APIs are considered stable, contributor guidelines will be expanded to include:

* Coding standards
* Pull request requirements
* Review process
* Testing requirements
* Release compatibility expectations

Thank you for helping improve Jeninnet.Assertions.

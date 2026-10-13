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
    Jeninnet.Assertions
    Jeninnet.Assertions.MSTest
    Jeninnet.Assertions.Analyzer

tests/
    Jeninnet.Assertions.Tests
    Jeninnet.Assertions.MSTest.Tests
    Jeninnet.Assertions.Analyzer.Tests

consumers/
    Jeninnet.Assertions.Consumer
    Jeninnet.Assertions.Consumer.Tests

samples/
    Jeninnet.Assertions.Playground
    Jeninnet.Assertions.FrameworkZoo

docs/
```

## Design Philosophy

The architecture emphasizes:

* Framework independence: the core package never depends on MSTest, xUnit, or NUnit
* A fluent, discoverable assertion surface with consistent verbs and chaining
* Deterministic failure messages: localized prose with invariant value formatting
* Extensibility through value formatters, equivalency steps, and custom assertions
* Measured performance, with an allocation-free success path for hot verbs
* Long-term API stability enforced by a committed public-API baseline

When evaluating proposals, maintainability and long-term API stability are generally preferred over short-term convenience.

## Development Standards

The project follows modern .NET engineering practices including:

* Automated CI validation
* Static analysis and quality gates
* Package validation
* Public API surface baselines
* Cross-platform testing
* Release verification procedures

Any future code contributions will be expected to satisfy these requirements.

## Future Contributor Program

External source-code contributions are planned for a future phase of the project.

Once the architecture, release workflow, and public APIs are considered stable, contributor guidelines will be expanded to include:

* Coding standards
* Pull request requirements
* Review process
* Testing requirements
* Release compatibility expectations

Thank you for helping improve Jeninnet.Assertions.

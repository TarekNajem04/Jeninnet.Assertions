//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions;

/// <summary>Identifies the serialization format used by <see cref="FailureReport.ToReport"/>.</summary>
public enum AssertionReportFormat {
    /// <summary>Compact JSON with a <c>count</c> and a <c>failures</c> array.</summary>
    Json = 0,

    /// <summary>SARIF 2.1.0 for GitHub Code Scanning and other SARIF consumers.</summary>
    Sarif = 1,

    /// <summary>JUnit XML for CI dashboards and test-report ingestion.</summary>
    JUnitXml = 2,

    /// <summary>GitHub Actions workflow commands (<c>::error ...</c>).</summary>
    GitHubAnnotations = 3,

    /// <summary>Markdown table suitable for pull-request comments.</summary>
    Markdown = 4,
}

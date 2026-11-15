//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions;

/// <summary>Escapes values for the CI report formats.</summary>
internal static class ReportEscaping {
    /// <summary>Escapes a GitHub workflow-command message (<c>%</c>, CR, LF).</summary>
    /// <param name="value">The value to escape.</param>
    /// <returns>The escaped value.</returns>
    internal static string GitHubData(string value) =>
        value.Replace("%", "%25").Replace("\r", "%0D").Replace("\n", "%0A");

    /// <summary>Escapes a GitHub workflow-command property (<c>%</c>, CR, LF, <c>:</c>, <c>,</c>).</summary>
    /// <param name="value">The value to escape.</param>
    /// <returns>The escaped value.</returns>
    internal static string GitHubProperty(string value) =>
        GitHubData(value).Replace(":", "%3A").Replace(",", "%2C");

    /// <summary>Escapes a Markdown table cell (<c>|</c> and line breaks).</summary>
    /// <param name="value">The value to escape.</param>
    /// <returns>The escaped cell value.</returns>
    internal static string MarkdownCell(string value) =>
        value.Replace("\r\n", "<br>").Replace("\r", "<br>").Replace("\n", "<br>").Replace("|", "\\|");

    /// <summary>Normalizes CRLF and CR to LF for deterministic reports.</summary>
    /// <param name="value">The value to normalize.</param>
    /// <returns>The LF-only value.</returns>
    internal static string NormalizeLineEndings(string value) =>
        value.Replace("\r\n", "\n").Replace('\r', '\n');
}

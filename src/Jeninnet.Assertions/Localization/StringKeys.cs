//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Localization;

/// <summary>Resource keys for localizable assertion failure messages (see <c>Strings.resx</c>).</summary>
/// <remarks>
/// Adding a language never touches code: copy <c>Strings.resx</c> to
/// <c>Strings.&lt;culture&gt;.resx</c>, translate the values, and the build
/// produces the satellite assembly automatically.
/// </remarks>
internal static partial class StringKeys {
    internal const string BOOLEAN_BE_TRUE = "BOOLEAN_BE_TRUE";
    internal const string BOOLEAN_BE_FALSE = "BOOLEAN_BE_FALSE";
    internal const string BOOLEAN_NOT_BE = "BOOLEAN_NOT_BE";
    internal const string BOOLEAN_IMPLY = "BOOLEAN_IMPLY";

    internal const string NULLABLE_BOOLEAN_BE = "NULLABLE_BOOLEAN_BE";
    internal const string NULLABLE_BOOLEAN_BE_TRUE = "NULLABLE_BOOLEAN_BE_TRUE";
    internal const string NULLABLE_BOOLEAN_NOT_BE_TRUE = "NULLABLE_BOOLEAN_NOT_BE_TRUE";
    internal const string NULLABLE_BOOLEAN_BE_FALSE = "NULLABLE_BOOLEAN_BE_FALSE";
    internal const string NULLABLE_BOOLEAN_NOT_BE_FALSE = "NULLABLE_BOOLEAN_NOT_BE_FALSE";
    internal const string NULLABLE_BOOLEAN_BE_NULL = "NULLABLE_BOOLEAN_BE_NULL";
    internal const string NULLABLE_BOOLEAN_NOT_BE_NULL = "NULLABLE_BOOLEAN_NOT_BE_NULL";

    internal const string NUMERIC_BE_POSITIVE = "NUMERIC_BE_POSITIVE";
    internal const string NUMERIC_BE_NEGATIVE = "NUMERIC_BE_NEGATIVE";
    internal const string NUMERIC_APPROXIMATE = "NUMERIC_APPROXIMATE";
    internal const string NUMERIC_NOT_APPROXIMATE = "NUMERIC_NOT_APPROXIMATE";
    internal const string NUMERIC_BE_NAN = "NUMERIC_BE_NAN";
    internal const string NUMERIC_NOT_BE_NAN = "NUMERIC_NOT_BE_NAN";

    internal const string NULLABLE_NUMERIC_BE = "NULLABLE_NUMERIC_BE";
    internal const string NULLABLE_NUMERIC_NOT_BE = "NULLABLE_NUMERIC_NOT_BE";
    internal const string NULLABLE_NUMERIC_BE_GREATER_THAN = "NULLABLE_NUMERIC_BE_GREATER_THAN";
    internal const string NULLABLE_NUMERIC_BE_GREATER_THAN_OR_EQUAL = "NULLABLE_NUMERIC_BE_GREATER_THAN_OR_EQUAL";
    internal const string NULLABLE_NUMERIC_BE_LESS_THAN = "NULLABLE_NUMERIC_BE_LESS_THAN";
    internal const string NULLABLE_NUMERIC_BE_LESS_THAN_OR_EQUAL = "NULLABLE_NUMERIC_BE_LESS_THAN_OR_EQUAL";
    internal const string NULLABLE_NUMERIC_BE_BETWEEN = "NULLABLE_NUMERIC_BE_BETWEEN";
    internal const string NULLABLE_NUMERIC_NOT_BE_BETWEEN = "NULLABLE_NUMERIC_NOT_BE_BETWEEN";
    internal const string NULLABLE_NUMERIC_BE_ONE_OF = "NULLABLE_NUMERIC_BE_ONE_OF";
    internal const string NULLABLE_NUMERIC_MATCH = "NULLABLE_NUMERIC_MATCH";
    internal const string NULLABLE_NUMERIC_BE_NULL = "NULLABLE_NUMERIC_BE_NULL";
    internal const string NULLABLE_NUMERIC_NOT_BE_NULL = "NULLABLE_NUMERIC_NOT_BE_NULL";
}

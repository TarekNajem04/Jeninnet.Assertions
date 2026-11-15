//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions;

/// <summary>Stable explanations used by trimming and AOT annotations.</summary>
internal static class TrimmingMessages {
    /// <summary>Type metadata inspection requires preserved members.</summary>
    internal const string TYPE_METADATA = "Inspects reflected type metadata (members, attributes, or relationships). Preserve the inspected types with DynamicallyAccessedMembers or use the non-reflection assertion surface.";

    /// <summary>Assembly metadata inspection requires preserved types/references.</summary>
    internal const string ASSEMBLY_METADATA = "Enumerates assembly types or references. Preserve the assembly's types and references when trimming.";

    /// <summary>Equivalency walks object members by reflection.</summary>
    internal const string EQUIVALENCY = "Compares object graphs through reflection; preserve the compared members when trimming or use explicit member assertions.";

    /// <summary>Snapshot naming/inspection uses stack metadata and object serialization.</summary>
    internal const string SNAPSHOT = "Uses caller stack metadata and/or runtime-type serialization; snapshot names and content require untrimmed metadata.";

    /// <summary>Event monitoring reflects over events and compiles delegate accessors.</summary>
    internal const string EVENT_MONITORING = "Reflects over events and compiles delegate accessors at runtime; preserve the monitored event members.";

    /// <summary>Runtime-type serialization requires reflection or dynamic code.</summary>
    internal const string SERIALIZATION = "Serializes an arbitrary runtime type; use a JsonSerializerContext/source-generated contract or keep the type members untrimmed.";

    /// <summary>Runtime-type JSON serialization requires dynamic code generation.</summary>
    internal const string DYNAMIC_SERIALIZATION = "Serializes an arbitrary runtime type with System.Text.Json, which may require runtime code generation under NativeAOT.";

    /// <summary>Assembly/member discovery scans attributes and methods.</summary>
    internal const string DISCOVERY = "Scans assemblies and members through reflection; preserve the discovered types and methods when trimming.";
}

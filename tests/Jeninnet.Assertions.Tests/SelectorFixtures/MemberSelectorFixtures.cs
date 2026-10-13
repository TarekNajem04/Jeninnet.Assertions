//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Tests.SelectorFixtures;

#pragma warning disable S2094, S1144, S1186, S3993, CA1852, CA1822, S2325, IDE0051, RCS1170, RCS1213, RCS1251 // Member-selector fixtures: shapes only for reflection selection in MemberSelectorTests; empty bodies, unused privates, read-write shapes, and instance-only shapes are intentional.
internal class TinyService {
    public void Start() {
    }

    public void Stop() {
    }

    public int Count() => 0;
}

internal class OrderService {
    public void PlaceOrder() {
    }

    public void CancelOrder() {
    }

    public int CountOrders() => 0;

    public string GetName() => "";

    public static void ResetCache() {
    }

    public virtual void Validate() {
    }

    public async Task SaveAsync() => await Task.CompletedTask;

    [Marker]
    public void Marked() {
    }

    private void Hidden() {
    }

    public string Name { get; set; } = "";

    public int Total { get; }

    public static string CacheKey { get; set; } = "";

    private int Secret { get; set; }

    public class Nested {
    }

    private class Buried {
    }
}

internal class VirtualService {
    [PropertyMarker]
    public virtual string? Label { get; set; }

    public string? Plain { get; set; }
}

[AttributeUsage(AttributeTargets.Method)]
internal sealed class MarkerAttribute : Attribute;

[AttributeUsage(AttributeTargets.Property)]
internal sealed class PropertyMarkerAttribute : Attribute;
#pragma warning restore S2094, S1144, S1186, S3993, CA1852, CA1822, S2325, IDE0051, RCS1170, RCS1213, RCS1251

//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Tests.SelectorFixtures;

#pragma warning disable S2094, CA1852 // Filter fixtures: exist only for reflection selection in SelectorTests; abstract base intentionally childless.
internal interface IWidget;

internal interface IWorker {
    void Work();
    int Load { get; }
}

internal class Widget : IWidget;

[Tag]
internal class TaggedWidget : Widget;

internal sealed class ChildWidget : TaggedWidget;

internal struct StructWidget : IWidget;

internal static class WidgetStatic {
    public static string Name => "static";
}

internal abstract class WidgetBase : IWidget {
    public abstract void Run();
}

[AttributeUsage(AttributeTargets.Class)]
internal sealed class TagAttribute : Attribute;
#pragma warning restore S2094, CA1852

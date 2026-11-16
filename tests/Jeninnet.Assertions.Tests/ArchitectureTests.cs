//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
using ApplicationFixtures = Jeninnet.Assertions.Tests.ArchitectureFixtures.Application;
using CycleFixtures = Jeninnet.Assertions.Tests.ArchitectureFixtures.Cycles;
using DomainFixtures = Jeninnet.Assertions.Tests.ArchitectureFixtures.Domain;
using InfrastructureFixtures = Jeninnet.Assertions.Tests.ArchitectureFixtures.Infrastructure;
using ShapeFixtures = Jeninnet.Assertions.Tests.ArchitectureFixtures.Shapes;

namespace Jeninnet.Assertions.Tests;

[TestClass]
public class ArchitectureTests {
    private static string DomainNamespace => typeof(DomainFixtures.Order).Namespace!;

    private static string InfrastructureNamespace => typeof(InfrastructureFixtures.SqlConnection).Namespace!;

    private static string ApplicationNamespace => typeof(ApplicationFixtures.OrderService).Namespace!;

    private static string ShapesNamespace => typeof(ShapeFixtures.PublicSurface).Namespace!;

    [TestMethod]
    public void Selectors_FilterByAccessAssemblyAndNesting() {
        new TypeSelector([typeof(ShapeFixtures.PublicSurface), typeof(ShapeFixtures.InternalThing)])
            .ThatArePublic().Should().HaveCount(1);
        new TypeSelector([typeof(ShapeFixtures.PublicSurface), typeof(ShapeFixtures.InternalThing)])
            .ThatAreInternal().Should().HaveCount(1);
        new TypeSelector([typeof(ShapeFixtures.Outer.NestedPublic), typeof(ShapeFixtures.AuditService)])
            .ThatAreNested().Should().HaveCount(1);
        new TypeSelector([typeof(ShapeFixtures.Outer.NestedPublic), typeof(ShapeFixtures.AuditService)])
            .ThatAreNotNested().Should().HaveCount(1);
        new TypeSelector([typeof(ShapeFixtures.PublicSurface), typeof(object)])
            .ThatAreInAssembly(typeof(ShapeFixtures.PublicSurface).Assembly).Should().HaveCount(1);
        new TypeSelector([typeof(ShapeFixtures.PublicSurface), typeof(object)])
            .ThatAreNotInAssembly(typeof(ShapeFixtures.PublicSurface).Assembly).Should().HaveCount(1);
    }

    [TestMethod]
    public void Selectors_FilterByGenericRecordAndImmutability() {
        new TypeSelector([typeof(ShapeFixtures.GenericThing<>), typeof(ShapeFixtures.PublicSurface)])
            .ThatAreGeneric().Should().HaveCount(1);
        new TypeSelector([typeof(ShapeFixtures.GenericThing<>), typeof(ShapeFixtures.PublicSurface)])
            .ThatAreNotGeneric().Should().HaveCount(1);
        new TypeSelector([typeof(ShapeFixtures.Person), typeof(ShapeFixtures.Point), typeof(ShapeFixtures.AuditService)])
            .ThatAreRecords().Should().HaveCount(2);
        new TypeSelector([typeof(ShapeFixtures.Person), typeof(ShapeFixtures.Point), typeof(ShapeFixtures.AuditService)])
            .ThatAreNotRecords().Should().HaveCount(1);
        new TypeSelector([typeof(ShapeFixtures.ImmutableBox), typeof(ShapeFixtures.MutableBox), typeof(ShapeFixtures.Point)])
            .ThatAreImmutable().Should().HaveCount(2);
        new TypeSelector([typeof(ShapeFixtures.ImmutableBox), typeof(ShapeFixtures.MutableBox), typeof(ShapeFixtures.Point)])
            .ThatAreNotImmutable().Should().HaveCount(1);
    }

    [TestMethod]
    public void Selectors_FilterByNames() {
        var selector = new TypeSelector([typeof(ShapeFixtures.PaymentHandler), typeof(ShapeFixtures.OrderHandler), typeof(ShapeFixtures.AuditService)]);

        selector.ThatHaveName("AuditService").Should().HaveCount(1);
        selector.ThatHaveNameStartingWith("Pay").Should().HaveCount(1);
        selector.ThatHaveNameEndingWith("Handler").Should().HaveCount(2);
        selector.ThatHaveNameMatching("^Order.*$").Should().HaveCount(1);

        Throws<ArgumentNullException>(() => selector.ThatHaveName(null!));
        Throws<ArgumentNullException>(() => selector.ThatHaveNameStartingWith(null!));
        Throws<ArgumentNullException>(() => selector.ThatHaveNameEndingWith(null!));
        Throws<ArgumentNullException>(() => selector.ThatHaveNameMatching(null!));
        Throws<RegexParseException>(() => selector.ThatHaveNameMatching("["));
    }

    [TestMethod]
    public void Selectors_FilterByAssignability() {
        var selector = new TypeSelector([typeof(ShapeFixtures.ShapeBase), typeof(ShapeFixtures.IShape), typeof(ShapeFixtures.Circle), typeof(ShapeFixtures.AuditService)]);

        selector.ThatAreAssignableTo<ShapeFixtures.IShape>().Should().HaveCount(1);
        selector.ThatAreAssignableTo<ShapeFixtures.ShapeBase>().Should().HaveCount(1);
#pragma warning disable CA2263 // Intentional: exercises the non-generic overload; generics are covered immediately above.
        selector.ThatAreAssignableTo(typeof(ShapeFixtures.ShapeBase)).Should().HaveCount(1);
        selector.ThatAreAssignableFrom(typeof(ShapeFixtures.Circle)).Should().HaveCount(2);
#pragma warning restore CA2263
        selector.ThatAreAssignableFrom<ShapeFixtures.Circle>().Should().HaveCount(2);

        Throws<ArgumentNullException>(() => selector.ThatAreAssignableTo((Type)null!));
        Throws<ArgumentNullException>(() => selector.ThatAreAssignableFrom((Type)null!));
    }

    [TestMethod]
    public void Selectors_FilterByDependency() {
        var selector = new TypeSelector([
            typeof(DomainFixtures.Order),
            typeof(DomainFixtures.LeakyOrder),
            typeof(DomainFixtures.AttributedLeak),
            typeof(ApplicationFixtures.OrderService),
        ]);

        selector.ThatHaveDependencyOn(InfrastructureNamespace).Should().HaveCount(2);
        selector.ThatHaveDependencyOn(nameof(InfrastructureFixtures.SqlConnection)).Should().HaveCount(2);
        selector.ThatHaveDependencyOn(typeof(InfrastructureFixtures.SqlConnection).FullName!).Should().HaveCount(2);
        selector.ThatDoNotHaveDependencyOn(InfrastructureNamespace).Should().HaveCount(2);
        new TypeSelector([typeof(DomainFixtures.Order)])
            .ThatHaveDependencyOn(typeof(IReadOnlyList<>).FullName!).Should().HaveCount(1);
        new TypeSelector([typeof(DomainFixtures.MethodOnlyLeak)])
            .ThatHaveDependencyOn(InfrastructureNamespace).Should().HaveCount(0);

        Throws<ArgumentNullException>(() => selector.ThatHaveDependencyOn(null!));
        Throws<ArgumentException>(() => selector.ThatHaveDependencyOn("  "));
        Throws<ArgumentNullException>(() => selector.ThatDoNotHaveDependencyOn(null!));
    }

    [TestMethod]
    public void Selectors_RejectNullArchitectureArguments() {
        var selection = new TypeSelector([typeof(DomainFixtures.Order)]).Should();
        Type[] exposedWithNull = [null!];

        Throws<ArgumentNullException>(() => selection.HaveNameStartingWith(null!));
        Throws<ArgumentNullException>(() => selection.HaveNameMatching(null!));
        Throws<ArgumentNullException>(() => selection.NotDependOn((Type)null!));
        Throws<ArgumentNullException>(() => selection.NotExposeTypes(exposedWithNull));
        Throws<ArgumentNullException>(() => new TypeSelector([typeof(object)]).ThatAreInAssembly(null!));
        Throws<ArgumentNullException>(() => new TypeSelector([typeof(object)]).ThatAreNotInAssembly(null!));
    }

    [TestMethod]
    public void NotDependOn_ReportsOffenders() {
        var selection = new TypeSelector([
            typeof(DomainFixtures.Order),
            typeof(DomainFixtures.LeakyOrder),
            typeof(DomainFixtures.AttributedLeak),
            typeof(DomainFixtures.MethodOnlyLeak),
        ]);

        var ex = Throws<AssertionFailedException>(() => selection.Should().NotDependOn(InfrastructureNamespace));

        ex.Message.Should().Contain("2 type(s) do");
        ex.Message.Should().Contain("LeakyOrder");
        ex.Message.Should().Contain("AttributedLeak");
        ex.Message.Should().Contain("SqlConnection");
        ex.Message.Should().NotContain("MethodOnlyLeak");

        new TypeSelector([typeof(ApplicationFixtures.OrderService)]).Should().NotDependOn(InfrastructureNamespace);

        var typeEx = Throws<AssertionFailedException>(
            () => new TypeSelector([typeof(DomainFixtures.LeakyOrder)]).Should().NotDependOn(typeof(InfrastructureFixtures.SqlConnection)));
        typeEx.Message.Should().Contain("SqlConnection");

        Throws<ArgumentNullException>(
            () => new TypeSelector([typeof(DomainFixtures.Order)]).Should().NotDependOn((string)null!));
        Throws<ArgumentException>(
            () => new TypeSelector([typeof(DomainFixtures.Order)]).Should().NotDependOn("  "));
    }

    [TestMethod]
    public void OnlyDependOn_AllowsListedLayersAndReportsOthers() {
        var application = new TypeSelector([typeof(ApplicationFixtures.OrderService), typeof(ApplicationFixtures.PricingService)]);

        application.Should().OnlyDependOn(DomainNamespace, "System");
        application.Should().OnlyDependOn([DomainNamespace, "System"], "the application layer stays above domain and the BCL");

        var ex = Throws<AssertionFailedException>(() => application.Should().OnlyDependOn(DomainNamespace));

        ex.Message.Should().Contain("other dependencies");
        ex.Message.Should().Contain("System");
        ex.Message.Should().Contain("OrderService");
        ex.Message.Should().Contain("PricingService");

        Throws<ArgumentNullException>(() => application.Should().OnlyDependOn((string[])null!));
        Throws<ArgumentException>(() => application.Should().OnlyDependOn());
        Throws<ArgumentException>(() => application.Should().OnlyDependOn("  "));
    }

    [TestMethod]
    public void NotDependOnAny_MatchesEveryTarget() {
        var selection = new TypeSelector([typeof(DomainFixtures.LeakyOrder)]);

        var ex = Throws<AssertionFailedException>(
            () => selection.Should().NotDependOnAny(InfrastructureNamespace, "System.Data"));
        ex.Message.Should().Contain("any of");
        ex.Message.Should().Contain("LeakyOrder");

        selection.Should().NotDependOnAny("No.Such.Namespace");

        Throws<ArgumentNullException>(() => selection.Should().NotDependOnAny((string[])null!));
        Throws<ArgumentException>(() => selection.Should().NotDependOnAny());
        Throws<ArgumentException>(() => selection.Should().NotDependOnAny("  "));
    }

    [TestMethod]
    public void DependOn_RequiresEveryTypeToHaveTheDependency() {
        new TypeSelector([typeof(ApplicationFixtures.OrderService)]).Should().DependOn(DomainNamespace);
        new TypeSelector([typeof(ApplicationFixtures.OrderService)]).Should().DependOn(typeof(DomainFixtures.IOrderRepository));

        var ex = Throws<AssertionFailedException>(
            () => new TypeSelector([typeof(DomainFixtures.Order)]).Should().DependOn(InfrastructureNamespace));
        ex.Message.Should().Contain("do not");
        ex.Message.Should().Contain("Order");

        Throws<ArgumentNullException>(
            () => new TypeSelector([typeof(DomainFixtures.Order)]).Should().DependOn((string)null!));
        Throws<ArgumentException>(
            () => new TypeSelector([typeof(DomainFixtures.Order)]).Should().DependOn("  "));
        Throws<ArgumentNullException>(
            () => new TypeSelector([typeof(DomainFixtures.Order)]).Should().DependOn((Type)null!));
    }

    [TestMethod]
    public void DependOnAny_RequiresAtLeastOneTarget() {
        new TypeSelector([typeof(ApplicationFixtures.OrderService)]).Should().DependOnAny(DomainNamespace, "No.Such.Namespace");

        var ex = Throws<AssertionFailedException>(
            () => new TypeSelector([typeof(DomainFixtures.Order)]).Should().DependOnAny(InfrastructureNamespace, "No.Such.Namespace"));
        ex.Message.Should().Contain("at least one");
        ex.Message.Should().Contain("Order");

        Throws<ArgumentNullException>(
            () => new TypeSelector([typeof(DomainFixtures.Order)]).Should().DependOnAny((string[])null!));
        Throws<ArgumentException>(
            () => new TypeSelector([typeof(DomainFixtures.Order)]).Should().DependOnAny());
        Throws<ArgumentException>(
            () => new TypeSelector([typeof(DomainFixtures.Order)]).Should().DependOnAny("  "));
    }

    [TestMethod]
    public void OnlyBeDependedOnBy_ChecksIncomingSources() {
        var order = new TypeSelector([typeof(DomainFixtures.Order)]);

        order.Should().OnlyBeDependedOnBy(ApplicationNamespace, InfrastructureNamespace, DomainNamespace);
        order.Should().OnlyBeDependedOnBy([ApplicationNamespace, InfrastructureNamespace, DomainNamespace], "the domain is the bottom layer");

        var ex = Throws<AssertionFailedException>(
            () => order.Should().OnlyBeDependedOnBy(ApplicationNamespace, InfrastructureNamespace));
        ex.Message.Should().Contain("only by");
        ex.Message.Should().Contain("IOrderRepository");
        ex.Message.Should().Contain("Quote");
        ex.Message.Should().NotContain("OrderService");
        ex.Message.Should().NotContain("SqlOrderRepository");

        Throws<ArgumentNullException>(() => order.Should().OnlyBeDependedOnBy((string[])null!));
        Throws<ArgumentException>(() => order.Should().OnlyBeDependedOnBy());
        Throws<ArgumentException>(() => order.Should().OnlyBeDependedOnBy("  "));
    }

    [TestMethod]
    public void NotBeDependedOnBy_MatchesSourcesAndIgnoresInternalDependencies() {
        new TypeSelector([typeof(DomainFixtures.Order)]).Should().NotBeDependedOnBy("No.Such.Namespace");

        var ex = Throws<AssertionFailedException>(
            () => new TypeSelector([typeof(DomainFixtures.Order)]).Should().NotBeDependedOnBy(ApplicationNamespace));
        ex.Message.Should().Contain("OrderService");

        var internalSelection = new TypeSelector([typeof(DomainFixtures.IOrderRepository), typeof(InfrastructureFixtures.SqlOrderRepository)]);
        internalSelection.Should().NotBeDependedOnBy(DomainNamespace);

        Throws<ArgumentNullException>(
            () => new TypeSelector([typeof(DomainFixtures.Order)]).Should().NotBeDependedOnBy(null!));
        Throws<ArgumentException>(
            () => new TypeSelector([typeof(DomainFixtures.Order)]).Should().NotBeDependedOnBy("  "));
    }

    [TestMethod]
    public void FollowLayers_AcceptsDeclaredDirectionAndRejectsReverse() {
        var domain = new ArchitectureLayer("Domain", DomainNamespace);
        var application = new ArchitectureLayer("Application", ApplicationNamespace).DependsOn("Domain");
        var infrastructure = new ArchitectureLayer("Infrastructure", InfrastructureNamespace).DependsOn("Domain");
        var selection = new TypeSelector([
            typeof(DomainFixtures.Order),
            typeof(DomainFixtures.OrderLine),
            typeof(DomainFixtures.Money),
            typeof(DomainFixtures.IOrderRepository),
            typeof(ApplicationFixtures.OrderService),
            typeof(ApplicationFixtures.PricingService),
            typeof(InfrastructureFixtures.SqlConnection),
            typeof(InfrastructureFixtures.SqlOrderRepository),
        ]);

        selection.Should().FollowLayers(domain, application, infrastructure);

        var reversed = new ArchitectureLayer("Application", ApplicationNamespace);
        var ex = Throws<AssertionFailedException>(
            () => selection.Should().FollowLayers(domain, reversed, infrastructure));
        ex.Message.Should().Contain("Application -> Domain");
        ex.Message.Should().Contain("OrderService");
    }

    [TestMethod]
    public void FollowLayers_UsesTheLongestMatchingNamespacePrefix() {
        var domain = new ArchitectureLayer("Domain", DomainNamespace);
        var sales = new ArchitectureLayer("Sales", typeof(DomainFixtures.Sales.Quote).Namespace!).DependsOn("Domain");
        var selection = new TypeSelector([typeof(DomainFixtures.OrderWithQuote), typeof(DomainFixtures.Sales.Quote)]);

        var ex = Throws<AssertionFailedException>(() => selection.Should().FollowLayers(domain, sales));
        ex.Message.Should().Contain("Domain -> Sales");
        ex.Message.Should().Contain("OrderWithQuote");

        new TypeSelector([typeof(DomainFixtures.Sales.Quote)]).Should().FollowLayers(domain, sales);
    }

    [TestMethod]
    public void FollowLayers_IgnoresFrameworkNamespacesByDefault() {
        var shapes = new ArchitectureLayer("Shapes", ShapesNamespace);
        var selection = new TypeSelector([typeof(ShapeFixtures.PublicSurface)]);

        selection.Should().FollowLayers(shapes);

        var ex = Throws<AssertionFailedException>(
            () => selection.Should().FollowLayers([shapes], includingFrameworkDependencies: true));
        ex.Message.Should().Contain("Shapes -> Framework");

        var frameworkAware = new ArchitectureLayer("Shapes", ShapesNamespace).DependsOn(ArchitectureLayer.FrameworkLayerName);
        selection.Should().FollowLayers([frameworkAware], includingFrameworkDependencies: true);
    }

    [TestMethod]
    public void FollowLayers_ValidatesLayerDefinitions() {
        var selection = new TypeSelector([typeof(DomainFixtures.Order)]);

        Throws<ArgumentNullException>(() => selection.Should().FollowLayers((ArchitectureLayer[])null!));
        Throws<ArgumentException>(() => selection.Should().FollowLayers());
        ArchitectureLayer[] layersWithNull = [null!];
        Throws<ArgumentNullException>(() => selection.Should().FollowLayers(layersWithNull));
        Throws<ArgumentException>(() => selection.Should().FollowLayers(
            new ArchitectureLayer("A", DomainNamespace),
            new ArchitectureLayer("A", ApplicationNamespace)));
        Throws<ArgumentException>(() => selection.Should().FollowLayers(
            new ArchitectureLayer("A", DomainNamespace),
            new ArchitectureLayer("B", DomainNamespace)));
        Throws<ArgumentException>(() => selection.Should().FollowLayers(
            new ArchitectureLayer("A", DomainNamespace).DependsOn("Missing")));
    }

    [TestMethod]
    public void ArchitectureLayer_ValidatesNamePrefixAndDependencies() {
        Throws<ArgumentNullException>(() => GC.KeepAlive(new ArchitectureLayer(null!, DomainNamespace)));
        Throws<ArgumentException>(() => GC.KeepAlive(new ArchitectureLayer(" ", DomainNamespace)));
        Throws<ArgumentNullException>(() => GC.KeepAlive(new ArchitectureLayer("Domain", null!)));
        Throws<ArgumentException>(() => GC.KeepAlive(new ArchitectureLayer("Domain", "  ")));

        var layer = new ArchitectureLayer(" Domain ", " " + DomainNamespace + " ");
        layer.Name.Should().Be("Domain");
        layer.NamespacePrefix.Should().Be(DomainNamespace);
        layer.DependsOn("Application").Should().BeSameAs(layer);
        Throws<ArgumentNullException>(() => layer.DependsOn((string[])null!));
        Throws<ArgumentException>(() => layer.DependsOn());
        Throws<ArgumentException>(() => layer.DependsOn(" "));
    }

    [TestMethod]
    public void HaveNoCycles_ReportsConcretePaths() {
        var selection = new TypeSelector([
            typeof(CycleFixtures.CycleA),
            typeof(CycleFixtures.CycleB),
            typeof(CycleFixtures.SelfCycle),
            typeof(CycleFixtures.CycleLeaf),
            typeof(CycleFixtures.NoCycleRoot),
        ]);

        var ex = Throws<AssertionFailedException>(() => selection.Should().HaveNoCycles());

        ex.Message.Should().Contain("2 cycle(s)");
        var cycleA = typeof(CycleFixtures.CycleA).FullName;
        var cycleB = typeof(CycleFixtures.CycleB).FullName;
        var selfCycle = typeof(CycleFixtures.SelfCycle).FullName;
        ex.Message.Should().Contain($"{cycleA} -> {cycleB} -> {cycleA}");
        ex.Message.Should().Contain($"{selfCycle} -> {selfCycle}");

        new TypeSelector([typeof(CycleFixtures.CycleLeaf), typeof(CycleFixtures.NoCycleRoot)]).Should().HaveNoCycles();
    }

    [TestMethod]
    public void BePublicAndBeInternal_ReportOffenders() {
        new TypeSelector([typeof(ShapeFixtures.PublicSurface)]).Should().BePublic();
        new TypeSelector([typeof(ShapeFixtures.InternalThing)]).Should().BeInternal();

        var publicEx = Throws<AssertionFailedException>(
            () => new TypeSelector([typeof(ShapeFixtures.InternalThing)]).Should().BePublic());
        publicEx.Message.Should().Contain("1 type(s) are not");
        publicEx.Message.Should().Contain("InternalThing");

        var internalEx = Throws<AssertionFailedException>(
            () => new TypeSelector([typeof(ShapeFixtures.PublicSurface)]).Should().BeInternal());
        internalEx.Message.Should().Contain("PublicSurface");
    }

    [TestMethod]
    public void BeNested_ReportsOffenders() {
        new TypeSelector([typeof(ShapeFixtures.Outer.NestedPublic)]).Should().BeNested();
        new TypeSelector([typeof(ShapeFixtures.AuditService)]).Should().NotBeNested();

        var ex = Throws<AssertionFailedException>(
            () => new TypeSelector([typeof(ShapeFixtures.AuditService)]).Should().BeNested());
        ex.Message.Should().Contain("AuditService");
    }

    [TestMethod]
    public void BeRecordAndBeImmutable_UseDocumentedHeuristics() {
        new TypeSelector([typeof(ShapeFixtures.Person), typeof(ShapeFixtures.Point)]).Should().BeRecord();
        new TypeSelector([typeof(ShapeFixtures.ImmutableBox), typeof(ShapeFixtures.Point)]).Should().BeImmutable();
        new TypeSelector([typeof(ShapeFixtures.AuditService)]).Should().NotBeRecord();
        new TypeSelector([typeof(ShapeFixtures.MutableBox)]).Should().NotBeImmutable();

        var recordEx = Throws<AssertionFailedException>(
            () => new TypeSelector([typeof(ShapeFixtures.AuditService)]).Should().BeRecord());
        recordEx.Message.Should().Contain("AuditService");

        var immutableEx = Throws<AssertionFailedException>(
            () => new TypeSelector([typeof(ShapeFixtures.MutableBox)]).Should().BeImmutable());
        immutableEx.Message.Should().Contain("MutableBox");
    }

    [TestMethod]
    public void NameAssertions_ReportOffenders() {
        new TypeSelector([typeof(ShapeFixtures.PaymentHandler)]).Should().HaveNameStartingWith("Pay");
        new TypeSelector([typeof(ShapeFixtures.PaymentHandler), typeof(ShapeFixtures.OrderHandler)])
            .Should().HaveNameEndingWith("Handler");
        new TypeSelector([typeof(ShapeFixtures.OrderHandler)]).Should().HaveNameMatching("^Order.*$");

        var ex = Throws<AssertionFailedException>(
            () => new TypeSelector([typeof(ShapeFixtures.PaymentHandler), typeof(ShapeFixtures.AuditService)])
                .Should().HaveNameEndingWith("Handler"));
        ex.Message.Should().Contain("1 type(s) do not");
        ex.Message.Should().Contain("AuditService");

        Throws<ArgumentNullException>(
            () => new TypeSelector([typeof(ShapeFixtures.AuditService)]).Should().HaveNameEndingWith(null!));
        Throws<RegexParseException>(
            () => new TypeSelector([typeof(ShapeFixtures.AuditService)]).Should().HaveNameMatching("["));
    }

    [TestMethod]
    public void ResideInNamespace_AliasesBeInNamespace() {
        new TypeSelector([typeof(ShapeFixtures.PublicSurface)]).Should().ResideInNamespace(ShapesNamespace);

        var ex = Throws<AssertionFailedException>(
            () => new TypeSelector([typeof(ShapeFixtures.PublicSurface)]).Should().ResideInNamespace("No.Such.Namespace"));
        ex.Message.Should().Contain("PublicSurface");
    }

    [TestMethod]
    public void OnlyHavePublicMembers_IgnoresPrivateImplementation() {
        new TypeSelector([typeof(ShapeFixtures.PublicSurface)]).Should().OnlyHavePublicMembers();

        var ex = Throws<AssertionFailedException>(
            () => new TypeSelector([typeof(ShapeFixtures.MixedVisibility)]).Should().OnlyHavePublicMembers());
        ex.Message.Should().Contain("MixedVisibility");
        (ex.Message.Contains("InternalValue", StringComparison.Ordinal) || ex.Message.Contains("ProtectedValue", StringComparison.Ordinal)).Should().BeTrue();
    }

    [TestMethod]
    public void NotExposeTypes_ReportsPublicSignatures() {
        new TypeSelector([typeof(ApplicationFixtures.OrderService)]).Should().NotExposeTypes(typeof(InfrastructureFixtures.SqlConnection));

        var ex = Throws<AssertionFailedException>(
            () => new TypeSelector([typeof(DomainFixtures.LeakyOrder)]).Should().NotExposeTypes(typeof(InfrastructureFixtures.SqlConnection)));
        ex.Message.Should().Contain("LeakyOrder");
        ex.Message.Should().Contain("Connection");
        ex.Message.Should().Contain("SqlConnection");

        Throws<ArgumentNullException>(
            () => new TypeSelector([typeof(DomainFixtures.LeakyOrder)]).Should().NotExposeTypes((Type[])null!));
        Throws<ArgumentException>(
            () => new TypeSelector([typeof(DomainFixtures.LeakyOrder)]).Should().NotExposeTypes());
    }

    [TestMethod]
    public void AllTypes_ComposesWithDependencyRules() {
        var assembly = typeof(DomainFixtures.Order).Assembly;

        AllTypes.From(assembly)
            .ThatAreUnderNamespace(DomainNamespace)
            .ThatHaveDependencyOn(InfrastructureNamespace)
            .Should().HaveCount(2);

        var ex = Throws<AssertionFailedException>(
            () => AllTypes.From(assembly)
                .ThatAreUnderNamespace(DomainNamespace)
                .Should().NotDependOn(InfrastructureNamespace, "the domain layer stays pure"));
        ex.Message.Should().Contain("LeakyOrder");

        AllTypes.From(assembly)
            .ThatAreUnderNamespace(DomainNamespace)
            .Should().HaveNoCycles();
    }

    /// <summary>Asserts the exact exception type through the library and returns the caught exception.</summary>
    /// <typeparam name="TException">The expected exception type.</typeparam>
    /// <param name="action">The action expected to throw.</param>
    /// <returns>The caught exception.</returns>
    private static TException Throws<TException>(Action action) where TException : Exception =>
        action.Should().ThrowExactly<TException>().Which;
}

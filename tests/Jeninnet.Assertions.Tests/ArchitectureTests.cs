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

        Assert.ThrowsExactly<ArgumentNullException>(() => selector.ThatHaveName(null!));
        Assert.ThrowsExactly<ArgumentNullException>(() => selector.ThatHaveNameStartingWith(null!));
        Assert.ThrowsExactly<ArgumentNullException>(() => selector.ThatHaveNameEndingWith(null!));
        Assert.ThrowsExactly<ArgumentNullException>(() => selector.ThatHaveNameMatching(null!));
        Assert.ThrowsExactly<RegexParseException>(() => selector.ThatHaveNameMatching("["));
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

        Assert.ThrowsExactly<ArgumentNullException>(() => selector.ThatAreAssignableTo((Type)null!));
        Assert.ThrowsExactly<ArgumentNullException>(() => selector.ThatAreAssignableFrom((Type)null!));
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

        Assert.ThrowsExactly<ArgumentNullException>(() => selector.ThatHaveDependencyOn(null!));
        Assert.ThrowsExactly<ArgumentException>(() => selector.ThatHaveDependencyOn("  "));
        Assert.ThrowsExactly<ArgumentNullException>(() => selector.ThatDoNotHaveDependencyOn(null!));
    }

    [TestMethod]
    public void Selectors_RejectNullArchitectureArguments() {
        var selection = new TypeSelector([typeof(DomainFixtures.Order)]).Should();
        Type[] exposedWithNull = [null!];

        Assert.ThrowsExactly<ArgumentNullException>(() => selection.HaveNameStartingWith(null!));
        Assert.ThrowsExactly<ArgumentNullException>(() => selection.HaveNameMatching(null!));
        Assert.ThrowsExactly<ArgumentNullException>(() => selection.NotDependOn((Type)null!));
        Assert.ThrowsExactly<ArgumentNullException>(() => selection.NotExposeTypes(exposedWithNull));
        Assert.ThrowsExactly<ArgumentNullException>(() => new TypeSelector([typeof(object)]).ThatAreInAssembly(null!));
        Assert.ThrowsExactly<ArgumentNullException>(() => new TypeSelector([typeof(object)]).ThatAreNotInAssembly(null!));
    }

    [TestMethod]
    public void NotDependOn_ReportsOffenders() {
        var selection = new TypeSelector([
            typeof(DomainFixtures.Order),
            typeof(DomainFixtures.LeakyOrder),
            typeof(DomainFixtures.AttributedLeak),
            typeof(DomainFixtures.MethodOnlyLeak),
        ]);

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => selection.Should().NotDependOn(InfrastructureNamespace));

        Assert.Contains("2 type(s) do", ex.Message);
        Assert.Contains("LeakyOrder", ex.Message);
        Assert.Contains("AttributedLeak", ex.Message);
        Assert.Contains("SqlConnection", ex.Message);
        Assert.DoesNotContain("MethodOnlyLeak", ex.Message);

        new TypeSelector([typeof(ApplicationFixtures.OrderService)]).Should().NotDependOn(InfrastructureNamespace);

        var typeEx = Assert.ThrowsExactly<AssertionFailedException>(
            () => new TypeSelector([typeof(DomainFixtures.LeakyOrder)]).Should().NotDependOn(typeof(InfrastructureFixtures.SqlConnection)));
        Assert.Contains("SqlConnection", typeEx.Message);

        Assert.ThrowsExactly<ArgumentNullException>(
            () => new TypeSelector([typeof(DomainFixtures.Order)]).Should().NotDependOn((string)null!));
        Assert.ThrowsExactly<ArgumentException>(
            () => new TypeSelector([typeof(DomainFixtures.Order)]).Should().NotDependOn("  "));
    }

    [TestMethod]
    public void OnlyDependOn_AllowsListedLayersAndReportsOthers() {
        var application = new TypeSelector([typeof(ApplicationFixtures.OrderService), typeof(ApplicationFixtures.PricingService)]);

        application.Should().OnlyDependOn(DomainNamespace, "System");
        application.Should().OnlyDependOn([DomainNamespace, "System"], "the application layer stays above domain and the BCL");

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => application.Should().OnlyDependOn(DomainNamespace));

        Assert.Contains("other dependencies", ex.Message);
        Assert.Contains("System", ex.Message);
        Assert.Contains("OrderService", ex.Message);
        Assert.Contains("PricingService", ex.Message);

        Assert.ThrowsExactly<ArgumentNullException>(() => application.Should().OnlyDependOn((string[])null!));
        Assert.ThrowsExactly<ArgumentException>(() => application.Should().OnlyDependOn());
        Assert.ThrowsExactly<ArgumentException>(() => application.Should().OnlyDependOn("  "));
    }

    [TestMethod]
    public void NotDependOnAny_MatchesEveryTarget() {
        var selection = new TypeSelector([typeof(DomainFixtures.LeakyOrder)]);

        var ex = Assert.ThrowsExactly<AssertionFailedException>(
            () => selection.Should().NotDependOnAny(InfrastructureNamespace, "System.Data"));
        Assert.Contains("any of", ex.Message);
        Assert.Contains("LeakyOrder", ex.Message);

        selection.Should().NotDependOnAny("No.Such.Namespace");

        Assert.ThrowsExactly<ArgumentNullException>(() => selection.Should().NotDependOnAny((string[])null!));
        Assert.ThrowsExactly<ArgumentException>(() => selection.Should().NotDependOnAny());
        Assert.ThrowsExactly<ArgumentException>(() => selection.Should().NotDependOnAny("  "));
    }

    [TestMethod]
    public void DependOn_RequiresEveryTypeToHaveTheDependency() {
        new TypeSelector([typeof(ApplicationFixtures.OrderService)]).Should().DependOn(DomainNamespace);
        new TypeSelector([typeof(ApplicationFixtures.OrderService)]).Should().DependOn(typeof(DomainFixtures.IOrderRepository));

        var ex = Assert.ThrowsExactly<AssertionFailedException>(
            () => new TypeSelector([typeof(DomainFixtures.Order)]).Should().DependOn(InfrastructureNamespace));
        Assert.Contains("do not", ex.Message);
        Assert.Contains("Order", ex.Message);

        Assert.ThrowsExactly<ArgumentNullException>(
            () => new TypeSelector([typeof(DomainFixtures.Order)]).Should().DependOn((string)null!));
        Assert.ThrowsExactly<ArgumentException>(
            () => new TypeSelector([typeof(DomainFixtures.Order)]).Should().DependOn("  "));
        Assert.ThrowsExactly<ArgumentNullException>(
            () => new TypeSelector([typeof(DomainFixtures.Order)]).Should().DependOn((Type)null!));
    }

    [TestMethod]
    public void DependOnAny_RequiresAtLeastOneTarget() {
        new TypeSelector([typeof(ApplicationFixtures.OrderService)]).Should().DependOnAny(DomainNamespace, "No.Such.Namespace");

        var ex = Assert.ThrowsExactly<AssertionFailedException>(
            () => new TypeSelector([typeof(DomainFixtures.Order)]).Should().DependOnAny(InfrastructureNamespace, "No.Such.Namespace"));
        Assert.Contains("at least one", ex.Message);
        Assert.Contains("Order", ex.Message);

        Assert.ThrowsExactly<ArgumentNullException>(
            () => new TypeSelector([typeof(DomainFixtures.Order)]).Should().DependOnAny((string[])null!));
        Assert.ThrowsExactly<ArgumentException>(
            () => new TypeSelector([typeof(DomainFixtures.Order)]).Should().DependOnAny());
        Assert.ThrowsExactly<ArgumentException>(
            () => new TypeSelector([typeof(DomainFixtures.Order)]).Should().DependOnAny("  "));
    }

    [TestMethod]
    public void OnlyBeDependedOnBy_ChecksIncomingSources() {
        var order = new TypeSelector([typeof(DomainFixtures.Order)]);

        order.Should().OnlyBeDependedOnBy(ApplicationNamespace, InfrastructureNamespace, DomainNamespace);
        order.Should().OnlyBeDependedOnBy([ApplicationNamespace, InfrastructureNamespace, DomainNamespace], "the domain is the bottom layer");

        var ex = Assert.ThrowsExactly<AssertionFailedException>(
            () => order.Should().OnlyBeDependedOnBy(ApplicationNamespace, InfrastructureNamespace));
        Assert.Contains("only by", ex.Message);
        Assert.Contains("IOrderRepository", ex.Message);
        Assert.Contains("Quote", ex.Message);
        Assert.DoesNotContain("OrderService", ex.Message);
        Assert.DoesNotContain("SqlOrderRepository", ex.Message);

        Assert.ThrowsExactly<ArgumentNullException>(() => order.Should().OnlyBeDependedOnBy((string[])null!));
        Assert.ThrowsExactly<ArgumentException>(() => order.Should().OnlyBeDependedOnBy());
        Assert.ThrowsExactly<ArgumentException>(() => order.Should().OnlyBeDependedOnBy("  "));
    }

    [TestMethod]
    public void NotBeDependedOnBy_MatchesSourcesAndIgnoresInternalDependencies() {
        new TypeSelector([typeof(DomainFixtures.Order)]).Should().NotBeDependedOnBy("No.Such.Namespace");

        var ex = Assert.ThrowsExactly<AssertionFailedException>(
            () => new TypeSelector([typeof(DomainFixtures.Order)]).Should().NotBeDependedOnBy(ApplicationNamespace));
        Assert.Contains("OrderService", ex.Message);

        var internalSelection = new TypeSelector([typeof(DomainFixtures.IOrderRepository), typeof(InfrastructureFixtures.SqlOrderRepository)]);
        internalSelection.Should().NotBeDependedOnBy(DomainNamespace);

        Assert.ThrowsExactly<ArgumentNullException>(
            () => new TypeSelector([typeof(DomainFixtures.Order)]).Should().NotBeDependedOnBy(null!));
        Assert.ThrowsExactly<ArgumentException>(
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
        var ex = Assert.ThrowsExactly<AssertionFailedException>(
            () => selection.Should().FollowLayers(domain, reversed, infrastructure));
        Assert.Contains("Application -> Domain", ex.Message);
        Assert.Contains("OrderService", ex.Message);
    }

    [TestMethod]
    public void FollowLayers_UsesTheLongestMatchingNamespacePrefix() {
        var domain = new ArchitectureLayer("Domain", DomainNamespace);
        var sales = new ArchitectureLayer("Sales", typeof(DomainFixtures.Sales.Quote).Namespace!).DependsOn("Domain");
        var selection = new TypeSelector([typeof(DomainFixtures.OrderWithQuote), typeof(DomainFixtures.Sales.Quote)]);

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => selection.Should().FollowLayers(domain, sales));
        Assert.Contains("Domain -> Sales", ex.Message);
        Assert.Contains("OrderWithQuote", ex.Message);

        new TypeSelector([typeof(DomainFixtures.Sales.Quote)]).Should().FollowLayers(domain, sales);
    }

    [TestMethod]
    public void FollowLayers_IgnoresFrameworkNamespacesByDefault() {
        var shapes = new ArchitectureLayer("Shapes", ShapesNamespace);
        var selection = new TypeSelector([typeof(ShapeFixtures.PublicSurface)]);

        selection.Should().FollowLayers(shapes);

        var ex = Assert.ThrowsExactly<AssertionFailedException>(
            () => selection.Should().FollowLayers([shapes], includingFrameworkDependencies: true));
        Assert.Contains("Shapes -> Framework", ex.Message);

        var frameworkAware = new ArchitectureLayer("Shapes", ShapesNamespace).DependsOn(ArchitectureLayer.FrameworkLayerName);
        selection.Should().FollowLayers([frameworkAware], includingFrameworkDependencies: true);
    }

    [TestMethod]
    public void FollowLayers_ValidatesLayerDefinitions() {
        var selection = new TypeSelector([typeof(DomainFixtures.Order)]);

        Assert.ThrowsExactly<ArgumentNullException>(() => selection.Should().FollowLayers((ArchitectureLayer[])null!));
        Assert.ThrowsExactly<ArgumentException>(() => selection.Should().FollowLayers());
        ArchitectureLayer[] layersWithNull = [null!];
        Assert.ThrowsExactly<ArgumentNullException>(() => selection.Should().FollowLayers(layersWithNull));
        Assert.ThrowsExactly<ArgumentException>(() => selection.Should().FollowLayers(
            new ArchitectureLayer("A", DomainNamespace),
            new ArchitectureLayer("A", ApplicationNamespace)));
        Assert.ThrowsExactly<ArgumentException>(() => selection.Should().FollowLayers(
            new ArchitectureLayer("A", DomainNamespace),
            new ArchitectureLayer("B", DomainNamespace)));
        Assert.ThrowsExactly<ArgumentException>(() => selection.Should().FollowLayers(
            new ArchitectureLayer("A", DomainNamespace).DependsOn("Missing")));
    }

    [TestMethod]
    public void ArchitectureLayer_ValidatesNamePrefixAndDependencies() {
        Assert.ThrowsExactly<ArgumentNullException>(() => new ArchitectureLayer(null!, DomainNamespace));
        Assert.ThrowsExactly<ArgumentException>(() => new ArchitectureLayer(" ", DomainNamespace));
        Assert.ThrowsExactly<ArgumentNullException>(() => new ArchitectureLayer("Domain", null!));
        Assert.ThrowsExactly<ArgumentException>(() => new ArchitectureLayer("Domain", "  "));

        var layer = new ArchitectureLayer(" Domain ", " " + DomainNamespace + " ");
        Assert.AreEqual("Domain", layer.Name);
        Assert.AreEqual(DomainNamespace, layer.NamespacePrefix);
        Assert.AreSame(layer, layer.DependsOn("Application"));
        Assert.ThrowsExactly<ArgumentNullException>(() => layer.DependsOn((string[])null!));
        Assert.ThrowsExactly<ArgumentException>(() => layer.DependsOn());
        Assert.ThrowsExactly<ArgumentException>(() => layer.DependsOn(" "));
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

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => selection.Should().HaveNoCycles());

        Assert.Contains("2 cycle(s)", ex.Message);
        var cycleA = typeof(CycleFixtures.CycleA).FullName;
        var cycleB = typeof(CycleFixtures.CycleB).FullName;
        var selfCycle = typeof(CycleFixtures.SelfCycle).FullName;
        Assert.Contains($"{cycleA} -> {cycleB} -> {cycleA}", ex.Message);
        Assert.Contains($"{selfCycle} -> {selfCycle}", ex.Message);

        new TypeSelector([typeof(CycleFixtures.CycleLeaf), typeof(CycleFixtures.NoCycleRoot)]).Should().HaveNoCycles();
    }

    [TestMethod]
    public void BePublicAndBeInternal_ReportOffenders() {
        new TypeSelector([typeof(ShapeFixtures.PublicSurface)]).Should().BePublic();
        new TypeSelector([typeof(ShapeFixtures.InternalThing)]).Should().BeInternal();

        var publicEx = Assert.ThrowsExactly<AssertionFailedException>(
            () => new TypeSelector([typeof(ShapeFixtures.InternalThing)]).Should().BePublic());
        Assert.Contains("1 type(s) are not", publicEx.Message);
        Assert.Contains("InternalThing", publicEx.Message);

        var internalEx = Assert.ThrowsExactly<AssertionFailedException>(
            () => new TypeSelector([typeof(ShapeFixtures.PublicSurface)]).Should().BeInternal());
        Assert.Contains("PublicSurface", internalEx.Message);
    }

    [TestMethod]
    public void BeNested_ReportsOffenders() {
        new TypeSelector([typeof(ShapeFixtures.Outer.NestedPublic)]).Should().BeNested();
        new TypeSelector([typeof(ShapeFixtures.AuditService)]).Should().NotBeNested();

        var ex = Assert.ThrowsExactly<AssertionFailedException>(
            () => new TypeSelector([typeof(ShapeFixtures.AuditService)]).Should().BeNested());
        Assert.Contains("AuditService", ex.Message);
    }

    [TestMethod]
    public void BeRecordAndBeImmutable_UseDocumentedHeuristics() {
        new TypeSelector([typeof(ShapeFixtures.Person), typeof(ShapeFixtures.Point)]).Should().BeRecord();
        new TypeSelector([typeof(ShapeFixtures.ImmutableBox), typeof(ShapeFixtures.Point)]).Should().BeImmutable();
        new TypeSelector([typeof(ShapeFixtures.AuditService)]).Should().NotBeRecord();
        new TypeSelector([typeof(ShapeFixtures.MutableBox)]).Should().NotBeImmutable();

        var recordEx = Assert.ThrowsExactly<AssertionFailedException>(
            () => new TypeSelector([typeof(ShapeFixtures.AuditService)]).Should().BeRecord());
        Assert.Contains("AuditService", recordEx.Message);

        var immutableEx = Assert.ThrowsExactly<AssertionFailedException>(
            () => new TypeSelector([typeof(ShapeFixtures.MutableBox)]).Should().BeImmutable());
        Assert.Contains("MutableBox", immutableEx.Message);
    }

    [TestMethod]
    public void NameAssertions_ReportOffenders() {
        new TypeSelector([typeof(ShapeFixtures.PaymentHandler)]).Should().HaveNameStartingWith("Pay");
        new TypeSelector([typeof(ShapeFixtures.PaymentHandler), typeof(ShapeFixtures.OrderHandler)])
            .Should().HaveNameEndingWith("Handler");
        new TypeSelector([typeof(ShapeFixtures.OrderHandler)]).Should().HaveNameMatching("^Order.*$");

        var ex = Assert.ThrowsExactly<AssertionFailedException>(
            () => new TypeSelector([typeof(ShapeFixtures.PaymentHandler), typeof(ShapeFixtures.AuditService)])
                .Should().HaveNameEndingWith("Handler"));
        Assert.Contains("1 type(s) do not", ex.Message);
        Assert.Contains("AuditService", ex.Message);

        Assert.ThrowsExactly<ArgumentNullException>(
            () => new TypeSelector([typeof(ShapeFixtures.AuditService)]).Should().HaveNameEndingWith(null!));
        Assert.ThrowsExactly<RegexParseException>(
            () => new TypeSelector([typeof(ShapeFixtures.AuditService)]).Should().HaveNameMatching("["));
    }

    [TestMethod]
    public void ResideInNamespace_AliasesBeInNamespace() {
        new TypeSelector([typeof(ShapeFixtures.PublicSurface)]).Should().ResideInNamespace(ShapesNamespace);

        var ex = Assert.ThrowsExactly<AssertionFailedException>(
            () => new TypeSelector([typeof(ShapeFixtures.PublicSurface)]).Should().ResideInNamespace("No.Such.Namespace"));
        Assert.Contains("PublicSurface", ex.Message);
    }

    [TestMethod]
    public void OnlyHavePublicMembers_IgnoresPrivateImplementation() {
        new TypeSelector([typeof(ShapeFixtures.PublicSurface)]).Should().OnlyHavePublicMembers();

        var ex = Assert.ThrowsExactly<AssertionFailedException>(
            () => new TypeSelector([typeof(ShapeFixtures.MixedVisibility)]).Should().OnlyHavePublicMembers());
        Assert.Contains("MixedVisibility", ex.Message);
        Assert.IsTrue(ex.Message.Contains("InternalValue", StringComparison.Ordinal) || ex.Message.Contains("ProtectedValue", StringComparison.Ordinal));
    }

    [TestMethod]
    public void NotExposeTypes_ReportsPublicSignatures() {
        new TypeSelector([typeof(ApplicationFixtures.OrderService)]).Should().NotExposeTypes(typeof(InfrastructureFixtures.SqlConnection));

        var ex = Assert.ThrowsExactly<AssertionFailedException>(
            () => new TypeSelector([typeof(DomainFixtures.LeakyOrder)]).Should().NotExposeTypes(typeof(InfrastructureFixtures.SqlConnection)));
        Assert.Contains("LeakyOrder", ex.Message);
        Assert.Contains("Connection", ex.Message);
        Assert.Contains("SqlConnection", ex.Message);

        Assert.ThrowsExactly<ArgumentNullException>(
            () => new TypeSelector([typeof(DomainFixtures.LeakyOrder)]).Should().NotExposeTypes((Type[])null!));
        Assert.ThrowsExactly<ArgumentException>(
            () => new TypeSelector([typeof(DomainFixtures.LeakyOrder)]).Should().NotExposeTypes());
    }

    [TestMethod]
    public void AllTypes_ComposesWithDependencyRules() {
        var assembly = typeof(DomainFixtures.Order).Assembly;

        AllTypes.From(assembly)
            .ThatAreUnderNamespace(DomainNamespace)
            .ThatHaveDependencyOn(InfrastructureNamespace)
            .Should().HaveCount(2);

        var ex = Assert.ThrowsExactly<AssertionFailedException>(
            () => AllTypes.From(assembly)
                .ThatAreUnderNamespace(DomainNamespace)
                .Should().NotDependOn(InfrastructureNamespace, "the domain layer stays pure"));
        Assert.Contains("LeakyOrder", ex.Message);

        AllTypes.From(assembly)
            .ThatAreUnderNamespace(DomainNamespace)
            .Should().HaveNoCycles();
    }
}

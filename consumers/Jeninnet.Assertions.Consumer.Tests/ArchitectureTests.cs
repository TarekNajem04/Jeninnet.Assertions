using Jeninnet.Assertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jeninnet.Assertions.Consumer.Tests;

[TestClass]
public class ArchitectureTests {
    [TestMethod]
    public void Domain_records_are_sealed() =>
        typeof(Order).Should().BeSealed().And.NotBeAbstract();

    [TestMethod]
    public void The_order_service_is_sealed() =>
        typeof(OrderService).Should().BeSealed();

    [TestMethod]
    public void The_payment_gateway_implements_its_contract() =>
        typeof(PaymentGateway).Should().BeAssignableTo<IPaymentGateway>();

    [TestMethod]
    public void The_order_service_exposes_a_public_dependency_constructor() =>
        typeof(OrderService).Should().HaveConstructor(typeof(IPaymentGateway), typeof(ShippingClient)).Which.Should().BePublic();

    [TestMethod]
    public void The_assembly_defines_the_order_type() =>
        typeof(OrderService).Assembly.Should().DefineType(typeof(Order).FullName!).Which.FullName.Should().Be(typeof(Order).FullName);
}

using Jeninnet.Assertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jeninnet.Assertions.Consumer.Tests;

[TestClass]
public class ValidationTests {
    [TestMethod]
    public void A_valid_order_produces_no_violations() =>
        OrderValidator.Validate(TestOrders.SampleOrder()).Should().BeEmpty();

    [TestMethod]
    public void An_order_without_lines_is_rejected() =>
        OrderValidator.Validate(new Order("C-1", [])).Should().Contain("The order must contain at least one line.");

    [TestMethod]
    public void Lines_require_positive_quantities() =>
        OrderValidator.Validate(new Order("C-1", [new OrderLine("WIDGET", 0, 10m)]))
            .Should().Contain("Line WIDGET must have a positive quantity.");

    [TestMethod]
    public void Lines_require_positive_unit_prices() =>
        OrderValidator.Validate(new Order("C-1", [new OrderLine("WIDGET", 1, 0m)]))
            .Should().Contain("Line WIDGET must have a positive unit price.");

    [TestMethod]
    public void The_customer_id_must_not_be_empty() =>
        OrderValidator.Validate(new Order(" ", [new OrderLine("WIDGET", 1, 10m)]))
            .Should().Contain("The customer id must not be empty.");
}

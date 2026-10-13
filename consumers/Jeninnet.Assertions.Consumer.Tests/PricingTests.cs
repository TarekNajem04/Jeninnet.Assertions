using Jeninnet.Assertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jeninnet.Assertions.Consumer.Tests;

[TestClass]
public class PricingTests {
    [TestMethod]
    public void Subtotal_is_the_sum_of_line_totals() =>
        PriceCalculator.Calculate(TestOrders.SampleOrder()).Subtotal.Should().Be(130m);

    [TestMethod]
    public void Volume_discount_applies_at_the_threshold() {
        var price = PriceCalculator.Calculate(TestOrders.SampleOrder());

        price.Discount.Should().Be(13m, "orders at or above 100 receive a 10% volume discount");
    }

    [TestMethod]
    public void Tax_is_computed_on_the_discounted_amount() =>
        PriceCalculator.Calculate(TestOrders.SampleOrder()).Tax.Should().Be(16.38m);

    [TestMethod]
    public void Total_is_subtotal_minus_discount_plus_tax() =>
        PriceCalculator.Calculate(TestOrders.SampleOrder()).Total.Should().Be(133.38m).And.BeGreaterThan(0m);

    [TestMethod]
    public void Small_orders_receive_no_discount() {
        var order = new Order("C-1", [new OrderLine("STICKER", 1, 5m)]);

        PriceCalculator.Calculate(order).Discount.Should().Be(0m);
    }
}

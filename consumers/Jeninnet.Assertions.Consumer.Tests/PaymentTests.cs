using Jeninnet.Assertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jeninnet.Assertions.Consumer.Tests;

[TestClass]
public class PaymentTests {
    public TestContext TestContext { get; set; } = null!;

    [TestMethod]
    public async Task A_small_charge_completes_within_the_time_budget() {
        var gateway = new PaymentGateway();

        Func<Task> charge = () => gateway.ChargeAsync("C-1", 133.38m, TestContext.CancellationToken);

        await charge.Should().CompleteWithinAsync(TimeSpan.FromSeconds(5));
    }

    [TestMethod]
    public async Task Charges_above_the_threshold_are_declined() {
        var gateway = new PaymentGateway();

        Func<Task> charge = () => gateway.ChargeAsync("C-1", PaymentGateway.DECLINED_THRESHOLD + 1m, TestContext.CancellationToken);

        var thrown = await charge.Should().ThrowAsync<PaymentDeclinedException>();

        thrown.Exception.Message.Should().Contain("declined");
    }

    [TestMethod]
    public async Task Successful_charges_return_a_deterministic_receipt() {
        var gateway = new PaymentGateway();

        var receipt = await gateway.ChargeAsync("C-7", 10m, TestContext.CancellationToken);

        receipt.TransactionId.Should().Be("R-C-7-10");
    }
}

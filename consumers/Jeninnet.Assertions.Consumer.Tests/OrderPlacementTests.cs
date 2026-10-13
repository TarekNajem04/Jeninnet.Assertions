using System.Text.Json.Nodes;
using Jeninnet.Assertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jeninnet.Assertions.Consumer.Tests;

[TestClass]
public class OrderPlacementTests {
    public TestContext TestContext { get; set; } = null!;

    private static OrderService CreateService(HttpClient httpClient) =>
        new(new PaymentGateway(), new ShippingClient(httpClient));

    [TestMethod]
    public async Task Placing_an_order_produces_an_invoice_and_receipt() {
        using var httpClient = new HttpClient(new StubShippingHandler());
        var service = CreateService(httpClient);

        var confirmation = await service.PlaceOrderAsync(TestOrders.SampleOrder(), TestContext.CancellationToken);

        confirmation.TrackingNumber.Should().Be("TRACK-42");
        confirmation.TransactionId.Should().Be("R-C-42-133.38");
        confirmation.Invoice.Should().Contain("INVOICE").And.Contain("Total: 133.38");
    }

    [TestMethod]
    public async Task Placing_an_order_raises_the_order_placed_event() {
        using var httpClient = new HttpClient(new StubShippingHandler());
        var service = CreateService(httpClient);
        var placed = new List<OrderPlacedEventArgs>();
        service.OrderPlaced += (_, args) => placed.Add(args);

        using var monitor = service.Monitor();

        await service.PlaceOrderAsync(TestOrders.SampleOrder(), TestContext.CancellationToken);

        monitor.Should().Raise("OrderPlaced");
        placed.Should().HaveCount(1);
        placed[0].Total.Should().Be(133.38m);
        placed[0].TrackingNumber.Should().Be("TRACK-42");
    }

    [TestMethod]
    public async Task Placing_an_order_stays_within_the_time_budget() {
        using var httpClient = new HttpClient(new StubShippingHandler());
        var service = CreateService(httpClient);

        Func<Task> place = () => service.PlaceOrderAsync(TestOrders.SampleOrder(), TestContext.CancellationToken);

        await place.Should().CompleteWithinAsync(TimeSpan.FromSeconds(5));
    }

    [TestMethod]
    public async Task The_invoice_matches_the_verified_snapshot() {
        using var httpClient = new HttpClient(new StubShippingHandler());
        var service = CreateService(httpClient);

        var confirmation = await service.PlaceOrderAsync(TestOrders.SampleOrder(), TestContext.CancellationToken);

        confirmation.Invoice.Should().MatchSnapshot("order-invoice");
    }

    [TestMethod]
    public void The_json_export_exposes_the_invoice_fields() {
        var order = TestOrders.SampleOrder();
        var price = PriceCalculator.Calculate(order);

        var json = InvoiceRenderer.ExportJson(order, price, "TRACK-42");
        var node = JsonNode.Parse(json)!;

        node.Should().HaveJsonPath("$.customerId").Which.Should().HaveValue("C-42");
        node.Should().HaveJsonPath("$.trackingNumber").Which.Should().HaveValue("TRACK-42");
        node.Should().HaveJsonPath("$.lines[0].sku").Which.Should().HaveValue("WIDGET");
    }
}

using Jeninnet.Assertions.Consumer;

// End-to-end run of the consumer pipeline against a stubbed shipping provider.
var handler = new StubShippingHandler();
var service = new OrderService(new PaymentGateway(), new ShippingClient(new HttpClient(handler)));
var order = new Order("C-42", [new OrderLine("WIDGET", 2, 60m)]);

var confirmation = await service.PlaceOrderAsync(order);

await Console.Out.WriteLineAsync(confirmation.Invoice);
await Console.Out.WriteLineAsync("Transaction: " + confirmation.TransactionId);

return 0;

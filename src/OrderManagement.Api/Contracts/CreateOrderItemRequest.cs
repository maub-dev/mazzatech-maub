namespace OrderManagement.Api.Contracts;

public sealed record CreateOrderItemRequest(
    string ProductName,
    int Quantity,
    decimal UnitPrice);

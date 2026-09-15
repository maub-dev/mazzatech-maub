namespace OrderManagement.Application.Orders;

public sealed record CreateOrderItem(string ProductName, int Quantity, decimal UnitPrice);

using OrderManagement.Domain;

namespace OrderManagement.Application.Orders;

public sealed record OrderDto(
    Guid Id,
    Guid CustomerId,
    OrderStatus Status,
    DateTime CreatedAt,
    decimal TotalAmount,
    IReadOnlyCollection<OrderItemDto> Items);

using MediatR;

namespace OrderManagement.Application.Orders;

public sealed record CreateOrderCommand(
    Guid CustomerId,
    IReadOnlyCollection<CreateOrderItem> Items) : IRequest<OrderDto>;

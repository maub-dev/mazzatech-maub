using MediatR;

namespace OrderManagement.Application.Orders;

public sealed record CancelOrderCommand(Guid Id) : IRequest<OrderDto?>;

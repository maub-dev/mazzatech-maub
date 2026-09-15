using MediatR;

namespace OrderManagement.Application.Orders;

public sealed record GetOrderByIdQuery(Guid Id) : IRequest<OrderDto?>;

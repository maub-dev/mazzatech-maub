using MediatR;

namespace OrderManagement.Application.Orders;

public sealed record GetOrdersQuery(int Page, int PageSize) : IRequest<PagedOrdersDto>;

using MediatR;
using OrderManagement.Application.Abstractions;

namespace OrderManagement.Application.Orders;

public sealed class GetOrdersQueryHandler(IOrderRepository orderRepository)
    : IRequestHandler<GetOrdersQuery, PagedOrdersDto>
{
    public async Task<PagedOrdersDto> Handle(GetOrdersQuery request, CancellationToken cancellationToken)
    {
        var page = await orderRepository.GetPagedAsync(request.Page, request.PageSize, cancellationToken);
        return new PagedOrdersDto(
            page.Items.Select(order => order.ToDto()).ToArray(),
            request.Page,
            request.PageSize,
            page.TotalCount);
    }
}

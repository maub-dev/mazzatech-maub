namespace OrderManagement.Application.Orders;

public sealed record PagedOrdersDto(
    IReadOnlyCollection<OrderDto> Items,
    int Page,
    int PageSize,
    int TotalCount);

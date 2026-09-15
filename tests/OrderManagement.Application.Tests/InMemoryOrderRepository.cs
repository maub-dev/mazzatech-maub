using OrderManagement.Application.Abstractions;
using OrderManagement.Domain;

namespace OrderManagement.Application.Tests;

internal sealed class InMemoryOrderRepository(params Order[] orders) : IOrderRepository
{
    public List<Order> Orders { get; } = orders.ToList();
    public bool SaveChangesCalled { get; private set; }

    public Task AddAsync(Order order, CancellationToken cancellationToken)
    {
        Orders.Add(order);
        return Task.CompletedTask;
    }

    public Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken) =>
        Task.FromResult(Orders.SingleOrDefault(order => order.Id == id));

    public Task<PagedResult<Order>> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken)
    {
        var ordersPage = Orders.Skip((page - 1) * pageSize).Take(pageSize).ToArray();
        return Task.FromResult(new PagedResult<Order>(ordersPage, Orders.Count));
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        SaveChangesCalled = true;
        return Task.CompletedTask;
    }
}

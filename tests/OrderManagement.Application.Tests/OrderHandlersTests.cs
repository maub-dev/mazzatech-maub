using OrderManagement.Application.Abstractions;
using OrderManagement.Application.Orders;
using OrderManagement.Domain;

namespace OrderManagement.Application.Tests;

public sealed class OrderHandlersTests
{
    [Fact]
    public async Task CreateOrder_persists_order_and_calculates_domain_total()
    {
        var repository = new InMemoryOrderRepository();
        var handler = new CreateOrderCommandHandler(repository);

        var result = await handler.Handle(
            new CreateOrderCommand(
                Guid.NewGuid(),
                [new CreateOrderItem("Keyboard", 2, 99.95m), new CreateOrderItem("Mouse", 1, 49.90m)]),
            CancellationToken.None);

        Assert.Equal(249.80m, result.TotalAmount);
        Assert.Equal(OrderStatus.Pending, result.Status);
        Assert.Single(repository.Orders);
        Assert.True(repository.SaveChangesCalled);
    }

    [Fact]
    public async Task CreateOrder_does_not_persist_when_response_mapping_overflows()
    {
        var repository = new InMemoryOrderRepository();
        var handler = new CreateOrderCommandHandler(repository);

        await Assert.ThrowsAsync<OverflowException>(() =>
            handler.Handle(
                new CreateOrderCommand(
                    Guid.NewGuid(),
                    [new CreateOrderItem("Large item", int.MaxValue, decimal.MaxValue)]),
                CancellationToken.None));

        Assert.Empty(repository.Orders);
        Assert.False(repository.SaveChangesCalled);
    }

    [Fact]
    public async Task CancelOrder_cancels_a_pending_order()
    {
        var order = Order.Create(Guid.NewGuid(), [("Keyboard", 1, 99.95m)]);
        var repository = new InMemoryOrderRepository(order);
        var handler = new CancelOrderCommandHandler(repository);

        var result = await handler.Handle(new CancelOrderCommand(order.Id), CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(OrderStatus.Cancelled, result.Status);
        Assert.True(repository.SaveChangesCalled);
    }

    [Fact]
    public async Task GetOrders_returns_requested_page_and_total_count()
    {
        var firstOrder = Order.Create(Guid.NewGuid(), [("Keyboard", 1, 99.95m)]);
        var secondOrder = Order.Create(Guid.NewGuid(), [("Mouse", 1, 49.90m)]);
        var repository = new InMemoryOrderRepository(firstOrder, secondOrder);
        var handler = new GetOrdersQueryHandler(repository);

        var result = await handler.Handle(new GetOrdersQuery(1, 1), CancellationToken.None);

        Assert.Single(result.Items);
        Assert.Equal(2, result.TotalCount);
        Assert.Equal(1, result.Page);
        Assert.Equal(1, result.PageSize);
    }

}

using MediatR;
using OrderManagement.Application.Abstractions;
using OrderManagement.Domain;

namespace OrderManagement.Application.Orders;

public sealed class CreateOrderCommandHandler(IOrderRepository orderRepository)
    : IRequestHandler<CreateOrderCommand, OrderDto>
{
    public async Task<OrderDto> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var order = Order.Create(
            request.CustomerId,
            request.Items.Select(item => (item.ProductName, item.Quantity, item.UnitPrice)));

        await orderRepository.AddAsync(order, cancellationToken);
        await orderRepository.SaveChangesAsync(cancellationToken);

        return order.ToDto();
    }
}

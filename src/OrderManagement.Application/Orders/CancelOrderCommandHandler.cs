using MediatR;
using OrderManagement.Application.Abstractions;

namespace OrderManagement.Application.Orders;

public sealed class CancelOrderCommandHandler(IOrderRepository orderRepository)
    : IRequestHandler<CancelOrderCommand, OrderDto?>
{
    public async Task<OrderDto?> Handle(CancelOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await orderRepository.GetByIdAsync(request.Id, cancellationToken);
        if (order is null)
        {
            return null;
        }

        order.Cancel();
        await orderRepository.SaveChangesAsync(cancellationToken);

        return order.ToDto();
    }
}

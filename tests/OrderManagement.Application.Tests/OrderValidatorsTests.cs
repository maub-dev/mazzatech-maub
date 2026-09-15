using OrderManagement.Application.Orders;

namespace OrderManagement.Application.Tests;

public sealed class OrderValidatorsTests
{
    [Fact]
    public void CreateOrder_rejects_empty_order_and_invalid_items()
    {
        var validator = new CreateOrderCommandValidator();
        var command = new CreateOrderCommand(
            Guid.Empty,
            [new CreateOrderItem(string.Empty, 0, 0)]);

        var result = validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == "CustomerId");
        Assert.Contains(result.Errors, error => error.PropertyName == "Items[0].ProductName");
        Assert.Contains(result.Errors, error => error.PropertyName == "Items[0].Quantity");
        Assert.Contains(result.Errors, error => error.PropertyName == "Items[0].UnitPrice");
    }

    [Fact]
    public void Identifier_validators_reject_empty_identifiers()
    {
        var getResult = new GetOrderByIdQueryValidator().Validate(new GetOrderByIdQuery(Guid.Empty));
        var cancelResult = new CancelOrderCommandValidator().Validate(new CancelOrderCommand(Guid.Empty));

        Assert.False(getResult.IsValid);
        Assert.False(cancelResult.IsValid);
    }
}

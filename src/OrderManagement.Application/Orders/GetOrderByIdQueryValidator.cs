using FluentValidation;

namespace OrderManagement.Application.Orders;

public sealed class GetOrderByIdQueryValidator : AbstractValidator<GetOrderByIdQuery>
{
    public GetOrderByIdQueryValidator()
    {
        RuleFor(query => query.Id).NotEmpty();
    }
}

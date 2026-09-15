namespace OrderManagement.Domain;

public sealed class Order
{
    private readonly List<OrderItem> _items = [];

    private Order()
    {
    }

    private Order(Guid customerId, IEnumerable<(string ProductName, int Quantity, decimal UnitPrice)> items)
    {
        Id = Guid.NewGuid();
        CustomerId = customerId;
        Status = OrderStatus.Pending;
        CreatedAt = DateTime.UtcNow;

        foreach (var item in items)
        {
            _items.Add(new OrderItem(Id, item.ProductName, item.Quantity, item.UnitPrice));
        }
    }

    public Guid Id { get; private set; }
    public Guid CustomerId { get; private set; }
    public OrderStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();
    public decimal TotalAmount => _items.Sum(item => item.UnitPrice * item.Quantity);

    public static Order Create(
        Guid customerId,
        IEnumerable<(string ProductName, int Quantity, decimal UnitPrice)> items) =>
        new(customerId, items);

    public void Cancel()
    {
        if (Status != OrderStatus.Pending)
        {
            throw new OrderConflictException("Only pending orders can be cancelled.");
        }

        Status = OrderStatus.Cancelled;
    }
}

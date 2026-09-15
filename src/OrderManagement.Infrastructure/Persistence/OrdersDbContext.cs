using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using OrderManagement.Domain;

namespace OrderManagement.Infrastructure.Persistence;

public sealed class OrdersDbContext(DbContextOptions<OrdersDbContext> options) : DbContext(options)
{
    public DbSet<Order> Orders => Set<Order>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var order = modelBuilder.Entity<Order>();
        order.ToTable("Orders");
        order.HasKey(entity => entity.Id);
        order.Property(entity => entity.CustomerId).IsRequired();
        order.Property(entity => entity.Status).HasConversion<string>().HasMaxLength(20).IsRequired();
        order.Property(entity => entity.CreatedAt).IsRequired();
        order.HasMany(entity => entity.Items)
            .WithOne()
            .HasForeignKey(entity => entity.OrderId)
            .OnDelete(DeleteBehavior.Cascade);
        order.Navigation(entity => entity.Items).UsePropertyAccessMode(PropertyAccessMode.Field);

        var item = modelBuilder.Entity<OrderItem>();
        item.ToTable("OrderItems");
        item.HasKey(entity => entity.Id);
        item.Property(entity => entity.ProductName).HasMaxLength(200).IsRequired();
        item.Property(entity => entity.Quantity).IsRequired();
        item.Property(entity => entity.UnitPrice)
            .HasConversion(new ValueConverter<decimal, string>(
                value => value.ToString(CultureInfo.InvariantCulture),
                value => decimal.Parse(value, CultureInfo.InvariantCulture)))
            .HasColumnType("TEXT")
            .IsRequired();
    }
}

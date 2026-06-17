using DarkKitchen.Domain.Enums;

namespace DarkKitchen.Domain.Entities;

public class Order
{
    public int Id { get; set; }
    public int ClientId { get; set; }
    public int ShippingTypeId { get; set; }
    public required Address Address { get; set; }
    public string Status { get; set; } = nameof(OrderStatus.Pending);
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public decimal Subtotal { get; set; }
    public decimal Discount { get; set; }
    public decimal Iva { get; set; }
    public decimal ShippingCost { get; set; }
    public decimal Total { get; set; }
    public List<OrderProduct> Products { get; set; } = [];
}

namespace DarkKitchen.Domain.Entities;

public class ProductImage
{
    public int Id { get; set; }
    public string Url { get; set; } = string.Empty;
    public int ProductId { get; set; }
}

namespace DarkKitchen.Domain.Entities;

public class ProductFilter
{
    public string? ProductLine { get; set; }
    public List<string>? Categories { get; set; }
    public string? Name { get; set; }
    public bool? IsActive { get; set; }
}

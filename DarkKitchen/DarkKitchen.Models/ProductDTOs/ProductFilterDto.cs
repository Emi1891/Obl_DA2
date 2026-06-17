namespace DarkKitchen.Models.ProductDTOs;

public sealed record ProductFilterDto
{
    public string? ProductLine { get; set; }
    public List<string>? Categories { get; set; }
    public string? Name { get; set; }
}

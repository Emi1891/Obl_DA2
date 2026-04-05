namespace DarkKitchen.Models.ProductDTOs;

public class CreateProductDto
{
    public string? Code { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? ProductLine { get; set; }
    public string? Category { get; set; }
    public decimal? Price { get; set; }
    public string[]? ImageUrl { get; set; }
}

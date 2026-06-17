namespace DarkKitchen.Models.PromotionDTOs;

public sealed record PromotionFiltersDto
{
    public DateTime? Date { get; init; }
    public string? ProductLine { get; init; }
    public string? ProductName { get; init; }
}

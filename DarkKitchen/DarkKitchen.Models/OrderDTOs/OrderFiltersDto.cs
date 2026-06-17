namespace DarkKitchen.Models.OrderDTOs;

public sealed record OrderFiltersDto
{
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
    public string? Status { get; set; }
}

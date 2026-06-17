namespace DarkKitchen.Models.OrderDTOs;

public sealed record OrderFilterByStatusDto
{
    public DateTime DateFrom { get; set; }
    public DateTime DateTo { get; set; }
    public string? Address { get; set; }
    public string? Status { get; set; }
}

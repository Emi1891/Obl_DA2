namespace DarkKitchen.Models.DateDTOs;

public sealed record DateRangeDto
{
    public DateTime DateFrom { get; set; }
    public DateTime DateTo { get; set; }
}

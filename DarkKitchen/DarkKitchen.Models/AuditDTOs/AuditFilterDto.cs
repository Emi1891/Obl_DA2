namespace DarkKitchen.Models.AuditDTOs;

public class AuditFilterDto
{
    public required string EntityName { get; set; }
    public int? EntityId { get; set; }
    public required DateTime DateFrom { get; set; }
    public required DateTime DateTo { get; set; }
}

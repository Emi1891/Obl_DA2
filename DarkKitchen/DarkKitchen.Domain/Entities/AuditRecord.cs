namespace DarkKitchen.Domain.Entities;

public class AuditRecord
{
    public int Id { get; set; }
    public DateTime DateTime { get; set; }
    public required string EntityName { get; set; }
    public int EntityId { get; set; }
    public required string ChangeDescription { get; set; }
    public required string ResponsibleUser { get; set; }
}

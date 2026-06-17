namespace DarkKitchen.Models.AuditDTOs;

public record AuditRecordDto(
    int Id,
    DateTime DateTime,
    string EntityName,
    int EntityId,
    string ChangeDescription,
    string ResponsibleUser
);

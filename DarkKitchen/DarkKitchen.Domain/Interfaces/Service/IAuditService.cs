using DarkKitchen.Domain.Entities;

namespace DarkKitchen.Domain.Interfaces.Service;

public interface IAuditService
{
    void LogChange(string entityName, int entityId, string changeDescription, string responsibleUser);
    IEnumerable<AuditRecord> GetByFilter(string entityName, int? entityId, DateTime dateFrom, DateTime dateTo);
}

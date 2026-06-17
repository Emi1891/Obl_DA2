using DarkKitchen.Domain.Entities;

namespace DarkKitchen.Domain.Interfaces.Repository;

public interface IAuditRepository
{
    void Add(AuditRecord auditRecord);
    IEnumerable<AuditRecord> GetByFilter(string entityName, int? entityId, DateTime dateFrom, DateTime dateTo);
}

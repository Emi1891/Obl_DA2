using DarkKitchen.Domain.Entities;
using DarkKitchen.Domain.Interfaces.Repository;

namespace DarkKitchen.DataAccess.Repositories;

public class AuditRepository(AppDbContext context) : IAuditRepository
{
    private readonly AppDbContext _context = context;

    public void Add(AuditRecord auditRecord)
    {
        auditRecord.DateTime = DateTime.UtcNow;
        _context.AuditRecords.Add(auditRecord);
        _context.SaveChanges();
    }

    public IEnumerable<AuditRecord> GetByFilter(string entityName, int? entityId, DateTime dateFrom, DateTime dateTo)
    {
        return _context.AuditRecords
            .Where(a => a.EntityName == entityName
                && (entityId == null || a.EntityId == entityId)
                && a.DateTime >= dateFrom
                && a.DateTime <= dateTo)
            .ToList();
    }
}

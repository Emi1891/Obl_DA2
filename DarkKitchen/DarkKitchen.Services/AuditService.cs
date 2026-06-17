using DarkKitchen.Domain.Entities;
using DarkKitchen.Domain.Interfaces.Repository;
using DarkKitchen.Domain.Interfaces.Service;

namespace DarkKitchen.Services;

public class AuditService(IAuditRepository auditRepository) : IAuditService
{
    private readonly IAuditRepository _auditRepository = auditRepository;

    public void LogChange(string entityName, int entityId, string changeDescription, string responsibleUser)
    {
        var auditRecord = new AuditRecord
        {
            EntityName = entityName,
            EntityId = entityId,
            ChangeDescription = changeDescription,
            ResponsibleUser = responsibleUser
        };

        _auditRepository.Add(auditRecord);
    }

    public IEnumerable<AuditRecord> GetByFilter(string entityName, int? entityId, DateTime dateFrom, DateTime dateTo)
    {
        return _auditRepository.GetByFilter(entityName, entityId, dateFrom, dateTo);
    }
}

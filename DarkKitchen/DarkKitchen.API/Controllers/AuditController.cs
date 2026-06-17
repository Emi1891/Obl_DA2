using DarkKitchen.Domain.Enums;
using DarkKitchen.Domain.Interfaces.Service;
using DarkKitchen.Models.AuditDTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DarkKitchen.API.Controllers;

[ApiController]
[Route("api/audit")]
public class AuditController(IAuditService auditService) : ControllerBase
{
    private readonly IAuditService _auditService = auditService;

    [Authorize(Policy = nameof(Permission.GetAuditRecords))]
    [HttpGet]
    public IActionResult GetAuditRecords([FromQuery] AuditFilterDto filter)
    {
        if(filter.DateFrom >= filter.DateTo)
        {
            return BadRequest("DateFrom must be before DateTo.");
        }

        var records = _auditService.GetByFilter(filter.EntityName, filter.EntityId, filter.DateFrom, filter.DateTo);

        var result = records.Select(r => new AuditRecordDto(r.Id, r.DateTime, r.EntityName, r.EntityId, r.ChangeDescription, r.ResponsibleUser));

        return Ok(result);
    }
}

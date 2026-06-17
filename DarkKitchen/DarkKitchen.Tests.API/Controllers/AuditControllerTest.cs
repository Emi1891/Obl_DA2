using DarkKitchen.API.Controllers;
using DarkKitchen.Domain.Entities;
using DarkKitchen.Domain.Interfaces.Service;
using DarkKitchen.Models.AuditDTOs;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace DarkKitchen.Tests.API.Controllers;

[TestClass]
public class AuditControllerTest
{
    private Mock<IAuditService>? auditServiceMock;
    private AuditController? auditController;

    [TestInitialize]
    public void Setup()
    {
        auditServiceMock = new Mock<IAuditService>(MockBehavior.Strict);
        auditController = new AuditController(auditServiceMock.Object);
    }

    [TestMethod]
    public void GetAuditRecords_ValidFilter_ReturnsOk()
    {
        var records = new List<AuditRecord>
        {
            new AuditRecord
            {
                Id = 1,
                EntityName = "Product",
                EntityId = 1,
                ChangeDescription = "Product created",
                ResponsibleUser = "admin@gmail.com",
                DateTime = DateTime.UtcNow
            }
        };

        auditServiceMock!.Setup(s => s.GetByFilter("Product", null, It.IsAny<DateTime>(), It.IsAny<DateTime>()))
            .Returns(records);

        var filter = new AuditFilterDto
        {
            EntityName = "Product",
            DateFrom = DateTime.UtcNow.AddDays(-1),
            DateTo = DateTime.UtcNow
        };

        var result = auditController!.GetAuditRecords(filter);
        var resultObj = result as ObjectResult;

        Assert.IsNotNull(resultObj);
        Assert.AreEqual(200, resultObj!.StatusCode);
    }

    [TestMethod]
    public void GetAuditRecords_DateFromGreaterThanDateTo_ReturnsBadRequest()
    {
        var filter = new AuditFilterDto
        {
            EntityName = "Product",
            DateFrom = DateTime.UtcNow,
            DateTo = DateTime.UtcNow.AddDays(-1)
        };

        var result = auditController!.GetAuditRecords(filter);
        var resultObj = result as ObjectResult;

        Assert.IsNotNull(resultObj);
        Assert.AreEqual(400, resultObj!.StatusCode);
    }
}

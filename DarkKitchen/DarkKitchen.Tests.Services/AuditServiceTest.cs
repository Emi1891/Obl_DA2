using DarkKitchen.Domain.Entities;
using DarkKitchen.Domain.Interfaces.Repository;
using DarkKitchen.Services;
using Moq;

namespace DarkKitchen.Tests.Services;

[TestClass]
public class AuditServiceTest
{
    private Mock<IAuditRepository>? auditRepositoryMock;
    private AuditService? auditService;

    [TestInitialize]
    public void Setup()
    {
        auditRepositoryMock = new Mock<IAuditRepository>(MockBehavior.Strict);
        auditService = new AuditService(auditRepositoryMock.Object);
    }

    [TestMethod]
    public void LogChange_ValidData_CallsRepository()
    {
        auditRepositoryMock!.Setup(r => r.Add(It.IsAny<AuditRecord>()));

        auditService!.LogChange("Product", 1, "Product created", "admin@gmail.com");

        auditRepositoryMock!.Verify(r => r.Add(It.Is<AuditRecord>(a =>
            a.EntityName == "Product" &&
            a.EntityId == 1 &&
            a.ChangeDescription == "Product created" &&
            a.ResponsibleUser == "admin@gmail.com")), Times.Once);
    }

    [TestMethod]
    public void GetByFilter_ValidData_ReturnsRecords()
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

        auditRepositoryMock!.Setup(r => r.GetByFilter("Product", null, It.IsAny<DateTime>(), It.IsAny<DateTime>()))
            .Returns(records);

        var result = auditService!.GetByFilter("Product", null, DateTime.UtcNow.AddDays(-1), DateTime.UtcNow);

        Assert.AreEqual(1, result.Count());
    }
}

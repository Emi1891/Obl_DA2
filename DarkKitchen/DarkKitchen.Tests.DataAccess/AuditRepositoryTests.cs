using DarkKitchen.DataAccess;
using DarkKitchen.DataAccess.Repositories;
using DarkKitchen.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DarkKitchen.Tests.DataAccess;

[TestClass]
public class AuditRepositoryTests
{
    private AppDbContext? context;
    private AuditRepository? auditRepository;

    [TestInitialize]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        context = new AppDbContext(options);
        auditRepository = new AuditRepository(context);
    }

    [TestCleanup]
    public void Cleanup()
    {
        context!.Dispose();
    }

    [TestMethod]
    public void Add_ValidAuditRecord_SavesRecord()
    {
        var record = new AuditRecord
        {
            EntityName = "Product",
            EntityId = 1,
            ChangeDescription = "Product created",
            ResponsibleUser = "admin@gmail.com"
        };

        auditRepository!.Add(record);

        Assert.AreEqual(1, context!.AuditRecords.Count());
    }

    [TestMethod]
    public void GetByFilter_ValidFilter_ReturnsRecords()
    {
        var record = new AuditRecord
        {
            EntityName = "Product",
            EntityId = 1,
            ChangeDescription = "Product created",
            ResponsibleUser = "admin@gmail.com",
            DateTime = DateTime.UtcNow
        };
        context!.AuditRecords.Add(record);
        context.SaveChanges();

        var result = auditRepository!.GetByFilter("Product", null, DateTime.UtcNow.AddDays(-1), DateTime.UtcNow.AddDays(1));

        Assert.AreEqual(1, result.Count());
    }

    [TestMethod]
    public void GetByFilter_WithEntityId_ReturnsMatchingRecords()
    {
        context!.AuditRecords.AddRange(
            new AuditRecord { EntityName = "Product", EntityId = 1, ChangeDescription = "Created", ResponsibleUser = "admin@gmail.com", DateTime = DateTime.UtcNow },
            new AuditRecord { EntityName = "Product", EntityId = 2, ChangeDescription = "Created", ResponsibleUser = "admin@gmail.com", DateTime = DateTime.UtcNow });
        context.SaveChanges();

        var result = auditRepository!.GetByFilter("Product", 1, DateTime.UtcNow.AddDays(-1), DateTime.UtcNow.AddDays(1));

        Assert.AreEqual(1, result.Count());
        Assert.AreEqual(1, result.First().EntityId);
    }
}

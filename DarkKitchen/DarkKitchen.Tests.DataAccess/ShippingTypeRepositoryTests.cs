using DarkKitchen.DataAccess;
using DarkKitchen.DataAccess.Repositories;
using DarkKitchen.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DarkKitchen.Tests.DataAccess;

[TestClass]
public class ShippingTypeRepositoryTests
{
    private ShippingType? shippingType;
    private AppDbContext? context;
    private ShippingTypeRepository? shippingTypeRepository;

    [TestInitialize]
    public void TestInitialize()
    {
        shippingType = new ShippingType { Name = "Express", Price = 250 };

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        context = new AppDbContext(options);
        shippingTypeRepository = new ShippingTypeRepository(context);
    }

    [TestCleanup]
    public void TestCleanup()
    {
        context!.Database.EnsureDeleted();
        context.Dispose();
    }

    [TestMethod]
    public void Add_WhenShippingTypeIsValid_AddsToDatabase()
    {
        shippingTypeRepository!.Add(shippingType!);

        var result = context!.ShippingTypes.FirstOrDefault(s => s.Name == "Express");

        Assert.IsNotNull(result);
        Assert.AreEqual("Express", result.Name);
        Assert.AreEqual(250, result.Price);
    }

    [TestMethod]
    public void GetById_WhenExists_ReturnsShippingType()
    {
        context!.ShippingTypes.Add(shippingType!);
        context.SaveChanges();

        var result = shippingTypeRepository!.GetById(shippingType!.Id);

        Assert.IsNotNull(result);
        Assert.AreEqual("Express", result.Name);
    }

    [TestMethod]
    public void GetById_WhenNotExists_ReturnsNull()
    {
        var result = shippingTypeRepository!.GetById(999);

        Assert.IsNull(result);
    }

    [TestMethod]
    public void GetAll_WhenShippingTypesExist_ReturnsAll()
    {
        context!.ShippingTypes.Add(shippingType!);
        context.SaveChanges();

        var result = shippingTypeRepository!.GetAll().ToList();

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("Express", result[0].Name);
    }

    [TestMethod]
    public void Update_WhenShippingTypeExists_UpdatesInDatabase()
    {
        context!.ShippingTypes.Add(shippingType!);
        context.SaveChanges();
        shippingType!.Name = "En el día";
        shippingType.Price = 200;

        shippingTypeRepository!.Update(shippingType!);
        var result = context.ShippingTypes.FirstOrDefault(s => s.Id == shippingType.Id);

        Assert.AreEqual("En el día", result!.Name);
        Assert.AreEqual(200, result.Price);
    }

    [TestMethod]
    public void Update_WhenNotExists_DoesNothing()
    {
        shippingTypeRepository!.Update(shippingType!);

        Assert.AreEqual(0, context!.ShippingTypes.Count());
    }
}

using DarkKitchen.DataAccess;
using DarkKitchen.DataAccess.Repositories;
using DarkKitchen.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DarkKitchen.Tests.DataAccess;

[TestClass]
public class OrderRepositoryTests
{
    private Order? order;
    private AppDbContext? context;
    private OrderRepository? orderRepository;

    [TestInitialize]
    public void TestInitialize()
    {
        order = new Order
        {
            ClientId = 1,
            ShippingTypeId = 1,
            Address = new Address { Street = "18 de Julio", DoorNumber = "1234", Apartment = "101" },
            Status = "Pending",
            CreatedAt = DateTime.Now
        };

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        context = new AppDbContext(options);
        orderRepository = new OrderRepository(context);
    }

    [TestCleanup]
    public void TestCleanup()
    {
        context!.Database.EnsureDeleted();
        context.Dispose();
    }

    [TestMethod]
    public void AddOrder_WhenOrderIsValid_AddsOrderToDatabase()
    {
        orderRepository!.Add(order!);

        var result = context!.Orders.FirstOrDefault(o => o.ClientId == 1);

        Assert.IsNotNull(result);
        Assert.AreEqual("Pending", result.Status);
    }

    [TestMethod]
    public void GetById_WhenOrderExists_ReturnsOrder()
    {
        context!.Orders.Add(order!);
        context.SaveChanges();

        var result = orderRepository!.GetById(order!.Id);

        Assert.IsNotNull(result);
        Assert.AreEqual("Pending", result.Status);
    }

    [TestMethod]
    public void GetById_WhenOrderDoesNotExist_ReturnsNull()
    {
        var result = orderRepository!.GetById(999);

        Assert.IsNull(result);
    }

    [TestMethod]
    public void UpdateOrder_WhenOrderExists_UpdatesOrderInDatabase()
    {
        context!.Orders.Add(order!);
        context.SaveChanges();
        order!.Status = "Prepared";

        orderRepository!.Update(order!);
        var result = context.Orders.FirstOrDefault(o => o.Id == order.Id);

        Assert.AreEqual("Prepared", result!.Status);
    }

    [TestMethod]
    public void UpdateOrder_WhenOrderDoesNotExist_DoesNothing()
    {
        orderRepository!.Update(order!);

        Assert.AreEqual(0, context!.Orders.Count());
    }

    [TestMethod]
    public void GetClientOrders_WhenOrdersExist_ReturnsOrders()
    {
        context!.Orders.Add(order!);
        context.SaveChanges();

        var result = orderRepository!.GetClientOrders(1, null, null, null);

        Assert.AreEqual(1, result.Count());
    }

    [TestMethod]
    public void GetClientOrders_WhenNoOrdersExist_ReturnsEmptyList()
    {
        var result = orderRepository!.GetClientOrders(1, null, null, null);

        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.Count());
    }

    [TestMethod]
    public void GetOrdersByStatus_WhenOrdersExist_ReturnsOrders()
    {
        context!.Orders.Add(order!);
        context.SaveChanges();

        var result = orderRepository!.GetOrdersByStatus(DateTime.Now.AddDays(-1), DateTime.Now.AddDays(1), null, null);

        Assert.AreEqual(1, result.Count());
    }

    [TestMethod]
    public void GetOrdersByStatus_WhenNoOrdersExist_ReturnsEmptyList()
    {
        var result = orderRepository!.GetOrdersByStatus(DateTime.Now.AddDays(-1), DateTime.Now.AddDays(1), null, null);

        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.Count());
    }

    [TestMethod]
    public void Save_WhenCalled_SavesChangesToDatabase()
    {
        context!.Orders.Add(order!);
        orderRepository!.Save();

        var result = context.Orders.FirstOrDefault(o => o.ClientId == 1);

        Assert.IsNotNull(result);
    }

    [TestMethod]
    public void GetClientOrders_WithDateFilter_ReturnsFilteredOrders()
    {
        context!.Orders.Add(order!);
        context.SaveChanges();

        var result = orderRepository!.GetClientOrders(1, DateTime.Now.AddDays(-1), DateTime.Now.AddDays(1), null);

        Assert.AreEqual(1, result.Count());
    }

    [TestMethod]
    public void GetClientOrders_WithStatusFilter_ReturnsFilteredOrders()
    {
        context!.Orders.Add(order!);
        context.SaveChanges();

        var result = orderRepository!.GetClientOrders(1, null, null, "Pending");

        Assert.AreEqual(1, result.Count());
    }

    [TestMethod]
    public void GetOrdersByStatus_WithAddressFilter_ReturnsFilteredOrders()
    {
        context!.Orders.Add(order!);
        context.SaveChanges();

        var result = orderRepository!.GetOrdersByStatus(DateTime.Now.AddDays(-1), DateTime.Now.AddDays(1), "18 de Julio", null);

        Assert.AreEqual(1, result.Count());
    }

    [TestMethod]
    public void GetOrdersByStatus_WithStatusFilter_ReturnsFilteredOrders()
    {
        context!.Orders.Add(order!);
        context.SaveChanges();

        var result = orderRepository!.GetOrdersByStatus(DateTime.Now.AddDays(-1), DateTime.Now.AddDays(1), null, "Pending");

        Assert.AreEqual(1, result.Count());
    }

    [TestMethod]
    public void GetAll_WhenOrdersExist_ReturnsAllOrders()
    {
        context!.Orders.Add(order!);
        context.SaveChanges();

        var result = orderRepository!.GetAll().ToList();

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual(1, result[0].ClientId);
    }
}

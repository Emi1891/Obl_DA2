using DarkKitchen.Domain.Entities;

namespace DarkKitchen.Tests.Domain.Entities;

[TestClass]
public class OrderTest
{
    private readonly int id = 1;
    private readonly int clientId = 1;
    private readonly int shippingTypeId = 1;
    private readonly string street = "18 de Julio";
    private readonly string doorNumber = "1234";
    private readonly string apartment = "101";
    private readonly string status = "Pending";
    private readonly DateTime createdAt = new DateTime(2026, 1, 25);
    private Order? order;

    [TestInitialize]
    public void Setup()
    {
        order = new Order
        {
            Id = id,
            ClientId = clientId,
            ShippingTypeId = shippingTypeId,
            Address = new Address { Street = street, DoorNumber = doorNumber, Apartment = apartment },
            Status = status,
            CreatedAt = createdAt
        };
    }

    [TestMethod]
    public void Order_WithValidData_HasCorrectClientId()
    {
        Assert.AreEqual(clientId, order!.ClientId);
    }

    [TestMethod]
    public void Order_WithValidData_HasCorrectShippingTypeId()
    {
        Assert.AreEqual(shippingTypeId, order!.ShippingTypeId);
    }

    [TestMethod]
    public void Order_WithValidData_HasCorrectStreet()
    {
        Assert.AreEqual(street, order!.Address.Street);
    }

    [TestMethod]
    public void Order_WithValidData_HasCorrectDoorNumber()
    {
        Assert.AreEqual(doorNumber, order!.Address.DoorNumber);
    }

    [TestMethod]
    public void Order_WithValidData_HasCorrectApartment()
    {
        Assert.AreEqual(apartment, order!.Address.Apartment);
    }

    [TestMethod]
    public void Order_WithNullApartment_ApartmentIsNull()
    {
        var orderWithoutApartment = new Order
        {
            ClientId = clientId,
            ShippingTypeId = shippingTypeId,
            Address = new Address { Street = street, DoorNumber = doorNumber },
            Status = status,
            CreatedAt = createdAt
        };

        Assert.IsNull(orderWithoutApartment.Address.Apartment);
    }

    [TestMethod]
    public void Order_WithValidData_HasCorrectStatus()
    {
        Assert.AreEqual(status, order!.Status);
    }

    [TestMethod]
    public void Order_WithValidData_HasCorrectCreatedAt()
    {
        Assert.AreEqual(createdAt, order!.CreatedAt);
    }

    [TestMethod]
    public void Order_UpdatedAtIsNullByDefault()
    {
        Assert.IsNull(order!.UpdatedAt);
    }
}

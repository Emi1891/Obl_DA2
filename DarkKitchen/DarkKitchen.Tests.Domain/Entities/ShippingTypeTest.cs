using DarkKitchen.Domain.Entities;

namespace DarkKitchen.Tests.Domain.Entities;

[TestClass]
public class ShippingTypeTest
{
    [TestMethod]
    public void ShippingType_WithValidData_HasCorrectName()
    {
        var shippingType = new ShippingType { Name = "Express", Price = 250 };

        Assert.AreEqual("Express", shippingType.Name);
    }

    [TestMethod]
    public void ShippingType_WithValidData_HasCorrectPrice()
    {
        var shippingType = new ShippingType { Name = "Express", Price = 250 };

        Assert.AreEqual(250, shippingType.Price);
    }
}

using DarkKitchen.Domain.Entities;
using DarkKitchen.Domain.Exceptions;
using DarkKitchen.Domain.Interfaces.Repository;
using DarkKitchen.Models.ShippingTypeDTOs;
using DarkKitchen.Services;
using Moq;

namespace DarkKitchen.Tests.Services;

[TestClass]
public class ShippingTypeServiceTest
{
    private Mock<IShippingTypeRepository>? shippingTypeRepositoryMock;
    private ShippingTypeService? shippingTypeService;
    private ShippingTypeDto validDto;
    private ShippingType? shippingTypeEntity;

    [TestInitialize]
    public void Setup()
    {
        shippingTypeRepositoryMock = new Mock<IShippingTypeRepository>(MockBehavior.Strict);
        shippingTypeService = new ShippingTypeService(shippingTypeRepositoryMock.Object);

        validDto = new ShippingTypeDto("Express", 250);
        shippingTypeEntity = new ShippingType { Id = 1, Name = "Express", Price = 250 };
    }

    [TestMethod]
    public void Create_WithValidData_CallsRepositoryAndReturnsDto()
    {
        shippingTypeRepositoryMock!.Setup(r => r.Add(It.IsAny<ShippingType>()));

        var result = shippingTypeService!.Create(validDto);

        shippingTypeRepositoryMock!.Verify(r => r.Add(It.IsAny<ShippingType>()), Times.Once);
        Assert.AreEqual(validDto.name, result.name);
        Assert.AreEqual(validDto.price, result.price);
    }

    [TestMethod]
    public void Create_WithEmptyName_ThrowsBadRequestException()
    {
        var dto = validDto with { name = string.Empty };

        Assert.ThrowsException<BadRequestException>(() => shippingTypeService!.Create(dto));
    }

    [TestMethod]
    public void Create_WithNegativePrice_ThrowsBadRequestException()
    {
        var dto = validDto with { price = -1 };

        Assert.ThrowsException<BadRequestException>(() => shippingTypeService!.Create(dto));
    }

    [TestMethod]
    public void GetAll_ReturnsAllShippingTypes()
    {
        shippingTypeRepositoryMock!.Setup(r => r.GetAll()).Returns([shippingTypeEntity!]);

        var result = shippingTypeService!.GetAll();

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual(shippingTypeEntity!.Name, result[0].name);
        Assert.AreEqual(shippingTypeEntity!.Price, result[0].price);
    }

    [TestMethod]
    public void GetById_ExistingId_ReturnsDto()
    {
        shippingTypeRepositoryMock!.Setup(r => r.GetById(1)).Returns(shippingTypeEntity!);

        var result = shippingTypeService!.GetById(1);

        Assert.AreEqual(shippingTypeEntity!.Id, result.id);
        Assert.AreEqual(shippingTypeEntity!.Name, result.name);
        Assert.AreEqual(shippingTypeEntity!.Price, result.price);
    }

    [TestMethod]
    public void GetById_NotFound_ThrowsNotFoundException()
    {
        shippingTypeRepositoryMock!.Setup(r => r.GetById(99)).Returns((ShippingType?)null);

        Assert.ThrowsException<NotFoundException>(() => shippingTypeService!.GetById(99));
    }

    [TestMethod]
    public void Update_WithValidData_CallsRepositoryAndReturnsDto()
    {
        shippingTypeRepositoryMock!.Setup(r => r.GetById(1)).Returns(shippingTypeEntity!);
        shippingTypeRepositoryMock!.Setup(r => r.Update(It.IsAny<ShippingType>()));

        var result = shippingTypeService!.Update(1, validDto);

        shippingTypeRepositoryMock!.Verify(r => r.Update(It.IsAny<ShippingType>()), Times.Once);
        Assert.AreEqual(validDto.name, result.name);
        Assert.AreEqual(validDto.price, result.price);
    }

    [TestMethod]
    public void Update_WithEmptyName_ThrowsBadRequestException()
    {
        shippingTypeRepositoryMock!.Setup(r => r.GetById(1)).Returns(shippingTypeEntity!);

        Assert.ThrowsException<BadRequestException>(() =>
            shippingTypeService!.Update(1, validDto with { name = string.Empty }));
    }

    [TestMethod]
    public void Update_WithNegativePrice_ThrowsBadRequestException()
    {
        shippingTypeRepositoryMock!.Setup(r => r.GetById(1)).Returns(shippingTypeEntity!);

        Assert.ThrowsException<BadRequestException>(() =>
            shippingTypeService!.Update(1, validDto with { price = -1 }));
    }

    [TestMethod]
    public void Update_NotFound_ThrowsNotFoundException()
    {
        shippingTypeRepositoryMock!.Setup(r => r.GetById(99)).Returns((ShippingType?)null);

        Assert.ThrowsException<NotFoundException>(() => shippingTypeService!.Update(99, validDto));
    }
}

using DarkKitchen.API.Controllers;
using DarkKitchen.Domain.Exceptions;
using DarkKitchen.Domain.Interfaces.Service;
using DarkKitchen.Models.ShippingTypeDTOs;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace DarkKitchen.Tests.API.Controllers;

[TestClass]
public class ShippingTypesControllerTest
{
    private Mock<IShippingTypeService>? shippingTypeServiceMock;
    private ShippingTypesController? shippingTypesController;
    private ShippingTypeDto validDto;
    private ShippingTypeResponseDto shippingTypeResponse;
    private List<ShippingTypeResponseDto>? shippingTypes;

    [TestInitialize]
    public void Setup()
    {
        shippingTypeServiceMock = new Mock<IShippingTypeService>(MockBehavior.Strict);
        shippingTypesController = new ShippingTypesController(shippingTypeServiceMock.Object);

        validDto = new ShippingTypeDto("Express", 250);
        shippingTypeResponse = new ShippingTypeResponseDto(1, "Express", 250);
        shippingTypes = [shippingTypeResponse];
    }

    [TestMethod]
    public void GetAll_ReturnsOk()
    {
        shippingTypeServiceMock!.Setup(s => s.GetAll()).Returns(shippingTypes!);

        var result = shippingTypesController!.GetAll();
        var resultObj = result as ObjectResult;

        Assert.IsNotNull(resultObj);
        Assert.AreEqual(200, resultObj.StatusCode);
    }

    [TestMethod]
    public void GetById_WhenExistingId_ReturnsOk()
    {
        shippingTypeServiceMock!.Setup(s => s.GetById(1)).Returns(shippingTypeResponse);

        var result = shippingTypesController!.GetById(1);
        var resultObj = result as ObjectResult;

        Assert.IsNotNull(resultObj);
        Assert.AreEqual(200, resultObj.StatusCode);
    }

    [TestMethod]
    public void GetById_NotFound_ThrowsNotFoundException()
    {
        shippingTypeServiceMock!.Setup(s => s.GetById(99)).Throws(new NotFoundException("Shipping type not found."));

        Assert.ThrowsException<NotFoundException>(() => shippingTypesController!.GetById(99));
    }

    [TestMethod]
    public void Create_WithValidData_ReturnsCreated()
    {
        shippingTypeServiceMock!.Setup(s => s.Create(validDto)).Returns(shippingTypeResponse);

        var result = shippingTypesController!.Create(validDto);
        var resultObj = result as ObjectResult;

        Assert.IsNotNull(resultObj);
        Assert.AreEqual(201, resultObj.StatusCode);
    }

    [TestMethod]
    public void Update_WithValidData_ReturnsOk()
    {
        shippingTypeServiceMock!.Setup(s => s.Update(1, validDto)).Returns(shippingTypeResponse);

        var result = shippingTypesController!.Update(1, validDto);
        var resultObj = result as ObjectResult;

        Assert.IsNotNull(resultObj);
        Assert.AreEqual(200, resultObj.StatusCode);
    }

    [TestMethod]
    public void Update_WhenNotFound_ThrowsNotFoundException()
    {
        shippingTypeServiceMock!.Setup(s => s.Update(99, validDto)).Throws(new NotFoundException("Shipping type not found."));

        Assert.ThrowsException<NotFoundException>(() => shippingTypesController!.Update(99, validDto));
    }
}

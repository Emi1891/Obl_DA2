using DarkKitchen.API.Controllers;
using DarkKitchen.Domain.Exceptions;
using DarkKitchen.Domain.Interfaces;
using DarkKitchen.Models.DateDTOs;
using DarkKitchen.Models.ProductDTOs;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace DarkKitchen.Tests.Controllers;

[TestClass]
public class ProductControllerTest
{
    private readonly int validProductId = 1;
    private readonly string validProductName = "Valid Product Name";
    private readonly string validProductDescription = "Valid Product Description";
    private readonly string validProductLine = "Valid Product Line";
    private readonly string validCategory = "Valid Category";
    private readonly string[] validImageUrl = ["http://example.com/image.jpg"];
    private readonly bool isActive = true;
    private UpdateProductDto? validProduct;
    private CreateProductDto? validCreateProduct;
    private ProductsController? productController;
    private Mock<IProductService>? productServiceMock;

    [TestInitialize]
    public void Setup()
    {
        productServiceMock = new Mock<IProductService>();
        productController = new ProductsController(productServiceMock.Object);

        validProduct = new UpdateProductDto()
        {
            Id = validProductId,
            Name = validProductName,
            Description = validProductDescription,
            Category = validCategory,
            ImageUrl = validImageUrl,
            IsActive = isActive
        };

        validCreateProduct = new CreateProductDto()
        {
            Name = validProductName,
            Description = validProductDescription,
            Category = validCategory,
            ImageUrl = validImageUrl,
        };
    }

    [TestMethod]
    public void CreateProduct_WhenValidParams_ReturnsCreated()
    {
        var result = productController.CreateProduct(validCreateProduct!);
        var resultObj = result as ObjectResult;

        Assert.IsNotNull(resultObj);
        Assert.AreEqual(201, resultObj!.StatusCode);
    }

    [TestMethod]
    public void CreateProduct_WhenNameIsNull_ThrowsBadRequestException()
    {
        var nullProductName = new CreateProductDto()
        {
            Name = null,
            Description = validProductDescription,
            Category = validCategory,
            ImageUrl = validImageUrl
        };

        productServiceMock.Setup(s => s.CreateProduct(nullProductName)).Throws(new BadRequestException("Invalid name."));

        Assert.ThrowsException<BadRequestException>(() => productController.CreateProduct(nullProductName));
    }

    [TestMethod]
    public void GetProducts_WhenValidProducts_ReturnsOk()
    {
        List<string> categories = ["Fútbol", "Baloncesto", "Tenis"];
        var products = new List<UpdateProductDto>();
        productServiceMock.Setup(s => s.GetProducts(validProductLine, categories, validProductName)).Returns(products!);
        var result = productController.GetProducts(validProductLine, categories, validProductName);
        var resultObj = result as ObjectResult;

        Assert.IsNotNull(resultObj);
        Assert.AreEqual(200, resultObj!.StatusCode);
        Assert.AreEqual(products, resultObj.Value);
    }

    [TestMethod]
    public void GetProducts_WhenNoProducts_ThrowsNotFoundException()
    {
        List<string> categories = [];
        productServiceMock.Setup(s => s.GetProducts(validProductLine, categories, validProductName)).Throws(new NotFoundException("No products found."));

        Assert.ThrowsException<NotFoundException>(() => productController.GetProducts(validProductLine, categories, validProductName));
    }

    [TestMethod]
    public void GetMostRequestedProducts_WhenValidProducts_ReturnsOkWithProdcuts()
    {
        var products = new List<UpdateProductDto> { validProduct! };
        var dateFrom = DateTime.Now.AddDays(-7);
        var dateTo = DateTime.Now;
        productServiceMock.Setup(s => s.GetMostRequestedProducts(It.IsAny<DateRangeDto>())).Returns(products);
        var result = productController.GetMostRequestedProducts(dateFrom, dateTo);
        var resultObj = result as ObjectResult;

        Assert.IsNotNull(resultObj);
        Assert.AreEqual(200, resultObj!.StatusCode);
        Assert.AreEqual(products, resultObj.Value);
    }

    [TestMethod]
    public void GetMostRequestedProducts_WhenNoProducts_ThrowsNotFoundException()
    {
        var dateFrom = DateTime.Now.AddDays(-7);
        var dateTo = DateTime.Now;
        productServiceMock.Setup(s => s.GetMostRequestedProducts(It.IsAny<DateRangeDto>())).Throws(new NotFoundException("No products found."));

        Assert.ThrowsException<NotFoundException>(() => productController.GetMostRequestedProducts(dateFrom, dateTo));
    }

    [TestMethod]
    public void UpdateProduct_WhenValidParams_ReturnsOk()
    {
        var result = productController.UpdateProduct(validProduct!);
        var resultObj = result as ObjectResult;

        Assert.IsNotNull(resultObj);
        Assert.AreEqual(200, resultObj!.StatusCode);
    }

    [TestMethod]
    public void UpdateProduct_WhenProductNotFound_ThrowsNotFoundException()
    {
        productServiceMock.Setup(s => s.UpdateProduct(validProduct!)).Throws(new NotFoundException("Product not found."));

        Assert.ThrowsException<NotFoundException>(() => productController.UpdateProduct(validProduct!));
    }

    [TestMethod]
    public void UpdateProductState_WhenProductFound_ReturnsOk()
    {
        var id = 1;
        var status = new ProductStatusDto
        {
            IsActive = true
        };
        var result = productController.UpdateStatus(id, status);
        var resultObj = result as ObjectResult;

        Assert.IsNotNull(resultObj);
        Assert.AreEqual(200, resultObj!.StatusCode);
    }

    [TestMethod]
    public void UpdateProductState_WhenProductNotFound_ThrowsNotFoundException()
    {
        var id = 1;
        var status = new ProductStatusDto
        {
            IsActive = true
        };
        productServiceMock.Setup(s => s.UpdateStatus(id, status)).Throws(new NotFoundException("Product not found."));

        Assert.ThrowsException<NotFoundException>(() => productController.UpdateStatus(id, status));
    }
}

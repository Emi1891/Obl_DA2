using System.Security.Claims;
using DarkKitchen.API.Controllers;
using DarkKitchen.Domain.Exceptions;
using DarkKitchen.Domain.Interfaces.Service;
using DarkKitchen.Models.DateDTOs;
using DarkKitchen.Models.ProductDTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace DarkKitchen.Tests.API.Controllers;

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
    private ProductDto validProduct;
    private CreateProductDto validCreateProduct;
    private ProductsController? productController;
    private Mock<IProductService>? productServiceMock;

    [TestInitialize]
    public void Setup()
    {
        productServiceMock = new Mock<IProductService>(MockBehavior.Strict);
        productController = new ProductsController(productServiceMock.Object);

        var user = new ClaimsPrincipal(new ClaimsIdentity(
        [
            new Claim(ClaimTypes.Name, "admin@gmail.com")
        ], "mock"));

        productController.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };

        validProduct = new ProductDto(validProductId, null, validProductName, validProductDescription, null, validCategory, null, validImageUrl, isActive, 0);
        validCreateProduct = new CreateProductDto(null, validProductName, validProductDescription, null, validCategory, null, validImageUrl);
    }

    [TestMethod]
    public void CreateProduct_WhenValidParams_ReturnsCreated()
    {
        productServiceMock!.Setup(s => s.CreateProduct(validCreateProduct, "admin@gmail.com"));

        var result = productController!.CreateProduct(validCreateProduct);
        var resultObj = result as ObjectResult;

        Assert.IsNotNull(resultObj);
        Assert.AreEqual(201, resultObj!.StatusCode);
    }

    [TestMethod]
    public void CreateProduct_WhenNameIsNull_ThrowsBadRequestException()
    {
        var nullProductName = new CreateProductDto(null, null, validProductDescription, null, validCategory, null, validImageUrl);
        productServiceMock!.Setup(s => s.CreateProduct(nullProductName, "admin@gmail.com")).Throws(new BadRequestException("Invalid name."));

        Assert.ThrowsException<BadRequestException>(() => productController!.CreateProduct(nullProductName));
    }

    [TestMethod]
    public void GetProducts_WhenValidProducts_ReturnsOk()
    {
        var filters = new ProductFilterDto
        {
            ProductLine = validProductLine,
            Categories = ["Fútbol", "Baloncesto", "Tenis"],
            Name = validProductName
        };
        var products = new List<ProductDto>();
        productServiceMock!.Setup(s => s.GetProducts(It.IsAny<ProductFilterDto>())).Returns(products!);

        var result = productController!.GetProducts(filters);
        var resultObj = result as ObjectResult;

        Assert.IsNotNull(resultObj);
        Assert.AreEqual(200, resultObj!.StatusCode);
        Assert.AreEqual(products, resultObj.Value);
    }

    [TestMethod]
    public void GetProducts_WhenNoProducts_ThrowsNotFoundException()
    {
        productServiceMock!.Setup(s => s.GetProducts(It.IsAny<ProductFilterDto>())).Throws(new NotFoundException("No products found."));
        var filters = new ProductFilterDto
        {
            ProductLine = validProductLine,
            Name = validProductName
        };

        Assert.ThrowsException<NotFoundException>(() => productController!.GetProducts(filters));
    }

    [TestMethod]
    public void GetMostRequestedProducts_WhenValidProducts_ReturnsOkWithProdcuts()
    {
        var products = new List<ProductDto> { validProduct };
        var dates = new DateRangeDto
        {
            DateFrom = DateTime.Now.AddDays(-7),
            DateTo = DateTime.Now
        };
        productServiceMock!.Setup(s => s.GetMostRequestedProducts(It.IsAny<DateRangeDto>())).Returns(products);

        var result = productController!.GetMostRequestedProducts(dates);
        var resultObj = result as ObjectResult;

        Assert.IsNotNull(resultObj);
        Assert.AreEqual(200, resultObj!.StatusCode);
        Assert.AreEqual(products, resultObj.Value);
    }

    [TestMethod]
    public void GetMostRequestedProducts_WhenNoProducts_ThrowsNotFoundException()
    {
        var dates = new DateRangeDto
        {
            DateFrom = DateTime.Now.AddDays(-7),
            DateTo = DateTime.Now
        };
        productServiceMock!.Setup(s => s.GetMostRequestedProducts(It.IsAny<DateRangeDto>())).Throws(new NotFoundException("No products found."));

        Assert.ThrowsException<NotFoundException>(() => productController!.GetMostRequestedProducts(dates));
    }

    [TestMethod]
    public void UpdateProduct_WhenValidParams_ReturnsOk()
    {
        productServiceMock!.Setup(s => s.UpdateProduct(validProduct, "admin@gmail.com"));

        var result = productController!.UpdateProduct(validProduct);
        var resultObj = result as ObjectResult;

        Assert.IsNotNull(resultObj);
        Assert.AreEqual(200, resultObj!.StatusCode);
    }

    [TestMethod]
    public void UpdateProduct_WhenProductNotFound_ThrowsNotFoundException()
    {
        productServiceMock!.Setup(s => s.UpdateProduct(validProduct, "admin@gmail.com")).Throws(new NotFoundException("Product not found."));

        Assert.ThrowsException<NotFoundException>(() => productController!.UpdateProduct(validProduct));
    }

    [TestMethod]
    public void UpdateProductState_WhenProductFound_ReturnsOk()
    {
        var id = 1;
        var status = new ProductStatusDto(true);
        productServiceMock!.Setup(s => s.UpdateStatus(id, status, "admin@gmail.com"));

        var result = productController!.UpdateStatus(id, status);
        var resultObj = result as ObjectResult;

        Assert.IsNotNull(resultObj);
        Assert.AreEqual(200, resultObj!.StatusCode);
    }

    [TestMethod]
    public void UpdateProductState_WhenProductNotFound_ThrowsNotFoundException()
    {
        var id = 1;
        var status = new ProductStatusDto(true);
        productServiceMock!.Setup(s => s.UpdateStatus(id, status, "admin@gmail.com")).Throws(new NotFoundException("Product not found."));

        Assert.ThrowsException<NotFoundException>(() => productController!.UpdateStatus(id, status));
    }
}

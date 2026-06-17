using DarkKitchen.API.Controllers;
using DarkKitchen.Domain.Interfaces.Service;
using DarkKitchen.Models.ProductDTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace DarkKitchen.Tests.API.Controllers;

[TestClass]
public class ProductImportersControllerTest
{
    private ProductImportersController? controller;
    private Mock<IProductImporterService>? productImporterServiceMock;

    [TestInitialize]
    public void Setup()
    {
        productImporterServiceMock = new Mock<IProductImporterService>(MockBehavior.Strict);
        controller = new ProductImportersController(productImporterServiceMock.Object);
    }

    [TestMethod]
    public void GetAvailableImporters_ReturnsOkWithList()
    {
        var importers = new List<ImporterInfoDto>
        {
            new("JSON", ".json"),
            new("XML", ".xml"),
        };
        productImporterServiceMock!.Setup(s => s.GetAvailableImporters()).Returns(importers);

        var result = controller!.GetAvailableImporters();
        var resultObj = result as ObjectResult;

        Assert.IsNotNull(resultObj);
        Assert.AreEqual(200, resultObj!.StatusCode);
        Assert.AreEqual(importers, resultObj.Value);
    }

    [TestMethod]
    public void ImportProducts_WhenValidFile_ReturnsOk()
    {
        var fileMock = new Mock<IFormFile>();
        var ms = new MemoryStream(System.Text.Encoding.UTF8.GetBytes("[]"));
        fileMock.Setup(f => f.Length).Returns(ms.Length);
        fileMock.Setup(f => f.OpenReadStream()).Returns(ms);
        productImporterServiceMock!.Setup(s => s.ImportProducts("JSON", It.IsAny<Stream>())).Returns(3);

        var result = controller!.ImportProducts("JSON", fileMock.Object);
        var resultObj = result as ObjectResult;

        Assert.IsNotNull(resultObj);
        Assert.AreEqual(200, resultObj!.StatusCode);
    }

    [TestMethod]
    public void ImportProducts_WhenFileIsNull_ReturnsBadRequest()
    {
        var result = controller!.ImportProducts("JSON", null!);
        var resultObj = result as ObjectResult;

        Assert.IsNotNull(resultObj);
        Assert.AreEqual(400, resultObj!.StatusCode);
    }

    [TestMethod]
    public void ImportProducts_WhenFileIsEmpty_ReturnsBadRequest()
    {
        var fileMock = new Mock<IFormFile>();
        fileMock.Setup(f => f.Length).Returns(0);

        var result = controller!.ImportProducts("JSON", fileMock.Object);
        var resultObj = result as ObjectResult;

        Assert.IsNotNull(resultObj);
        Assert.AreEqual(400, resultObj!.StatusCode);
    }

    [TestMethod]
    public void UploadImporter_WhenValidFile_ReturnsOk()
    {
        var fileMock = new Mock<IFormFile>();
        var ms = new MemoryStream([1, 2, 3]);
        fileMock.Setup(f => f.Length).Returns(ms.Length);
        fileMock.Setup(f => f.FileName).Returns("Custom.dll");
        fileMock.Setup(f => f.OpenReadStream()).Returns(ms);
        productImporterServiceMock!.Setup(s => s.UploadImporter("Custom.dll", It.IsAny<Stream>()));

        var result = controller!.UploadImporter(fileMock.Object);
        var resultObj = result as ObjectResult;

        Assert.IsNotNull(resultObj);
        Assert.AreEqual(200, resultObj!.StatusCode);
    }

    [TestMethod]
    public void UploadImporter_WhenFileIsNull_ReturnsBadRequest()
    {
        var result = controller!.UploadImporter(null!);
        var resultObj = result as ObjectResult;

        Assert.IsNotNull(resultObj);
        Assert.AreEqual(400, resultObj!.StatusCode);
    }

    [TestMethod]
    public void UploadImporter_WhenFileIsEmpty_ReturnsBadRequest()
    {
        var fileMock = new Mock<IFormFile>();
        fileMock.Setup(f => f.Length).Returns(0);

        var result = controller!.UploadImporter(fileMock.Object);
        var resultObj = result as ObjectResult;

        Assert.IsNotNull(resultObj);
        Assert.AreEqual(400, resultObj!.StatusCode);
    }
}

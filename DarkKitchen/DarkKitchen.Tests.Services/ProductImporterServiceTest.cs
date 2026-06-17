using DarkKitchen.Domain.Entities;
using DarkKitchen.Domain.Exceptions;
using DarkKitchen.Domain.Interfaces.Repository;
using DarkKitchen.Domain.Interfaces.Service;
using DarkKitchen.Models.ProductDTOs;
using DarkKitchen.Services;
using Moq;

namespace DarkKitchen.Tests.Services;

[TestClass]
public class ProductImporterServiceTest
{
    private Mock<IImporterRegistry>? registryMock;
    private Mock<IProductRepository>? productRepositoryMock;
    private Mock<IProductImporter>? importerMock;
    private ProductImporterService? service;

    [TestInitialize]
    public void Setup()
    {
        registryMock = new Mock<IImporterRegistry>(MockBehavior.Strict);
        productRepositoryMock = new Mock<IProductRepository>(MockBehavior.Strict);
        importerMock = new Mock<IProductImporter>(MockBehavior.Strict);
        service = new ProductImporterService(registryMock.Object, productRepositoryMock.Object);
    }

    [TestMethod]
    public void GetAvailableImporters_WhenCalled_ReturnsRegistryEntries()
    {
        importerMock!.Setup(i => i.Name).Returns("JSON");
        importerMock.Setup(i => i.Extension).Returns(".json");
        registryMock!.Setup(r => r.Refresh());
        registryMock.Setup(r => r.GetAll()).Returns([importerMock.Object]);

        var result = service!.GetAvailableImporters().ToList();

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("JSON", result[0].name);
        Assert.AreEqual(".json", result[0].extension);
    }

    [TestMethod]
    public void GetAvailableImporters_WhenCalled_RefreshesRegistry()
    {
        registryMock!.Setup(r => r.Refresh());
        registryMock.Setup(r => r.GetAll()).Returns([]);

        _ = service!.GetAvailableImporters().ToList();

        registryMock.Verify(r => r.Refresh(), Times.Once);
    }

    [TestMethod]
    public void UploadImporter_WhenValidDll_InstallsIntoRegistry()
    {
        using var stream = new MemoryStream();
        registryMock!.Setup(r => r.InstallImporter("Custom.dll", stream));

        service!.UploadImporter("Custom.dll", stream);

        registryMock.Verify(r => r.InstallImporter("Custom.dll", stream), Times.Once);
    }

    [TestMethod]
    [ExpectedException(typeof(BadRequestException))]
    public void UploadImporter_WhenFileNameEmpty_ThrowsBadRequest()
    {
        using var stream = new MemoryStream();
        service!.UploadImporter(string.Empty, stream);
    }

    [TestMethod]
    [ExpectedException(typeof(BadRequestException))]
    public void UploadImporter_WhenNotDll_ThrowsBadRequest()
    {
        using var stream = new MemoryStream();
        service!.UploadImporter("Custom.txt", stream);
    }

    [TestMethod]
    public void ImportProducts_WhenValid_AddsAllAndReturnsCount()
    {
        var items = new List<ImportedProductDto>
        {
            new("P001", "Pizza", 250, "desc", "line", "cat", ["/img/p1.jpg"]),
            new("P002", "Burger", 180, null, null, null, []),
        };
        SetupImporterAndRegistry(items);
        productRepositoryMock!.Setup(r => r.GetByCode(It.IsAny<string>())).Returns((Product?)null);
        productRepositoryMock.Setup(r => r.Add(It.IsAny<Product>()));

        using var stream = new MemoryStream();
        var imported = service!.ImportProducts("JSON", stream);

        Assert.AreEqual(2, imported);
        productRepositoryMock.Verify(r => r.Add(It.IsAny<Product>()), Times.Exactly(2));
    }

    [TestMethod]
    public void ImportProducts_WhenValid_MapsAllFields()
    {
        var items = new List<ImportedProductDto>
        {
            new("P001", "Pizza", 250.5m, "desc", "line", "cat", ["/img/p1.jpg", "/img/p2.jpg"]),
        };
        SetupImporterAndRegistry(items);
        productRepositoryMock!.Setup(r => r.GetByCode("P001")).Returns((Product?)null);
        Product? captured = null;
        productRepositoryMock.Setup(r => r.Add(It.IsAny<Product>())).Callback<Product>(p => captured = p);

        using var stream = new MemoryStream();
        service!.ImportProducts("JSON", stream);

        Assert.IsNotNull(captured);
        Assert.AreEqual("P001", captured.Code);
        Assert.AreEqual("Pizza", captured.Name);
        Assert.AreEqual(250.5m, captured.Price);
        Assert.AreEqual("desc", captured.Description);
        Assert.AreEqual("line", captured.ProductLine);
        Assert.AreEqual("cat", captured.Category);
        Assert.IsTrue(captured.IsActive);
        Assert.AreEqual(2, captured.Images.Count);
        Assert.AreEqual("/img/p1.jpg", captured.Images[0].Url);
    }

    [TestMethod]
    [ExpectedException(typeof(BadRequestException))]
    public void ImportProducts_WhenImporterNameEmpty_ThrowsBadRequest()
    {
        using var stream = new MemoryStream();
        service!.ImportProducts(string.Empty, stream);
    }

    [TestMethod]
    [ExpectedException(typeof(BadRequestException))]
    public void ImportProducts_WhenSourceHasNoItems_ThrowsBadRequest()
    {
        SetupImporterAndRegistry([]);

        using var stream = new MemoryStream();
        service!.ImportProducts("JSON", stream);
    }

    [TestMethod]
    [ExpectedException(typeof(BadRequestException))]
    public void ImportProducts_WhenCodeMissing_ThrowsBadRequest()
    {
        var items = new List<ImportedProductDto>
        {
            new(string.Empty, "Pizza", 250, null, null, null, []),
        };
        SetupImporterAndRegistry(items);

        using var stream = new MemoryStream();
        service!.ImportProducts("JSON", stream);
    }

    [TestMethod]
    [ExpectedException(typeof(BadRequestException))]
    public void ImportProducts_WhenNameMissing_ThrowsBadRequest()
    {
        var items = new List<ImportedProductDto>
        {
            new("P001", string.Empty, 250, null, null, null, []),
        };
        SetupImporterAndRegistry(items);

        using var stream = new MemoryStream();
        service!.ImportProducts("JSON", stream);
    }

    [TestMethod]
    [ExpectedException(typeof(BadRequestException))]
    public void ImportProducts_WhenDuplicateCodesInBatch_ThrowsBadRequest()
    {
        var items = new List<ImportedProductDto>
        {
            new("P001", "Pizza", 250, null, null, null, []),
            new("P001", "Burger", 180, null, null, null, []),
        };
        SetupImporterAndRegistry(items);
        productRepositoryMock!.Setup(r => r.GetByCode("P001")).Returns((Product?)null);

        using var stream = new MemoryStream();
        service!.ImportProducts("JSON", stream);
    }

    [TestMethod]
    public void ImportProducts_WhenCodeExistsInDb_ThrowsBadRequestAndAddsNothing()
    {
        var items = new List<ImportedProductDto>
        {
            new("P001", "Pizza", 250, null, null, null, []),
            new("P002", "Burger", 180, null, null, null, []),
        };
        SetupImporterAndRegistry(items);
        productRepositoryMock!.Setup(r => r.GetByCode("P001")).Returns((Product?)null);
        productRepositoryMock.Setup(r => r.GetByCode("P002")).Returns(new Product { Code = "P002", Name = "Existing" });

        using var stream = new MemoryStream();
        Assert.ThrowsException<BadRequestException>(() => service!.ImportProducts("JSON", stream));
        productRepositoryMock.Verify(r => r.Add(It.IsAny<Product>()), Times.Never);
    }

    [TestMethod]
    public void ImportProducts_WhenImagePathsIsNull_MapsToEmptyImages()
    {
        var items = new List<ImportedProductDto>
        {
            new("P001", "Pizza", 250, null, null, null, null!),
        };
        SetupImporterAndRegistry(items);
        productRepositoryMock!.Setup(r => r.GetByCode("P001")).Returns((Product?)null);
        Product? captured = null;
        productRepositoryMock.Setup(r => r.Add(It.IsAny<Product>())).Callback<Product>(p => captured = p);

        using var stream = new MemoryStream();
        service!.ImportProducts("JSON", stream);

        Assert.IsNotNull(captured);
        Assert.AreEqual(0, captured.Images.Count);
    }

    [TestMethod]
    [ExpectedException(typeof(NotFoundException))]
    public void ImportProducts_WhenImporterUnknown_ThrowsNotFound()
    {
        registryMock!.Setup(r => r.Refresh());
        registryMock.Setup(r => r.Get("Bogus")).Throws(new NotFoundException("Importer 'Bogus' not found."));

        using var stream = new MemoryStream();
        service!.ImportProducts("Bogus", stream);
    }

    private void SetupImporterAndRegistry(List<ImportedProductDto> items)
    {
        importerMock!.Setup(i => i.Import(It.IsAny<Stream>())).Returns(items);
        registryMock!.Setup(r => r.Refresh());
        registryMock.Setup(r => r.Get("JSON")).Returns(importerMock.Object);
    }
}

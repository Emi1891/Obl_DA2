using System.Text;
using DarkKitchen.Domain.Exceptions;
using DarkKitchen.Services.Importers;

namespace DarkKitchen.Tests.Services.Importers;

[TestClass]
public class JsonProductImporterTest
{
    private JsonProductImporter? importer;

    [TestInitialize]
    public void Setup()
    {
        importer = new JsonProductImporter();
    }

    [TestMethod]
    public void Name_Returns_Json()
    {
        Assert.AreEqual("JSON", importer!.Name);
    }

    [TestMethod]
    public void Extension_Returns_DotJson()
    {
        Assert.AreEqual(".json", importer!.Extension);
    }

    [TestMethod]
    public void Import_WhenValidJson_ReturnsProducts()
    {
        var json = """
        [
          {
            "code": "P001",
            "name": "Pizza",
            "price": 250.50,
            "description": "Tasty",
            "productLine": "Main",
            "category": "Italian",
            "imagePaths": ["/img/pizza1.jpg", "/img/pizza2.jpg"]
          },
          {
            "code": "P002",
            "name": "Burger",
            "price": 180,
            "description": null,
            "productLine": null,
            "category": null,
            "imagePaths": []
          }
        ]
        """;
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));

        var products = importer!.Import(stream).ToList();

        Assert.AreEqual(2, products.Count);
        Assert.AreEqual("P001", products[0].code);
        Assert.AreEqual("Pizza", products[0].name);
        Assert.AreEqual(250.50m, products[0].price);
        Assert.AreEqual(2, products[0].imagePaths.Count);
        Assert.AreEqual("P002", products[1].code);
        Assert.AreEqual(0, products[1].imagePaths.Count);
    }

    [TestMethod]
    [ExpectedException(typeof(BadRequestException))]
    public void Import_WhenMalformedJson_ThrowsBadRequest()
    {
        var json = "{ this is not json";
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));

        importer!.Import(stream).ToList();
    }

    [TestMethod]
    [ExpectedException(typeof(BadRequestException))]
    public void Import_WhenJsonIsNullLiteral_ThrowsBadRequest()
    {
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes("null"));

        importer!.Import(stream).ToList();
    }
}

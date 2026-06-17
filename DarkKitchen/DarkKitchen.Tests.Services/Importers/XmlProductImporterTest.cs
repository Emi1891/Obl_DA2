using System.Text;
using DarkKitchen.Domain.Exceptions;
using DarkKitchen.Services.Importers;

namespace DarkKitchen.Tests.Services.Importers;

[TestClass]
public class XmlProductImporterTest
{
    private XmlProductImporter? importer;

    [TestInitialize]
    public void Setup()
    {
        importer = new XmlProductImporter();
    }

    [TestMethod]
    public void Name_Returns_Xml()
    {
        Assert.AreEqual("XML", importer!.Name);
    }

    [TestMethod]
    public void Extension_Returns_DotXml()
    {
        Assert.AreEqual(".xml", importer!.Extension);
    }

    [TestMethod]
    public void Import_WhenValidXml_ReturnsProducts()
    {
        var xml = """
        <?xml version="1.0" encoding="utf-8"?>
        <products>
          <product>
            <code>P001</code>
            <name>Pizza</name>
            <price>250.50</price>
            <description>Tasty</description>
            <productLine>Main</productLine>
            <category>Italian</category>
            <imagePaths>
              <imagePath>/img/pizza1.jpg</imagePath>
              <imagePath>/img/pizza2.jpg</imagePath>
            </imagePaths>
          </product>
          <product>
            <code>P002</code>
            <name>Burger</name>
            <price>180</price>
          </product>
        </products>
        """;
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(xml));

        var products = importer!.Import(stream).ToList();

        Assert.AreEqual(2, products.Count);
        Assert.AreEqual("P001", products[0].code);
        Assert.AreEqual(250.50m, products[0].price);
        Assert.AreEqual(2, products[0].imagePaths.Count);
        Assert.AreEqual("P002", products[1].code);
        Assert.AreEqual(0, products[1].imagePaths.Count);
    }

    [TestMethod]
    [ExpectedException(typeof(BadRequestException))]
    public void Import_WhenMalformedXml_ThrowsBadRequest()
    {
        var xml = "<products><product><code>P001</";
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(xml));

        importer!.Import(stream).ToList();
    }

    [TestMethod]
    [ExpectedException(typeof(BadRequestException))]
    public void Import_WhenInvalidPrice_ThrowsBadRequest()
    {
        var xml = """
        <products>
          <product>
            <code>P001</code>
            <name>Pizza</name>
            <price>not-a-number</price>
          </product>
        </products>
        """;
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(xml));

        importer!.Import(stream).ToList();
    }

    [TestMethod]
    [ExpectedException(typeof(BadRequestException))]
    public void Import_WhenPriceMissing_ThrowsBadRequest()
    {
        var xml = """
        <products>
          <product>
            <code>P001</code>
            <name>Pizza</name>
          </product>
        </products>
        """;
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(xml));

        importer!.Import(stream).ToList();
    }

    [TestMethod]
    public void Import_WhenCodeAndNameMissing_MapsToEmptyStrings()
    {
        var xml = """
        <products>
          <product>
            <price>10</price>
          </product>
        </products>
        """;
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(xml));

        var products = importer!.Import(stream).ToList();

        Assert.AreEqual(1, products.Count);
        Assert.AreEqual(string.Empty, products[0].code);
        Assert.AreEqual(string.Empty, products[0].name);
    }
}

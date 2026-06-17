using System.Globalization;
using System.Xml;
using System.Xml.Linq;
using DarkKitchen.Domain.Exceptions;
using DarkKitchen.Domain.Interfaces.Service;
using DarkKitchen.Models.ProductDTOs;

namespace DarkKitchen.Services.Importers;

public class XmlProductImporter : IProductImporter
{
    public string Name => "XML";

    public string Extension => ".xml";

    public IEnumerable<ImportedProductDto> Import(Stream source)
    {
        XDocument doc;
        try
        {
            doc = XDocument.Load(source);
        }
        catch(XmlException ex)
        {
            throw new BadRequestException($"Invalid XML: {ex.Message}");
        }

        return doc.Root!.Elements("product").Select(ParseProduct).ToList();
    }

    private static ImportedProductDto ParseProduct(XElement p)
    {
        var priceText = (string?)p.Element("price")
            ?? throw new BadRequestException("Missing <price> on a product.");
        if(!decimal.TryParse(priceText, NumberStyles.Number, CultureInfo.InvariantCulture, out var price))
        {
            throw new BadRequestException($"Invalid price value: '{priceText}'.");
        }

        var imagePaths = p.Element("imagePaths")?.Elements("imagePath")
            .Select(e => e.Value)
            .ToList() ?? [];

        return new ImportedProductDto(
            code: (string?)p.Element("code") ?? string.Empty,
            name: (string?)p.Element("name") ?? string.Empty,
            price: price,
            description: (string?)p.Element("description"),
            productLine: (string?)p.Element("productLine"),
            category: (string?)p.Element("category"),
            imagePaths: imagePaths);
    }
}

using DarkKitchen.Domain.Interfaces.Service;
using DarkKitchen.Models.ProductDTOs;

namespace DarkKitchen.Tests.Services.Plugins;

public class TestPluginImporter : IProductImporter
{
    public string Name => "TestPlugin";

    public string Extension => ".test";

    public IEnumerable<ImportedProductDto> Import(Stream source) => [];
}

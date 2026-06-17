using DarkKitchen.Domain.Exceptions;
using DarkKitchen.Domain.Interfaces.Service;
using DarkKitchen.Models.ProductDTOs;
using DarkKitchen.Services.Plugins;
using Microsoft.Extensions.Configuration;
using Moq;

namespace DarkKitchen.Tests.Services.Plugins;

[TestClass]
public class ImporterRegistryTest
{
    private Mock<PluginLoader>? loaderMock;
    private Mock<IConfiguration>? configurationMock;

    [TestInitialize]
    public void Setup()
    {
        loaderMock = new Mock<PluginLoader>();
        loaderMock.Setup(l => l.LoadFrom(It.IsAny<string>())).Returns([]);

        configurationMock = new Mock<IConfiguration>();
        configurationMock.Setup(c => c["Plugins:Folder"]).Returns("test-plugins");
    }

    [TestMethod]
    public void GetAll_WhenBuiltInsRegistered_ReturnsAll()
    {
        var json = new FakeImporter("JSON", ".json");
        var xml = new FakeImporter("XML", ".xml");
        var registry = new ImporterRegistry([json, xml], loaderMock!.Object, configurationMock!.Object);

        var all = registry.GetAll().ToList();

        Assert.AreEqual(2, all.Count);
    }

    [TestMethod]
    public void Get_WhenNameMatches_ReturnsImporter()
    {
        var json = new FakeImporter("JSON", ".json");
        var registry = new ImporterRegistry([json], loaderMock!.Object, configurationMock!.Object);

        var result = registry.Get("JSON");

        Assert.AreSame(json, result);
    }

    [TestMethod]
    public void Get_WhenNameIsCaseInsensitive_ReturnsImporter()
    {
        var json = new FakeImporter("JSON", ".json");
        var registry = new ImporterRegistry([json], loaderMock!.Object, configurationMock!.Object);

        var result = registry.Get("json");

        Assert.AreSame(json, result);
    }

    [TestMethod]
    [ExpectedException(typeof(NotFoundException))]
    public void Get_WhenNameUnknown_ThrowsNotFound()
    {
        var registry = new ImporterRegistry([], loaderMock!.Object, configurationMock!.Object);

        registry.Get("Unknown");
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void Constructor_WhenDuplicateBuiltInNames_Throws()
    {
        var a = new FakeImporter("JSON", ".json");
        var b = new FakeImporter("JSON", ".json");

        _ = new ImporterRegistry([a, b], loaderMock!.Object, configurationMock!.Object);
    }

    [TestMethod]
    public void Refresh_WhenPluginNameCollidesWithBuiltIn_KeepsBuiltIn()
    {
        var builtIn = new FakeImporter("JSON", ".json");
        var plugin = new FakeImporter("JSON", ".json");
        var registry = new ImporterRegistry([builtIn], loaderMock!.Object, configurationMock!.Object);
        loaderMock.Setup(l => l.LoadFrom(It.IsAny<string>())).Returns([plugin]);

        registry.Refresh();

        Assert.AreSame(builtIn, registry.Get("JSON"));
        Assert.AreEqual(1, registry.GetAll().Count());
    }

    [TestMethod]
    public void Refresh_WhenPluginsFolderNotConfigured_FallsBackToDefault()
    {
        var emptyConfig = new Mock<IConfiguration>();
        var registry = new ImporterRegistry([], loaderMock!.Object, emptyConfig.Object);

        registry.Refresh();

        loaderMock.Verify(l => l.LoadFrom("Plugins"), Times.Once);
    }

    [TestMethod]
    public void GetAll_WhenPluginsLoadedFromLoader_IncludesThem()
    {
        var plugin = new FakeImporter("CSV", ".csv");
        loaderMock!.Setup(l => l.LoadFrom(It.IsAny<string>())).Returns([plugin]);
        var builtIn = new FakeImporter("JSON", ".json");

        var registry = new ImporterRegistry([builtIn], loaderMock.Object, configurationMock!.Object);
        registry.Refresh();
        var all = registry.GetAll().ToList();

        Assert.AreEqual(2, all.Count);
        Assert.AreSame(plugin, registry.Get("CSV"));
    }

    [TestMethod]
    public void Refresh_WhenNewPluginAppears_AddsImporter()
    {
        var registry = new ImporterRegistry([], loaderMock!.Object, configurationMock!.Object);
        var plugin = new FakeImporter("CSV", ".csv");
        loaderMock.Setup(l => l.LoadFrom(It.IsAny<string>())).Returns([plugin]);

        registry.Refresh();

        Assert.AreSame(plugin, registry.Get("CSV"));
    }

    [TestMethod]
    public void Refresh_WhenPluginAlreadyRegistered_DoesNotDuplicateOrThrow()
    {
        var plugin = new FakeImporter("CSV", ".csv");
        loaderMock!.Setup(l => l.LoadFrom(It.IsAny<string>())).Returns([plugin]);
        var registry = new ImporterRegistry([], loaderMock.Object, configurationMock!.Object);

        registry.Refresh();

        Assert.AreEqual(1, registry.GetAll().Count());
    }

    [TestMethod]
    public void InstallImporter_WhenCalled_SavesFileAndRefreshes()
    {
        var registry = new ImporterRegistry([], loaderMock!.Object, configurationMock!.Object);
        var plugin = new FakeImporter("CSV", ".csv");
        loaderMock.Setup(l => l.LoadFrom(It.IsAny<string>())).Returns([plugin]);
        using var content = new MemoryStream();

        registry.InstallImporter("Custom.dll", content);

        loaderMock.Verify(l => l.Save("test-plugins", "Custom.dll", content), Times.Once);
        Assert.AreSame(plugin, registry.Get("CSV"));
    }

    private sealed class FakeImporter(string name, string extension) : IProductImporter
    {
        public string Name { get; } = name;
        public string Extension { get; } = extension;
        public IEnumerable<ImportedProductDto> Import(Stream source) => [];
    }
}

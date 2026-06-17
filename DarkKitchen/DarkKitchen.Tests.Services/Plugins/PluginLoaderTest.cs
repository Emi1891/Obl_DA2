using DarkKitchen.Services.Plugins;

namespace DarkKitchen.Tests.Services.Plugins;

[TestClass]
public class PluginLoaderTest
{
    private PluginLoader? loader;
    private string? tempFolder;

    [TestInitialize]
    public void Setup()
    {
        loader = new PluginLoader();
        tempFolder = Directory.CreateTempSubdirectory().FullName;
    }

    [TestCleanup]
    public void Cleanup()
    {
        if(Directory.Exists(tempFolder!))
        {
            Directory.Delete(tempFolder!, recursive: true);
        }
    }

    [TestMethod]
    public void LoadFrom_WhenFolderDoesNotExist_ReturnsEmpty()
    {
        var missing = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

        var result = loader!.LoadFrom(missing).ToList();

        Assert.AreEqual(0, result.Count);
    }

    [TestMethod]
    public void LoadFrom_WhenFolderHasNoDlls_ReturnsEmpty()
    {
        var result = loader!.LoadFrom(tempFolder!).ToList();

        Assert.AreEqual(0, result.Count);
    }

    [TestMethod]
    public void LoadFrom_WhenFolderHasValidPluginDll_ReturnsImporterInstance()
    {
        var sourceDll = typeof(TestPluginImporter).Assembly.Location;
        File.Copy(sourceDll, Path.Combine(tempFolder!, Path.GetFileName(sourceDll)));

        var result = loader!.LoadFrom(tempFolder!).ToList();

        Assert.IsTrue(result.Any(i => i is TestPluginImporter));
    }

    [TestMethod]
    public void LoadFrom_WhenDllIsMalformed_SkipsItAndReturnsEmpty()
    {
        File.WriteAllText(Path.Combine(tempFolder!, "bogus.dll"), "not a real assembly");

        var result = loader!.LoadFrom(tempFolder!).ToList();

        Assert.AreEqual(0, result.Count);
    }

    [TestMethod]
    public void Save_WhenCalled_WritesFileContentToFolder()
    {
        var bytes = new byte[] { 1, 2, 3, 4 };
        using var content = new MemoryStream(bytes);

        loader!.Save(tempFolder!, "Custom.dll", content);

        var saved = Path.Combine(tempFolder!, "Custom.dll");
        Assert.IsTrue(File.Exists(saved));
        CollectionAssert.AreEqual(bytes, File.ReadAllBytes(saved));
    }

    [TestMethod]
    public void Save_WhenFolderDoesNotExist_CreatesItAndWritesFile()
    {
        var nested = Path.Combine(tempFolder!, "nested");
        using var content = new MemoryStream([9]);

        loader!.Save(nested, "Custom.dll", content);

        Assert.IsTrue(File.Exists(Path.Combine(nested, "Custom.dll")));
    }
}

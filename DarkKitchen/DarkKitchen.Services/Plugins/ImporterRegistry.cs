using DarkKitchen.Domain.Exceptions;
using DarkKitchen.Domain.Interfaces.Service;
using Microsoft.Extensions.Configuration;

namespace DarkKitchen.Services.Plugins;

public class ImporterRegistry : IImporterRegistry
{
    private readonly Dictionary<string, IProductImporter> _importers
        = new(StringComparer.OrdinalIgnoreCase);
    private readonly PluginLoader _loader;
    private readonly string _pluginsFolder;

    public ImporterRegistry(IEnumerable<IProductImporter> builtIns, PluginLoader loader, IConfiguration configuration)
    {
        _loader = loader;
        _pluginsFolder = configuration["Plugins:Folder"] ?? "Plugins";

        foreach(var importer in builtIns)
        {
            Register(importer);
        }
    }

    public IEnumerable<IProductImporter> GetAll() => _importers.Values;

    public void Refresh()
    {
        foreach(var importer in _loader.LoadFrom(_pluginsFolder))
        {
            _importers.TryAdd(importer.Name, importer);
        }
    }

    public void InstallImporter(string fileName, Stream content)
    {
        _loader.Save(_pluginsFolder, fileName, content);
        Refresh();
    }

    public IProductImporter Get(string name)
    {
        if(!_importers.TryGetValue(name, out var importer))
        {
            throw new NotFoundException($"Importer '{name}' not found.");
        }

        return importer;
    }

    private void Register(IProductImporter importer)
    {
        if(!_importers.TryAdd(importer.Name, importer))
        {
            throw new InvalidOperationException($"Duplicate importer name: '{importer.Name}'.");
        }
    }
}

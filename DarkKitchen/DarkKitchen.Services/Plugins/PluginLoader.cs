using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using DarkKitchen.Domain.Interfaces.Service;

namespace DarkKitchen.Services.Plugins;

public class PluginLoader
{
    public virtual IEnumerable<IProductImporter> LoadFrom(string pluginsFolder)
    {
        if(!Directory.Exists(pluginsFolder))
        {
            return [];
        }

        var importers = new List<IProductImporter>();

        foreach(var dllPath in Directory.GetFiles(pluginsFolder, "*.dll"))
        {
            Assembly assembly;
            try
            {
                assembly = Assembly.LoadFrom(dllPath);
            }
            catch(BadImageFormatException)
            {
                continue;
            }

            foreach(var type in GetTypesSafely(assembly))
            {
                if(!typeof(IProductImporter).IsAssignableFrom(type)
                    || type.IsAbstract
                    || type.IsInterface
                    || type.GetConstructor(Type.EmptyTypes) == null)
                {
                    continue;
                }

                if(Activator.CreateInstance(type) is IProductImporter instance)
                {
                    importers.Add(instance);
                }
            }
        }

        return importers;
    }

    public virtual void Save(string pluginsFolder, string fileName, Stream content)
    {
        Directory.CreateDirectory(pluginsFolder);
        var destination = Path.Combine(pluginsFolder, Path.GetFileName(fileName));
        using var file = File.Create(destination);
        content.CopyTo(file);
    }

    [ExcludeFromCodeCoverage]
    private static Type[] GetTypesSafely(Assembly assembly)
    {
        try
        {
            return assembly.GetTypes();
        }
        catch(ReflectionTypeLoadException ex)
        {
            return ex.Types.Where(t => t != null).ToArray()!;
        }
    }
}

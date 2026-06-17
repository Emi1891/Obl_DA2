using System.Diagnostics.CodeAnalysis;

namespace DarkKitchen.API.Extensions;

[ExcludeFromCodeCoverage]
public static class PluginExtensions
{
    public static WebApplicationBuilder ConfigurePluginsFolder(this WebApplicationBuilder builder)
    {
        var pluginsFolder = builder.Configuration["Plugins:Folder"];
        if(string.IsNullOrEmpty(pluginsFolder))
        {
            pluginsFolder = Path.Combine(builder.Environment.ContentRootPath, "Plugins");
            builder.Configuration["Plugins:Folder"] = pluginsFolder;
        }

        Directory.CreateDirectory(pluginsFolder);
        return builder;
    }
}

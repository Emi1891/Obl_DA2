using System.Diagnostics.CodeAnalysis;
using DarkKitchen.Domain.Interfaces.Service;
using DarkKitchen.Services;
using DarkKitchen.Services.Importers;
using DarkKitchen.Services.Plugins;

namespace DarkKitchen.API.Extensions;

[ExcludeFromCodeCoverage]
public static class ServiceExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<IPromotionService, PromotionService>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<ISessionService, SessionService>();
        services.AddScoped<IRolePermissionsService, RolePermissionsService>();
        services.AddScoped<IAuditService, AuditService>();
        services.AddScoped<IShippingTypeService, ShippingTypeService>();
        services.AddScoped<IProductImporterService, ProductImporterService>();

        services.AddSingleton<IProductImporter, JsonProductImporter>();
        services.AddSingleton<IProductImporter, XmlProductImporter>();
        services.AddSingleton<PluginLoader>();
        services.AddSingleton<IImporterRegistry, ImporterRegistry>();

        return services;
    }
}

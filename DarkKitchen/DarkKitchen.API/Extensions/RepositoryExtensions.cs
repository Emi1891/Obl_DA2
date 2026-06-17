using System.Diagnostics.CodeAnalysis;
using DarkKitchen.DataAccess.Repositories;
using DarkKitchen.Domain.Interfaces.Repository;

namespace DarkKitchen.API.Extensions;

[ExcludeFromCodeCoverage]
public static class RepositoryExtensions
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IPromotionRepository, PromotionRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IRolePermissionsRepository, RolePermissionsRepository>();
        services.AddScoped<IAuditRepository, AuditRepository>();
        services.AddScoped<IShippingTypeRepository, ShippingTypeRepository>();
        return services;
    }
}

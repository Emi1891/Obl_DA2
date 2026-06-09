using DarkKitchen.Domain.Entities;
using DarkKitchen.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace DarkKitchen.DataAccess;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderProduct> OrderProducts { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<ProductImage> ProductImages { get; set; }
    public DbSet<Promotion> Promotion { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<RolePermissions> RolePermissions { get; set; }
    public DbSet<AuditRecord> AuditRecords { get; set; }
    public DbSet<ShippingType> ShippingTypes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<RolePermissions>(b =>
        {
            b.HasKey(r => r.Role);

            b.HasData(
                new RolePermissions
                {
                    Role = Role.Admin,
                    Permissions =
                    [
                        Permission.GetOrderDetails,
                        Permission.SetOrderStatusToPrepared,
                        Permission.SetOrderStatusToCanceled,
                        Permission.SetOrderStatusToOnItsWay,
                        Permission.SetOrderStatusToDelivered,
                        Permission.CreateUser,
                        Permission.GetUsers,
                        Permission.UpdateUser,
                        Permission.DeleteUser,
                        Permission.CreateProduct,
                        Permission.GetProducts,
                        Permission.GetMostPopularProducts,
                        Permission.UpdateProduct,
                        Permission.UpdateProductStatus,
                        Permission.CreatePromotion,
                        Permission.GetCurrentPromotions,
                        Permission.UpdatePromotion,
                        Permission.UpdatePromotionProducts,
                        Permission.GetSalesReport,
                        Permission.GetAuditRecords,
                        Permission.GetShippingTypes,
                        Permission.CreateShippingType,
                        Permission.UpdateShippingType,
                        Permission.ImportProducts,
                    ]
                },
                new RolePermissions
                {
                    Role = Role.Client,
                    Permissions =
                    [
                        Permission.PlaceOrder,
                        Permission.GetMyOrders,
                        Permission.GetProducts,
                        Permission.GetCurrentPromotions,
                        Permission.GetShippingTypes
                    ]
                },
                new RolePermissions
                {
                    Role = Role.Dispatcher,
                    Permissions =
                    [
                        Permission.GetOrdersByStatus,
                        Permission.GetOrderDetails,
                        Permission.SetOrderStatusToPrepared,
                        Permission.SetOrderStatusToOnItsWay,
                        Permission.SetOrderStatusToDelivered,
                        Permission.SetOrderStatusToNotDelivered,
                        Permission.SetOrderStatusToDelayed
                    ]
                });
        });

        modelBuilder.Entity<User>(b =>
        {
            b.HasOne<RolePermissions>()
                .WithMany()
                .HasForeignKey(u => u.Role)
                .OnDelete(DeleteBehavior.Restrict);
            b.HasIndex(u => u.Email).IsUnique();
        });

        modelBuilder.Entity<Product>(b =>
        {
            b.HasIndex(p => p.Code).IsUnique();
        });

        modelBuilder.Entity<Promotion>(b =>
        {
            b.HasMany(p => p.Products)
                .WithMany();
        });

        modelBuilder.Entity<Order>(b =>
        {
            b.HasOne<User>()
                .WithMany()
                .HasForeignKey(o => o.ClientId)
                .OnDelete(DeleteBehavior.Restrict);

            b.OwnsOne(o => o.Address, a =>
            {
                a.Property(x => x.Street).HasColumnName("Street");
                a.Property(x => x.DoorNumber).HasColumnName("DoorNumber");
                a.Property(x => x.Apartment).HasColumnName("Apartment");
            });
        });

        modelBuilder.Entity<OrderProduct>(b =>
        {
            b.HasOne<Product>()
                .WithMany()
                .HasForeignKey(op => op.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<ShippingType>(b =>
        {
            b.HasIndex(st => st.Name).IsUnique();
            b.HasData(
                new ShippingType { Id = 1, Name = "Express", Price = 250 },
                new ShippingType { Id = 2, Name = "Same day", Price = 200 },
                new ShippingType { Id = 3, Name = "Next day", Price = 180 });
        });
    }
}

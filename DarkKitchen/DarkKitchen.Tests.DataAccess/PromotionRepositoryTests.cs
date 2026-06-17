using DarkKitchen.DataAccess;
using DarkKitchen.DataAccess.Repositories;
using DarkKitchen.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DarkKitchen.Tests.DataAccess;

[TestClass]
public class PromotionRepositoryTests
{
    private Promotion? promotion;
    private AppDbContext? context;
    private PromotionRepository? promotionRepository;

    [TestInitialize]
    public void TestInitialize()
    {
        promotion = new Promotion
        {
            Name = "Black Friday",
            DiscountPercentage = 10,
            DateFrom = new DateTime(2026, 1, 25),
            DateTo = new DateTime(2026, 1, 30)
        };

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        context = new AppDbContext(options);
        promotionRepository = new PromotionRepository(context);
    }

    [TestCleanup]
    public void TestCleanup()
    {
        context!.Database.EnsureDeleted();
        context.Dispose();
    }

    [TestMethod]
    public void AddPromotion_WhenPromotionIsValid_AddsPromotionToDatabase()
    {
        promotionRepository!.Add(promotion!);

        var result = context!.Promotion.FirstOrDefault(p => p.Name == "Black Friday");

        Assert.IsNotNull(result);
        Assert.AreEqual("Black Friday", result.Name);
    }

    [TestMethod]
    public void GetById_WhenPromotionExists_ReturnsPromotion()
    {
        context!.Promotion.Add(promotion!);
        context.SaveChanges();

        var result = promotionRepository!.GetById(promotion!.Id);

        Assert.IsNotNull(result);
        Assert.AreEqual("Black Friday", result.Name);
    }

    [TestMethod]
    public void GetById_WhenPromotionDoesNotExist_ReturnsNull()
    {
        var result = promotionRepository!.GetById(999);

        Assert.IsNull(result);
    }

    [TestMethod]
    public void UpdatePromotion_WhenPromotionExists_UpdatesPromotionInDatabase()
    {
        context!.Promotion.Add(promotion!);
        context.SaveChanges();
        promotion!.Name = "Updated Promotion";

        promotionRepository!.Update(promotion!);
        var result = context.Promotion.FirstOrDefault(p => p.Id == promotion.Id);

        Assert.AreEqual("Updated Promotion", result!.Name);
    }

    [TestMethod]
    public void UpdatePromotion_WhenPromotionDoesNotExist_ThrowsException()
    {
        Assert.ThrowsException<ArgumentNullException>(() =>
        {
            promotionRepository!.Update(promotion!);
        });
    }

    [TestMethod]
    public void GetPromotions_WhenDateFilter_ReturnsMatchingPromotions()
    {
        context!.Promotion.Add(promotion!);
        context.SaveChanges();

        var result = promotionRepository!.GetPromotions(new DateTime(2026, 1, 27), null, null);

        Assert.AreEqual(1, result.Count());
    }

    [TestMethod]
    public void GetPromotions_WhenNoFilter_ReturnsAllPromotions()
    {
        context!.Promotion.Add(promotion!);
        context.SaveChanges();

        var result = promotionRepository!.GetPromotions(null, null, null);

        Assert.AreEqual(1, result.Count());
    }

    [TestMethod]
    public void GetPromotions_WhenNoPromotionsExist_ReturnsEmptyList()
    {
        var result = promotionRepository!.GetPromotions(null, null, null);

        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.Count());
    }

    [TestMethod]
    public void GetPromotions_WhenProductLineFilterMatches_ReturnsMatchingPromotions()
    {
        promotion!.Products = [new Product { Code = "P1", Name = "Pizza", Price = 100, ProductLine = "Pizzas", IsActive = true }];
        context!.Promotion.Add(promotion);
        context.SaveChanges();

        var result = promotionRepository!.GetPromotions(null, "Pizzas", null);

        Assert.AreEqual(1, result.Count());
    }

    [TestMethod]
    public void GetPromotions_WhenProductLineFilterDoesNotMatch_ReturnsEmptyList()
    {
        promotion!.Products = [new Product { Code = "P1", Name = "Pizza", Price = 100, ProductLine = "Pizzas", IsActive = true }];
        context!.Promotion.Add(promotion);
        context.SaveChanges();

        var result = promotionRepository!.GetPromotions(null, "Bebidas", null);

        Assert.AreEqual(0, result.Count());
    }

    [TestMethod]
    public void GetPromotions_WhenProductNameFilterMatches_ReturnsMatchingPromotions()
    {
        promotion!.Products = [new Product { Code = "P1", Name = "Pizza Muzzarella", Price = 100, IsActive = true }];
        context!.Promotion.Add(promotion);
        context.SaveChanges();

        var result = promotionRepository!.GetPromotions(null, null, "Muzzarella");

        Assert.AreEqual(1, result.Count());
    }

    [TestMethod]
    public void GetPromotions_WhenProductNameFilterDoesNotMatch_ReturnsEmptyList()
    {
        promotion!.Products = [new Product { Code = "P1", Name = "Pizza Muzzarella", Price = 100, IsActive = true }];
        context!.Promotion.Add(promotion);
        context.SaveChanges();

        var result = promotionRepository!.GetPromotions(null, null, "Hamburguesa");

        Assert.AreEqual(0, result.Count());
    }

    [TestMethod]
    public void GetActiveForProducts_WhenActivePromotionCoversProduct_ReturnsPromotion()
    {
        var product = new Product { Id = 1, Code = "P1", Name = "P1", Price = 100, IsActive = true };
        promotion!.Products = [product];
        context!.Promotion.Add(promotion);
        context.SaveChanges();

        var result = promotionRepository!.GetActiveForProducts([1], new DateTime(2026, 1, 27)).ToList();

        Assert.AreEqual(1, result.Count);
    }

    [TestMethod]
    public void GetActiveForProducts_WhenDateOutsideRange_ReturnsEmpty()
    {
        var product = new Product { Id = 1, Code = "P1", Name = "P1", Price = 100, IsActive = true };
        promotion!.Products = [product];
        context!.Promotion.Add(promotion);
        context.SaveChanges();

        var result = promotionRepository!.GetActiveForProducts([1], new DateTime(2026, 2, 15)).ToList();

        Assert.AreEqual(0, result.Count);
    }

    [TestMethod]
    public void GetActiveForProducts_WhenProductNotInPromotion_ReturnsEmpty()
    {
        var product = new Product { Id = 1, Code = "P1", Name = "P1", Price = 100, IsActive = true };
        promotion!.Products = [product];
        context!.Promotion.Add(promotion);
        context.SaveChanges();

        var result = promotionRepository!.GetActiveForProducts([99], new DateTime(2026, 1, 27)).ToList();

        Assert.AreEqual(0, result.Count);
    }

    [TestMethod]
    public void SetProducts_WhenPromotionExists_UpdatesProducts()
    {
        var product = new Product
        {
            Code = "PROD01",
            Name = "Product 1",
            Price = 100,
            IsActive = true
        };

        context!.Promotion.Add(promotion!);
        context.Products.Add(product);
        context.SaveChanges();

        promotionRepository!.SetProducts(promotion!.Id, [product.Id]);

        var result = context.Promotion
            .Include(p => p.Products)
            .First(p => p.Id == promotion.Id);

        Assert.AreEqual(1, result.Products.Count);
        Assert.AreEqual(product.Id, result.Products[0].Id);
    }

    [TestMethod]
    public void SetProducts_WhenEmptyList_ClearsProducts()
    {
        context!.Promotion.Add(promotion!);
        context.SaveChanges();

        promotionRepository!.SetProducts(promotion!.Id, []);

        var result = context.Promotion
            .Include(p => p.Products)
            .First(p => p.Id == promotion.Id);

        Assert.AreEqual(0, result.Products.Count);
    }
}

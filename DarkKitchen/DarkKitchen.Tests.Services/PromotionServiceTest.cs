using DarkKitchen.Domain.Entities;
using DarkKitchen.Domain.Exceptions;
using DarkKitchen.Domain.Interfaces.Repository;
using DarkKitchen.Domain.Interfaces.Service;
using DarkKitchen.Models.PromotionDTOs;
using DarkKitchen.Services;
using Moq;

namespace DarkKitchen.Tests.Services;

[TestClass]
public class PromotionServiceTest
{
    private Mock<IPromotionRepository>? promotionRepositoryMock;
    private Mock<IAuditService>? auditServiceMock;
    private PromotionService? promotionService;
    private PromotionDto validPromotion;
    private Promotion? promotionEntity;

    [TestInitialize]
    public void Setup()
    {
        promotionRepositoryMock = new Mock<IPromotionRepository>(MockBehavior.Strict);
        auditServiceMock = new Mock<IAuditService>(MockBehavior.Strict);
        promotionService = new PromotionService(promotionRepositoryMock.Object, auditServiceMock.Object);

        validPromotion = new PromotionDto("Black Friday", 10, new DateTime(2026, 1, 25), new DateTime(2026, 1, 30));

        promotionEntity = new Promotion()
        {
            Id = 1,
            Name = "Black Friday",
            DiscountPercentage = 10,
            DateFrom = new DateTime(2026, 1, 25),
            DateTo = new DateTime(2026, 1, 30)
        };
    }

    [TestMethod]
    public void CreatePromotion_ValidData_PromotionCreated()
    {
        promotionRepositoryMock!.Setup(r => r.Add(It.IsAny<Promotion>()));
        auditServiceMock!.Setup(a => a.LogChange(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>()));

        promotionService!.CreatePromotion(validPromotion, "admin@gmail.com");

        promotionRepositoryMock!.Verify(r => r.Add(It.IsAny<Promotion>()), Times.Once);
    }

    [TestMethod]
    public void CreatePromotion_InvalidDiscountPercentage_ThrowsException()
    {
        validPromotion = validPromotion with { discountPercentage = 0 };

        Assert.ThrowsException<BadRequestException>(() => promotionService!.CreatePromotion(validPromotion, "admin@gmail.com"));
    }

    [TestMethod]
    public void CreatePromotion_InvalidDateRange_ThrowsException()
    {
        validPromotion = validPromotion with { dateFrom = new DateTime(2026, 1, 30), dateTo = new DateTime(2026, 1, 25) };

        Assert.ThrowsException<BadRequestException>(() => promotionService!.CreatePromotion(validPromotion, "admin@gmail.com"));
    }

    [TestMethod]
    public void CreatePromotion_EmptyName_ThrowsException()
    {
        validPromotion = validPromotion with { name = string.Empty };

        Assert.ThrowsException<BadRequestException>(() => promotionService!.CreatePromotion(validPromotion, "admin@gmail.com"));
    }

    [TestMethod]
    public void UpdatePromotion_ValidData_PromotionUpdated()
    {
        promotionRepositoryMock!.Setup(r => r.GetById(1)).Returns(promotionEntity!);
        promotionRepositoryMock!.Setup(r => r.Update(It.IsAny<Promotion>()));
        auditServiceMock!.Setup(a => a.LogChange(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>()));

        promotionService!.UpdatePromotion(1, validPromotion, "admin@gmail.com");

        promotionRepositoryMock!.Verify(r => r.Update(It.IsAny<Promotion>()), Times.Once);
    }

    [TestMethod]
    public void UpdatePromotion_PromotionNotFound_ThrowsException()
    {
        promotionRepositoryMock!.Setup(r => r.GetById(1)).Returns((Promotion?)null);

        Assert.ThrowsException<NotFoundException>(() => promotionService!.UpdatePromotion(1, validPromotion, "admin@gmail.com"));
    }

    [TestMethod]
    public void UpdatePromotionProducts_ValidData_ProductsUpdated()
    {
        promotionRepositoryMock!.Setup(r => r.GetById(1)).Returns(promotionEntity!);
        promotionRepositoryMock!.Setup(r => r.SetProducts(1, It.IsAny<List<int>>()));

        promotionService!.UpdatePromotionProducts(1, [1, 2, 3]);

        promotionRepositoryMock!.Verify(r => r.SetProducts(1, It.IsAny<List<int>>()), Times.Once);
    }

    [TestMethod]
    public void UpdatePromotionProducts_PromotionNotFound_ThrowsException()
    {
        promotionRepositoryMock!.Setup(r => r.GetById(1)).Returns((Promotion?)null);

        Assert.ThrowsException<NotFoundException>(() => promotionService!.UpdatePromotionProducts(1, [1, 2, 3]));
    }

    [TestMethod]
    public void GetPromotions_NoPromotionsFound_ReturnsEmptyList()
    {
        var date = new DateTime(2026, 1, 27);
        promotionRepositoryMock!.Setup(r => r.GetPromotions(date, null, null)).Returns([]);

        var result = promotionService!.GetPromotions(new PromotionFiltersDto { Date = date });

        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.Count);
    }

    [TestMethod]
    public void GetPromotions_ValidFilter_ReturnsPromotions()
    {
        var date = new DateTime(2026, 1, 27);
        promotionRepositoryMock!.Setup(r => r.GetPromotions(date, null, null)).Returns([promotionEntity!]);

        var result = promotionService!.GetPromotions(new PromotionFiltersDto { Date = date });

        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Count);
    }

    [TestMethod]
    public void GetBestDiscountByProduct_WhenNoActivePromotions_ReturnsEmptyDictionary()
    {
        promotionRepositoryMock!.Setup(r => r.GetActiveForProducts(It.IsAny<IEnumerable<int>>(), It.IsAny<DateTime>())).Returns([]);

        var result = promotionService!.GetBestDiscountByProduct([1, 2], DateTime.UtcNow);

        Assert.AreEqual(0, result.Count);
    }

    [TestMethod]
    public void GetBestDiscountByProduct_WhenSinglePromotion_ReturnsThatDiscount()
    {
        promotionEntity!.DiscountPercentage = 15;
        promotionEntity.Products = [new Product { Id = 1 }];
        promotionRepositoryMock!.Setup(r => r.GetActiveForProducts(It.IsAny<IEnumerable<int>>(), It.IsAny<DateTime>())).Returns([promotionEntity]);

        var result = promotionService!.GetBestDiscountByProduct([1], DateTime.UtcNow);

        Assert.AreEqual(15, result[1]);
    }

    [TestMethod]
    public void GetBestDiscountByProduct_WhenMultiplePromotionsForSameProduct_ReturnsHighest()
    {
        var lowPromo = new Promotion { Id = 1, Name = "Low", DiscountPercentage = 10, Products = [new Product { Id = 1 }] };
        var highPromo = new Promotion { Id = 2, Name = "High", DiscountPercentage = 25, Products = [new Product { Id = 1 }] };
        promotionRepositoryMock!.Setup(r => r.GetActiveForProducts(It.IsAny<IEnumerable<int>>(), It.IsAny<DateTime>())).Returns([lowPromo, highPromo]);

        var result = promotionService!.GetBestDiscountByProduct([1], DateTime.UtcNow);

        Assert.AreEqual(25, result[1]);
    }

    [TestMethod]
    public void GetBestDiscountByProduct_WhenPromotionCoversUnrequestedProduct_ItIsExcluded()
    {
        promotionEntity!.DiscountPercentage = 20;
        promotionEntity.Products = [new Product { Id = 1 }, new Product { Id = 2 }];
        promotionRepositoryMock!.Setup(r => r.GetActiveForProducts(It.IsAny<IEnumerable<int>>(), It.IsAny<DateTime>())).Returns([promotionEntity]);

        var result = promotionService!.GetBestDiscountByProduct([1], DateTime.UtcNow);

        Assert.IsTrue(result.ContainsKey(1));
        Assert.IsFalse(result.ContainsKey(2));
    }

    [TestMethod]
    public void GetPromotions_WhenDateIsNull_ReturnsAllPromotions()
    {
        promotionRepositoryMock!.Setup(r => r.GetPromotions(null, null, null)).Returns([promotionEntity!]);

        var result = promotionService!.GetPromotions(new PromotionFiltersDto());

        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Count);
    }

    [TestMethod]
    public void CreatePromotion_ValidData_AuditLogCreated()
    {
        promotionRepositoryMock!.Setup(r => r.Add(It.IsAny<Promotion>()));
        auditServiceMock!.Setup(a => a.LogChange("Promotion", It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>()));

        promotionService!.CreatePromotion(validPromotion, "admin@gmail.com");

        auditServiceMock!.Verify(a => a.LogChange("Promotion", It.IsAny<int>(), It.IsAny<string>(), "admin@gmail.com"), Times.Once);
    }

    [TestMethod]
    public void UpdatePromotion_ValidData_AuditLogCreated()
    {
        promotionRepositoryMock!.Setup(r => r.GetById(1)).Returns(promotionEntity!);
        promotionRepositoryMock!.Setup(r => r.Update(It.IsAny<Promotion>()));
        auditServiceMock!.Setup(a => a.LogChange("Promotion", It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>()));

        promotionService!.UpdatePromotion(1, validPromotion, "admin@gmail.com");

        auditServiceMock!.Verify(a => a.LogChange("Promotion", It.IsAny<int>(), It.IsAny<string>(), "admin@gmail.com"), Times.Once);
    }
}

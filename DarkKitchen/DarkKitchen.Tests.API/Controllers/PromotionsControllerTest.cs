using System.Security.Claims;
using DarkKitchen.API.Controllers;
using DarkKitchen.Domain.Exceptions;
using DarkKitchen.Domain.Interfaces.Service;
using DarkKitchen.Models.PromotionDTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace DarkKitchen.Tests.API.Controllers;

[TestClass]
public class PromotionsControllerTest
{
    private readonly int promotionId = 1;
    private Mock<IPromotionService>? promotionServiceMock;
    private PromotionsController? promotionsController;
    private PromotionDto validPromotion;
    private PromotionResponseDto promotionResponse;
    private List<PromotionResponseDto>? promotions;

    [TestInitialize]
    public void Setup()
    {
        promotionServiceMock = new Mock<IPromotionService>(MockBehavior.Strict);
        promotionsController = new PromotionsController(promotionServiceMock.Object);

        var user = new ClaimsPrincipal(new ClaimsIdentity(
        [
            new Claim(ClaimTypes.Name, "admin@gmail.com")
        ], "mock"));

        promotionsController.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };

        validPromotion = new PromotionDto("Black Friday", 10, new DateTime(2026, 1, 25), new DateTime(2026, 1, 30));
        promotionResponse = new PromotionResponseDto(promotionId, "Black Friday", 10, new DateTime(2026, 1, 25), new DateTime(2026, 1, 30), []);
        promotions = [promotionResponse];
    }

    [TestMethod]
    public void CreatePromotion_ValidData_ReturnsCreated()
    {
        promotionServiceMock!.Setup(s => s.CreatePromotion(validPromotion, "admin@gmail.com"));

        var result = promotionsController!.CreatePromotion(validPromotion);
        var resultObj = result as ObjectResult;

        Assert.IsNotNull(resultObj);
        Assert.AreEqual(201, resultObj.StatusCode);
    }

    [TestMethod]
    public void CreatePromotion_AlreadyExists_ThrowsBadRequestException()
    {
        promotionServiceMock!.Setup(s => s.CreatePromotion(validPromotion, "admin@gmail.com")).Throws(new BadRequestException("Promotion already exists."));

        Assert.ThrowsException<BadRequestException>(() => promotionsController!.CreatePromotion(validPromotion));
    }

    [TestMethod]
    public void UpdatePromotion_ExistingPromotion_ReturnsOk()
    {
        promotionServiceMock!.Setup(s => s.UpdatePromotion(promotionId, validPromotion, "admin@gmail.com"));

        var result = promotionsController!.UpdatePromotion(promotionId, validPromotion);
        var resultObj = result as ObjectResult;

        Assert.IsNotNull(resultObj);
        Assert.AreEqual(200, resultObj.StatusCode);
    }

    [TestMethod]
    public void UpdatePromotion_PromotionNotFound_ThrowsNotFoundException()
    {
        promotionServiceMock!.Setup(s => s.UpdatePromotion(promotionId, validPromotion, "admin@gmail.com")).Throws(new NotFoundException("Promotion not found."));

        Assert.ThrowsException<NotFoundException>(() => promotionsController!.UpdatePromotion(promotionId, validPromotion));
    }

    [TestMethod]
    public void UpdatePromotionProducts_ValidData_ReturnsOk()
    {
        var dto = new UpdatePromotionProductsDto([1, 2, 3]);
        promotionServiceMock!.Setup(s => s.UpdatePromotionProducts(1, dto.products, "admin@gmail.com"));

        var result = promotionsController!.UpdatePromotionProducts(1, dto);
        var resultObj = result as ObjectResult;

        Assert.IsNotNull(resultObj);
        Assert.AreEqual(200, resultObj.StatusCode);
    }

    [TestMethod]
    public void UpdatePromotionProducts_PromotionNotFound_ThrowsNotFoundException()
    {
        var dto = new UpdatePromotionProductsDto([1, 2, 3]);
        promotionServiceMock!.Setup(s => s.UpdatePromotionProducts(1, dto.products, "admin@gmail.com")).Throws(new NotFoundException("Promotion not found."));

        Assert.ThrowsException<NotFoundException>(() => promotionsController!.UpdatePromotionProducts(1, dto));
    }

    [TestMethod]
    public void GetPromotions_ValidFilter_ReturnsOk()
    {
        promotionServiceMock!.Setup(s => s.GetPromotions(It.IsAny<PromotionFiltersDto>())).Returns(promotions!);

        var result = promotionsController!.GetPromotions(new PromotionFiltersDto());
        var resultObj = result as ObjectResult;

        Assert.IsNotNull(resultObj);
        Assert.AreEqual(200, resultObj.StatusCode);
    }

    [TestMethod]
    public void GetPromotions_NoPromotionsFound_ReturnsNotFound()
    {
        promotionServiceMock!.Setup(s => s.GetPromotions(It.IsAny<PromotionFiltersDto>())).Throws(new NotFoundException("No promotions found."));

        Assert.ThrowsException<NotFoundException>(() => promotionsController!.GetPromotions(new PromotionFiltersDto()));
    }
}

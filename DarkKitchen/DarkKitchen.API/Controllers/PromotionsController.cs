using DarkKitchen.Domain.Enums;
using DarkKitchen.Domain.Interfaces.Service;
using DarkKitchen.Models.PromotionDTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DarkKitchen.API.Controllers;

[ApiController]
[Route("api/promotions")]
public class PromotionsController(IPromotionService promotionService) : ControllerBase
{
    private readonly IPromotionService _promotionService = promotionService;

    [Authorize(Policy = nameof(Permission.CreatePromotion))]
    [HttpPost]
    public IActionResult CreatePromotion([FromBody] PromotionDto newPromotion)
    {
        var responsibleUser = User.Identity!.Name!;
        _promotionService.CreatePromotion(newPromotion, responsibleUser);
        return Created(string.Empty, newPromotion);
    }

    [Authorize(Policy = nameof(Permission.UpdatePromotion))]
    [HttpPut("{id}")]
    public IActionResult UpdatePromotion(int id, [FromBody] PromotionDto updatedPromotion)
    {
        var responsibleUser = User.Identity!.Name!;
        _promotionService.UpdatePromotion(id, updatedPromotion, responsibleUser);
        return Ok("Promotion updated correctly.");
    }

    [Authorize(Policy = nameof(Permission.UpdatePromotionProducts))]
    [HttpPut("{id}/products")]
    public IActionResult UpdatePromotionProducts(int id, [FromBody] UpdatePromotionProductsDto dto)
    {
        var responsibleUser = User.Identity!.Name!;
        _promotionService.UpdatePromotionProducts(id, dto.products, responsibleUser);
        return Ok("Promotion products updated correctly.");
    }

    [Authorize(Policy = nameof(Permission.GetCurrentPromotions))]
    [HttpGet]
    public IActionResult GetPromotions([FromQuery] PromotionFiltersDto filter)
    {
        var promotions = _promotionService.GetPromotions(filter);
        return Ok(promotions);
    }
}

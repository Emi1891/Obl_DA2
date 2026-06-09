using DarkKitchen.Domain.Entities;
using DarkKitchen.Domain.Exceptions;
using DarkKitchen.Domain.Interfaces.Repository;
using DarkKitchen.Domain.Interfaces.Service;
using DarkKitchen.Models.PromotionDTOs;

namespace DarkKitchen.Services;

public class PromotionService(IPromotionRepository promotionRepository, IAuditService auditService) : IPromotionService
{
    private readonly IPromotionRepository _promotionRepository = promotionRepository;
    private readonly IAuditService _auditService = auditService;

    public void CreatePromotion(PromotionDto newPromotion, string responsibleUser)
    {
        if(string.IsNullOrEmpty(newPromotion.name))
        {
            throw new BadRequestException("Name cannot be empty.");
        }

        if(newPromotion.discountPercentage <= 0 || newPromotion.discountPercentage > 100)
        {
            throw new BadRequestException("Discount percentage must be between 1 and 100.");
        }

        if(newPromotion.dateFrom >= newPromotion.dateTo)
        {
            throw new BadRequestException("DateFrom must be before DateTo.");
        }

        var promotion = new Promotion()
        {
            Name = newPromotion.name,
            DiscountPercentage = newPromotion.discountPercentage,
            DateFrom = newPromotion.dateFrom,
            DateTo = newPromotion.dateTo
        };

        _promotionRepository.Add(promotion);
        _auditService.LogChange("Promotion", promotion.Id, "Promotion created", responsibleUser);
    }

    public void UpdatePromotion(int id, PromotionDto updatedPromotion, string responsibleUser)
    {
        var promotion = _promotionRepository.GetById(id)
            ?? throw new NotFoundException("Promotion not found.");

        promotion.Name = updatedPromotion.name;
        promotion.DiscountPercentage = updatedPromotion.discountPercentage;
        promotion.DateFrom = updatedPromotion.dateFrom;
        promotion.DateTo = updatedPromotion.dateTo;

        _promotionRepository.Update(promotion);
        _auditService.LogChange("Promotion", promotion.Id, "Promotion updated", responsibleUser);
    }

    public void UpdatePromotionProducts(int promotionId, List<int> productIds)
    {
        var promotion = _promotionRepository.GetById(promotionId);
        if(promotion == null)
        {
            throw new NotFoundException("Promotion not found.");
        }

        _promotionRepository.SetProducts(promotionId, productIds);
    }

    public List<PromotionResponseDto> GetPromotions(PromotionFiltersDto filter)
    {
        var promotions = _promotionRepository.GetPromotions(filter.Date, filter.ProductLine, filter.ProductName);

        return promotions.Select(p => new PromotionResponseDto(p.Id, p.Name, p.DiscountPercentage, p.DateFrom, p.DateTo, p.Products.Select(x => x.Code).ToList())).ToList();
    }

    public Dictionary<int, int> GetBestDiscountByProduct(IEnumerable<int> productIds, DateTime date)
    {
        var requested = productIds.ToHashSet();
        var promotions = _promotionRepository.GetActiveForProducts(requested, date);
        var best = new Dictionary<int, int>();

        foreach(var promo in promotions)
        {
            foreach(var product in promo.Products.Where(p => requested.Contains(p.Id)))
            {
                if(!best.TryGetValue(product.Id, out var current) || promo.DiscountPercentage > current)
                {
                    best[product.Id] = promo.DiscountPercentage;
                }
            }
        }

        return best;
    }
}

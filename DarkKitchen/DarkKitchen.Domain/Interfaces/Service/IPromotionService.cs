using DarkKitchen.Models.PromotionDTOs;

namespace DarkKitchen.Domain.Interfaces.Service;

public interface IPromotionService
{
    public void CreatePromotion(PromotionDto newPromotion, string responsibleUser);
    public void UpdatePromotion(int id, PromotionDto updatedPromotion, string responsibleUser);
    public void UpdatePromotionProducts(int promotionId, List<int> productIds, string responsibleUser);
    public List<PromotionResponseDto> GetPromotions(PromotionFiltersDto filter);
    public Dictionary<int, int> GetBestDiscountByProduct(IEnumerable<int> productIds, DateTime date);
}

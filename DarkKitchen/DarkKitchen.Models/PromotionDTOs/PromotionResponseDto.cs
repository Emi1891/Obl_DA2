namespace DarkKitchen.Models.PromotionDTOs;

public readonly record struct PromotionResponseDto(int id, string name, int discountPercentage, DateTime dateFrom, DateTime dateTo, List<string> productCodes);

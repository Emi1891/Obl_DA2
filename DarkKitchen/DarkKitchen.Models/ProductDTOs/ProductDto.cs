namespace DarkKitchen.Models.ProductDTOs;

public readonly record struct ProductDto(int? id, string? code, string? name, string? description, string? productLine, string? category, decimal? price, string[]? imageUrl, bool? isActive, int? unitsSold, int? discountPercentage = null);

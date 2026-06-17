namespace DarkKitchen.Models.ProductDTOs;

public readonly record struct ImportedProductDto(
    string code,
    string name,
    decimal price,
    string? description,
    string? productLine,
    string? category,
    IReadOnlyList<string> imagePaths);

using DarkKitchen.Models.ProductDTOs;

namespace DarkKitchen.Domain.Interfaces.Service;

public interface IProductImporter
{
    string Name { get; }
    string Extension { get; }
    IEnumerable<ImportedProductDto> Import(Stream source);
}

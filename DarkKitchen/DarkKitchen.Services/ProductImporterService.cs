using DarkKitchen.Domain.Entities;
using DarkKitchen.Domain.Exceptions;
using DarkKitchen.Domain.Interfaces.Repository;
using DarkKitchen.Domain.Interfaces.Service;
using DarkKitchen.Models.ProductDTOs;

namespace DarkKitchen.Services;

public class ProductImporterService(IImporterRegistry registry, IProductRepository productRepository) : IProductImporterService
{
    private readonly IImporterRegistry _registry = registry;
    private readonly IProductRepository _productRepository = productRepository;

    public IEnumerable<ImporterInfoDto> GetAvailableImporters()
    {
        _registry.Refresh();
        return _registry.GetAll().Select(i => new ImporterInfoDto(i.Name, i.Extension));
    }

    public void UploadImporter(string fileName, Stream content)
    {
        if(string.IsNullOrWhiteSpace(fileName)
            || !fileName.EndsWith(".dll", StringComparison.OrdinalIgnoreCase))
        {
            throw new BadRequestException("Importer file must be a .dll.");
        }

        _registry.InstallImporter(fileName, content);
    }

    public int ImportProducts(string importerName, Stream source)
    {
        if(string.IsNullOrWhiteSpace(importerName))
        {
            throw new BadRequestException("Importer name is required.");
        }

        _registry.Refresh();
        var importer = _registry.Get(importerName);
        var items = importer.Import(source).ToList();

        if(items.Count == 0)
        {
            throw new BadRequestException("No products found in the source.");
        }

        ValidateBatch(items);

        foreach(var item in items)
        {
            _productRepository.Add(ToEntity(item));
        }

        return items.Count;
    }

    private void ValidateBatch(List<ImportedProductDto> items)
    {
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var duplicatesInBatch = new List<string>();
        var missingFields = new List<int>();
        var conflictsInDb = new List<string>();

        for(var i = 0; i < items.Count; i++)
        {
            var item = items[i];

            if(string.IsNullOrWhiteSpace(item.code) || string.IsNullOrWhiteSpace(item.name))
            {
                missingFields.Add(i);
                continue;
            }

            if(!seen.Add(item.code))
            {
                duplicatesInBatch.Add(item.code);
            }
            else if(_productRepository.GetByCode(item.code) != null)
            {
                conflictsInDb.Add(item.code);
            }
        }

        if(missingFields.Count > 0)
        {
            throw new BadRequestException(
                $"Products at indexes [{string.Join(", ", missingFields)}] are missing code or name.");
        }

        if(duplicatesInBatch.Count > 0)
        {
            throw new BadRequestException(
                $"Duplicate codes in source: {string.Join(", ", duplicatesInBatch.Distinct())}.");
        }

        if(conflictsInDb.Count > 0)
        {
            throw new BadRequestException(
                $"Codes already exist: {string.Join(", ", conflictsInDb)}.");
        }
    }

    private static Product ToEntity(ImportedProductDto item) => new()
    {
        Code = item.code,
        Name = item.name,
        Description = item.description,
        ProductLine = item.productLine,
        Category = item.category,
        Price = item.price,
        IsActive = true,
        Images = item.imagePaths?.Select(url => new ProductImage { Url = url }).ToList() ?? [],
    };
}

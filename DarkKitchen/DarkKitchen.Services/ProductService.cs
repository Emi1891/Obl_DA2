using DarkKitchen.Domain.Entities;
using DarkKitchen.Domain.Exceptions;
using DarkKitchen.Domain.Interfaces.Repository;
using DarkKitchen.Domain.Interfaces.Service;
using DarkKitchen.Models.DateDTOs;
using DarkKitchen.Models.ProductDTOs;

namespace DarkKitchen.Services;

public class ProductService(IProductRepository repository, IPromotionService promotionService, IAuditService auditService) : IProductService
{
    private readonly IProductRepository _repository = repository;
    private readonly IPromotionService _promotionService = promotionService;
    private readonly IAuditService _auditService = auditService;

    public void CreateProduct(CreateProductDto newProduct, string responsibleUser)
    {
        if(newProduct.name == null || newProduct.name == string.Empty)
        {
            throw new BadRequestException("Name is required.");
        }

        if(_repository.GetByCode(newProduct.code!) != null)
        {
            throw new BadRequestException("Product code already in use.");
        }

        var product = new Product
        {
            Code = newProduct.code!,
            Name = newProduct.name!,
            Description = newProduct.description,
            ProductLine = newProduct.productLine,
            Category = newProduct.category,
            Price = newProduct.price!.Value,
            IsActive = true,
            Images = newProduct.imageUrl!.Select(url => new ProductImage { Url = url }).ToList()
        };
        _repository.Add(product);
        _auditService.LogChange("Product", product.Id, "Product created", responsibleUser);
    }

    public void UpdateProduct(ProductDto updatedProduct, string responsibleUser)
    {
        Product product = _repository.GetById(updatedProduct.id ?? 0)
            ?? throw new NotFoundException("Product not found.");

        if(updatedProduct.name != null)
        {
            product.Name = updatedProduct.name;
        }

        if(updatedProduct.description != null)
        {
            product.Description = updatedProduct.description;
        }

        if(updatedProduct.productLine != null)
        {
            product.ProductLine = updatedProduct.productLine;
        }

        if(updatedProduct.category != null)
        {
            product.Category = updatedProduct.category;
        }

        if(updatedProduct.price.HasValue)
        {
            product.Price = updatedProduct.price.Value;
        }

        if(updatedProduct.isActive.HasValue)
        {
            product.IsActive = updatedProduct.isActive.Value;
        }

        if(updatedProduct.imageUrl != null)
        {
            product.Images = updatedProduct.imageUrl.Select(url => new ProductImage { Url = url, ProductId = product.Id }).ToList();
        }

        _repository.Update(product);
        _auditService.LogChange("Product", product.Id, "Product updated", responsibleUser);
    }

    public ProductDto? GetByCode(string code)
    {
        var product = _repository.GetByCode(code);
        if(product == null)
        {
            return null;
        }

        return new ProductDto(product.Id, product.Code, product.Name, product.Description, product.ProductLine, product.Category, product.Price, product.Images?.Select(i => i.Url).ToArray(), product.IsActive, product.UnitsSold);
    }

    public IEnumerable<ProductDto> GetProducts(ProductFilterDto filter)
    {
        List<Product> products = _repository.GetProducts(filter).ToList();

        if(products.Count == 0)
        {
            throw new NotFoundException("No products found.");
        }

        var discounts = _promotionService.GetBestDiscountByProduct(products.Select(p => p.Id), DateTime.UtcNow.Date);

        return products.Select(p => new ProductDto(p.Id, p.Code, p.Name, p.Description, p.ProductLine, p.Category, p.Price, p.Images?.Select(i => i.Url).ToArray(), p.IsActive, p.UnitsSold, discounts.TryGetValue(p.Id, out var discount) ? discount : null));
    }

    public IEnumerable<ProductDto> GetMostRequestedProducts(DateRangeDto dates)
    {
        IEnumerable<Product> products = _repository.GetMostRequestedProducts(dates);

        if(!products.Any())
        {
            throw new NotFoundException("No products found.");
        }

        return products.Select(p => new ProductDto(p.Id, p.Code, p.Name, p.Description, p.ProductLine, p.Category, p.Price, p.Images?.Select(i => i.Url).ToArray(), p.IsActive, p.UnitsSold));
    }

    public void UpdateStatus(int id, ProductStatusDto status, string responsibleUser)
    {
        Product product = _repository.GetById(id)
            ?? throw new NotFoundException("Product not found.");

        product.IsActive = status.isActive;
        _repository.Update(product);
        _auditService.LogChange("Product", product.Id, "Product status updated", responsibleUser);
    }

    public void RegisterSale(int productId, int quantity)
    {
        Product product = _repository.GetById(productId)
            ?? throw new NotFoundException("Product not found.");

        product.UnitsSold += quantity;
        _repository.Update(product);
    }

    public Dictionary<int, int> GetBestDiscountByProduct(IEnumerable<int> productIds, DateTime date)
    {
        return _promotionService.GetBestDiscountByProduct(productIds, date);
    }
}

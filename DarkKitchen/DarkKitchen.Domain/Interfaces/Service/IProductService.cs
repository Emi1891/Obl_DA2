using DarkKitchen.Models.DateDTOs;
using DarkKitchen.Models.ProductDTOs;

namespace DarkKitchen.Domain.Interfaces.Service;

public interface IProductService
{
    public void CreateProduct(CreateProductDto newProduct, string responsibleUser);
    public void UpdateProduct(ProductDto updatedProduct, string responsibleUser);
    public IEnumerable<ProductDto> GetProducts(ProductFilterDto filter);
    public ProductDto? GetByCode(string code);
    public IEnumerable<ProductDto> GetMostRequestedProducts(DateRangeDto dates);
    public void UpdateStatus(int id, ProductStatusDto status, string responsibleUser);
    public void RegisterSale(int productId, int quantity);
    public Dictionary<int, int> GetBestDiscountByProduct(IEnumerable<int> productIds, DateTime date);
}

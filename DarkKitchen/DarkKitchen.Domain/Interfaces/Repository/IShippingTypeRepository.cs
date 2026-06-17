using DarkKitchen.Domain.Entities;

namespace DarkKitchen.Domain.Interfaces.Repository;

public interface IShippingTypeRepository
{
    IEnumerable<ShippingType> GetAll();
    ShippingType? GetById(int id);
    void Add(ShippingType shippingType);
    void Update(ShippingType shippingType);
}

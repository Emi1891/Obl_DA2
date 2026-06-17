using DarkKitchen.Models.ShippingTypeDTOs;

namespace DarkKitchen.Domain.Interfaces.Service;

public interface IShippingTypeService
{
    List<ShippingTypeResponseDto> GetAll();
    ShippingTypeResponseDto GetById(int id);
    ShippingTypeResponseDto Create(ShippingTypeDto dto);
    ShippingTypeResponseDto Update(int id, ShippingTypeDto dto);
}

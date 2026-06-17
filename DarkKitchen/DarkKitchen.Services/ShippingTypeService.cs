using DarkKitchen.Domain.Entities;
using DarkKitchen.Domain.Exceptions;
using DarkKitchen.Domain.Interfaces.Repository;
using DarkKitchen.Domain.Interfaces.Service;
using DarkKitchen.Models.ShippingTypeDTOs;

namespace DarkKitchen.Services;

public class ShippingTypeService(IShippingTypeRepository shippingTypeRepository) : IShippingTypeService
{
    private readonly IShippingTypeRepository _shippingTypeRepository = shippingTypeRepository;

    public List<ShippingTypeResponseDto> GetAll()
    {
        return _shippingTypeRepository.GetAll()
            .Select(s => new ShippingTypeResponseDto(s.Id, s.Name, s.Price))
            .ToList();
    }

    public ShippingTypeResponseDto GetById(int id)
    {
        var shippingType = _shippingTypeRepository.GetById(id)
            ?? throw new NotFoundException("Shipping type not found.");

        return new ShippingTypeResponseDto(shippingType.Id, shippingType.Name, shippingType.Price);
    }

    public ShippingTypeResponseDto Create(ShippingTypeDto dto)
    {
        if(string.IsNullOrEmpty(dto.name))
        {
            throw new BadRequestException("Shipping type name cannot be empty.");
        }

        if(dto.price <= 0)
        {
            throw new BadRequestException("Shipping type price must be greater than zero.");
        }

        var shippingType = new ShippingType
        {
            Name = dto.name,
            Price = dto.price
        };

        _shippingTypeRepository.Add(shippingType);

        return new ShippingTypeResponseDto(shippingType.Id, shippingType.Name, shippingType.Price);
    }

    public ShippingTypeResponseDto Update(int id, ShippingTypeDto dto)
    {
        var existing = _shippingTypeRepository.GetById(id)
            ?? throw new NotFoundException("Shipping type not found.");

        if(string.IsNullOrEmpty(dto.name))
        {
            throw new BadRequestException("Shipping type name cannot be empty.");
        }

        if(dto.price <= 0)
        {
            throw new BadRequestException("Shipping type price must be greater than zero.");
        }

        existing.Name = dto.name;
        existing.Price = dto.price;

        _shippingTypeRepository.Update(existing);

        return new ShippingTypeResponseDto(existing.Id, existing.Name, existing.Price);
    }
}

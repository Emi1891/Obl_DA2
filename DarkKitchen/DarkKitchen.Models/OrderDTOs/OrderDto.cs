namespace DarkKitchen.Models.OrderDTOs;

public readonly record struct OrderDto(int shippingTypeId, AddressDto address, List<OrderProductDto> products);

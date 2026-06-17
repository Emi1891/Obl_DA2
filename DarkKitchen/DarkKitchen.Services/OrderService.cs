using DarkKitchen.Domain.Entities;
using DarkKitchen.Domain.Enums;
using DarkKitchen.Domain.Exceptions;
using DarkKitchen.Domain.Interfaces.Repository;
using DarkKitchen.Domain.Interfaces.Service;
using DarkKitchen.Models.OrderDTOs;

namespace DarkKitchen.Services;

public class OrderService(IOrderRepository orderRepository, IUserService userService, IProductService productService, IShippingTypeRepository shippingTypeRepository) : IOrderService
{
    private readonly IOrderRepository orderRepository = orderRepository;
    private readonly IUserService userService = userService;
    private readonly IProductService productService = productService;
    private readonly IShippingTypeRepository shippingTypeRepository = shippingTypeRepository;
    private readonly decimal ivaRate = 0.22m;

    public OrderResponseDto CreateOrder(OrderDto newOrder, int clientId)
    {
        if(newOrder.products.Count == 0)
        {
            throw new BadRequestException("An order must have at least one product.");
        }

        var shippingType = shippingTypeRepository.GetById(newOrder.shippingTypeId)
            ?? throw new NotFoundException("Shipping type not found.");

        if(string.IsNullOrEmpty(newOrder.address.street))
        {
            throw new BadRequestException("Street cannot be empty.");
        }

        if(string.IsNullOrEmpty(newOrder.address.doorNumber))
        {
            throw new BadRequestException("Door number cannot be empty.");
        }

        var orderProducts = new List<OrderProduct>();
        foreach(var item in newOrder.products)
        {
            var product = productService.GetByCode(item.productCode)
                ?? throw new NotFoundException($"Product {item.productCode} not found.");

            orderProducts.Add(new OrderProduct
            {
                ProductId = product.id!.Value,
                Quantity = item.quantity,
                UnitPrice = product.price!.Value
            });
        }

        var discountByProduct = productService.GetBestDiscountByProduct(orderProducts.Select(op => op.ProductId), DateTime.UtcNow.Date);

        decimal subtotal = 0;
        decimal discount = 0;
        foreach(var line in orderProducts)
        {
            var lineSubtotal = line.UnitPrice * line.Quantity;
            subtotal += lineSubtotal;

            if(discountByProduct.TryGetValue(line.ProductId, out var percentage))
            {
                line.DiscountPercentage = percentage;
                discount += lineSubtotal * percentage / 100;
            }
        }

        var iva = (subtotal - discount) * ivaRate;
        var shippingCost = shippingType.Price;

        var order = new Order
        {
            ClientId = clientId,
            ShippingTypeId = newOrder.shippingTypeId,
            Address = new Address
            {
                Street = newOrder.address.street,
                DoorNumber = newOrder.address.doorNumber,
                Apartment = newOrder.address.apartment,
            },
            Subtotal = subtotal,
            Discount = discount,
            Iva = iva,
            ShippingCost = shippingCost,
            Total = subtotal - discount + iva + shippingCost,
            Products = orderProducts
        };

        orderRepository.Add(order);

        foreach(var line in orderProducts)
        {
            productService.RegisterSale(line.ProductId, line.Quantity);
        }

        return new OrderResponseDto(order.Id, order.ClientId, order.Status, order.CreatedAt, order.Subtotal, order.Discount, order.Iva, order.ShippingCost, order.Total, []);
    }

    public List<OrderResponseDto> GetClientOrders(int clientId, OrderFiltersDto filter)
    {
        var orders = orderRepository.GetClientOrders(clientId, filter.DateFrom, filter.DateTo, filter.Status);

        return orders.Select(o => new OrderResponseDto(o.Id, o.ClientId, o.Status, o.CreatedAt, o.Subtotal, o.Discount, o.Iva, o.ShippingCost, o.Total, [])).ToList();
    }

    public List<OrderResponseDto> GetOrdersByStatus(OrderFilterByStatusDto filter)
    {
        var orders = orderRepository.GetOrdersByStatus(filter.DateFrom, filter.DateTo, filter.Address, filter.Status);

        return orders.Select(o => new OrderResponseDto(o.Id, o.ClientId, o.Status, o.CreatedAt, o.Subtotal, o.Discount, o.Iva, o.ShippingCost, o.Total, [])).ToList();
    }

    public OrderResponseDto GetOrderById(int orderId)
    {
        var order = orderRepository.GetById(orderId)
            ?? throw new NotFoundException("Order not found.");

        return new OrderResponseDto(order.Id, order.ClientId, order.Status, order.CreatedAt, order.Subtotal, order.Discount, order.Iva, order.ShippingCost, order.Total, []);
    }

    public UpdateOrderStatusResponseDto UpdateOrderStatus(int orderId, UpdateOrderStatusDto newStatus, List<Permission> userPermissions)
    {
        if(!Enum.TryParse<OrderStatus>(newStatus.status, out var status))
        {
            throw new BadRequestException("Invalid order status.");
        }

        var requiredPermission = status switch
        {
            OrderStatus.Prepared => Permission.SetOrderStatusToPrepared,
            OrderStatus.Canceled => Permission.SetOrderStatusToCanceled,
            OrderStatus.OnItsWay => Permission.SetOrderStatusToOnItsWay,
            OrderStatus.Delivered => Permission.SetOrderStatusToDelivered,
            OrderStatus.NotDelivered => Permission.SetOrderStatusToNotDelivered,
            OrderStatus.Delayed => Permission.SetOrderStatusToDelayed,
            _ => throw new BadRequestException("Invalid order status.")
        };

        if(!userPermissions.Contains(requiredPermission))
        {
            throw new AccessDeniedException("User does not have permission to set this order status.");
        }

        var order = orderRepository.GetById(orderId)
            ?? throw new NotFoundException("Order not found.");

        if(!Enum.TryParse<OrderStatus>(order.Status, out var currentStatus))
        {
            throw new BadRequestException("Invalid current order status.");
        }

        var validTransitions = new Dictionary<OrderStatus, List<OrderStatus>>
        {
            { OrderStatus.Pending, [OrderStatus.Prepared, OrderStatus.Canceled, OrderStatus.Delayed] },
            { OrderStatus.Prepared, [OrderStatus.OnItsWay] },
            { OrderStatus.OnItsWay, [OrderStatus.Delivered, OrderStatus.NotDelivered] },
            { OrderStatus.Canceled, [] },
            { OrderStatus.Delivered, [] },
            { OrderStatus.NotDelivered, [] },
            { OrderStatus.Delayed, [OrderStatus.Prepared, OrderStatus.Canceled] }
        };

        if(!validTransitions[currentStatus].Contains(status))
        {
            throw new BadRequestException($"Cannot change order status from {currentStatus} to {status}.");
        }

        order.Status = status.ToString();
        order.UpdatedAt = DateTime.UtcNow;

        orderRepository.Update(order);

        return new UpdateOrderStatusResponseDto($"Order status updated to: {status}", order.UpdatedAt.Value);
    }

    public SalesReportDto GetSalesReport()
    {
        var orders = orderRepository.GetAll().ToList();

        var aggregated = new Dictionary<(int Year, int Month), Dictionary<string, decimal>>();

        foreach(var order in orders)
        {
            var user = userService.GetUserById(order.ClientId)!.Value;
            var clientName = $"{user.name} {user.surname}";
            var key = (order.CreatedAt.Year, order.CreatedAt.Month);

            if(!aggregated.ContainsKey(key))
            {
                aggregated[key] = [];
            }

            if(!aggregated[key].ContainsKey(clientName))
            {
                aggregated[key][clientName] = 0;
            }

            aggregated[key][clientName] += order.Total;
        }

        var months = new List<MonthlySalesDto>();
        decimal grandTotal = 0;

        foreach(var monthEntry in aggregated.OrderBy(m => m.Key.Year).ThenBy(m => m.Key.Month))
        {
            var lines = new List<ClientSalesLineDto>();
            decimal monthTotal = 0;

            foreach(var clientEntry in monthEntry.Value.OrderBy(c => c.Key))
            {
                lines.Add(new ClientSalesLineDto(clientEntry.Key, clientEntry.Value));
                monthTotal += clientEntry.Value;
            }

            months.Add(new MonthlySalesDto(monthEntry.Key.Year, monthEntry.Key.Month, lines, monthTotal));
            grandTotal += monthTotal;
        }

        return new SalesReportDto(months, grandTotal);
    }
}

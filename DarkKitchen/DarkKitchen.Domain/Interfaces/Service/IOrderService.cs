using DarkKitchen.Domain.Enums;
using DarkKitchen.Models.OrderDTOs;

namespace DarkKitchen.Domain.Interfaces.Service;

public interface IOrderService
{
    public OrderResponseDto CreateOrder(OrderDto newOrder, int clientId);
    public List<OrderResponseDto> GetClientOrders(int clientId, OrderFiltersDto filter);
    public List<OrderResponseDto> GetOrdersByStatus(OrderFilterByStatusDto filter);
    public OrderResponseDto GetOrderById(int orderId);
    public UpdateOrderStatusResponseDto UpdateOrderStatus(int orderId, UpdateOrderStatusDto newStatus, List<Permission> userPermissions);
    public SalesReportDto GetSalesReport();
}

using System.Security.Claims;
using DarkKitchen.Domain.Enums;
using DarkKitchen.Domain.Interfaces.Service;
using DarkKitchen.Models.OrderDTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DarkKitchen.API.Controllers;

[ApiController]
[Route("api/orders")]
public class OrdersController(IOrderService orderService) : ControllerBase
{
    private readonly IOrderService _orderService = orderService;

    [Authorize(Policy = nameof(Permission.PlaceOrder))]
    [HttpPost]
    public IActionResult CreateOrder([FromBody] OrderDto newOrder)
    {
        var clientId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var order = _orderService.CreateOrder(newOrder, clientId);
        return Created(string.Empty, order);
    }

    [Authorize(Policy = nameof(Permission.GetMyOrders))]
    [HttpGet("client")]
    public IActionResult GetClientOrders([FromQuery] OrderFiltersDto filters)
    {
        var clientId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var orders = _orderService.GetClientOrders(clientId, filters);
        return Ok(orders);
    }

    [Authorize(Policy = nameof(Permission.GetOrdersByStatus))]
    [HttpGet]
    public IActionResult GetOrdersByStatus([FromQuery] OrderFilterByStatusDto filters)
    {
        var orders = _orderService.GetOrdersByStatus(filters);
        return Ok(orders);
    }

    [Authorize(Policy = nameof(Permission.GetOrderDetails))]
    [HttpGet("{orderId}")]
    public IActionResult GetOrderById(int orderId)
    {
        var order = _orderService.GetOrderById(orderId);
        return Ok(order);
    }

    [Authorize]
    [HttpPatch("{orderId}/status")]
    public IActionResult UpdateOrderStatus(int orderId, [FromBody] UpdateOrderStatusDto newStatus)
    {
        var permissions = User.FindAll("permission")
            .Select(c => Enum.Parse<Permission>(c.Value))
            .ToList();
        var result = _orderService.UpdateOrderStatus(orderId, newStatus, permissions);
        return Ok(result);
    }

    [Authorize(Policy = nameof(Permission.GetSalesReport))]
    [HttpGet("sales-report")]
    public IActionResult GetSalesReport()
    {
        var report = _orderService.GetSalesReport();
        return Ok(report);
    }
}

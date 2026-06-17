using System.Security.Claims;
using DarkKitchen.API.Controllers;
using DarkKitchen.Domain.Enums;
using DarkKitchen.Domain.Exceptions;
using DarkKitchen.Domain.Interfaces.Service;
using DarkKitchen.Models.OrderDTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace DarkKitchen.Tests.API.Controllers;

[TestClass]
public class OrdersControllerTest
{
    private Mock<IOrderService>? orderServiceMock;
    private OrdersController? ordersController;
    private OrderDto validOrder;
    private OrderResponseDto orderResponse;
    private List<OrderResponseDto>? orders;

    [TestInitialize]
    public void Setup()
    {
        orderServiceMock = new Mock<IOrderService>(MockBehavior.Strict);
        ordersController = new OrdersController(orderServiceMock.Object);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, "1"),
            new Claim("permission", nameof(Permission.SetOrderStatusToPrepared))
        };
        var identity = new ClaimsIdentity(claims);
        var claimsPrincipal = new ClaimsPrincipal(identity);
        ordersController.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = claimsPrincipal }
        };

        validOrder = new OrderDto(1, new AddressDto("18 de Julio", "1234", "101"), [new OrderProductDto("PROD01", 2)]);

        orderResponse = new OrderResponseDto(1, 1, "Pending", DateTime.Now, 100, 0, 22, 10, 132, []);

        orders = [orderResponse];
    }

    [TestMethod]
    public void CreateOrder_ValidData_ReturnsCreated()
    {
        orderServiceMock!.Setup(s => s.CreateOrder(validOrder, 1)).Returns(orderResponse);

        var result = ordersController!.CreateOrder(validOrder);
        var resultObj = result as ObjectResult;

        Assert.IsNotNull(resultObj);
        Assert.AreEqual(201, resultObj.StatusCode);
    }

    [TestMethod]
    public void CreateOrder_NoProducts_ThrowsBadRequestException()
    {
        var emptyOrder = new OrderDto(1, new AddressDto("18 de Julio", "1234", "101"), []);
        orderServiceMock!.Setup(s => s.CreateOrder(emptyOrder, 1)).Throws(new BadRequestException("Order must have at least one product."));

        Assert.ThrowsException<BadRequestException>(() => ordersController!.CreateOrder(emptyOrder));
    }

    [TestMethod]
    public void GetClientOrders_ValidFilter_ReturnsOk()
    {
        orderServiceMock!.Setup(s => s.GetClientOrders(It.IsAny<int>(), It.IsAny<OrderFiltersDto>())).Returns(orders!);
        var filters = new OrderFiltersDto { };

        var result = ordersController!.GetClientOrders(filters);
        var resultObj = result as ObjectResult;

        Assert.IsNotNull(resultObj);
        Assert.AreEqual(200, resultObj.StatusCode);
    }

    [TestMethod]
    public void GetClientOrders_NoOrdersFound_ThrowsNotFoundException()
    {
        orderServiceMock!.Setup(s => s.GetClientOrders(It.IsAny<int>(), It.IsAny<OrderFiltersDto>())).Throws(new NotFoundException("No orders found."));
        var filters = new OrderFiltersDto { };

        Assert.ThrowsException<NotFoundException>(() => ordersController!.GetClientOrders(filters));
    }

    [TestMethod]
    public void GetOrdersByStatus_ValidFilter_ReturnsOk()
    {
        orderServiceMock!.Setup(s => s.GetOrdersByStatus(It.IsAny<OrderFilterByStatusDto>())).Returns(orders!);
        var filters = new OrderFilterByStatusDto
        {
            DateFrom = DateTime.Now.AddDays(-7),
            DateTo = DateTime.Now
        };

        var result = ordersController!.GetOrdersByStatus(filters);
        var resultObj = result as ObjectResult;

        Assert.IsNotNull(resultObj);
        Assert.AreEqual(200, resultObj.StatusCode);
    }

    [TestMethod]
    public void GetOrdersByStatus_NoOrdersFound_ThrowsNotFoundException()
    {
        orderServiceMock!.Setup(s => s.GetOrdersByStatus(It.IsAny<OrderFilterByStatusDto>())).Throws(new NotFoundException("No orders found."));
        var filters = new OrderFilterByStatusDto
        {
            DateFrom = DateTime.Now.AddDays(-7),
            DateTo = DateTime.Now
        };

        Assert.ThrowsException<NotFoundException>(() => ordersController!.GetOrdersByStatus(filters));
    }

    [TestMethod]
    public void GetOrderById_ValidId_ReturnsOk()
    {
        orderServiceMock!.Setup(s => s.GetOrderById(1)).Returns(orderResponse);
        var result = ordersController!.GetOrderById(1);
        var resultObj = result as ObjectResult;

        Assert.IsNotNull(resultObj);
        Assert.AreEqual(200, resultObj.StatusCode);
    }

    [TestMethod]
    public void GetOrderById_OrderNotFound_ThrowsNotFoundException()
    {
        orderServiceMock!.Setup(s => s.GetOrderById(1)).Throws(new NotFoundException("Order not found."));

        Assert.ThrowsException<NotFoundException>(() => ordersController!.GetOrderById(1));
    }

    [TestMethod]
    public void UpdateOrderStatus_ValidStatus_ReturnsOk()
    {
        var newStatus = new UpdateOrderStatusDto(nameof(OrderStatus.Prepared));
        orderServiceMock!.Setup(s => s.UpdateOrderStatus(1, newStatus, It.IsAny<List<Permission>>()))
            .Returns(new UpdateOrderStatusResponseDto("Order status updated to: Prepared", DateTime.UtcNow));
        var result = ordersController!.UpdateOrderStatus(1, newStatus);
        var resultObj = result as ObjectResult;

        Assert.IsNotNull(resultObj);
        Assert.AreEqual(200, resultObj.StatusCode);
    }

    [TestMethod]
    public void UpdateOrderStatus_InvalidStatus_ThrowsBadRequestException()
    {
        var newStatus = new UpdateOrderStatusDto("InvalidStatus");
        orderServiceMock!.Setup(s => s.UpdateOrderStatus(1, newStatus, It.IsAny<List<Permission>>())).Throws(new BadRequestException("Invalid order status."));

        Assert.ThrowsException<BadRequestException>(() => ordersController!.UpdateOrderStatus(1, newStatus));
    }

    [TestMethod]
    public void UpdateOrderStatus_OrderNotFound_ThrowsNotFoundException()
    {
        var newStatus = new UpdateOrderStatusDto(nameof(OrderStatus.Prepared));
        orderServiceMock!.Setup(s => s.UpdateOrderStatus(1, newStatus, It.IsAny<List<Permission>>())).Throws(new NotFoundException("Order not found."));

        Assert.ThrowsException<NotFoundException>(() => ordersController!.UpdateOrderStatus(1, newStatus));
    }

    [TestMethod]
    public void GetSalesReport_ValidData_ReturnsOk()
    {
        var report = new SalesReportDto([], 0);
        orderServiceMock!.Setup(s => s.GetSalesReport()).Returns(report);

        var result = ordersController!.GetSalesReport();
        var resultObj = result as ObjectResult;

        Assert.IsNotNull(resultObj);
        Assert.AreEqual(200, resultObj.StatusCode);
    }
}

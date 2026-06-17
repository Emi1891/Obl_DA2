namespace DarkKitchen.Domain.Enums;

/// <summary>
/// Represents an action that a user is authorized to perform in the system.
/// </summary>
public enum Permission
{
    /// <summary>
    /// Permission to place a new order.
    /// </summary>
    PlaceOrder,

    /// <summary>
    /// Permission to retrieve the orders belonging to the current user.
    /// </summary>
    GetMyOrders,

    /// <summary>
    /// Permission to retrieve orders filtered by their status.
    /// </summary>
    GetOrdersByStatus,

    /// <summary>
    /// Permission to retrieve the details of a specific order.
    /// </summary>
    GetOrderDetails,

    /// <summary>
    /// Permission to mark an order as prepared.
    /// </summary>
    SetOrderStatusToPrepared,

    /// <summary>
    /// Permission to mark an order as canceled.
    /// </summary>
    SetOrderStatusToCanceled,

    /// <summary>
    /// Permission to mark an order as on its way to the customer.
    /// </summary>
    SetOrderStatusToOnItsWay,

    /// <summary>
    /// Permission to mark an order as delivered.
    /// </summary>
    SetOrderStatusToDelivered,

    /// <summary>
    /// Permission to mark an order as not delivered.
    /// </summary>
    SetOrderStatusToNotDelivered,

    /// <summary>
    /// Permission to mark an order as delayed.
    /// </summary>
    SetOrderStatusToDelayed,

    /// <summary>
    /// Permission to create a new user.
    /// </summary>
    CreateUser,

    /// <summary>
    /// Permission to retrieve the list of users.
    /// </summary>
    GetUsers,

    /// <summary>
    /// Permission to update an existing user.
    /// </summary>
    UpdateUser,

    /// <summary>
    /// Permission to delete an existing user.
    /// </summary>
    DeleteUser,

    /// <summary>
    /// Permission to create a new product.
    /// </summary>
    CreateProduct,

    /// <summary>
    /// Permission to retrieve the catalog of available products.
    /// </summary>
    GetProducts,

    /// <summary>
    /// Permission to retrieve a report of the most popular products.
    /// </summary>
    GetMostPopularProducts,

    /// <summary>
    /// Permission to update an existing product.
    /// </summary>
    UpdateProduct,

    /// <summary>
    /// Permission to update the status of a product.
    /// </summary>
    UpdateProductStatus,

    /// <summary>
    /// Permission to create a new promotion.
    /// </summary>
    CreatePromotion,

    /// <summary>
    /// Permission to retrieve the promotions currently active.
    /// </summary>
    GetCurrentPromotions,

    /// <summary>
    /// Permission to update an existing promotion.
    /// </summary>
    UpdatePromotion,

    /// <summary>
    /// Permission to update the products of an existing promotion.
    /// </summary>
    UpdatePromotionProducts,

    /// <summary>
    /// Permission to retrieve the sales report.
    /// </summary>
    GetSalesReport,

    /// <summary>
    /// Permission to retrieve audit records.
    /// </summary>
    GetAuditRecords,

    /// <summary>
    /// Permission to retrieve the list of shipping types.
    /// </summary>
    GetShippingTypes,

    /// <summary>
    /// Permission to create a new shipping type.
    /// </summary>
    CreateShippingType,

    /// <summary>
    /// Permission to update an existing shipping type.
    /// </summary>
    UpdateShippingType,

    /// <summary>
    /// Permission to import products from external sources (JSON, XML, plugins).
    /// </summary>
    ImportProducts
}

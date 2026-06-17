namespace DarkKitchen.Domain.Enums;

/// <summary>
/// Represents the status of an order through its lifecycle.
/// </summary>
public enum OrderStatus
{
    /// <summary>
    /// Order has been created and is awaiting preparation.
    /// </summary>
    Pending,

    /// <summary>
    /// Order has been prepared and is ready to be dispatched.
    /// </summary>
    Prepared,

    /// <summary>
    /// Order has been canceled.
    /// </summary>
    Canceled,

    /// <summary>
    /// Order is on its way to the customer.
    /// </summary>
    OnItsWay,

    /// <summary>
    /// Order has been delivered to the customer.
    /// </summary>
    Delivered,

    /// <summary>
    /// Order could not be delivered to the customer.
    /// </summary>
    NotDelivered,

    /// <summary>
    /// Order has been delayed by the food preparer.
    /// </summary>
    Delayed,
}

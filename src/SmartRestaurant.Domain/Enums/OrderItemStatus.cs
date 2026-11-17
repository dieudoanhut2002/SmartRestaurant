namespace SmartRestaurant.Domain.Enums;

public enum OrderItemStatus
{
    Draft = 0,
    Pending = 1,
    Cooking = 2,
    ReadyToServe = 3,
    Served = 4,
    Cancelled = 5
}

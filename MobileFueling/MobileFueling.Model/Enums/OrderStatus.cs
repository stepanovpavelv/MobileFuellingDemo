namespace MobileFueling.Model.Enums
{
    /// <summary>
    /// Статусы заказа в системе
    /// </summary>
    public enum OrderStatus
    {
        Created = 0,
        DriverAssigned,
        Completed,
        Cancelled,
        Paid
    }
}
using MobileFueling.Model.Enums;

namespace MobileFueling.Model
{
    public class OrderStatusHistory : IEntity
    {
        public long Id { get; set; }
        public long OrderId { get; set; }
        public Order Order { get; set; }
        public OrderStatus Status { get; set; }
    }
}

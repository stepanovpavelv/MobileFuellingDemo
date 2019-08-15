namespace MobileFueling.Model
{
    public class DriverOrderDetalization : IEntity
    {
        public long Id { get; set; }
        public long OrderId { get; set; }
        public Order Order { get; set; }
        public long DriverId { get; set; }
        public Driver Driver { get; set; }
        public System.DateTime ReceiptDate { get; set; }
    }
}

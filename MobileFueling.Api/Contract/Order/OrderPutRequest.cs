namespace MobileFueling.Api.Contract.Order
{
    public class OrderPutRequest
    {
        public long OrderId { get; set; }

        public long DriverId { get; set; }
    }
}
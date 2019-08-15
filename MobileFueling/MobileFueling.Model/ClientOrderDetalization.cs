namespace MobileFueling.Model
{
    public class ClientOrderDetalization : IEntity
    {
        public long Id { get; set; }
        public long OrderId { get; set; }
        public Order Order { get; set; }
        public System.DateTime CreationDate { get; set; }
        public decimal Quantity { get; set; }
        public long FuelTypeId { get; set; }
        public FuelType FuelType { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string Address { get; set; }
    }
}
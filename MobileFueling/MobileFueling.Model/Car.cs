namespace MobileFueling.Model
{
    public class Car : IEntity
    {
        public long Id { get; set; }
        public string Description { get; set; }
        public long FuelTypeId { get; set; }
        public FuelType FuelType { get; set; }
        public bool IsDefault { get; set; }
        public long ClientId { get; set; }
        public Client Client { get; set; }
    }
}
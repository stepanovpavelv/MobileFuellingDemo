namespace MobileFueling.Model
{
    /// <summary>
    /// Цена топлива
    /// </summary>
    public class FuelPrice : IEntity
    {
        public long Id { get; set; }

        public System.DateTime ChangedDate { get; set; }

        public long FuelTypeId { get; set; }

        public FuelType FuelType { get; set; }

        public decimal Price { get; set; }
    }
}
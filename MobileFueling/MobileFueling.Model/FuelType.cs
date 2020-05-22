namespace MobileFueling.Model
{
    /// <summary>
    /// Типы топлива
    /// </summary>
    public class FuelType : IEntity
    {
        public long Id { get; set; }

        public string Name { get; set; }
    }
}
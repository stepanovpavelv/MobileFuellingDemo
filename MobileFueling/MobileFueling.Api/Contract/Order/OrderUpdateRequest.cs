namespace MobileFueling.Api.Contract.Order
{
    /// <summary>
    /// Запрос на обновление заказа клиентом
    /// </summary>
    public class OrderUpdateRequest
    {
        public long? Id { get; set; }

        public long? ClientId { get; set; }

        public string ClientPhone { get; set; }

        public string Address { get; set; }

        public decimal Quantity { get; set; }

        public decimal Price { get; set; }

        public long FuelTypeId { get; set; }

        public float? Latitude { get; set; }

        public float? Longitude { get; set; }
    }
}
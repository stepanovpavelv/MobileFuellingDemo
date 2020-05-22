namespace MobileFueling.Api.Contract.FuelType
{
    /// <summary>
    /// Запрос на добавление стоимости за литр топлива
    /// </summary>
    public class FuelTypePutOneRequest
    {
        /// <summary>
        /// Дата изменения цены
        /// </summary>
        public System.DateTime? ChangedDate { get; set; }
        /// <summary>
        /// Цена
        /// </summary>
        public decimal Price { get; set; }
    }
}
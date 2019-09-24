using System.Collections.Generic;

namespace MobileFueling.ViewModel
{
    /// <summary>
    /// Модель представления заказа
    /// </summary>
    public class OrderVM
    {
        public long? Id { get; set; }

        public string Number { get; set; }

        public ClientDetalizationVM ClientDetalization { get; set; }

        public IEnumerable<DriverDetalizationVM> DriverDetalizations { get; set; }

        public IEnumerable<HistoryStatusItemVM> HistoryStatuses { get; set; }
    }
        
    /// <summary>
    /// Детализация заказа клиентом
    /// </summary>
    public class ClientDetalizationVM
    {
        public long ClientId { get; set; }

        public System.DateTime CreationDate { get; set; }

        public string Address { get; set; }

        public decimal Quantity { get; set; }

        public FuelTypeVM FuelType { get; set; }

        public float? Latitude { get; set; }

        public float? Longitude { get; set; }
    }

    /// <summary>
    /// Детализация заказа водителем
    /// </summary>
    public class DriverDetalizationVM
    {
        public System.DateTime ReceiptDate { get; set; }

        public long DriverId { get; set; }
    }
}
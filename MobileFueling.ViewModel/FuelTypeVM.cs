using System.ComponentModel.DataAnnotations;

namespace MobileFueling.ViewModel
{
    /// <summary>
    /// Модель типа топлива
    /// </summary>
    public class FuelTypeVM
    {
        /// <summary>
        /// Идентификатор типа топлива
        /// </summary>
        public long? Id { get; set; }
        /// <summary>
        /// Наименование типа топлива
        /// </summary>
        public string Name { get; set; }
    }
}
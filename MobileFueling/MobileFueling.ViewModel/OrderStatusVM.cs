using System.ComponentModel;

namespace MobileFueling.ViewModel
{
    public enum OrderStatusVM
    {
        [Description("Создан")]
        Created = 0,
        [Description("Назначен водитель")]
        DriverAssigned,
        [Description("Завершен")]
        Completed,
        [Description("Аннулирован")]
        Cancelled
    }
}
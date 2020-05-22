using System;

namespace MobileFueling.ViewModel
{
    /// <summary>
    /// Элемент истории изменения заказа
    /// </summary>
    public class HistoryStatusItemVM
    {
        public DateTime ChangeTime { get; set; }
        public OrderStatusVM Status { get; set; }
    }
}
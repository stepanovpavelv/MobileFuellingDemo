﻿using MobileFueling.ViewModel;
using System.Collections.Generic;

namespace MobileFueling.Api.Contract.Order
{
    /// <summary>
    /// Запрос-контракт для получения списка всех заказов
    /// </summary>
    public class OrderPostAllRequest
    {
        /// <summary>
        /// Дата начала создания заказа
        /// </summary>
        public System.DateTime? BeginDate { get; set; }

        /// <summary>
        /// Дата окончания создания заказа
        /// </summary>
        public System.DateTime? EndDate { get; set; }

        /// <summary>
        /// Коллекция номеров заказов
        /// </summary>
        public IEnumerable<string> Numbers { get; set; }

        /// <summary>
        /// Статусы заказов
        /// </summary>
        public IEnumerable<OrderStatusVM> Statuses { get; set; }
    }
}
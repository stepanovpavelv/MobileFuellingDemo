using MobileFueling.Api.Common.BaseResponseResources;

namespace MobileFueling.Api.Contract.Order
{
    /// <summary>
    /// Добавление/обновление заказа
    /// </summary>
    public class OrderUpdateResponse : BaseResponse
    {
        public long Id { get; set; }
    }
}
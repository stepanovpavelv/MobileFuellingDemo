using MobileFueling.Api.Common.BaseResponseResources;
using MobileFueling.ViewModel;

namespace MobileFueling.Api.Contract.Order
{
    /// <summary>
    /// Заказ
    /// </summary>
    public class OrderGetOneResponse : BaseResponse
    {
        public OrderVM Item { get; set; }
    }
}
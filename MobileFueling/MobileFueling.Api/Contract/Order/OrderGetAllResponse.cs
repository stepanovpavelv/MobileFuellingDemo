using MobileFueling.Api.Common.BaseResponseResources;
using MobileFueling.ViewModel;
using System.Collections.Generic;

namespace MobileFueling.Api.Contract.Order
{
    public class OrderPostAllResponse : BaseResponse
    {
        public IEnumerable<OrderVM> Items { get; set; }
    }
}
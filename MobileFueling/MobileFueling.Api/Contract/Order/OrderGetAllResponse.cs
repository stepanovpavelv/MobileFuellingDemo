using MobileFueling.Api.Common.BaseResponseResources;
using MobileFueling.ViewModel;
using System.Collections.Generic;

namespace MobileFueling.Api.Contract.Order
{
    public class OrderGetAllResponse : BaseResponse
    {
        public IEnumerable<OrderVM> Items { get; set; }
    }
}
using MobileFueling.Api.Common.BaseResponseResources;

namespace MobileFueling.Api.Contract.Order
{
    public class OrderPutResponse : BaseResponse
    {
        public bool IsSuccess { get; set; }
    }
}
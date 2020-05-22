using MobileFueling.Api.Common.BaseResponseResources;
using MobileFueling.ViewModel;

namespace MobileFueling.Api.Contract.FuelType
{
    public class FuelTypeGetOneResponse : BaseResponse
    {
        public FuelTypeVM Item { get; set; }
    }
}
using MobileFueling.Api.Common.BaseResponseResources;
using MobileFueling.ViewModel;
using System.Collections.Generic;

namespace MobileFueling.Api.Contract.FuelType
{
    public class FuelTypeGetAllResponse : BaseResponse
    {
        public IEnumerable<FuelTypeVM> Items { get; set; }
    }
}
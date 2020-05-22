using MobileFueling.Api.Common.BaseResponseResources;
using MobileFueling.ViewModel;

namespace MobileFueling.Api.Contract.FuelType
{
    /// <summary>
    /// Ответ на добавление цены к виду топлива
    /// </summary>
    public class FuelTypePutOneResponse : BaseResponse
    {
        public FuelTypeVM Item { get; set; }
    }
}
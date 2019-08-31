using Microsoft.Extensions.Localization;
using MobileFueling.DB;

namespace MobileFueling.Api.ApiModels.Order
{
    public class OrderModel
    {
        private readonly FuelDbContext _fuelContext;
        private readonly IStringLocalizer _stringLocalizer;

        public OrderModel(FuelDbContext fuelContext, IStringLocalizer stringLocalizer)
        {
            _fuelContext = fuelContext;
            _stringLocalizer = stringLocalizer;
        }
    }
}
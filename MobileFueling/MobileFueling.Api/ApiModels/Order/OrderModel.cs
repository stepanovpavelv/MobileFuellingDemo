using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Localization;
using MobileFueling.Api.Contract.Order;
using MobileFueling.DB;
using MobileFueling.Model;

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

        public async Task<OrderGetAllResponse> GetAll(ApplicationUser currentUser, OrderGetAllRequest request)
        {
            return null;
        }

        public async Task<OrderGetOneResponse> GetOne(ApplicationUser currentUser, long id)
        {
            return null;
        }

        //private async Task<Order>
    }
}
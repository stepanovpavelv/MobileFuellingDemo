using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using MobileFueling.Api.Common.Localization;
using MobileFueling.Api.Contract.Order;
using MobileFueling.DB;
using MobileFueling.Model;
using MobileFueling.ViewModel;

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

        public async Task<OrderUpdateResponse> PostOne(ApplicationUser currentUser, OrderUpdateRequest request)
        {
            var response = new OrderUpdateResponse();
            Model.Order order = null;

            if (currentUser is Driver) // водитель не имеет права добавлять заказ
            {
                response.AddError(_stringLocalizer[CustomStringLocalizer.NO_RIGHTS_TO_CREATE_UPDATE_ORDER]);
                return response;
            }

            if (request.Id.HasValue)
            {
                order = await _fuelContext.Orders.FirstOrDefaultAsync(x => x.Id == request.Id.Value);
            }
            else
            {
                order = new Model.Order();
            }

            if (string.IsNullOrEmpty(order.Number))
            {
                order.Number = OrderNumberGenerator.GetNumber(_fuelContext);
            }
            order.ClientId = request.ClientId;

            ClientOrderDetalization clientDetalization = null;
            if (request.Id.HasValue)
            {
                clientDetalization = await _fuelContext.ClientDetalizations.FirstOrDefaultAsync(x => x.OrderId == order.Id);
            }
            else
            {
                clientDetalization = new ClientOrderDetalization();
            }
            if (clientDetalization.CreationDate == default)
            {
                clientDetalization.CreationDate = DateTime.Now;
            }
            clientDetalization.Address = request.Address;
            clientDetalization.FuelTypeId = request.FuelTypeId;
            clientDetalization.Latitude = request.Latitude;
            clientDetalization.Longitude = request.Longitude;
            clientDetalization.Quantity = request.Quantity;
            clientDetalization.OrderId = order.Id;

            if (request.Id.HasValue)
            {
                _fuelContext.Orders.Update(order);
                _fuelContext.ClientDetalizations.Update(clientDetalization);
            }
            else
            {
                await _fuelContext.Orders.AddAsync(order);
                await _fuelContext.ClientDetalizations.AddAsync(clientDetalization);
            }
            await _fuelContext.SaveChangesAsync();

            response.Id = order.Id;
            return response;
        }

        private async Task<List<DriverOrderDetalization>> GetDriverDetalizationsAsync(long orderId)
        {
            return await _fuelContext.DriverDetalizations.Where(x => x.OrderId == orderId).OrderBy(x => x.ReceiptDate).ToListAsync();
        }

        private async Task<List<OrderStatusHistory>> GetStatusHistoryAsync(long orderId)
        {
            return await _fuelContext.StatusHistory.Where(x => x.OrderId == orderId).OrderBy(x => x.ChangeTime).ToListAsync();
        }

        private async Task<OrderVM> Convert(Model.Order order, ClientOrderDetalization clientDetalization, IEnumerable<DriverOrderDetalization> driverDetalizations, IEnumerable<OrderStatusHistory> statusHistory)
        {
            var orderVM = new OrderVM
            {
                Id = order.Id,
                Number = order.Number,
                ClientId = order.ClientId
            };
            // необходимо кастовать driverDetalizations и statusHistory
            // предусмотреть, чтоб сам клиент не смог водителя назначить на заказ

            return orderVM;
        }
    }
}
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using MobileFueling.Api.ApiModels.Order.Payment;
using MobileFueling.Api.Common.Localization;
using MobileFueling.Api.Contract.Order;
using MobileFueling.DB;
using MobileFueling.Model;
using MobileFueling.ViewModel;
using Newtonsoft.Json;

namespace MobileFueling.Api.ApiModels.Order
{
    public class OrderModel
    {
        private readonly FuelDbContext _fuelContext;
        private readonly IStringLocalizer _stringLocalizer;
        private readonly ILogger<OrderModel> _logger;
        private readonly IConfiguration _configuration;
        
        private static HttpClientHandler _httpClientHandler = new HttpClientHandler
        {
            AutomaticDecompression = System.Net.DecompressionMethods.GZip | System.Net.DecompressionMethods.Deflate,
            AllowAutoRedirect = false
        };
        private static HttpClient _httpClient = new HttpClient(_httpClientHandler);

        public OrderModel(FuelDbContext fuelContext, IStringLocalizer stringLocalizer, IConfiguration configuration, ILogger<OrderModel> logger)
        {
            _fuelContext = fuelContext;
            _stringLocalizer = stringLocalizer;
            _logger = logger;
            _configuration = configuration;
        }

        public async Task<OrderPostAllResponse> PostAll(ApplicationUser currentUser, OrderPostAllRequest request)
        {
            var response = new OrderPostAllResponse();

            var query = _fuelContext.Orders.AsQueryable();
            if (request.Numbers != null && request.Numbers.Any())
            {
                query = query.Where(x => request.Numbers.Contains(x.Number));
            }

            var queryOrderIds = query.Select(x => x.Id);
            if (request.Statuses != null && request.Statuses.Any())
            {
                // узнаем последние статусы у заказов, полученных на предыдущем шаге
                var lastStatuses = await _fuelContext.StatusHistory
                    .Where(x => queryOrderIds.Contains(x.OrderId))
                    .GroupBy(x => x.OrderId)
                    .SelectMany(x => x.OrderByDescending(t => t.ChangeTime).Take(1))
                    .Where(x => request.Statuses.Contains((OrderStatusVM)x.Status))
                    .ToListAsync();

                queryOrderIds = lastStatuses.Select(x => x.OrderId).AsQueryable();
                query = query.Where(x => lastStatuses.Any(t => t.OrderId == x.Id));
            }

            if (request.BeginDate.HasValue)
            {
                var beginDateDetalizations = await _fuelContext.ClientDetalizations
                    .Where(x => queryOrderIds.Contains(x.OrderId) && x.CreationDate >= request.BeginDate.Value)
                    .ToListAsync();

                queryOrderIds = beginDateDetalizations.Select(x => x.OrderId).AsQueryable();
                query = query.Where(x => beginDateDetalizations.Any(t => t.OrderId == x.Id));
            }

            if (request.EndDate.HasValue)
            {
                var endDateDetalizations = await _fuelContext.ClientDetalizations
                    .Where(x => queryOrderIds.Contains(x.OrderId) && x.CreationDate <= request.EndDate.Value)
                    .ToListAsync();

                queryOrderIds = endDateDetalizations.Select(x => x.OrderId).AsQueryable();
                query = query.Where(x => endDateDetalizations.Any(t => t.OrderId == x.Id));
            }

            var orders = await query.ToListAsync();
            if(!orders.Any())
            {
                response.AddMessage(Common.BaseResponseResources.MessageType.INFO, _stringLocalizer[CustomStringLocalizer.ORDERS_NOT_FOUND]);
                return response;
            }

            var ordersVM = new List<OrderVM>();
            foreach (var order in orders)
            {
                var clientDetalization = await _fuelContext.ClientDetalizations.FirstOrDefaultAsync(x => x.OrderId == order.Id);
                var driverDetalizations = await GetDriverDetalizationsAsync(order.Id);
                var statusHistories = await GetStatusHistoryAsync(order.Id);

                var item = Convert(order, clientDetalization, driverDetalizations, statusHistories);
                ordersVM.Add(item);
            }
            response.Items = ordersVM;
            return response;
        }

        public async Task<OrderGetOneResponse> GetOne(ApplicationUser currentUser, long id)
        {
            var response = new OrderGetOneResponse();

            var order = await _fuelContext.Orders.FirstOrDefaultAsync(x => x.Id == id);
            if (order == null)
            {
                response.AddError(_stringLocalizer[CustomStringLocalizer.ORDER_NOT_FOUND]);
                return response;
            }

            var clientDetalization = await _fuelContext.ClientDetalizations.FirstOrDefaultAsync(x => x.OrderId == id);
            var driverDetalizations = await GetDriverDetalizationsAsync(id);
            var statusHistories = await GetStatusHistoryAsync(id);

            response.Item = Convert(order, clientDetalization, driverDetalizations, statusHistories);
            return response;
        }

        public async Task<OrderUpdateResponse> PostOne(ApplicationUser currentUser, IConfiguration configuration, OrderUpdateRequest request)
        {
            var response = new OrderUpdateResponse();
            Model.Order order = null;

            if((currentUser == null && string.IsNullOrEmpty(request.ClientPhone)) ||
               (currentUser != null && !(currentUser is Client))) // только клиент может добавлять заказ (или по номеру телефону)
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

            if (order == null)
            {
                response.AddError(_stringLocalizer[CustomStringLocalizer.ORDER_NOT_FOUND]);
                return response;
            }

            if (string.IsNullOrEmpty(order.Number))
            {
                order.Number = OrderNumberGenerator.GetNumber(_fuelContext);
            }

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
            clientDetalization.ClientId = currentUser?.Id;
            clientDetalization.ClientPhone = request.ClientPhone;
            clientDetalization.Address = request.Address;
            clientDetalization.FuelTypeId = request.FuelTypeId;
            clientDetalization.Latitude = request.Latitude;
            clientDetalization.Longitude = request.Longitude;
            clientDetalization.Quantity = request.Quantity;
            clientDetalization.FuelPrice = request.Price;
            clientDetalization.Cost = request.Price * request.Quantity;

            if (string.IsNullOrEmpty(clientDetalization.PaymentIdempotenceKey))
            {
                clientDetalization.PaymentIdempotenceKey = Guid.NewGuid().ToString();
            }

            if (request.Id.HasValue)
            {
                clientDetalization.OrderId = order.Id;

                _fuelContext.Orders.Update(order);
                _fuelContext.ClientDetalizations.Update(clientDetalization);
            }
            else
            {
                clientDetalization.Order = order;

                //await _fuelContext.Orders.AddAsync(order);
                await _fuelContext.ClientDetalizations.AddAsync(clientDetalization);
            }
            await _fuelContext.SaveChangesAsync();

            if (!request.Id.HasValue)
            {
                // записать в историю, что заказ создан
                await AddStatusToHistory(order.Id, OrderStatusVM.Created);
            }

            var currentOrderStatus = await GetLastStatusRecord(order.Id);
            if (currentOrderStatus != null && currentOrderStatus.Status == Model.Enums.OrderStatus.Created)
            {
                // отправить запрос на создание платежа в яндекс.кассе
                var yandexResponse = await CreatePaymentAsync(order.Number, clientDetalization);
                if (yandexResponse != null)
                {
                    response.PaymentLink = yandexResponse.Confirmation?.ConfirmationUrl;

                    // повторяющаяся таска до тех пор, пока не станет ясен статус заказа (оплачен/отменен)
                    BackgroundJob.Schedule(() => CheckPaymentProcessingAsync(order.Id, yandexResponse.Id), TimeSpan.FromSeconds(15.0));
                }
            }

            response.Id = order.Id;
            return response;
        }

        public async Task<OrderPutResponse> PutOne(ApplicationUser currentUser, long orderId, OrderStatusVM status)
        {
            var response = new OrderPutResponse { IsSuccess = false };
            // данным методом нельзя назначать следующие статусы, для этого есть отдельные методы
            if (status == OrderStatusVM.Created || status == OrderStatusVM.DriverAssigned || status == OrderStatusVM.Paid)
            {
                response.AddError(_stringLocalizer[CustomStringLocalizer.NO_RIGHTS_TO_ASSIGN_DRIVER]);
                return response;
            }

            if (currentUser is Driver && status == OrderStatusVM.Cancelled)
            {
                response.AddError(_stringLocalizer[CustomStringLocalizer.NO_RIGHTS_TO_CANCEL_ORDER]);
                return response;
            }

            await AddStatusToHistory(orderId, status);

            response.IsSuccess = true;
            return response;
        }

        public async Task<OrderPutResponse> PutOne(ApplicationUser currentUser, OrderPutRequest request)
        {
            var response = new OrderPutResponse { IsSuccess = false };
            if (currentUser is Client)
            {
                response.AddError(_stringLocalizer[CustomStringLocalizer.NO_RIGHTS_TO_ASSIGN_DRIVER]);
                return response;
            }

            var order = await _fuelContext.Orders.FirstOrDefaultAsync(x => x.Id == request.OrderId);
            if (order == null)
            {
                response.AddError(_stringLocalizer[CustomStringLocalizer.ORDER_NOT_FOUND]);
                return response;
            }

            var driver = await _fuelContext.DriverUsers.FirstOrDefaultAsync(x => x.Id == request.DriverId);
            if (driver == null)
            {
                response.AddError(_stringLocalizer[CustomStringLocalizer.USER_NOT_FOUND]);
                return response;
            }

            var lastHistoryRecord = await _fuelContext.StatusHistory
                    .Where(x => x.OrderId == order.Id)
                    .OrderByDescending(x => x.ChangeTime)
                    .FirstOrDefaultAsync();
            if (lastHistoryRecord != null && lastHistoryRecord.Status == Model.Enums.OrderStatus.Completed)
            {
                response.AddError(_stringLocalizer[CustomStringLocalizer.USER_CAN_NOT_ASSIGN_DRIVER_COMPLETED_ORDER]);
                return response;
            }

            await _fuelContext.DriverDetalizations.AddAsync(new DriverOrderDetalization
            {
                DriverId = driver.Id,
                OrderId = order.Id,
                ReceiptDate = DateTime.Now
            });
            await _fuelContext.SaveChangesAsync();

            // записать в историю, что водитель назначен
            await AddStatusToHistory(order.Id, OrderStatusVM.DriverAssigned);

            response.IsSuccess = true;
            return response;
        }

        private async Task AddStatusToHistory(long orderId, OrderStatusVM statusVM)
        {
            await _fuelContext.StatusHistory.AddAsync(new OrderStatusHistory
            {
                ChangeTime = DateTime.Now,
                OrderId = orderId,
                Status = (Model.Enums.OrderStatus)statusVM
            });
            await _fuelContext.SaveChangesAsync();
        }

        private async Task<GetYandexPaymentLinkResponse> CreatePaymentAsync(string orderNumber, ClientOrderDetalization clientDetalization)
        {
            var createPaymentUrl = string.Concat(_configuration["PaymentCashOptions:APIEndpoint"], _configuration["PaymentCashOptions:CreatePaymentMethod"]);
            var supplierRequest = new GetYandexPaymentLinkRequest
            {
                Amount = new PaymentAmount { Value = clientDetalization.Cost },
                Confirmation = new PaymentReturn { ReturnUrl = _configuration["PaymentCashOptions:ReturnUrl"] },
                Description = $"Оплата заказа №{orderNumber}"
            };
            var strSupplierRequest = JsonConvert.SerializeObject(supplierRequest);
            var strSupplierResponse = string.Empty;
            try
            {
                var credentials = Encoding.ASCII.GetBytes($"{_configuration["PaymentCashOptions:ShopId"]}:{_configuration["PaymentCashOptions:SecretKey"]}");
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", System.Convert.ToBase64String(credentials));
                _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");
                _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Idempotence-Key", clientDetalization.PaymentIdempotenceKey);
                
                using (var response = await _httpClient.PostAsync(createPaymentUrl,
                                                                        new StringContent(strSupplierRequest, Encoding.UTF8, "application/json")))
                {
                    strSupplierResponse = await response.Content.ReadAsStringAsync();
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        return JsonConvert.DeserializeObject<GetYandexPaymentLinkResponse>(strSupplierResponse);
                    }
                    else
                    {
                        // логгируем некорректный ответ
                        _logger.LogError(GetFatalPaymentMessage(response.StatusCode, strSupplierRequest, strSupplierResponse));
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, GetFatalPaymentMessage(System.Net.HttpStatusCode.InternalServerError, strSupplierRequest, strSupplierResponse));
            } 

            // иначе была ошибка, и она была залоггирована
            return null;
        }

        /// <summary>
        /// Проверка с определенной периодичностью статуса заказа 
        /// </summary>
        /// <param name="orderId">Идентификатор заказа</param>
        /// <param name="paymentId">Идентификатор заказа</param>
        /// <returns>Задача</returns>
        public async Task CheckPaymentProcessingAsync(long orderId, string paymentId)
        {
            GetYandexPaymentStatusResponse supplierStatusResponse = null;

            var statusPaymentRequestUrl = string.Concat(_configuration["PaymentCashOptions:APIEndpoint"], _configuration["PaymentCashOptions:CreatePaymentMethod"],$"/{paymentId}");
            var strSupplierResponse = string.Empty;
            try
            {
                var credentials = Encoding.ASCII.GetBytes($"{_configuration["PaymentCashOptions:ShopId"]}:{_configuration["PaymentCashOptions:SecretKey"]}");
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", System.Convert.ToBase64String(credentials));
                _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");

                using (var response = await _httpClient.GetAsync(statusPaymentRequestUrl))
                {
                    strSupplierResponse = await response.Content.ReadAsStringAsync();
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        supplierStatusResponse = JsonConvert.DeserializeObject<GetYandexPaymentStatusResponse>(strSupplierResponse);
                    }
                    else
                    {
                        // логгируем некорректный ответ
                        _logger.LogError(GetFatalPaymentMessage(response.StatusCode, statusPaymentRequestUrl, strSupplierResponse));
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, GetFatalPaymentMessage(System.Net.HttpStatusCode.InternalServerError, statusPaymentRequestUrl, strSupplierResponse));
            }

            if (supplierStatusResponse != null && supplierStatusResponse.Paid)
            {
                // установить для заказа статус "Оплачен"
                await AddStatusToHistory(orderId, OrderStatusVM.Paid);
            }
            else if(supplierStatusResponse != null && supplierStatusResponse.Status == "cancelled")
            {
                // установить для заказа статус "Отменен"
                await AddStatusToHistory(orderId, OrderStatusVM.Cancelled);
            }
            else
            {
                // продолжать ждать "оплаты заказа"
                BackgroundJob.Schedule(() => CheckPaymentProcessingAsync(orderId, paymentId), TimeSpan.FromSeconds(15.0));
            }
        }

        private string GetFatalPaymentMessage(System.Net.HttpStatusCode status, string strSupplierRequest, string strSupplierResponse)
        {
            return $"Incorrect response from Yandex! Status: {status}, full request: {strSupplierRequest}, full response: {strSupplierResponse}";
        }

        private async Task<OrderStatusHistory> GetLastStatusRecord(long orderId)
        {
            return await _fuelContext.StatusHistory.Where(x => x.OrderId == orderId).OrderByDescending(x => x.ChangeTime).FirstOrDefaultAsync();
        }

        private async Task<List<DriverOrderDetalization>> GetDriverDetalizationsAsync(long orderId)
        {
            return await _fuelContext.DriverDetalizations.Where(x => x.OrderId == orderId).OrderBy(x => x.ReceiptDate).ToListAsync();
        }

        private async Task<List<OrderStatusHistory>> GetStatusHistoryAsync(long orderId)
        {
            return await _fuelContext.StatusHistory.Where(x => x.OrderId == orderId).OrderBy(x => x.ChangeTime).ToListAsync();
        }

        private OrderVM Convert(Model.Order order, ClientOrderDetalization clientDetalization, IEnumerable<DriverOrderDetalization> driverDetalizations, IEnumerable<OrderStatusHistory> statusHistories)
        {
            var orderVM = new OrderVM
            {
                Id = order.Id,
                Number = order.Number,
                ClientDetalization = new ClientDetalizationVM
                {
                    ClientId = clientDetalization.ClientId,
                    ClientPhone = clientDetalization.ClientPhone,
                    Address = clientDetalization.Address,
                    Latitude = clientDetalization.Latitude,
                    Longitude = clientDetalization.Longitude,
                    Quantity = clientDetalization.Quantity,
                    Price = clientDetalization.FuelPrice,
                    Cost = clientDetalization.Cost,
                    CreationDate = clientDetalization.CreationDate,
                    FuelType = new FuelTypeVM
                    {
                        Id = clientDetalization.FuelTypeId
                    }
                }
            };

            if (driverDetalizations != null && driverDetalizations.Any())
            {
                var driverDetalizationsVM = new List<DriverDetalizationVM>();
                foreach (var driverDetalization in driverDetalizations)
                {
                    driverDetalizationsVM.Add(new DriverDetalizationVM
                    {
                        ReceiptDate = driverDetalization.ReceiptDate,
                        DriverId = driverDetalization.DriverId
                    });
                }
                orderVM.DriverDetalizations = driverDetalizationsVM;
            }

            if (statusHistories != null && statusHistories.Any())
            {
                var statusHistoriesVM = new List<HistoryStatusItemVM>();
                foreach(var statusHistory in statusHistories)
                {
                    statusHistoriesVM.Add(new HistoryStatusItemVM
                    {
                        ChangeTime = statusHistory.ChangeTime,
                        Status = (OrderStatusVM)statusHistory.Status
                    });
                }
                orderVM.HistoryStatuses = statusHistoriesVM;
            }

            return orderVM;
        }
    }
}
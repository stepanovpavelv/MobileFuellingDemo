using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using MobileFueling.Api.ApiModels.Order;
using MobileFueling.Api.Contract.Order;
using MobileFueling.DB;
using MobileFueling.Model;
using MobileFueling.ViewModel;
using System.Threading.Tasks;

namespace MobileFueling.Api.Controllers.v1
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly OrderModel _orderModel;
        private readonly IConfiguration _configuration;

        public OrderController(UserManager<ApplicationUser> userManager, FuelDbContext fuelContext, IStringLocalizer stringLocalizer, IConfiguration configuration, ILogger<OrderModel> logger)
        {
            _userManager = userManager;
            _orderModel = new OrderModel(fuelContext, stringLocalizer, configuration, logger);
            _configuration = configuration;
        }

        /// <summary>
        /// Получение списка заказов
        /// </summary>
        /// <param name="request">Запрос (контракт)</param>
        /// <returns>Список заказов</returns>
        [Authorize]
        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        public async Task<OrderGetAllResponse> Get(OrderGetAllRequest request)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            return await _orderModel.GetAll(currentUser, request);
        }

        /// <summary>
        /// Получение информации по заказу
        /// </summary>
        /// <param name="id">Идентификатор заказа</param>
        /// <returns>Заказ</returns>
        [Authorize]
        [HttpGet("{id}", Name = "GetOrder")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        public async Task<OrderGetOneResponse> Get(long id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            return await _orderModel.GetOne(currentUser, id);
        }

        /// <summary>
        /// Добавление/обновление клиентом деталей заказа
        /// </summary>
        /// <param name="request">Запрос (контракт)</param>
        /// <returns>Заказ</returns>
        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        public async Task<OrderUpdateResponse> Post([FromBody] OrderUpdateRequest request)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            return await _orderModel.PostOne(currentUser, _configuration, request);
        }

        /// <summary>
        /// Обновление водителем/менеджером деталей заказа
        /// </summary>
        /// <param name="id"></param>
        /// <param name="request"></param>
        /// <returns>Ответ (контракт)</returns>
        [Authorize]
        [HttpPut("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        public async Task<OrderPutResponse> Put(long id, [FromBody] OrderPutRequest request)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            return await _orderModel.PutOne(currentUser, request);
        }

        /// <summary>
        /// Обновление статуса заказа
        /// </summary>
        /// <param name="id"></param>
        /// <param name="status"></param>
        /// <returns>Ответ (контракт)</returns>
        [Authorize]
        [HttpPut("{id}/{status}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        public async Task<OrderPutResponse> Put(long id, OrderStatusVM status)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            return await _orderModel.PutOne(currentUser, id, status);
        }
    }
}
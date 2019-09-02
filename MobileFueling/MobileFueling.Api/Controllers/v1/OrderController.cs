using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using MobileFueling.Api.ApiModels.Order;
using MobileFueling.Api.Contract.Order;
using MobileFueling.DB;
using MobileFueling.Model;
using System.Threading.Tasks;

namespace MobileFueling.Api.Controllers.v1
{
    [Authorize]
    [Route("api/v1/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly OrderModel _orderModel;

        public OrderController(UserManager<ApplicationUser> userManager, FuelDbContext fuelContext, IStringLocalizer stringLocalizer)
        {
            _userManager = userManager;
            _orderModel = new OrderModel(fuelContext, stringLocalizer);
        }

        /// <summary>
        /// Получение списка заказов
        /// </summary>
        /// <param name="request">Запрос (контракт)</param>
        /// <returns>Список заказов</returns>
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
        [HttpGet("{id}", Name = "GetOrder")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        public async Task<OrderGetOneResponse> Get(long id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            return await _orderModel.GetOne(currentUser, id);
        }

        /// <summary>
        /// Обновление клиентом деталей заказа
        /// </summary>
        /// <param name="request">Запрос (контракт(</param>
        /// <returns>Заказ</returns>
        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        public async Task<OrderUpdateResponse> Post([FromBody] OrderUpdateRequest request)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            return await _orderModel.PostOne(currentUser, request);
        }

        /// <summary>
        /// Обновление водителем/менеджером деталей заказа
        /// </summary>
        /// <param name="id"></param>
        /// <param name="value"></param>
        // PUT: api/Order/5
        [HttpPut("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        public void Put(long id, [FromBody] string value)
        {

        }
    }
}
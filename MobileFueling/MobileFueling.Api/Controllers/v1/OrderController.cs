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

        // GET: api/Order/5
        [HttpGet("{id}", Name = "GetOrder")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Order
        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        public void Post([FromBody] string value)
        {
        }

        // PUT: api/Order/5
        [HttpPut("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        public void Delete(int id)
        {
        }
    }
}
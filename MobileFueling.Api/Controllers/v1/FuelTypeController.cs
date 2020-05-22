using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using MobileFueling.Api.ApiModels.FuelType;
using MobileFueling.Api.Contract.FuelType;
using MobileFueling.DB;
using MobileFueling.ViewModel;
using System.Threading.Tasks;

namespace MobileFueling.Api.Controllers.v1
{
    [Produces("application/json")]
    [Route("api/v1/[controller]")]
    [ApiController]
    public class FuelTypeController : ControllerBase
    {
        private readonly FuelTypeModel _fuelTypeModel;

        public FuelTypeController(FuelDbContext fuelContext, IStringLocalizer stringLocalizer)
        {
            _fuelTypeModel = new FuelTypeModel(fuelContext, stringLocalizer);
        }

        /// <summary>
        /// Получение списка всех типов топлива
        /// </summary>
        /// <returns>Типы топлива</returns>
        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        public async Task<FuelTypeGetAllResponse> Get()
        {
            return await _fuelTypeModel.GetAll();
        }

        /// <summary>
        /// Получение типа топлива
        /// </summary>
        /// <param name="id">Идентификатор типа топлива</param>
        /// <returns>Тип топлива</returns>
        [Authorize]
        [HttpGet("{id}", Name = "GetFuelType")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        public async Task<FuelTypeGetOneResponse> Get(long id)
        {
            return await _fuelTypeModel.GetOne(id);
        }

        /// <summary>
        /// Сохранение типа топлива
        /// </summary>
        /// <param name="value">Тип топлива</param>
        /// <returns>Идентификатор</returns>
        [Authorize]
        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        public async Task<FuelTypeUpdateResponse> Post([FromBody] FuelTypeVM value)
        {
            return await _fuelTypeModel.PostOne(value);
        }

        /// <summary>
        /// Удаление типа топлива
        /// </summary>
        /// <param name="id">Идентификатор</param>
        /// <returns>Ответ на удаление типов топлива</returns>
        [Authorize]
        [HttpDelete("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        public async Task<FuelTypeDeleteResponse> Delete(long id)
        {
            return await _fuelTypeModel.DeleteOne(id);
        }

        /// <summary>
        /// Метод добавляет цену для типа топлива
        /// </summary>
        /// <param name="id">Идентификатор типа топлива</param>
        /// <param name="request">Запрос</param>
        /// <returns>Тип топлива</returns>
        [Authorize]
        [HttpPut("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        public async Task<FuelTypePutOneResponse> Put(long id, [FromBody] FuelTypePutOneRequest request)
        {
            return await _fuelTypeModel.PutOne(id, request);
        }
    }
}
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using MobileFueling.Api.ApiModels.FuelType;
using MobileFueling.DB;
using MobileFueling.ViewModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MobileFueling.Api.Controllers.v1
{
    [Authorize]
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
        public async Task<IEnumerable<FuelTypeVM>> Get()
        {
            return await _fuelTypeModel.GetAll();
        }

        /// <summary>
        /// Получение типа топлива
        /// </summary>
        /// <param name="id">Идентификатор типа топлива</param>
        /// <returns>Тип топлива</returns>
        [HttpGet("{id}", Name = "GetFuelType")]
        [ProducesResponseType(200)]
        public async Task<FuelTypeVM> Get(long id)
        {
            return await _fuelTypeModel.GetOne(id);
        }

        /// <summary>
        /// Сохранение типа топлива
        /// </summary>
        /// <param name="value">Тип топлива</param>
        /// <returns>Идентификатор</returns>
        [HttpPost]
        [ProducesResponseType(200)]
        public async Task<long> Post([FromBody] FuelTypeVM value)
        {
            return await _fuelTypeModel.PostOne(value);
        }

        /// <summary>
        /// Удаление типа топлива
        /// </summary>
        /// <param name="id">Идентификатор</param>
        [HttpDelete("{id}")]
        [ProducesResponseType(200)]
        public async Task Delete(long id)
        {
            await _fuelTypeModel.DeleteOne(id);
        }
    }
}
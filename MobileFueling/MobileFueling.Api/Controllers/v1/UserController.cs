using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MobileFueling.ViewModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MobileFueling.Api.Controllers.v1
{
    /// <summary>
    /// Controller for managing users. For adding new please use "Auth" and method "register"
    /// </summary>
    [Authorize]
    [Route("api/v1/[controller]/{userType}")]
    [ApiController]
    public class UserController : ControllerBase
    {
        /// <summary>
        /// Получение списка пользователей
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        public async Task<IEnumerable<ApplicationUserVM>> Get(UserTypeVM userType)
        {
            return null;
        }

        /// <summary>
        /// Получение информации по пользователю
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}", Name = "GetUser")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        public async Task<ApplicationUserVM> Get(long id)
        {
            return new ApplicationUserVM();
        }

        /// <summary>
        /// Изменение информации по пользователю
        /// </summary>
        /// <param name="id">Идентификатор пользователя</param>
        /// <param name="value">Информация по пользователю</param>
        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        public async Task Post([FromBody] ApplicationUserVM value)
        {
        }

        /// <summary>
        /// Удаление пользователя
        /// </summary>
        /// <param name="id"></param>
        [HttpDelete("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        public async Task Delete(long id)
        {
        }
    }
}
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using MobileFueling.Api.ApiModels.User;
using MobileFueling.DB;
using MobileFueling.Model;
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
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly UserModel _userModel;

        public UserController(UserManager<ApplicationUser> userManager, FuelDbContext fuelContext, IStringLocalizer stringLocalizer)
        {
            _userManager = userManager;
            _userModel = new UserModel(stringLocalizer, fuelContext);
        }

        /// <summary>
        /// Получение списка пользователей
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        public async Task<IEnumerable<ApplicationUserVM>> Get(UserTypeVM userType)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            return await _userModel.GetAll(_userManager, currentUser, userType);
        }

        /// <summary>
        /// Получение информации по пользователю
        /// </summary>
        /// <param name="userType"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}", Name = "GetUser")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        public async Task<ApplicationUserVM> Get(UserTypeVM userType, long id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            return await _userModel.GetOne(_userManager, currentUser, userType, id);
        }

        /// <summary>
        /// Изменение информации по пользователю
        /// </summary>
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
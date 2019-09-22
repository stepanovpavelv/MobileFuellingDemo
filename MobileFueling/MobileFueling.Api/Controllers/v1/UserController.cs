using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using MobileFueling.Api.ApiModels.User;
using MobileFueling.Api.Contract.UserData;
using MobileFueling.DB;
using MobileFueling.Model;
using MobileFueling.ViewModel;
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
            _userModel = new UserModel(stringLocalizer, userManager, fuelContext);
        }

        /// <summary>
        /// Получение списка пользователей
        /// </summary>
        /// <returns>Список пользователей</returns>
        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        public async Task<UserGetAllResponse> Get(UserTypeVM userType)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            return await _userModel.GetAll(currentUser, userType);
        }

        /// <summary>
        /// Получение информации по пользователю
        /// </summary>
        /// <param name="userType">Тип пользователя</param>
        /// <param name="id">Идентификатор пользователя</param>
        /// <returns>Информация по пользователю</returns>
        [HttpGet("{id}", Name = "GetUser")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        public async Task<UserGetOneResponse> Get(UserTypeVM userType, long id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            return await _userModel.GetOne(currentUser, userType, id);
        }

        /// <summary>
        /// Получение информации по текущему пользователю
        /// </summary>
        /// <returns>Информация по пользователю</returns>
        [Route("~/api/v1/[controller]/meta")]
        [HttpGet()]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        public async Task<UserGetOneResponse> Get()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            return await _userModel.GetOne(currentUser);
        }

        /// <summary>
        /// Изменение информации по пользователю
        /// </summary>
        /// <param name="userType">Тип пользователя</param>
        /// <param name="value">Информация по пользователю</param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        public async Task<UserUpdateResponse> Post(UserTypeVM userType, [FromBody] ApplicationUserVM value)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            return await _userModel.PostOne(currentUser, userType, value);
        }

        /// <summary>
        /// Удаление пользователя
        /// </summary>
        /// <param name="userType">Тип пользователя</param>
        /// <param name="id">Идентификатор пользователя</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        public async Task<UserDeleteResponse> Delete(UserTypeVM userType, long id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            return await _userModel.DeleteOne(currentUser, userType, id);
        }
    }
}
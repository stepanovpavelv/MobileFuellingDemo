using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using MobileFueling.Api.ApiModels;
using MobileFueling.Api.ApiModels.User;
using MobileFueling.Model;
using MobileFueling.ViewModel;
using System.Threading.Tasks;

namespace MobileFueling.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly UserModel _userModel;
        private readonly IStringLocalizer _stringLocalizer;

        public AuthController(UserManager<ApplicationUser> userManager, IConfiguration configuration, IStringLocalizer stringLocalizer)
        {
            _userManager = userManager;
            _configuration = configuration;
            _stringLocalizer = stringLocalizer;
            _userModel = new UserModel(_stringLocalizer);
        }

        /// <summary>
        /// Регистрация пользователя в системе MobileFuelling
        /// </summary>
        /// <param name="viewModel">Модель регистрации</param>
        /// <returns>Имя созданного пользователя или сообщение об ошибке</returns>
        /// <response code="200">Возвращает имя созданного пользователя</response>
        /// <response code="400">Возвращает, если модель была null</response>
        [Route("register")]
        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult> AddUser([FromBody] RegisterViewModel viewModel)
        {
            var applicationUser = ApplicationUserFactory.CreateApplicationUser(viewModel);
            return Ok(await _userModel.SaveUserAccountAsync(_userManager, applicationUser, viewModel));
        }

        /// <summary>
        /// Получение авторизационного bearer-токена
        /// </summary>
        /// <param name="viewModel">Модель входа в систему</param>
        /// <returns>Токен, срок действия или сообщение об ошибке</returns>
        /// <response code="200">Возвращает авторизационный токен и срок действия</response>
        /// <response code="400">Возвращает, если модель была null</response>
        [Route("login")]
        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult> GetUser([FromBody] LoginViewModel viewModel)
        {
            return Ok(await _userModel.GetUserAsync(_userManager, _configuration, viewModel));
        }
    }
}
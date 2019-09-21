using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using MobileFueling.Api.ApiModels;
using MobileFueling.Api.ApiModels.User;
using MobileFueling.Model;
using MobileFueling.ViewModel;
using System.Threading.Tasks;

namespace MobileFueling.Api.Controllers.v1
{
    [Produces("application/json")]
    [Route("api/v1/[controller]")]
    public class AuthController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly UserModel _userModel;
        private readonly IStringLocalizer _stringLocalizer;
        private readonly UserManager<ApplicationUser> _userManager;

        public AuthController(UserManager<ApplicationUser> userManager, IConfiguration configuration, IStringLocalizer stringLocalizer)
        {
            _configuration = configuration;
            _stringLocalizer = stringLocalizer;
            _userManager = userManager;
            _userModel = new UserModel(_stringLocalizer, _userManager);
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
        public async Task<IActionResult> AddUser([FromBody] RegisterViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var currentUser = await _userManager.GetUserAsync(User);
            var applicationUser = ApplicationUserFactory.CreateApplicationUser(viewModel);
            return Ok(await _userModel.SaveUserAccountAsync(applicationUser, currentUser, viewModel));
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
        public async Task<IActionResult> GetUser([FromBody] LoginViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(await _userModel.GetUserAsync(_configuration, viewModel));
        }
    }
}
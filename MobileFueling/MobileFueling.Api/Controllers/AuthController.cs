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

        [Route("register")]
        [HttpPost]
        public async Task<ActionResult> AddUser([FromBody] RegisterViewModel viewModel)
        {
            var applicationUser = ApplicationUserFactory.CreateApplicationUser(viewModel);
            return Ok(await _userModel.SaveUserAccountAsync(_userManager, applicationUser, viewModel));
        }

        [Route("login")]
        [HttpPost]
        public async Task<ActionResult> GetUser([FromBody] LoginViewModel viewModel)
        {
            return Ok(await _userModel.GetUserAsync(_userManager, _configuration, viewModel));
        }
    }
}
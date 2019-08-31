using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Microsoft.IdentityModel.Tokens;
using MobileFueling.Api.Common.BaseResponseResources;
using MobileFueling.Api.Common.Localization;
using MobileFueling.Api.Contract;
using MobileFueling.Api.Contract.UserData;
using MobileFueling.DB;
using MobileFueling.Model;
using MobileFueling.Model.Enums;
using MobileFueling.ViewModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MobileFueling.Api.ApiModels.User
{
    public class UserModel
    {
        private const string dateFormat = "dd.MM.yyyy";
        private readonly IStringLocalizer _stringLocalizer;
        private readonly FuelDbContext _fuelContext;

        public UserModel(IStringLocalizer stringLocalizer)
        {
            _stringLocalizer = stringLocalizer;
        }

        public UserModel(IStringLocalizer stringLocalizer, FuelDbContext fuelContext) : this(stringLocalizer)
        {
            _fuelContext = fuelContext;
        }

        #region AuthController methods
        internal async Task<RegisterResponse> SaveUserAccountAsync(UserManager<ApplicationUser> userManager, ApplicationUser applicationUser, RegisterViewModel viewModel)
        {
            var response = new RegisterResponse();
            try
            {
                var creationResult = await userManager.CreateAsync(applicationUser, viewModel.Password);
                if (creationResult.Succeeded)
                {
                    // save user's claims
                    await AddUserClaimsAsync(userManager, applicationUser, viewModel);

                    response.UserName = viewModel.Email;
                }
                else
                {
                    response.AddMessage(MessageType.ERROR, _stringLocalizer[CustomStringLocalizer.USER_NOT_CREATED]);
                }
            }
            catch (Exception ex)
            {
                response.AddException(ex);
            }
            return response;
        }

        internal async Task<LoginResponse> GetUserAsync(UserManager<ApplicationUser> userManager, IConfiguration configuration, LoginViewModel viewModel)
        {
            var response = new LoginResponse();

            try
            {
                var applicationUser = await userManager.FindByNameAsync(viewModel.Username);
                if (applicationUser == null)
                {
                    response.AddMessage(MessageType.ERROR, _stringLocalizer[CustomStringLocalizer.USERNAME_NOT_FOUND]);
                    return response;
                }

                if (!await userManager.CheckPasswordAsync(applicationUser, viewModel.Password))
                {
                    response.AddMessage(MessageType.ERROR, _stringLocalizer[CustomStringLocalizer.USER_PASSWORD_WRONG]);
                    return response;
                }

                var loginClaim = await GetUserClaimAsync(userManager, applicationUser, UserConstants.CanLogin);
                if (loginClaim == null || loginClaim.Value == "0")
                {
                    response.AddMessage(MessageType.ERROR, _stringLocalizer[CustomStringLocalizer.USER_CAN_NOT_LOGIN]);
                    return response;
                }

                // заполнение в identity клеймов и другое
                var encodedKey = Encoding.UTF8.GetBytes(configuration["Jwt:SigningKey"]);
                var signingKey = new SymmetricSecurityKey(encodedKey);
                var expireInMinutes = System.Convert.ToInt32(configuration["Jwt:ExpireInMinutes"]);
                var expireDate = DateTime.Now.AddMinutes(expireInMinutes);

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Audience = configuration["Jwt:Site"],
                    Issuer = configuration["Jwt:Site"],
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, applicationUser.Id.ToString()),
                        new Claim(ClaimTypes.Name, applicationUser.UserName)
                    }),
                    Expires = expireDate,
                    SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256)
                };
                var tokenHandler = new JwtSecurityTokenHandler();
                var token = tokenHandler.CreateToken(tokenDescriptor);

                response = new LoginResponse
                {
                    Expiration = expireDate,
                    Token = tokenHandler.WriteToken(token)
                };
            }
            catch (Exception ex)
            {
                response.AddException(ex);
            }

            return response;
        }
        
        private async Task AddUserClaimsAsync(UserManager<ApplicationUser> userManager, ApplicationUser applicationUser, RegisterViewModel viewModel)
        {
            var userClaims = new List<Claim>
                    {
                        new Claim(UserConstants.Name, viewModel.Name),
                        new Claim(UserConstants.FirstName, viewModel.FirstName),
                        new Claim(UserConstants.CanLogin, "1")
                    };

            if (!string.IsNullOrEmpty(viewModel.MiddleName))
            {
                userClaims.Add(new Claim(UserConstants.MiddleName, viewModel.MiddleName));
            }
            if (viewModel.DateOfBirth.HasValue)
            {
                userClaims.Add(new Claim(UserConstants.DateOfBirth, viewModel.DateOfBirth.Value.ToString(dateFormat)));
            }

            await userManager.AddClaimsAsync(applicationUser, userClaims);
        }

        private async Task<Claim> GetUserClaimAsync(UserManager<ApplicationUser> userManager, ApplicationUser applicationUser, string claimName)
        {
            var allUserClaims = await userManager.GetClaimsAsync(applicationUser).ConfigureAwait(false);
            return allUserClaims.FirstOrDefault(x => x.Type == claimName);
        }
        #endregion

        #region UserController methods
        public async Task<UserGetAllResponse> GetAll(UserManager<ApplicationUser> userManager, ApplicationUser currentUser, UserTypeVM userType)
        {
            var response = new UserGetAllResponse();

            var applicationUserType = GetApplicationUserType(currentUser);

            Func<IEnumerable<ApplicationUser>, Task<IEnumerable<ApplicationUserVM>>> convertFunc = async (items) =>
            {
                var tasks = items.Select(x => Convert(userManager, x));
                return await Task.WhenAll(tasks);
            };

            switch (userType)
            {
                case UserTypeVM.Admin when applicationUserType == UserType.Admin:
                    var admins = await _fuelContext.AdminUsers.ToListAsync();
                    response.Items = await convertFunc(admins);
                    break;
                case UserTypeVM.Admin when applicationUserType != UserType.Admin:
                    response.AddMessage(MessageType.ERROR, _stringLocalizer[CustomStringLocalizer.NO_RIGHTS_TO_RECEIVE_USERLIST]);
                    break;
                case UserTypeVM.Client when applicationUserType == UserType.Admin || applicationUserType == UserType.Manager:
                    var clients = await _fuelContext.ClientUsers.ToListAsync();
                    response.Items = await convertFunc(clients);
                    break;
                case UserTypeVM.Client when applicationUserType != UserType.Admin && applicationUserType != UserType.Manager:
                    response.AddMessage(MessageType.ERROR, _stringLocalizer[CustomStringLocalizer.NO_RIGHTS_TO_RECEIVE_USERLIST]);
                    break;
                case UserTypeVM.Driver when applicationUserType == UserType.Admin || applicationUserType == UserType.Manager:
                    var drivers = await _fuelContext.DriverUsers.ToListAsync();
                    response.Items = await convertFunc(drivers);
                    break;
                case UserTypeVM.Driver when applicationUserType != UserType.Admin && applicationUserType != UserType.Manager:
                    response.AddMessage(MessageType.ERROR, _stringLocalizer[CustomStringLocalizer.NO_RIGHTS_TO_RECEIVE_USERLIST]);
                    break;
                case UserTypeVM.Manager when applicationUserType == UserType.Admin:
                    var managers = await _fuelContext.ManagerUsers.ToListAsync();
                    response.Items = await convertFunc(managers);
                    break;
                case UserTypeVM.Manager when applicationUserType != UserType.Admin:
                    response.AddMessage(MessageType.ERROR, _stringLocalizer[CustomStringLocalizer.NO_RIGHTS_TO_RECEIVE_USERLIST]);
                    break;
                default:
                    throw new NotImplementedException("Not implemented user type");
            }

            return response;
        }

        public async Task<UserGetOneResponse> GetOne(UserManager<ApplicationUser> userManager, ApplicationUser currentUser, UserTypeVM userType, long id)
        {
            var response = new UserGetOneResponse();
            var applicationUserType = GetApplicationUserType(currentUser);

            ApplicationUser item = null;
            switch (userType)
            {
                case UserTypeVM.Admin when applicationUserType == UserType.Admin:
                    item = await _fuelContext.AdminUsers.FirstOrDefaultAsync(x => x.Id == id);
                    break;
                case UserTypeVM.Admin when applicationUserType != UserType.Admin:
                    response.AddMessage(MessageType.ERROR, _stringLocalizer[CustomStringLocalizer.NO_RIGHTS_TO_RECEIVE_USER]);
                    break;
                case UserTypeVM.Client when applicationUserType == UserType.Admin || applicationUserType == UserType.Manager:
                    item = await _fuelContext.ClientUsers.FirstOrDefaultAsync(x => x.Id == id);
                    break;
                case UserTypeVM.Client when applicationUserType != UserType.Admin && applicationUserType != UserType.Manager:
                    response.AddMessage(MessageType.ERROR, _stringLocalizer[CustomStringLocalizer.NO_RIGHTS_TO_RECEIVE_USER]);
                    break;
                case UserTypeVM.Driver when applicationUserType == UserType.Admin || applicationUserType == UserType.Manager:
                    item = await _fuelContext.DriverUsers.FirstOrDefaultAsync(x => x.Id == id);
                    break;
                case UserTypeVM.Driver when applicationUserType != UserType.Admin && applicationUserType != UserType.Manager:
                    response.AddMessage(MessageType.ERROR, _stringLocalizer[CustomStringLocalizer.NO_RIGHTS_TO_RECEIVE_USER]);
                    break;
                case UserTypeVM.Manager when applicationUserType == UserType.Admin:
                    item = await _fuelContext.ManagerUsers.FirstOrDefaultAsync(x => x.Id == id);
                    break;
                case UserTypeVM.Manager when applicationUserType != UserType.Admin:
                    response.AddMessage(MessageType.ERROR, _stringLocalizer[CustomStringLocalizer.NO_RIGHTS_TO_RECEIVE_USER]);
                    break;
                default:
                    throw new NotImplementedException("Not implemented user type");
            }

            if (item != null)
            {
                response.Item = await Convert(userManager, item);
            }
            else if(!response.HasError())
            {
                response.AddMessage(MessageType.ERROR, _stringLocalizer[CustomStringLocalizer.USER_NOT_FOUND]);
            }

            return response;
        }

        public async Task<UserUpdateResponse> PostOne(UserManager<ApplicationUser> userManager, ApplicationUser currentUser, UserTypeVM userType, ApplicationUserVM applicationUserVM)
        {
            var response = new UserUpdateResponse();
            var applicationUserType = GetApplicationUserType(currentUser);

            if (applicationUserType != UserType.Admin && applicationUserType != UserType.Manager) // редактировать может только админ или менеджер
            {
                response.AddMessage(MessageType.ERROR, _stringLocalizer[CustomStringLocalizer.NO_RIGHTS_TO_ADD_OR_UPDATE_USER]);
                return response;
            }

            if (applicationUserType == UserType.Manager && userType == UserTypeVM.Admin) // менеджер хочет редактировать админа
            {
                response.AddMessage(MessageType.ERROR, _stringLocalizer[CustomStringLocalizer.NO_RIGHTS_TO_ADD_OR_UPDATE_USER]);
                return response;
            }

            ApplicationUser applicationUserValue = null;
            if (applicationUserVM.Id.HasValue)
            {
                applicationUserValue = await ApplicationUserFactory.GetApplicationUserAsync(_fuelContext, userType, applicationUserVM.Id.Value);
            }

            if (applicationUserValue == null)
            {
                response.AddMessage(MessageType.ERROR, _stringLocalizer[CustomStringLocalizer.USER_NOT_FOUND]);
                return response;
            }

            applicationUserValue.Email = applicationUserVM.Email;
            applicationUserValue.UserName = applicationUserVM.Email;

            var result = await userManager.UpdateAsync(applicationUserValue);
            if (!result.Succeeded)
            {
                response.AddMessage(MessageType.ERROR, _stringLocalizer[CustomStringLocalizer.USER_NOT_FOUND]);
                return response;
            }

            await UpdateUserClaimsAsync(userManager, applicationUserVM, applicationUserValue).ConfigureAwait(false);

            response.Id = applicationUserValue.Id;
            return response;
        }

        public async Task<UserDeleteResponse> DeleteOne(UserManager<ApplicationUser> userManager, ApplicationUser currentUser, UserTypeVM userType, long id)
        {
            var response = new UserDeleteResponse();
            var applicationUserType = GetApplicationUserType(currentUser);

            if (applicationUserType != UserType.Admin && applicationUserType != UserType.Manager) // удалить может только админ или менеджер
            {
                response.AddMessage(MessageType.ERROR, _stringLocalizer[CustomStringLocalizer.NO_RIGHTS_TO_DELETE_USER]);
                return response;
            }

            if (applicationUserType == UserType.Manager && userType == UserTypeVM.Admin) // менеджер хочет удалить админа
            {
                response.AddMessage(MessageType.ERROR, _stringLocalizer[CustomStringLocalizer.NO_RIGHTS_TO_DELETE_USER]);
                return response;
            }

            var applicationUserValue = await ApplicationUserFactory.GetApplicationUserAsync(_fuelContext, userType, id);
            if (applicationUserValue == null)
            {
                response.AddMessage(MessageType.ERROR, _stringLocalizer[CustomStringLocalizer.USER_NOT_FOUND]);
                return response;
            }

            var result = await userManager.DeleteAsync(applicationUserValue);
            response.IsSuccess = result.Succeeded;
            return response;
        }

        private async Task UpdateUserClaimsAsync(UserManager<ApplicationUser> userManager, ApplicationUserVM applicationUserVM, ApplicationUser applicationUser)
        {
            await UpdateUserClaimAsync(userManager, applicationUser, UserConstants.Name, applicationUserVM.Name).ConfigureAwait(false);

            await UpdateUserClaimAsync(userManager, applicationUser, UserConstants.FirstName, applicationUserVM.FirstName).ConfigureAwait(false);

            await UpdateUserClaimAsync(userManager, applicationUser, UserConstants.MiddleName, applicationUserVM.MiddleName).ConfigureAwait(false);

            await UpdateUserClaimAsync(userManager, applicationUser, UserConstants.CanLogin, applicationUserVM.CanLogin ? "1" : "0").ConfigureAwait(false);

            await UpdateUserClaimAsync(userManager, applicationUser, UserConstants.DateOfBirth, applicationUserVM.DateOfBirth?.ToString(dateFormat)).ConfigureAwait(false);
        }

        private async Task UpdateUserClaimAsync(UserManager<ApplicationUser> userManager, ApplicationUser applicationUser, string type, string value)
        {
            var claim = await GetUserClaimAsync(userManager, applicationUser, type);
            if (claim != null && claim.Value != value)
            {
                await ReplaceUserClaimAsync(userManager, applicationUser, claim, new Claim(type, value));
            }
            else
            {
                await AddUserClaimAsync(userManager, applicationUser, new Claim(type, value));
            }
        }

        private async Task AddUserClaimAsync(UserManager<ApplicationUser> userManager, ApplicationUser applicationUser, Claim claim)
        {
            await userManager.AddClaimAsync(applicationUser, claim).ConfigureAwait(false);
        }

        private async Task ReplaceUserClaimAsync(UserManager<ApplicationUser> userManager, ApplicationUser applicationUser, Claim oldClaim, Claim newClaim)
        {
            await userManager.ReplaceClaimAsync(applicationUser, oldClaim, newClaim).ConfigureAwait(false);
        }

        private UserType GetApplicationUserType(ApplicationUser applicationUser)
        {
            if (applicationUser is Driver)
            {
                return UserType.Driver;
            }
            else if (applicationUser is Client)
            {
                return UserType.Client;
            }
            else if (applicationUser is Manager)
            {
                return UserType.Manager;
            }
            else if (applicationUser is SystemAdmin)
            {
                return UserType.Admin;
            }
            throw new NotImplementedException("Not implemented user type");
        }

        private async Task<ApplicationUserVM> Convert(UserManager<ApplicationUser> userManager, ApplicationUser applicationUser)
        {
            if (applicationUser == null)
                return null;

            var claims = await GetUserClaimsAsync(userManager, applicationUser);
            var birthdayClaim = claims?.FirstOrDefault(x => x.Type == UserConstants.DateOfBirth)?.Value;
            return new ApplicationUserVM
            {
                Id = applicationUser.Id,
                Email = applicationUser.Email,
                PhoneNumber = applicationUser.PhoneNumber,
                Name = claims?.FirstOrDefault(x => x.Type == UserConstants.Name)?.Value,
                FirstName = claims?.FirstOrDefault(x => x.Type == UserConstants.FirstName)?.Value,
                MiddleName = claims?.FirstOrDefault(x => x.Type == UserConstants.MiddleName)?.Value,
                UserType = (UserTypeVM)GetApplicationUserType(applicationUser),
                CanLogin = claims?.FirstOrDefault(x => x.Type == UserConstants.CanLogin)?.Value == "1",
                DateOfBirth = !string.IsNullOrEmpty(birthdayClaim) ? DateTime.ParseExact(birthdayClaim, dateFormat, CultureInfo.InvariantCulture) : (DateTime?)null
            };
        }

        private async Task<IEnumerable<Claim>> GetUserClaimsAsync(UserManager<ApplicationUser> userManager, ApplicationUser applicationUser)
        {
            return await userManager.GetClaimsAsync(applicationUser).ConfigureAwait(false);
        }
        #endregion
    }
}
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Microsoft.IdentityModel.Tokens;
using MobileFueling.Api.Common.BaseResponseResources;
using MobileFueling.Api.Common.Localization;
using MobileFueling.Api.Contract;
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
        public async Task<IEnumerable<ApplicationUserVM>> GetAll(UserManager<ApplicationUser> userManager, ApplicationUser currentUser, UserTypeVM userType)
        {
            var applicationUserType = GetApplicationUserType(currentUser);

            Func<IEnumerable<ApplicationUser>, Task<IEnumerable<ApplicationUserVM>>> convertFunc = async (items) =>
            {
                var tasks = items.Select(x => Convert(userManager, x));
                return await Task.WhenAll(tasks);
            };

            switch (userType)
            {
                case UserTypeVM.Admin:
                    if (applicationUserType == UserType.Admin)
                    {
                        var items = await _fuelContext.AdminUsers.ToListAsync();
                        return await convertFunc(items);
                    }
                    else
                    {
                        throw new AccessViolationException(_stringLocalizer[CustomStringLocalizer.NO_RIGTHS_TO_RECEIVE_USERLIST]);
                    }
                case UserTypeVM.Client:
                    if (applicationUserType == UserType.Admin || applicationUserType == UserType.Manager)
                    {
                        var items = await _fuelContext.ClientUsers.ToListAsync();
                        return await convertFunc(items);
                    }
                    else
                    {
                        throw new AccessViolationException(_stringLocalizer[CustomStringLocalizer.NO_RIGTHS_TO_RECEIVE_USERLIST]);
                    }
                case UserTypeVM.Driver:
                    if (applicationUserType == UserType.Admin || applicationUserType == UserType.Manager)
                    {
                        var items = await _fuelContext.DriverUsers.ToListAsync();
                        return await convertFunc(items);
                    }
                    else
                    {
                        throw new AccessViolationException(_stringLocalizer[CustomStringLocalizer.NO_RIGTHS_TO_RECEIVE_USERLIST]);
                    }
                case UserTypeVM.Manager:
                    if (applicationUserType == UserType.Admin)
                    {
                        var items = await _fuelContext.ManagerUsers.ToListAsync();
                        return await convertFunc(items);
                    }
                    else
                    {
                        throw new AccessViolationException(_stringLocalizer[CustomStringLocalizer.NO_RIGTHS_TO_RECEIVE_USERLIST]);
                    }
            }
            throw new NotImplementedException("Not implemented user type");
        }

        public async Task<ApplicationUserVM> GetOne(UserManager<ApplicationUser> userManager, ApplicationUser currentUser, UserTypeVM userType, long id)
        {
            var applicationUserType = GetApplicationUserType(currentUser);

            ApplicationUser item = null;
            switch (userType)
            {
                case UserTypeVM.Admin:
                    if (applicationUserType == UserType.Admin)
                    {
                        item = await _fuelContext.AdminUsers.FirstOrDefaultAsync(x => x.Id == id);
                    }
                    else
                    {
                        throw new AccessViolationException(_stringLocalizer[CustomStringLocalizer.NO_RIGTHS_TO_RECEIVE_USERLIST]);
                    }
                    break;
                case UserTypeVM.Client:
                    if (applicationUserType == UserType.Admin || applicationUserType == UserType.Manager)
                    {
                        item = await _fuelContext.ClientUsers.FirstOrDefaultAsync(x => x.Id == id);
                    }
                    else
                    {
                        throw new AccessViolationException(_stringLocalizer[CustomStringLocalizer.NO_RIGTHS_TO_RECEIVE_USERLIST]);
                    }
                    break;
                case UserTypeVM.Driver:
                    if (applicationUserType == UserType.Admin || applicationUserType == UserType.Manager)
                    {
                        item = await _fuelContext.DriverUsers.FirstOrDefaultAsync(x => x.Id == id);
                    }
                    else
                    {
                        throw new AccessViolationException(_stringLocalizer[CustomStringLocalizer.NO_RIGTHS_TO_RECEIVE_USERLIST]);
                    }
                    break;
                case UserTypeVM.Manager:
                    if (applicationUserType == UserType.Admin)
                    {
                        item = await _fuelContext.ManagerUsers.FirstOrDefaultAsync(x => x.Id == id);
                    }
                    else
                    {
                        throw new AccessViolationException(_stringLocalizer[CustomStringLocalizer.NO_RIGTHS_TO_RECEIVE_USERLIST]);
                    }
                    break;
            }

            if (item != null)
            {
                return await Convert(userManager, item);
            }
            throw new NotImplementedException("Not implemented user type");
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
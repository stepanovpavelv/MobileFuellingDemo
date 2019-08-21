using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Microsoft.IdentityModel.Tokens;
using MobileFueling.Api.Common.BaseResponseResources;
using MobileFueling.Api.Common.Localization;
using MobileFueling.Api.Contract;
using MobileFueling.Model;
using MobileFueling.Model.Enums;
using MobileFueling.ViewModel;

namespace MobileFueling.Api.ApiModels.User
{
    public class UserModel
    {
        private IStringLocalizer _stringLocalizer;

        public UserModel(IStringLocalizer stringLocalizer)
        {
            _stringLocalizer = stringLocalizer;
        }

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
                ApplicationUser applicationUser = await userManager.FindByNameAsync(viewModel.Username);
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
                var expireInMinutes = Convert.ToInt32(configuration["Jwt:ExpireInMinutes"]);
                var expireDate = DateTime.Now.AddMinutes(expireInMinutes);

                var token = new JwtSecurityToken(
                    issuer: configuration["Jwt:Site"],
                    audience: configuration["Jwt:Site"],
                    expires: expireDate,
                    signingCredentials: new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256)
                );

                response = new LoginResponse
                {
                    Expiration = expireDate,
                    Token = token
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
                userClaims.Add(new Claim(UserConstants.DateOfBirth, viewModel.DateOfBirth.Value.ToString("dd.MM.yyyy")));
            }

            await userManager.AddClaimsAsync(applicationUser, userClaims);
        }

        private async Task<Claim> GetUserClaimAsync(UserManager<ApplicationUser> userManager, ApplicationUser applicationUser, string claimName)
        {
            var allUserClaims = await userManager.GetClaimsAsync(applicationUser).ConfigureAwait(false);
            return allUserClaims.FirstOrDefault(x => x.Type == claimName);
        }
    }
}
using MobileFueling.Api.Common.BaseResponseResources;
using System;
using System.IdentityModel.Tokens.Jwt;

namespace MobileFueling.Api.Contract
{
    public class LoginResponse : BaseResponse
    {
        public JwtSecurityToken Token { get; set; }

        public DateTime Expiration { get; set; }
    }
}
using MobileFueling.Api.Common.BaseResponseResources;
using MobileFueling.ViewModel;
using System;
using System.IdentityModel.Tokens.Jwt;

namespace MobileFueling.Api.Contract
{
    public class LoginResponse : BaseResponse
    {
        public string Token { get; set; }

        public DateTime Expiration { get; set; }
    }
}
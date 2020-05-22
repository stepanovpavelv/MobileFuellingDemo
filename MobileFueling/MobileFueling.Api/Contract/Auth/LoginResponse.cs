using MobileFueling.Api.Common.BaseResponseResources;
using System;

namespace MobileFueling.Api.Contract.Auth
{
    public class LoginResponse : BaseResponse
    {
        public string Token { get; set; }

        public DateTime Expiration { get; set; }
    }
}
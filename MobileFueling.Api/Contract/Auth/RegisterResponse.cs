using MobileFueling.Api.Common.BaseResponseResources;

namespace MobileFueling.Api.Contract.Auth
{
    public class RegisterResponse : BaseResponse
    {
        public string UserName { get; set; }
    }
}
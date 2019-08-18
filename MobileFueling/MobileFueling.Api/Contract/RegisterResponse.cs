using MobileFueling.Api.Common.BaseResponseResources;

namespace MobileFueling.Api.Contract
{
    public class RegisterResponse : BaseResponse
    {
        public string UserName { get; set; }
    }
}
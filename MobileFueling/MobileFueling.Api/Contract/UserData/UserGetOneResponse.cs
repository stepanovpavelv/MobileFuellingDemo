using MobileFueling.Api.Common.BaseResponseResources;
using MobileFueling.ViewModel;

namespace MobileFueling.Api.Contract.UserData
{
    public class UserGetOneResponse : BaseResponse
    {
        public ApplicationUserVM Item { get; set; }
    }
}
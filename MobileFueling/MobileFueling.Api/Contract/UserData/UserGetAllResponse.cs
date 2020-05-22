using MobileFueling.Api.Common.BaseResponseResources;
using MobileFueling.ViewModel;
using System.Collections.Generic;

namespace MobileFueling.Api.Contract.UserData
{
    public class UserGetAllResponse : BaseResponse
    {
        public IEnumerable<ApplicationUserVM> Items { get; set; }
    }
}
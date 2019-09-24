using Microsoft.AspNetCore.Identity;

namespace MobileFueling.Model
{
    public class ApplicationUser : IdentityUser<long>, IEntity
    {
    }
    public class ApplicationRole : IdentityRole<long> { }
    public class ApplicationUserClaim : IdentityUserClaim<long> { }
    public class ApplicationUserRole : IdentityUserRole<long> { }
    public class ApplicationUserLogin : IdentityUserLogin<long> { }
    public class ApplicationRoleClaim : IdentityRoleClaim<long> { }
    public class ApplicationUserToken : IdentityUserToken<long> { }
}
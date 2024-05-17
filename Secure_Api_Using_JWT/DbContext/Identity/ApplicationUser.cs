using Microsoft.AspNetCore.Identity;

namespace Secure_Api_Using_JWT.DbContext.Identity
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string lastName { get; set; }
    }
}

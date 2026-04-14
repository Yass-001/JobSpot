using Microsoft.AspNetCore.Identity;

namespace JobSpot.Data
{
    public class ApplicationUserRole : IdentityUserRole<string>
    {
        public string UserName { get; set; }
    }
}

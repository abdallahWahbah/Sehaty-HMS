using Microsoft.AspNetCore.Identity;

namespace Sehaty.Core.Entities.User_Entities
{
    public class ApplicationRole : IdentityRole<int>
    {
        public string Description { get; set; }

        public List<ApplicationUser> Users { get; set; }
    }
}

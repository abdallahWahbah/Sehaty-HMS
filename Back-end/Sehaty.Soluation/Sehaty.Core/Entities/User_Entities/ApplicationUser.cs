using Microsoft.AspNetCore.Identity;
using Sehaty.Core.Entites;

namespace Sehaty.Core.Entities.User_Entities
{
    public class ApplicationUser : IdentityUser<int>
    {
        public enum LanguagePreferenceEnum
        {
            Arabic,
            English
        }
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public LanguagePreferenceEnum LanguagePreference { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime LastLogin { get; set; }

        public List<Notification> Notifications { get; set; }
        public int RoleId { get; set; }
        public ApplicationRole Role { get; set; }

    }
}

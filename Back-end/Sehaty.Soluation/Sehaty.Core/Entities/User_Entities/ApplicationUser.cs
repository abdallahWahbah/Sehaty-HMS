using Microsoft.AspNetCore.Identity;
using Sehaty.Core.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sehaty.Core.Entities.User_Entities
{
    public class ApplicationUser : IdentityUser
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

        public List<Notification> Notifications { get; set; } = new();
    }
}

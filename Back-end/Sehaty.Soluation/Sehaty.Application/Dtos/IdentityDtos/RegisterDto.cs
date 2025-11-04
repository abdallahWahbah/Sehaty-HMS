using System.ComponentModel.DataAnnotations;
using static Sehaty.Core.Entities.User_Entities.ApplicationUser;

namespace Sehaty.Application.Dtos.IdentityDtos
{
    public class RegisterDto
    {
        [Required]
        public string UserName { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required]
        public string PhoneNumber { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string ConfirmPassword { get; set; }

        [Required]
        public LanguagePreferenceEnum LanguagePreference { get; set; }

    }
}

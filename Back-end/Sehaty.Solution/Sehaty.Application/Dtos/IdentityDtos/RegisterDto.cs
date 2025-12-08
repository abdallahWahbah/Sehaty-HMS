namespace Sehaty.Application.Dtos.IdentityDtos
{
    public class RegisterDto
    {
        [Required]
        public string UserName { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required]
        [RegularExpression(@"^\+20(10|11|12|15)\d{8}$",
            ErrorMessage = "Phone number must be a valid Egyptian number, e.g. +201012345678")]
        public string PhoneNumber { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }

        [Required]
        public LanguagePreferenceEnum LanguagePreference { get; set; }

    }
}

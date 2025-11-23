namespace Sehaty.Core.Entities.User_Entities
{
    public enum LanguagePreferenceEnum
    {
        Arabic,
        English
    }
    public class ApplicationUser : IdentityUser<int>
    {

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public LanguagePreferenceEnum LanguagePreference { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? LastLogin { get; set; }

        public List<Notification> Notifications { get; set; }
        public List<RefreshToken> RefreshTokens { get; set; }

    }
}

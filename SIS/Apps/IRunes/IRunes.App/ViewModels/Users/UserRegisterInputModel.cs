using SIS.MvcFramework.Attributes.Validation;

namespace IRunes.App.ViewModels.Users
{
    public class UserRegisterInputModel
    {
        [RequiredSis]
        [StringLengthSis(5, 20)]
        public string Username { get; set; }

        [RequiredSis]
        [PasswordSis(3, nameof(ConfirmPassword))]
        public string Password { get; set; }

        [RequiredSis]
        public string ConfirmPassword { get; set; }

        [RequiredSis]
        [EmailSis]
        public string Email { get; set; }
    }
}
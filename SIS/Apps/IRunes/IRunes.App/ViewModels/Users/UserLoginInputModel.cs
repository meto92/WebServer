using SIS.MvcFramework.Attributes.Validation;

namespace IRunes.App.ViewModels.Users
{
    public class UserLoginInputModel
    {
        [StringLengthSis(3, 20)]
        public string UsernameOrEmail { get; set; }

        [StringLengthSis(3, 256)]
        public string Password { get; set; }
    }
}
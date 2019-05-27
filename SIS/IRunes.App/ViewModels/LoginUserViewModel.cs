namespace IRunes.ViewModels
{
    public class LoginUserViewModel
    {
        public LoginUserViewModel(string usernameOrEmail, string password)
        {
            this.UsernameOrEmail = usernameOrEmail;
            this.Password = password;
        }

        public string UsernameOrEmail { get; set; }

        public string Password { get; set; }
    }
}
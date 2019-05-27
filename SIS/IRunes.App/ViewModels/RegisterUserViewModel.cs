namespace IRunes.ViewModels
{
    public class RegisterUserViewModel
    {
        public RegisterUserViewModel(
            string username, 
            string password, 
            string confirmPassword,
            string email)
        {
            this.Username = username;
            this.Password = password;
            this.Confirmpassword = confirmPassword;
            this.Email = email;
        }

        public string Username { get; set; }

        public string Password { get; set; }

        public string Confirmpassword { get; set; }

        public string Email { get; set; }
    }
}
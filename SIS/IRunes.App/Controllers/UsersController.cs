using IRunes.Models;
using IRunes.Services;
using IRunes.ViewModels;

using SIS.MvcFramework;
using SIS.MvcFramework.Attributes.Methods;
using SIS.MvcFramework.Results;

namespace IRunes.Controllers
{
    public class UsersController : Controller
    {
        private const int MinUsernameLength = 3;
        private const int MinPasswordLength = 4;

        private readonly IUserService userService;
        private readonly IHashService hashService;

        public UsersController()
        {
            this.userService = new UserService();
            this.hashService = new HashService();
        }

        public ActionResult Login()
        {
            SignOut();

            return View();
        }

        private ActionResult ValidateUsernameAndPassword(string username, string password)
        {
            if (username.Length < MinUsernameLength)
            {
                return base.BadRequestError($"Username must contain at least {MinUsernameLength} characters.");
            }

            if (password.Length < MinUsernameLength)
            {
                return base.BadRequestError($"Password must contain at least {MinPasswordLength} characters.");
            }

            return null;
        }

        [HttpPost(ActionName = nameof(Login))]
        public ActionResult PostLogin(LoginUserViewModel model)
        {
            model.UsernameOrEmail = model.UsernameOrEmail.Trim();

            ActionResult responseForValidation = this.ValidateUsernameAndPassword(model.UsernameOrEmail, model.Password);

            if (responseForValidation != null)
            {
                return responseForValidation;
            }

            User user = this.userService.Get(model.UsernameOrEmail, this.hashService.Hash(model.Password));

            if (user == null)
            {
                return base.Redirect("/Users/Register");
            }

            Request.Session.ClearParameters();
            Request.Session.AddParameter("username", user.Username);

            SignIn(user.Id, user.Username, user.Email);

            ActionResult response = base.Redirect("/");

            //HttpCookie authCookie = new HttpCookie(UserAuthCookieKey, UserCookieService.GetEncryptedUsername(model.UsernameOrEmail));

            //response.AddCookie(authCookie);

            return response;
        }

        public ActionResult Register()
        {
            if (IsLoggedIn())
            {
                Request.Session.ClearParameters();
            }

            return View();
        }

        [HttpPost(ActionName = nameof(Register))]
        public ActionResult PostRegister(RegisterUserViewModel model)
        {
            model.Username = model.Username.Trim();
            
            ActionResult responseForValidation = this.ValidateUsernameAndPassword(model.Username, model.Password);

            if (responseForValidation != null)
            {
                return responseForValidation;
            }

            if (model.Password != model.ConfirmPassword
                || model.Email.Length < 5)
            {
                return Redirect("/Users/Register");
            }

            bool exists = this.userService.Exists(model.Username);

            if (exists)
            {
                return base.BadRequestError("Username already taken.");
            }

            bool created = this.userService.Create(model.Username, this.hashService.Hash(model.Password), model.Email);

            if (!created)
            {
                return base.ServerError("An error occurred while creating the account.");
            }

            return base.Redirect("/Users/Login");
        }

        public ActionResult Logout()
        {
            SignOut();

            return Redirect("/");
        }
    }
}
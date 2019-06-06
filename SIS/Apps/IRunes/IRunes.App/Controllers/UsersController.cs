using IRunes.App.ViewModels;
using IRunes.Models;
using IRunes.Services;

using SIS.MvcFramework;
using SIS.MvcFramework.Attributes.Action;
using SIS.MvcFramework.Attributes.Methods;
using SIS.MvcFramework.Results;

namespace IRunes.App.Controllers
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

        [NonAction]
        private ActionResult ValidateUsernameAndPassword(string username, string password)
        {
            if (username.Length < MinUsernameLength)
            {
                return BadRequestError($"Username must contain at least {MinUsernameLength} characters.");
            }

            if (password.Length < MinUsernameLength)
            {
                return BadRequestError($"Password must contain at least {MinPasswordLength} characters.");
            }

            return null;
        }

        [HttpPost(ActionName = nameof(Login))]
        public ActionResult PostLogin(UserLoginViewModel model)
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
                return Redirect("/Users/Register");
            }

            Request.Session.ClearParameters();
            Request.Session.AddParameter("username", user.Username);

            SignIn(user.Id, user.Username, user.Email);

            return Redirect("/");
        }

        public ActionResult Register()
        {
            SignOut();

            return View();
        }

        [HttpPost(ActionName = nameof(Register))]
        public ActionResult PostRegister(UserRegisterViewModel model)
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
                return BadRequestError("Username already taken.");
            }

            bool created = this.userService.Create(model.Username, this.hashService.Hash(model.Password), model.Email);

            if (!created)
            {
                return ServerError("An error occurred while creating the account.");
            }

            return Redirect("/Users/Login");
        }

        public ActionResult Logout()
        {
            SignOut();

            return Redirect("/");
        }
    }
}
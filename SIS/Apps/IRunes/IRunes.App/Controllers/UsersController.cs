using IRunes.App.ViewModels.Users;
using IRunes.Models;
using IRunes.Services;

using SIS.HTTP.Enums;
using SIS.MvcFramework;
using SIS.MvcFramework.Attributes.Methods;
using SIS.MvcFramework.Identity;
using SIS.MvcFramework.Results;

namespace IRunes.App.Controllers
{
    public class UsersController : Controller
    {
        private const string LoginFailedMessage = "Login failed.";
        private const string InvalidUsernameOrPasswordMessage = "Invalid username or password.";
        private const string RegistrationFailedMessage = "Registration failed";
        private const string UsernameAlreadyTakenMessage = "Username already taken.";

        private readonly IUserService userService;
        private readonly IHashService hashService;

        public UsersController(IUserService userService, IHashService hashService)
        {
            this.userService = userService;
            this.hashService = hashService;
        }

        public IActionResult Login()
        {
            SignOut();

            return View();
        }

        [HttpPost]
        public IActionResult Login(UserLoginInputModel model)
        {
            model.UsernameOrEmail = model.UsernameOrEmail.Trim();

            if (!ModelState.IsValid)
            {
                return View();
            }

            User user = this.userService.Get(model.UsernameOrEmail, this.hashService.Hash(model.Password));

            if (user == null)
            {
                ModelState.AddErrorMessage(LoginFailedMessage, InvalidUsernameOrPasswordMessage);

                return View();
            }

            SignIn(user.Id, user.Username, user.Email, Role.User);

            return Redirect("/");
        }

        public IActionResult Register()
        {
            SignOut();

            return View();
        }

        [HttpPost]
        public IActionResult Register(UserRegisterInputModel model)
        {
            model.Username = model.Username.Trim();

            if (!ModelState.IsValid)
            {
                return View();
            }

            bool exists = this.userService.Exists(model.Username);

            if (exists)
            {
                ModelState.AddErrorMessage(RegistrationFailedMessage, UsernameAlreadyTakenMessage);

                return View(new object(), "/Users/Register", HttpResponseStatusCode.BadRequest);
            }

            bool created = this.userService.Create(model.Username, this.hashService.Hash(model.Password), model.Email);

            if (!created)
            {
                return View(new object(), "/Users/Register", HttpResponseStatusCode.InternalServerError);
            }

            return Redirect("/Users/Login");
        }

        public IActionResult Logout()
        {
            SignOut();

            return Redirect("/");
        }
    }
}
using IRunes.Models;
using IRunes.ViewModels;

using SIS.HTTP.Cookies;
using SIS.HTTP.Responses;
using SIS.MvcFramework.Attributes.Methods;

namespace IRunes.Controllers
{
    public class UsersController : Controller
    {
        private const int MinUsernameLength = 3;
        private const int MinPasswordLength = 4;

        public IHttpResponse Login()
        {
            Request.Session.ClearParameters();

            return View();
        }

        private IHttpResponse ValidateUsernameAndPassword(string username, string password)
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
        public IHttpResponse PostLogin(LoginUserViewModel model)
        {
            model.UsernameOrEmail = model.UsernameOrEmail.Trim();

            IHttpResponse responseForValidation = this.ValidateUsernameAndPassword(model.UsernameOrEmail, model.Password);

            if (responseForValidation != null)
            {
                return responseForValidation;
            }

            User user = base.UserService.Get(model.UsernameOrEmail, base.HashService.Hash(model.Password));

            if (user == null)
            {
                return base.Redirect("/Users/Register");
            }

            Request.Session.ClearParameters();
            Request.Session.AddParameter("username", user.Username);

            IHttpResponse response = base.Redirect("/");

            //HttpCookie authCookie = new HttpCookie(UserAuthCookieKey, UserCookieService.GetEncryptedUsername(model.UsernameOrEmail));

            //response.AddCookie(authCookie);

            return response;
        }

        public IHttpResponse Register()
        {
            if (IsLoggedIn())
            {
                Request.Session.ClearParameters();
            }

            return View();
        }

        [HttpPost(ActionName = nameof(Register))]
        public IHttpResponse PostRegister(RegisterUserViewModel model)
        {
            model.Username = model.Username.Trim();
            
            IHttpResponse responseForValidation = this.ValidateUsernameAndPassword(model.Username, model.Password);

            if (responseForValidation != null)
            {
                return responseForValidation;
            }

            if (model.Password != model.ConfirmPassword
                || model.Email.Length < 5)
            {
                return Redirect("/Users/Register");
            }

            bool exists = base.UserService.Exists(model.Username);

            if (exists)
            {
                return base.BadRequestError("Username already taken.");
            }

            bool created = base.UserService.Create(model.Username, base.HashService.Hash(model.Password), model.Email);

            if (!created)
            {
                return base.ServerError("An error occurred while creating the account.");
            }

            return base.Redirect("/Users/Login");
        }

        public IHttpResponse Logout()
        {
            IHttpResponse response = base.Redirect("/");

            //HttpCookie authCookie = req.Cookies.GetCookie(UserAuthCookieKey);

            //if (authCookie != null)
            //{
            //    authCookie.Delete();
            //    response.AddCookie(authCookie);
            //}

            Request.Session.ClearParameters();

            return response;
        }
    }
}
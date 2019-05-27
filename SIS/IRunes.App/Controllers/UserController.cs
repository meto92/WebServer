﻿using IRunes.Models;
using IRunes.ViewModels;

using SIS.HTTP.Cookies;
using SIS.HTTP.Requests;
using SIS.HTTP.Responses;

namespace IRunes.Controllers
{
    public class UserController : Controller
    {
        private const int MinUsernameLength = 3;
        private const int MinPasswordLength = 4;

        public UserController(IHttpRequest request) 
            : base(request)
        { }

        public IHttpResponse GetLogin()
        {
            Request.Session.ClearParameters();

            return View("user/login", false);
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

        public IHttpResponse GetRegister()
        {
            if (IsLoggedIn())
            {
                Request.Session.ClearParameters();
            }

            return View("user/register", false);
        }

        public IHttpResponse PostRegister(RegisterUserViewModel model)
        {
            model.Username = model.Username.Trim();
            
            IHttpResponse responseForValidation = this.ValidateUsernameAndPassword(model.Username, model.Password);

            if (responseForValidation != null)
            {
                return responseForValidation;
            }

            if (model.Password != model.Confirmpassword
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
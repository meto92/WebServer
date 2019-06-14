using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

using SIS.HTTP.Enums;
using SIS.HTTP.Requests;
using SIS.MvcFramework.Extensions;
using SIS.MvcFramework.Identity;
using SIS.MvcFramework.Results;
using SIS.MvcFramework.Validation;
using SIS.MvcFramework.ViewEngine;

namespace SIS.MvcFramework
{
    public abstract class Controller
    {
        private const string AppDir = "../../../";
        private const string LayoutPath = AppDir + "views/_layout.html";
        private const string TypeNotEnumMessage = "The given type is not enum.";

        private readonly IViewEngine viewEngine;

        protected Controller()
        {
            this.viewEngine = new SisViewEngine();
            this.ModelState = new ModelStateDictionary();
        }

        public IHttpRequest Request { get; set; }

        public ModelStateDictionary ModelState { get; set; }

        public Principal User
            => this.Request.Session.GetParameter<Principal>("principal");

        protected void SignIn<TRoleEnum>(string id, string username, string email, TRoleEnum role)
            where TRoleEnum : struct
        {
            if (!typeof(TRoleEnum).IsEnum)
            {
                throw new ArgumentException(TypeNotEnumMessage);
            }

            List<string>  roles = Enum.GetValues(typeof(TRoleEnum))
                .Cast<TRoleEnum>()
                .Where(r => (int) Enum.Parse(typeof(TRoleEnum), r.ToString()) <= (int) Enum.Parse(typeof(TRoleEnum), role.ToString()))
                .Select(r => r.ToString())
                .ToList();

            Principal user = new Principal
            {
                Id = id,
                Username = username,
                Email = email,
                Roles = roles
            };

            this.Request.Session.AddParameter("principal", user);
        }

        protected void SignOut()
            => this.Request.Session.ClearParameters();

        private string GetFileContent(string filePath)
            => System.IO.File.ReadAllText(filePath);

        protected IActionResult View<T>(T model, string htmlFilePath, HttpResponseStatusCode httpResponseStatusCode = HttpResponseStatusCode.OK)
            where T : class
        {
            string layout = this.GetFileContent(LayoutPath);
            string content = this.GetFileContent($"{AppDir}views/{htmlFilePath}.html");

            string html = layout.Replace("@RenderBody()", content);

            html = this.viewEngine.GetHtml(html, model, this.ModelState, this.User);

            return new HtmlResult(html, httpResponseStatusCode);
        }

        protected IActionResult View<T>(T model, [CallerMemberName]string action = "")
            where T : class
        {
            string fullControllerName = this.GetType().Name;
            string controllerName = fullControllerName.Substring(0, fullControllerName.Length - nameof(Controller).Length);

            return View(model, $"{controllerName}/{action}", HttpResponseStatusCode.OK);
        }

        protected IActionResult View([CallerMemberName]string action = "")
        {
            string fullControllerName = this.GetType().Name;
            string controllerName = fullControllerName.Substring(0, fullControllerName.Length - nameof(Controller).Length);

            return View<object>(null, $"{controllerName}/{action}", HttpResponseStatusCode.OK);
        }

        protected string GetUsername()
            => this.User?.Username;

        protected bool IsLoggedIn()
            => this.User != null;

        protected IActionResult Redirect(string location)
            => new RedirectResult(location);

        protected IActionResult Xml(object obj)
            => new XmlResult(obj.ToXml());

        protected IActionResult Json(object obj)
            => new JsonResult(obj.ToJson());

        protected IActionResult File(byte[] fileContent)
            => new FileResult(fileContent);

        protected IActionResult NotFound(string message)
            => new NotFoundResult(message);
    }
}
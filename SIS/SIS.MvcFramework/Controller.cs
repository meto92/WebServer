using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;

using SIS.MvcFramework.Results;
using SIS.HTTP.Enums;
using SIS.HTTP.Requests;
using SIS.HTTP.Responses;
using SIS.MvcFramework.Extensions;
using SIS.MvcFramework.Identity;

namespace SIS.MvcFramework
{
    public abstract class Controller
    {
        private const string AppDir = "../../../";
        private const string LayoutPath = AppDir + "views/shared/_layout.html";
        private const string GuestNavPath = AppDir + "views/shared/guestNav.html";
        private const string LoggedInUserNavPath = AppDir + "views/shared/loggedInUserNav.html";

        protected const string UserAuthCookieKey = ".auth-IRunes";

        protected Controller()
        {
            this.ViewData = new Dictionary<string, string>();            
        }

        public IHttpRequest Request { get; set; }

        protected Dictionary<string, string> ViewData { get; }

        public Principal User
            => this.Request.Session.GetParameter<Principal>("principal");

        protected void SignIn(string id, string username, string email)
        {
            Principal user = new Principal
            {
                Id = id,
                Username = username,
                Email = email
            };

            this.Request.Session.AddParameter("principal", user);
        }

        protected void SignOut()
            => this.Request.Session.ClearParameters();

        private void SetPlaceholdersContent(ref string html)
        {
            foreach (KeyValuePair<string, string> pair in this.ViewData)
            {
                (string key, string value) = (pair.Key, pair.Value);

                html = html.Replace($"{{{{{{{key}}}}}}}", value);
            }
        }

        private string GetFileContent(string filePath)
            => System.IO.File.ReadAllText(filePath);

        protected ActionResult View(string htmlFilePath, bool loggedIn = true, HttpResponseStatusCode httpResponseStatusCode = HttpResponseStatusCode.OK)
        {
            string layout = this.GetFileContent(LayoutPath);
            string content = this.GetFileContent($"{AppDir}views/{htmlFilePath}.html");
            string html = layout.Replace("{{{content}}}", content);

            this.ViewData["nav"] = loggedIn
                ? GetFileContent(LoggedInUserNavPath)
                : GetFileContent(GuestNavPath);

            this.SetPlaceholdersContent(ref html);

            return new HtmlResult(html, httpResponseStatusCode);
        }

        protected ActionResult View([CallerMemberName]string action = "")
        {
            string fullControllerName = this.GetType().Name;
            string controllerName = fullControllerName.Substring(0, fullControllerName.Length - nameof(Controller).Length);

            return View($"{controllerName}/{action}", this.IsLoggedIn());
        }

        protected ActionResult BadRequestError(string errorMessage)
            => new HtmlResult(
                $"<h1>{errorMessage}</h1>",
                HttpResponseStatusCode.BadRequest);

        protected ActionResult ServerError(string errorMessage)
            => new HtmlResult(
                $"<h1>{errorMessage}</h1>",
                HttpResponseStatusCode.InternalServerError);

        protected string GetUsername()
            => this.Request
                .Session
                .GetParameter<string>("username");

        protected bool IsLoggedIn()
            => this.User != null;

        protected ActionResult Redirect(string location)
            => new RedirectResult(location);

        protected ActionResult Xml(object obj)
            => new XmlResult(obj.ToXml());

        protected ActionResult Json(object obj)
            => new JsonResult(obj.ToJson());

        protected ActionResult File(byte[] fileContent)
            => new FileResult(fileContent);

        protected ActionResult NotFound(string message)
            => new NotFoundResult(message);
    }
}
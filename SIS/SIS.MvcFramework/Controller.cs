using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;

using SIS.MvcFramework.Results;
using SIS.HTTP.Enums;
using SIS.HTTP.Requests;
using SIS.HTTP.Responses;

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

        private void SetPlaceholdersContent(ref string html)
        {
            foreach (KeyValuePair<string, string> pair in this.ViewData)
            {
                (string key, string value) = (pair.Key, pair.Value);

                html = html.Replace($"{{{{{{{key}}}}}}}", value);
            }
        }

        private string GetFileContent(string filePath)
            => File.ReadAllText(filePath);

        protected IHttpResponse View(string htmlFilePath, bool loggedIn = true, HttpResponseStatusCode httpResponseStatusCode = HttpResponseStatusCode.OK)
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

        protected IHttpResponse View([CallerMemberName]string action = "")
        {
            string fullControllerName = this.GetType().Name;
            string controllerName = fullControllerName.Substring(0, fullControllerName.Length - nameof(Controller).Length);

            return View($"{controllerName}/{action}", this.IsLoggedIn());
        }

        protected IHttpResponse BadRequestError(string errorMessage)
            => new HtmlResult(
                $"<h1>{errorMessage}</h1>",
                HttpResponseStatusCode.BadRequest);

        protected IHttpResponse ServerError(string errorMessage)
            => new HtmlResult(
                $"<h1>{errorMessage}</h1>",
                HttpResponseStatusCode.InternalServerError);

        protected string GetUsername()
            => this.Request
                .Session
                .GetParameter<string>("username");

        protected bool IsLoggedIn()
            => this.GetUsername() != null;

        protected IHttpResponse Redirect(string location)
            => new RedirectResult(location);
    }
}
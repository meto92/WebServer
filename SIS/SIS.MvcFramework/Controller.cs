using System.Runtime.CompilerServices;

using SIS.MvcFramework.Results;
using SIS.HTTP.Enums;
using SIS.HTTP.Requests;

using SIS.MvcFramework.Extensions;
using SIS.MvcFramework.Identity;
using SIS.MvcFramework.ViewEngine;

namespace SIS.MvcFramework
{
    public abstract class Controller
    {
        private const string AppDir = "../../../";
        private const string LayoutPath = AppDir + "views/_layout.html";
        private const string GuestNavPath = AppDir + "views/shared/guestNav.html";
        private const string LoggedInUserNavPath = AppDir + "views/shared/loggedInUserNav.html";

        private readonly IViewEngine viewEngine;

        protected Controller()
        {
            this.viewEngine = new SisViewEngine();
        }

        public IHttpRequest Request { get; set; }

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

        private string GetFileContent(string filePath)
            => System.IO.File.ReadAllText(filePath);

        protected ActionResult View<T>(T model, string htmlFilePath, HttpResponseStatusCode httpResponseStatusCode = HttpResponseStatusCode.OK)
            where T : class
        {
            string layout = this.GetFileContent(LayoutPath);
            string content = this.GetFileContent($"{AppDir}views/{htmlFilePath}.html");

            string nav = this.IsLoggedIn()
                ? GetFileContent(LoggedInUserNavPath)
                : GetFileContent(GuestNavPath);

            string html = layout.Replace("@RenderBody()", content)
                .Replace("@RenderNav()", nav);

            html = this.viewEngine.GetHtml(html, model, this.User);

            return new HtmlResult(html, httpResponseStatusCode);
        }

        protected ActionResult View<T>(T model, [CallerMemberName]string action = "")
            where T : class
        {
            string fullControllerName = this.GetType().Name;
            string controllerName = fullControllerName.Substring(0, fullControllerName.Length - nameof(Controller).Length);

            return View(model, $"{controllerName}/{action}", HttpResponseStatusCode.OK);
        }

        protected ActionResult View([CallerMemberName]string action = "")
        {
            string fullControllerName = this.GetType().Name;
            string controllerName = fullControllerName.Substring(0, fullControllerName.Length - nameof(Controller).Length);

            return View<object>(null, $"{controllerName}/{action}", HttpResponseStatusCode.OK);
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
            => this.User?.Username;

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
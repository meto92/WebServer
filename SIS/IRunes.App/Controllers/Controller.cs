using System.Collections.Generic;
using System.IO;

using IRunes.Data;
using IRunes.Services;
using IRunes.Services.Contracts;

using SIS.HTTP.Enums;
using SIS.HTTP.Requests;
using SIS.HTTP.Responses;
using SIS.WebServer.Results;

namespace IRunes.Controllers
{
    public abstract class Controller
    {
        private const string AppDir = "../../../";
        private const string LayoutPath = AppDir + "views/shared/_layout.html";
        private const string GuestNavPath = AppDir + "views/shared/guestNav.html";
        private const string LoggedInUserNavPath = AppDir + "views/shared/loggedInUserNav.html";

        protected const string UserAuthCookieKey = ".auth-IRunes";

        protected Controller(IHttpRequest request)
        {
            this.Request = request;
            this.Db = new RunesDbContext();
            this.HashService = new HashService();
            this.ViewData = new Dictionary<string, string>();
            this.UserCookieService = new UserCookieService();
            this.UserService = new UserService(this.Db);
            this.AlbumService = new AlbumService(this.Db);
            this.TrackService = new TrackService(this.Db);
        }

        protected IHttpRequest Request { get; }

        protected RunesDbContext Db { get; }

        protected IHashService HashService { get; }

        protected Dictionary<string, string> ViewData { get; }

        protected IUserCookieService UserCookieService { get; }

        protected IUserService UserService { get; }

        protected IAlbumService AlbumService { get; }
        
        protected ITrackService TrackService { get; }

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
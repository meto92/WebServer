using SIS.HTTP.Enums;
using SIS.HTTP.Responses;
using SIS.WebServer.Results;

namespace SIS.Demo
{
    public class HomeController
    {
        public IHttpResponse Index()
            => new HtmlResult("<h1>Hello, World!</h1>", HttpResponseStatusCode.OK);
    }
}
using SIS.HTTP.Responses;
using SIS.MvcFramework.Attributes.Methods;

namespace IRunes.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet(Path = "/")]
        public IHttpResponse IndexSlash()
            => Index();

        public IHttpResponse Index()
        {
            string username = GetUsername();

            if (username == null)
            {
                return View("guest");
            }

            ViewData["username"] = username;

            Request.Session.AddParameter("username", username);

            return View("loggedInUser");
        }
    }
}
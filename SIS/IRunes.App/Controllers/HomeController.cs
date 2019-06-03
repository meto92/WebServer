using SIS.MvcFramework;
using SIS.MvcFramework.Attributes.Methods;
using SIS.MvcFramework.Results;

namespace IRunes.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet(Path = "/")]
        public ActionResult IndexSlash()
            => Index();

        public ActionResult Index()
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
using IRunes.App.ViewModels;

using SIS.MvcFramework;
using SIS.MvcFramework.Attributes.Methods;
using SIS.MvcFramework.Results;

namespace IRunes.App.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet(Path = "/")]
        public ActionResult IndexSlash()
            => Index();

        public ActionResult Index()
        {
            string username = GetUsername();

            return View(new JustUsernameViewModel
            {
                Username = GetUsername(),
                IsLoggedIn = username != null
            });
        }
    }
}
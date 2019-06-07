using SIS.MvcFramework;
using SIS.MvcFramework.Attributes.Methods;
using SIS.MvcFramework.Results;

namespace IRunes.App.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet(Path = "/")]
        public IActionResult IndexSlash()
            => Index();

        public IActionResult Index()
            => View();
    }
}
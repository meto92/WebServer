using SIS.HTTP.Requests;
using SIS.HTTP.Responses;

namespace IRunes.Controllers
{
    public class HomeController : Controller
    {
        public HomeController(IHttpRequest request) 
            : base(request)
        { }

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
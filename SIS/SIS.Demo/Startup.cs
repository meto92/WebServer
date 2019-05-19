using SIS.WebServer;
using SIS.WebServer.Routing;

namespace SIS.Demo
{
    public class Startup
    {
        static void Main()
        {
            IServerRoutingTable routes = new ServerRoutingTable();

            routes.Get("/", req => new HomeController().Index());

            new Server(80, routes).Run();
        }
    }
}
using IRunes.Controllers;
using IRunes.Data;
using IRunes.ViewModels;

using Microsoft.EntityFrameworkCore;

using SIS.WebServer;
using SIS.WebServer.Routing;

namespace IRunes
{
    public class Startup
    {
        private static void ConfigureRoutes(IServerRoutingTable routes)
        {
            routes.Get("/", req => new HomeController(req).Index());
            routes.Get("/Home/Index", req => new HomeController(req).Index());

            routes.Get("/Users/Login", req => new UserController(req).GetLogin());

            routes.Post(
                "/Users/Login",
                req => new UserController(req)
                    .PostLogin(new LoginUserViewModel(
                        (string)req.FormData["usernameOrEmail"],
                        (string)req.FormData["password"])));

            routes.Get("/Users/Register", req => new UserController(req).GetRegister());

            routes.Post(
                "/Users/Register",
                req => new UserController(req)
                    .PostRegister(new RegisterUserViewModel(
                        (string)req.FormData["username"],
                        (string)req.FormData["password"],
                        (string)req.FormData["confirmPassword"],
                        (string)req.FormData["email"])));

            routes.Get("/Users/Logout", req => new UserController(req).Logout());

            routes.Get("/Albums/All", req => new AlbumController(req).All());
            routes.Get("/Albums/Create", req => new AlbumController(req).GetCreate());

            routes.Post(
                "/Albums/Create",
                req => new AlbumController(req)
                    .PostCreate(new CreateAlbumViewModel(
                        (string)req.FormData["name"],
                        (string)req.FormData["cover"])));

            routes.Get("/Albums/Details", req => new AlbumController(req).Details());
            routes.Get("/Tracks/Create", req => new TrackController(req).GetCreate());

            routes.Post(
                "/Tracks/Create",
                req => new TrackController(req).PostCreate(new CreateTrackViewModel(
                    (string)req.FormData["name"],
                    (string)req.FormData["link"],
                    (string)req.FormData["price"])));

            routes.Get("/Tracks/Details", req => new TrackController(req).Details());
        }

        private static void MigrateDatabase()
        {
            using (RunesDbContext db = new RunesDbContext())
            {
                db.Database.Migrate();
            }
        }

        public static void Main()
        {
            MigrateDatabase();

            IServerRoutingTable serverRoutingTable = new ServerRoutingTable();
            
            ConfigureRoutes(serverRoutingTable);

            new Server(80, serverRoutingTable).Run();
        }
    }
}
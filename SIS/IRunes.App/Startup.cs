using IRunes.Controllers;
using IRunes.Data;
using IRunes.ViewModels;

using Microsoft.EntityFrameworkCore;

using SIS.Framework;
using SIS.Framework.Routing;

namespace IRunes
{
    public class Startup : IMvcApplication
    {
        private static void MigrateDatabase()
        {
            using (RunesDbContext db = new RunesDbContext())
            {
                db.Database.Migrate();
            }
        }

        public void Configure(IServerRoutingTable serverRoutingTable)
        {
            //MigrateDatabase();

            serverRoutingTable.Get("/", req => new HomeController(req).Index());
            serverRoutingTable.Get("/Home/Index", req => new HomeController(req).Index());

            serverRoutingTable.Get("/Users/Login", req => new UsersController(req).Login());

            serverRoutingTable.Post(
                "/Users/Login",
                req => new UsersController(req)
                    .PostLogin(new LoginUserViewModel(
                        (string)req.FormData["usernameOrEmail"],
                        (string)req.FormData["password"])));

            serverRoutingTable.Get("/Users/Register", req => new UsersController(req).Register());

            serverRoutingTable.Post(
                "/Users/Register",
                req => new UsersController(req)
                    .PostRegister(new RegisterUserViewModel(
                        (string)req.FormData["username"],
                        (string)req.FormData["password"],
                        (string)req.FormData["confirmPassword"],
                        (string)req.FormData["email"])));

            serverRoutingTable.Get("/Users/Logout", req => new UsersController(req).Logout());

            serverRoutingTable.Get("/Albums/All", req => new AlbumsController(req).All());
            serverRoutingTable.Get("/Albums/Create", req => new AlbumsController(req).Create());

            serverRoutingTable.Post(
                "/Albums/Create",
                req => new AlbumsController(req)
                    .PostCreate(new CreateAlbumViewModel(
                        (string)req.FormData["name"],
                        (string)req.FormData["cover"])));

            serverRoutingTable.Get("/Albums/Details", req => new AlbumsController(req).Details());
            serverRoutingTable.Get("/Tracks/Create", req => new TracksController(req).Create());

            serverRoutingTable.Post(
                "/Tracks/Create",
                req => new TracksController(req).PostCreate(new CreateTrackViewModel(
                    (string)req.FormData["name"],
                    (string)req.FormData["link"],
                    (string)req.FormData["price"])));

            serverRoutingTable.Get("/Tracks/Details", req => new TracksController(req).Details());
        }

        public void ConfigureServices()
        {
            
        }
    }
}
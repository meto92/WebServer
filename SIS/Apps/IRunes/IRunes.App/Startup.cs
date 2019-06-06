using IRunes.Data;
using IRunes.Services;
using Microsoft.EntityFrameworkCore;

using SIS.MvcFramework;
using SIS.MvcFramework.DependencyContainer;

namespace IRunes.App
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

        public void Configure()
        {
            //MigrateDatabase();
        }

        public void ConfigureServices(IServiceProvider serviceProvider)
        {
            serviceProvider.Add<IAlbumService, AlbumService>();
            serviceProvider.Add<IHashService, HashService>();
            serviceProvider.Add<ITrackService, TrackService>();
            serviceProvider.Add<IUserService, UserService>();
        }
    }
}
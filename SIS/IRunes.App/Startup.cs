using IRunes.Data;

using Microsoft.EntityFrameworkCore;

using SIS.MvcFramework;

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

        public void Configure()
        {
            //MigrateDatabase();
        }

        public void ConfigureServices()
        {
            
        }
    }
}
using IRunes.Data;
using IRunes.Services;

namespace IRunes.Controllers
{
    public abstract class Controller : SIS.MvcFramework.Controller
    {
        //protected const string UserAuthCookieKey = ".auth-IRunes";

        protected Controller()
        {
            this.HashService = new HashService();
            this.UserCookieService = new UserCookieService();
            this.UserService = new UserService(Db);
            this.AlbumService = new AlbumService(Db);
            this.TrackService = new TrackService(Db);
        }

        protected static RunesDbContext Db { get; } = new RunesDbContext();

        protected IHashService HashService { get; }

        protected IUserCookieService UserCookieService { get; }

        protected IUserService UserService { get; }

        protected IAlbumService AlbumService { get; }
        
        protected ITrackService TrackService { get; }
    }
}
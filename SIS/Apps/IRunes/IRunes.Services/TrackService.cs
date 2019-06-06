using IRunes.Data;
using IRunes.Models;

namespace IRunes.Services
{
    public class TrackService : ITrackService
    {
        private readonly RunesDbContext db;

        public TrackService(RunesDbContext db)
            => this.db = db;

        public void Add(Track track)
        {
            this.db.Tracks.Add(track);
            this.db.SaveChanges();
        }
    }
}
using IRunes.Data;
using IRunes.Models;

namespace IRunes.Services
{
    public class TrackService : ITrackService
    {
        private readonly RunesDbContext db;

        public TrackService()
            => this.db = new RunesDbContext();

        public void Add(Track track)
        {
            this.db.Tracks.Add(track);
            this.db.SaveChanges();
        }
    }
}
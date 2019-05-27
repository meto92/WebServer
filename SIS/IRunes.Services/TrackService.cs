using IRunes.Data;
using IRunes.Models;
using IRunes.Services.Contracts;

namespace IRunes.Services
{
    public class TrackService : ITrackService
    {
        private readonly RunesDbContext db;

        public TrackService(RunesDbContext db)
            => this.db = db;

        public void Add(Track track)
        {
            using (this.db)
            {
                this.db.Tracks.Add(track);
                this.db.SaveChanges();
            }
        }
    }
}
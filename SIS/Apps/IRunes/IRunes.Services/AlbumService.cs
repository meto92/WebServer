using System.Collections.Generic;
using System.Linq;

using IRunes.Data;
using IRunes.Models;

using Microsoft.EntityFrameworkCore;

namespace IRunes.Services
{
    public class AlbumService : IAlbumService
    {
        private readonly RunesDbContext db;

        public AlbumService(RunesDbContext db)
            => this.db = db;

        public bool Add(Album album)
        {
            this.db.Albums.Add(album);
            this.db.SaveChanges();

            return true;
        }

        public IEnumerable<Album> All()
            => this.db.Albums.AsNoTracking();

        public Album Find(string id)
            => this.db.Albums.FirstOrDefault(a => a.Id == id);
    }
}
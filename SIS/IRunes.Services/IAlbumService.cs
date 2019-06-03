using System.Collections.Generic;

using IRunes.Models;

namespace IRunes.Services
{
    public interface IAlbumService
    {
        IEnumerable<Album> All();

        Album Find(string id);

        bool Add(Album album);
    }
}
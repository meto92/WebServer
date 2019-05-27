using System.Collections.Generic;

using IRunes.Models;

namespace IRunes.Services.Contracts
{
    public interface IAlbumService
    {
        IEnumerable<Album> All();

        Album Find(string id);

        void Add(Album album);
    }
}
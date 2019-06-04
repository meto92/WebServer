using System.Collections.Generic;

namespace IRunes.App.ViewModels
{
    public class AllAlbumsViewModel
    {
        public AllAlbumsViewModel()
            => this.AllAlbums = new List<AlbumViewModel>();

        public IEnumerable<AlbumViewModel> AllAlbums { get; set; }
    }
}
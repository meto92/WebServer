using System.Collections.Generic;
using IRunes.App.ViewModels.Tracks;

namespace IRunes.App.ViewModels.Albums
{
    public class AlbumDetailsViewModel
    {
        public AlbumDetailsViewModel()
            => this.Tracks = new List<TrackViewModel>();

        public string Id { get; set; }

        public string Name { get; set; }

        public string Cover { get; set; }

        public decimal Price { get; set; }

        public List<TrackViewModel> Tracks { get; set; }
    }
}
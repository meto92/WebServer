using System.Linq;
using IRunes.App.ViewModels;
using IRunes.Models;
using IRunes.Services;

using SIS.MvcFramework;
using SIS.MvcFramework.Attributes.Methods;
using SIS.MvcFramework.Attributes.Security;
using SIS.MvcFramework.Mapping;
using SIS.MvcFramework.Results;

namespace IRunes.App.Controllers
{
    public class TracksController : Controller
    {
        private const int MinTrackNameLength = 3;
        private const int MinTrackLinkLength = 7;

        private readonly ITrackService trackService;
        private readonly IAlbumService albumService;

        public TracksController()
        {
            this.trackService = new TrackService();
            this.albumService = new AlbumService();
        }

        [Authorize]
        public ActionResult Create(string albumId)
        {
            if (albumId == null)
            {
                return Redirect("/Albums/All");
            }

            return View(new AlbumViewModel
            {
                Id = albumId
            });
        }

        [Authorize]
        [HttpPost(ActionName = nameof(Create))]
        public ActionResult PostCreate(TrackCreateViewModel model, string albumId)
        {
            if (albumId == null)
            {
                return Redirect("/Albums/All");
            }

            Album album = this.albumService.Find(albumId);

            if (album == null)
            {
                return Redirect("/Albums/All");
            }

            if (model.Name.Length < MinTrackNameLength
                || model.Link.Length < MinTrackLinkLength
                || !decimal.TryParse(model.PriceStr, out decimal price)
                || price < 0)
            {
                return BadRequestError("Invalid data.");
            }

            Track track = new Track()
            {
                Name = model.Name,
                Link = model.Link,
                Price = price,
                AlbumId = album.Id
            };

            this.trackService.Add(track);

            return Redirect("/");
        }

        [Authorize]
        public ActionResult Details(string albumId, string trackId)
        {
            if (albumId == null || trackId == null)
            {
                return Redirect("/Albums/All");
            }

            Album album = this.albumService.Find(albumId);
            Track track = album?.Tracks.FirstOrDefault(t => t.Id == trackId);

            if (album == null || track == null)
            {
                return Redirect("/Albums/All");
            }

            TrackViewModel model = ModelMapper.ProjectTo<TrackViewModel>(track);

            return View(model);
        }
    }
}
using System.Linq;

using IRunes.Models;
using IRunes.Services;
using IRunes.ViewModels;

using SIS.MvcFramework;
using SIS.MvcFramework.Attributes.Methods;
using SIS.MvcFramework.Attributes.Security;
using SIS.MvcFramework.Results;

namespace IRunes.Controllers
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
        public ActionResult Create()
        {
            if (!Request.QueryData.TryGetValue("albumId", out object albumId))
            {
                return Redirect("/Albums/All");
            }

            ViewData["albumId"] = (string) albumId;

            return View();
        }

        [Authorize]
        [HttpPost(ActionName = nameof(Create))]
        public ActionResult PostCreate(CreateTrackViewModel model)
        {
            if (!Request.QueryData.TryGetValue("albumId", out object albumId))
            {
                return Redirect("/Albums/All");
            }

            Album album = this.albumService.Find((string) albumId);

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
                Album = album
            };

            this.trackService.Add(track);

            return Redirect("/");
        }

        [Authorize]
        public ActionResult Details()
        {
            if (!Request.QueryData.TryGetValue("albumId", out object albumId)
                || !Request.QueryData.TryGetValue("trackId", out object trackId))
            {
                return Redirect("/Albums/All");
            }

            Album album = this.albumService.Find((string) albumId);
            Track track = album?.Tracks.FirstOrDefault(t => t.Id == (string) trackId);

            if (album == null || track == null)
            {
                return Redirect("/Albums/All");
            }

            ViewData["name"] = track.Name;
            ViewData["link"] = track.Link;
            ViewData["price"] = $"${track.Price:f2}";
            ViewData["albumId"] = (string) albumId;

            return View();
        }
    }
}
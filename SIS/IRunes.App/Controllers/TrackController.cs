using System.Linq;

using IRunes.Models;
using IRunes.ViewModels;

using SIS.HTTP.Requests;
using SIS.HTTP.Responses;

namespace IRunes.Controllers
{
    public class TrackController : Controller
    {
        private const int MinTrackNameLength = 3;
        private const int MinTrackLinkLength = 7;

        public TrackController(IHttpRequest request)
            : base(request)
        { }

        public IHttpResponse GetCreate()
        {
            if (!IsLoggedIn())
            {
                return Redirect("/Users/Login");
            }

            if (!Request.QueryData.TryGetValue("albumId", out object albumId))
            {
                return Redirect("/Albums/All");
            }

            ViewData["albumId"] = (string) albumId;

            return View("tracks/create");
        }

        public IHttpResponse PostCreate(CreateTrackViewModel model)
        {
            if (!IsLoggedIn())
            {
                return Redirect("/Users/Login");
            }

            if (!Request.QueryData.TryGetValue("albumId", out object albumId))
            {
                return Redirect("/Albums/All");
            }

            Album album = AlbumService.Find((string) albumId);

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

            TrackService.Add(track);

            return Redirect("/");
        }

        public IHttpResponse Details()
        {
            if (!IsLoggedIn())
            {
                return Redirect("/Users/Login");
            }

            if (!Request.QueryData.TryGetValue("albumId", out object albumId)
                || !Request.QueryData.TryGetValue("trackId", out object trackId))
            {
                return Redirect("/Albums/All");
            }

            Album album = AlbumService.Find((string) albumId);
            Track track = album?.Tracks.FirstOrDefault(t => t.Id == (string) trackId);

            if (album == null || track == null)
            {
                return Redirect("/Albums/All");
            }

            ViewData["name"] = track.Name;
            ViewData["link"] = track.Link;
            ViewData["price"] = $"${track.Price:f2}";
            ViewData["albumId"] = (string) albumId;

            return View("tracks/details");
        }
    }
}
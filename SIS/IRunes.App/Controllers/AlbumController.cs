using System.Collections.Generic;
using System.Text;

using IRunes.Models;
using IRunes.ViewModels;

using SIS.HTTP.Requests;
using SIS.HTTP.Responses;

namespace IRunes.Controllers
{
    public class AlbumController : Controller
    {
        private const int MinAlbumNameLength = 3;
        private const int MinAlbumCoverLength = 3;

        public AlbumController(IHttpRequest request)
            : base(request)
        { }

        public IHttpResponse All()
        {
            if (!IsLoggedIn())
            {
                return Redirect("/Users/Login");
            }

            IEnumerable<Album> allAlbums = AlbumService.All();

            StringBuilder result = new StringBuilder();

            foreach (Album album in allAlbums)
            {
                string html = $"<a href='/Albums/Details?id={album.Id}' class='d-block'><strong>{album.Name}</strong></a>";

                result.AppendLine(html);
            }

            ViewData["albumLinks"] = result.Length == 0
                ? "There are currently no albums."
                : result.ToString();

            return View("albums/all");
        }

        public IHttpResponse GetCreate()
            => IsLoggedIn()
                ? View("albums/create")
                : Redirect("/Users/Login");

        public IHttpResponse PostCreate(CreateAlbumViewModel model)
        {
            if (!IsLoggedIn())
            {
                return Redirect("/Users/Login");
            }
            
            if (model.Name.Length < MinAlbumNameLength
                || model.Cover.Length < MinAlbumCoverLength)
            {
                return BadRequestError("Invalid data");
            }

            Album album = new Album()
            {
                Name = model.Name,
                Cover = model.Cover
            };

            AlbumService.Add(album);

            return Redirect("Albums/All");
        }

        public IHttpResponse Details()
        {
            if (!IsLoggedIn())
            {
                return Redirect("/Users/Login");
            }

            if (!Request.QueryData.TryGetValue("id", out object id))
            {
                return Redirect("/Albums/All");
            }

            Album album = AlbumService.Find((string) id);

            if (album == null)
            {
                return Redirect("/Albums/All");
            }

            int trackNumber = 0;
            StringBuilder tracksResult = new StringBuilder("<ul>");

            foreach (Track track in album.Tracks)
            {
                string html = $"<li>{++trackNumber}. <a href='/Tracks/Details?albumId={album.Id}&trackId={track.Id}'><em>{track.Name}</em></a></li>";

                tracksResult.AppendLine(html);
            }

            tracksResult.AppendLine("</ul>");

            ViewData["cover"] = album.Cover;
            ViewData["name"] = album.Name;
            ViewData["price"] = $"${album.Price:f2}";
            ViewData["albumId"] = album.Id;
            ViewData["tracks"] = tracksResult.ToString();

            return View("albums/details");
        }
    }
}
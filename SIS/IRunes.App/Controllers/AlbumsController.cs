using System.Collections.Generic;
using System.Text;

using IRunes.Models;
using IRunes.Services;
using IRunes.ViewModels;

using SIS.MvcFramework;
using SIS.MvcFramework.Attributes.Methods;
using SIS.MvcFramework.Attributes.Security;
using SIS.MvcFramework.Results;

namespace IRunes.Controllers
{
    [Authorize]
    public class AlbumsController : Controller
    {
        private const int MinAlbumNameLength = 3;
        private const int MinAlbumCoverLength = 3;

        private readonly IAlbumService albumService;

        public AlbumsController()
            => this.albumService = new AlbumService();

        public ActionResult All()
        {
            IEnumerable<Album> allAlbums = this.albumService.All();

            StringBuilder result = new StringBuilder();

            foreach (Album album in allAlbums)
            {
                string html = $"<a href='/Albums/Details?id={album.Id}' class='d-block'><strong>{album.Name}</strong></a>";

                result.AppendLine(html);
            }

            ViewData["albumLinks"] = result.Length == 0
                ? "There are currently no albums."
                : result.ToString();

            return View();
        }

        public ActionResult Create()
            => View();

        [HttpPost(ActionName = nameof(Create))]
        public ActionResult PostCreate(CreateAlbumViewModel model)
        {
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

            this.albumService.Add(album);

            return Redirect("/Albums/All");
        }

        public ActionResult Details()
        {
            if (!Request.QueryData.TryGetValue("id", out object id))
            {
                return Redirect("/Albums/All");
            }

            Album album = this.albumService.Find((string) id);

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

            return View();
        }
    }
}
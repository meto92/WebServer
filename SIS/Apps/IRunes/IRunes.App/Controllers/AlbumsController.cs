﻿using System.Collections.Generic;

using IRunes.App.ViewModels.Albums;
using IRunes.Models;
using IRunes.Services;

using SIS.MvcFramework;
using SIS.MvcFramework.Attributes.Methods;
using SIS.MvcFramework.Attributes.Security;
using SIS.MvcFramework.Mapping;
using SIS.MvcFramework.Results;

namespace IRunes.App.Controllers
{
    [Authorize]
    public class AlbumsController : Controller
    {
        private readonly IAlbumService albumService;

        public AlbumsController(IAlbumService albumService)
            => this.albumService = albumService;

        public IActionResult All()
        {
            IEnumerable<Album> allAlbums = this.albumService.All();

            AllAlbumsViewModel allAlbumsViewModel = new AllAlbumsViewModel
            {
                AllAlbums = allAlbums.To<AlbumViewModel>()
            };

            return View(allAlbumsViewModel);
        }

        public IActionResult Create()
            => View();

        [HttpPost]
        public IActionResult Create(AlbumCreateInputModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            Album album = new Album()
            {
                Name = model.Name,
                Cover = model.Cover
            };

            this.albumService.Add(album);

            return Redirect("/Albums/All");
        }

        public IActionResult Details(string id)
        {
            if (id == null)
            {
                return Redirect("/Albums/All");
            }

            Album album = this.albumService.Find(id);

            if (album == null)
            {
                return Redirect("/Albums/All");
            }

            AlbumDetailsViewModel model = ModelMapper.ProjectTo<AlbumDetailsViewModel>(album);

            return View(model);
        }
    }
}
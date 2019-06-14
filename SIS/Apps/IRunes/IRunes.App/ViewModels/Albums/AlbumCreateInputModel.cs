using SIS.MvcFramework.Attributes.Validation;

namespace IRunes.App.ViewModels.Albums
{
    public class AlbumCreateInputModel
    {
        [StringLengthSis(3, 55)]
        public string Name { get; set; }

        [StringLengthSis(5, 2000)]
        public string Cover { get; set; }
    }
}
using SIS.MvcFramework.Attributes.Validation;

namespace IRunes.App.ViewModels.Tracks
{
    public class TrackCreateInputModel
    {
        [StringLengthSis(3, 22)]
        public string Name { get; set; }

        [StringLengthSis(5, 2000)]
        public string Link { get; set; }

        [RequiredSis]
        [RangeSis(0.1, 1111)]
        public double Price { get; set; }
    }
}
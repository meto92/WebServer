namespace IRunes.ViewModels
{
    public class CreateAlbumViewModel
    {
        public CreateAlbumViewModel(string name, string cover)
        {
            this.Name = name;
            this.Cover = cover;
        }

        public string Name { get; set; }
        
        public string Cover { get; set; }
    }
}
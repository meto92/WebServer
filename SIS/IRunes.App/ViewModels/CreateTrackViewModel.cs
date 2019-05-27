namespace IRunes.ViewModels
{
    public class CreateTrackViewModel
    {
        public CreateTrackViewModel(string name, string link, string priceStr)
        {
            this.Name = name;
            this.Link = link;
            this.PriceStr = priceStr;
        }

        public string Name { get; set; }

        public string Link { get; set; }

        public string PriceStr { get; set; }
    }
}
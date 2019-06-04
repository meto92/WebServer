using System.Collections.Generic;
using System.Linq;

namespace IRunes.Models
{
    public class Album : BaseModel
    {
        public Album()
        {
            this.Tracks = new List<Track>();
        }

        public string Name { get; set; }

        public string Cover { get; set; }

        public decimal Price => this.Tracks.Sum(t => t.Price) * 0.87m;

        public virtual ICollection<Track> Tracks { get; set; }
    }
}
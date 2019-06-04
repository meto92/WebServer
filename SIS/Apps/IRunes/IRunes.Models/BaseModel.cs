using System;

namespace IRunes.Models
{
    public abstract class BaseModel
    {
        protected BaseModel()
            => this.Id = Guid.NewGuid().ToString();

        public string Id { get; set; }
    }
}
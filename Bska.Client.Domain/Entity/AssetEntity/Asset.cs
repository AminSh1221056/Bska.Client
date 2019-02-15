
namespace Bska.Client.Domain.Entity.AssetEntity
{
    using Bska.Client.API.EF6;
    using Bska.Client.API.Infrastructure;
    using System;

    public abstract class Asset : Entity
    {
        public String Name { get; set; }
        public DateTime InsertDate { get; set; }
        public DateTime ModeifiedDate { get; set; }
        public override string ToString()
        {
            return $"{Name}--{InsertDate}--Type:{this.GetType().Name}";
        }
    }
}

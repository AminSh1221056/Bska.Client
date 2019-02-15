
namespace Bska.Client.Domain.Entity.AssetEntity
{
    using System;
    public class Belonging : MovableAsset
    {
        public virtual UnConsumption ParentMAsset { get; set; }
    }
}

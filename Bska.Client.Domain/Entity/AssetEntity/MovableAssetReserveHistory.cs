
namespace Bska.Client.Domain.Entity.AssetEntity
{
    using Bska.Client.API.EF6;
    using Bska.Client.Common;
    using System;
    public class MovableAssetReserveHistory:Entity
    {
        public Int64 Id { get; set; }
        public String Description { get; set; }
        public Int64? AssetId { get; set; }
        public MAssetReserveStatus Status { get; set; }
        public virtual MovableAsset MAsset { get; set; }
    }
}

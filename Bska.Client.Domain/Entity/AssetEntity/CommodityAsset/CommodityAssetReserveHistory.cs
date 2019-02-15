
namespace Bska.Client.Domain.Entity.AssetEntity.CommodityAsset
{
    using Bska.Client.API.EF6;
    using Bska.Client.Common;
    using System;
    public class CommodityAssetReserveHistory : Entity
    {
        public Int64 Id { get; set; }
        public String Description { get; set; }
        public Int64? CommodityId { get; set; }
        public MAssetReserveStatus Status { get; set; }
        public virtual Commodity CommodityAsset { get; set; }
    }
}

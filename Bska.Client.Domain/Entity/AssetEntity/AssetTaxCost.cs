
namespace Bska.Client.Domain.Entity.AssetEntity
{
    using Bska.Client.Common;
    using Bska.Client.API.EF6;
    using System;
    public class AssetTaxCost : Entity
    {
        public Int32 Id { get; set; }
        public TaxCostType TaxCostType { get; set; }
        public String Description { get; set; }
        public decimal Cost { get; set; }
        public DateTime ModifiedDate { get; set; }
        public Int64? AssetId { get; set; }
        public virtual MovableAsset MAsset { get; set; }
    }
}

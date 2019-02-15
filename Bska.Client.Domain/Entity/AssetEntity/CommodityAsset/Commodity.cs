
namespace Bska.Client.Domain.Entity.AssetEntity
{
    using CommodityAsset;
    using Common;
    using OrderEntity;
    using System;
    using System.Collections.Generic;

    public class Commodity : PortableAsset,IPortotype<Commodity>
    {
        public Commodity()
        {
            this.PlaceOfUses = new List<PlaceOfUse>();
            this.Orders = new List<Order>();
            this.CommodityAssetReserveHistories = new List<CommodityAssetReserveHistory>();
        }
       
        public string Country { get; set; }
        public string Company { get; set; }
        public string StoreAddress { get; set; }
        public string BatchNumber { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public Int64? UnConsuptionId { get; set; }
        public Int32? StoreBillId { get; set; }
        public virtual StoreBill StoreBill { get; set; }
        public virtual ICollection<Order> Orders { get; private set; }
        public virtual ICollection<PlaceOfUse> PlaceOfUses { get; private set; }
        public virtual ICollection<CommodityAssetReserveHistory> CommodityAssetReserveHistories { get; private set; }
        public virtual UnConsumption UnConsumption { get; set; }
        public virtual SupplierIndent SupplierIndent { get; set; }
        public Commodity Clone()
        {
            return (Commodity)this.MemberwiseClone();
        }
    }
}


namespace Bska.Client.Domain.Entity.AssetEntity
{
    using Bska.Client.API.EF6;
    using Bska.Client.Common;
    using Bska.Client.Domain.Entity.OrderEntity;
    using System;
    using System.Collections.Generic;

    public class StoreBill : Entity,IEquatable<StoreBill>
    {
        public StoreBill()
        {
            MAssets = new List<MovableAsset>();
            Commodities = new List<Commodity>();
            StoreBillEdits = new List<StoreBillEdit>();
            SupplierIndents = new List<SupplierIndent>();
        }
        public Int32 StoreBillId { get; set; }
        public String StoreBillNo { get; set; }
        public DateTime ArrivalDate { get; set; }
        public StateOwnership AcqType { get; set; }
        public String Desc1 { get; set; }
        public String Desc2 { get; set; }
        public String Desc3 { get; set; }
        public StuffType StuffType { get; set; }
        public DateTime ModifiedDate { get; set; }
        public Int32? StoreId { get; set; }
        public Int32? SellerId { get; set; }
        public virtual ICollection<MovableAsset> MAssets { get; private set; }
        public virtual ICollection<Commodity> Commodities { get; private set; }
        public virtual ICollection<StoreBillEdit> StoreBillEdits { get; private set; }
        public virtual Store Store { get; set; }
        public virtual AccountDocumentMaster AccountDocument { get; set; }
        public virtual ICollection<SupplierIndent> SupplierIndents { get; private set; }
        public override string ToString()
        {
            return string.Format("No:{0} and storeId:{1}",StoreBillNo,StoreId);
        }

        public bool Equals(StoreBill other)
        {
            if (other == null)
                return base.Equals(other);
            return this.StoreBillId == other.StoreBillId && this.StoreBillNo == other.StoreBillNo
                && this.AcqType==other.AcqType && this.StuffType==other.StuffType;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return base.Equals(obj);
            return Equals(obj as StoreBill);
        }

        public override int GetHashCode()
        {
            return this.StoreBillId.GetHashCode();
        }
    }
}

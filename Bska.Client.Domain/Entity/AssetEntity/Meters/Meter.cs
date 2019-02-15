
namespace Bska.Client.Domain.Entity.AssetEntity.Meters
{
    using Bska.Client.Domain.Entity.AssetEntity.MeterBills;
    using System;
    using System.Collections.Generic;
    public abstract class Meter : ImmovableAsset, IEquatable<Meter>
    {
        public Meter()
        {
            this.MeterBills = new List<MeterBill>();
        }
        
        public String SubscriptionNo { get; set; }
        public String AddressLine { get; set; }
        public String PostalCode { get; set; }
        public String Plake { get; set; }
        public String CaseNo { get; set; }
        public String BodyNo { get; set; }
        public Int32 TariffType { get; set; }
        public Int32? BuildingId { get; set; }
        public virtual Building Building { get; set; }
        public virtual ICollection<MeterBill> MeterBills { get; private set; }

        public bool Equals(Meter other)
        {
            if (other == null)
                return base.Equals(other);
            return this.ImAssetId == other.ImAssetId && this.CaseNo == other.CaseNo;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return base.Equals(obj);
            return Equals(obj as Meter);
        }
        public override int GetHashCode()
        {
            return this.ImAssetId.GetHashCode();
        }
    }
}

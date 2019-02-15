
namespace Bska.Client.Domain.Entity.OrderEntity
{
    using Bska.Client.API.EF6;
    using Bska.Client.Common;
    using System;
    using System.Collections.Generic;

    public class SubOrder : Entity, IEquatable<SubOrder>
    {
        public SubOrder()
        {
            SupplierIndents = new List<SupplierIndent>();
            SupplierTrenderOffers = new List<SupplierTrenderOffer>();
        }
        public Int64 SubOrderId { get; set; }
        public SubOrderType Type { get; set; }
        public Double Num { get; set; }
        public Double Remain { get; set; }
        public SubOrderState State { get; set; }
        public String Identity { get; set; }
        public Int32 UnitId { get; set; }
        public Int64? OrderDetailsId { get; set; }
        public virtual OrderDetails OrderDetails { get; set; }
        public virtual ICollection<SupplierIndent> SupplierIndents { get; private set; }
        public virtual ICollection<SupplierTrenderOffer> SupplierTrenderOffers { get; set; }

        public override string ToString()
        {
            return string.Format("Order type:{0},Order Id:{1}", this.SubOrderId, this.Type);
        }

        public bool Equals(SubOrder other)
        {
            if (other == null)
                return base.Equals(other);
            return this.SubOrderId == other.SubOrderId && this.Type == other.Type;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return base.Equals(obj);
            return Equals(obj as SubOrder);
        }

        public override int GetHashCode()
        {
            return SubOrderId.GetHashCode();
        }
    }
}

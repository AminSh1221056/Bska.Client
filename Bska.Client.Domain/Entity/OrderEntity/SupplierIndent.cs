
namespace Bska.Client.Domain.Entity.OrderEntity
{
    using Bska.Client.Common;
    using Bska.Client.API.EF6;
    using System;
    using System.Collections.Generic;
    using AssetEntity;

    public class SupplierIndent : Entity
    {
        public SupplierIndent()
        {
            this.StoreBills = new List<StoreBill>();
            this.ReturnIndentRequsts = new List<ReturnIndentRequest>();
        }

        public Int64 ID { get; set; }
        public double Num { get; set; }
        public double Remain { get; set; }
        public Int32 SupplierId { get; set; }
        public Int32 UnitId { get; set; }
        public SupplierIndentState State { get; set; }
        public Int32? SellerId { get; set; }
        public Int64? SubOrderId { get; set; }
        public virtual Seller Seller { get; set; }
        public virtual SubOrder SubOrder { get; set; }
        public virtual ICollection<ReturnIndentRequest> ReturnIndentRequsts { get; set; }
        public virtual ICollection<StoreBill> StoreBills { get; private set; }
    }
}

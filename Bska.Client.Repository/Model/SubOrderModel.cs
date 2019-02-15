
namespace Bska.Client.Repository.Model
{
    using Bska.Client.Common;
    using System;
    public class SubOrderModel:IEquatable<SubOrderModel>
    {
        public Int64 OrderId { get; set; }
        public Int64 SubOrderId { get; set; }
        public Int64 OrderDetailsId { get; set; }
        public Int64 SupplierIndentId { get; set; }
        public Double Num { get; set; }
        public double Remain { get; set; }
        public String StuffName { get; set; }
        public int KalaUid { get; set; }
        public string KalaNo { get; set; }
        public Int32 UnitId { get; set; }
        public DateTime OrderDate { get; set; }
        public StuffType StuffType { get; set; }
        public String Identity { get; set; }
        public OrderType OrderType { get; set; }
        public SubOrderState State { get; set; }
        public SupplierIndentState SpState { get; set; }
        public Int32? SellerId { get; set; }
        public Int32 SupplierId { get; set; }
        public Boolean IsSelected { get; set; }

        public bool Equals(SubOrderModel other)
        {
            if (other == null) return base.Equals(other);
            return this.SubOrderId == other.SubOrderId && string.Equals(this.StuffName,other.StuffName);
        }
        public override bool Equals(object obj)
        {
            if (obj == null)
                return base.Equals(obj);
            return Equals(obj as SubOrderModel);
        }
        public override int GetHashCode()
        {
            return this.SubOrderId.GetHashCode();
        }
    }
}

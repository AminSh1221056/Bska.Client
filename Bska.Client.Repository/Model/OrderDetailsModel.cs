
namespace Bska.Client.Repository.Model
{
    using Bska.Client.Common;
    using System;
    public class OrderDetailsModel
    {
        public Int64 OrderId { get; set; }
        public Int64 OrderDetailsId { get; set; }
        public String StuffName { get; set; }
        public OrderType OrderType { get; set; }
        public DateTime OrderDate { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public StuffType StuffType { get; set; }
        public int kalaUid { get; set; }
        public string KalaNo { get; set; }
        public Double Num { get; set; }
        public String PersonName { get; set; }
        public Int32 UnitId { get; set; }
        public Int32? OrganizId { get; set; }
        public Int32? StrategyId { get; set; }
        public String NationalId { get; set; }
        public DateTime? DueDate { get; set; }
        public Int32 PersonId { get; set; }
        public PersianDate PersianOrderDate { get; set; }
        public PersianDate? PersianDueDate { get; set; }
    }
}

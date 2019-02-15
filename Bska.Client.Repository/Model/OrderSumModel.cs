
namespace Bska.Client.Repository.Model
{
    using Common;
    using System;
    public class OrderSumModel
    {
        public int KalaUid { get; set; }
        public int UnitId { get; set; }
        public double Num { get; set; }
        public OrderType OrderType { get; set; }
        public OrderStatus Status { get; set; }
        public DateTime OrderDate { get; set; }
    }
}

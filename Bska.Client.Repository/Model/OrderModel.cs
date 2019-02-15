
namespace Bska.Client.Repository.Model
{
    using Bska.Client.Common;
    using System;
    public class OrderModel
    {
        public Int64 OrderId { get; set; }
        public OrderType OrderType { get; set; }
        public DateTime OrderDate { get; set; }
        public OrderStatus Status { get; set; }
        public Boolean IsEditable { get; set; }
        public String PersonName { get; set; }
        public String NationalId { get; set; }
        public DateTime? DueDate { get; set; }
        public Int32 PersonId { get; set; }
        public string Description { get; set; }
    }
}

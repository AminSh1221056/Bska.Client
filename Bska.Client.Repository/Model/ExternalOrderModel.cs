
namespace Bska.Client.Repository.Model
{
    using System;
    public class ExternalOrderModel
    {
        public Int64 OrderId { get; set; }
        public Int32 Status { get; set; }
        public Int32 OrderType { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public DateTime? DueDate { get; set; }
        public Int32 TargetEmployee { get; set; }
        public string EmployeeName { get; set; }
    }
}

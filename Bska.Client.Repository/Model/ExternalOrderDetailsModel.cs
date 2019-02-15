
namespace Bska.Client.Repository.Model
{
    using Common;
    using System;
    public class ExternalOrderDetailsModel
    {
        public Int64 OrderDetialsId { get; set; }
        public String StuffName { get; set; }
        public StuffType StuffType { get; set; }
        public Int32 State { get; set; }
        public Double Num { get; set; }
        public Int32 UnitId { get; set; }
        public Int32 OrderType { get; set; }
        public Int32 TargetEmployee { get; set; }
    }
}


namespace Bska.Client.Repository.Model
{
    using Bska.Client.Common;
    using System;
    public class StoreBillEditModel
    {
        public Int32 Id { get; set; }
        public DateTime InsertDate { get; set; }
        public string Description { get; set; }
        public GlobalRequestStatus State { get; set; }
        public Int32 StoreBillId { get; set; }
        public string StoreBillNo { get; set; }
        public StateOwnership AcqType { get; set; }
        public decimal Price { get; set; }
    }
}

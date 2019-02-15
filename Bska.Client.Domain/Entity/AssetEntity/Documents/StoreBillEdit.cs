
namespace Bska.Client.Domain.Entity.AssetEntity
{
    using Bska.Client.API.EF6;
    using Bska.Client.Common;
    using System;
    public class StoreBillEdit : Entity
    {
        public Int32 Id { get; set; }
        public DateTime InsertDate { get; set; }
        public string Description { get; set; }
        public GlobalRequestStatus State { get; set; }
        public Int32? StoreBillId { get; set; }
        public virtual StoreBill StoreBill { get; set; }
    }
}

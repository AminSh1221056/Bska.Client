
namespace Bska.Client.Domain.Entity.OrderEntity
{
    using Bska.Client.API.EF6;
    using System;
    public class SupplierTrenderOffer : Entity
    {
        public Int32 Id { get; set; }
        public Int32 SupplierId { get; set; }
        public byte[] ProForma { get; set; }
        public Boolean ISEnableTrender { get; set; }
        public DateTime InsertDate { get; set; }
        public Int64? SubOrderId { get; set; }
        public virtual SubOrder SubOrder { get; set; }
    }
}


namespace Bska.Client.Domain.Entity.OrderEntity
{
    using Bska.Client.API.EF6;
    using System;
    public class OrderUserHistory : Entity
    {
        public Int64 Id { get; set; }
        public Int32 UserId { get; set; }
        public Boolean UserDecision { get; set; }
        public String Description { get; set; }
        public String Identity { get; set; }
        public Boolean IsCurrent { get; set; }
        public Int64? OrderDetailsId { get; set; }
        public virtual OrderDetails OrderDetails { get; set; }
    }
}


namespace Bska.Client.Domain.Entity.OrderEntity
{
    using AssetEntity.CommodityAsset;
    using Bska.Client.API.EF6;
    using Bska.Client.Common;
    using Bska.Client.Domain.Entity.AssetEntity;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    [Serializable()]
    public class Order : Entity , IEquatable<Order>
    {
        public Order()
        {
            this.OrderDetails = new List<OrderDetails>();
            this.MovableAssets = new List<MovableAsset>();
            this.Commodities = new List<Commodity>();
        }

        public Int64 OrderId { get; set; }
        public Int32? PersonId { get; set; }
        public OrderStatus Status { get; set; }
        public OrderType OrderType { get; set; }
        public ProceedingsType? OrderProcType { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public DateTime? DueDate { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }
        public String Description { get; set; }
        public virtual Person Person { get; set; }
        public virtual ICollection<OrderDetails> OrderDetails { get; private set; }
        public virtual ICollection<MovableAsset> MovableAssets { get; private set; }
        public virtual ICollection<Commodity> Commodities { get; private set; }
        public override string ToString()
        {
            return string.Format("Order type:{0},Order Id:{1}", this.OrderType, this.OrderId);
        }

        public bool Equals(Order other)
        {
            if (other == null)
                return base.Equals(other);
            return this.OrderId == other.OrderId && this.OrderDate == other.OrderDate;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return base.Equals(obj);
            return Equals(obj as Order);
        }

        public override int GetHashCode()
        {
            return OrderId.GetHashCode();
        }
    }
}

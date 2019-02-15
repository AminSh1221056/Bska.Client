
namespace Bska.Client.Domain.Entity.OrderEntity
{
    using Bska.Client.API.EF6;
    using Bska.Client.Common;
    using System;
    using System.Collections.Generic;

    [Serializable()]
    public class OrderDetails : Entity, ICloneable
    {
        public OrderDetails()
        {
            this.SubOrders = new List<SubOrder>();
            this.OrderUserHistories = new List<OrderUserHistory>();
        }

        public Int64 OrderDetialsId { get; set; }
        public String StuffName { get; set; }
        public int KalaUid { get; set; }
        public string kalaNo { get; set; }
        public StuffType StuffType { get; set; }
        public OrderDetailsState State { get; set; }
        public Double Num { get; set; }
        public Int32 UnitId { get; set; }
        public String UsingLocation { get; set; }
        public Decimal EstimatePrice { get; set; }
        public Int32 ImportantDegree { get; set; }
        public String OfferQuality { get; set; }
        public String OfferSpecification { get; set; }
        public Int32? StrategyId { get; set; }
        public Int32? OrganizId { get; set; }
        public Int32? StoreId { get; set; }
        public Int32? StoreDesignId { get; set; }
        public Int64? OrderId { get; set; }
        public Boolean IsReject { get; set; }
        public String Description { get; set; }
        public Int64? BelongingParentLable { get; set; }
        public virtual Order Order { get; set; }
        public virtual ICollection<SubOrder> SubOrders { get; private set; }
        public virtual ICollection<OrderUserHistory> OrderUserHistories { get; private set; }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}

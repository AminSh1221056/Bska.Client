
namespace Bska.Client.Domain.Entity
{
    using Bska.Client.API.EF6;
    using Bska.Client.Common;
    using Bska.Client.Domain.Entity.AssetEntity;
    using System;
    using System.Collections.Generic;
    public class Store : Entity,IEquatable<Store>
    {
        public Store()
        {
            this.StoreDesign = new List<StoreDesign>();
        }
        public Int32 StoreId { get; set; }
        public String Name { get; set; }
        public StoreType StoreType { get; set; }
        public String Storage { get; set; }
        public DateTime CreateDate { get; set; }
        public String Description { get; set; }
        public Int32? StrategyId { get; set; }
        public virtual StrategyDesign Strategy { get; set; }
        public virtual ICollection<StoreDesign> StoreDesign { get; private set; }
        public virtual ICollection<StoreBill> StoreBills { get; private set; }
        public virtual ICollection<Document> Documents { get; private set; }
        public virtual ICollection<Proceeding> Proceedings { get; private set; }
        public bool Equals(Store other)
        {
            if (other == null)
                return base.Equals(other);
            return this.StoreId == other.StoreId && this.Name== other.Name;
        }
        public override bool Equals(object obj)
        {
            if (obj == null)
                return base.Equals(obj);
            return Equals(obj as Store);
        }
        public override string ToString()
        {
            return string.Format("{0} , {1}", this.Name,this.StoreId);
        }
        public override int GetHashCode()
        {
            return this.StoreId.GetHashCode();
        }
    }
}

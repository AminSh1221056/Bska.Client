
namespace Bska.Client.Domain.Entity
{
    using Bska.Client.API.EF6;
    using Bska.Client.Domain.Entity.AssetEntity;
    using System;
    using System.Collections.Generic;
    public class StoreDesign : Entity
    {
        public StoreDesign()
        {
            this.ChildNode = new List<StoreDesign>();
        }
        public Int32 StoreDesignId { get; set; }
        public Int32? StoreId { get; set; }
        public String Name { get; set; }
        public virtual StoreDesign ParentNode { get; set; }
        public virtual ICollection<StoreDesign> ChildNode { get; private set; }
        public virtual Store Store { get; set; }
    }
}

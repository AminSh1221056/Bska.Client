
namespace Bska.Client.Domain.Entity
{
    using AssetEntity;
    using OrderEntity;
    using Bska.Client.API.EF6;
    using Bska.Client.Common;
    using System;
    using System.Collections.Generic;

    public class Seller : Entity
    {
        public Seller()
        {
            this.SupplierIndents = new List<SupplierIndent>();
        }
        public Int32 SellerId { get; set; }
        public SellerType Type { get; set; }
        public String Name { get; set; }
        public String Lastname { get; set; }
        public String Tell { get; set; }
        public String Coding { get; set; }
        public String Mobile { get; set; }
        public String Province { get; set; }
        public String Zone { get; set; }
        public String City { get; set; }
        public String TownShip { get; set; }
        public String AddressLine { get; set; }
        public Int32? EmployeeId { get; set; }
        public virtual Employee Employee { get; set; }
        public virtual ICollection<SupplierIndent> SupplierIndents { get; private set; }
    }
}


namespace Bska.Client.Domain.Entity
{
    using Bska.Client.API.EF6;
    using Bska.Client.Domain.Entity.AssetEntity;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;
    public class EmployeeDesign : Entity
    {
        public EmployeeDesign()
        {
            this.ChildNode = new List<EmployeeDesign>();
        }

        public int BuidldingDesignId { get; set; }
        public Int32? EmployeeId { get; set; }
        public String Name { get; set; }
        public string Code { get; set; }
        public virtual EmployeeDesign ParentNode { get; set; }
        public virtual ICollection<EmployeeDesign> ChildNode { get; private set; }
        public virtual Employee Employee { get; set; }

        [NotMapped]
        public Boolean HaveRole { get; set; }
    }

    public class StrategyDesign : EmployeeDesign
    {
        public StrategyDesign()
        {
            this.Stores = new List<Store>();
        }

        public virtual Building Building { get; set; }

        public virtual ICollection<Store> Stores { get; private set; }
    }

    public class OrganizationDesign : EmployeeDesign
    {
        public OrganizationDesign()
        {
            OrganizationPerfectStuffs = new List<OrganizationPefectStuff>();
        }
        public virtual ICollection<OrganizationPefectStuff> OrganizationPerfectStuffs { get; private set; }
    }
}

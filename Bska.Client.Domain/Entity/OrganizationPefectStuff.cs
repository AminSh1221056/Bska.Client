
namespace Bska.Client.Domain.Entity
{
    using Bska.Client.API.EF6;
    using Bska.Client.Common;
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class OrganizationPefectStuff : Entity
    {
        public OrganizationPefectStuff()
        {

        }

        [Key, Column(Order = 0)]
        public string KalaNo { get; set; }

        [Key, Column(Order = 1)]
        public Int32 BuidldingDesignId { get; set; }
        public virtual Stuff Stuff { get; set; }
        public virtual OrganizationDesign EmployeeDesign { get; set; }
    }
}

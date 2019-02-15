
namespace Bska.Client.Domain.Entity.OrderEntity
{
    using System;
    using Bska.Client.API.EF6;
    using Bska.Client.Common;
    using System.Collections.Generic;

    public class ReturnIndentRequest : Entity
    {
        public ReturnIndentRequest()
        {
            SupplierIndents = new List<SupplierIndent>();
        }
        public Int32 Id { get; set; }
        public GlobalRequestStatus Status { get; set; }
        public string Description { get; set; }
        public DateTime InsertDate { get; set; }
        public Int32? EmployeeId { get; set; }
        public virtual Employee Employee { get; set; }
        public virtual ICollection<SupplierIndent> SupplierIndents { get; private set; }
    }
}


namespace Bska.Client.Domain.Entity
{
    using Bska.Client.API.EF6;
    using System;
    public class RequestPermit : Entity
    {
        public Int32 RequestPermitId { get; set; }
        public Int32? PersonId { get; set; }
        public Int32 OrganizId { get; set; }
        public Int32 StrategyId { get; set; }
        public String OrganizName { get; set; }
        public String StragegyName { get; set; }
        public Boolean IsEnable { get; set; }
        public virtual Person Person { get; set; } 
    }
}

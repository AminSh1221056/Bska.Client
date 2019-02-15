
namespace Bska.Client.Domain.Entity
{
    using Bska.Client.API.EF6;
    using Bska.Client.Common;
    using System;
    using System.Collections.Generic;
    public class Users :Entity
    {
        public Users()
        {
            this.Roles = new HashSet<Roles>();
            this.EventLogs = new List<EventLog>();
        }
        public Int32 UserId { get; set; }
        public String FullName { get; set; }
        public String UserName { get; set; }
        public String Password { get; set; }
        public Int32? PersonId { get; set; }
        public PermissionsType PermissionType { get; set; }
        public virtual Person Person { get; set; }
        public virtual UserAttribute UserAttribute { get; set; }
        public virtual ICollection<Roles> Roles { get; private set; }
        public virtual ICollection<EventLog> EventLogs { get; private set; }
    }
}

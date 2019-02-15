
namespace Bska.Client.Domain.Entity
{
    using Bska.Client.API.EF6;
    using Bska.Client.Common;
    using System;
    public class Roles : Entity, IEquatable<Roles>
    {
        public Int32 RolesId { get; set; }
        public Int32? UserId { get; set; }
        public Int32? StoreId { get; set; }
        public Int32? OrganizId { get; set; }
        public PermissionsType RoleType { get; set; }
        public virtual Users User { get; set; }

        public bool Equals(Roles other)
        {
            if (other == null)
                return base.Equals(other);

            return this.RolesId==other.RolesId;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return base.Equals(obj);

            return Equals(obj as Roles);
        }

        public override int GetHashCode()
        {
            return this.RolesId.GetHashCode();
        }
    }
}

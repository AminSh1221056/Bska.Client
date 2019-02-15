
namespace Bska.Client.Domain.Entity
{
    using Bska.Client.API.EF6;
    using Bska.Client.Common;
    using Bska.Client.Domain.Entity.OrderEntity;
    using System;
    using System.Collections.Generic;
    public class Person : Entity,IEquatable<Person>
    {
        public Person()
        {
            this.Users = new HashSet<Users>();
            this.RequestPermit = new HashSet<RequestPermit>();
            this.Orders = new List<Order>();
        }

        public Int32 PersonId { get; set; }
        public String FirstName { get; set; }
        public String LastName { get; set; }
        public String FatherName { get; set; }
        public String NationalId { get; set; }
        public String PersonCode { get; set; }
        public String Mobile { get; set; }
        public Boolean Married { get; set; }
        public String Postalcode { get; set; }
        public String AddressLine { get; set; }
        public ContractType ContractcType { get; set; }
        public DateTime? BirthDate { get; set; }
        public DateTime? EmployeeDate { get; set; }
        public Byte[] Photo { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public Int32? EmployeeId { get; set; }
        public virtual Employee Employee { get; set; }
        public virtual ICollection<Users> Users { get; private set; }
        public virtual ICollection<RequestPermit> RequestPermit { get; private set; }
        public virtual ICollection<Order> Orders { get; private set; }
        public override string ToString()
        {
            return string.Format("{0} {1} : {2}",this.FirstName,this.LastName,this.PersonId);
        }

        public bool Equals(Person other)
        {
            if (other == null)
                return base.Equals(other);

            return String.Equals(this.NationalId,other.NationalId,StringComparison.CurrentCulture) && this.FirstName == other.FirstName && this.LastName == other.LastName;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return base.Equals(obj);

            return Equals(obj as Person);
        }

        public override int GetHashCode()
        {
            return this.PersonId.GetHashCode();
        }
    }
}

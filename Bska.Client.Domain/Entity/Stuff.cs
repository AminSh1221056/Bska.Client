
namespace Bska.Client.Domain.Entity
{
    using Bska.Client.API.EF6;
    using Bska.Client.Common;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Text;
    public class Stuff : Entity, IEquatable<Stuff>
    {
        public Stuff()
        {
            this.Childeren = new List<Stuff>();
            this.OrganizationPefectStuffs = new List<OrganizationPefectStuff>();
        }
        public Int32 StuffId { get; set; }
        public string KalaNo { get; set; }
        public string GS1 { get; set; }
        public String Name { get; set; }
        public Boolean IsStuff { get; set; }
        public StuffType StuffType { get; set; }
        public Int32? FloorOld { get; set; }
        public Int32? FloorNew { get; set; }

        [NotMapped]
        public String Description { get; set; }
        public virtual Stuff Parent { get; set; } 
        public virtual ICollection<Stuff> Childeren { get; private set; }
        public virtual ICollection<OrganizationPefectStuff> OrganizationPefectStuffs { get; private set; }
        public bool Equals(Stuff other)
        {
            if (other == null)
                return base.Equals(other);
            return this.StuffId == other.StuffId && this.Name == other.Name;
        }
        public override bool Equals(object obj)
        {
            if (obj == null)
                return base.Equals(obj);
            return Equals(obj as Stuff);
        }

        public override int GetHashCode()
        {
            return StuffId.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("{0} {1} {2}", this.Name,Environment.NewLine, $"کد: {this.KalaNo}");
        }
    }
}

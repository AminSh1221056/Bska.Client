
namespace Bska.Client.Domain.Entity.AssetEntity
{
    using Bska.Client.API.EF6;
    using Bska.Client.Common;
    using System;
    using System.Collections.Generic;

    public class Proceeding : Entity,IEquatable<Proceeding>
    {
        public Proceeding()
        {
            this.AssetProceedings = new List<AssetProceeding>();
            this.ExportDetailsProceeding = new List<ExportDetailsProceeding>();
        }
        public Int32 ProceedingId { get; set; }
        public ProceedingsType Type { get; set; }
        public ProceedingState State { get; set; }
        public DateTime ProceedingDate { get; set; }
        public DateTime? ExecutionTime { get; set; }
        public String Desc1 { get; set; }
        public String Desc2 { get; set; }
        public String Desc3 { get; set; }
        public String Desc4 { get; set; }
        public String Desc5 { get; set; }
        public String Desc6 { get; set; }
        public String Description { get; set; }
        public Guid ProcIdentity { get; set; }
        public Boolean IsSended { get; set; }
        public Int32? StoreId { get; set; }
        public virtual Store Store { get; set; }
        public virtual ICollection<AssetProceeding> AssetProceedings { get; set; }
        public virtual ICollection<ExportDetailsProceeding> ExportDetailsProceeding { get; private set; }
        public bool Equals(Proceeding other)
        {
            if (other == null)
                return base.Equals(other);
            return this.ProceedingId == other.ProceedingId && this.Type == other.Type;
        }

        public override string ToString()
        {
            return string.Format("{0}---{1}", this.ProceedingId, this.Type);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                base.Equals(obj);
            return this.Equals(obj as Proceeding);
        }

        public override int GetHashCode()
        {
            return this.ProceedingId.GetHashCode();
        }
    }
}

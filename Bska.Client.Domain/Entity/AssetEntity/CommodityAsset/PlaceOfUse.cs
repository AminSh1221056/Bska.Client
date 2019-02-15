
namespace Bska.Client.Domain.Entity.AssetEntity.CommodityAsset
{
    using Bska.Client.API.EF6;
    using OrderEntity;
    using System;
    public class PlaceOfUse : Entity, IEquatable<PlaceOfUse>
    {
        public Int64 Id { get; set; }
        public int OrganizId { get; set; }
        public int StrategtyId { get; set; }
        public string PersonId { get; set; }
        public double Num { get; set; }
        public int UnitId { get; set; }
        public DateTime InsertDate { get; set; }
        public Int64? CommodityId { get; set; }
        public virtual Commodity Commodity { get; set; }
        public Int64? DocumentId { get; set; }
        public virtual Document Document { get; set; }
        public bool Equals(PlaceOfUse other)
        {
            if (other == null)
                return base.Equals(other);
            else return this.Id == other.Id && this.OrganizId == other.OrganizId && this.StrategtyId==other.StrategtyId;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return base.Equals(obj);
            return Equals(obj as PlaceOfUse);
        }

        public override string ToString()
        {
            return string.Format("Place of use :organizId:{0}---strategyId:{1}", this.OrganizId, this.StrategtyId);
        }
    }
}

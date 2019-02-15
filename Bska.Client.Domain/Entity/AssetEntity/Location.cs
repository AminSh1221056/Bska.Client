
namespace Bska.Client.Domain.Entity.AssetEntity
{
    using Bska.Client.API.EF6;
    using Bska.Client.Common;
    using System;
    using System.Collections.Generic;

    public class Location : Entity,IEquatable<Location>,IPortotype<Location>
    {
        public Int64 LocationId { get; set; }
        public Int64? AssetId { get; set; }
        public string PersonId { get; set; }
        public Int32 OrganizId { get; set; }
        public Int32 StrategyId { get; set; }
        public Int32 StoreId { get; set; }
        public Int32 StoreAddressId { get; set; }
        public LocationStatus Status { get; set; }
        public AccountDocumentType AccountDocumentType { get; set; }
        public DateTime InsertDate { get; set; }
        public DateTime? MovedRequestDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public virtual MovableAsset MovableAsset { get; set; }

        public bool Equals(Location other)
        {
            if (other == null)
                return base.Equals(other);
            if (Status == LocationStatus.Active)
                return this.PersonId == other.PersonId && this.OrganizId == other.OrganizId && this.StrategyId == other.StrategyId;
            else if (Status == LocationStatus.StoreActive) return this.StoreId == other.StoreId;
            else return this.LocationId == other.LocationId && this.Status == other.Status;
        }

        public override int GetHashCode()
        {
            return LocationId.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return base.Equals(obj);
            return Equals(obj as Location);
        }

        public override string ToString()
        {
            return string.Format("location to status:{0}---assetId:{1}", this.Status, this.AssetId);
        }

        public Location Clone()
        {
            return (Location)this.MemberwiseClone();
        }
    }
}

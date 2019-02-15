
namespace Bska.Client.Domain.Entity.AssetEntity
{
    using System;
    using System.Collections.Generic;

    public abstract class ImmovableAsset : Asset, IEquatable<ImmovableAsset>
    {
        public Int32 ImAssetId { get; set; }
        public bool Equals(ImmovableAsset other)
        {
            if (other == null)
                return base.Equals(other);
            return this.ImAssetId == other.ImAssetId && this.Name == other.Name;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return base.Equals(obj);
            return Equals(obj as ImmovableAsset);
        }
        public override int GetHashCode()
        {
            return this.ImAssetId.GetHashCode();
        }
    }
}

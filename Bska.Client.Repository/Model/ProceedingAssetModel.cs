
namespace Bska.Client.Repository.Model
{
    using Bska.Client.Common;
    using System;
    public class ProceedingAssetModel : IEquatable<ProceedingAssetModel>
    {
        public Int64 AssetId { get; set; }
        public String Name { get; set; }
        public Int32? Label { get; set; }
        public Int32 UnitId { get; set; }
        public Double Num { get; set; }
        public Decimal Price { get; set; }
        public String LicenseNumber { get; set; }
        public String AccidentDivanNo { get; set; }
        public Boolean IsOrganFault { get; set; }
        public String RecipetNo { get; set; }
        public AssetProceedingState State { get; set; }
        public Boolean IsEditablePrice { get; set; }
        public Boolean IsEditableLicense { get; set; }
        public Boolean IsEditableDivan { get; set; }
        public Boolean IsSelectable { get; set; }
        public Boolean IsSelected { get; set; }
        public String TempUid1 { get; set; }
        public String TempUid2 { get; set; }
        public String TempUid3 { get; set; }
        public String TempUid4 { get; set; }
        public String TempDesc1 { get; set; }
        public String TempDesc2 { get; set; }
        public String TempDesc3 { get; set; }
        public String TempDesc4 { get; set; }
        public int? TempYear { get; set; }
        public bool Equals(ProceedingAssetModel other)
        {
            if (other == null) return base.Equals(other);
            return this.AssetId == other.AssetId && string.Equals(this.Name,other.Name);
        }
        public override bool Equals(object obj)
        {
            if (obj == null)
                return base.Equals(obj);
            return Equals(obj as ProceedingAssetModel);
        }
        public override int GetHashCode()
        {
            return this.AssetId.GetHashCode();
        }
    }
}


namespace Bska.Client.Domain.Entity.AssetEntity
{
    using Bska.Client.Common;
    using Bska.Client.Domain.Entity.OrderEntity;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;
    public class MovableAsset : PortableAsset, IFormattable, IPortotype<MovableAsset>,IEquatable<MovableAsset>
    {
        public MovableAsset()
        {
            this.Locations = new List<Location>();
            this.Documetns = new List<Document>();
            this.AssetProceedings = new List<AssetProceeding>();
            this.AssetTaxCost = new List<AssetTaxCost>();
            this.ExportDetailsMAsset = new List<ExportDetailsMAsset>();
            this.Orders = new List<Order>();
            this.MovableAssetReserveHistories = new List<MovableAssetReserveHistory>();
        }
        
        public Int32? Label { get; set; }
        public String Floor { get; set; }
        public Int32? FloorType { get; set; }
        public Int32? OldLabel { get; set; }
        public Int32? OrganLabel { get; set; }
        public MAssetCurState CurState { get; set; }
        public String Uid1 { get; set; }
        public String Uid2 { get; set; }
        public String Uid3 { get; set; }
        public String Uid4 { get; set; }
        public String Desc1 { get; set; }
        public String Desc2 { get; set; }
        public String Desc3 { get; set; }
        public String Desc4 { get; set; }
        public String Desc5 { get; set; }
        public String Desc6 { get; set; }
        public String Desc7 { get; set; }
        public String Desc8 { get; set; }
        public String Desc9 { get; set; }
        public String Desc10 { get; set; }
        public String Desc11 { get; set; }
        public CompietionState ISCompietion { get; set; }
        public Boolean ISConfirmed { get; set; }
        public Int32? StoreBillId { get; set; }
        public virtual StoreBill StoreBill { get; set; }
        public virtual ICollection<Order> Orders { get; private set; }
        public virtual ICollection<ExportDetailsMAsset> ExportDetailsMAsset { get; private set; }
        public virtual ICollection<Location> Locations { get; private set; }
        public virtual ICollection<AssetProceeding> AssetProceedings { get; private set; }
        public virtual ICollection<AssetTaxCost> AssetTaxCost { get; private set; }
        public virtual ICollection<Document> Documetns { get; private set; }
        public virtual ICollection<MovableAssetReserveHistory> MovableAssetReserveHistories { get; private set; }
        public override string ToString()
        {
            return string.Format("Name:{0}---Num:{1}---UnitId:{2}", Name, Num, UnitId);
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            if (format == null)
            {
                return ToString();
            }

            string formatUpper = format.ToUpper();
            switch (formatUpper)
            {
                case "T":
                    return GetItemType();
                default:
                    return ToString();
            }
        }

        private String GetItemType()
        {
            string value = "ناشناخته";
            if (this is UnConsumption) value = "غیرمصرفی";
            else if (this is Belonging) value = "متعلقات";
            else if (this is InCommidity) value = "در حکم مصرف";
            else if (this is Installable) value = "قابل نصب در بنا";
            else value = "ناشناخته";
            return value;
        }

        [NotMapped]
        public Boolean IsInStore { get; set; }

        public MovableAsset Clone()
        {
            return (MovableAsset)this.MemberwiseClone();
        }


        public bool Equals(MovableAsset other)
        {
            if (other == null)
                return base.Equals(other);
            return this.AssetId == other.AssetId && this.Name == other.Name;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return base.Equals(obj);
            return Equals(obj as MovableAsset);
        }
        public override int GetHashCode()
        {
            return this.AssetId.GetHashCode();
        }
    }
}

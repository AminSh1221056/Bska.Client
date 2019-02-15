
namespace Bska.Client.Domain.Entity.AssetEntity
{
    using Bska.Client.API.EF6;
    using Bska.Client.Common;
    using CommodityAsset;
    using System;
    using System.Collections.Generic;

    public class Document : Entity, IEquatable<Document>
    {
        public Document()
        {
            this.MovableAsset = new List<MovableAsset>();
            this.Commodities = new List<PlaceOfUse>();
        }
        public Int64 DocumentId { get; set; }
        public String Desc1 { get; set; }
        public String Desc2 { get; set; }
        public String Desc3 { get; set; }
        public String Desc4 { get; set; }
        public string Transferee { get; set; }
        public Int32? StoreId { get; set; }
        public DocumentType DocumentType { get; set; }
        public DateTime DocumentDate { get; set; }
        public virtual ICollection<MovableAsset> MovableAsset { get; set; }
        public virtual ICollection<PlaceOfUse> Commodities { get; private set; }
        public virtual AccountDocumentMaster AccountDocument { get; set; }
        public virtual Store Store { get; set; }
        public override string ToString()
        {
            return string.Format("{0} {1}", Desc1, DocumentType);
        }
        public bool Equals(Document other)
        {
            if (other == null)
                return base.Equals(other);
            return this.DocumentId == other.DocumentId && this.Desc1==other.Desc1 && this.DocumentType == other.DocumentType;
        }
        public override bool Equals(object obj)
        {
            if (obj == null)
                return base.Equals(obj);
            return Equals(obj as Document);
        }
        public override int GetHashCode()
        {
            return this.DocumentId.GetHashCode();
        }
    }
}

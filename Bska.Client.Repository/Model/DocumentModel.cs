
namespace Bska.Client.Repository.Model
{
    using Bska.Client.Common;
    using System;
    public class DocumentModel
    {
        public Int64 DocumentId { get; set; }
        public String Desc1 { get; set; }
        public string Transferee { get; set; }
        public Int32? StoreId { get; set; }
        public DocumentType DocumentType { get; set; }
        public DateTime DocumentDate { get; set; }
        public PersianDate PersianDocumentDate { get; set; }
        public string StoreName { get; set; }
    }
}

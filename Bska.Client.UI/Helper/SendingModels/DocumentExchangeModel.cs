
namespace Bska.Client.UI.Helper
{
    using Bska.Client.Common;
    using System;
    using System.Collections.Generic;
    public class DocumentExchangeModel
    {
        public DocumentExchangeModel()
        {
            this.MAssets = new List<PortableAssetDocumentExchangeModel>();
        }
        public string DocumentId { get; set; }
        public String Desc1 { get; set; }
        public String Desc2 { get; set; }
        public String Desc3 { get; set; }
        public String Desc4 { get; set; }
        public string Transferee { get; set; }
        public string StoreName { get; set; }
        public Int32 EmployeeId { get; set; }
        public DocumentType DocumentType { get; set; }
        public DateTime DocumentDate { get; set; }
        public ICollection<PortableAssetDocumentExchangeModel> MAssets { get; private set; }
    }
}


namespace Bska.Client.UI.Helper
{
    using Bska.Client.Common;
    using System;
    using System.Collections.Generic;

    public class StoreBillExchangeModel
    {
        public StoreBillExchangeModel()
        {
            this.MAssets = new List<MovableAssetExchangeModel>();
        }

        public string StoreBillId { get; set; }
        public string StoreBillNo { get; set; }
        public DateTime ArrivalDate { get; set; }
        public StateOwnership AcqType { get; set; }
        public string Desc1 { get; set; }
        public string Desc2 { get; set; }
        public string Desc3 { get; set; }
        public StuffType StuffType { get; set; }
        public string StoreName { get; set; }
        public string SellerName { get; set; }
        public DateTime InsertDate { get; set; }
        public ICollection<MovableAssetExchangeModel> MAssets { get; private set; }
    }
}

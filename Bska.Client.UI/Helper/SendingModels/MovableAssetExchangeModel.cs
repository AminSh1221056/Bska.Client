using Bska.Client.Common;
using System;

namespace Bska.Client.UI.Helper
{
    public class MovableAssetExchangeModel
    {
        public string AssetId { get; set; }
        public String Name { get; set; }
        public Double Num { get; set; }
        public Int32 UnitId { get; set; }
        public Int32? Label { get; set; }
        public string KalaUid { get; set; }
        public Decimal Cost { get; set; }
        public String Description { get; set; }
        public MAssetCurState CurState { get; set; }
        public String Desc1 { get; set; }
        public String Desc2 { get; set; }
        public String Desc3 { get; set; }
        public String Desc4 { get; set; }
        public String Uid1 { get; set; }
        public String Uid2 { get; set; }
        public String Uid3 { get; set; }
        public String Uid4 { get; set; }
        public string StoreBillId { get; set; }
        public string BelongingParentId { get; set; }
        public DateTime InsertDate { get; set; }
        public DateTime ModeifiedDate { get; set; }
        public Int32 Type { get; set; }
        public CompietionState ISCompietion { get; set; }
        public Boolean ISConfirmed { get; set; }
        public String Floor { get; set; }
        public Int32? FloorType { get; set; }
        public Int32? OldLabel { get; set; }
        public Int32? OrganLabel { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public DateTime? ExpirationDate { get; set; }
    }
}

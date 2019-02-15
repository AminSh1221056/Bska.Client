
namespace Bska.Client.Repository.Model
{
    using Bska.Client.Common;
    using System;
    public class MovableAssetModel
    {
        public Int64 AssetId { get; set; }
        public MAssetCurState CurState { get; set; }
        public StateOwnership AcqType { get; set; }
        public LocationStatus Status { get; set; }
        public MAssetReserveStatus ReserveStatus { get; set; }
        public Int32 kalaUid { get; set; }
        public string KalaNo { get; set; }
        public String Name { get; set; }
        public Double Num { get; set; }
        public Int32 UnitId { get; set; }
        public Int32? Label { get; set; }
        public DateTime InsertDate { get; set; }
        public PersianDate PersianInsertDate { get; set; }
        public string PersonId { get; set; }
        public Int32 OrganizId { get; set; }
        public Int32 StrategyId { get; set; }
        public Int32 StoreId { get; set; }
        public Int32 StoreDesignId { get; set; }
        public String MAssetType { get; set; }
        public CompietionState IsCompietion { get; set; }
        public Boolean IsConfirmed { get; set; }
        public Boolean IsRowEnabled { get; set; }
        public Boolean IsSelected { get; set; }
        public Boolean IsInStore { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public decimal Cost { get; set; }
        public string Quality { get; set; }
        public string UnitName { get; set; }
    }
}

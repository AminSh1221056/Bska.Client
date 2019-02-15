using Bska.Client.Common;
using Bska.Client.API.Infrastructure;
using Bska.Client.Domain.Entity;

namespace Bska.Client.UI.Helper
{
    public class BskaUIHelper
    {
        public MAssetCurState getAssetLicencingStateByProc(ProceedingsType procType)
        {
            MAssetCurState curState = MAssetCurState.AtOperation;
            switch (procType)
            {
                case ProceedingsType.Accident:
                case ProceedingsType.Earthquake:
                case ProceedingsType.Fire:
                case ProceedingsType.Flood:
                case ProceedingsType.Theft:
                    curState = MAssetCurState.AccidentLicensing;
                    break;
                case ProceedingsType.BudgetLicencing:
                    curState = MAssetCurState.BudgetLicencing;
                    break;
                case ProceedingsType.DefinitiveTransfer:
                    curState = MAssetCurState.TransferLicensing;
                    break;
                case ProceedingsType.Delete:
                    curState = MAssetCurState.DeleteUnsaleableLicencing;
                    break;
                case ProceedingsType.Sale:
                    curState = MAssetCurState.SurplusSalesLicensing;
                    break;
                case ProceedingsType.SpecialLicencing:
                    curState = MAssetCurState.SpecialProvisionsLicencing;
                    break;
                case ProceedingsType.StateTransfer:
                    curState = MAssetCurState.TransferStateLicensing;
                    break;
                case ProceedingsType.TrustTransfer:
                    curState = MAssetCurState.TrustLicensing;
                    break;
            }
            return curState;
        }

        public MAssetCurState getAssetDefenitiveStateByProc(ProceedingsType procType)
        {
            MAssetCurState curState = MAssetCurState.AtOperation;
            switch (procType)
            {
                case ProceedingsType.Accident:
                case ProceedingsType.Earthquake:
                case ProceedingsType.Fire:
                case ProceedingsType.Flood:
                case ProceedingsType.Theft:
                    curState = MAssetCurState.Disaster;
                    break;
                case ProceedingsType.BudgetLicencing:
                    curState = MAssetCurState.DeleteBudget;
                    break;
                case ProceedingsType.DefinitiveTransfer:
                    curState = MAssetCurState.GovCompanyTransfer;
                    break;
                case ProceedingsType.Delete:
                    curState = MAssetCurState.DeleteUnsaleable;
                    break;
                case ProceedingsType.Sale:
                    curState = MAssetCurState.Sold;
                    break;
                case ProceedingsType.SpecialLicencing:
                    curState = MAssetCurState.DeleteSpecialProvisions;
                    break;
                case ProceedingsType.StateTransfer:
                    curState = MAssetCurState.OutStateTransfer;
                    break;
                case ProceedingsType.TrustTransfer:
                    curState = MAssetCurState.SendTrust;
                    break;
                case ProceedingsType.RefundTrust:
                    curState = MAssetCurState.RefundTrust;
                    break;
                case ProceedingsType.ReturnFromRetiring:
                    curState = MAssetCurState.AtOperation;
                    break;
            }
            return curState;
        }

        public LocationStatus getLocationStatusByProc(ProceedingsType procType)
        {
            LocationStatus locaState = LocationStatus.Active;
            switch (procType)
            {
                case ProceedingsType.Accident:
                case ProceedingsType.Earthquake:
                case ProceedingsType.Fire:
                case ProceedingsType.Flood:
                case ProceedingsType.Theft:
                    locaState = LocationStatus.Accident;
                    break;
                case ProceedingsType.BudgetLicencing:
                case ProceedingsType.Delete:
                case ProceedingsType.SpecialLicencing:
                    locaState = LocationStatus.Delete;
                    break;
                case ProceedingsType.DefinitiveTransfer:
                    locaState = LocationStatus.Transfer;
                    break;
                case ProceedingsType.Sale:
                    locaState = LocationStatus.Sale;
                    break;
                case ProceedingsType.AssetRetiring:
                    locaState = LocationStatus.Retiring;
                    break;
                case ProceedingsType.StateTransfer:
                    locaState = LocationStatus.TransferState;
                    break;
                case ProceedingsType.TrustTransfer:
                    locaState = LocationStatus.Trust;
                    break;
                case ProceedingsType.RefundTrust:
                    locaState = LocationStatus.RefundTrust;
                    break;
                case ProceedingsType.EditRequest:
                    locaState = LocationStatus.Active;
                    break;
            }
            return locaState;
        }

        public EventLog eventLogCreator(string msg, EventType evType)
        {
            return new EventLog
            {
                EntryDate = GlobalClass._Today,
                Key = UserLog.UniqueInstance.LogedUser.FullName,
                Message = msg,
                ObjectState = ObjectState.Added,
                Type = evType,
                UserId = UserLog.UniqueInstance.LogedUser.UserId
            };
        }
    }
}

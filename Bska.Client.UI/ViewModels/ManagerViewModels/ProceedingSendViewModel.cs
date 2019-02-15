
namespace Bska.Client.UI.ViewModels
{
    using Bska.Client.API.Infrastructure;
    using Bska.Client.API.UnitOfWork;
    using Bska.Client.Common;
    using Bska.Client.Data.Service;
    using Bska.Client.Domain.Entity.AssetEntity;
    using Bska.Client.Repository.Model;
    using Bska.Client.UI.API;
    using Bska.Client.UI.Helper;
    using Bska.Client.UI.Services;
    using Microsoft.Practices.Unity;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Net.Http;
    using System.ServiceModel;
    using System.Threading.Tasks;
    using System.Transactions;
    using System.Windows.Input;
    public sealed class ProceedingSendViewModel : BaseViewModel
    {
        #region ctor

        public ProceedingSendViewModel(IUnityContainer container)
        {
            this._container = container;
            this._unitOfWork = _container.Resolve<IUnitOfWorkAsync>();
            this._proceedingService = _container.Resolve<IProceedingService>(new ParameterOverride("repository", _unitOfWork.Repository<Proceeding>()));
            this._movableAssetService = _container.Resolve<IMovableAssetService>();
            this._storeService = _container.Resolve<IStoreService>();
            this._dialogService = _container.Resolve<IDialogService>();
            this._proceedingItems = new ObservableCollection<Proceeding>();
            this._client = UserLog.UniqueInstance.Client;
            this.IdentificationCode = "";
            this.initalizObj();
            this.initializCommands();
        }

        #endregion

        #region properties
        public String IdentificationCode
        {
            get { return GetValue(() => IdentificationCode); }
            set
            {
                SetValue(() => IdentificationCode, value);
            }
        }
        public CustomerModel Customer
        {
            get { return GetValue(() => Customer); }
            set
            {
                SetValue(() => Customer, value);
            }
        }

        public String AutheniticationInfo
        {
            get { return GetValue(() => AutheniticationInfo); }
            set
            {
                SetValue(() => AutheniticationInfo, value);
            }
        }

        public Boolean IsAuthenticated
        {
            get { return GetValue(() => IsAuthenticated); }
            set
            {
                SetValue(() => IsAuthenticated, value);
            }
        }

        public ObservableCollection<Proceeding> ProceddingsItems
        {
            get { return _proceedingItems; }
        }
        
        public Proceeding Selected
        {
            get { return GetValue(() => Selected); }
            set
            {
                SetValue(() => Selected, value);
            }
        }
        
        public Boolean ChGetData
        {
            get { return GetValue(() => ChGetData); }
            set
            {
                SetValue(() => ChGetData, value);
            }
        }

        public Boolean ChSendData
        {
            get { return GetValue(() => ChSendData); }
            set
            {
                SetValue(() => ChSendData, value);
            }
        }

        #endregion

        #region methods

        private void initalizObj()
        {
            if (UserLog.UniqueInstance.LogedEmployee != null)
            {
                var dtnDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(APPSettings.Default.employeeCer);
                if (dtnDictionary != null)
                {
                    if (dtnDictionary.ContainsKey(UserLog.UniqueInstance.LogedEmployee.BudgetNo.ToString()))
                    {
                        IdentificationCode = GlobalClass.DecryptStringAES(dtnDictionary[UserLog.UniqueInstance.LogedEmployee.BudgetNo.ToString()], "66Ak679Du4V3qo92");
                    }
                }
            }
        }

        private async void PerformIdentityAccuracy()
        {
            if (GlobalClass.CheckForInternetConnection(Settings.Default.ServerUrl))
            {
                _dialogService.ShowAlert("توجه", ErrorMessages.Default.NoInternetAccess);
                return;
            }

            if (IdentificationCode.Length != 16)
            {
                _dialogService.ShowAlert("توجه", "کد شناسایی نامعتبر است");
                return;
            }

            Mouse.SetCursor(Cursors.Wait);
            try
            {
                string path = $"Identification/{IdentificationCode}";
                this.AutheniticationInfo = "در حال شناسایی...";
                Customer = await this.GetCustomerAsync(path);
                if (Customer != null)
                {
                    this.AutheniticationInfo = "هویت این سازمان شناسایی شد";
                }
                else
                {
                    _dialogService.ShowAlert("توجه", "هیچ سازمانی یافت نشد");
                    this.AutheniticationInfo = "هویت این سازمان شناخته شده نیست";
                }
            }
            catch (TimeoutException)
            {
                _dialogService.ShowError("خطا", "هیچ پاسخی از سیستم مرکزی دریافت نشد");
            }
            catch (Exception ex)
            {
                throw ex;
            }

            Mouse.SetCursor(Cursors.Arrow);
        }

        private async Task<CustomerModel> GetCustomerAsync(string path)
        {
            CustomerModel customer = null;
            HttpResponseMessage response = await _client.GetAsync(path);
            if (response.IsSuccessStatusCode)
            {
                customer = await response.Content.ReadAsAsync<CustomerModel>();
            }
            return customer;
        }

        private LocationStatus getLocationStatusByProc(ProceedingsType procType)
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
                case ProceedingsType.StateTransfer:
                    locaState = LocationStatus.TransferState;
                    break;
                case ProceedingsType.TrustTransfer:
                    locaState = LocationStatus.Trust;
                    break;
            }
            return locaState;
        }

        private MAssetCurState getAssetLicencingStateByProc(ProceedingsType procType)
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

        private MAssetCurState getAssetDefenitiveStateByProc(ProceedingsType procType)
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
            }
            return curState;
        }

        #endregion

        #region commands

        public ICommand IdentifyCommand { get; private set; }
        public ICommand SendCommand { get; private set; }
        public ICommand SearchCommand { get; private set; }
        public ICommand ConfirmExchangeCommand { get; private set; }
        private void initializCommands()
        {
            IdentifyCommand = new MvvmCommand(
                 (parameter) =>
                 {
                     this.PerformIdentityAccuracy();
                 },
                 (parameter) =>
                 {
                     return IdentificationCode.Length == 16;
                 }
                ).AddListener<AssetSendViewModel>(this, x => x.IdentificationCode);
        }

        #endregion

        #region fields

        private readonly IUnityContainer _container;
        private readonly IUnitOfWorkAsync _unitOfWork;
        private readonly IDialogService _dialogService;
        private readonly IProceedingService _proceedingService;
        private readonly IMovableAssetService _movableAssetService;
        private readonly IStoreService _storeService;
        private readonly ObservableCollection<Proceeding> _proceedingItems;
        private readonly HttpClient _client;

        #endregion
    }
}


namespace Bska.Client.UI.ViewModels.GeneralManagerViewModels
{
    using Bska.Client.Common;
    using Bska.Client.Data.Service;
    using Bska.Client.Domain.Entity;
    using Bska.Client.Repository.Model;
    using Bska.Client.UI.Services;
    using Bska.Client.UI.Helper;
    using Bska.Client.UI.API;
    using Microsoft.Practices.Unity;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Windows;
    using System.Linq;
    using System.Windows.Data;
    using System.Windows.Input;
    using System.Net.Http;
    using System.Threading.Tasks;

    public sealed class ExternalOrderRecivedViewModel : BaseViewModel
    {
        #region ctor

        public ExternalOrderRecivedViewModel(IUnityContainer container)
        {
            this._container = container;
            this._unitService = _container.Resolve<IUnitService>();
            this._dialogService = _container.Resolve<IDialogService>();
            this._navigationService = _container.Resolve<INavigationService>();
            this._exOrderModel = new ObservableCollection<ExternalOrderModel>();
            this._exOrderDetails = new ObservableCollection<ExternalOrderDetailsModel>();
            this.ExOrderFilteredView = new CollectionViewSource { Source = ExOrderModel }.View;
            this._seedDataHelper = new SeedDataHelper();
            this._client = UserLog.UniqueInstance.Client;
            this.initializObj();
            this.initializCommands();
        }

        #endregion

        #region properties

        public Window Window
        {
            get;
            set;
        }

        public List<Unit> Units
        {
            get { return GetValue(() => Units); }
            set
            {
                SetValue(() => Units, value);
            }
        }
        
        public ObservableCollection<ExternalOrderModel> ExOrderModel
        {
            get { return _exOrderModel; }
        }

        public ExternalOrderModel SelectedOrder
        {
            get { return GetValue(() => SelectedOrder); }
            set
            {
                SetValue(() => SelectedOrder, value);
            }
        }
        public ObservableCollection<ExternalOrderDetailsModel> ExOrderCollection
        {
            get { return _exOrderDetails; }
        }
        public ICollectionView ExOrderFilteredView { get; set; }
        public List<OrganizationModel> OrganNames
        {
            get { return GetValue(() => OrganNames); }
            set
            {
                SetValue(() => OrganNames, value);
            }
        }
        public String SearchCriteria
        {
            get { return GetValue(() => SearchCriteria); }
            set
            {
                SetValue(() => SearchCriteria, value);
                this.searchOrder();
            }
        }
        #endregion

        #region methods

        private async void initializObj()
        {
            Units = _unitService.Queryable().ToList();

            if (UserLog.UniqueInstance.LogedEmployee != null)
            {
                var dtnDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(APPSettings.Default.employeeCer);
                if (dtnDictionary != null)
                {
                    if (dtnDictionary.ContainsKey(UserLog.UniqueInstance.LogedEmployee.BudgetNo.ToString()))
                    {
                        _identificationCode = GlobalClass.DecryptStringAES(dtnDictionary[UserLog.UniqueInstance.LogedEmployee.BudgetNo.ToString()], "66Ak679Du4V3qo92");
                    }
                }
                await getOrgansAsync();
            }
        }
        
        private Task getOrgansAsync()
        {
            Task ts = new Task(() =>
            {
                var items = _seedDataHelper.GetOrganizations().ToList();
                DispatchService.Invoke(() =>
                {
                    OrganNames = items;
                });
            });
            ts.Start();
            return ts;
        }

        private async void confirmExOrder()
        {
            if (GlobalClass.CheckForInternetConnection(Settings.Default.ServerUrl))
            {
                _dialogService.ShowAlert("توجه", ErrorMessages.Default.NoInternetAccess);
                return;
            }
            if (SelectedOrder == null)
            {
                _dialogService.ShowError("انتخاب درخواست", "هیچ درخواستی انتخاب نشده است");
                return;
            }
            bool isConfirm = _dialogService.AskConfirmation("توجه", ErrorMessages.Default.AskConfrimation);
            if (isConfirm)
            {
                SelectedOrder.Status = 2;
                try
                {
                    await UpdateOrderAsync($"Order/{SelectedOrder.OrderId}", SelectedOrder);
                    _dialogService.ShowInfo("توجه", ErrorMessages.Default.SuccessfullyMessage);
                    this.initOrdes();
                }
                catch (HttpRequestException ex)
                {
                    _dialogService.ShowError("خطا در بروز رسانی اطلاعات", ex.Message);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        private async void rejectExOrder()
        {
            if (GlobalClass.CheckForInternetConnection(Settings.Default.ServerUrl))
            {
                _dialogService.ShowAlert("توجه", ErrorMessages.Default.NoInternetAccess);
                return;
            }
            if (SelectedOrder == null)
            {
                _dialogService.ShowError("انتخاب درخواست", "هیچ درخواستی انتخاب نشده است");
                return;
            }
            bool isConfirm = _dialogService.AskConfirmation("توجه", ErrorMessages.Default.AskConfrimation);
            if (isConfirm)
            {
                SelectedOrder.Status = 5;
                try
                {
                    await UpdateOrderAsync($"Order/{SelectedOrder.OrderId}", SelectedOrder);
                    _dialogService.ShowInfo("توجه", ErrorMessages.Default.SuccessfullyMessage);
                    this.initOrdes();
                }
                catch (HttpRequestException ex)
                {
                    _dialogService.ShowError("خطا در بروز رسانی اطلاعات", ex.Message);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        private void searchOrder()
        {
            ExOrderFilteredView.Filter = (obj) =>
            {
                var exMoel = obj as ExternalOrderModel;
                return exMoel.OrderId.ToString().StartsWith(SearchCriteria);
            };
        }

        private async void initOrdes()
        {
            if (GlobalClass.CheckForInternetConnection(Settings.Default.ServerUrl))
            {
                _dialogService.ShowAlert("توجه", ErrorMessages.Default.NoInternetAccess);
                return;
            }

            if (string.IsNullOrEmpty(_identificationCode))
            {
                return;
            }
            await GetOrdrsAsync($"Order/{_identificationCode}");
        }

        private async Task GetOrdrsAsync(string uri)
        {
            HttpResponseMessage response = await _client.GetAsync(uri);
            if (response.IsSuccessStatusCode)
            {
                var items = await response.Content.ReadAsAsync<ExternalOrderModel[]>();
                DispatchService.Invoke(() =>
                {
                    _exOrderModel.Clear();
                    items.ForEach(o =>
                    {
                        _exOrderModel.Add(o);
                    });
                });
            }
        }

        private async void showExOrderDetailsWin(IList<object> parameters)
        {
            var exOrder = parameters[0] as ExternalOrderModel;
            if (exOrder == null) return;
            this.SelectedOrder = exOrder;
            Mouse.SetCursor(Cursors.Wait);
            var items = await getExternalOrderDetails($"Order/byOrderDetails/{exOrder.OrderId}");
            _exOrderDetails.Clear();
            items.ForEach(exd =>
            {
                _exOrderDetails.Add(exd);
            });
            Mouse.SetCursor(Cursors.Arrow);
        }

        private async Task<List<ExternalOrderDetailsModel>> getExternalOrderDetails(string uri)
        {
            HttpResponseMessage response = await _client.GetAsync(uri);
            if (response.IsSuccessStatusCode)
            {
                var items = await response.Content.ReadAsAsync<ExternalOrderDetailsModel[]>();
                return items.ToList();
            }
            return null;
        }

        private async Task<Uri> UpdateOrderAsync(string uri, ExternalOrderModel model)
        {
            HttpResponseMessage response = null;
            response = await _client.PutAsJsonAsync(uri, model.Status);
            response.EnsureSuccessStatusCode();
            return response?.Headers.Location;
        }

        #endregion

        #region commands
        
        public ICommand ConfirmEXCommand { get; private set; }
        public ICommand RejectEXCommand { get; private set; }
        public ICommand RemoveCommand { get; private set; }
        public ICommand ExOrderDetailsCommand { get; private set; }
        private void initializCommands()
        {
            ConfirmEXCommand = new MvvmCommand(
              (parameter) => { this.confirmExOrder(); },
              (parameter) =>
              {
                  if (SelectedOrder == null) return false;
                  else return SelectedOrder.Status==1;
              }
              ).AddListener<ExternalOrderRecivedViewModel>(this, x => x.SelectedOrder);

            RejectEXCommand = new MvvmCommand(
                 (parameter) => { this.rejectExOrder(); },
                (parameter) => {
                    if (SelectedOrder == null) return false;
                    else return SelectedOrder.Status == 1;
                }
                ).AddListener<ExternalOrderRecivedViewModel>(this,x=>x.SelectedOrder);

            ExOrderDetailsCommand = new MvvmCommand(
               (parameter) => { this.showExOrderDetailsWin(parameter as IList<object>); },
               (parameter) => { return true; }
               );
        }

        #endregion

        #region fields

        private readonly IUnityContainer _container;
        private readonly IUnitService _unitService;
        private readonly IDialogService _dialogService;
        private readonly INavigationService _navigationService;
        private readonly ObservableCollection<ExternalOrderModel> _exOrderModel;
        private readonly ObservableCollection<ExternalOrderDetailsModel> _exOrderDetails;
        private readonly HttpClient _client;
        private readonly SeedDataHelper _seedDataHelper;
        private string _identificationCode;

        #endregion

    }
}

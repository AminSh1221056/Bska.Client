
namespace Bska.Client.UI.ViewModels.StuffHonestViewModel
{
    using Microsoft.Practices.Unity;
    using Bska.Client.UI.Helper;
    using System;
    using System.Windows.Input;
    using System.Collections.ObjectModel;
    using Domain.Entity;
    using Data.Service;
    using System.Threading.Tasks;
    using Common;
    using System.Linq;
    using System.Collections.Generic;
    using API;
    using Services;
    using System.Net.Http;
    using System.Windows;
    using System.ComponentModel;
    using System.Windows.Data;
    using Newtonsoft.Json;
    using Bska.Client.Repository.Model;

    public sealed class ExternalOrderViewModel : BaseViewModel
    {
        #region ctor

        public ExternalOrderViewModel(IUnityContainer container)
        {
            this._container = container;
            this._unitService = _container.Resolve<IUnitService>();
            this._dialogService = _container.Resolve<IDialogService>();
            this._navigationService = _container.Resolve<INavigationService>();
            this._exOrderModel = new ObservableCollection<ExternalOrderModel>();
            this._client = UserLog.UniqueInstance.Client;
            this.ExOrderFilteredView= new CollectionViewSource { Source = ExOrderModel }.View;
            this._seedDataHelper = new SeedDataHelper();
            this.initializObj();
            this.initializCommands();
        }
        #endregion

        #region properties
        public Window Window
        {
            get { return GetValue(() => Window); }
            set
            {
                SetValue(() => Window, value);
            }
        }

        public List<Unit> Units
        {
            get { return GetValue(() => Units); }
            set
            {
                SetValue(() => Units, value);
            }
        }

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

        public string IdentityInfo
        {
            get { return GetValue(() => IdentityInfo); }
            set
            {
                SetValue(() => IdentityInfo, value);
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

        public Boolean AllOrderShow
        {
            get { return GetValue(() => AllOrderShow); }
            set
            {
                SetValue(() => AllOrderShow, value);
                if (value)
                {
                    this.initOrdes();
                }
            }
        }

        public Boolean RecivedExOrderShow
        {
            get { return GetValue(() => RecivedExOrderShow); }
            set
            {
                SetValue(() => RecivedExOrderShow, value);
                if (value)
                {
                    this.initRecivedOrders();
                }
            }
        }

        public ICollectionView ExOrderFilteredView { get; set; }

        public String SearchCriteria
        {
            get { return GetValue(() => SearchCriteria); }
            set
            {
                SetValue(() => SearchCriteria, value);
                this.searchExOrder();
            }
        }
        public List<OrganizationModel> OrganNames
        {
            get { return GetValue(() => OrganNames); }
            set
            {
                SetValue(() => OrganNames, value);
            }
        }
        
        #endregion

        #region methods

        private async void initializObj()
        {
            IdentityInfo = "درحال شناسایی هویت...";
            Units = _unitService.Queryable().ToList();
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
            this.PerformIdentityAccuracy();
            await getOrgansAsync();
        }

        private void searchExOrder()
        {
            ExOrderFilteredView.Filter = (obj) =>
            {
                var exMoel = obj as ExternalOrderModel;
                return exMoel.OrderId.ToString().StartsWith(SearchCriteria);
            };
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

        private async void initOrdes()
        {
            if (Customer == null)
            {
                _dialogService.ShowAlert("توجه", "هویت سازمان شناخته شده نیست");
                AllOrderShow = false;
                return;
            }
            await GetOrdrsAsync($"Order/{IdentificationCode}");
        }

        private async void initRecivedOrders()
        {
            if (Customer == null)
            {
                _dialogService.ShowAlert("توجه", "هویت سازمان شناخته شده نیست");
                RecivedExOrderShow = false;
                return;
            }
            await GetOrdrsAsync($"Order/byRecivedOrder/{Customer.BudgetNo}");
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
                string path = $"Customer/{IdentificationCode}";
                Customer = await this.GetCustomerAsync(path);
                if (Customer != null)
                {
                    IdentityInfo = "هویت این سازمان شناسایی شد";
                }
                else
                {
                    IdentityInfo = "هیچ سازمانی یافت نشد";
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
            StoryboardManager.PlayStoryboard("StoryboardFadeOut", this.Window);
            var viewModel = new ExternalOrderDetailsViewModel();
            viewModel.CurrentExOrder = exOrder;
            viewModel.Units = this.Units;
            viewModel.ExOrderCollection = await getExternalOrderDetails($"Order/byOrderDetails/{exOrder.OrderId}");
            _navigationService.ShowExternalOrderDetailsWindow(viewModel);
            StoryboardManager.PlayStoryboard("StoryboardFadeIn", this.Window);
            Mouse.SetCursor(Cursors.Arrow);
        }

        private void showAddExternalWindow()
        {
            if (Customer == null)
            {
                _dialogService.ShowAlert("توجه", "هویت سازمان شناخته شده نیست");
                RecivedExOrderShow = false;
                return;
            }
            Mouse.SetCursor(Cursors.Wait);
            StoryboardManager.PlayStoryboard("StoryboardFadeOut", this.Window);
            var viewModel = new AddExternalOrderViewModel(_container) { Units=this.Units,OrganNames=this.OrganNames,IdentificationCode=this.IdentificationCode};
            _navigationService.ShowAddExternalWindow(viewModel);
            StoryboardManager.PlayStoryboard("StoryboardFadeIn", this.Window);
            Mouse.SetCursor(Cursors.Arrow);
        }

        private async Task<List<ExternalOrderDetailsModel>> getExternalOrderDetails(string uri)
        {
            HttpResponseMessage response = await _client.GetAsync(uri);
            if (response.IsSuccessStatusCode)
            {
                var items= await response.Content.ReadAsAsync<ExternalOrderDetailsModel[]>();
                return items.ToList();
            }
            return null;
        }

        #endregion

        #region commands

        public ICommand ExOrderDetailsCommand { get; private set; }
        public ICommand ShowExOrderWindowCommand { get; private set; }
        private void initializCommands()
        {
            ExOrderDetailsCommand = new MvvmCommand(
                (parameter) => { this.showExOrderDetailsWin(parameter as IList<object>); },
                (parameter) => { return true; }
                );

            ShowExOrderWindowCommand = new MvvmCommand(
                (parameter) => { this.showAddExternalWindow(); },
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
        private readonly SeedDataHelper _seedDataHelper;
        private readonly HttpClient _client;
      
        #endregion
    }
}

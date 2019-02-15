
namespace Bska.Client.UI.ViewModels
{
    using Bska.Client.Data.Service;
    using Bska.Client.UI.API;
    using Bska.Client.UI.Helper;
    using Bska.Client.UI.Services;
    using Microsoft.Practices.Unity;
    using System;
    using System.Windows.Input;
    using System.Collections.Generic;
    using System.Linq;
    using Bska.Client.Common;
    using Bska.Client.Domain.Entity.AssetEntity.Meters;
    using System.Data.Entity;
    using System.Threading.Tasks;
    using System.Net.Http;
    using Repository.Model;
    using Newtonsoft.Json;
    using System.Text;

    public sealed class BaseInfoSendViewModel : BaseViewModel
    {
        #region ctor

        public BaseInfoSendViewModel(IUnityContainer container)
        {
            this._container = container;
            this._dialogService = _container.Resolve<IDialogService>();
            this._personSerivce = _container.Resolve<IPersonService>();
            this._employeeService = _container.Resolve<IEmployeeService>();
            this._buildingService = _container.Resolve<IBuildingService>();
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
        public Boolean IsAuthenticated
        {
            get { return GetValue(() => IsAuthenticated); }
            set
            {
                SetValue(() => IsAuthenticated, value);
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
        public CustomerModel Customer
        {
            get { return GetValue(() => Customer); }
            set
            {
                SetValue(() => Customer, value);
            }
        }

        public string PersonReport
        {
            get { return GetValue(() => PersonReport); }
            set
            {
                SetValue(() => PersonReport, value);
            }
        }

        public string EstateReport
        {
            get { return GetValue(() => EstateReport); }
            set
            {
                SetValue(() => EstateReport, value);
            }
        }

        public string MeterReport
        {
            get { return GetValue(() => MeterReport); }
            set
            {
                SetValue(() => MeterReport, value);
            }
        }
        #endregion

        #region methods

        private void initalizObj()
        {
            PersonReport=EstateReport=MeterReport = "";
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

        private async void sendPersons()
        {
            if (this.Customer == null)
            {
                _dialogService.ShowAlert("توجه", "هویت ناشناخته است");
                return;
            }
            try
            {
                var persons = _personSerivce.Queryable().Where(p => p.PersonId != 1).AsEnumerable().ToList();
                PersonReport = "در حال ارسال اطلاعات پرسنل";
                await CreateCustomerAssetAsync(persons, $"Exchange/bySendPerson/{IdentificationCode}");
                PersonReport = "";
                _dialogService.ShowInfo("توجه", "عملیات ارسال اطلاعات پرسنل با موفقیت انجام شد");
            }
            catch (HttpRequestException ex)
            {
                _dialogService.ShowError("خطا در بروز رسانی اطلاعات", ex.Message + ".برای اطلاعات بیشتر به آدرس زیر مراجعه کنید" + ex.HelpLink);
                EstateReport = "";
            }
            catch (Exception ex)
            {
                throw ex;
            }
           
        }

        private async void sendEstate()
        {
            if (this.Customer == null)
            {
                _dialogService.ShowAlert("توجه", "هویت ناشناخته است");
                return;
            }

            var estates = _employeeService.Queryable().SelectMany(e => e.Estates).AsEnumerable().ToList();

            try
            {
                EstateReport = "در حال ارسال اطلاعات املاک";
                await CreateCustomerAssetAsync(estates, $"Exchange/bySendEstates/{IdentificationCode}");
                EstateReport = "";
                _dialogService.ShowInfo("توجه", "عملیات ارسال اطلاعات املاک با موفقیت انجام شد");
            }
            catch (HttpRequestException ex)
            {
                _dialogService.ShowError("خطا در بروز رسانی اطلاعات", ex.Message + ".برای اطلاعات بیشتر به آدرس زیر مراجعه کنید" + ex.HelpLink);
                EstateReport = "";
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private async void sendMeter()
        {
            if (this.Customer == null)
            {
                _dialogService.ShowAlert("توجه", "هویت ناشناخته است");
                return;
            }
            var meters = _buildingService.Queryable().SelectMany(b => b.Meters).Include(m => m.MeterBills).AsEnumerable().ToLookup(m => m.GetType());
            try
            {
                MeterReport = "در حال ارسال اطلاعات کنتور ها";
                await CreateCustomerMeterAssetAsync(meters, $"Exchange/bySendMeters/{IdentificationCode}");
                MeterReport = "";
                _dialogService.ShowInfo("توجه", "عملیات ارسال اطلاعات کنتور ها با موفقیت انجام شد");
            }
            catch (HttpRequestException ex)
            {
                _dialogService.ShowError("خطا در بروز رسانی اطلاعات", ex.Message + ".برای اطلاعات بیشتر به آدرس زیر مراجعه کنید" + ex.HelpLink);
                MeterReport = "";
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private async Task<Uri> CreateCustomerAssetAsync<T>(IEnumerable<T> model, string uri)
        {
            HttpResponseMessage response = null;

            var sendModel = model;

            response = await _client.PutAsJsonAsync(uri, sendModel);
            response.EnsureSuccessStatusCode();
            return response?.Headers.Location;
        }

        private async Task CreateCustomerMeterAssetAsync(ILookup<Type,Meter> model,string uri)
        {
            HttpResponseMessage response = null;
            foreach(var m in model)
            {
                var sendModel = new MeterSendModel { EmployeeCode = IdentificationCode, Items = m.ToList() };
                if (m.Key == typeof(PowerMeter))
                {
                    sendModel.Type = 1;
                }
                else if (m.Key == typeof(GasMeter))
                {
                    sendModel.Type = 2;
                }
                else if (m.Key == typeof(WaterMeter))
                {
                    sendModel.Type = 3;
                }
                else if (m.Key == typeof(TellMeter))
                {
                    sendModel.Type = 4;
                }
                else if (m.Key == typeof(MobileMeter))
                {
                    sendModel.Type = 5;
                }
                else
                {
                    sendModel.Type = 0;
                }
                string sendDate = JsonConvert.SerializeObject(sendModel, Formatting.None,
                            new JsonSerializerSettings()
                            {
                                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                            });
                response = await _client.PostAsync(uri, new StringContent(sendDate, Encoding.UTF8, "application/json"));
                response.EnsureSuccessStatusCode();
            }
        }
        #endregion

        #region commands

        public ICommand IdentifyCommand { get; private set; }
        public ICommand SendPersonCommand { get; private set; }
        public ICommand SendEstateCommand { get; private set; }
        public ICommand SendMeterCommand { get; private set; }
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

            SendPersonCommand = new MvvmCommand(
                (paremter) => { this.sendPersons(); },
                (parameter) => { return this.PersonReport.Length==0; }
                ).AddListener<BaseInfoSendViewModel>(this,x=>x.PersonReport);

            SendEstateCommand=new MvvmCommand((paremter) => { this.sendEstate(); },
                (parameter) => { return this.EstateReport.Length == 0; }
                ).AddListener<BaseInfoSendViewModel>(this, x => x.EstateReport);

            SendMeterCommand= new MvvmCommand((paremter) => { this.sendMeter(); },
                (parameter) => { return this.MeterReport.Length == 0; }
                ).AddListener<BaseInfoSendViewModel>(this, x => x.MeterReport);

        }

        #endregion

        #region fields

        private readonly IUnityContainer _container;
        private readonly IDialogService _dialogService;
        private readonly IPersonService _personSerivce;
        private readonly IEmployeeService _employeeService;
        private readonly IBuildingService _buildingService;
        private readonly HttpClient _client;

        #endregion

    }
}


namespace Bska.Client.UI.ViewModels
{
    using Bska.Client.Common;
    using Bska.Client.UI.API;
    using Bska.Client.UI.Helper;
    using Bska.Client.UI.Services;
    using Microsoft.Practices.Unity;
    using System;
    using System.Windows.Input;
    using Domain.Entity;
    using Client.API.Infrastructure;
    using Bska.Client.UI.API.Client;
    using System.Threading.Tasks;

    public sealed class SeedDataUpdateViewModel : BaseViewModel
    {
        #region ctor

        public SeedDataUpdateViewModel(IUnityContainer container)
        {
            this._container = container;
            this._navigationService = _container.Resolve<INavigationService>();
            this._dialogService = _container.Resolve<IDialogService>();
            this._baseInfoService = _container.Resolve<IBaseInfoService>();
            this.initializObj();
            this.initalizCommands();
        }

        #endregion

        #region properties
        
        public string ReportStuff
        {
            get { return GetValue(() => ReportStuff); }
            set
            {
                SetValue(() => ReportStuff, value);
            }
        }
        public string ReportUnit
        {
            get { return GetValue(() => ReportUnit); }
            set
            {
                SetValue(() => ReportUnit, value);
            }
        }
        
        public string ReportOrgan
        {
            get { return GetValue(() => ReportOrgan); }
            set
            {
                SetValue(() => ReportOrgan, value);
            }
        }

        public string ReportInsurance
        {
            get { return GetValue(() => ReportInsurance); }
            set
            {
                SetValue(() => ReportInsurance, value);
            }
        }

        public string ReportState
        {
            get { return GetValue(() => ReportState); }
            set
            {
                SetValue(() => ReportState, value);
            }
        }

        public string ReportAutomobile
        {
            get { return GetValue(() => ReportAutomobile); }
            set
            {
                SetValue(() => ReportAutomobile, value);
            }
        }

        public string ReportCountry
        {
            get { return GetValue(() => ReportCountry); }
            set
            {
                SetValue(() => ReportCountry, value);
            }
        }

        public string ReportEStateOrgan
        {
            get { return GetValue(() => ReportEStateOrgan); }
            set
            {
                SetValue(() => ReportEStateOrgan, value);
            }
        }
        public string ReportAccountCoding
        {
            get { return GetValue(() => ReportAccountCoding); }
            set
            {
                SetValue(() => ReportAccountCoding, value);
            }
        }

        public double Report
        {
            get { return GetValue(() => Report); }
            set
            {
                SetValue(() => Report, value);
            }
        }
        public string ReportString
        {
            get { return GetValue(() => ReportString); }
            set
            {
                SetValue(() => ReportString, value);
            }
        }
        
        #endregion

        #region methods

        private void initializObj()
        {
            this.ReportStuff=ReportAccountCoding=ReportUnit=ReportOrgan=ReportInsurance=ReportCountry=ReportAutomobile =ReportState=ReportEStateOrgan="";
            //if (UserLog.UniqueInstance.LogedEmployee != null)
            //{
            //    var dtnDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(APPSettings.Default.employeeCer);
            //    if (dtnDictionary != null)
            //    {
            //        if (dtnDictionary.ContainsKey(UserLog.UniqueInstance.LogedEmployee.BudgetNo.ToString()))
            //        {
            //            IdentificationCode = GlobalClass.DecryptStringAES(dtnDictionary[UserLog.UniqueInstance.LogedEmployee.BudgetNo.ToString()], "66Ak679Du4V3qo92");
            //        }
            //    }
            //}
        }
        
        private async void UpdateStuffByWebApi()
        {
            if (GlobalClass.CheckForInternetConnection(Settings.Default.ServerUrl))
            {
                _dialogService.ShowAlert("توجه", ErrorMessages.Default.NoInternetAccess);
                return;
            }
            
            ReportStuff = "درحال بروز رسانی اموال...";
            int ok = await _baseInfoService.saveStuffAsync(HelpPageingAddress.Default.StuffApiLink);
            if (ok > 0)
            {
                ReportStuff = "";
                UserLog.UniqueInstance.AddLog(new BskaUIHelper().eventLogCreator("بروز رسانی فایل اموال", EventType.Update));
                _dialogService.ShowInfo("توجه", "فایل اموال با موفقیت ذخیره شد");
                return;
            }
            ReportStuff = "مشکل در بروز رسانی رخ داد";
        }

        private async void UpdateUnitFileByWebApi()
        {
            if (GlobalClass.CheckForInternetConnection(Settings.Default.ServerUrl))
            {
                _dialogService.ShowAlert("توجه", ErrorMessages.Default.NoInternetAccess);
                return;
            }

            ReportUnit = "درحال بروز رسانی واحد ها...";
            int ok=await _baseInfoService.saveUnitAsync(HelpPageingAddress.Default.UnitApiLink);
            if (ok > 0)
            {
                ReportUnit = "";
                UserLog.UniqueInstance.AddLog(new BskaUIHelper().eventLogCreator("بروز رسانی فایل واحدها", EventType.Update));
                _dialogService.ShowInfo("توجه", "فایل واحد ها با موفقیت ذخیره شد");
            }
            else
            {
                ReportUnit = "مشکل در بروز رسانی رخ داد";
            }
        }

        //private async void UpdateAccountCodingByWebApi()
        //{
        //    if (GlobalClass.CheckForInternetConnection(Settings.Default.ServerUrl))
        //    {
        //        _dialogService.ShowAlert("توجه", ErrorMessages.Default.NoInternetAccess);
        //        return;
        //    }

        //    ReportAccountCoding = "درحال بروز رسانی سرفصل حسابداری...";
        //    int ok = await _baseInfoService.saveAccounCodingAsync(HelpPageingAddress.Default.AccountCodingLink);
        //    if (ok > 0)
        //    {
        //        ReportAccountCoding = "";
        //        UserLog.UniqueInstance.AddLog(new BskaUIHelper().eventLogCreator("بروز رسانی فایل سرفصل حسابها", EventType.Update));
        //        _dialogService.ShowInfo("توجه", "فایل سرفصل حسابها با موفقیت ذخیره شد");
        //    }
        //    else
        //    {
        //        ReportAccountCoding = "مشکل در بروز رسانی رخ داد";
        //    }
        //}

        private async void UpdateCompanyAndCountryFileByWebApi()
        {
            if (GlobalClass.CheckForInternetConnection(Settings.Default.ServerUrl))
            {
                _dialogService.ShowAlert("توجه", ErrorMessages.Default.NoInternetAccess);
                return;
            }

            ReportCountry = "در حال بروز رسانی کشورها و کارخانه ها";
            int ok = await _baseInfoService.saveCountryAsync(HelpPageingAddress.Default.CountryApiLink, HelpPageingAddress.Default.CountryApiLink);
            if (ok > 0)
            {
                ReportCountry = "";
                UserLog.UniqueInstance.AddLog(new BskaUIHelper().eventLogCreator("بروز رسانی فایل کشورها و کارخانه ها", EventType.Update));
               
                _dialogService.ShowInfo("پایان عملیات", "فایل کشور ها و کارخانه ها با موفقیت ذخیره شد");
            }
            else
            {
                ReportInsurance = "مشکل در بروز رسانی";
            }
        }

        private async void UpdateInsuranceCompanyByWebApi()
        {
            if (GlobalClass.CheckForInternetConnection(Settings.Default.ServerUrl))
            {
                _dialogService.ShowAlert("توجه", ErrorMessages.Default.NoInternetAccess);
                return;
            }

            ReportInsurance = "در حال بروز رسانی شرکت های بیمه...";
            int ok = await _baseInfoService.saveInsuranceAsync(HelpPageingAddress.Default.InsuranceApiLink);
            if (ok > 0)
            {
                ReportInsurance = "";
                UserLog.UniqueInstance.AddLog(new BskaUIHelper().eventLogCreator("بروز رسانی فایل بیمه", EventType.Update));
               
                _dialogService.ShowInfo("پایان عملیات", "فایل شرکت های بیمه با موفقیت ذخیره شد");
            }
            else
            {
                ReportInsurance = "مشکل در بروز رسانی";
            }
        }

        private async void UpdateCarCompanyByWebApi()
        {
            if (GlobalClass.CheckForInternetConnection(Settings.Default.ServerUrl))
            {
                _dialogService.ShowAlert("توجه", ErrorMessages.Default.NoInternetAccess);
                return;
            }

            ReportAutomobile = "در حال بروز رسانی شرکت های اتومبیل...";
            int ok = await _baseInfoService.saveCarCompanyAsync(HelpPageingAddress.Default.CarDetailsApiLink);
            if (ok > 0)
            {
                ReportAutomobile = "";
                UserLog.UniqueInstance.AddLog(new BskaUIHelper().eventLogCreator("بروز رسانی فایل اتومبیل ها", EventType.Update));
               
                _dialogService.ShowInfo("پایان عملیات", "فایل شرکت های اتومبیل با موفقیت ذخیره شد");
            }
            else
            {
                ReportAutomobile = "مشکل در بروز رسانی رخ داد";
            }
        }

        private async void updateOrganByWebApi()
        {
            if (GlobalClass.CheckForInternetConnection(Settings.Default.ServerUrl))
            {
                _dialogService.ShowAlert("توجه", ErrorMessages.Default.NoInternetAccess);
                return;
            }

            ReportOrgan = "در حال بروز رسانی فایل سازمانها...";
            int ok = await _baseInfoService.saveOrganAsync(HelpPageingAddress.Default.OrganizationApiLink);
            if (ok>0)
            {
                ReportOrgan = "";
                UserLog.UniqueInstance.AddLog(new BskaUIHelper().eventLogCreator("بروز رسانی فایل سازمانها", EventType.Update));
                _dialogService.ShowInfo("پایان عملیات", "فایل سازمانها با موفقیت ذخیره شد");
            }
            else
            {
                ReportOrgan = "مشکل در بروز رسانی رخ داد";
            }
        }

        private async void updateStateByWebApi()
        {
            if (GlobalClass.CheckForInternetConnection(Settings.Default.ServerUrl))
            {
                _dialogService.ShowAlert("توجه", ErrorMessages.Default.NoInternetAccess);
                return;
            }

            ReportState = "در حال بروز رسانی فایل تقسیمات استانی...";
            int ok = await _baseInfoService.saveStateAsync(HelpPageingAddress.Default.ProvinceApiLink, HelpPageingAddress.Default.TownShipsApiLink
                , HelpPageingAddress.Default.ZonesApiLink, HelpPageingAddress.Default.CitiesApiLink);
            if (ok > 0)
            {
                ReportState = "";
                UserLog.UniqueInstance.AddLog(new BskaUIHelper().eventLogCreator("بروز رسانی فایل تقسیمات استانی", EventType.Update));
               
                _dialogService.ShowInfo("پایان عملیات", "فایل تقسمات استانی با موفقیت ذخیره شد");
            }
            else
            {
                ReportState = "مشکل در بروز رسانی رخ داد";
            }
        }

        private async void updateEstateOrganByWebApi()
        {
            if (GlobalClass.CheckForInternetConnection(Settings.Default.ServerUrl))
            {
                _dialogService.ShowAlert("توجه", ErrorMessages.Default.NoInternetAccess);
                return;
            }

            ReportEStateOrgan = "در حال بروز رسانی فایل مراکز ثبت املاک...";
            int ok = await _baseInfoService.saveEstateOrganAsync(HelpPageingAddress.Default.EstateOrganApiLink);
            if (ok > 0)
            {
                ReportEStateOrgan = "";
                UserLog.UniqueInstance.AddLog(new BskaUIHelper().eventLogCreator("بروز رسانی فایل مراکز ثبت املاک", EventType.Update));
               
                _dialogService.ShowInfo("پایان عملیات", "فایل مراکز ثبت املاک با موفقیت ذخیره شد");
            }
            else
            {
                ReportState = "مشکل در بروز رسانی رخ داد";
            }
        }

        private async void allUpdate()
        {
            if (GlobalClass.CheckForInternetConnection(Settings.Default.ServerUrl))
            {
                _dialogService.ShowAlert("توجه", ErrorMessages.Default.NoInternetAccess);
                return;
            }

            Report = 1;
            var progressIndicator = new Progress<double>(ReportProgress);
           
            var ts1 = _baseInfoService.saveUnitAsync(HelpPageingAddress.Default.UnitApiLink, progressIndicator);
            var ts2 = _baseInfoService.saveCountryAsync(HelpPageingAddress.Default.CountryApiLink, HelpPageingAddress.Default.CompanyApiLink, progressIndicator);
            var ts3 = _baseInfoService.saveCarCompanyAsync(HelpPageingAddress.Default.CarDetailsApiLink, progressIndicator);
            var ts4 = _baseInfoService.saveInsuranceAsync(HelpPageingAddress.Default.InsuranceApiLink, progressIndicator);
            var ts5 = _baseInfoService.saveOrganAsync(HelpPageingAddress.Default.OrganizationApiLink, progressIndicator);
            var ts6 = _baseInfoService.saveStateAsync(HelpPageingAddress.Default.ProvinceApiLink, HelpPageingAddress.Default.TownShipsApiLink
                , HelpPageingAddress.Default.ZonesApiLink, HelpPageingAddress.Default.CitiesApiLink, progressIndicator);
            var ts7 = _baseInfoService.saveEstateOrganAsync(HelpPageingAddress.Default.EstateOrganApiLink, progressIndicator);
            //var ts8 = _baseInfoService.saveAccounCodingAsync(HelpPageingAddress.Default.AccountCodingLink, progressIndicator);

            var ts = _baseInfoService.saveStuffAsync(HelpPageingAddress.Default.StuffApiLink, progressIndicator);
            await Task.WhenAll(ts, ts1, ts2, ts3, ts4, ts5, ts6, ts7);

           // await ts.ContinueWith(t => ts1
           //.ContinueWith(s=> ts2
           //.ContinueWith(v=>ts3
           //.ContinueWith(j=>ts4
           //.ContinueWith(h=> ts5
           //.ContinueWith(i=>ts6.ContinueWith(g=>ts7.ContinueWith(n=>ts8))))))));

            UserLog.UniqueInstance.AddLog(new EventLog
            {
                EntryDate = GlobalClass._Today,
                Key = UserLog.UniqueInstance.LogedUser.FullName,
                Message = "بروز رسانی کلی اطلاعات اولیه ",
                ObjectState = ObjectState.Added,
                Type = EventType.Update,
                UserId = UserLog.UniqueInstance.LogedUser.UserId
            });
        }
        
        void ReportProgress(double value)
        {
            Report = value;
           _baseInfoService.CurrentProgress = value;
            if (_baseInfoService.CurrentProgress == 100)
            {
                _dialogService.ShowInfo("توجه", "عملیات بروز رسانی با موفقیت انجام شد");
                ReportString = "";
                Report = 0;
                _baseInfoService.CurrentProgress = 0;
            }
        }

        #endregion

        #region commands

        public ICommand UpdateStuffCommand { get; private set; }
        public ICommand UpdateCountryCommand { get; private set; }
        public ICommand UpdateInsuranceCommand { get; private set; }
        public ICommand UpdateUnitCommand { get; private set; }
        public ICommand UpdateCarCommand { get; private set; }
        public ICommand UpdateOrganCommand { get; private set; }
        public ICommand AllUpdateCommand { get; private set; }
        public ICommand UpdateStateCommand { get; private set; }
        public ICommand EstateOrganCommand { get; private set; }
        public ICommand AccountCodingCommand { get; private set; }
        private void initalizCommands()
        {
            UpdateStuffCommand = new MvvmCommand(
                  (parameter) =>
                  {
                      UpdateStuffByWebApi();
                  },
                  (parameter) =>
                  {
                      return this.ReportStuff=="";
                  }
                  ).AddListener<SeedDataUpdateViewModel>(this, s => s.ReportStuff);

            UpdateUnitCommand = new MvvmCommand(
                (parameter) =>
                {
                    UpdateUnitFileByWebApi();
                },
                (parameter) =>
                {
                    return ReportUnit=="";
                }
                ).AddListener<SeedDataUpdateViewModel>(this, s => s.ReportUnit);

            UpdateCountryCommand = new MvvmCommand(
                (parameter) =>
                {
                    UpdateCompanyAndCountryFileByWebApi();
                },
                (parameter) =>
                {
                    return ReportCountry=="";
                }
                ).AddListener<SeedDataUpdateViewModel>(this, s => s.ReportCountry);

            UpdateInsuranceCommand = new MvvmCommand(
                (parameter) =>
                {
                    UpdateInsuranceCompanyByWebApi();
                },
                (parameter) =>
                {
                    return ReportInsurance=="";
                }
                ).AddListener<SeedDataUpdateViewModel>(this, s => s.ReportInsurance);

            UpdateCarCommand = new MvvmCommand(
                 (parameter) =>
                 {
                     UpdateCarCompanyByWebApi();
                 },
                (parameter) =>
                {
                    return ReportAutomobile=="";
                }
                ).AddListener<SeedDataUpdateViewModel>(this, s => s.ReportAutomobile);

            UpdateOrganCommand = new MvvmCommand(
                 (parameter) =>
                 {
                     updateOrganByWebApi();
                 },
                (parameter) =>
                {
                    return ReportOrgan=="";
                }
                ).AddListener<SeedDataUpdateViewModel>(this, s => s.ReportOrgan);

            AccountCodingCommand = new MvvmCommand(
                 (parameter) =>
                 {
                     //UpdateAccountCodingByWebApi();
                 },
                (parameter) =>
                {
                    return ReportAccountCoding == "";
                }
                ).AddListener<SeedDataUpdateViewModel>(this, s => s.ReportAccountCoding);

            UpdateStateCommand = new MvvmCommand(
                 (parameter) =>
                 {
                     updateStateByWebApi();
                 },
                (parameter) =>
                {
                    return ReportState == "";
                }
                ).AddListener<SeedDataUpdateViewModel>(this, s => s.ReportState);


            EstateOrganCommand = new MvvmCommand(
                (parameter) => { updateEstateOrganByWebApi(); },
                (parameter) => { return ReportEStateOrgan == ""; }
                ).AddListener<SeedDataUpdateViewModel>(this, s => s.ReportEStateOrgan);

            AllUpdateCommand = new MvvmCommand(
                (parameter) =>
                {
                    this.allUpdate();
                },
                (parameter) =>
                {
                    return Report==0;
                }
                ).AddListener<SeedDataUpdateViewModel>(this,s=>s.Report);
            
        }

        #endregion

        #region fields

        private readonly IUnityContainer _container;
        private readonly IDialogService _dialogService;
        private readonly INavigationService _navigationService;
        private readonly IBaseInfoService _baseInfoService;

        #endregion
    }
}

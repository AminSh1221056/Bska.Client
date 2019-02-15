
namespace Bska.Client.UI.ViewModels
{
    using Bska.Client.Common;
    using Bska.Client.UI.API;
    using Bska.Client.UI.Services;
    using Microsoft.Practices.Unity;
    using System;
    using System.Text.RegularExpressions;
    using System.Windows.Input;
    public sealed class SSRSConfigViewModel : BaseViewModel
    {
        #region ctor

        public SSRSConfigViewModel(IUnityContainer container)
        {
            this._container = container;
            this._dialogService = _container.Resolve<IDialogService>();
            this.initalizObj();
            this.initalizCommand();
        }

        #endregion

        #region properties

        public String SSRSAddress
        {
            get { return GetValue(() => SSRSAddress); }
            set
            {
                SetValue(() => SSRSAddress, value);
            }
        }

        public String Username
        {
            get { return GetValue(() => Username); }
            set
            {
                SetValue(() => Username, value);
            }
        }

        public String Password
        {
            get { return GetValue(() => Password); }
            set
            {
                SetValue(() => Password, value);
            }
        }

        public String DomainName
        {
            get { return GetValue(() => DomainName); }
            set
            {
                SetValue(() => DomainName, value);
            }
        }

        public Boolean IsDomain
        {
            get { return GetValue(() => IsDomain); }
            set
            {
                SetValue(() => IsDomain, value);
                if (value)
                {
                    DomainSpecificetion();
                }
                else
                {
                    this.Username = "";
                    this.Password = "";
                    this.DomainName = "";
                }
            }
        }

        #endregion

        #region methods

        private void initalizObj()
        {
            SSRSAddress = Settings.Default.ReportServerUrl;
            IsDomain = Settings.Default.NETTypeIsDomain;
        }

        private void DomainSpecificetion()
        {
            this.Username = Settings.Default.SSRSUsername;
            this.Password = Settings.Default.SSRSPassword;
            this.DomainName = Settings.Default.SSRSDomainName;
        }

        private void ConfirmSave()
        {
            Regex regex = new Regex(@"^http(s)?://([\w-]+.)+[\w-]+(/[\w- ./?%&=])?$");
            if (!regex.IsMatch(this.SSRSAddress))
            {
                _dialogService.ShowError("خطا", "آدرس سرور نامعتبر می باشد");
                return;
            }

            if (IsDomain)
            {
                if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password)
                    || string.IsNullOrWhiteSpace(DomainName))
                {
                    _dialogService.ShowError("خطا", "نام کاربری ، رمز عبور ، و نام دامین الزامی است");
                    return;
                }
                Settings.Default.SSRSDomainName = DomainName;
                Settings.Default.SSRSUsername = Username;
                Settings.Default.SSRSPassword = Password;
                Settings.Default.NETTypeIsDomain = true;
            }
            else
            {
                Settings.Default.NETTypeIsDomain = false;
            }

            Settings.Default.ReportServerUrl = SSRSAddress;
            Settings.Default.Save();
            Settings.Default.Reload();
            _dialogService.ShowInfo("توجه", "تغییرات با موفقیت انجام شد");
        }

        #endregion

        #region commands

        public ICommand SaveCommand { get; private set; }

        private void initalizCommand()
        {
            SaveCommand = new MvvmCommand(
                (parameter) => { this.ConfirmSave(); },
                (parameter) => { return true; }
                );
        }

        #endregion

        #region fields

        private readonly IUnityContainer _container;
        private readonly IDialogService _dialogService;

        #endregion
    }
}

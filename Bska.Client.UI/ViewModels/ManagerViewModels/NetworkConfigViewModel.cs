
namespace Bska.Client.UI.ViewModels
{
    using Bska.Client.Common;
    using Bska.Client.UI.API;
    using Bska.Client.UI.Services;
    using Microsoft.Practices.Unity;
    using System;
    using System.Text.RegularExpressions;
    using System.Windows.Input;
    public sealed class NetworkConfigViewModel : BaseViewModel
    {
        #region ctor

        public NetworkConfigViewModel(IUnityContainer container)
        {
            this._container = container;
            this._dialogService = _container.Resolve<IDialogService>();
            this.initalizObj();
            this.initalizCommands();
        }

        #endregion

        #region properties

        public String APIAddress
        {
            get { return GetValue(() => APIAddress); }
            set
            {
                SetValue(() => APIAddress, value);
            }
        }

        public String WCFAddress
        {
            get { return GetValue(() => WCFAddress); }
            set
            {
                SetValue(() => WCFAddress, value);
            }
        }

        #endregion

        #region methods

        private void initalizObj()
        {
            APIAddress = Settings.Default.APIServiceURL;
            WCFAddress = Settings.Default.WCFServiceUrl;
        }

        private void SaveUrl(bool isWcf)
        {
            Regex regex = new Regex(@"/((([A-Za-z]{3,9}:(?:\/\/)?)(?:[-;:&=\+\$,\w]+@)?[A-Za-z0-9.-]+|(?:www.|[-;:&=\+\$,\w]+@)[A-Za-z0-9.-]+)((?:\/[\+~%\/.\w-_]*)?\??(?:[-\+=&;%@.\w_]*)#?(?:[\w]*))?)/");
            if (!isWcf)
            {
                if (!regex.IsMatch(APIAddress))
                {
                    _dialogService.ShowError("خطا", "ورودی نامعتبر است");
                    return;
                }
                Settings.Default.APIServiceURL = APIAddress;
            }
            else
            {
                if (!regex.IsMatch(WCFAddress))
                {
                    _dialogService.ShowError("خطا", "ورودی نامعتبر است");
                    return;
                }
                Settings.Default.WCFServiceUrl = WCFAddress;
            }
            Settings.Default.Save();
            Settings.Default.Reload();
            _dialogService.ShowInfo("توجه", "تغییرات با موفقیت انجام شد");
        }

        #endregion

        #region commands

        public ICommand SaveCommand { get; private set; }
        private void initalizCommands()
        {
            SaveCommand = new MvvmCommand(
                (parameter) => { this.SaveUrl((bool)parameter); },
                (parameter) => { return true; }
                );
        }

        #endregion

        #region fields

        private readonly IDialogService _dialogService;
        private readonly IUnityContainer _container;

        #endregion
    }
}


namespace Bska.Client.UI.ViewModels
{
    using Bska.Client.Common;
    using Bska.Client.UI.API;
    using Bska.Client.UI.Services;
    using Microsoft.Practices.Unity;
    using System;
    using System.Reflection;
    using System.Windows.Input;
    public sealed class InitAppViewModel : BaseViewModel
    {
        #region ctro

        public InitAppViewModel(IUnityContainer container)
        {
            this._container = container;
            _dialogService = _container.Resolve<IDialogService>();
            this.initializObj();
            this.initializCommands();
        }

        #endregion

        #region properties

        public decimal MStuffPrice
        {
            get { return GetValue(() => MStuffPrice); }
            set
            {
                SetValue(() => MStuffPrice, value);
            }
        }

        public decimal CostForOrderMonaghese
        {
            get { return GetValue(() => CostForOrderMonaghese); }
            set
            {
                SetValue(() => CostForOrderMonaghese, value);
            }
        }

        public Boolean ModirConfirmForDisplacementPersonOrder
        {
            get { return GetValue(() => ModirConfirmForDisplacementPersonOrder); }
            set
            {
                SetValue(() => ModirConfirmForDisplacementPersonOrder, value);
            }
        }

        public int SearchDateMonth
        {
            get { return GetValue(() => SearchDateMonth); }
            set
            {
                SetValue(() => SearchDateMonth, value);
            }
        }

        public Boolean FlorOld
        {
            get { return GetValue(() => FlorOld); }
            set
            {
                SetValue(() => FlorOld, value);
            }
        }

        public Boolean Flor707
        {
            get { return GetValue(() => Flor707); }
            set
            {
                SetValue(() => Flor707, value);
            }
        }

        public bool FlorBoth
        {
            get { return GetValue(() => FlorBoth); }
            set
            {
                SetValue(() => FlorBoth, value);
            }
        }

        public bool BookTypeBoth
        {
            get { return GetValue(() => BookTypeBoth); }
            set
            {
                SetValue(() => BookTypeBoth, value);
            }
        }

        public Boolean BookTypeOldSystem
        {
            get { return GetValue(() => BookTypeOldSystem); }
            set
            {
                SetValue(() => BookTypeOldSystem, value);
            }
        }

        public Boolean BookTypeNewSystem
        {
            get { return GetValue(() => BookTypeNewSystem); }
            set
            {
                SetValue(() => BookTypeNewSystem, value);
            }
        }

        public Boolean AccidentConfirmed
        {
            get { return GetValue(() => AccidentConfirmed); }
            set
            {
                SetValue(() => AccidentConfirmed, value);
            }
        }

        public Boolean AccidentConfirmOnDelete
        {
            get { return GetValue(() => AccidentConfirmOnDelete); }
            set
            {
                SetValue(() => AccidentConfirmOnDelete, value);
            }
        }

        public Boolean AccidentConfirmBoth
        {
            get { return GetValue(() => AccidentConfirmBoth); }
            set
            {
                SetValue(() => AccidentConfirmBoth, value);
            }
        }

        public string ConfigPass
        {
            get { return GetValue(() => ConfigPass); }
            set
            {
                SetValue(() => ConfigPass, value);
            }
        }

        public string ConfigUserName
        {
            get { return GetValue(() => ConfigUserName); }
            set
            {
                SetValue(() => ConfigUserName, value);
            }
        }

        public Boolean StoreDescEnabled
        {
            get { return GetValue(() => StoreDescEnabled); }
            set
            {
                SetValue(() => StoreDescEnabled, value);
            }
        }

        public Boolean StuffDepricationEnabled
        {
            get { return GetValue(() => StuffDepricationEnabled); }
            set
            {
                SetValue(() => StuffDepricationEnabled, value);
            }
        }

        public string LogFolderPath
        {
            get { return GetValue(() => LogFolderPath); }
            set
            {
                SetValue(() => LogFolderPath, value);
            }
        }
        #endregion

        #region methods

        private void initializObj()
        {
            CostForOrderMonaghese = APPSettings.Default.CostForOrderMonaghese;
            MStuffPrice = APPSettings.Default.MStuffPrice;
            this.SearchDateMonth = APPSettings.Default.SearchDateMonth;
            this.ModirConfirmForDisplacementPersonOrder = APPSettings.Default.ModirConfirmForDisplacementPersonOrder;
            int bookTpe = APPSettings.Default.BookType;
            int floorType = APPSettings.Default.OldSystemFloorType;
            int accidentConfirm = APPSettings.Default.AccidentProccedingConfirm;
            this.ConfigUserName = APPSettings.Default.ConfigUserName;
            this.ConfigPass = GlobalClass.DecryptStringAES(APPSettings.Default.ConfigPass, "66Ak679Du4V3qo92");
            this.StoreDescEnabled = APPSettings.Default.EnabledNBookStore;
            this.StuffDepricationEnabled = APPSettings.Default.EnabledStuffDeprication;
            this.LogFolderPath = APPSettings.Default.LogPath;
            //config book type
            switch (bookTpe)
            {
                case 1002:
                    BookTypeBoth = false;
                    BookTypeNewSystem = false;
                    BookTypeOldSystem = true;
                    break;
                case 1003:
                    BookTypeBoth = false;
                    BookTypeNewSystem =true;
                    BookTypeOldSystem = false;
                    break;
                default:
                    BookTypeBoth = true;
                    BookTypeNewSystem = false;
                    BookTypeOldSystem = false;
                    break;
            }

            if (floorType == 701)
            {
                Flor707 = false;
                FlorBoth = true;
                FlorOld = false;
            }
            else if (floorType == 704)
            {
                Flor707 = false;
                FlorBoth = false;
                FlorOld = true;
            }
            else
            {
                Flor707 = true;
                FlorBoth = false;
                FlorOld = false;
            }

            switch (accidentConfirm)
            {
                case 2002:
                    AccidentConfirmed = true;
                    AccidentConfirmBoth = false;
                    AccidentConfirmOnDelete = false;
                    break;
                case 2003:
                  AccidentConfirmed = false;
                    AccidentConfirmBoth = false;
                    AccidentConfirmOnDelete = true;
                    break;
                default:
                   AccidentConfirmed =false;
                    AccidentConfirmBoth = true;
                    AccidentConfirmOnDelete =false;
                    break;
            }
        }

        private void confirmChange()
        {
            APPSettings.Default.MStuffPrice = MStuffPrice;
            APPSettings.Default.CostForOrderMonaghese = this.CostForOrderMonaghese;
            int florType = 707;
            if (FlorBoth) florType = 701;
            else if (FlorOld) florType = 704;
            else florType = 707;

            int bookType = 1001;
            if (BookTypeNewSystem) bookType = 1003;
            else if (BookTypeOldSystem) bookType = 1002;
            
            int accidentConfirm = 2001;
            if (AccidentConfirmed) accidentConfirm = 2002;
            else if (AccidentConfirmOnDelete) accidentConfirm = 2003;
            APPSettings.Default.ConfigUserName = this.ConfigUserName;
            APPSettings.Default.ConfigPass = GlobalClass.EncryptStringAES(this.ConfigPass, "66Ak679Du4V3qo92");
            APPSettings.Default.BookType = bookType;
            APPSettings.Default.OldSystemFloorType = florType;
            APPSettings.Default.AccidentProccedingConfirm = accidentConfirm;
            APPSettings.Default.LogPath = LogFolderPath;
            APPSettings.Default.EnabledNBookStore = this.StoreDescEnabled;
            APPSettings.Default.EnabledStuffDeprication = this.StuffDepricationEnabled;
            APPSettings.Default.ModirConfirmForDisplacementPersonOrder = this.ModirConfirmForDisplacementPersonOrder;
            APPSettings.Default.SearchDateMonth = this.SearchDateMonth;
            APPSettings.Default.Save();
            APPSettings.Default.Reload();
            _dialogService.ShowInfo("توجه", "تغییرات با موفقیت انجام شد");
        }

        private void setLogPath()
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                if (!string.IsNullOrEmpty(dialog.SelectedPath))
                {
                    LogFolderPath = dialog.SelectedPath;
                }
            }
        }

        #endregion

        #region commands

        public ICommand SaveCommand { get; private set; }
        public ICommand LogFilePathCommand { get; private set; }
        private void initializCommands()
        {
            SaveCommand = new MvvmCommand(
                (parameter) => { this.confirmChange(); },
                (parameter) => { return true; }
                );

            LogFilePathCommand = new MvvmCommand(
                (parameter) => { this.setLogPath(); },
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

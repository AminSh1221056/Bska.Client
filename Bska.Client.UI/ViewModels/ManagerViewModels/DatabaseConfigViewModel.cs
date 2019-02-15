
namespace Bska.Client.UI.ViewModels
{
    using Bska.Client.Common;
    using Bska.Client.UI.API;
    using Bska.Client.UI.Services;
    using Microsoft.Practices.Unity;
    using System;
    using System.Configuration;
    using System.Data.SqlClient;
    using System.Windows.Input;
    using System.Linq;
    using System.Xml.Linq;
    using System.Collections.Generic;
    using Repository.Model;
    using Helper;
    using System.Runtime.ExceptionServices;
    using Newtonsoft.Json;

    public sealed class DatabaseConfigViewModel : BaseViewModel
    {
        #region ctor

        public DatabaseConfigViewModel(IUnityContainer container)
        {
            this._container = container;
            this._dialogService = _container.Resolve<IDialogService>();
            this._helper = new SeedDataHelper();
            this.initalizObj();
            this.initalizCommands();
        }

        #endregion

        #region properties

        public string Provider
        {
            get { return GetValue(() => Provider); }
            set
            {
                SetValue(() => Provider, value);
            }
        }

        public Boolean Security
        {
            get { return GetValue(() => Security); }
            set
            {
                SetValue(() => Security, value);
            }
        }

        public string ServerName
        {
            get { return GetValue(() => ServerName); }
            set
            {
                SetValue(() => ServerName, value);
            }
        }
        
        public String UserName
        {
            get { return GetValue(() => UserName); }
            set
            {
                SetValue(() => UserName, value);
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

        public int TimeOut
        {
            get { return GetValue(() => TimeOut); }
            set
            {
                SetValue(() => TimeOut, value);
            }
        }

        public List<DBServersModel> DbServers
        {
            get { return GetValue(() => DbServers); }
            set
            {
                SetValue(() => DbServers, value);
            }
        }

        public DBServersModel SelectedDB
        {
            get { return GetValue(() => SelectedDB); }
            set
            {
                SetValue(() => SelectedDB, value);
            }
        }

        #endregion

        #region methods

        private void initalizObj()
        {
            Mouse.SetCursor(Cursors.Wait);
            var dtnDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(APPSettings.Default.DbServers);
            Provider = Settings.Default.providerName;
            ServerName = Settings.Default.dataSource;
            UserName = Settings.Default.username;
            if (!string.IsNullOrWhiteSpace(Settings.Default.password))
                Password = GlobalClass.DecryptStringAES(Settings.Default.password, "66Ak679Du4V3qo92");
            TimeOut = Settings.Default.timeOut;
            Security = Settings.Default.security;
            if (dtnDictionary!=null)
            {
                int counter = 1;
                var lst = new List<DBServersModel>();
                dtnDictionary.ForEach(dic =>
                {
                    var item = new DBServersModel
                    {
                        DatabaseName = dic.Key,
                        Organization = dic.Value,
                        IsCurrent = false,
                    };
                    counter++;
                    if (string.Equals(dic.Key, Settings.Default.initialCatalog))
                    {
                        item.IsCurrent = true;
                    }
                    lst.Add(item);
                });
                DbServers = lst;
                SelectedDB = DbServers.Where(db => db.DatabaseName == Settings.Default.initialCatalog).FirstOrDefault();
            }
            
            Mouse.SetCursor(Cursors.Arrow);
        }

        private void TestServerConnection()
        {
            if (SelectedDB == null)
            {
                return;
            }

            SqlConnectionStringBuilder Builder = new SqlConnectionStringBuilder();

            Builder.DataSource = ServerName;
            Builder.InitialCatalog = SelectedDB.DatabaseName;
            Builder.UserID = UserName;
            Builder.Password = Password;
            Builder.ConnectTimeout = Convert.ToInt32(TimeOut);

            SqlConnection con = new SqlConnection(Builder.ConnectionString);

            try
            {
                con.Open();
                con.Close();

                _dialogService.ShowInfo("توجه", "تست اتصال موفقیت آمیز بود");
            }
            catch (SqlException ex)
            {
                _dialogService.ShowError("Error", ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                _dialogService.ShowError("Error", ex.Message);
            }
            catch (ConfigurationException ex)
            {
                _dialogService.ShowError("Error", ex.Message);
            }
            catch (Exception ex)
            {
                ExceptionDispatchInfo.Capture(ex).Throw();
            }

        }

        private void SaveServerDataConnection()
        {
            if (string.IsNullOrWhiteSpace(ServerName) || (SelectedDB==null)
                || string.IsNullOrWhiteSpace(UserName) || string.IsNullOrWhiteSpace(Password))
            {
                _dialogService.ShowError("خطا", "لطفا ورودی های خود را کنترل کنید");
                return;
            }
           
            Settings.Default.providerName= Provider;
            Settings.Default.dataSource=ServerName;
            Settings.Default.initialCatalog=SelectedDB.DatabaseName;
            Settings.Default.username=UserName;
            Settings.Default.password=GlobalClass.EncryptStringAES(Password, "66Ak679Du4V3qo92");
            Settings.Default.timeOut=TimeOut;
            Settings.Default.security= Security;

            Settings.Default.Save();
            Settings.Default.Reload();

            _dialogService.ShowInfo("توجه", "تغییرات با موفقیت ذخیره شد.برنامه بسته خواهد شد،آنرا دوباره اجرا کنید");
            App.Current.Shutdown();
        }

        #endregion

        #region commands

        public ICommand TestCommand { get; private set; }
        public ICommand SaveCommand { get; private set; }

        private void initalizCommands()
        {
            TestCommand = new MvvmCommand(
                (parameter) => { this.TestServerConnection(); },
                (parameter) => { return true; }
                );

            SaveCommand = new MvvmCommand(
                (parameter) => { this.SaveServerDataConnection(); },
                (parameter) => { return true; }
                );
        }

        #endregion

        #region fields

        private readonly IDialogService _dialogService;
        private readonly IUnityContainer _container;
        private readonly SeedDataHelper _helper;
        private XDocument _xdocuments;

        #endregion
    }
}

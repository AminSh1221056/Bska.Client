
namespace Bska.Client.UI.ViewModels
{
    using Microsoft.Practices.Unity;
    using Services;
    using Repository.Model;
    using System.Windows.Input;
    using API;
    using Helper;
    using System.Collections.ObjectModel;
    using System;
    using System.Windows.Controls;
    using Common;
    using System.Runtime.ExceptionServices;
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public sealed class DBServersViewModel : BaseViewModel
    {
        #region ctor

        public DBServersViewModel(IUnityContainer container)
        {
            this._container = container;
            this._dialogService = _container.Resolve<IDialogService>();
            this._helper = new SeedDataHelper();
            this._collection = new ObservableCollection<DBServersModel>();
            this.initDocuments();
            this.initializCommands();
        }

        #endregion

        #region proeperties

        public ObservableCollection<DBServersModel> DBServers
        {
           get { return _collection; }
        }

        public DBServersModel Selected
        {
            get { return GetValue(() => Selected); }
            set
            {
                SetValue(() => Selected, value);
            }
        }

        public string Organization
        {
            get { return GetValue(() => Organization); }
            set
            {
                SetValue(() => Organization, value);
            }
        }

        public string DatabaseName
        {
            get { return GetValue(() => DatabaseName); }
            set
            {
                SetValue(() => DatabaseName, value);
            }
        }

        #endregion

        #region methods
        
        private void saveDb()
        {
            if (string.IsNullOrEmpty(this.Organization) || string.IsNullOrEmpty(this.DatabaseName))
            {
                _dialogService.ShowError("Error", "Please insert valid value...");
                return;
            }
            
            var dbServerModel =new DBServersModel {Organization=this.Organization,DatabaseName=this.DatabaseName,IsCurrent=false};
          
            try
            {
                var dtnDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(APPSettings.Default.DbServers);
                if (dtnDictionary == null)
                {
                    dtnDictionary = new Dictionary<string, string>();
                }

                if (dtnDictionary.ContainsKey(this.DatabaseName))
                {
                    _dialogService.ShowError("Error", "Database Exists,Please insert new Database Name...");
                    return;
                }
                dtnDictionary.Add(DatabaseName, Organization);
                APPSettings.Default.DbServers = JsonConvert.SerializeObject(dtnDictionary);
                APPSettings.Default.Save();
                APPSettings.Default.Reload();
            }
            catch (Exception ex)
            {
                ExceptionDispatchInfo.Capture(ex).Throw();
            }
            _collection.Add(dbServerModel);
            this.Selected = dbServerModel;
        }

        private void initDocuments()
        {
            Mouse.SetCursor(Cursors.Wait);
            _collection.Clear();
            if (!string.IsNullOrWhiteSpace(APPSettings.Default.employeeCer))
            {
                var dtnDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(APPSettings.Default.DbServers);
                int counter = 1;
                dtnDictionary.ForEach(dic =>
                {
                    var item = new DBServersModel
                    {
                        DatabaseName=dic.Key,
                        Organization=dic.Value,
                        IsCurrent=false,
                    };
                    counter++;
                    if (string.Equals(dic.Key, Settings.Default.initialCatalog))
                    {
                        item.IsCurrent = true;
                    }
                    _collection.Add(item);
                });
            }
            Mouse.SetCursor(Cursors.Arrow);
        }

        private void deleteDbServer(object parameter)
        {
            var dbModel = parameter as DBServersModel;
            if (dbModel == null) return;
            this.Selected = dbModel;
            bool confirm = _dialogService.AskConfirmation("Question", "Are you sure you want to perform this operation??");
            if (confirm)
            {
                var dtnDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(APPSettings.Default.DbServers);
                try
                {
                    var item = dtnDictionary[dbModel.DatabaseName];
                    if (item != null)
                    {
                        dtnDictionary.Remove(dbModel.DatabaseName);
                    }
                    APPSettings.Default.DbServers = JsonConvert.SerializeObject(dtnDictionary);
                    APPSettings.Default.Save();
                    APPSettings.Default.Reload();
                }
                catch (Exception ex)
                {
                    ExceptionDispatchInfo.Capture(ex).Throw();
                }
                _collection.Remove(dbModel);
            }
        }

        private void setCurrentDb(object parameter)
        {
            var rb = parameter as RadioButton;
            var dbServer = rb.Tag as DBServersModel;
            if (dbServer != null)
            {
                bool confirm = _dialogService.AskConfirmation("Question", "Are you sure you want to perform this operation??");
                if (confirm)
                {
                    Mouse.SetCursor(Cursors.Wait);
                    this.Selected = dbServer;
                    dbServer.IsCurrent = true;
                    Settings.Default.initialCatalog = dbServer.DatabaseName;
                    Settings.Default.Save();
                    Settings.Default.Reload();

                    _dialogService.ShowInfo("توجه", "تغییرات با موفقیت ذخیره شد.برنامه بسته خواهد شد،آنرا دوباره اجرا کنید");
                    App.Current.Shutdown();
                    Mouse.SetCursor(Cursors.Arrow);
                }
                else
                {
                    this.initDocuments();
                }
            }
        }

        #endregion

        #region commands

        public ICommand SaveCommand { get; private set; }
        public ICommand DeleteCommand { get; private set; }
        public ICommand SetCurrentDbCommand { get; private set; }
        private void initializCommands()
        {
            SaveCommand = new MvvmCommand(
                (parameter) => { this.saveDb(); },
                (parameter) => { return true; }
                );

            SetCurrentDbCommand = new MvvmCommand(
                (parameter) => { this.setCurrentDb(parameter); },
                (parameter) => { return true; }
                );

            DeleteCommand = new MvvmCommand(
                (parameter) =>
                {
                    this.deleteDbServer(parameter);
                },
                (parameter) => { return true; }
                );
        }

        #endregion

        #region fields

        private readonly IUnityContainer _container;
        private readonly IDialogService _dialogService;
        private readonly SeedDataHelper _helper;
        private readonly ObservableCollection<DBServersModel> _collection;

        #endregion
    }
}

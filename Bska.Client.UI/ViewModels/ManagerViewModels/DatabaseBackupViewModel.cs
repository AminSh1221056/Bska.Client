
namespace Bska.Client.UI.ViewModels
{
    using Bska.Client.Common;
    using API;
    using Microsoft.Practices.Unity;
    using Services;
    using System;
    using System.Windows.Input;
    using Microsoft.SqlServer.Management.Smo;
    using Microsoft.SqlServer.Management.Common;

    public sealed class DatabaseBackupViewModel : BaseViewModel
    {
        #region ctor

        public DatabaseBackupViewModel(IUnityContainer container)
        {
            this._container = container;
            this._dialogService = _container.Resolve<IDialogService>();
            this.initialzObj();
            this.initializCommands();
        }
        #endregion

        #region properties

        public string BackupPath
        {
            get { return GetValue(() => BackupPath); }
            set
            {
                SetValue(() => BackupPath, value);
            }
        }

        public string RestorePath
        {
            get { return GetValue(() => RestorePath); }
            set
            {
                SetValue(() => RestorePath, value);
            }
        }

        public int BackupReport
        {
            get { return GetValue(() => BackupReport); }
            set
            {
                SetValue(() => BackupReport, value);
            }
        }

        public int RestoreReport
        {
            get { return GetValue(() => RestoreReport); }
            set
            {
                SetValue(() => RestoreReport, value);
            }
        }

        public string BackupFileName
        {
            get { return GetValue(() => BackupFileName); }
            set
            {
                SetValue(() => BackupFileName, value);
            }
        }

        #endregion

        #region methods

        private void initialzObj()
        {
            this.BackupReport = 0;
            PersianDate today = PersianDate.Today;
            this.BackupPath = APPSettings.Default.BackupFolder;
            BackupFileName = "bskaBackup-"+today.Year + "-" + today.Month + "-" + today.Day+".bak";
        }

        private void selectFolder()
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                if (!string.IsNullOrEmpty(dialog.SelectedPath))
                {
                    this.BackupPath = dialog.SelectedPath;
                    APPSettings.Default.BackupFolder = dialog.SelectedPath;
                    APPSettings.Default.Save();
                    APPSettings.Default.Reload();
                }
            }
        }

        private void selectRestoreFile()
        {
            using (var dialog = new System.Windows.Forms.OpenFileDialog())
            {
                System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                if (!string.IsNullOrEmpty(dialog.FileName))
                {
                    RestorePath = dialog.FileName;
                }
            }
        }

        private void perfromBackup()
        {
            if (string.IsNullOrEmpty(BackupPath) || string.IsNullOrWhiteSpace(BackupFileName))
            {
                _dialogService.ShowAlert("توجه", "مسیر ذخیره سازی مشخص نیست");
                return;
            }
            
            bool confirm = _dialogService.AskConfirmation("پرسش", ErrorMessages.Default.AskConfrimation);
            if (confirm)
            {
                this.BackupReport = 0;
                Server dbServer = new Server(new ServerConnection(Common.Settings.Default.dataSource,
                    Common.Settings.Default.username, GlobalClass.DecryptStringAES(Common.Settings.Default.password, "66Ak679Du4V3qo92")));
                Backup dbBackup = new Backup();
                /* The type of device you going to backup:  Database, Files or Log */
                dbBackup.Action = BackupActionType.Database;
                /* The SQL Database name you need to backup */
                dbBackup.Database = Common.Settings.Default.initialCatalog;
                /* Keep in mind that the file path you use here is relative to the SQL Server, not your local file system */
                dbBackup.Devices.AddDevice(BackupPath +"\\"+ BackupFileName, DeviceType.File);
                dbBackup.BackupSetName = "Automated Daily Backup Example";
                dbBackup.BackupSetDescription = "Client system database - Automated Daily Backup";
                /* This is an optional item that you can use to set when the backup data expires and
                 * should no longer be considered "restorable"
                */
                //dbBackup.ExpirationDate = DateTime.Now.AddDays(7);

                /* Leaving the Initialize value set to the default of false will create
                 * a new backup item as the last backup set.
                 * If you change the Initialize value to true then this backup set will
                 * become the first backup set and will overwrite other backups that have
                 * the same BackupSetName
                */
                dbBackup.Initialize = true;

                /* Leaving the Incremental value set to the default of false will create
                 * a full backup.
                 * If you change the Incremental value to true it will only perform a 
                 * delta backup since the last full backup that was performed
                */
                //dbBackup.Incremental = false;

                /* There are several events that you can wire up to the Backup object
                 * These will allow you to keep track of:
                 *      The current progress of the backup
                 *      When the current backup media is exhausted (full disk)
                 *      When the current backup is completed
                 *      When a message not captured by the above options is sent
                */
                dbBackup.PercentComplete += new PercentCompleteEventHandler(Event_PercentComplete);
                dbBackup.Complete += new ServerMessageEventHandler(Event_Complete);

                /* You can also use SqlBackupAsync with the same parameter if you want to run
                 * asynchronously
                */
                try
                {
                    dbBackup.SqlBackup(dbServer);
                }
                catch (Exception err)
                {
                    _dialogService.ShowError("", err.Message);
                }
            }
        }

        private void performRestore()
        {
            if (string.IsNullOrEmpty(RestorePath))
            {
                _dialogService.ShowAlert("توجه", "فایل مورد نظر معتبر نمی باشد");
                return;
            }

            bool confirm = _dialogService.AskConfirmation("پرسش", ErrorMessages.Default.AskConfrimation);
            if (confirm)
            {
                this.RestoreReport = 0;
                Server dbServer = new Server(new ServerConnection(Common.Settings.Default.dataSource,
                  Common.Settings.Default.username, GlobalClass.DecryptStringAES(Common.Settings.Default.password, "66Ak679Du4V3qo92")));
                dbServer.KillAllProcesses(Common.Settings.Default.initialCatalog);
                Restore dbRestore = new Restore();
                dbRestore.Database = Common.Settings.Default.initialCatalog;
                dbRestore.Action = RestoreActionType.Database;
                /* Keep in mind that the file path you use here is relative to the SQL Server, not your local file system */
                dbRestore.Devices.AddDevice(RestorePath, DeviceType.File);

                /* Leaving the ReplaceDatabase value set to the default of false will not create
                 * a new database image so the database that is set must exist on the SQL server
                 * If you change the Incremental value to true it will create a new image of the
                 * database regardless of whether it currently exists or not
                */
                dbRestore.ReplaceDatabase = true;

                /* Leaving the NoRecovery value set to the default of false the tail end of the log
                 * is backed up and the database will not be in a Restoring state
                 * If you change the NoRecovery value to true then the database will be left in a
                 * Restoring state.
                 * This option is only used when the log is backed up as well
                */
                //dbRestore.NoRecovery = false;


                /* There are several events that you can wire up to the Restore object
                 * These will allow you to keep track of:
                 *      The current progress of the restore
                 *      When the current backup media is exhausted (next disk)
                 *      When the current backup is completed
                 *      When a message not captured by the above options is sent
                */
                dbRestore.PercentComplete += new PercentCompleteEventHandler(Event_PercentComplete1);
                dbRestore.Complete += new ServerMessageEventHandler(Event_Complete);

                /* You can also use SqlRestoreAsync with the same parameter if you want to run
                 * asynchronously
                 * 
                 ***************************** IMPORTANT NOTE ********************************
                 * In order to perform a restore you must be able to acquire an exclusive lock
                 * on the database or it will fail
                 * 
                 * You can easily check to see what processes are currently running on the
                 * database with the following command (alternatively you can use SP_WHO)
                 * 
                 * SELECT spid 
                 * FROM master..SysProcesses 
                 * WHERE DBID IN (SELECT dbid FROM master..SysDatabases WHERE name = 'EPSS_PRD_EN')
                 * 
                 * Once you have the list of the running processes against the DB you can use the
                 * KILL procedure to terminate the session in order to free up the DB for restore
                */
                try
                {
                    dbRestore.SqlRestore(dbServer);
                }
                catch (Exception err)
                {
                    _dialogService.ShowError("", err.Message);
                }
            }
        }

        void Event_Complete(object sender, ServerMessageEventArgs e)
        {
            _dialogService.ShowInfo("توجه", e.Error.Message);
        }

        void Event_PercentComplete(object sender, PercentCompleteEventArgs e)
        {
            BackupReport = e.Percent;
        }

        void Event_PercentComplete1(object sender, PercentCompleteEventArgs e)
        {
            RestoreReport = e.Percent;
        }

        #endregion

        #region commands

        public ICommand SelectFolderCommand { get; private set; }
        public ICommand SelectFileCommand { get; private set; }
        public ICommand BackupCommand { get; private set; }
        public ICommand RestoreCommand { get; private set; }
        private void initializCommands()
        {
            SelectFolderCommand = new MvvmCommand(
                (parameter) => { this.selectFolder(); },
                (parameter) => { return true; }
                );

            BackupCommand = new MvvmCommand(
                (parameter) => { this.perfromBackup(); },
                (parameter) => { return true; }
                );

            SelectFileCommand = new MvvmCommand(
                (parameter) => { this.selectRestoreFile(); },
                (parameter) => { return true; }
                );

            RestoreCommand = new MvvmCommand(
                (parameter) => { this.performRestore(); },
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

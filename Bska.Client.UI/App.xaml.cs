using Bska.Client.API.DataContext;
using Bska.Client.API.EF6;
using Bska.Client.API.Repositories;
using Bska.Client.API.UnitOfWork;
using Bska.Client.Data.Service;
using Bska.Client.Domain.Concrete;
using Bska.Client.Domain.Entity;
using Bska.Client.Domain.Entity.AssetEntity;
using Bska.Client.Domain.Entity.OrderEntity;
using Bska.Client.Domain.Entity.StoredProcedures;
using Bska.Client.UI.API;
using Bska.Client.UI.API.Client;
using Bska.Client.UI.Services;
using Bska.Client.UI.ViewModels;
using Bska.Client.UI.Views;
using Bska.Client.UI.Views.AssetDetailsView;
using Bska.Client.UI.Views.MunitionView;
using Bska.Client.UI.Views.OrderView;
using Bska.Client.UI.Views.StoreView;
using Bska.Client.UI.Views.StuffHonestView;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Threading;

namespace Bska.Client.UI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application, ISingleInstanceApp
    {
        public IUnityContainer container;

        private INavigationService navigation;

        public App()
            : base()
        {
            System.Threading.Thread.CurrentThread.CurrentUICulture = new CultureInfo("fa");
            //System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("fa");

           // this.Dispatcher.UnhandledException += Current_DispatcherUnhandledException;
            ConfigureContainer();
        }

        public bool SignalExternalCommandLineArgs(IList<string> args)
        {
            StartWindow objstartWin = new StartWindow(container);
            // handle command line arguments from external execution
            return objstartWin.ProcessCommandLineArgs(args, false);
        }

        [STAThread]
        public static void Main()
        {
            if (SingleInstance<App>.InitializeAsFirstInstance("Bska"))
            {
                var application = new App();

                application.Init();
                application.Run();

                // Allow single instance code to perform cleanup operations
                SingleInstance<App>.Cleanup();
            }
        }

        private void ConfigureContainer()
        {
            container = new UnityContainer();
            container.RegisterType<IDialogService, DialogService>();
            container.RegisterType<INavigationService, NavigationService>();
            container.RegisterType<IDataContextAsync, BskaContext>(new PerResolveLifetimeManager());
            container.RegisterType<IUnitOfWorkAsync, UnitOfWork>(new PerResolveLifetimeManager());
            container.RegisterType<IBskaStoredProcedures, BskaContext>();
            container.RegisterType<IRepositoryAsync<Employee>, Repository<Employee>>();
            container.RegisterType<IEmployeeService, EmployeeService>();
            container.RegisterType<IRepositoryAsync<Person>, Repository<Person>>();
            container.RegisterType<IPersonService, PersonService>();
            container.RegisterType<IRepositoryAsync<Building>, Repository<Building>>();
            container.RegisterType<IBuildingService, BuildingService>();
            container.RegisterType<IRepositoryAsync<Store>, Repository<Store>>();
            container.RegisterType<IStoreService, StoreService>();
            container.RegisterType<IRepositoryAsync<Stuff>, Repository<Stuff>>();
            container.RegisterType<IStuffService, StuffService>();
            container.RegisterType<IRepositoryAsync<MovableAsset>, Repository<MovableAsset>>();
            container.RegisterType<IMovableAssetService, MovableAssetService>();
            container.RegisterType<IRepositoryAsync<Unit>, Repository<Unit>>();
            container.RegisterType<IUnitService, UnitService>();
            container.RegisterType<IRepositoryAsync<StoreBill>, Repository<StoreBill>>();
            container.RegisterType<IStoreBillService, StoreBillService>();
            container.RegisterType<IRepositoryAsync<Order>, Repository<Order>>();
            container.RegisterType<IOrderService, OrderService>();
            container.RegisterType<IRepositoryAsync<Proceeding>, Repository<Proceeding>>();
            container.RegisterType<IProceedingService, ProceedingService>();
            container.RegisterType<IRepositoryAsync<Seller>,Repository<Seller>>();
            container.RegisterType<ISellerService, SellerService>();
            container.RegisterType<IRepositoryAsync<Commodity>, Repository<Commodity>>();
            container.RegisterType<IMAssetCommodityService, MAssetCommodityService>();

            //api service
            container.RegisterType<IBaseInfoService, BaseInfoService>();

            container.RegisterType<MainWindow>(new ContainerControlledLifetimeManager());
            container.RegisterType<StartWindow>();
            container.RegisterType<LoginWindow>();
            container.RegisterType<ConfigWindow>();
            container.RegisterType<InitializWindow>();
            container.RegisterType<PersonManageWindow>();
            container.RegisterType<UserWindow>();
            container.RegisterType<StuffHonestMainWindow>();
            container.RegisterType<AddInfoWindow>();
            container.RegisterType<MovableAssetSplitWindow>();
            container.RegisterType<MovableAssetDetailsWindow>();
            container.RegisterType<OrderMainWindow>();
            container.RegisterType<OrderDetailsWindow>();
            container.RegisterType<OrderTrackWindow>();
            container.RegisterType<OrderEditWindow>();
            container.RegisterType<IndentWindow>();
            container.RegisterType<SuggestViewWindow>();
            container.RegisterType<DisplacementIndentWindow>();
            container.RegisterType<StoreIndentConfirmWindow>();
            container.RegisterType<MunitionMainWindow>();
            container.RegisterType<AddProceddingWindow>();
            container.RegisterType<GeneralManagerWindow>();
            container.RegisterType<PersonDetailsWindow>();
            container.RegisterType<DataUpdateWindow>();
            container.RegisterType<DocumentShowWindow>();
            container.RegisterType<OrderHistoryWindow>();
            container.RegisterType<StoreAssetDetailsWindow>();
            container.RegisterType<ProceeedingDetailsWindow>();
            container.RegisterType<ProceedingHistoryWindow>();
            container.RegisterType<DocumentHistoryWindow>();
            container.RegisterType<StoreBillDetailsWindow>();
            container.RegisterType<MeterBillWindow>();
            container.RegisterType<ParentAssetForBelongingWindow>();
            container.RegisterType<BelongingsWindow>();
            container.RegisterType<SubOrderDetailsWindow>();
            container.RegisterType<MAssetListWindow>();
            container.RegisterType<ReportViewWindow>();
            container.RegisterType<OrderUserHistoryWindow>();
            container.RegisterType<UserConfigWindow>();
            container.RegisterType<AccountingMainWindow>();
            container.RegisterType<OldLabelEditWindow>();
            container.RegisterType<EventLogWindow>();
            container.RegisterType<MAssetCostWindow>();
            container.RegisterType<ShowAccessFileWindow>();
            container.RegisterType<PermEditDetailsWindow>();
            container.RegisterType<DatabaseManageWindow>();
            container.RegisterType<ExternalOrderDetailsWindow>();
            container.RegisterType<AddExternalOrderWindow>();
            container.RegisterType<SupplierDetailsWindow>();
            container.RegisterType<SupplierIndentWindow>();
            container.RegisterType<MainAccounDocWindow>();
            container.RegisterType<InsuranceManageWindow>();
            container.RegisterType<CommodityDetailsWindow>();
            container.RegisterType<CommoditySplitWindow>();
            container.RegisterType<StoreBillToDocumentWindow>();
            container.RegisterType<AddBuyAssetWindow>();
            container.RegisterType<HelpWindow>();
            container.RegisterType<AddReturnIndentRequestWindow>();
            container.RegisterType<StoreBillMAssetEdtiWindow>();
            container.RegisterType<SupplierProFormaUploadWindow>();
            container.RegisterType<TrenderOffersWindow>();
            container.RegisterType<StuffConfigWindow>();
            navigation = container.Resolve<INavigationService>();
        }

        public void Init()
        {
            this.InitializeComponent();
        }

        /// <summary> 
        /// Raises the <see cref="E:System.Windows.Application.Startup"/> event. 
        /// </summary> 
        /// <param name="e">A <see cref="T:System.Windows.StartupEventArgs"/> that contains theevent data.</param> 
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            //navigation.ShowStartWindow();
            var viewModel = container.Resolve<MainWindowViewModel>();
            navigation.ShowMainWindow(viewModel);
        }

        public void Current_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            var exception = e.Exception;
            MessageBox.Show(exception.Message, "خطای ناشناخته", MessageBoxButton.OK, MessageBoxImage.Error);
            string path = APPSettings.Default.LogPath;
            if (!string.IsNullOrWhiteSpace(path))
            {
                DateTime date = Common.GlobalClass._Today;
                string name =path+"\\"+date.Year + "-" + date.Month + "-" + date.Day + "---" + date.Hour + "-" + date.Minute+"-"+date.Second+".txt";
                File.AppendAllText(name, exception.ToString());
            }
            e.Handled = true;
            App.Current.Shutdown();
        }
    }
}

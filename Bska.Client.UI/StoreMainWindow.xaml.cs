using Bska.Client.Common;
using Bska.Client.UI.Helper;
using Bska.Client.UI.ViewModels;
using Bska.Client.UI.ViewModels.OrderViewModel;
using Bska.Client.UI.ViewModels.StoreViewModel;
using Bska.Client.UI.ViewModels.StuffHonestViewModel;
using Bska.Client.UI.Views.StoreView;
using Microsoft.Practices.Unity;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace Bska.Client.UI
{
    /// <summary>
    /// Interaction logic for StoreMainWindow.xaml
    /// </summary>
    public partial class StoreMainWindow : Window
    {
        private readonly IUnityContainer _container;
        private readonly bool _isQuickLunch;
        private int _quickNo;
        private string _title = "";
        public StoreMainWindow(IUnityContainer container, bool isQuciLunch, int quickNo)
        {
            InitializeComponent();
            _container = container;
            this._isQuickLunch = isQuciLunch;
            this._quickNo = quickNo;
        }
        
        private void showContent()
        {
            if (_quickNo == 1002)
            {
                _title = "A1";
            }
            else if (_quickNo == 1001)
            {
                _title = "A2";
            }
            else if (_quickNo == 1003)
            {
                _title = "A5";
            }
            else if (_quickNo == 1010)
            {
                _title = "A6";
            }
            else if (_quickNo == 1011)
            {
                _title = "A7";
            }
            else if (_quickNo == 1007)
            {
                _title = "A10";
            }
            else if (_quickNo == 1008)
            {
                _title = "A8";
            }
            else if (_quickNo == 1009)
            {
                _title = "A9";
            }
            else if (_quickNo == 1005)
            {
                _title = "A3";
            }
            else if (_quickNo == 1006)
            {
                _title = "A4";
            }
            else if (_quickNo == 1012)
            {
                _title = "A11";
            }

            if (!string.IsNullOrEmpty(_title))
            {
                var item = this.LbxMenu.Items.OfType<MetroMenuItem>().Where(t => t.Id == _title).Single();
                this.LbxMenu.SelectedItem = item;
            }
        }

        private void window_Loaded(object sender, RoutedEventArgs e)
        {
            this.showContent();
            if (!APPSettings.Default.IsClosedMenu)
            {
                ((Storyboard)this.Resources["ExpandingStoryboard"]).Begin(this);
            }
        }

        private async void LbxMenu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var lsb = sender as ListBox;
            var menu = lsb.SelectedItem as MetroMenuItem;
            if (menu == null) return;
            if (APPSettings.Default.IsClosedMenu)
            {
                ((Storyboard)this.Resources["ContractingStoryboard"]).Begin(this);
            }
            this.Cursor = Cursors.Wait;
            Task ts = new Task(() =>
            {
                DispatchService.Invoke(() =>
                {
                    Page page = null;
                    if ( menu.Id== "A2")
                    {
                        page = new StoreMAssetViewPage();
                        var viewModel = new StoreMAssetManageViewModel(_container);
                        viewModel.Window = this;
                        page.DataContext = viewModel;
                    }
                    else if (menu.Id == "A1")
                    {
                        page = _container.Resolve<StoreDesignPage>();
                        var viewModel = new StoreDesignViewModel(_container);
                        page.DataContext = viewModel;

                    }
                    else if (menu.Id == "A6")
                    {
                        page = new Bska.Client.UI.Views.StuffHonestView.DocumentPage("storeBill");
                        var viewModel = new DocumentManageViewModel(_container, 1);
                        viewModel.Window = this;
                        page.DataContext = viewModel;
                    }
                    else if (menu.Id == "A7")
                    {
                        page = new Bska.Client.UI.Views.StuffHonestView.DocumentPage("documents");
                        var viewModel = new DocumentManageViewModel(_container, 2);
                        viewModel.Window = this;
                        page.DataContext = viewModel;

                    }
                    else if (menu.Id == "A5")
                    {
                        page = _container.Resolve<StoreBillIssuancePage>();
                        var viewModel = new StoreBillIssuanceViewModel(_container);
                        viewModel.Window = this;
                        page.DataContext = viewModel;
                    }
                    else if (menu.Id == "A3")
                    {
                        page = new RecivedOrderStorePage();
                        var viewmodel = new RecivedOrderStoreViewModel(_container);
                        viewmodel.Window = this;
                        page.DataContext = viewmodel;

                    }
                    else if (menu.Id == "A4")
                    {
                        var viewModel = new StoreBuyIndentViewModel(_container);
                        viewModel.Window = this;
                        page = new StoreBuyIndentPage();
                        page.DataContext = viewModel;
                    }
                    else if(menu.Id == "A10")
                    {
                        var viewModel = new StoreProceedingViewModel(_container);
                        viewModel.Window = this;
                        page = _container.Resolve<StoreProceedingManagePage>();
                        page.DataContext = viewModel;
                    }
                    else if (menu.Id == "A8")
                    {
                        var viewModel3 = new InternalOrderViewModel(_container, OrderType.Store);
                        viewModel3.Window = Window.GetWindow(this);
                        page = _container.Resolve<StoreOrderPage>();
                        page.DataContext = viewModel3;
                    }
                    else if (menu.Id == "A9")
                    {
                        page = _container.Resolve<StoreOrderManagePage>();
                    }
                    else if (menu.Id == "A11")
                    {
                        //page = _container.Resolve<KalaManagePage>();
                        //var viewModel = new KalaManageViewModel(_container);
                        //page.DataContext = viewModel;
                    }
                    this.Title = page.Title;
                    this.frame.Content = page;
                });
            });
            ts.Start();
            await ts;
            this.Cursor = Cursors.Arrow;
        }
    }
}

using Bska.Client.UI.Helper;
using Bska.Client.UI.ViewModels;
using Bska.Client.UI.ViewModels.GeneralManagerViewModels;
using Bska.Client.UI.ViewModels.MunitionViewModel;
using Bska.Client.UI.Views.MunitionView;
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
    /// Interaction logic for MunitionMainWindow.xaml
    /// </summary>
    public partial class MunitionMainWindow : Window
    {
        private readonly IUnityContainer _container;
        private readonly bool _isQuickLunch;
        private int _quickNo;
        private string _title="";
        public MunitionMainWindow(IUnityContainer container, bool isQuciLunch, int quickNo)
        {
            InitializeComponent();
            this._container = container;
            this._isQuickLunch = isQuciLunch;
            this._quickNo = quickNo;
        }
        private void showContent()
        {
            if (_quickNo == 1001)
            {
                _title = "A2";
            }
            else if(_quickNo==1002)
            {
                _title = "A3";
            }
            else if (_quickNo == 1004)
            {
                _title = "A1";
            }
            else if(_quickNo==1003)
            {
                _title = "A4";
            }
            else if (_quickNo == 1005)
            {
                _title = "A5";
            }
            else if (_quickNo == 1006)
            {
                _title = "A6";
            }
            else if (_quickNo == 1007)
            {
                _title = "A7";
            }
            else if (_quickNo == 1008)
            {
                _title = "A8";
            }

            if (!string.IsNullOrEmpty(_title))
            {
                var item = this.LbxMenu.Items.OfType<MetroMenuItem>().Where(t => t.Id == _title).SingleOrDefault();
                this.LbxMenu.SelectedItem = item;
            }
        }

        private void munitionWindow_Loaded(object sender, RoutedEventArgs e)
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
            var index = lsb.SelectedItem as MetroMenuItem;

            if (index == null) return;
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
                    if (index.Id == "A2")
                    {
                        page = new RecivedOrderPage();
                        var viewModel = new MunitionRecivedIndentViewModel(_container);
                        viewModel.Window = this;
                        page.DataContext = viewModel;
                    }
                    else if (index.Id == "A3")
                    {
                        page = new MeterBillPage();
                        var viewModel = new MeterBillListViewModel(_container);
                        page.DataContext = viewModel;
                        viewModel.Window = this;
                    }
                    else if (index.Id == "A4")
                    {
                        page = new SupplierHistoryPage();
                        var viewModel = new SupplierHistoryViewModel(_container);
                        viewModel.Window = this;
                        page.DataContext = viewModel;
                    }
                    else if (index.Id == "A1")
                    {
                        page = new SellerManagePage();
                        var viewModel = new SellerViewModel(_container);
                        viewModel.Window = this;
                        page.DataContext = viewModel;
                    }
                    else if (index.Id == "A5")
                    {
                        page = new Views.OrderView.RecivedOrderPage();
                        var viewModel = new InternalOrderRecivedViewModel(_container, "MunitionManager");
                        viewModel.Window = this;
                        page.DataContext = viewModel;
                    }
                    else if (index.Id == "A6")
                    {
                        page = new ReturnIndentOrderPage();
                        var viewModel = new ReturnSupplierIndentViewModel(_container);
                        viewModel.Window = this;
                        page.DataContext = viewModel;
                    }
                    else if (index.Id == "A7")
                    {
                        page = new RecivedTrenderOfferPage();
                        var viewModel = new ViewModels.OrderViewModel.TrenderOfferRecivedRequestViewModel(_container, true);
                        viewModel.Window = this;
                        page.DataContext = viewModel;
                    }
                    else if (index.Id == "A8")
                    {
                        page = new RecivedtTrenderOfferToSupplierPage();
                        var viewModel = new RecivetTrenderToSupplierViewModel(_container);
                        viewModel.Window = this;
                        page.DataContext = viewModel;
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

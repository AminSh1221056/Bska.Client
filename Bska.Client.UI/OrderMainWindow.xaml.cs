using Microsoft.Practices.Unity;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using Bska.Client.UI.Views.OrderView;
using Bska.Client.UI.ViewModels.OrderViewModel;
using Bska.Client.UI.Helper;
using Bska.Client.UI.ViewModels;
using System.Linq;

namespace Bska.Client.UI
{
    /// <summary>
    /// Interaction logic for OrderMainWindow.xaml
    /// </summary>
    public partial class OrderMainWindow : Window
    {
        private readonly IUnityContainer _container;
        private readonly bool _isQuickLunch;
        private int _quickNo;
        private string _title = "";
        public OrderMainWindow(IUnityContainer container,bool isQuciLunch,int quickNo)
        {
            InitializeComponent();
            this._container = container;
            this._isQuickLunch = isQuciLunch;
            this._quickNo = quickNo;
        }
        private void window_Loaded(object sender, RoutedEventArgs e)
        {
            this.showContent();
            if (!APPSettings.Default.IsClosedMenu)
            {
                ((Storyboard)this.Resources["ExpandingStoryboard"]).Begin(this);
            }
        }

        private void showContent()
        {
            switch (_quickNo)
            {
                case 1001:
                    _title = "A1";
                    break;
                case 1002:
                    _title = "A2";
                    break;
                case 1003:
                    _title = "A3";
                    break;
                case 1004:
                    _title = "A4";
                    break;
                case 1005:
                    _title = "A5";
                    break;
            }

            if (!string.IsNullOrEmpty(_title))
            {
                var item = this.LbxMenu.Items.OfType<MetroMenuItem>().Where(t => t.Id == _title).SingleOrDefault();
                this.LbxMenu.SelectedItem = item;
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
                    if (index.Id == "A1")
                    {
                        page = new NewOrderPage(_container);
                        this.Title = page.Title;
                    }
                    else if (index.Id == "A2")
                    {
                        //page = new RecivedOrderPage();
                        //var viewModel = new RecivedOrderViewModel(_container);
                        //viewModel.Window = this;
                        //page.DataContext = viewModel;
                    }
                    else if (index.Id == "A3")
                    {
                        page = new OrderManagePage();
                        var viewModel1 = new OrderManageViewModel(_container);
                        viewModel1.Window = this;
                        page.DataContext = viewModel1;
                    }
                    else if (index.Id == "A4")
                    {
                        var uc = new Views.StuffHonestView.MAssetViewPage();
                        var viewModel = new ViewModels.StuffHonestViewModel.MAssetManageViewModel(_container);
                        viewModel.Window = this;
                        uc.DataContext = viewModel;
                    }
                    else if(index.Id == "A5")
                    {
                        page = new TrenderOfferRequestPage();
                        var viewModel = new TrenderOfferRecivedRequestViewModel(_container,false);
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

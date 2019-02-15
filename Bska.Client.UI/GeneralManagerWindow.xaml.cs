using Bska.Client.UI.Helper;
using Bska.Client.UI.ViewModels;
using Bska.Client.UI.ViewModels.GeneralManagerViewModels;
using Bska.Client.UI.Views.GeneralManagerView;
using Bska.Client.UI.Views.OrderView;
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
    /// Interaction logic for GeneralManagerWindow.xaml
    /// </summary>
    public partial class GeneralManagerWindow : Window
    {
        private readonly IUnityContainer _container;
        private readonly bool _isQuickLunch;
        private int _quickNo;
        private string _title = "";

        public GeneralManagerWindow(IUnityContainer container, bool isQuciLunch, int quickNo)
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
                _title = "A1";
            }
            else if (_quickNo == 1003)
            {
                _title = "A3";
            }
            else if (_quickNo == 1002)
            {
                _title = "A2";
            }
            else if(_quickNo==1005)
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
            if (!string.IsNullOrEmpty(_title))
            {
                var item = this.LbxMenu.Items.OfType<MetroMenuItem>().Where(t => t.Id == _title).Single();
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
                    if (index.Id == "A1")
                    {
                        page = new RecivedProceedingPage();
                        var viewModel = new RecivedProceedingViewModel(_container);
                        viewModel.Window = this;
                        page.DataContext = viewModel;
                    }
                    else if (index.Id == "A3")
                    {
                        page = new RecivedOrderPage();
                        var viewModel = new InternalOrderRecivedViewModel(_container, "GeneralManager");
                        viewModel.Window = this;
                        page.DataContext = viewModel;
                    }
                    else if (index.Id == "A2")
                    {
                        page = new StoreOrderRecivedPage();
                        var vieModel = new ExternalOrderRecivedViewModel(_container);
                        vieModel.Window = this;
                        page.DataContext = vieModel;
                    }
                    else if(index.Id == "A5")
                    {
                        page = new StoreBillEditRequestRecivedPage();
                        var viewModel = new StoreBillEditRecivedViewModel(_container);
                        viewModel.Window = this;
                        page.DataContext = viewModel;
                    }
                    else if (index.Id == "A6")
                    {
                        page = new IndentReturnRequestPage();
                        var viewModel = new RecivedIndentReturnRequestViewModel(_container);
                        viewModel.Window = this;
                        page.DataContext = viewModel;
                    }
                    else if (index.Id == "A7")
                    {
                        page = new RelaseStuffRecivedRequestPage();
                        var viewModel = new RelaseAssetRequestViewModel(_container);
                        viewModel.Window = this;
                        page.DataContext = viewModel;
                    }

                    this.frame.Content = page;
                    this.Title = page.Title;
                });
            });
            ts.Start();
            await ts;
            this.Cursor = Cursors.Arrow;
        }
    }
}

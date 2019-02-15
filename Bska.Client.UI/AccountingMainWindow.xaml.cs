using Bska.Client.UI.Helper;
using Bska.Client.UI.ViewModels;
using Bska.Client.UI.ViewModels.AccountingViewModels;
using Bska.Client.UI.Views.AccountingView;
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
    /// Interaction logic for AccountingMainWindow.xaml
    /// </summary>
    public partial class AccountingMainWindow : Window
    {
        private readonly IUnityContainer _container;
        private readonly bool _isQuickLunch;
        private int _quickNo;
        private string _title = "";
        public AccountingMainWindow(IUnityContainer container, bool isQuciLunch, int quickNo)
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
            else if (_quickNo == 1002)
            {
                _title = "A2";
            }

            if (!string.IsNullOrEmpty(_title))
            {
                var item = this.LbxMenu.Items.OfType<MetroMenuItem>().Where(t => t.Id == _title).Single();
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
                        page = new AccountCodingPage();
                        var viewModel = new AccountCodingDesignViewModel(_container);
                        page.DataContext = viewModel;
                    }
                    else if (index.Id == "A2")
                    {
                        page = new AccountDocumentHistoryPage();
                        var viewModel = new AccountDocumentHistoryViewModel(_container);
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

        private void munitionWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.showContent();
            if (!APPSettings.Default.IsClosedMenu)
            {
                ((Storyboard)this.Resources["ExpandingStoryboard"]).Begin(this);
            }
        }
    }
}

using Bska.Client.UI.API;
using Bska.Client.UI.ViewModels.AccountingViewModels;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Bska.Client.UI.Views.AccountingView
{
    /// <summary>
    /// Interaction logic for AccountInitializationUC.xaml
    /// </summary>
    public partial class AccountInitializationUC : UserControl
    {
        private readonly IUnityContainer _container;
        public AccountInitializationUC(IUnityContainer container)
        {
            InitializeComponent();
            this._container = container;

            List<Tuple<int, string, Color>> folders = new List<Tuple<int, string, Color>>();
            folders.Add(new Tuple<int, string, Color>(1, "کدینگ اسناد(Ctrl+S)", BOT.ParseHexColor("#FF1FAEFF")));

            FoldersShow.DataContext = folders;
            this.codingPane.Visibility = Visibility.Collapsed;
        }

        private void FoldersShow_ComboColorOrganizChanged(object sender, RoutedEventArgs e)
        {
            ListBox item = e.OriginalSource as ListBox;
            if (item != null)
            {
                int selectedValue = (int)item.SelectedValue;
                switch (selectedValue)
                {
                    case 1:
                        var viewModel = new AccountCodingDesignViewModel(_container);
                        this.codingPane.Visibility = Visibility.Visible;
                        this.codingPane.DataContext = viewModel;
                        break;
                    case 2:
                       
                        break;
                }
            }
        }
    }
}

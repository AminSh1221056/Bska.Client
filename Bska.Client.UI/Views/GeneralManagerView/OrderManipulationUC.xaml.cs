using Bska.Client.UI.API;
using Bska.Client.UI.ViewModels.GeneralManagerViewModels;
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

namespace Bska.Client.UI.Views.GeneralManagerView
{
    /// <summary>
    /// Interaction logic for OrderManipulationUC.xaml
    /// </summary>
    public partial class OrderManipulationUC : UserControl
    {
        private readonly IUnityContainer _container;
        public OrderManipulationUC(IUnityContainer container)
        {
            InitializeComponent();
            this._container = container;

            List<Tuple<int, string, Color>> folders = new List<Tuple<int, string, Color>>();
            folders.Add(new Tuple<int, string, Color>(1, "درخواست کالا انبار(Ctrl+S)", BOT.ParseHexColor("#FF1FAEFF")));
            folders.Add(new Tuple<int, string, Color>(2, "درخواست کالا خارجی(Ctrl+D)", BOT.ParseHexColor("#FF1FAEFF")));

            FoldersShow.DataContext = folders;
            this.internalOrderUc.Visibility = Visibility.Collapsed;
            this.storeOrderUc.Visibility = Visibility.Collapsed;
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
                        this.internalOrderUc.Visibility = Visibility.Visible;
                        this.storeOrderUc.Visibility = Visibility.Collapsed;
                        var viewModel = new InternalOrderRecivedViewModel(_container, "GeneralManager");
                         viewModel.Window = Window.GetWindow(this);
                        this.internalOrderUc.DataContext = viewModel;
                        break;
                    case 2:
                        this.internalOrderUc.Visibility = Visibility.Collapsed;
                        this.storeOrderUc.Visibility = Visibility.Visible;
                        break;
                }
            }
        }
    }
}

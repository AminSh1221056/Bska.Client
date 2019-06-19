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
    /// Interaction logic for RequestManipulationUC.xaml
    /// </summary>
    public partial class RequestManipulationUC : UserControl
    {
        private readonly IUnityContainer _container;

        public RequestManipulationUC(IUnityContainer container)
        {
            InitializeComponent();
            this._container = container;

            List<Tuple<int, string, Color>> folders = new List<Tuple<int, string, Color>>();
            folders.Add(new Tuple<int, string, Color>(1, "درخواست صورت جلسه(Ctrl+S)", BOT.ParseHexColor("#FF1FAEFF")));
            folders.Add(new Tuple<int, string, Color>(2, "درخواست ویرایش قبض انبار(Ctrl+D)", BOT.ParseHexColor("#FF1FAEFF")));
            folders.Add(new Tuple<int, string, Color>(3, "درخواست عودت خرید(Ctrl+F)", BOT.ParseHexColor("#FF1FAEFF")));
            folders.Add(new Tuple<int, string, Color>(4, "درخواست آزاد کردن مال(Ctrl+G)", BOT.ParseHexColor("#FF1FAEFF")));

            FoldersShow.DataContext = folders;
            this.indentReturnPane.Visibility = Visibility.Collapsed;
            this.proceedingPane.Visibility = Visibility.Collapsed;
            this.storeBillEditPane.Visibility = Visibility.Collapsed;
            this.relaseStuffPane.Visibility = Visibility.Collapsed;
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
                        var proceedingViewModel = new RecivedProceedingViewModel(_container);
                        this.proceedingPane.DataContext = proceedingViewModel;

                        this.indentReturnPane.Visibility = Visibility.Collapsed;
                        this.proceedingPane.Visibility = Visibility.Visible;
                        this.storeBillEditPane.Visibility = Visibility.Collapsed;
                        this.relaseStuffPane.Visibility = Visibility.Collapsed;
                        break;
                    case 2:
                        var storeBillEditViewModel = new StoreBillEditRecivedViewModel(_container);
                        this.indentReturnPane.Visibility = Visibility.Collapsed;
                        this.proceedingPane.Visibility = Visibility.Collapsed;
                        this.storeBillEditPane.Visibility = Visibility.Visible;
                        this.relaseStuffPane.Visibility = Visibility.Collapsed;

                        this.storeBillEditPane.DataContext = storeBillEditViewModel;
                        break;
                    case 3:
                        var indentReturnViewModel = new RecivedIndentReturnRequestViewModel(_container);

                        this.indentReturnPane.Visibility = Visibility.Visible;
                        this.proceedingPane.Visibility = Visibility.Collapsed;
                        this.storeBillEditPane.Visibility = Visibility.Collapsed;
                        this.relaseStuffPane.Visibility = Visibility.Collapsed;

                        this.indentReturnPane.DataContext = indentReturnViewModel;
                        break;
                    case 4:
                        var relaseViewModel = new RelaseAssetRequestViewModel(_container);

                        this.indentReturnPane.Visibility = Visibility.Collapsed;
                        this.proceedingPane.Visibility = Visibility.Collapsed;
                        this.storeBillEditPane.Visibility = Visibility.Collapsed;
                        this.relaseStuffPane.Visibility = Visibility.Visible;

                        this.relaseStuffPane.DataContext = relaseViewModel;
                        break;
                }
            }
        }
    }
}

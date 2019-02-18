using Bska.Client.UI.API;
using Bska.Client.UI.ViewModels.StuffHonestViewModel;
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

namespace Bska.Client.UI.Views.StuffHonestView
{
    /// <summary>
    /// Interaction logic for ProcedingManipulationUC.xaml
    /// </summary>
    public partial class ProcedingManipulationUC : UserControl
    {
        private readonly IUnityContainer _container;
        public ProcedingManipulationUC(IUnityContainer container)
        {
            InitializeComponent();

            this._container = container;
            List<Tuple<int, string, Color>> folders = new List<Tuple<int, string, Color>>();
            folders.Add(new Tuple<int, string, Color>(1, "درخواست های صورت جلسه(Ctrl+S)", BOT.ParseHexColor("#FF1FAEFF")));
            folders.Add(new Tuple<int, string, Color>(2, "ثبت صورت جلسه(Ctrl+D)", BOT.ParseHexColor("#FF1FAEFF")));
            folders.Add(new Tuple<int, string, Color>(3, "صورت جلسات ثبت شده(Ctrl+F)", BOT.ParseHexColor("#FF1FAEFF")));

            this.procedingPane.Visibility = Visibility.Collapsed;
            this.addProcedingPane.Visibility = Visibility.Collapsed;

            FoldersShow.DataContext = folders;
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
                        this.procedingPane.Visibility = Visibility.Visible;
                        this.addProcedingPane.Visibility = Visibility.Collapsed;

                        var viewModel = new ProceedingViewModel(_container);
                        viewModel.Window = Window.GetWindow(this);
                        procedingPane.DataContext = viewModel;
                        break;
                    case 2:
                        this.procedingPane.Visibility = Visibility.Collapsed;
                        this.addProcedingPane.Visibility = Visibility.Visible;

                        var addProcViewModel = new AddProceedingViewModel(_container);
                        addProcedingPane.DataContext = addProcViewModel;
                        break;
                    default:
                        this.procedingPane.Visibility = Visibility.Collapsed;
                        break;
                }
            }
        }
    }
}

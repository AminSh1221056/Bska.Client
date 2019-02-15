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
    /// Interaction logic for MAssetManipulationUC.xaml
    /// </summary>
    public partial class MAssetManipulationUC : UserControl
    {
        private readonly IUnityContainer _container;
        public MAssetManipulationUC(IUnityContainer container)
        {
            InitializeComponent();

            this._container = container;
            List<Tuple<int, string, Color>> folders = new List<Tuple<int, string, Color>>();
            folders.Add(new Tuple<int, string, Color>(1, "دفتر اموال(Ctrl+S)", BOT.ParseHexColor("#FF1FAEFF")));
            folders.Add(new Tuple<int, string, Color>(2, "موجودی خارج از انبار(Ctrl+D)", BOT.ParseHexColor("#FF1FAEFF")));
            folders.Add(new Tuple<int, string, Color>(3, "بررسی اموال(Ctrl+F)", BOT.ParseHexColor("#FF1FAEFF")));

            FoldersShow.DataContext = folders;
            this.FoldersShow.m_listbox.SelectedIndex = 0;
            this.MAssetPane.Visibility = Visibility.Collapsed;
            this.organAssetPane.Visibility = Visibility.Collapsed;
            this.stuffInformationPane.Visibility = Visibility.Collapsed;
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
                        organAssetPane.Visibility = Visibility.Visible;
                        MAssetPane.Visibility = Visibility.Collapsed;
                        stuffInformationPane.Visibility = Visibility.Collapsed;
                        
                        var viewModel = new OrganMovabelAssetHistoryViewModel(_container);
                        organAssetPane.DataContext = viewModel;
                        break;
                    case 2:
                        organAssetPane.Visibility = Visibility.Collapsed;
                        MAssetPane.Visibility = Visibility.Visible;
                        stuffInformationPane.Visibility = Visibility.Collapsed;

                        var massetViewModel = new MAssetManageViewModel(_container);
                        this.MAssetPane.DataContext = massetViewModel;
                        break;
                    case 3:
                        organAssetPane.Visibility = Visibility.Collapsed;
                        MAssetPane.Visibility = Visibility.Collapsed;
                        stuffInformationPane.Visibility = Visibility.Visible;

                        var stuffInfoViewModel = new StuffInformationViewModel(_container);
                        this.stuffInformationPane.DataContext = stuffInfoViewModel;
                        break;
                }
            }
        }
    }
}

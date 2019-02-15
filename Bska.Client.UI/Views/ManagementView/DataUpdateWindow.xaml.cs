using Bska.Client.UI.API;
using Bska.Client.UI.ViewModels;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Bska.Client.UI.Views
{
    /// <summary>
    /// Interaction logic for DataUpdateWindow.xaml
    /// </summary>
    public partial class DataUpdateWindow : Window
    {
        private IUnityContainer _containre;
        public DataUpdateWindow(IUnityContainer container)
        {
            InitializeComponent(); 
            this._containre = container;
            List<Tuple<int, string, Color>> folders = new List<Tuple<int, string, Color>>();
            folders.Add(new Tuple<int, string, Color>(1, "اطلاعات اولیه(Ctrl+S)", BOT.ParseHexColor("#FF1FAEFF")));
            folders.Add(new Tuple<int, string, Color>(2, "سازمان(Ctrl+D)", BOT.ParseHexColor("#FF1FAEFF")));
            folders.Add(new Tuple<int, string, Color>(3, "اموال(Ctrl+F)", BOT.ParseHexColor("#FF1FAEFF")));
            folders.Add(new Tuple<int, string, Color>(4, "صورت جلسه(Ctrl+G)", BOT.ParseHexColor("#FF1FAEFF")));
            folders.Add(new Tuple<int, string, Color>(5, "دارایی(Ctrl+H)", BOT.ParseHexColor("#FF1FAEFF")));
            FoldersShow.DataContext = folders;
        }
        private void CloseCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.Close();
        }
        private void FoldersShow_ComboColorOrganizChanged(object sender, RoutedEventArgs e)
        {
            Mouse.SetCursor(Cursors.Wait);
            ListBox item = e.OriginalSource as ListBox;
            if (item != null)
            {
                int selectedValue = (int)item.SelectedValue;
                switch (selectedValue)
                {
                    case 1:
                        this.accessExportUC.Visibility = Visibility.Collapsed;
                        this.assetSendPane.Visibility = Visibility.Collapsed;
                        this.proceedingPane.Visibility = Visibility.Collapsed;
                        this.BaseInfoSendUC.Visibility = Visibility.Collapsed;
                        this.seedDataPane.Visibility = Visibility.Visible;
                        var viewModel = new SeedDataUpdateViewModel(_containre);
                        this.seedDataPane.DataContext = viewModel;
                        break;
                    case 2:
                        this.accessExportUC.Visibility = Visibility.Collapsed;
                        this.assetSendPane.Visibility = Visibility.Collapsed;
                        this.proceedingPane.Visibility = Visibility.Collapsed;
                        this.seedDataPane.Visibility = Visibility.Collapsed;
                        this.BaseInfoSendUC.Visibility = Visibility.Visible;
                        var viewModel3 = new BaseInfoSendViewModel(_containre);
                        this.BaseInfoSendUC.DataContext = viewModel3;
                        break;
                    case 3:
                        this.accessExportUC.Visibility = Visibility.Collapsed;
                        this.assetSendPane.Visibility = Visibility.Visible;
                        this.proceedingPane.Visibility = Visibility.Collapsed;
                        this.seedDataPane.Visibility = Visibility.Collapsed;
                        this.BaseInfoSendUC.Visibility = Visibility.Collapsed;
                        var viewModel1 = new AssetSendViewModel(_containre, this);
                        this.assetSendPane.DataContext = viewModel1;
                        break;
                    case 4:
                        this.accessExportUC.Visibility = Visibility.Collapsed;
                        this.assetSendPane.Visibility = Visibility.Collapsed;
                        this.BaseInfoSendUC.Visibility = Visibility.Collapsed;
                        this.seedDataPane.Visibility = Visibility.Collapsed;
                        this.proceedingPane.Visibility = Visibility.Visible;
                        var viewModel2 = new ProceedingSendViewModel(_containre);
                        this.proceedingPane.DataContext = viewModel2;
                        break;
                    case 5:
                        this.assetSendPane.Visibility = Visibility.Collapsed;
                        this.proceedingPane.Visibility = Visibility.Collapsed;
                        this.seedDataPane.Visibility = Visibility.Collapsed;
                        this.BaseInfoSendUC.Visibility = Visibility.Collapsed;
                        this.accessExportUC.Visibility = Visibility.Visible;
                        var viewModel4 = new AccessExportViewModel(_containre);
                        viewModel4.Window = this;
                        this.accessExportUC.DataContext = viewModel4;
                        break;
                }
                Mouse.SetCursor(Cursors.Arrow);
            }
        }

        private void userWin_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}

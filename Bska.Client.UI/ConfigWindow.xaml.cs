using Bska.Client.UI.API;
using Bska.Client.UI.ViewModels;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Bska.Client.UI
{
    /// <summary>
    /// Interaction logic for ConfigWindow.xaml
    /// </summary>
    public partial class ConfigWindow : Window
    {
        private IUnityContainer _containre;
        public ConfigWindow(IUnityContainer container)
        {
            InitializeComponent();
            this._containre = container;
            List<Tuple<int, string, Color>> folders = new List<Tuple<int, string, Color>>();
            folders.Add(new Tuple<int, string, Color>(2, "DB Servers", BOT.ParseHexColor("#FF1FAEFF")));
            folders.Add(new Tuple<int, string, Color>(1, "Connection Config", BOT.ParseHexColor("#FF1FAEFF")));
            folders.Add(new Tuple<int, string, Color>(3, "Services", BOT.ParseHexColor("#FF1FAEFF")));
            folders.Add(new Tuple<int, string, Color>(4, "Report Server", BOT.ParseHexColor("#FF1FAEFF")));
            folders.Add(new Tuple<int, string, Color>(5, "Init", BOT.ParseHexColor("#FF1FAEFF")));

            FoldersShow.DataContext = folders;
        }

        private void CloseCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.Close();
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
                        var viewModel = new DatabaseConfigViewModel(_containre);
                        this.databaseConfigPane.Visibility = Visibility.Visible;
                        this.NetConfigPane.Visibility = Visibility.Collapsed;
                        this.SsrsPane.Visibility = Visibility.Collapsed;
                        this.initPane.Visibility = Visibility.Collapsed;
                        this.databaseConfigPane.DataContext = viewModel;
                        this.dbServersPane.Visibility = Visibility.Collapsed;
                        break;
                    case 2:
                        var viewModel4 = new DBServersViewModel(_containre);
                        this.databaseConfigPane.Visibility = Visibility.Collapsed;
                        this.NetConfigPane.Visibility = Visibility.Collapsed;
                        this.SsrsPane.Visibility = Visibility.Collapsed;
                        this.initPane.Visibility = Visibility.Collapsed;
                        this.dbServersPane.Visibility = Visibility.Visible;
                        this.dbServersPane.DataContext = viewModel4;
                        break;
                    case 3:
                        var viewModel1 = new NetworkConfigViewModel(_containre);
                        this.databaseConfigPane.Visibility = Visibility.Collapsed;
                        this.SsrsPane.Visibility = Visibility.Collapsed;
                        this.NetConfigPane.Visibility = Visibility.Visible;
                        this.dbServersPane.Visibility = Visibility.Collapsed;
                        this.initPane.Visibility = Visibility.Collapsed;
                        this.NetConfigPane.DataContext = viewModel1;
                        break;
                    case 4:
                        var viewModel2 = new SSRSConfigViewModel(_containre);
                        this.databaseConfigPane.Visibility = Visibility.Collapsed;
                        this.NetConfigPane.Visibility = Visibility.Collapsed;
                        this.SsrsPane.Visibility = Visibility.Visible;
                        this.dbServersPane.Visibility = Visibility.Collapsed;
                        this.initPane.Visibility = Visibility.Collapsed;
                        this.SsrsPane.DataContext = viewModel2;
                        break;
                    case 5:
                        var viewModel3 = new InitAppViewModel(_containre);
                        this.databaseConfigPane.Visibility = Visibility.Collapsed;
                        this.NetConfigPane.Visibility = Visibility.Collapsed;
                        this.SsrsPane.Visibility = Visibility.Collapsed;
                        this.initPane.Visibility = Visibility.Visible;
                        this.dbServersPane.Visibility = Visibility.Collapsed;
                        this.initPane.DataContext = viewModel3;
                        this.initPane.globalToolPane.cancelbtn.Visibility = Visibility.Collapsed;
                        this.initPane.globalToolPane.deletebtn.Visibility = Visibility.Collapsed;
                        this.initPane.globalToolPane.newbtn.Visibility = Visibility.Collapsed;
                        this.initPane.globalToolPane.reportbtn.Visibility = Visibility.Collapsed;
                        this.initPane.globalToolPane.editbtn.Visibility = Visibility.Collapsed;
                        this.initPane.globalToolPane.filterbtn.Visibility = Visibility.Collapsed;
                        this.initPane.globalToolPane.gridsearchbtn.Visibility = Visibility.Collapsed;
                        break;
                }
            }
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}

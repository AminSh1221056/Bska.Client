using Bska.Client.UI.API;
using Bska.Client.UI.ViewModels.ManagerViewModels;
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
    /// Interaction logic for StuffConfigWindow.xaml
    /// </summary>
    public partial class StuffConfigWindow : Window
    {
        private readonly IUnityContainer _container;
        public StuffConfigWindow(IUnityContainer container)
        {
            InitializeComponent();
            this._container = container;
            this.stuffConfig.Visibility = Visibility.Collapsed;
            this.kalaManage.Visibility = Visibility.Collapsed;

            List<Tuple<int, string, Color>> folders = new List<Tuple<int, string, Color>>();
            folders.Add(new Tuple<int, string, Color>(1, "تنظیمات تایید(Ctrl+S)", BOT.ParseHexColor("#FF1FAEFF")));
            folders.Add(new Tuple<int, string, Color>(2, "ثبت مال جدید(Ctrl+D)", BOT.ParseHexColor("#FF1FAEFF")));

            FoldersShow.DataContext = folders;
        }

        private void stuffConfigWin_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
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
                        this.kalaManage.Visibility = Visibility.Collapsed;
                        this.stuffConfig.Visibility = Visibility.Visible;
                        this.stuffConfig.DataContext = new StuffConfigViewModel(_container);
                        break;
                    case 2:
                        this.stuffConfig.Visibility = Visibility.Collapsed;
                        this.kalaManage.Visibility = Visibility.Visible;
                        this.kalaManage.globalToolPane.cancelbtn.Visibility = Visibility.Collapsed;
                        this.kalaManage.globalToolPane.filterbtn.Visibility = Visibility.Collapsed;
                        this.kalaManage.globalToolPane.editbtn.Visibility = Visibility.Collapsed;
                        this.kalaManage.globalToolPane.newbtn.Visibility = Visibility.Collapsed;
                        this.kalaManage.globalToolPane.gridsearchbtn.Visibility = Visibility.Collapsed;
                        var viewModel = new ViewModels.StoreViewModel.KalaManageViewModel(_container);
                        this.kalaManage.DataContext = viewModel;
                        break;
                }
            }
        }
    }
}

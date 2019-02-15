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
    /// Interaction logic for PersonManageWindow.xaml
    /// </summary>
    public partial class PersonManageWindow : Window
    {
        private readonly IUnityContainer _container;
        public PersonManageWindow(IUnityContainer container)
        {
            InitializeComponent();
            this._container = container;
            List<Tuple<int, string, Color>> folders = new List<Tuple<int, string, Color>>();
            folders.Add(new Tuple<int, string, Color>(1, "پرسنل(Ctrl+S)", BOT.ParseHexColor("#FF1FAEFF")));
            folders.Add(new Tuple<int, string, Color>(2, "مدیریت درخواست(Ctrl+D)", BOT.ParseHexColor("#FF1FAEFF")));

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
                        this.PersonPane.Visibility = Visibility.Visible;
                        this.RequestManagerPane.Visibility = Visibility.Collapsed;
                        var viewModel = new PersonListViewModel(_container);
                        this.PersonPane.DataContext = viewModel;
                        this.PersonPane.toolbarPane.FilterTextBox.Focus();
                        this.PersonPane.toolbarPane.editbtn.Visibility = Visibility.Collapsed;
                        this.PersonPane.toolbarPane.filterbtn.Visibility = Visibility.Collapsed;
                        this.PersonPane.importPane.accessbtn.Visibility = Visibility.Collapsed;
                        break;
                    case 2:
                        this.PersonPane.Visibility = Visibility.Collapsed;
                        this.RequestManagerPane.Visibility = Visibility.Visible;
                        var viewModel1 = new RequestManagerViewModel(_container);
                        this.RequestManagerPane.DataContext = viewModel1;
                        this.RequestManagerPane.toolbarPane.FilterTextBox.Focus();
                        this.RequestManagerPane.toolbarPane.editbtn.Visibility = Visibility.Collapsed;
                        this.RequestManagerPane.toolbarPane.filterbtn.Visibility = Visibility.Collapsed;
                        this.RequestManagerPane.toolbarPane.deletebtn.Visibility = Visibility.Collapsed;
                        this.RequestManagerPane.toolbarPane.savebtn.Visibility = Visibility.Collapsed;
                        this.RequestManagerPane.toolbarPane.reportbtn.Visibility = Visibility.Collapsed;
                        this.RequestManagerPane.toolbarPane.cancelbtn.Visibility = Visibility.Collapsed;
                        this.RequestManagerPane.toolbarPane.gridsearchbtn.Visibility = Visibility.Collapsed;
                        this.RequestManagerPane.cmbPersons.Focus();
                        break;
                }
            }
        }

        private void personManageWin_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}

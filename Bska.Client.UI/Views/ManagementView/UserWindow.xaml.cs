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
    /// Interaction logic for UserWindow.xaml
    /// </summary>
    public partial class UserWindow : Window
    {
        private IUnityContainer _containre;
        public UserWindow(IUnityContainer container)
        {
            InitializeComponent();
            this._containre = container;
            List<Tuple<int, string, Color>> folders = new List<Tuple<int, string, Color>>();
            folders.Add(new Tuple<int, string, Color>(1, "کاربری(Ctrl+S)", BOT.ParseHexColor("#FF1FAEFF")));
            folders.Add(new Tuple<int, string, Color>(2, "مجوز ها(Ctrl+D)", BOT.ParseHexColor("#FF1FAEFF")));
            folders.Add(new Tuple<int, string, Color>(3, "مدیریت نقش ها(Ctrl+F)", BOT.ParseHexColor("#FF1FAEFF")));

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
                        this.UserPane.Visibility = Visibility.Visible;
                        this.userMangePane.Visibility = Visibility.Collapsed;
                        this.userRolePane.Visibility = Visibility.Collapsed;
                        this.UserPane.DataContext = new UsersListViewModel(_containre);

                        this.UserPane.toolbarPane.FilterTextBox.Focus();
                        this.UserPane.toolbarPane.editbtn.Visibility = Visibility.Collapsed;
                        this.UserPane.toolbarPane.gridToolsbtn.Visibility = Visibility.Collapsed;
                        break;
                    case 2:
                        this.UserPane.Visibility = Visibility.Collapsed;
                        this.userMangePane.Visibility = Visibility.Visible;
                        this.userRolePane.Visibility = Visibility.Collapsed;
                        var viewModel1 = new UserManageViewModel(_containre);
                        this.userMangePane.DataContext = viewModel1;
                        this.userMangePane.toolbarPane.editbtn.Visibility = Visibility.Collapsed;
                        this.userMangePane.toolbarPane.gridToolsbtn.Visibility = Visibility.Collapsed;
                        this.userMangePane.toolbarPane.cancelbtn.Visibility = Visibility.Collapsed;
                        this.userMangePane.toolbarPane.newbtn.Visibility = Visibility.Collapsed;
                        this.userMangePane.toolbarPane.deletebtn.Visibility = Visibility.Collapsed;
                        this.userMangePane.toolbarPane.gridsearchbtn.Visibility = Visibility.Collapsed;
                        this.userMangePane.borderUserList.Focus();
                        break;
                    case 3:
                        this.UserPane.Visibility = Visibility.Collapsed;
                        this.userMangePane.Visibility = Visibility.Collapsed;
                        this.userRolePane.Visibility = Visibility.Visible;
                        var viewModel2 = new UserRoleViewModel(_containre);
                        this.userRolePane.DataContext = viewModel2;
                        break;
                }
            }
        }

        private void userWin_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}

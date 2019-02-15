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
    /// Interaction logic for DatabaseManageWindow.xaml
    /// </summary>
    public partial class DatabaseManageWindow : Window
    {
        private IUnityContainer _containre;
        public DatabaseManageWindow(IUnityContainer container)
        {
            InitializeComponent();
            this._containre = container;
            List<Tuple<int, string, Color>> folders = new List<Tuple<int, string, Color>>();
            folders.Add(new Tuple<int, string, Color>(1, "پشتیبان گیری", BOT.ParseHexColor("#FF1FAEFF")));
            FoldersShow.DataContext = folders;
        }

        private void databaseWin_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
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
                        this.backupPane.Visibility = Visibility.Visible;
                        var viewModel = new DatabaseBackupViewModel(_containre);
                        this.backupPane.DataContext = viewModel;
                        break;
                    default:
                        this.backupPane.Visibility = Visibility.Collapsed;
                        break;
                }
            }
        }
    }
}

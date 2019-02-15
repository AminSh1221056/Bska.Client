using Bska.Client.UI.API;
using Bska.Client.UI.ViewModels.PersonDetailsInfoViewModels;
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
    /// Interaction logic for PersonDetailsWindow.xaml
    /// </summary>
    public partial class PersonDetailsWindow : Window
    {
        private readonly IUnityContainer _container;
        public PersonDetailsWindow(IUnityContainer container)
        {
            InitializeComponent();
            this._container = container;
            List<Tuple<int, string, Color>> folders = new List<Tuple<int, string, Color>>();
            folders.Add(new Tuple<int, string, Color>(1, "اموال(Ctrl+S)", BOT.ParseHexColor("#FF1FAEFF")));
            folders.Add(new Tuple<int, string, Color>(2, "درخواست ها(Ctrl+D)", BOT.ParseHexColor("#FF1FAEFF")));

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
                var context = this.DataContext as PersonDetailsInfoViewModel;
                int selectedValue = (int)item.SelectedValue;
                switch (selectedValue)
                {
                    case 1:
                        this.personAssetPane.Visibility = Visibility.Visible;
                        this.personOrdersPane.Visibility = Visibility.Collapsed;
                        var viewModel = new PersonAssetDetailsViewModel(_container, context.PersonId);
                        this.personAssetPane.DataContext = viewModel;
                        break;
                    case 2:
                        this.personAssetPane.Visibility = Visibility.Collapsed;
                        this.personOrdersPane.Visibility = Visibility.Visible;
                        var viewModel1 = new PersonOrdersInfoViewModel(_container, context.PersonId);
                        this.personOrdersPane.DataContext = viewModel1;
                        break;
                }
            }
        }

        private void personDetailsWin_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}

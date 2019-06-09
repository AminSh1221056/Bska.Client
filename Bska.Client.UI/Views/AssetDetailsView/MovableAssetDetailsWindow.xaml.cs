using Bska.Client.Common;
using Bska.Client.Domain.Entity;
using Bska.Client.Domain.Entity.AssetEntity;
using Bska.Client.UI.API;
using Bska.Client.UI.ViewModels;
using Bska.Client.UI.ViewModels.AccessoriesViewModels;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Bska.Client.UI.Views
{
    /// <summary>
    /// Interaction logic for MovableAssetDetailsWindow.xaml
    /// </summary>
    public partial class MovableAssetDetailsWindow : Window
    {
        private readonly IUnityContainer _container;
        public MovableAssetDetailsWindow(IUnityContainer container)
        {
            InitializeComponent();
            this._container = container;

            List<Tuple<int, string, Color>> folders = new List<Tuple<int, string, Color>>();
            folders.Add(new Tuple<int, string, Color>(7, "جزییات(Ctrl+J)", BOT.ParseHexColor("#FF1FAEFF")));
            folders.Add(new Tuple<int, string, Color>(1, "گردش ها(Ctrl+S)", BOT.ParseHexColor("#FF1FAEFF")));
            folders.Add(new Tuple<int, string, Color>(2, "درخواست(Ctrl+D)", BOT.ParseHexColor("#FF1FAEFF")));
            folders.Add(new Tuple<int, string, Color>(3, "صورت جلسه(Ctrl+F)", BOT.ParseHexColor("#FF1FAEFF")));
            folders.Add(new Tuple<int, string, Color>(4, "سند(Ctrl+G)", BOT.ParseHexColor("#FF1FAEFF")));
            folders.Add(new Tuple<int, string, Color>(5, "هزینه(Ctrl+H)", BOT.ParseHexColor("#FF1FAEFF")));
            folders.Add(new Tuple<int, string, Color>(6, "بیمه(Ctrl+J)", BOT.ParseHexColor("#FF1FAEFF")));
            folders.Add(new Tuple<int, string, Color>(8, "متعلقات(Ctrl+J)", BOT.ParseHexColor("#FF1FAEFF")));

            FoldersShow.DataContext = folders;

            this.mainDetailPane.belongingPane.txtNum.IsEnabled = false;
            this.mainDetailPane.installableUC.txtNum.IsEnabled = false;
            this.mainDetailPane.unConsumptionPane.txtNum.IsEnabled = false;
            this.mainDetailPane.inCommodityPane.txtNum.IsEnabled = false;

            this.mainDetailPane.Visibility = Visibility.Collapsed;
            this.splitUc.Visibility = Visibility.Collapsed;
        }
        private void CloseCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.Close();
        }

        private void borderProperty_MouseEnter(object sender, MouseEventArgs e)
        {
            this.PopUpSelectProp.IsOpen = true;
        }

        private void PopUpSelectProp_MouseLeave(object sender, MouseEventArgs e)
        {
            this.PopUpSelectProp.IsOpen = false;
        }

        private void window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void FoldersShow_ComboColorOrganizChanged(object sender, RoutedEventArgs e)
        {
            ListBox item = e.OriginalSource as ListBox;

            if (item != null)
            {
                int selectedValue = (int)item.SelectedValue;
                var mainViewModel = this.DataContext as MovableAssetDetailsViewModel;

                switch (selectedValue)
                {
                    case 7:
                        this.mainDetailPane.Visibility = Visibility.Visible;
                        this.splitUc.Visibility = Visibility.Collapsed;
                        break;
                    case 1:
                        this.mainDetailPane.Visibility = Visibility.Collapsed;
                        this.splitUc.Visibility = Visibility.Visible;

                        var splitViewModel = new MovableAssetSplitViewModel(_container);
                        splitViewModel.CurrentMovableAsset = mainViewModel.CurrentAsset;
                        this.splitUc.DataContext = splitViewModel;
                        break;
                    default:
                        break;
                }
            }
        }
    }
}

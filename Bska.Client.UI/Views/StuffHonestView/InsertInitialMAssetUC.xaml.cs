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
    /// Interaction logic for InsertInitialMAssetUC.xaml
    /// </summary>
    public partial class InsertInitialMAssetUC : UserControl
    {
        private readonly IUnityContainer _container;
        public InsertInitialMAssetUC(IUnityContainer container)
        {
            InitializeComponent();
            this._container = container;
            List<Tuple<int, string, Color>> folders = new List<Tuple<int, string, Color>>();
            folders.Add(new Tuple<int, string, Color>(1, "خارج از انبار(Ctrl+S)", BOT.ParseHexColor("#FF1FAEFF")));
            folders.Add(new Tuple<int, string, Color>(2, "داخل انبار(Ctrl+D)", BOT.ParseHexColor("#FF1FAEFF")));

            this.initMassetView.Visibility = Visibility.Collapsed;
            FoldersShow.DataContext = folders;
        }

        private void FoldersShow_ComboColorOrganizChanged(object sender, RoutedEventArgs e)
        {
            ListBox item = e.OriginalSource as ListBox;
            if (item != null)
            {
                this.initMassetView.Visibility = Visibility.Visible;
                int selectedValue = (int)item.SelectedValue;
                switch (selectedValue)
                {
                    case 1:
                        var viewModel = new InitialMAssetViewModel(_container) { Num = 0, IsInStore = false };
                        viewModel.Window = Window.GetWindow(this);
                        this.initMassetView.DataContext = viewModel;
                        break;
                    case 2:
                        var viewModel1 = new InitialMAssetViewModel(_container) { Num = 0, IsInStore = true };
                        viewModel1.Window = Window.GetWindow(this);
                        this.initMassetView.DataContext = viewModel1;
                        break;
                }
            }
        }
    }
}

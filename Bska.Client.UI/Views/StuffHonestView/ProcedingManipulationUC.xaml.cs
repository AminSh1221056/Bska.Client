using Bska.Client.UI.API;
using Bska.Client.UI.ViewModels.StuffHonestViewModel;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Bska.Client.UI.Views.StuffHonestView
{
    /// <summary>
    /// Interaction logic for ProcedingManipulationUC.xaml
    /// </summary>
    public partial class ProcedingManipulationUC : UserControl
    {
        private readonly IUnityContainer _container;
        public ProcedingManipulationUC(IUnityContainer container)
        {
            InitializeComponent();

            this._container = container;
            List<Tuple<int, string, Color>> folders = new List<Tuple<int, string, Color>>();
            folders.Add(new Tuple<int, string, Color>(2, "ثبت صورت جلسه(Ctrl+S)", BOT.ParseHexColor("#FF1FAEFF")));
            folders.Add(new Tuple<int, string, Color>(3, "درخواست های صورت جلسه(Ctrl+D)", BOT.ParseHexColor("#FF1FAEFF")));
            folders.Add(new Tuple<int, string, Color>(1, "صورت جلسات ثبت شده(Ctrl+F)", BOT.ParseHexColor("#FF1FAEFF")));

            this.procedingPane.Visibility = Visibility.Collapsed;
            this.addProcedingPane.Visibility = Visibility.Collapsed;
            this.procOrderPanePage.Visibility = Visibility.Collapsed;

            FoldersShow.DataContext = folders;
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
                        this.procedingPane.Visibility = Visibility.Visible;
                        this.addProcedingPane.Visibility = Visibility.Collapsed;
                        this.procOrderPanePage.Visibility = Visibility.Collapsed;

                        var viewModel = new ProceedingViewModel(_container);
                        viewModel.Window = Window.GetWindow(this);
                        procedingPane.DataContext = viewModel;
                        break;
                    case 2:
                        this.procedingPane.Visibility = Visibility.Collapsed;
                        this.addProcedingPane.Visibility = Visibility.Visible;
                        this.procOrderPanePage.Visibility = Visibility.Collapsed;

                        var addProcViewModel = new AddProceedingViewModel(_container);
                        addProcedingPane.DataContext = addProcViewModel;
                        break;
                    case 3:
                        this.procedingPane.Visibility = Visibility.Collapsed;
                        this.addProcedingPane.Visibility = Visibility.Collapsed;
                        this.procOrderPanePage.Visibility = Visibility.Visible;

                        var procOrderViewModel = new ProcedingOrderViewModel(_container);
                        procOrderViewModel.Window= Window.GetWindow(this);
                        this.procOrderPanePage.DataContext = procOrderViewModel;
                        break;
                    default:
                        this.procedingPane.Visibility = Visibility.Collapsed;
                        this.addProcedingPane.Visibility = Visibility.Collapsed;
                        this.procOrderPanePage.Visibility = Visibility.Collapsed;
                        break;
                }
            }
        }
    }
}

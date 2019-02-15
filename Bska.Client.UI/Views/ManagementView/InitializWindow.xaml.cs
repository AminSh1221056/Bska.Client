using Bska.Client.Domain.Entity;
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
    /// Interaction logic for InitializWindow.xaml
    /// </summary>
    public partial class InitializWindow : Window
    {
        private readonly IUnityContainer _container;
        public InitializWindow(IUnityContainer container)
        {
            InitializeComponent();
            this._container = container;
            List<Tuple<int, string, Color>> folders = new List<Tuple<int, string, Color>>();
            folders.Add(new Tuple<int, string, Color>(1, "جمعداری(Ctrl+S)", BOT.ParseHexColor("#FF1FAEFF")));
            folders.Add(new Tuple<int, string, Color>(2, "طراحی استراتژیکی(Ctrl+D)", BOT.ParseHexColor("#FF1FAEFF")));
            folders.Add(new Tuple<int, string, Color>(3, "طراحی سازمانی(Ctrl+F)", BOT.ParseHexColor("#FF1FAEFF")));
            folders.Add(new Tuple<int, string, Color>(4, "انبار(Ctrl+G)", BOT.ParseHexColor("#FF1FAEFF")));
            folders.Add(new Tuple<int, string, Color>(5, "اموال نامشهود(Ctrl+H)", BOT.ParseHexColor("#FF1FAEFF")));
            folders.Add(new Tuple<int, string, Color>(6, "املاک(Ctrl+J)", BOT.ParseHexColor("#FF1FAEFF")));

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
                          var viewModel = new EmployeeViewModel(_container, new Employee() { });
                          this.buildingDesignPane.Visibility = Visibility.Collapsed;
                          this.buildingPane.Visibility = Visibility.Collapsed;
                          this.employeePane.Visibility = Visibility.Visible;
                          this.storePane.Visibility = Visibility.Collapsed;
                          this.stuffHonestPane.Visibility = Visibility.Collapsed;
                          this.addMeterPane.Visibility = Visibility.Collapsed;
                          this.estatePane.Visibility = Visibility.Collapsed;
                          this.employeePane.DataContext = viewModel;
                          this.employeePane.toolbarPane.gridsearchbtn.Visibility = Visibility.Collapsed;
                          this.employeePane.toolbarPane.filterbtn.Visibility = Visibility.Collapsed;
                          this.employeePane.toolbarPane.editbtn.Visibility = Visibility.Collapsed;
                          this.employeePane.toolbarPane.deletebtn.Visibility = Visibility.Collapsed;
                          this.employeePane.toolbarPane.cancelbtn.Visibility = Visibility.Collapsed;
                          this.employeePane.toolbarPane.newbtn.Visibility = Visibility.Collapsed;
                          this.employeePane.txtCode.Focus();
                        break;
                      case 2:
                          var viewModel1 = new BuildingListViewModel(_container);
                          this.buildingDesignPane.Visibility = Visibility.Collapsed;
                          this.buildingPane.Visibility = Visibility.Visible;
                          this.employeePane.Visibility = Visibility.Collapsed;
                          this.stuffHonestPane.Visibility = Visibility.Collapsed;
                          this.storePane.Visibility = Visibility.Collapsed;
                          this.addMeterPane.Visibility = Visibility.Collapsed;
                          this.estatePane.Visibility = Visibility.Collapsed;
                          this.buildingPane.DataContext = viewModel1;
                          this.buildingPane.toolbarPane.gridsearchbtn.Visibility = Visibility.Collapsed;
                          this.buildingPane.toolbarPane.filterbtn.Visibility = Visibility.Collapsed;
                          this.buildingPane.toolbarPane.editbtn.Visibility = Visibility.Collapsed;
                          this.buildingPane.toolbarPane.newbtn.Visibility = Visibility.Collapsed;
                          this.buildingPane.toolbarPane.cancelbtn.Visibility = Visibility.Collapsed;
                          this.buildingPane.importToolPane.accessbtn.Visibility = Visibility.Collapsed;
                          //this.buildingPane.treeStrategy.Items.MoveCurrentToNext();
                        break;
                      case 3:
                          var viewModel2 = new OrganizationDesignViewModel(_container);
                          this.buildingDesignPane.Visibility = Visibility.Visible;
                          this.buildingPane.Visibility = Visibility.Collapsed;
                          this.employeePane.Visibility = Visibility.Collapsed;
                          this.stuffHonestPane.Visibility = Visibility.Collapsed;
                          this.storePane.Visibility = Visibility.Collapsed;
                          this.addMeterPane.Visibility = Visibility.Collapsed;
                          this.estatePane.Visibility = Visibility.Collapsed;
                          this.buildingDesignPane.DataContext = viewModel2;
                          this.buildingDesignPane.importPane.accessbtn.Visibility = Visibility.Collapsed;
                          break;
                      case 4:
                          var viewModel3 = new StoreListViewModel(_container);
                          this.buildingDesignPane.Visibility = Visibility.Collapsed;
                          this.buildingPane.Visibility = Visibility.Collapsed;
                          this.employeePane.Visibility = Visibility.Collapsed;
                          this.stuffHonestPane.Visibility = Visibility.Collapsed;
                          this.storePane.Visibility = Visibility.Visible;
                          this.addMeterPane.Visibility = Visibility.Collapsed;
                          this.estatePane.Visibility = Visibility.Collapsed;
                          this.storePane.DataContext = viewModel3;
                          this.storePane.toolbarPane.gridsearchbtn.Visibility = Visibility.Collapsed;
                          this.storePane.toolbarPane.filterbtn.Visibility = Visibility.Collapsed;
                          this.storePane.toolbarPane.editbtn.Visibility = Visibility.Collapsed;
                          this.storePane.toolbarPane.cancelbtn.Visibility = Visibility.Collapsed;
                          this.storePane.txtStoreName.Focus();
                          break;
                      case 5:
                          this.buildingDesignPane.Visibility = Visibility.Collapsed;
                          this.buildingPane.Visibility = Visibility.Collapsed;
                          this.employeePane.Visibility = Visibility.Collapsed;
                          this.storePane.Visibility = Visibility.Collapsed;
                          this.stuffHonestPane.Visibility = Visibility.Collapsed;
                          this.estatePane.Visibility = Visibility.Collapsed;
                          this.addMeterPane.Visibility = Visibility.Visible;
                          var viewModel5 = new AddMeterForBuildingViewModel(_container);
                          this.addMeterPane.DataContext = viewModel5;

                        this.addMeterPane.toolbarPane.FilterTextBox.Focus();
                        this.addMeterPane.toolbarPane.filterbtn.Visibility = Visibility.Collapsed;
                          this.addMeterPane.toolbarPane.editbtn.Visibility = Visibility.Collapsed;
                          this.addMeterPane.toolbarPane.cancelbtn.Visibility = Visibility.Collapsed;
                        break;
                    case 6:
                        this.buildingDesignPane.Visibility = Visibility.Collapsed;
                        this.buildingPane.Visibility = Visibility.Collapsed;
                        this.employeePane.Visibility = Visibility.Collapsed;
                        this.storePane.Visibility = Visibility.Collapsed;
                        this.stuffHonestPane.Visibility = Visibility.Collapsed;
                        this.estatePane.Visibility = Visibility.Visible;
                        this.addMeterPane.Visibility = Visibility.Collapsed;
                        var viewModel6 = new EstateViewModel(_container);
                        this.estatePane.toolbarPane.FilterTextBox.Focus();
                        this.estatePane.DataContext = viewModel6;
                        this.estatePane.toolbarPane.filterbtn.Visibility = Visibility.Collapsed;
                        this.estatePane.toolbarPane.editbtn.Visibility = Visibility.Collapsed;
                        this.estatePane.toolbarPane.cancelbtn.Visibility = Visibility.Collapsed;
                        break;
                }
             }
        }

        private void employeeWin_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}

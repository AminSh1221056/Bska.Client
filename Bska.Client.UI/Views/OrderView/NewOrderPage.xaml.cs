using System;
using Microsoft.Practices.Unity;
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
using Bska.Client.UI.ViewModels.OrderViewModel;
using Bska.Client.UI.Helper;
using Bska.Client.Common;
using System.Threading;

namespace Bska.Client.UI.Views.OrderView
{
    /// <summary>
    /// Interaction logic for NewOrderPage.xaml
    /// </summary>
    public partial class NewOrderPage : Page
    {
        private readonly IUnityContainer _container;
        public NewOrderPage(IUnityContainer container)
        {
            InitializeComponent();
            Dictionary<int, string> orderType = new Dictionary<int, string>();
            if (Thread.CurrentPrincipal.IsInRole("Manager"))
            {
                orderType.Add(1, "داخلی");
                orderType.Add(2, "اعلام مازاد");
                orderType.Add(3, "صورت جلسه"); 
                orderType.Add(5, "انتقال بین واحدها");
            }
            else
            {
                if (UserLog.UniqueInstance.LogedUser.UserAttribute.InternalRequest)
                {
                    orderType.Add(1, "داخلی");
                }

                if (UserLog.UniqueInstance.LogedUser.UserAttribute.SurplusRequest)
                {
                    orderType.Add(2, "اعلام مازاد");
                }

                if (UserLog.UniqueInstance.LogedUser.UserAttribute.ProceedingRequest)
                {
                    orderType.Add(3, "صورت جلسه");
                }

                if (UserLog.UniqueInstance.LogedUser.UserAttribute.InternalMovedRequest)
                {
                    orderType.Add(5, "انتقال بین واحدها");
                }
            }

            this.orderTypeDropDown.SourceContoller.ItemsSource = orderType;
            this.internalOrderPane.orderToolPane.listbtn.Visibility = Visibility.Collapsed;
            this.internalOrderPane.cmbStuffs.Focus();
            this._container = container;
        }

        private void orderTypeDropDown_FilterButtonChanged(object sender, RoutedEventArgs e)
        {
              var btn = e.OriginalSource as Button;
              if (btn != null)
              {
                  string orderType = btn.Content.ToString();
                  orderTypeDropDown.popupToggle.Content = orderType;
                  orderTypeDropDown.Popup.IsOpen = false;
                  switch (orderType)
                  {
                      case "داخلی":
                          this.Cursor = Cursors.Wait;
                          this.disOrderPane.Visibility = Visibility.Collapsed;
                          this.internalOrderPane.Visibility = Visibility.Visible;
                          var viewModel = new InternalOrderViewModel(_container,OrderType.InternalRequest);
                          viewModel.Window = Window.GetWindow(this);
                          this.internalOrderPane.DataContext = viewModel;
                          this.internalOrderPane.buildingPersonManage.Visibility = Visibility.Visible;
                          this.internalOrderPane.storeManage.Visibility = Visibility.Collapsed;
                          this.Cursor = Cursors.Arrow;
                          break;
                      case "اعلام مازاد":
                          this.Cursor = Cursors.Wait;
                          this.internalOrderPane.Visibility = Visibility.Collapsed;
                          this.disOrderPane.Visibility = Visibility.Visible;
                          var viewModel1 = new DisplacementOrderViewModel(_container);
                          viewModel1.Window = Window.GetWindow(this);
                          this.disOrderPane.DataContext = viewModel1;
                          this.Cursor = Cursors.Arrow;
                          break;
                      case "صورت جلسه":
                          this.Cursor = Cursors.Wait;
                          this.internalOrderPane.Visibility = Visibility.Collapsed;
                          this.disOrderPane.Visibility = Visibility.Visible;
                          var viewModel2 = new DisplacementOrderViewModel(_container,Common.OrderType.Procceding);
                          viewModel2.Window = Window.GetWindow(this);
                          this.disOrderPane.DataContext = viewModel2;
                          this.Cursor = Cursors.Arrow;
                          break;
                      case "انتقال بین واحدها":
                            this.Cursor = Cursors.Wait;
                          this.internalOrderPane.Visibility = Visibility.Collapsed;
                          this.disOrderPane.Visibility = Visibility.Visible;
                          var viewModel4 = new DisplacementOrderViewModel(_container,Common.OrderType.InternalTransfer);
                          viewModel4.Window = Window.GetWindow(this);
                          this.disOrderPane.DataContext = viewModel4;
                          this.Cursor = Cursors.Arrow;
                          break;
                  }
              }
        }
    }
}

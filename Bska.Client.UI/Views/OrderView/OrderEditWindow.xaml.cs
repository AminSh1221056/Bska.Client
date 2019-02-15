using Bska.Client.Common;
using Bska.Client.UI.ViewModels.OrderViewModel;
using Bska.Client.UI.Helper;
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
using System.Windows.Shapes;
using System.Windows.Media.Animation;

namespace Bska.Client.UI.Views.OrderView
{
    /// <summary>
    /// Interaction logic for OrderEditWindow.xaml
    /// </summary>
    public partial class OrderEditWindow : Window
    {
        public OrderEditWindow()
        {
            InitializeComponent();
        }

        private void window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void CloseCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.Close();
        }

        private void window_Loaded(object sender, RoutedEventArgs e)
        {
             var context = this.DataContext as OrderEditViewModel;
             if (context != null)
             {
                context.Window = this;
                if (context.CurrentOrder.OrderType == OrderType.InternalRequest
                    || context.CurrentOrder.OrderType == OrderType.Store)
                {
                    this.internalOrderList.Visibility = Visibility.Visible;
                    this.internalOrdrUc.Visibility = Visibility.Visible;
                    if (UserLog.UniqueInstance.LogedUser.PermissionType == PermissionsType.Manager || UserLog.UniqueInstance.LogedUser.PermissionType == PermissionsType.StuffHonest
                        || UserLog.UniqueInstance.LogedUser.PermissionType == PermissionsType.GeneralManager)
                    {
                        if (context.CurrentOrder.OrderType == OrderType.Store && (context.CurrentOrder.Status == OrderStatus.ManagerConfirm || context.CurrentOrder.Status == OrderStatus.StuffHonest))
                        {
                            this.btnStoreOrderfinal.Visibility = Visibility.Visible;
                        }
                    }
                }
                else
                {
                    this.displacementOrderList.Visibility = Visibility.Visible;
                }
             }
        }
    }
}

using Bska.Client.Common;
using Bska.Client.Domain.Entity.OrderEntity;
using Bska.Client.UI.ViewModels.OrderViewModel;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Bska.Client.UI.Views.OrderView
{
    /// <summary>
    /// Interaction logic for OrderTrackWindow.xaml
    /// </summary>
    public partial class OrderTrackWindow : Window
    {
        public OrderTrackWindow()
        {
            InitializeComponent();
            this.TryFindResource("ContractingStoryboard");
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.expanderPrice.IsExpanded = true;
            var context = this.DataContext as OrderTrackViewModel;
            if (context != null)
            {
                if (context.CurrentOrder.OrderType == OrderType.InternalRequest
                    || context.CurrentOrder.OrderType==OrderType.Store)
                {
                    this.internalOrderTracking.Visibility = Visibility.Visible;
                    this.orderDetailsGridView.Visibility = Visibility.Visible;
                    this.orderDetailsGridView.SelectedIndex = -1;
                    this.initInternalOrderTrackign(context.CurrentOrder.Status);
                    if(context.CurrentOrder.Status==OrderStatus.Deliviry
                        || context.CurrentOrder.Status == OrderStatus.SubOrder)
                    {
                        this.initOrderDetailsTracking();
                    }
                }
                else
                {
                    this.displacementOrderTracking.Visibility = Visibility.Visible;
                    this.orderMAssetsGridView.Visibility = Visibility.Visible;
                    this.orderMAssetsGridView.SelectedIndex = -1;
                    this.initDisplacementOrderTracking(context.CurrentOrder.Status);
                }
                ((Storyboard)this.Resources["ExpandingStoryboard"]).Begin(this);
            }
        }

        private void initInternalOrderTrackign(OrderStatus status)
        {
            switch (status)
            {
                case OrderStatus.OrganizManagerConfirm:
                case OrderStatus.None:
                    this.internalOrderTracking.chConfirming.IsChecked = true;
                    break;
                case OrderStatus.ManagerConfirm:
                    this.internalOrderTracking.chConfirming.IsChecked = true;
                    this.internalOrderTracking.chConfirmed.IsChecked = true;
                    break;
                case OrderStatus.StuffHonest:
                    this.internalOrderTracking.chConfirming.IsChecked = true;
                    this.internalOrderTracking.chConfirmed.IsChecked = true;
                    this.internalOrderTracking.chHonestStuff.IsChecked = true;
                    break;
                case OrderStatus.SubOrder:
                    this.internalOrderTracking.chConfirming.IsChecked = true;
                    this.internalOrderTracking.chConfirmed.IsChecked = true;
                    this.internalOrderTracking.chHonestStuff.IsChecked = true;
                    break;
                case OrderStatus.Deliviry:
                    this.internalOrderTracking.chConfirming.IsChecked = true;
                    this.internalOrderTracking.chConfirmed.IsChecked = true;
                    this.internalOrderTracking.chHonestStuff.IsChecked = true;
                    this.internalOrderTracking.chDeliviry.IsChecked = true;
                    this.internalOrderTracking.chOrderBill.IsChecked = true;
                    break;
                case OrderStatus.Reject:
                    break;
            }
        }

        private async void initOrderDetailsTracking()
        {
            var context = this.DataContext as OrderTrackViewModel;
            var subOrders = await context.AllSubOrders();
            if (subOrders != null)
            {
                if (subOrders.Any(x => x.Type == SubOrderType.Store))
                {
                    this.internalOrderTracking.chStore.IsChecked = true;
                }

                if (subOrders.Any(x => x.Type == SubOrderType.Supplier))
                {
                    this.internalOrderTracking.chBuy.IsChecked = true;
                }
            }
        }

        private void initDisplacementOrderTracking(OrderStatus status)
        {
            switch (status)
            {
                case OrderStatus.OrganizManagerConfirm:
                case OrderStatus.None:
                    this.displacementOrderTracking.chConfirming.IsChecked = true;
                    break;
                case OrderStatus.ManagerConfirm:
                    this.displacementOrderTracking.chConfirming.IsChecked = true;
                    this.displacementOrderTracking.chConfirmed.IsChecked = true;
                    break;
                case OrderStatus.StuffHonest:
                case OrderStatus.Deliviry:
                    this.displacementOrderTracking.chConfirming.IsChecked = true;
                    this.displacementOrderTracking.chConfirmed.IsChecked = true;
                    this.displacementOrderTracking.chHonestStuff.IsChecked = true;
                    break;
            }
        }
    }
}

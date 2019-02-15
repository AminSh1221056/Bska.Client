using Bska.Client.Repository.Model;
using Bska.Client.UI.ViewModels;
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

namespace Bska.Client.UI.UserControlls
{
    /// <summary>
    /// Interaction logic for Timeline.xaml
    /// </summary>
    public partial class Timeline : UserControl
    {
        public Timeline()
        {
            InitializeComponent();
        }
        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            OrderModel Ovm = (sender as FrameworkElement).DataContext as OrderModel;
            var item = (this.Parent as FrameworkElement).DataContext as MainWindowViewModel;
            if (item != null)
            {
                item.Selected = Ovm;
            }
        }
    }
}

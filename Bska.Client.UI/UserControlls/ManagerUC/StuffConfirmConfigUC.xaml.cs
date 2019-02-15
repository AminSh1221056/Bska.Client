using Bska.Client.UI.ViewModels.ManagerViewModels;
using Bska.Client.UI.ViewModels.StuffHonestViewModel;
using Bska.Client.UI.ViewModels.TreeViewModels;
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

namespace Bska.Client.UI.UserControlls.ManagerUC
{
    /// <summary>
    /// Interaction logic for StuffConfirmConfigUC.xaml
    /// </summary>
    public partial class StuffConfirmConfigUC : UserControl
    {
        Boolean dragStarted = false;
        public StuffConfirmConfigUC()
        {
            InitializeComponent();
        }


        private void StuffView_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            dragStarted = true;
            base.OnPreviewMouseLeftButtonDown(e);
        }

        private void StuffView_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                if (dragStarted && sender is TreeView)
                {
                    TreeView li = sender as TreeView;
                    DataObject data = new DataObject();
                    data.SetData(li.SelectedItem);
                    System.Windows.DragDrop.DoDragDrop(li, data, DragDropEffects.Copy);
                    dragStarted = false;
                    base.OnPreviewMouseMove(e as MouseEventArgs);
                }
            }
            catch (ArgumentNullException) { }
        }

        private void StuffView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var item = e.OriginalSource as TreeView;
            var stuffTree = item.SelectedItem as KalaManageTreeViewModel;
            if (stuffTree != null)
                ((StuffConfigViewModel)this.DataContext).SelectedNode = stuffTree;
        }
    }
}

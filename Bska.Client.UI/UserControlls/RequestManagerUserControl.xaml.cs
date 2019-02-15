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
    /// Interaction logic for RequestManagerUserControl.xaml
    /// </summary>
    public partial class RequestManagerUserControl : UserControl
    {
        public RequestManagerUserControl()
        {
            InitializeComponent();
        }
        Boolean dragStarted = false;
        private void organizTree_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            dragStarted = true;
            base.OnPreviewMouseLeftButtonDown(e);
        }

        private void organizTree_PreviewMouseMove(object sender, MouseEventArgs e)
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
                    base.OnPreviewMouseMove(e);
                }
            }
            catch (ArgumentNullException) { }
        }
    }
}

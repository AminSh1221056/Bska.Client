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
    /// Interaction logic for StoreUserControl.xaml
    /// </summary>
    public partial class StoreUserControl : UserControl
    {
        public StoreUserControl()
        {
            InitializeComponent();
        }
        
        Boolean dragStarted = false;
        private void lstBuilding_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                if (dragStarted && sender is ListBox)
                {
                    ListBox li = sender as ListBox;
                    DataObject data = new DataObject();
                    data.SetData(li.SelectedItem);
                    System.Windows.DragDrop.DoDragDrop(li, data, DragDropEffects.Copy);
                    dragStarted = false;
                    base.OnPreviewMouseMove(e);
                }
            }
            catch (ArgumentNullException) { }
        }

        private void lstBuilding_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            dragStarted = true;
            base.OnPreviewMouseLeftButtonDown(e);
        }
    }
}

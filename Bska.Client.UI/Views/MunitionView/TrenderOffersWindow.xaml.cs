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

namespace Bska.Client.UI.Views.MunitionView
{
    /// <summary>
    /// Interaction logic for TrenderOffersWindow.xaml
    /// </summary>
    public partial class TrenderOffersWindow : Window
    {
        private Point origin;
        private Point start;
        public TrenderOffersWindow()
        {
            InitializeComponent();
            this.globalTollPane.cancelbtn.Visibility = Visibility.Collapsed;
            this.globalTollPane.filterbtn.Visibility = Visibility.Collapsed;
            this.globalTollPane.newbtn.Visibility = Visibility.Collapsed;
            this.globalTollPane.deletebtn.Visibility = Visibility.Collapsed;
            this.globalTollPane.editbtn.Visibility = Visibility.Collapsed;
            this.globalTollPane.gridsearchbtn.Visibility = Visibility.Collapsed;

            this.MouseWheel += SupplierProFormaUploadWindow_MouseWheel;

            imgGaranty.MouseLeftButtonDown += imgGaranty_MouseLeftButtonDown;
            imgGaranty.MouseLeftButtonUp += imgGaranty_MouseLeftButtonUp;
            imgGaranty.MouseMove += imgGaranty_MouseMove;
        }

        private void CloseCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.Close();
        }

        private void imgGaranty_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            imgGaranty.ReleaseMouseCapture();
        }

        private void imgGaranty_MouseMove(object sender, MouseEventArgs e)
        {
            if (!imgGaranty.IsMouseCaptured) return;
            Point p = e.MouseDevice.GetPosition(bo6);

            Matrix m = imgGaranty.RenderTransform.Value;
            m.OffsetX = origin.X + (p.X - start.X);
            m.OffsetY = origin.Y + (p.Y - start.Y);

            imgGaranty.RenderTransform = new MatrixTransform(m);
        }

        private void imgGaranty_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (imgGaranty.IsMouseCaptured) return;
            imgGaranty.CaptureMouse();

            start = e.GetPosition(bo6);
            origin.X = imgGaranty.RenderTransform.Value.OffsetX;
            origin.Y = imgGaranty.RenderTransform.Value.OffsetY;
        }

        private void SupplierProFormaUploadWindow_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            Point p = e.MouseDevice.GetPosition(imgGaranty);

            Matrix m = imgGaranty.RenderTransform.Value;
            if (e.Delta > 0)
                m.ScaleAtPrepend(1.1, 1.1, p.X, p.Y);
            else
                m.ScaleAtPrepend(1 / 1.1, 1 / 1.1, p.X, p.Y);

            imgGaranty.RenderTransform = new MatrixTransform(m);
        }
    }
}

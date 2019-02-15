using Bska.Client.UI.ViewModels.MunitionViewModel;
using System;
using System.Collections.Generic;
using System.IO;
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
    /// Interaction logic for SupplierProFormaUploadWindow.xaml
    /// </summary>
    public partial class SupplierProFormaUploadWindow : Window
    {
        private Point origin;
        private Point start;
        public SupplierProFormaUploadWindow()
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

        private void uploadImage_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog();
            ofd.Filter = "Image files|*.jpg;*.png;*.gif;*.ico";
            ofd.Multiselect = false;
            System.Windows.Forms.DialogResult dgr = ofd.ShowDialog();
            if (dgr == System.Windows.Forms.DialogResult.OK)
            {
                string strFileName = ofd.FileName;
                byte[] baSource = File.ReadAllBytes(strFileName);
                using (Stream streamPhoto = new MemoryStream(baSource))
                {
                    BitmapFrame bfPhoto = ReadBitmapFrame(streamPhoto);

                    int nThumbnailSize = 800, nWidth, nHeight;
                    if (bfPhoto.Width > bfPhoto.Height)
                    {
                        nWidth = nThumbnailSize;
                        nHeight = (int)(bfPhoto.Height * nThumbnailSize / bfPhoto.Width);
                    }
                    else
                    {
                        nHeight = nThumbnailSize;
                        nWidth = (int)(bfPhoto.Width * nThumbnailSize / bfPhoto.Height);
                    }
                    BitmapFrame bfResize = FastResize(bfPhoto, nWidth, nHeight);
                    byte[] baResize = ToByteArray(bfResize);
                    var context = this.DataContext as SupplierProFormaUploadViewModel;
                    if (context != null)
                    {
                        context.ProForma = baResize;
                    }
                }
            }
        }

        private  BitmapFrame FastResize(BitmapFrame bfPhoto, int nWidth, int nHeight)
        {
            TransformedBitmap tbBitmap = new TransformedBitmap(bfPhoto, new ScaleTransform(nWidth / bfPhoto.Width, nHeight / bfPhoto.Height, 0, 0));
            return BitmapFrame.Create(tbBitmap);
        }

        private byte[] ToByteArray(BitmapFrame bfResize)
        {
            using (MemoryStream msStream = new MemoryStream())
            {
                PngBitmapEncoder pbdDecoder = new PngBitmapEncoder();
                pbdDecoder.Frames.Add(bfResize);
                pbdDecoder.Save(msStream);
                return msStream.ToArray();
            }
        }

        private BitmapFrame ReadBitmapFrame(Stream streamPhoto)
        {
            BitmapDecoder bdDecoder = BitmapDecoder.Create(streamPhoto, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.None);
            return bdDecoder.Frames[0];
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

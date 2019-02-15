using Bska.Client.UI.ViewModels;
using System;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace Bska.Client.UI.Views
{
    /// <summary>
    /// Interaction logic for AddInfoWindow.xaml
    /// </summary>
    public partial class AddInfoWindow : Window
    {
        public AddInfoWindow()
        {
            InitializeComponent();
        }

        private void CloseCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.Close();
        }

        private void ButtonImageStuff_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog();
            ofd.Filter = "Image files|*.jpg;*.png;*.gif;*.ico";
            ofd.Multiselect = true;
            System.Windows.Forms.DialogResult dgr = ofd.ShowDialog();

            if (dgr == System.Windows.Forms.DialogResult.OK)
            {
                int counter = 1;
                foreach (var img in ofd.FileNames)
                {
                    if (counter > 4)
                    {
                        break;
                    }

                    FileStream fsLogo = new FileStream(img, FileMode.Open, FileAccess.Read);
                    FileInfo objFileInfoLogo = new FileInfo(img);
                    BinaryReader brLogo = new BinaryReader(fsLogo);
                    long lengthLogo = objFileInfoLogo.Length;
                    byte[] arrImgLogo = new byte[Convert.ToInt32(lengthLogo)];
                    arrImgLogo = brLogo.ReadBytes(Convert.ToInt32(lengthLogo));

                    if (arrImgLogo.Length <= 150000)
                    {
                        fsLogo.Flush();
                        brLogo.Close();
                        fsLogo.Close();
                        if (counter == 1)
                        {
                            ((AddInfoViewModel)this.DataContext).Image1 = arrImgLogo;
                        }
                        else if (counter == 2)
                        {
                            ((AddInfoViewModel)this.DataContext).Image2 = arrImgLogo;
                        }
                        else if (counter == 3)
                        {
                            ((AddInfoViewModel)this.DataContext).Image3 = arrImgLogo;
                        }
                        else if (counter == 4)
                        {
                            ((AddInfoViewModel)this.DataContext).Image4 = arrImgLogo;
                        }

                        counter++;
                    }
                    else
                    {
                        MessageBox.Show("اندازه عکس انتخابی باید برابر یا کمتر از 150 کیلو بایت باشد", "", MessageBoxButton.OK, MessageBoxImage.Error);
                    }

                }
            }
        }
        
        private void ButtonImageGuarante_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog();
            ofd.Filter = "Image files|*.jpg;*.png;*.gif;*.ico";
            ofd.Multiselect = false;
            System.Windows.Forms.DialogResult dgr = ofd.ShowDialog();
            if (dgr == System.Windows.Forms.DialogResult.OK)
            {
                FileStream fsLogo = new FileStream(ofd.FileName, FileMode.Open, FileAccess.Read);
                FileInfo objFileInfoLogo = new FileInfo(ofd.FileName);
                BinaryReader brLogo = new BinaryReader(fsLogo);
                long lengthLogo = objFileInfoLogo.Length;
                byte[] arrImgLogo = new byte[Convert.ToInt32(lengthLogo)];
                arrImgLogo = brLogo.ReadBytes(Convert.ToInt32(lengthLogo));

                if (arrImgLogo.Length <= 100000)
                {
                    fsLogo.Flush();
                    brLogo.Close();
                    fsLogo.Close();

                    ((AddInfoViewModel)this.DataContext).GuaranteeImage = arrImgLogo;
                }
                else
                {
                    MessageBox.Show("اندازه عکس انتخابی باید برابر یا کمتر از 100 کیلو بایت باشد", "", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void addInfoWindow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}

using Bska.Client.UI.ViewModels;
using System;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace Bska.Client.UI.Views
{
    /// <summary>
    /// Interaction logic for InsuranceManageWindow.xaml
    /// </summary>
    public partial class InsuranceManageWindow : Window
    {
        public InsuranceManageWindow()
        {
            InitializeComponent();
        }

        private void mAssetinsuranceWin_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void CloseCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.Close();
        }

        private void ButtonImage_Click(object sender, RoutedEventArgs e)
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

                if (arrImgLogo.Length <= 200000)
                {
                    fsLogo.Flush();
                    brLogo.Close();
                    fsLogo.Close();

                    ((InsuranceManageDetailsViewModel)((InsuranceManageListViewModel)this.DataContext).Selected).InsurancePolicyImage = arrImgLogo;
                }
                else
                {
                    MessageBox.Show("اندازه عکس انتخابی باید برابر یا کمتر از 200 کیلو بایت باشد", "", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}

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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Bska.Client.UI.UserControlls
{
    /// <summary>
    /// Interaction logic for EmployeeUserControl.xaml
    /// </summary>
    public partial class EmployeeUserControl : UserControl
    {
        public EmployeeUserControl()
        {
            InitializeComponent();
        }
        private void btnBrowseLogo_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog();
            ofd.Filter = "Image files|*.jpg;*.png;*.gif;*.ico";
            System.Windows.Forms.DialogResult dgr = ofd.ShowDialog();
            if (dgr == System.Windows.Forms.DialogResult.OK)
            {
                FileStream fsLogo = new FileStream(ofd.FileName, FileMode.Open, FileAccess.Read);
                FileInfo objFileInfoLogo = new FileInfo(ofd.FileName);
                BinaryReader brLogo = new BinaryReader(fsLogo);
                long lengthLogo = objFileInfoLogo.Length;
                byte[] arrImgLogo = new byte[Convert.ToInt32(lengthLogo)];
                arrImgLogo = brLogo.ReadBytes(Convert.ToInt32(lengthLogo));

                if (arrImgLogo.Length <= 300000)
                {
                    fsLogo.Flush();
                    brLogo.Close();
                    fsLogo.Close();

                    //((EmployeeViewModel)this.DataContext).Image = arrImgLogo;
                }
                else
                {
                    MessageBox.Show("اندازه لوگدی انتخابی باید برابر یا کمتر از 300 کیلو بایت باشد", "", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}

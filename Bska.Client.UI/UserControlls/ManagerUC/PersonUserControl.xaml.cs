using Bska.Client.UI.ViewModels;
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
    /// Interaction logic for PersonUserControl.xaml
    /// </summary>
    public partial class PersonUserControl : UserControl
    {
        public PersonUserControl()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog();
            ofd.Filter = "csv files|*.csv";
            ofd.Multiselect = true;
            System.Windows.Forms.DialogResult dgr = ofd.ShowDialog();

            if (dgr == System.Windows.Forms.DialogResult.OK)
            {
                var contex = this.DataContext as PersonListViewModel;
                if (contex != null)
                {
                    contex.FilePath = ofd.FileName;
                }
            }
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

                if (arrImgLogo.Length <= 100000)
                {
                    fsLogo.Flush();
                    brLogo.Close();
                    fsLogo.Close();

                    ((PersonListViewModel)this.DataContext).PersonDetailsVM.Photo = arrImgLogo;
                }
                else
                {
                    MessageBox.Show("اندازه عکس انتخابی باید برابر یا کمتر از 100 کیلو بایت باشد", "", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}

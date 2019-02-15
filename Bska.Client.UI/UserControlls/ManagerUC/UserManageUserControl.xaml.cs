using Bska.Client.Common;
using Bska.Client.Domain.Entity;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Bska.Client.UI.UserControlls
{
    /// <summary>
    /// Interaction logic for UserManageUserControl.xaml
    /// </summary>
    public partial class UserManageUserControl : UserControl
    {
        public UserManageUserControl()
        {
            InitializeComponent();
        }

        private void borderUserList_MouseEnter(object sender, MouseEventArgs e)
        {
            this.PopUpSelectUser.IsOpen = true;
        }

        private void PopUpSelectUser_MouseLeave(object sender, MouseEventArgs e)
        {
            this.PopUpSelectUser.IsOpen = false;
        }

        private void cmbOrderNo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var cmb = sender as ComboBox;
            var user = cmb.SelectedItem as Users;
            if (user == null) return;
            if (user.PermissionType == PermissionsType.StoreLeader)
            {
                this.stuffHonestUcManage.Visibility = Visibility.Collapsed;
                this.storeUcManage.Visibility = Visibility.Visible;
                this.generalManagerUc.Visibility = Visibility.Collapsed;
                this.accountingManageUc.Visibility = Visibility.Collapsed;
                this.munitionManagerUc.Visibility = Visibility.Collapsed;
                this.supplierManageUc.Visibility = Visibility.Collapsed;
            }
            else if (user.PermissionType == PermissionsType.StuffHonest)
            {
                this.storeUcManage.Visibility = Visibility.Collapsed;
                this.stuffHonestUcManage.Visibility = Visibility.Visible;
                this.generalManagerUc.Visibility = Visibility.Collapsed;
                this.accountingManageUc.Visibility = Visibility.Collapsed;
                this.munitionManagerUc.Visibility = Visibility.Collapsed;
                this.supplierManageUc.Visibility = Visibility.Collapsed;
            }
            else if (user.PermissionType == PermissionsType.GeneralManager)
            {
                this.stuffHonestUcManage.Visibility = Visibility.Collapsed;
                this.storeUcManage.Visibility = Visibility.Collapsed;
                this.generalManagerUc.Visibility = Visibility.Visible;
                this.accountingManageUc.Visibility = Visibility.Collapsed;
                this.munitionManagerUc.Visibility = Visibility.Collapsed;
                this.supplierManageUc.Visibility = Visibility.Collapsed;
            }
            else if (user.PermissionType == PermissionsType.Accountant)
            {
                this.stuffHonestUcManage.Visibility = Visibility.Collapsed;
                this.storeUcManage.Visibility = Visibility.Collapsed;
                this.generalManagerUc.Visibility = Visibility.Collapsed;
                this.accountingManageUc.Visibility = Visibility.Visible;
                this.munitionManagerUc.Visibility = Visibility.Collapsed;
                this.supplierManageUc.Visibility = Visibility.Collapsed;
            }
            else if (user.PermissionType == PermissionsType.MunitionLeader)
            {
                this.stuffHonestUcManage.Visibility = Visibility.Collapsed;
                this.storeUcManage.Visibility = Visibility.Collapsed;
                this.generalManagerUc.Visibility = Visibility.Collapsed;
                this.accountingManageUc.Visibility = Visibility.Collapsed;
                this.munitionManagerUc.Visibility = Visibility.Visible;
                this.supplierManageUc.Visibility = Visibility.Collapsed;
            }
            else if (user.PermissionType == PermissionsType.Supplier)
            {
                this.stuffHonestUcManage.Visibility = Visibility.Collapsed;
                this.storeUcManage.Visibility = Visibility.Collapsed;
                this.generalManagerUc.Visibility = Visibility.Collapsed;
                this.accountingManageUc.Visibility = Visibility.Collapsed;
                this.munitionManagerUc.Visibility = Visibility.Collapsed;
                this.supplierManageUc.Visibility = Visibility.Visible;
            }
            else
            {
                this.storeUcManage.Visibility = Visibility.Collapsed;
                this.stuffHonestUcManage.Visibility = Visibility.Collapsed;
                this.generalManagerUc.Visibility = Visibility.Collapsed;
                this.accountingManageUc.Visibility = Visibility.Collapsed;
                this.munitionManagerUc.Visibility = Visibility.Collapsed;
                this.supplierManageUc.Visibility = Visibility.Collapsed;
            }
        }
    }
}

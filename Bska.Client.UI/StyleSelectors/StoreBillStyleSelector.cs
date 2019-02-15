
namespace Bska.Client.UI.StyleSelectors
{
    using Bska.Client.UI.Helper;
    using Bska.Client.Common;
    using System.Windows;
    using System.Windows.Controls;
    public class StoreBillStyleSelector : StyleSelector
    {
        public int IndexDisable
        {
            get;
            set;
        }

        public bool IsPurchaseEnabled
        {
            get;
            set;
        }

        public override Style SelectStyle(object item, DependencyObject container)
        {
            Style st = new Style();
            st.TargetType = typeof(ComboBoxItem);
            Setter enablingSetter = new Setter();
            enablingSetter.Property = ComboBoxItem.IsEnabledProperty;
            ComboBox comboBox = ItemsControl.ItemsControlFromItemContainer(container) as ComboBox;
            int index = comboBox.ItemContainerGenerator.IndexFromContainer(container);

            if (IndexDisable == index)
            {
                if (!IsPurchaseEnabled)
                    enablingSetter.Value = false;
                else enablingSetter.Value = true;
            }
            else
            {
                if (UserLog.UniqueInstance.LogedUser.PermissionType == PermissionsType.StoreLeader)
                {
                    enablingSetter.Value = false;
                }
                else
                {
                    enablingSetter.Value = true;
                }
            }
            st.Setters.Add(enablingSetter);
            return st;
        }
    }
}

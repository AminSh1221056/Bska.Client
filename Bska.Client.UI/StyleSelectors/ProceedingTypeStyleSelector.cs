
using System.Windows;
using System.Windows.Controls;
using System.Linq;

namespace Bska.Client.UI.StyleSelectors
{
    public class ProceedingTypeStyleSelector : StyleSelector
    {
        public int[] IndexItems { get; set; }
        public override Style SelectStyle(object item, DependencyObject container)
        {
            Style st = new Style();
            st.TargetType = typeof(ComboBoxItem);
            Setter enablingSetter = new Setter();
            enablingSetter.Property = ComboBoxItem.IsEnabledProperty;
            ComboBox comboBox = ItemsControl.ItemsControlFromItemContainer(container) as ComboBox;
            int index = comboBox.ItemContainerGenerator.IndexFromContainer(container);
            if (IndexItems.Contains(index))
            {
                enablingSetter.Value = false;
            }
            else
            {
                enablingSetter.Value = true;
            }
            st.Setters.Add(enablingSetter);
            return st;
        }
    }
}

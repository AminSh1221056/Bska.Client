using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bska.Client.UI.Controls.CustomGridView.Support
{
    /// <summary>
    /// Corresponds to the FilterCurrentData templates (DataTemplate) 
    /// of the DataGridColumnFilter defined in the Generic.xaml>
    /// </summary>
    public enum FilterType
    {
        Numeric,
        NumericBetween,
        Text,
        List,
        Boolean,
        DateTime,
        PersianTime,
        DateTimeBetween,
        PersianTimeBetween,
    }
}

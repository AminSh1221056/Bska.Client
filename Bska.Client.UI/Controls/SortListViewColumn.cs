﻿
namespace Bska.Client.UI.Controls
{
    using System.Windows;
    using System.Windows.Controls;

    public class SortListViewColumn : GridViewColumn
    {

        public string SortProperty
        {
            get { return (string)GetValue(SortPropertyProperty); }
            set { SetValue(SortPropertyProperty, value); }
        }

        public static readonly DependencyProperty SortPropertyProperty =
            DependencyProperty.Register("SortProperty",
            typeof(string), typeof(SortListViewColumn));

        public string SortStyle
        {
            get { return (string)GetValue(SortStyleProperty); }
            set { SetValue(SortStyleProperty, value); }
        }

        public static readonly DependencyProperty SortStyleProperty =
            DependencyProperty.Register("SortStyle",
            typeof(string), typeof(SortListViewColumn));

    }
}

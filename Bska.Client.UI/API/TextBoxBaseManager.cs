
namespace Bska.Client.UI.Helper
{
    using System.Windows;
    using System.Windows.Controls.Primitives;

    public static class TextBoxBaseManager
    {
        #region DependencyProperty - AddAutoSelect (type of "bool")

        public static bool GetAddAutoSelect(DependencyObject obj)
        {
            return (bool)obj.GetValue(AddAutoSelectProperty);
        }
        public static void SetAddAutoSelect(DependencyObject obj, bool value)
        {
            obj.SetValue(AddAutoSelectProperty, value);
        }
        /// <summary>
        /// Gets or sets the auto selection to the associated TextBox. This is an attached dependency property.
        /// </summary>
        public static readonly DependencyProperty AddAutoSelectProperty =
                    DependencyProperty.RegisterAttached("AddAutoSelect", typeof(bool),
                                                                     typeof(TextBoxBaseManager),
                                                                     new UIPropertyMetadata(false,
                                                                     new PropertyChangedCallback(OnAddAutoSelectChanged)));

        private static void OnAddAutoSelectChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            EventManager.RegisterClassHandler(typeof(TextBoxBase), TextBoxBase.GotFocusEvent,
                                                          new RoutedEventHandler(TextBoxBase_GotFocus));
        }

        private static void TextBoxBase_GotFocus(object sender, RoutedEventArgs e)
        {
            (sender as TextBoxBase).SelectAll();
        }

        #endregion
    }
}

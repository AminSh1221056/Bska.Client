
namespace Bska.Client.UI.Helper
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    public static class SelectedItemBehavior
    {
        public static DependencyProperty CommandProperty =
        DependencyProperty.RegisterAttached("Command",
        typeof(ICommand),
        typeof(SelectedItemBehavior),
        new UIPropertyMetadata(CommandChanged));

        public static DependencyProperty CommandParameterProperty =
            DependencyProperty.RegisterAttached("CommandParameter",
                                                typeof(object),
                                                typeof(SelectedItemBehavior),
                                                new UIPropertyMetadata(null));

        public static void SetCommand(DependencyObject target, ICommand value)
        {
            target.SetValue(CommandProperty, value);
        }

        public static object GetCommand(DependencyObject target)
        {
            return target.GetValue(CommandProperty);
        }

        public static void SetCommandParameter(DependencyObject target, object value)
        {
            target.SetValue(CommandParameterProperty, value);
        }

        public static object GetCommandParameter(DependencyObject target)
        {
            return target.GetValue(CommandParameterProperty);
        }

        private static void CommandChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
        {
            ComboBox control = target as ComboBox;
            if (control != null)
            {
                if ((e.NewValue != null) && (e.OldValue == null))
                {
                    control.SelectionChanged += OnSelectionChanged;
                }
                else if ((e.NewValue == null) && (e.OldValue != null))
                {
                    control.SelectionChanged -= OnSelectionChanged;
                }
            }
        }

        private static void OnSelectionChanged(object sender, RoutedEventArgs e)
        {
            ComboBox control = sender as ComboBox;
            ICommand command = (ICommand)control.GetValue(CommandProperty);
            object commandParameter = control.GetValue(CommandParameterProperty);
            command.Execute(commandParameter);
        }
    }
}

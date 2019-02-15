
namespace Bska.Client.UI.Helper
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
   // using System.Windows.Interactivity;

    //public class ScrollIntoViewForTreeView : Behavior<TreeView>
    //{
    //    protected override void OnAttached()
    //    {
    //        base.OnAttached();
    //        this.AssociatedObject.SelectedItemChanged += AssociatedObject_SelectionChanged;
    //    }

    //    /// <summary>
    //    /// On Selection Changed
    //    /// </summary>
    //    /// <param name="sender"></param>
    //    /// <param name="e"></param>
    //    void AssociatedObject_SelectionChanged(object sender,
    //                                           RoutedEventArgs e)
    //    {
    //        if (sender is TreeView)
    //        {
    //            TreeView treeView = (sender as TreeView);
    //            if (treeView.SelectedItem != null)
    //            {
    //                treeView.Dispatcher.BeginInvoke(
    //                    (Action)(() =>
    //                    {
    //                        treeView.UpdateLayout();
    //                        if (treeView.SelectedItem !=
    //                        null)
    //                            treeView.ScrollToCenterOfView(
    //                            treeView.SelectedItem);
    //                    }));
    //            }
    //        }
    //    }
    //    /// <summary>
    //    /// When behavior is detached
    //    /// </summary>
    //    protected override void OnDetaching()
    //    {
    //        base.OnDetaching();
    //        this.AssociatedObject.SelectedItemChanged -=
    //            AssociatedObject_SelectionChanged;

    //    }
    //}
}

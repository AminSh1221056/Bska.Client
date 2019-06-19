using Bska.Client.UI.Helper;
using Bska.Client.UI.ViewModels;
using Bska.Client.UI.ViewModels.AccountingViewModels;
using Bska.Client.UI.Views.AccountingView;
using MahApps.Metro.Controls;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Bska.Client.UI
{
    /// <summary>
    /// Interaction logic for AccountingMainWindow.xaml
    /// </summary>
    public partial class AccountingMainWindow : MetroWindow
    {
        private readonly IUnityContainer _container;
        private readonly bool _isQuickLunch;
        private string _quickNo;

        public AccountingMainWindow(IUnityContainer container, bool isQuciLunch, string quickNo)
        {
            InitializeComponent();
            this._container = container;
            this._isQuickLunch = isQuciLunch;
            this._quickNo = quickNo;
            this.getCurrentUserAccess();
        }

        private void UIElement_OnPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //until we had a StaysOpen glag to Drawer, this will help with scroll bars
            var dependencyObject = Mouse.Captured as DependencyObject;
            while (dependencyObject != null)
            {
                if (dependencyObject is ScrollBar) return;
                dependencyObject = VisualTreeHelper.GetParent(dependencyObject);
            }

            MenuToggleButton.IsChecked = false;
        }

        private void MenuPopupButton_OnClick(object sender, RoutedEventArgs e)
        {
            //var sampleMessageDialog = new SampleMessageDialog
            //{
            //    Message = { Text = ((ButtonBase)sender).Content.ToString() }
            //};

            //await DialogHost.Show(sampleMessageDialog, "RootDialog");
        }

        private void OnCopy(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Parameter is string stringValue)
            {
                try
                {
                    Clipboard.SetDataObject(stringValue);
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex.ToString());
                }
            }
        }

        private void exitBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void DemoItemsListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            var model = this.DemoItemsListBox.SelectedItem as DemoItem;
            if (model != null)
            {
                if (model.Key.Equals("A1", StringComparison.Ordinal))
                {
                    var page = new AccountInitializationUC(_container);
                    model.Content = page;
                }
                else if (model.Key.Equals("A2", StringComparison.Ordinal))
                {
                    var page = new AccountManipulationUC(_container);
                    model.Content = page; 
                }
            }
        }

        private void getCurrentUserAccess()
        {
            if (Thread.CurrentPrincipal != null)
            {
                var lstItems = new List<DemoItem>();

                if (Thread.CurrentPrincipal.IsInRole("Manager"))
                {
                    lstItems.Add(new DemoItem("کدینگ اسناد", "A1", null, null));
                    lstItems.Add(new DemoItem("اسناد", "A2", null, null));
                }

                this.DemoItemsListBox.ItemsSource = lstItems;

                if (!string.IsNullOrWhiteSpace(_quickNo))
                {
                    var item = lstItems.FirstOrDefault(s => s.Key == _quickNo);
                    if (item != null)
                    {
                        this.DemoItemsListBox.SelectedItem = item;
                    }
                }
            }
        }

        //private async void LbxMenu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    var lsb = sender as ListBox;
        //    var index = lsb.SelectedItem as MetroMenuItem;
        //    if (index == null) return;

        //    if (APPSettings.Default.IsClosedMenu)
        //    {
        //        ((Storyboard)this.Resources["ContractingStoryboard"]).Begin(this);
        //    }
        //    this.Cursor = Cursors.Wait;
        //    Task ts = new Task(() =>
        //    {
        //        DispatchService.Invoke(() =>
        //        {
        //            Page page = null;
        //            if (index.Id == "A1")
        //            {
        //                page = new AccountCodingPage();
        //                var viewModel = new AccountCodingDesignViewModel(_container);
        //                page.DataContext = viewModel;
        //            }
        //            else if (index.Id == "A2")
        //            {
        //                page = new AccountDocumentHistoryPage();
        //                var viewModel = new AccountDocumentHistoryViewModel(_container);
        //                viewModel.Window = this;
        //                page.DataContext = viewModel;
        //            }
        //            this.Title = page.Title;
        //            this.frame.Content = page;
        //        });
        //    });
        //    ts.Start();
        //    await ts;
        //    this.Cursor = Cursors.Arrow;
        //}

        //private void munitionWindow_Loaded(object sender, RoutedEventArgs e)
        //{
        //    this.showContent();
        //    if (!APPSettings.Default.IsClosedMenu)
        //    {
        //        ((Storyboard)this.Resources["ExpandingStoryboard"]).Begin(this);
        //    }
        //}
    }
}

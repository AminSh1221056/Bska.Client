using Bska.Client.UI.Helper;
using Bska.Client.UI.ViewModels;
using Bska.Client.UI.ViewModels.GeneralManagerViewModels;
using Bska.Client.UI.Views.GeneralManagerView;
using Bska.Client.UI.Views.OrderView;
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
    /// Interaction logic for GeneralManagerWindow.xaml
    /// </summary>
    public partial class GeneralManagerWindow : MetroWindow
    {
        private readonly IUnityContainer _container;
        private readonly bool _isQuickLunch;
        private string _quickNo;

        public GeneralManagerWindow(IUnityContainer container, bool isQuciLunch, string quickNo)
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
                }
                else if (model.Key.Equals("A2", StringComparison.Ordinal))
                {
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
                    lstItems.Add(new DemoItem("درخواست ها", "A1", null, null));
                    lstItems.Add(new DemoItem("صورت جلسات", "A2", null, null));
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
        //                page = new RecivedProceedingPage();
        //                var viewModel = new RecivedProceedingViewModel(_container);
        //                viewModel.Window = this;
        //                page.DataContext = viewModel;
        //            }
        //            else if (index.Id == "A3")
        //            {
        //                page = new RecivedOrderPage();
        //                var viewModel = new InternalOrderRecivedViewModel(_container, "GeneralManager");
        //                viewModel.Window = this;
        //                page.DataContext = viewModel;
        //            }
        //            else if (index.Id == "A2")
        //            {
        //                page = new StoreOrderRecivedPage();
        //                var vieModel = new ExternalOrderRecivedViewModel(_container);
        //                vieModel.Window = this;
        //                page.DataContext = vieModel;
        //            }
        //            else if(index.Id == "A5")
        //            {
        //                page = new StoreBillEditRequestRecivedPage();
        //                var viewModel = new StoreBillEditRecivedViewModel(_container);
        //                viewModel.Window = this;
        //                page.DataContext = viewModel;
        //            }
        //            else if (index.Id == "A6")
        //            {
        //                page = new IndentReturnRequestPage();
        //                var viewModel = new RecivedIndentReturnRequestViewModel(_container);
        //                viewModel.Window = this;
        //                page.DataContext = viewModel;
        //            }
        //            else if (index.Id == "A7")
        //            {
        //                page = new RelaseStuffRecivedRequestPage();
        //                var viewModel = new RelaseAssetRequestViewModel(_container);
        //                viewModel.Window = this;
        //                page.DataContext = viewModel;
        //            }

        //            this.frame.Content = page;
        //            this.Title = page.Title;
        //        });
        //    });
        //    ts.Start();
        //    await ts;
        //    this.Cursor = Cursors.Arrow;
        //}
    }
}

using Bska.Client.UI.Helper;
using Bska.Client.UI.ViewModels;
using Bska.Client.UI.ViewModels.StuffHonestViewModel;
using Bska.Client.UI.Views.StuffHonestView;
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
    /// Interaction logic for StuffHonestMainWindow.xaml
    /// </summary>
    public partial class StuffHonestMainWindow :  MetroWindow
    {
        private readonly IUnityContainer _container;
        private readonly bool _isQuickLunch;
        private string _quickNo;
        public StuffHonestMainWindow(IUnityContainer container, bool isQuciLunch, string quickNo)
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
                if (model.Key.Equals("A9", StringComparison.Ordinal))
                {
                    var page = new UploadArrivallPage();
                    var upViewModel = new UploadAccessFileViewModel(_container);
                    page.DataContext = upViewModel;
                    model.Content = page;
                }
                else if (model.Key.Equals("A1", StringComparison.Ordinal))
                {
                    var page = new InsertInitialMAssetUC(_container);
                    model.Content = page;
                }
                else if (model.Key.Equals("A2", StringComparison.Ordinal))
                {
                    var page = new MAssetManipulationUC(_container);
                    page.FoldersShow.m_listbox.SelectedIndex = 0;
                    model.Content = page;
                }
                else if (model.Key.Equals("A3", StringComparison.Ordinal))
                {
                    var page = new OrderManipulationUC(_container);
                    page.FoldersShow.m_listbox.SelectedIndex = 0;
                    model.Content = page;
                }
                else if (model.Key.Equals("A4", StringComparison.Ordinal))
                {
                    var page = new ProcedingManipulationUC(_container);
                    page.FoldersShow.m_listbox.SelectedIndex = 0;
                    model.Content = page;
                }
            }
        }
        
        private void showUser_Click(object sender, RoutedEventArgs e)
        {
           
        }

        private void getCurrentUserAccess()
        {
            if (Thread.CurrentPrincipal != null)
            {
                var lstItems = new List<DemoItem>();

                if (Thread.CurrentPrincipal.IsInRole("Manager"))
                {
                    if (!APPSettings.Default.IsCompletedAssets)
                    {
                        lstItems.Add(new DemoItem("آپلود فایل اکسس اموال", "A9", null, null));
                        lstItems.Add(new DemoItem("ثبت موجودی اولیه", "A1", null, null));
                    }
                    lstItems.Add(new DemoItem("اموال", "A2", null, null));
                    lstItems.Add(new DemoItem("درخواست ها", "A3", null, null));
                    lstItems.Add(new DemoItem("صورت جلسات", "A4", null, null));
                }
                else if (Thread.CurrentPrincipal.IsInRole("StuffHonest"))
                {
                    if (!APPSettings.Default.IsCompletedAssets)
                    {
                        if (UserLog.UniqueInstance.LogedUser.UserAttribute.Atttribute1)
                        {
                            lstItems.Add(new DemoItem("آپلود فایل اکسس اموال", "A9", null, null));
                        }

                        if (UserLog.UniqueInstance.LogedUser.UserAttribute.Atttribute2)
                        {
                            lstItems.Add(new DemoItem("ثبت موجودی اولیه", "A1", null, null));
                        }
                    }

                    if (UserLog.UniqueInstance.LogedUser.UserAttribute.Atttribute4)
                    {
                        lstItems.Add(new DemoItem("اموال", "A2", null, null));
                    }

                    if (UserLog.UniqueInstance.LogedUser.UserAttribute.Atttribute5)
                    {
                        lstItems.Add(new DemoItem("درخواست ها", "A3", null, null));
                    }

                    if (UserLog.UniqueInstance.LogedUser.UserAttribute.Atttribute6)
                    {
                        lstItems.Add(new DemoItem("صورت جلسات", "A4", null, null));
                    }
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

        //private void showContent()
        //{
        //    if (_quickNo == 1001)
        //    {
        //        _title = "A1";
        //    }
        //    else if (_quickNo == 1002)
        //    {
        //        _title = "A2";
        //    }
        //    else if (_quickNo == 1003)
        //    {
        //        _title = "A3";
        //    }
        //    else if (_quickNo == 1004)
        //    {
        //        _title = "A4";
        //    }
        //    else if (_quickNo == 1005)
        //    {
        //        _title = "A5";
        //    }
        //    else if (_quickNo == 1006)
        //    {
        //        _title = "A6";
        //    }
        //    else if (_quickNo == 1007)
        //    {
        //        _title = "A7";
        //    }
        //    else if (_quickNo == 1008)
        //    {
        //        _title = "A8";
        //    }
        //    else if (_quickNo == 1009)
        //    {
        //        _title = "A9";
        //    }

        //    if (!string.IsNullOrEmpty(_title))
        //    {
        //        var item = this.LbxMenu.Items.OfType<MetroMenuItem>().Where(t => t.Id == _title).Single();
        //        this.LbxMenu.SelectedItem = item;
        //    }
        //}

        //private void window_Loaded(object sender, RoutedEventArgs e)
        //{
        //    this.showContent();
        //    if (!APPSettings.Default.IsClosedMenu)
        //    {
        //        ((Storyboard)this.Resources["ExpandingStoryboard"]).Begin(this);
        //    }
        //}

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
        //            if (index.Id == "A9")
        //            {
        //                page = new UploadArrivallPage();
        //                var viewModel = new UploadAccessFileViewModel(_container);
        //                viewModel.Window = this;
        //                page.DataContext = viewModel;
        //            }
        //            else if (index.Id == "A1")
        //            {
        //                page = new InitialMAsset();
        //                var viewModel = new InitialMAssetViewModel(_container) { Num = 0, IsInStore = false };
        //                viewModel.Window = this;
        //                page.DataContext = viewModel;
        //                page.Title = "موجودی خارج انبار";
        //            }
        //            else if (index.Id == "A2")
        //            {
        //                page = new InitialMAsset();
        //                var viewModel = new InitialMAssetViewModel(_container) { Num = 0, IsInStore = true };
        //                viewModel.Window = this;
        //                page.DataContext = viewModel;
        //                page.Title = "موجودی داخل انبار";
        //            }
        //            else if (index.Id == "A3")
        //            {
        //                page = new OrganMovableAssetHistoryPage();
        //                var viewModel = new OrganMovabelAssetHistoryViewModel(_container);
        //                viewModel.Window = this;
        //                page.DataContext = viewModel;
        //            }
        //            else if (index.Id == "A4")
        //            {
        //                page = new MAssetViewPage();
        //                var viewModel = new MAssetManageViewModel(_container);
        //                viewModel.Window = this;
        //                page.DataContext = viewModel;
        //            }
        //            else if (index.Id == "A5")
        //            {
        //                page = new StuffInformationPage();
        //                var viewModel = new StuffInformationViewModel(_container);
        //                page.DataContext = viewModel;
        //            }
        //            else if (index.Id == "A6")
        //            {
        //                page = new OrderManagePage();
        //                var viewModel = new OrderManageViewModel(_container);
        //                viewModel.Window = this;
        //                page.DataContext = viewModel;
        //            }
        //            else if (index.Id == "A7")
        //            {
        //                page = new ProceedingPage();
        //                var viewModel = new ProceedingViewModel(_container);
        //                viewModel.Window = this;
        //                page.DataContext = viewModel;
        //            }
        //            else if(index.Id == "A8")
        //            {
        //                page = new ExternalOrderManagePage();
        //                var viewModel = new ExternalOrderViewModel(_container);
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
    }
}

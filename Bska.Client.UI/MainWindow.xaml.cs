using Bska.Client.Common;
using Bska.Client.Data.Service;
using Bska.Client.UI.ViewModels;
using Bska.Client.UI.Views;
using Microsoft.Practices.Unity;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Security.Principal;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Linq;
using MahApps.Metro.Controls;
using System.Threading.Tasks;

namespace Bska.Client.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private readonly IUnityContainer _container;

        MetroMenuListViewModel menu = null;
        public MainWindow(IUnityContainer container)
        {
            InitializeComponent();
            this.TryFindResource("StoryboardFadeOut");
            this.TryFindResource("StoryboardFadeIn");
            this.TryFindResource("StoryboardHideWindow");
            this.TryFindResource("StoryboardShowWindow");
            this._container = container;
            this.loginMenuItem.Click+=loginMenuItem_Click;
            this.KeyDown += new KeyEventHandler(initPage_KeyDown);
            this.UserConfigPasswordItem.Visibility = Visibility.Collapsed;
            this.UserEventItem.Visibility = Visibility.Collapsed;
            this.gridMainArea.Visibility = Visibility.Collapsed;
            //this.exitAPPMenuItem.Click+=exitAPPMenuItem_Click;
        }

        void initPage_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F2)
            {
                this.LoginMenu.IsSubmenuOpen = true;
                this.loginMenuItem.Focus();
            }
            else if (e.Key == Key.F1)
            {
                this.HelpMenu.IsSubmenuOpen = true;
                this.HelpMenu.Focus();
            }
            else if (e.Key == Key.F3)
            {
                this.ConfigMenuItem.IsSubmenuOpen = true;
                this.ConfigMenuItem.Focus();
            }
            else if (e.Key == Key.F4)
            {
                this.honestMenu.IsSubmenuOpen = true;
                this.honestMenu.Focus();
            }
            else if (e.Key == Key.F5)
            {
                this.managerMenu.IsSubmenuOpen = true;
                this.managerMenu.Focus();
            }
            else if (e.Key == Key.F6)
            {
                this.accountMenu.IsSubmenuOpen = true;
                this.accountMenu.Focus();
            }
            else if (e.Key == Key.F7)
            {
                this.storeMenu.IsSubmenuOpen = true;
                this.storeMenu.Focus();
            }
            else if (e.Key == Key.F8)
            {
                this.munitionMenu.IsSubmenuOpen = true;
            }
            else if (e.Key == Key.F9)
            {
                this.serviceMenu.IsSubmenuOpen = true;
                this.serviceMenu.Focus();
            }
            else if (e.Key == Key.Escape)
            {
                App.Current.Shutdown();
            }
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.Cursor = Cursors.Wait;
            this.WindowState = WindowState.Maximized;
            DateTime now = GlobalClass._Today;
            PersianCalendar pc = new PersianCalendar();
            String day = ((PersianDayOfWeek)now.DayOfWeek).ToString();
            String Mounth = ((PersianMonth)pc.GetMonth(now)).ToString();
            this.txtDateTime.Text = "امروز" + " " + day + " " + now.PersianDateTime().Day + " " + Mounth + " " + now.PersianDateTime().Year;
            this.PopUpSelectFilter.PlacementTarget = this.orderList.btnRefresh;

            //comment on depoloy
            await Task.Run(() =>
            {
                var loginVm = new LoginWindowViewModel(_container);
                loginVm.excecuteFackeUser();
            });

            this.initUserCreditByPrinciple();

            this.Cursor = Cursors.Arrow;
        }

        private void ExitMenuItem_Click(object sender, RoutedEventArgs e)
        {
            App.Current.Shutdown();
        }

        private void loginMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ((Storyboard)this.Resources["StoryboardFadeOut"]).Begin(this);
            var window = _container.Resolve<LoginWindow>();
            var viewModel = new LoginWindowViewModel(_container);
            window.DataContext = viewModel;
            window.ShowDialog();
            ((Storyboard)this.Resources["StoryboardFadeIn"]).Begin(this);
            if (window.DialogResult == true)
            {
                 this.initUserCreditByPrinciple();
            }
        }

        private void exitAPPMenuItem_Click(object sender, RoutedEventArgs e)
        {
            this.ConfigMenuItem.Visibility = Visibility.Collapsed;
            this.honestMenu.Visibility = Visibility.Collapsed;
            this.managerMenu.Visibility = Visibility.Collapsed;
            this.storeMenu.Visibility = Visibility.Collapsed;
            this.munitionMenu.Visibility = Visibility.Collapsed;
            this.serviceMenu.Visibility = Visibility.Collapsed;
            this.accountMenu.Visibility = Visibility.Collapsed;
            //this.exitAPPMenuItem.Visibility = Visibility.Collapsed;
            this.gridMainArea.Visibility = Visibility.Collapsed;
            this.loginMenuItem.IsEnabled = true;
            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.SetPrincipalPolicy(PrincipalPolicy.UnauthenticatedPrincipal);
            ((Storyboard)this.Resources["StoryboardTimeLineOut"]).Begin(this);
        }
        
        private void btnExpand_Click(object sender, RoutedEventArgs e)
        {
            ((Storyboard)this.Resources["ExpandingStoryboard"]).Begin(this);
        }

        private void btnContract_Click(object sender, RoutedEventArgs e)
        {
            ((Storyboard)this.Resources["ContractingStoryboard"]).Begin(this);
        }

        private void image_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            Process.Start(new ProcessStartInfo("http://www.bska.ir"));
            e.Handled = true;
        }

        private void initUserCreditByPrinciple()
        {
            if (Thread.CurrentPrincipal != null)
            {
               // this.exitAPPMenuItem.Visibility = Visibility.Visible;
                
                if(Thread.CurrentPrincipal.Identity.IsAuthenticated)
                {
                    this.gridMainArea.Visibility = Visibility.Visible;
                    this.loginMenuItem.IsEnabled = false;
                    ((Storyboard)this.Resources["StoryboardTimeLineIn"]).Begin(this);

                    this.orderList.txtUserName.Text = Thread.CurrentPrincipal.Identity.Name;
                    this.UserConfigPasswordItem.Visibility = Visibility.Visible;
                    this.UserEventItem.Visibility = Visibility.Visible;
                }

                if (Thread.CurrentPrincipal.IsInRole("Manager"))
                {
                    this.ConfigMenuItem.Visibility = Visibility.Visible;
                    this.honestMenu.Visibility = Visibility.Visible;
                    this.managerMenu.Visibility = Visibility.Visible;
                    this.storeMenu.Visibility = Visibility.Visible;
                    this.munitionMenu.Visibility = Visibility.Visible;
                    this.serviceMenu.Visibility = Visibility.Visible;   
                    this.accountMenu.Visibility = Visibility.Visible;
                    
                    var empservice = _container.Resolve<IEmployeeService>();
                    var emp = empservice.Queryable().FirstOrDefault();
                    if (emp == null)
                    {
                        this.showInitWindow();
                    }
                    menu = new MetroMenuListViewModel("manager");

                    menu.MenuItems["manager"].ForEach(mu =>
                    {
                        int temp=Convert.ToInt32(mu.Tag);
                        if (temp == 1001)
                        {
                            mu.Click += OfficeInitalData_Click;
                        }
                        else if (temp == 1002)
                        {
                            mu.Click += OfficeManageMenuItem_Click;
                        }
                        else if (temp == 1003)
                        {
                            mu.Click += userMenutItem_Click;
                        }
                        else if (temp == 1004)
                        {
                            mu.Click += UpdateMenuItem_Click;
                        }
                        else if (temp == 1005)
                        {
                            mu.Click += DatabaseMenuItem_Click;
                        }
                        else if (temp == 1006)
                        {
                            mu.Click += StuffConfigMenuItem_Click;
                        }

                        this.ConfigMenuItem.Items.Add(mu);
                    });

                    menu.MenuItems["stuffHonest"].ForEach(mu =>
                    {
                        mu.Click += SystemLabelMenuItem_Click;
                        this.honestMenu.Items.Add(mu);
                    });

                    menu.MenuItems["accounting"].ForEach(mu =>
                    {
                        mu.Click += accountManageMenuItem_Click;
                        this.accountMenu.Items.Add(mu);
                    });

                    menu.MenuItems["gManager"].ForEach(mu =>
                    {
                        mu.Click += EmissionOrderMenuItem_Click;
                        this.managerMenu.Items.Add(mu);
                    });

                    menu.MenuItems["order"].ForEach(mu =>
                    {
                        mu.Click += RequestMenuItem_Click;
                        this.serviceMenu.Items.Add(mu);
                    });

                    menu.MenuItems["store"].ForEach(mu =>
                    {
                        mu.Click += ENTREPOTmAnageMenuItem_Click;
                        this.storeMenu.Items.Add(mu);
                    });

                    menu.MenuItems["munition"].ForEach(mu =>
                    {
                        mu.Click += munitionManageMenuItem_Click;
                        this.munitionMenu.Items.Add(mu);
                    });
                }
                else if (Thread.CurrentPrincipal.IsInRole("Accountant"))
                {
                    this.ConfigMenuItem.Visibility = Visibility.Collapsed;
                    this.honestMenu.Visibility = Visibility.Collapsed;
                    this.managerMenu.Visibility = Visibility.Collapsed;
                    this.accountMenu.Visibility = Visibility.Visible;
                    this.storeMenu.Visibility = Visibility.Collapsed;
                    this.munitionMenu.Visibility = Visibility.Collapsed;
                    this.serviceMenu.Visibility = Visibility.Visible;

                     menu = new MetroMenuListViewModel("accounting");

                    menu.MenuItems["accounting"].ForEach(mu =>
                    {
                        mu.Click += accountManageMenuItem_Click;
                        this.accountMenu.Items.Add(mu);
                    });

                    menu.MenuItems["order"].ForEach(mu =>
                    {
                        mu.Click += RequestMenuItem_Click;
                        this.serviceMenu.Items.Add(mu);
                    });
                }
                else if (Thread.CurrentPrincipal.IsInRole("GeneralManager"))
                {
                    this.ConfigMenuItem.Visibility = Visibility.Collapsed;
                    this.honestMenu.Visibility = Visibility.Collapsed;
                    this.managerMenu.Visibility = Visibility.Visible;
                    this.accountMenu.Visibility = Visibility.Collapsed;
                    this.storeMenu.Visibility = Visibility.Collapsed;
                    this.munitionMenu.Visibility = Visibility.Collapsed;
                    this.serviceMenu.Visibility = Visibility.Visible;

                    menu = new MetroMenuListViewModel("generalmanager");

                    menu.MenuItems["gManager"].ForEach(mu =>
                    {
                        mu.Click += EmissionOrderMenuItem_Click;
                        this.managerMenu.Items.Add(mu);
                    });

                    menu.MenuItems["order"].ForEach(mu =>
                    {
                        mu.Click += RequestMenuItem_Click;
                        this.serviceMenu.Items.Add(mu);
                    });
                }
                else if (Thread.CurrentPrincipal.IsInRole("StandardUser"))
                {
                    this.ConfigMenuItem.Visibility = Visibility.Collapsed;
                    this.honestMenu.Visibility = Visibility.Collapsed;
                    this.managerMenu.Visibility = Visibility.Collapsed;
                    this.storeMenu.Visibility = Visibility.Collapsed;
                    this.accountMenu.Visibility = Visibility.Collapsed;
                    this.munitionMenu.Visibility = Visibility.Collapsed;
                    this.serviceMenu.Visibility = Visibility.Visible;

                     menu = new MetroMenuListViewModel("order");

                    menu.MenuItems["order"].ForEach(mu =>
                    {
                        mu.Click += RequestMenuItem_Click;
                        this.serviceMenu.Items.Add(mu);
                    });
                }
                else if (Thread.CurrentPrincipal.IsInRole("StoreLeader"))
                {
                    this.ConfigMenuItem.Visibility = Visibility.Collapsed;
                    this.honestMenu.Visibility = Visibility.Collapsed;
                    this.managerMenu.Visibility = Visibility.Collapsed;
                    this.storeMenu.Visibility = Visibility.Visible;
                    this.accountMenu.Visibility = Visibility.Collapsed;
                    this.munitionMenu.Visibility = Visibility.Collapsed;
                    this.serviceMenu.Visibility = Visibility.Visible;

                    menu = new MetroMenuListViewModel("store");

                    menu.MenuItems["store"].ForEach(mu =>
                    {
                        mu.Click += ENTREPOTmAnageMenuItem_Click;
                        this.storeMenu.Items.Add(mu);
                    });

                    menu.MenuItems["order"].ForEach(mu =>
                    {
                        mu.Click += RequestMenuItem_Click;
                        this.serviceMenu.Items.Add(mu);
                    });
                }
                else if (Thread.CurrentPrincipal.IsInRole("StuffHonest"))
                {
                    this.ConfigMenuItem.Visibility = Visibility.Collapsed;
                    this.honestMenu.Visibility = Visibility.Visible;
                    this.managerMenu.Visibility = Visibility.Collapsed;
                    this.accountMenu.Visibility = Visibility.Collapsed;
                    this.storeMenu.Visibility = Visibility.Collapsed;
                    this.munitionMenu.Visibility = Visibility.Collapsed;
                    this.serviceMenu.Visibility = Visibility.Visible;

                    menu = new MetroMenuListViewModel("stuffHonest");
                    menu.MenuItems["stuffHonest"].ForEach(mu =>
                    {
                        mu.Click += SystemLabelMenuItem_Click;
                        this.honestMenu.Items.Add(mu);
                    });

                    menu.MenuItems["order"].ForEach(mu =>
                    {
                        mu.Click += RequestMenuItem_Click;
                        this.serviceMenu.Items.Add(mu);
                    });
                }
                else if (Thread.CurrentPrincipal.IsInRole("MunitionLeader"))
                {
                    this.ConfigMenuItem.Visibility = Visibility.Collapsed;
                    this.honestMenu.Visibility = Visibility.Collapsed;
                    this.managerMenu.Visibility = Visibility.Collapsed;
                    this.storeMenu.Visibility = Visibility.Collapsed;
                    this.accountMenu.Visibility = Visibility.Collapsed;
                    this.munitionMenu.Visibility = Visibility.Visible;
                    this.serviceMenu.Visibility = Visibility.Visible;

                    menu = new MetroMenuListViewModel("munition");

                    menu.MenuItems["munition"].ForEach(mu =>
                    {
                        mu.Click += munitionManageMenuItem_Click;
                        this.munitionMenu.Items.Add(mu);
                    });

                    menu.MenuItems["order"].ForEach(mu =>
                    {
                        mu.Click += RequestMenuItem_Click;
                        this.serviceMenu.Items.Add(mu);
                    });
                }
                else if (Thread.CurrentPrincipal.IsInRole("Supplier"))
                {
                    this.ConfigMenuItem.Visibility = Visibility.Collapsed;
                    this.honestMenu.Visibility = Visibility.Collapsed;
                    this.managerMenu.Visibility = Visibility.Collapsed;
                    this.storeMenu.Visibility = Visibility.Collapsed;
                    this.accountMenu.Visibility = Visibility.Collapsed;
                    this.munitionMenu.Visibility = Visibility.Visible;
                    this.serviceMenu.Visibility = Visibility.Visible;
                    menu = new MetroMenuListViewModel("supplier");

                    menu.MenuItems["supplier"].ForEach(mu =>
                    {
                        mu.Click += supplierMenuItem_Click;
                        this.munitionMenu.Items.Add(mu);
                    });

                    menu.MenuItems["order"].ForEach(mu =>
                    {
                        mu.Click += RequestMenuItem_Click;
                        this.serviceMenu.Items.Add(mu);
                    });
                }
                else
                {
                    this.ConfigMenuItem.Visibility = Visibility.Collapsed;
                    this.honestMenu.Visibility = Visibility.Collapsed;
                    this.managerMenu.Visibility = Visibility.Collapsed;
                    this.storeMenu.Visibility = Visibility.Collapsed;
                    this.munitionMenu.Visibility = Visibility.Collapsed;
                    this.serviceMenu.Visibility = Visibility.Collapsed;
                    this.accountMenu.Visibility = Visibility.Collapsed;
                }
            }
            else
            {
                this.ConfigMenuItem.Visibility = Visibility.Collapsed;
                this.honestMenu.Visibility = Visibility.Collapsed;
                this.managerMenu.Visibility = Visibility.Collapsed;
                this.storeMenu.Visibility = Visibility.Collapsed;
                this.munitionMenu.Visibility = Visibility.Collapsed;
                this.serviceMenu.Visibility = Visibility.Collapsed;
                this.accountMenu.Visibility = Visibility.Collapsed;
            }
        }

        private void OfficeInitalData_Click(object sender, RoutedEventArgs e)
        {
            this.showInitWindow();
        }

        private void showInitWindow()
        {
            ((Storyboard)this.Resources["StoryboardFadeOut"]).Begin(this);
            var window = _container.Resolve<InitializWindow>();
            window.ShowDialog();
            ((Storyboard)this.Resources["StoryboardFadeIn"]).Begin(this);
        }
        private void OfficeManageMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ((Storyboard)this.Resources["StoryboardFadeOut"]).Begin(this);
            var window = _container.Resolve<PersonManageWindow>();
            window.ShowDialog();
            ((Storyboard)this.Resources["StoryboardFadeIn"]).Begin(this);
        }
        
        private void userMenutItem_Click(object sender, RoutedEventArgs e)
        {
            ((Storyboard)this.Resources["StoryboardFadeOut"]).Begin(this);
            var window = _container.Resolve<UserWindow>();
            window.ShowDialog();
            ((Storyboard)this.Resources["StoryboardFadeIn"]).Begin(this);
        }

        private void UpdateMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ((Storyboard)this.Resources["StoryboardFadeOut"]).Begin(this);
            var window = _container.Resolve<DataUpdateWindow>();
            window.ShowDialog();
            ((Storyboard)this.Resources["StoryboardFadeIn"]).Begin(this);
        }

        private void UserConfigPasswordItem_Click(object sender, RoutedEventArgs e)
        {
            ((Storyboard)this.Resources["StoryboardFadeOut"]).Begin(this);
            var window = _container.Resolve<UserConfigWindow>();
            var viewModel = new UserConfigViewModel(_container);
            window.DataContext = viewModel;
            window.ShowDialog();
            ((Storyboard)this.Resources["StoryboardFadeIn"]).Begin(this);
        }

        private void UserEventItem_Click(object sender, RoutedEventArgs e)
        {
            ((Storyboard)this.Resources["StoryboardFadeOut"]).Begin(this);
            var window = _container.Resolve<EventLogWindow>();
            Mouse.SetCursor(Cursors.Wait);
            var viewModel = new EventLogViewModel(_container);
            window.DataContext = viewModel;
            Mouse.SetCursor(Cursors.Arrow);
            window.ShowDialog();
            ((Storyboard)this.Resources["StoryboardFadeIn"]).Begin(this);
        }

        private void SystemLabelMenuItem_Click(object sender, RoutedEventArgs e)
        {
            this.Cursor = Cursors.Wait;
            var mItem = sender as MenuItem;
            string tag =mItem.Name;

            var window = _container.Resolve<StuffHonestMainWindow>(new ParameterOverrides { { "isQuciLunch", true }, { "quickNo", tag } });
          
            window.ShowDialog();
            this.Cursor = Cursors.Arrow;
        }

        private void ENTREPOTmAnageMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var mItem = sender as MenuItem;
            int tag = Convert.ToInt32(mItem.Tag);

            var window = _container.Resolve<StoreMainWindow>(new ParameterOverrides { { "isQuciLunch", true }, { "quickNo", tag } });
            menu.Menu["store"].ForEach(mu =>
            {
                window.LbxMenu.Items.Add(mu);
            });
            window.ShowDialog();
        }

        private void RequestMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var mItem = sender as MenuItem;
            int tag = Convert.ToInt32(mItem.Tag);
            var window = _container.Resolve<OrderMainWindow>(new ParameterOverrides { { "isQuciLunch", true }, { "quickNo", tag } });
            menu.Menu["order"].ForEach(mu =>
            {
                window.LbxMenu.Items.Add(mu);
            });
            window.ShowDialog();
        }

        private void munitionManageMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var mItem = sender as MenuItem;
            int tag = Convert.ToInt32(mItem.Tag);
            var window = _container.Resolve<MunitionMainWindow>(new ParameterOverrides { { "isQuciLunch", true }, { "quickNo", tag } });
            menu.Menu["munition"].ForEach(mu =>
            {
                window.LbxMenu.Items.Add(mu);
            });
            window.ShowDialog();
        }

        private void EmissionOrderMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var mItem = sender as MenuItem;
            string tag =mItem.Name;
            var window = _container.Resolve<GeneralManagerWindow>(new ParameterOverrides { { "isQuciLunch", true }, { "quickNo", tag } });
           
            window.ShowDialog();
        }

        private void accountManageMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var mItem = sender as MenuItem;

            string tag = mItem.Name;
            var window = _container.Resolve<AccountingMainWindow>(new ParameterOverrides { { "isQuciLunch", true }, { "quickNo", tag } });
          
            window.ShowDialog();
        }

        private void btnOrgOrder_Click(object sender, RoutedEventArgs e)
        {
            var window = _container.Resolve<OrderMainWindow>(new ParameterOverrides { { "isQuciLunch", true }, { "quickNo", 1002} });
            menu.Menu["order"].ForEach(mu =>
            {
                window.LbxMenu.Items.Add(mu);
            });
            window.ShowDialog();
        }

        private void btnhonestOrder_Click(object sender, RoutedEventArgs e)
        {
            var window = _container.Resolve<StuffHonestMainWindow>(new ParameterOverrides { { "isQuciLunch", true }, { "quickNo", 1006 } });
            //menu.Menu["stuffHonest"].ForEach(mu =>
            //{
            //    window.LbxMenu.Items.Add(mu);
            //});
            window.ShowDialog();
        }

        private void btnstoreOrder_Click(object sender, RoutedEventArgs e)
        {
            var window = _container.Resolve<StoreMainWindow>(new ParameterOverrides { { "isQuciLunch", true }, { "quickNo", 1005 } });
            menu.Menu["store"].ForEach(mu =>
            {
                window.LbxMenu.Items.Add(mu);
            });
            window.ShowDialog();
        }

        private void proceedingOrder_Click(object sender, RoutedEventArgs e)
        {
            var window = _container.Resolve<StuffHonestMainWindow>(new ParameterOverrides { { "isQuciLunch", true }, { "quickNo", 1007 } });
            //menu.Menu["stuffHonest"].ForEach(mu =>
            //{
            //    window.LbxMenu.Items.Add(mu);
            //});
            window.ShowDialog();
        }
        
        private void btnmanagerOrder_Click(object sender, RoutedEventArgs e)
        {
            var window = _container.Resolve<GeneralManagerWindow>(new ParameterOverrides { { "isQuciLunch", true }, { "quickNo","A1" } });
            
            window.ShowDialog();
        }

        private void ManagerproceedingOrder_Click(object sender, RoutedEventArgs e)
        {
            var window = _container.Resolve<GeneralManagerWindow>(new ParameterOverrides { { "isQuciLunch", true }, { "quickNo", "A1" } });
          
            window.ShowDialog();
        }

        private void munitionOrder_Click(object sender, RoutedEventArgs e)
        {
            var window = _container.Resolve<MunitionMainWindow>(new ParameterOverrides { { "isQuciLunch", true }, { "quickNo", 1001 } });
            menu.Menu["munition"].ForEach(mu =>
            {
                window.LbxMenu.Items.Add(mu);
            });
            window.ShowDialog();
        }

        private void supplierOrder_Click(object sender, RoutedEventArgs e)
        {
            var window = _container.Resolve<MunitionMainWindow>(new ParameterOverrides { { "isQuciLunch", true }, { "quickNo", 1003 } });
            menu.Menu["supplier"].ForEach(mu =>
            {
                window.LbxMenu.Items.Add(mu);
            });
            window.ShowDialog();
        }

        private void supplierMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var window = _container.Resolve<MunitionMainWindow>(new ParameterOverrides { { "isQuciLunch", true }, { "quickNo", 1003 } });
            menu.Menu["supplier"].ForEach(mu =>
            {
                window.LbxMenu.Items.Add(mu);
            });
            window.ShowDialog();
        }

        private void DatabaseMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ((Storyboard)this.Resources["StoryboardFadeOut"]).Begin(this);
            var window = _container.Resolve<DatabaseManageWindow>();
            window.ShowDialog();
            ((Storyboard)this.Resources["StoryboardFadeIn"]).Begin(this);
        }

        private void StuffConfigMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ((Storyboard)this.Resources["StoryboardFadeOut"]).Begin(this);
            var window = _container.Resolve<StuffConfigWindow>();
            window.ShowDialog();
            ((Storyboard)this.Resources["StoryboardFadeIn"]).Begin(this);
        }

        private void storeBillEditOrder_Click(object sender, RoutedEventArgs e)
        {

        }

        private void buyReturnRequestOrder_Click(object sender, RoutedEventArgs e)
        {

        }

        private void storeBillFreeRequest_Click(object sender, RoutedEventArgs e)
        {

        }

        private void TrenderOfferMunitionOrder_Click(object sender, RoutedEventArgs e)
        {

        }

        private void TrenderOfferSupplier_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}

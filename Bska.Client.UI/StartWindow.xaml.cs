using Bska.Client.Common;
using Bska.Client.Data.Service;
using Bska.Client.Domain.Concrete;
using Bska.Client.UI.Services;
using Bska.Client.UI.ViewModels;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace Bska.Client.UI
{
    /// <summary>
    /// Interaction logic for StartWindow.xaml
    /// </summary>
    public partial class StartWindow : Window
    {
        private readonly IUnityContainer _container;
        private readonly IPersonService _perSonService;
        Storyboard StoryboardMain1 = null;
        public StartWindow(IUnityContainer container)
        {
            InitializeComponent();
            this._container = container;
            this._perSonService = _container.Resolve<IPersonService>();
            StoryboardMain1 = this.Resources["StoryboardMain1"] as Storyboard;
            StoryboardMain1.Completed += Storyboard_Completed;
            this.txtDescription.Text = "by bska.ir © " + GlobalClass._Today.Year + ", All rights reserved.";
            var appVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            this.txtVersion.Text = string.Format("Version {0}.{1}", appVersion.Major, appVersion.Minor);
            this.GetServerTime();
        }

        private void Storyboard_Completed(object sender, EventArgs e)
        {
            this.showMainWindow();
        }

        private async void GetServerTime()
        {
            try
            {
                var t1 = new Task(() => ConnectionHelper.GetCurrentDateTime());
                var t2 = new Task(() => _perSonService.LoginToBeska("", ""));
                t1.Start();
                t2.Start();
                await Task.WhenAll(t1, t2);
                StoryboardMain1.Begin(this);
            }
            catch (SqlException)
            {
                this.showMainWindow();
            }
            catch(InvalidOperationException ex)
            {
                if (ex.Message.Contains("model backing"))
                {
                    this.showMainWindow();
                }
                else
                {
                    throw ex;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void showMainWindow()
        {
            this.Cursor = Cursors.Wait;
            var navigation = _container.Resolve<INavigationService>();
            var viewModel = _container.Resolve<MainWindowViewModel>();
            viewModel.Window= _container.Resolve<MainWindow>();
            navigation.ShowMainWindow(viewModel);
            this.Close();
            this.Dispatcher.InvokeShutdown();
            this.Cursor = Cursors.Arrow;
        }

        //..init command line
        public bool ProcessCommandLineArgs(IList<string> commandLineArgs, bool isFirstInstance)
        {
            if (commandLineArgs == null || commandLineArgs.Count == 0)
            {
                return true;
            }

            // if no arguments and first instance
            if ((commandLineArgs.Count == 1) && isFirstInstance)
            {
                // Do nothing
            }
            // if no arguments and not first instance
            else if ((commandLineArgs.Count == 1) && !isFirstInstance)
            {
                // Do nothing
            }
            return true;
        }
    }
}

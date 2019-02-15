using Bska.Client.UI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Bska.Client.UI.Views
{
    /// <summary>
    /// Interaction logic for HelpWindow.xaml
    /// </summary>
    public partial class HelpWindow : Window
    {
        public HelpWindow()
        {
            InitializeComponent();
        }

        private void helpWin_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void helpWin_Loaded(object sender, RoutedEventArgs e)
        {
            var context = this.DataContext as HelpViewModel;
            if (context != null)
            {
                this.moonPdfPanel.OpenFile(context.FileName);
                this.moonPdfPanel.GotoPage(context.Page);
                context.initializCommands(this,this.moonPdfPanel);
                this.viewFacing.Command = context.FacingCommand;
                this.viewFacing.Header = context.FacingCommand.Name;
                this.viewBook.Command = context.BookViewCommand;
                this.viewBook.Header = context.BookViewCommand.Name;
                this.viewSingle.Command = context.SinglePageCommand;
                this.viewSingle.Header = context.SinglePageCommand.Name;
              
                this.rotateLeft.Command = context.RotateLeftCommand;
                this.rotateLeft.Header = context.RotateLeftCommand.Name;
                this.rotateRight.Command = context.RotateRightCommand;
                this.rotateRight.Header = context.RotateRightCommand.Name;
                this.zoomin.Command = context.ZoomInCommand;
                this.zoomin.Header = context.ZoomInCommand.Name;
                this.zoomout.Command = context.ZoomOutCommand;
                this.zoomout.Header = context.ZoomOutCommand.Name;
                this.itmCustomZoom.Command = context.CustomZoomCommand;
                this.itmCustomZoom.Header = context.CustomZoomCommand.Name;
                this.itmFitWidth.Command = context.FitWidthCommand;
                this.itmFitHeight.Command = context.FitHeightCommand;
                this.itmFitHeight.Header = context.FitHeightCommand.Name;
                this.itmFitWidth.Header = context.FitWidthCommand.Name;
                this.nextPageCommand.Command = context.NextPageCommand;
                this.previousPageCommand.Command = context.PreviousPageCommand;
                this.nextPageCommand.Header = context.NextPageCommand.Name;
                this.previousPageCommand.Header = context.PreviousPageCommand.Name;
                this.lastPageCommand.Command = context.LastPageCommand;
                this.firstPageCommand.Command = context.FirstPageCommand;
                this.firstPageCommand.Header = context.FirstPageCommand.Name;
                this.lastPageCommand.Header = context.LastPageCommand.Name;
            }
        }

        private void CloseCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.Close();
        }

        private void itmFullscreen_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Normal;
            }
            else
            {
                this.WindowState = WindowState.Maximized;
            }
        }
    }
}

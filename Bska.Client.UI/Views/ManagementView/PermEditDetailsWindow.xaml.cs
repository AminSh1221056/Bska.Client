using Bska.Client.Domain.Entity.AssetEntity;
using Bska.Client.UI.ViewModels;
using Bska.Client.UI.ViewModels.AccessoriesViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Bska.Client.UI.Views
{
    /// <summary>
    /// Interaction logic for PermEditDetailsWindow.xaml
    /// </summary>
    public partial class PermEditDetailsWindow : Window
    {
        public PermEditDetailsWindow()
        {
            InitializeComponent();
        }

        private void permEditindow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
        private void CloseCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.Close();
        }

        private void permEditindow_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var context = this.DataContext as PermEditDetailsViewModel;
            this.ShowRelatedUnConsumDetails(context.CurrentStuff.Parent.StuffId, context);
        }

        private void ShowRelatedUnConsumDetails(int stuffId, PermEditDetailsViewModel unViewModel)
        {
            int childNo = 1;
            BaseDetailsViewModel<MovableAsset> viewModel = null;
            if (stuffId == 23102)
            {
                childNo = 0;
                unViewModel.ElectricModel = new ElectricViewModel(unViewModel.CurrentMovableAsset);
                unViewModel.CreateListener(unViewModel.ElectricModel);
                viewModel = unViewModel.ElectricModel;
            }
            else if (stuffId == 26101 || stuffId == 262)
            {
                childNo = 8;
                unViewModel.PrintedBooksModel = new PrintedBooksViewModel(unViewModel.CurrentMovableAsset);
                viewModel = unViewModel.PrintedBooksModel;
            }
            else if (stuffId >= 25101 && stuffId <= 25110)
            {
                childNo = 2;
                unViewModel.AutomotiveModel = new AutomotiveViewModel(unViewModel.CurrentMovableAsset);
                this.AutomotivePane.borderTop.Visibility = Visibility.Collapsed;
                viewModel = unViewModel.AutomotiveModel;
            }
            else if (stuffId == 23101)
            {
                childNo = 11;
                unViewModel.VideoAudioModel = new VideoAudioViewModel(unViewModel.CurrentMovableAsset);
                unViewModel.CreateListener(unViewModel.VideoAudioModel);
                viewModel = unViewModel.VideoAudioModel;
            }
            else if (stuffId == 23104)
            {
                childNo = 6;
                unViewModel.ComputerModel = new ComputerViewModel(unViewModel.CurrentMovableAsset);
                unViewModel.CreateListener(unViewModel.ComputerModel);
                viewModel = unViewModel.ComputerModel;
            }
            else if (stuffId == 23105)
            {
                childNo = 7;
                unViewModel.HandmadeCarpetModel = new HandmadeCarpetViewModel(unViewModel.CurrentMovableAsset);
                viewModel = unViewModel.HandmadeCarpetModel;
            }
            else if (stuffId == 23108)
            {
                childNo = 5;
                unViewModel.CDModel = new CDViewModel(unViewModel.CurrentMovableAsset);
                viewModel = unViewModel.CDModel;
            }
            else if (stuffId == 23201)
            {
                childNo = 9;
                unViewModel.SportModel = new SportViewModel(unViewModel.CurrentMovableAsset);
                unViewModel.CreateListener(unViewModel.SportModel);
                viewModel = unViewModel.SportModel;
            }
            else if (stuffId == 23202)
            {
                childNo = 3;
                unViewModel.AutomativeSportsModel = new AutomativeSportsViewModel(unViewModel.CurrentMovableAsset);
                unViewModel.CreateListener(unViewModel.AutomativeSportsModel);
                viewModel = unViewModel.AutomativeSportsModel;
            }
            else if (stuffId >= 23501 && stuffId <= 23505)
            {
                childNo = 4;
                unViewModel.CameraModel = new CameraViewModel(unViewModel.CurrentMovableAsset);
                unViewModel.CreateListener(unViewModel.CameraModel);
                viewModel = unViewModel.CameraModel;
            }
            else if ((stuffId >= 24101 && stuffId <= 24135) || stuffId == 24201)
            {
                childNo = 10;
                unViewModel.ToolModel = new ToolViewModel(unViewModel.CurrentMovableAsset);
                unViewModel.CreateListener(unViewModel.ToolModel);
                viewModel = unViewModel.ToolModel;
            }
            else
            {
                childNo = 1;
                unViewModel.NonElectricModel = new NonElectricViewModel(unViewModel.CurrentMovableAsset);
                viewModel = unViewModel.NonElectricModel;
            }

            for (int i = 0; i < this.grAccessories.Children.Count; i++)
            {
                this.grAccessories.Children[i].Visibility = Visibility.Collapsed;
            }

            var childView = this.grAccessories.Children[childNo] as UserControl;
            if (childView != null)
            {
                childView.Visibility = Visibility.Visible;
                childView.DataContext = viewModel;
            }
        }
    }
}

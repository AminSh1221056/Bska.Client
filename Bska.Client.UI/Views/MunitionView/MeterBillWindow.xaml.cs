using Bska.Client.Domain.Entity.AssetEntity.Meters;
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

namespace Bska.Client.UI.Views.MunitionView
{
    /// <summary>
    /// Interaction logic for MeterBillWindow.xaml
    /// </summary>
    public partial class MeterBillWindow : Window
    {
        public MeterBillWindow()
        {
            InitializeComponent();
        }

        private void meterBillWindow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
        private void CloseCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.Close();
        }
        private void borderProperty_MouseEnter(object sender, MouseEventArgs e)
        {
            this.PopUpSelectProp.IsOpen = true;
        }

        private void PopUpSelectProp_MouseLeave(object sender, MouseEventArgs e)
        {
            this.PopUpSelectProp.IsOpen = false;
        }

        private void cmbAllMeter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var cmb = sender as ComboBox;
            var meter = cmb.SelectedItem as Meter;
            if (meter != null)
            {
                if (meter is PowerMeter)
                {
                    this.powerPnae.Visibility = Visibility.Visible;
                    this.waterPane.Visibility = Visibility.Collapsed;
                    this.tellPane.Visibility = Visibility.Collapsed;
                    this.gasPane.Visibility = Visibility.Collapsed;
                    this.mobilePane.Visibility = Visibility.Collapsed;
                }
                else if (meter is GasMeter)
                {
                    this.powerPnae.Visibility = Visibility.Collapsed;
                    this.waterPane.Visibility = Visibility.Collapsed;
                    this.tellPane.Visibility = Visibility.Collapsed;
                    this.gasPane.Visibility = Visibility.Visible;
                    this.mobilePane.Visibility = Visibility.Collapsed;
                }
                else if (meter is MobileMeter)
                {
                    this.powerPnae.Visibility = Visibility.Collapsed;
                    this.waterPane.Visibility = Visibility.Collapsed;
                    this.tellPane.Visibility = Visibility.Collapsed;
                    this.gasPane.Visibility = Visibility.Collapsed;
                    this.mobilePane.Visibility = Visibility.Visible;
                }
                else if (meter is WaterMeter)
                {
                    this.powerPnae.Visibility = Visibility.Collapsed;
                    this.waterPane.Visibility = Visibility.Visible;
                    this.tellPane.Visibility = Visibility.Collapsed;
                    this.gasPane.Visibility = Visibility.Collapsed;
                    this.mobilePane.Visibility = Visibility.Collapsed;
                }
                else if (meter is TellMeter)
                {
                    this.powerPnae.Visibility = Visibility.Collapsed;
                    this.waterPane.Visibility = Visibility.Collapsed;
                    this.tellPane.Visibility = Visibility.Visible;
                    this.gasPane.Visibility = Visibility.Collapsed;
                    this.mobilePane.Visibility = Visibility.Collapsed;
                }
            }
        }
    }
}

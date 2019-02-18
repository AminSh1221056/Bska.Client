
using Microsoft.Practices.Unity;
using System.Windows;
using System.Windows.Controls;

namespace Bska.Client.UI.Views.StuffHonestView
{
    /// <summary>
    /// Interaction logic for DocumentPage.xaml
    /// </summary>
    public partial class DocumentPage : UserControl
    {
        private readonly string _docType;
        public DocumentPage(string docType)
        {
            InitializeComponent();
            this._docType = docType;
        }
        
        private void initalizUc(string docType)
        {
            switch (docType)
            {
                case "storeBill":
                    this.storeBillPane.Visibility = Visibility.Visible;
                    this.storeDraftPane.Visibility = Visibility.Collapsed;
                    this.storeBillPane.globalToolPane.gridMainBtn.Visibility = Visibility.Collapsed;
                    this.storeBillPane.globalToolPane.gridsearchbtn.Visibility = Visibility.Collapsed;
                    this.storeBillPane.globalToolPane.FilterTextBox.Focus();
                    break;
                case "documents":
                    this.storeBillPane.Visibility = Visibility.Collapsed;
                    this.storeDraftPane.Visibility = Visibility.Visible;
                    this.storeDraftPane.globalToolPane.gridsearchbtn.Visibility = Visibility.Collapsed;
                    this.storeDraftPane.globalToolPane.gridMainBtn.Visibility = Visibility.Collapsed;
                    this.storeDraftPane.globalToolPane.FilterTextBox.Focus();
                    break;
            }
        }

        private void docpage_Loaded(object sender, RoutedEventArgs e)
        {
            this.initalizUc(_docType);
        }
    }
}

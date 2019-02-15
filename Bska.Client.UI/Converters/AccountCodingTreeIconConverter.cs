
namespace Bska.Client.UI.Converters
{
    using Domain.Entity;
    using Repository.Model;
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;
    using ViewModels.TreeViewModels;

    public class AccountCodingTreeIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var tree = value as AccountCodingTreeViewModel;
            FrameworkElement FrameElem = new FrameworkElement();

            if (tree != null)
            {
                var coding = tree.AccountDocumentCodingCurrent as AccountDocumentCoding;
                if (coding.Parent != null)
                {
                    if (coding.Parent.Parent != null)
                    {
                        return FrameElem.TryFindResource("accountTIcon");
                    }
                    return FrameElem.TryFindResource("accountMIcon");
                }
                return FrameElem.TryFindResource("accountKIcon");
            }
            return FrameElem.TryFindResource("accountGIcon");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

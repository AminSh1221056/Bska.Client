
namespace Bska.Client.UI.Converters
{
    using System.Windows.Data;
    using System;
    using System.Globalization;
    using Domain.Entity;
    using System.Linq;

    public class ExportDetailsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var exDetails = value as ExportDetails;
            if (exDetails != null)
            {
                if (exDetails.ExportDetailsMAsset != null) return exDetails.ExportDetailsMAsset.Count;
                else if (exDetails.ExportDetailsProceeding != null) return exDetails.ExportDetailsProceeding.Count;
                else return 0;
            }
            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

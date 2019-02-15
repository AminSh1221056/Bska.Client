
namespace Bska.Client.UI.Converters
{
    using System.Windows.Data;
    using Bska.Client.Common;
    using Bska.Client.Domain.Entity.AssetEntity;
    using Bska.Client.Repository.Model;
    public class MovableAssetTypeConverter : IValueConverter
    {
        public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var val = value.GetType();
            if (val != null)
            {
                if (val == typeof(MovableAssetModel)) return ((MovableAssetModel)value).MAssetType;
                if (val == typeof(UnConsumption)) return "غیر مصرفی";
                else if (val == typeof(Belonging)) return "متعلقات";
                else if (val == typeof(InCommidity)) return "در حکم مصرف";
                else if (val == typeof(Commodity)) return "مصرفی";
                else return "نامشخص";
            }
            return "نامشخص";
        }

        public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new System.NotImplementedException();
        }
    }
}

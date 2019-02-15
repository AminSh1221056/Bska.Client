
namespace Bska.Client.UI.Converters
{
    using System.Windows.Data;
    using Bska.Client.Common;
    using Bska.Client.Domain.Entity;

    public class UsersPermissionConverter : IValueConverter
    {
        public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            PermissionsType permisions = (PermissionsType)value;
            string permisionsString = "";
            switch (permisions)
            {
                case PermissionsType.Accountant:
                    permisionsString = "ذی حساب";
                    break;
                case PermissionsType.GeneralManager:
                    permisionsString = "مدیر کل";
                    break;
                case PermissionsType.Manager:
                    permisionsString = "مدیر سیستم";
                    break;
                case PermissionsType.StandardUser:
                    permisionsString = "پرسنل";
                    break;
                case PermissionsType.StoreLeader:
                    permisionsString = "مدیر انبار";
                    break;
                case PermissionsType.StuffHonest:
                    permisionsString = "امین اموال";
                    break;
                case PermissionsType.Supplier:
                    permisionsString = "کارپرداز";
                    break;
                case PermissionsType.MunitionLeader:
                    permisionsString = "مدیر تدارکات";
                    break;
                default:
                    permisionsString = "UnKnown";
                    break;
            }
            return permisionsString;
        }

        public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new System.NotImplementedException();
        }
    }

    public class UsersRoleConverter : IValueConverter
    {
        public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var item = value as Roles;
            string role = "";
            if (item != null)
            {
                if (item.OrganizId != null)
                {
                    role = "مسئول بخش سازمانی";
                }
                else
                {
                    switch (item.RoleType)
                    {
                        case PermissionsType.GeneralManager:
                            role = "مدیر کل ساختمان";
                            break;
                        case PermissionsType.StandardUser:
                            role = "مسئول بخش سازمانی";
                            break;
                        case PermissionsType.StuffHonest:
                            if (item.OrganizId == null) role = "امین اموال ساختمان";
                            else if (item.StoreId != null) role = "امین اموال انبار";
                            break;
                        case PermissionsType.MunitionLeader:
                        case PermissionsType.Supplier:
                        case PermissionsType.Accountant:
                            role = "Not specific";
                            break;
                        case PermissionsType.StoreLeader:
                            if (item.StoreId != null) role = "مسئول انبار";
                            break;
                    }
                }
            }
            return role;
        }

        public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new System.NotImplementedException();
        }
    }
}

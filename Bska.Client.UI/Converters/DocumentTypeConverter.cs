
namespace Bska.Client.UI.Converters
{
    using Bska.Client.Common;
    using System;
    using System.Windows.Data;
    public class DocumentTypeConverter : IValueConverter
    {
        public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            DocumentType docType = (DocumentType)value;
            if (docType == DocumentType.InitialBalance)
            {
                return "سند موجودی اولیه";
            }
            else if (docType == DocumentType.StoreInternalDraft)
            {
                return "حواله انبار داخلی";
            }
            else if (docType == DocumentType.ReturnToStoreTrustDraft)
            {
                return "بازگشت به انبار امانی";
            }
            else if (docType == DocumentType.ReturnToStoreDraft)
            {
                return "بازگشت به انبار";
            }
            else if (docType == DocumentType.InternalStoreTrustDraft)
            {
                return "حواله انبار امانی داخلی";
            }
            else if (docType == DocumentType.ExitStoreDraft)
            {
                return "حواله انبار خروج";
            }
            else if (docType == DocumentType.ExitStoreTrustDraft)
            {
                return "حواله انبار امانی خروج";
            }
            else if (docType == DocumentType.StoreBillRetiring)
            {
                return "قبض انبار اسقاط";
            }
            else if (docType == DocumentType.StoreBillReturnFromTrust)
            {
                return "قبض انبار بازگشت از امانی";
            }
            else
            {
                return "ناشناخته";
            }
        }

        public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new System.NotImplementedException();
        }
    }
}

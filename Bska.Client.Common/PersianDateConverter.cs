
namespace Bska.Client.Common
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    public sealed class PersianDateConvertor
    {
        PersianCalendar pc = new PersianCalendar();
        public PersianDate getpersaindate(DateTime nowdate)
        {
            PersianDate outDate = new PersianDate(nowdate);

            return outDate;
        }

        public DateTime GetUniversialDateFromPersianDate(String date)
        {
            DateTime CSharpDate = DateTime.UtcNow;
            try
            {
                if (date.Split('/').Length == 3)
                {
                    CSharpDate = pc.ToDateTime(Int32.Parse(date.Split('/')[0]), Int32.Parse(date.Split('/')[1]), Int32.Parse(date.Split('/')[2]),
                        0, 0, 0, 0);

                }
                else if (date.Split('-').Length == 3)
                {
                    CSharpDate = pc.ToDateTime(Int32.Parse(date.Split('-')[0]), Int32.Parse(date.Split('-')[1]), Int32.Parse(date.Split('-')[2]),
                        0, 0, 0, 0);

                }
                else
                {

                    throw new Exception("خطا در تاریخ ورودی");
                }
                return CSharpDate;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Day must be between 1 and 29"))
                {
                    throw new ArgumentException("برای اسفند ماه روز باید بین 1 و 29 باشد", "خطا");

                }
                else if (ex.Message.Contains("Day must be between 1 and 30"))
                {
                    throw new ArgumentException("برای ماه های پاییز و زمستان روز باید بین 1 و 30 باشد");

                }
                else
                {
                    throw new ArgumentException(ex.Message);
                }
            }
        }

        public DateTime GetUniversialDateFromPersianDate(DateTime pcdate)
        {
            DateTime CSharpDate;

            try
            {
                CSharpDate = pc.ToDateTime(pcdate.Year, pcdate.Month, pcdate.Day, 0, 0, 0, 0);
                return CSharpDate;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Day must be between 1 and 29"))
                {
                    throw new ArgumentException("برای اسفند ماه روز باید بین 1 و 29 باشد");

                }
                else if (ex.Message.Contains("Day must be between 1 and 30"))
                {
                    throw new ArgumentException("برای ماه های پاییز و زمستان روز باید بین 1 و 30 باشد");

                }
                else
                {
                    throw new ArgumentException(ex.Message);
                }
            }
        }

    }

    class PersianDateConverter : TypeConverter
    {

        // Overrides the CanConvertFrom method of TypeConverter.
        // The ITypeDescriptorContext interface provides the context for the
        // conversion. Typically, this interface is used at design time to 
        // provide information about the design-time container.
        public override bool CanConvertFrom(ITypeDescriptorContext context,
           Type sourceType)
        {

            if (sourceType == typeof(string))
            {
                return true;
            }
            if (sourceType == typeof(DateTime))
            {
                return true;
            }
            return base.CanConvertFrom(context, sourceType);
        }
        // Overrides the ConvertFrom method of TypeConverter.
        public override object ConvertFrom(ITypeDescriptorContext context,
           CultureInfo culture, object value)
        {
            if (value is string)
            {
                return PersianDate.Parse(value as string);
            }
            if (value is DateTime)
            {
                return new PersianDate((DateTime)value);
            }
            return base.ConvertFrom(context, culture, value);
        }
        // Overrides the ConvertTo method of TypeConverter.
        public override object ConvertTo(ITypeDescriptorContext context,
           CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                PersianDate pd = (PersianDate)value;
                return value.ToString();
            }
            if (destinationType == typeof(DateTime))
            {
                PersianDate pd = (PersianDate)value;
                return pd.ToDateTime();
            }
            return base.ConvertTo(context, culture, value, destinationType);

        }
    }
}

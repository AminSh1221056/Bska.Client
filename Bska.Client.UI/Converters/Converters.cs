
namespace Bska.Client.UI.Converters
{
    using System;
    using System.Windows.Data;
    using System.Windows.Media;
    using System.Windows;
    using System.Diagnostics;
    using System.Globalization;
    using Bska.Client.Common;
    using Bska.Client.UI.Helper;
    using System.IO;
    using System.Windows.Media.Imaging;
    using Bska.Client.Domain.Entity;
    using System.Linq;
    using System.Windows.Markup;
    using System.Collections.Generic;

    public class BrushToAlphaBrushConverter : IValueConverter
    {
        #region Properties

        private byte Alpha
        {
            get
            {
                return (byte)(255.0 * this.Opacity);
            }
        }

        public double Opacity { get; set; }

        #endregion Properties

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            SolidColorBrush brush = value as SolidColorBrush;
            if (value != null)
            {
                SolidColorBrush alphaBrush = new SolidColorBrush(Color.FromArgb(this.Alpha, brush.Color.R, brush.Color.G, brush.Color.B));
                return alphaBrush;
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            // not implemented
            return null;
        }

        #endregion
    }

    public class BoolToDoubleConverter : IValueConverter
    {
        #region Fields

        private double _FalseValue = 0.5;
        private double _TrueValue = 1.0;

        #endregion Fields

        #region Properties

        public double FalseValue
        {
            get { return _FalseValue; }
            set { _FalseValue = value; }
        }

        public double TrueValue
        {
            get { return _TrueValue; }
            set { _TrueValue = value; }
        }

        #endregion Properties

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is bool)
            {
                if ((bool)value) return TrueValue; else return FalseValue;
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            // not implemented
            return null;
        }

        #endregion
    }

    public class ReverseBooleanToVisibilityConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is bool)
            {
                if ((bool)value)
                {
                    return Visibility.Collapsed;
                }
                else
                {
                    return Visibility.Visible;
                }
            }
            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }

        #endregion
    }

    public class MounthToStringConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            PersianCalendar pc = new PersianCalendar();
            DateTime? val = value as DateTime?;
            if (val != null)
            {
                return ((PersianMonth)pc.GetMonth(val.Value)).ToString() + " " + pc.GetYear(val.Value);
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }

        #endregion
    }

    public class DateTimeToStringConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string format = parameter.ToString();
            PersianCalendar pc = new PersianCalendar();
            DateTime? daytime = value as DateTime?;
            if (daytime != null)
            {
                // there is no support for a single letter representation of the day of the 
                // week so we invent our own
                if (format.EasyEquals("w"))
                {  //no problem for win8 and win7 but win xp no suported below code for culture specification
                    // return ((DateTime)value).ToString("ddd").Substring(0, 1);
                    if (daytime.Value.DayOfWeek == DayOfWeek.Friday) return "ج";
                    else if (daytime.Value.DayOfWeek == DayOfWeek.Monday) return "د";
                    else if (daytime.Value.DayOfWeek == DayOfWeek.Saturday) return "ش";
                    else if (daytime.Value.DayOfWeek == DayOfWeek.Sunday) return "ی";
                    else if (daytime.Value.DayOfWeek == DayOfWeek.Thursday) return "پ";
                    else if (daytime.Value.DayOfWeek == DayOfWeek.Tuesday) return "س";
                    else if (daytime.Value.DayOfWeek == DayOfWeek.Wednesday) return "چ";
                }
                else
                {
                    return pc.GetDayOfMonth((DateTime)value).ToString(format);
                }
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }

        #endregion
    }

    public class DebugConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Debug.WriteLine(string.Format("TargetType: {0} / Value: {1}", targetType, value));
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }

        #endregion
    }

    public class ColorToBrushConverterWithFallback : IValueConverter
    {
        #region Properties

        public Brush FallbackBrush { get; set; }

        #endregion Properties

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is Color && ((Color)value).A > 0)
            {
                return new SolidColorBrush((Color)value);
            }
            return FallbackBrush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }

        #endregion
    }

    public class ColorToColorConverterWithFallback : IValueConverter
    {
        #region Properties

        public Color FallbackColor { get; set; }

        #endregion Properties

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is Color && ((Color)value).A > 0)
            {
                return (Color)value;
            }
            return FallbackColor;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }

        #endregion
    }

    public class FallbackWhenNullConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string fallback = parameter.ToString();

            if (string.IsNullOrEmpty(value.ToString()))
            {
                return fallback;
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    public class ImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var byteArrayImage = value as byte[];

            if (byteArrayImage != null && byteArrayImage.Length > 0)
            {
                var ms = new MemoryStream(byteArrayImage);

                var bitmapImg = new BitmapImage();

                bitmapImg.BeginInit();
                bitmapImg.StreamSource = ms;
                bitmapImg.EndInit();

                return bitmapImg;
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class HierarchyAccountCodingConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var node = value as AccountDocumentCoding;
            return node.Childeren.Where(i => i.Parent == node).ToList();
        }


        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class HierarchyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var node = value as EmployeeDesign;
            return node.ChildNode.Where(i => i.ParentNode == node).ToList();
        }


        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Converts a Boolean value into a Visibility enumeration (and back)
    /// </summary>
    [ValueConversion(typeof(bool), typeof(Visibility))]
    [MarkupExtensionReturnType(typeof(BoolToVisibilityConverter))]
    public class BoolToVisibilityConverter : MarkupExtension, IValueConverter
    {
        /// <summary>
        /// FalseEquivalent (default : Visibility.Collapsed => see Constructor)
        /// </summary>
        public Visibility FalseEquivalent { get; set; }
        /// <summary>
        /// Define whether the opposite boolean value is crucial (default : false)
        /// </summary>
        public bool OppositeBooleanValue { get; set; }

        /// <summary>
        /// Initialize the properties with standard values
        /// </summary>
        public BoolToVisibilityConverter()
        {
            this.FalseEquivalent = Visibility.Collapsed;
            this.OppositeBooleanValue = false;
        }

        //+------------------------------------------------------------------------
        //+ Usage :
        //+ -------
        //+ 1. <conv:BoolToVisibilityConverter x:Key="BoolToVisibilityConverter" />
        //+ 2. {Binding ... Converter={StaticResource BoolToVisibilityConverter}
        //+------------------------------------------------------------------------
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool && targetType == typeof(Visibility))
            {
                bool? booleanValue = (bool?)value;

                if (this.OppositeBooleanValue == true)
                {
                    booleanValue = !booleanValue;
                }

                return booleanValue.GetValueOrDefault() ? Visibility.Visible : FalseEquivalent;
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Visibility)
            {
                Visibility visibilityValue = (Visibility)value;

                if (this.OppositeBooleanValue == true)
                {
                    visibilityValue = visibilityValue == Visibility.Visible ? FalseEquivalent : Visibility.Visible;
                }

                return (visibilityValue == Visibility.Visible);
            }

            return value;
        }

        #endregion // IValueConverter Members

        //+-----------------------------------------------------------------------------------
        //+ Usage :	(wpfsl: => XML Namespace mapping to the "BoolToVisibilityConverter" class)
        //+ -------
        //+ Use as follows : {Binding ... Converter={wpfsl:BoolToVisibilityConverter}
        //+ NO StaticResource required
        //+-----------------------------------------------------------------------------------
        #region MarkupExtension "overrides"

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return new BoolToVisibilityConverter
            {
                FalseEquivalent = this.FalseEquivalent,
                OppositeBooleanValue = this.OppositeBooleanValue
            };
        }

        #endregion
    }

    [ValueConversion(typeof(string), typeof(Visibility))]
    [MarkupExtensionReturnType(typeof(StringToVisibilityConverter))]
    public class StringToVisibilityConverter : MarkupExtension, IValueConverter
    {
        /// <summary>
        /// FalseEquivalent (default : Visibility.Collapsed => see Constructor)
        /// </summary>
        public Visibility FalseEquivalent { get; set; }

        /// <summary>
        /// Define whether the opposite boolean value is crucial (default : false)
        /// </summary>
        public bool OppositeStringValue { get; set; }

        /// <summary>
        /// Initialize the properties with standard values
        /// </summary>
        public StringToVisibilityConverter()
        {
            this.FalseEquivalent = Visibility.Collapsed;
            this.OppositeStringValue = false;
        }

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string && targetType == typeof(Visibility))
            {
                if (this.OppositeStringValue == true)
                {
                    return ((value as string).ToLower().Equals(String.Empty)) ? Visibility.Visible : this.FalseEquivalent;
                }

                return ((value as string).ToLower().Equals(String.Empty)) ? this.FalseEquivalent : Visibility.Visible;
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Visibility)
            {
                if (this.OppositeStringValue == true)
                {
                    return ((Visibility)value == Visibility.Visible) ? String.Empty : "visible";
                }

                return ((Visibility)value == Visibility.Visible) ? "visible" : String.Empty;
            }

            return value;
        }

        #endregion

        #region MarkupExtension "overrides"

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return new StringToVisibilityConverter
            {
                FalseEquivalent = this.FalseEquivalent,
                OppositeStringValue = this.OppositeStringValue
            };
        }

        #endregion
    }

    public class ObjectToTypeStringConverter : IValueConverter
    {
        public object Convert(
         object value, Type targetType,
         object parameter, CultureInfo culture)
        {
            return value.GetType().Name;
        }

        public object ConvertBack(
         object value, Type targetType,
         object parameter, CultureInfo culture)
        {
            // I don't think you'll need this
            throw new Exception("Can't convert back");
        }
    }

    public class QualityToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string returnVal = "ناشناخته";
            if (value != null)
            {
                string val = value.ToString();
                switch (val)
                {
                    case "A":
                        returnVal = "نو و آک";
                        break;
                    case "B":
                        returnVal = "نو";
                        break;
                    case "C":
                        returnVal = "معمولی";
                        break;
                    case "D":
                        returnVal = "مستعمل";
                        break;
                    case "E":
                        returnVal = "فرسوده";
                        break;
                }
            }
            return returnVal;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class PersonContractConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string returnVal = "ناشناخته";
            if (value != null)
            {
                ContractType contract = (ContractType)value;
                if (contract == ContractType.ToEmploye)
                {
                    returnVal = "استخدام";
                }
                else if (contract == ContractType.Contractual)
                {
                    returnVal = "قراردادی";
                }
            }
            return returnVal;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class CommandValuesConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            List<object> val = new List<object>();
            foreach (var item in values) val.Add(item);
            return val;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ReverseBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Boolean val = (Boolean)value;
            return !val;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

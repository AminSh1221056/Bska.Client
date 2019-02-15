
namespace Bska.Client.UI.ViewModels
{
    using Bska.Client.Common;
    using Bska.Client.Repository.Model;
    using PixelLab.Common;
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;
    
    public class TimelineViewModel : INotifyPropertyChanged
    {
        #region Fields
        
        private DateTime nowDate = GlobalClass._Today;
        private DateTime m_beginDate = GlobalClass._Today.AddMonths(-1);
        private DateTime m_endDate = GlobalClass._Today.AddMonths(1);
        private PersianCalendar pc = new PersianCalendar();

        private readonly ObservableCollectionPlus<DateTime> m_dates = new ObservableCollectionPlus<DateTime>();
        private readonly ObservableCollectionPlus<DateTime> m_months = new ObservableCollectionPlus<DateTime>();
        private readonly ObservableCollection<OrderModel> _orderCollection = new ObservableCollection<OrderModel>();

        #endregion Fields

        #region Constructors

        public TimelineViewModel()
        {
            refreshDates();
        }

        #endregion Constructors

        #region Properties

        public ObservableCollection<OrderModel> OrderCollection
        {
            get { return _orderCollection; }
        }

        public DateTime BeginDate
        {
            get { return m_beginDate; }
            private set
            {
                m_beginDate = value;
                RaisePropertyChanged("BeginDate");
                RaisePropertyChanged("PanelWidth");
            }
        }


        public ReadOnlyObservableCollection<DateTime> Dates
        {
            get { return m_dates.ReadOnly; }
        }

        public ReadOnlyObservableCollection<DateTime> Months
        {
            get { return m_months.ReadOnly; }
        }

        public static double TimeSliceHeight { get { return 2.25; } }
        public static double TimeSliceMinutes { get { return 15.0; } }
        public static double DayWidth { get { return 26; } }
        public static double ColumnWidth { get { return 16; } }

        public static double TimelineHeight { get { return TimeSliceHeight * TimeSliceCount; } }
        public static double TimeSliceCount { get { return (60.0 / TimeSliceMinutes) * 12; } } // assume each day has 12 hours -- we could expose config for this later
        public static GridLength TimelineHeightGridLength { get { return new GridLength(TimelineHeight); } }

        public DateTime EndDate
        {
            get
            {
                return m_endDate;
            }
            private set
            {
                m_endDate = value;
                RaisePropertyChanged("EndDate");
                RaisePropertyChanged("PanelWidth");
            }
        }

        public double PanelWidth
        {
            get { return DayWidth * (this.EndDate - this.BeginDate).Days; }
        }

        private static EstimateToHeightConverter _EstimateToHeightConverter;
        public static EstimateToHeightConverter EstimateToHeightConverter
        {
            get
            {
                if (_EstimateToHeightConverter == null) _EstimateToHeightConverter = new EstimateToHeightConverter();
                return _EstimateToHeightConverter;
            }
        }

        #endregion Properties

        #region Private Methods

        private void refreshDates()
        {
            // update the BegindDate / EndDate
            DateTime begin = this.BeginDate;

            begin = nowDate.AddMonths(-5);


            DateTime end = this.EndDate;

            end = nowDate.AddMonths(5);




            // adjust the dates (we want to get the range of dates for all of the visible data but always show at least 1 month 
            // in the past and 1 month the future. we won't show events more than 1 year in the past or 1 year in the future

            begin = DateTimeExtensions.Min(begin, nowDate.AddMonths(-2));
            begin = DateTimeExtensions.Max(begin, nowDate.AddYears(-1));

            end = DateTimeExtensions.Max(end, nowDate.AddMonths(2));
            end = DateTimeExtensions.Min(end, nowDate.AddYears(1));


            // set the props
            this.BeginDate = begin;
            this.EndDate = end;

            // calculate the width of the panel

            // generate a collection of dates that we can use to populate
            // the labels on our timeline

            m_dates.Clear();
            m_months.Clear();


            DateTime date = begin;
            m_months.Add(date);

            while (date <= end)
            {
                m_dates.Add(date);
                date = date.AddDays(1);
                if (date.Day == 1) m_months.Add(date);
            }

        }

        #endregion Private Methods

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;
        protected void RaisePropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        #endregion

    }

    public class EstimateToHeightConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is TimeSpan)
            {
                TimeSpan t = (TimeSpan)value;

                double m = (double)t.TotalMinutes;
                m = Math.Max(m, 30);
                m = Math.Min(m, 12 * 60);

                return (m / TimelineViewModel.TimeSliceMinutes) * TimelineViewModel.TimeSliceHeight;
            }

            // we go with 1 hour if there isn't a timespan
            return (60 / TimelineViewModel.TimeSliceMinutes) * TimelineViewModel.TimeSliceHeight;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }

        #endregion
    }
}

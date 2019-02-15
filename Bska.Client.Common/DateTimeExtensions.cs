
namespace Bska.Client.Common
{
    using Bska.Client.Common;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public static class DateTimeExtensions
    {
        /// <summary>
        /// Gets a DateTime representing the first day in the current month
        /// </summary>
        /// <param name="current">The current date</param>
        /// <returns></returns>
        public static DateTime First(this DateTime current)
        {
            DateTime first = current.AddDays(1 - current.Day);
            return first;
        }

        /// <summary>
        /// Gets a DateTime representing the first specified day in the current month
        /// </summary>
        /// <param name="current">The current day</param>
        /// <param name="dayOfWeek">The current day of week</param>
        /// <returns></returns>
        public static DateTime First(this DateTime current, DayOfWeek dayOfWeek)
        {
            DateTime first = current.First();

            if (first.DayOfWeek != dayOfWeek)
            {
                first = first.Next(dayOfWeek);
            }

            return first;
        }

        /// <summary>
        /// Gets a DateTime representing the last day in the current month
        /// </summary>
        /// <param name="current">The current date</param>
        /// <returns></returns>
        public static DateTime Last(this DateTime current)
        {
            int daysInMonth = DateTime.DaysInMonth(current.Year, current.Month);

            DateTime last = current.First().AddDays(daysInMonth - 1);
            return last;
        }

        /// <summary>
        /// Gets a DateTime representing the last specified day in the current month
        /// </summary>
        /// <param name="current">The current date</param>
        /// <param name="dayOfWeek">The current day of week</param>
        /// <returns></returns>
        public static DateTime Last(this DateTime current, DayOfWeek dayOfWeek)
        {
            DateTime last = current.Last();

            last = last.AddDays(Math.Abs(dayOfWeek - last.DayOfWeek) * -1);
            return last;
        }

        /// <summary>
        /// Gets a DateTime representing the first date following the current date which falls on the given day of the week
        /// </summary>
        /// <param name="current">The current date</param>
        /// <param name="dayOfWeek">The day of week for the next date to get</param>
        public static DateTime Next(this DateTime current, DayOfWeek dayOfWeek)
        {
            int offsetDays = dayOfWeek - current.DayOfWeek;

            if (offsetDays <= 0)
            {
                offsetDays += 7;
            }

            DateTime result = current.AddDays(offsetDays);
            return result;
        }

        public static DateTime Max(DateTime d1, DateTime d2)
        {
            if (d1 > d2) return d1; else return d2;
        }

        public static DateTime Min(DateTime d1, DateTime d2)
        {
            if (d1 < d2) return d1; else return d2;
        }

        public static String PersianDateString(this DateTime Date)
        {
            PersianDateConvertor pcConv = new PersianDateConvertor();

            return pcConv.getpersaindate(Date).ToString();
        }

        public static DateTime ShamsyDateTime(this DateTime Date)
        {
            PersianDateConvertor pcConv = new PersianDateConvertor();

            return DateTime.Parse(pcConv.getpersaindate(Date).ToString());
        }

        public static PersianDate PersianDateTime(this DateTime Date)
        {
            PersianDateConvertor pcConv = new PersianDateConvertor();
            return pcConv.getpersaindate(Date);
        }

        public static DateTime UniversialDate(this DateTime PersianDate)
        {
            PersianDateConvertor pcConv = new PersianDateConvertor();
            return pcConv.GetUniversialDateFromPersianDate(PersianDate);
        }
    }
}

using System;

namespace DAL.Extensions
{
    public static class DateTimeExtension
    {
        public static bool IsDateOnly(this DateTime dateTime)
        {
            return dateTime.Equals(dateTime.Date);
        }       
        
        public static bool IsInRange(this DateTime dateTime, DateTime min, DateTime max)
        {
            return min <= dateTime && dateTime <= max;
        }

        //public static DateTime CenterTimeOfDay(this DateTime value, DateTime other)
        //{
        //    if(other == DateTime.MinValue)
        //        return value;

        //    var duration = other - value;
        //    var halfDuration = duration / 2;

        //    return duration.TotalDays switch
        //    {
        //        >= 1 => value.AddDays(0.5d),
        //        > 0 and < 1 => value + halfDuration,
        //        0 => value,
        //        < 0 and > -1 => value + halfDuration,
        //        <= -1 => value.AddDays(-0.5d),
        //        _ => value
        //    };
        //}


        public static DateTime ToTimeZone(this DateTime dateTime, string timeZoneId = "W. Europe Standard Time")
        {
            var value = DateTime.SpecifyKind(dateTime,
                DateTimeKind.Local);

            var easternZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            var offset = easternZone.GetUtcOffset(dateTime);

            var sourceTime = new DateTimeOffset(dateTime, offset);
            var targetTime = sourceTime.DateTime;

            return targetTime;
        }

        public static DateTimeOffset TimeZoneToUtc(this DateTime dateTime, string timeZoneId = "W. Europe Standard Time")
        {
            var easternZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            var offset = easternZone.GetUtcOffset(dateTime);

            dateTime = DateTime.SpecifyKind(dateTime, DateTimeKind.Unspecified);
            var sourceTime = new DateTimeOffset(dateTime, offset);
            var targetTime = sourceTime.ToUniversalTime();
            return targetTime;
        }

        public enum DateTimeSnapPoint
        {
            Year,
            Month,
            Week,
            Day,
            StartOfDay,
            EndOfDay,
            HalfDay,
            QuarterDay,
            EvenHour,
            Hour,
            Minute30,
            Minute15,
            Minute10,
            Minute5,
            Minute2,
            Minute
        }

        public static DateTime StartOfWeek(this DateTime dt, DayOfWeek startOfWeek = DayOfWeek.Monday)
        {
            int diff = (7 + (dt.DayOfWeek - startOfWeek)) % 7;
            return dt.AddDays(-1 * diff).Date;
        }
        public static DateTime EndOfWeek(this DateTime dt, DayOfWeek startOfWeek = DayOfWeek.Monday)
        {
            return dt.StartOfWeek(startOfWeek).AddDays(6).Date;
        }
        public static DateTime StartOfMonth(this DateTime dt)
        {
            return new DateTime(dt.Year, dt.Month, 1);
        }
        public static DateTime EndOfMonth(this DateTime dt)
        {
            var firstDayOfMonth = dt.StartOfMonth();
            return firstDayOfMonth.AddMonths(1).AddDays(-1);
        }
        public static DateTime StartOfYear(this DateTime dt, DayOfWeek startOfWeek = DayOfWeek.Monday)
        {
            return new DateTime(dt.Year, 1, 1);
        }       
        public static DateTime EndOfYear(this DateTime dt, DayOfWeek startOfWeek = DayOfWeek.Monday)
        {
            return new DateTime(dt.Year, 12, 31);
        }

        public static DateTime SnapPoint(this DateTime dateTime, DateTimeSnapPoint scale = DateTimeSnapPoint.Day)
        {
            switch (scale)
            {
                case DateTimeSnapPoint.Minute:
                    return dateTime.Date.AddMinutes((int)dateTime.TimeOfDay.TotalMinutes + 1);
                case DateTimeSnapPoint.Minute2:
                    return dateTime.Date.AddMinutes(NextInterval((int)dateTime.TimeOfDay.TotalMinutes, 2));                
                case DateTimeSnapPoint.Minute5:
                    return dateTime.Date.AddMinutes(NextInterval((int)dateTime.TimeOfDay.TotalMinutes, 5));
                case DateTimeSnapPoint.Minute10:
                    return dateTime.Date.AddMinutes(NextInterval((int)dateTime.TimeOfDay.TotalMinutes, 10));
                case DateTimeSnapPoint.Minute15:
                    return dateTime.Date.AddMinutes(NextInterval((int)dateTime.TimeOfDay.TotalMinutes, 15));
                case DateTimeSnapPoint.Minute30:
                    return dateTime.Date.AddMinutes(NextInterval((int)dateTime.TimeOfDay.TotalMinutes, 30));
                case DateTimeSnapPoint.Hour:
                    return dateTime.Date.AddHours(dateTime.TimeOfDay.Hours + 1);
                case DateTimeSnapPoint.EvenHour:                    
                    return dateTime.Date.AddHours(NextInterval((int)dateTime.TimeOfDay.TotalHours, 2));
                case DateTimeSnapPoint.QuarterDay:                    
                    return dateTime.Date.AddHours(NextInterval((int)dateTime.TimeOfDay.TotalHours, 4));
                case DateTimeSnapPoint.HalfDay:
                    return dateTime.TimeOfDay > new TimeSpan(0, 12, 0, 0) ? dateTime.Date.AddDays(1) : dateTime.Date.AddHours(12);
                case DateTimeSnapPoint.Day:
                    return dateTime.TimeOfDay > new TimeSpan(0, 12, 0, 0) ? dateTime.Date.AddDays(1) : dateTime.Date;
                case DateTimeSnapPoint.StartOfDay:
                    return dateTime.Date;
                case DateTimeSnapPoint.EndOfDay:
                    return dateTime.Date.AddDays(1).Subtract(TimeSpan.FromTicks(1));
                case DateTimeSnapPoint.Week:
                    return dateTime.StartOfWeek();
                default:
                    return dateTime;
            }
        }

        private static int NextInterval(int value, int interval)
        {
            var multiply = (int)value / interval;
            var actInterval = interval * multiply;
            return actInterval + interval;
        }


        public static DateTime EndOfDay(this DateTime dateTime)
        {
            return dateTime.Date.Add(new TimeSpan(0, 23, 59, 59));
        }

        public static string DateString(this DateTime dateTime)
        {
            return dateTime.Date.ToString("yyyy-MM-dd");
        }
    }
}
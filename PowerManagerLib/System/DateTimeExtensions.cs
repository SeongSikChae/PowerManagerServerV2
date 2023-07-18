namespace System
{
    public static class DateTimeExtensions
    {
        internal static readonly DateTime BaseTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Unspecified);

        public static uint ToTimestamp(this DateTime dateTime)
        {
            return (uint)(dateTime.ToUniversalTime() - BaseTime).TotalSeconds;
        }

        public static long ToMilliseconds(this DateTime dateTime)
        {
            return (long)(dateTime.ToUniversalTime() - BaseTime).TotalMilliseconds;
        }

        public static DateTime Snap(this DateTime dateTime, int factor, TimeGranularityUnit unit)
        {
            if (factor != 1)
            {
                switch (unit)
                {
                    case TimeGranularityUnit.SECONDS:
                        return dateTime.Snap(1000L * factor);
                    case TimeGranularityUnit.MINUTES:
                        return dateTime.Snap(1000L * 60 * factor);
                    case TimeGranularityUnit.HOURS:
                        return dateTime.Snap(1000L * 60 * 60 * factor);
                    case TimeGranularityUnit.DAYS:
                        return dateTime.Snap(1000L * 60 * 60 * 24 * factor);
                    case TimeGranularityUnit.MONTHS:
                        int r = dateTime.Month - 1;
                        int m = (r - r % factor) + 1;
                        return new DateTime(dateTime.Year, m, 1, 0, 0, 0);
                    default:
                        throw new Exception("unreachable!");
                }
            }
            else
                return dateTime.Truncate(unit);
        }

        public static DateTime Snap(this DateTime dateTime, long ratio)
        {
            long time = dateTime.ToMilliseconds();
            return (time - (time % ratio)).FromMilliseconds(TimeZoneInfo.Local);
        }

        public static DateTime Truncate(this DateTime dateTime, TimeGranularityUnit unit)
        {
            switch (unit)
            {
                case TimeGranularityUnit.SECONDS:
                    return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second);
                case TimeGranularityUnit.MINUTES:
                    return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, 0);
                case TimeGranularityUnit.HOURS:
                    return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, 0, 0);
                case TimeGranularityUnit.DAYS:
                    return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0);
                case TimeGranularityUnit.WEEK:
                    DateTime targetTime = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0);
                    switch (targetTime.DayOfWeek)
                    {
                        case DayOfWeek.Monday:
                            targetTime = targetTime.AddDays(-1);
                            break;
                        case DayOfWeek.Tuesday:
                            targetTime = targetTime.AddDays(-2);
                            break;
                        case DayOfWeek.Wednesday:
                            targetTime = targetTime.AddDays(-3);
                            break;
                        case DayOfWeek.Thursday:
                            targetTime = targetTime.AddDays(-4);
                            break;
                        case DayOfWeek.Friday:
                            targetTime = targetTime.AddDays(-5);
                            break;
                        case DayOfWeek.Saturday:
                            targetTime = targetTime.AddDays(-6);
                            break;
                    }
                    return targetTime;
                case TimeGranularityUnit.MONTHS:
                    return new DateTime(dateTime.Year, dateTime.Month, 1, 0, 0, 0);
                case TimeGranularityUnit.YEARS:
                    return new DateTime(dateTime.Year, 1, 1, 0, 0, 0);
                default:
                    throw new Exception("unreachable!");
            }
        }

        public static DateTime Next(this DateTime dateTime, TimeGranularityUnit unit, int amount)
        {
            if (amount == 0)
                return dateTime;

            if (amount < 0)
                throw new Exception($"required amount > 0 '{amount}'");

            switch (unit)
            {
                case TimeGranularityUnit.SECONDS:
                    return dateTime.AddSeconds(amount);
                case TimeGranularityUnit.MINUTES:
                    return dateTime.AddMinutes(amount);
                case TimeGranularityUnit.HOURS:
                    return dateTime.AddHours(amount);
                case TimeGranularityUnit.DAYS:
                    return dateTime.AddDays(amount);
                case TimeGranularityUnit.MONTHS:
                    return dateTime.AddMonths(amount);
                case TimeGranularityUnit.YEARS:
                    return dateTime.AddYears(amount);
                default:
                    throw new Exception("unreachable!");
            }
        }

        public static DateTime Previous(this DateTime dateTime, TimeGranularityUnit unit, int amount)
        {
            if (amount == 0)
                return dateTime;

            if (amount < 0)
                throw new Exception($"required amount > 0 '{amount}'");

            switch (unit)
            {
                case TimeGranularityUnit.SECONDS:
                    return dateTime.AddSeconds(-amount);
                case TimeGranularityUnit.MINUTES:
                    return dateTime.AddMinutes(-amount);
                case TimeGranularityUnit.HOURS:
                    return dateTime.AddHours(-amount);
                case TimeGranularityUnit.DAYS:
                    return dateTime.AddDays(-amount);
                case TimeGranularityUnit.MONTHS:
                    return dateTime.AddMonths(-amount);
                case TimeGranularityUnit.YEARS:
                    return dateTime.AddYears(-amount);
                default:
                    throw new Exception("unreachable!");
            }
        }

        public static bool IsBefore(this DateTime dateTime, DateTime target)
        {
            return dateTime < target;
        }

        public static bool IsSame(this DateTime dateTime, DateTime target)
        {
            return dateTime == target;
        }

        public static bool IsAfter(this DateTime dateTime, DateTime target)
        {
            return dateTime > target;
        }
    }
}

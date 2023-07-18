namespace System
{
    public static class Int64Extensions
    {
        public static DateTime FromMilliseconds(this long millisecond, TimeZoneInfo timeZoneInfo)
        {
            return DateTimeExtensions.BaseTime.AddMilliseconds(millisecond).AddSeconds(timeZoneInfo.BaseUtcOffset.TotalSeconds);
        }
    }
}

namespace Dragonfly.Migration7To8Helpers
{
    using System;
    using System.Linq;

    public static class HtmlHelpers
    {
        public static string GetViewsPath()
        {
            var path = Config.GetConfig().GetViewsPath();
            return path;
        }

        #region Date/Time
        //public enum TimezoneFormatOption
        //{
        //    Full,
        //    Abbreviated
        //}
        //public static string FormatUtcDateTime(DateTime dt, TimezoneFormatOption TzFormat = TimezoneFormatOption.Full, string LocalTimezone = "", string DateFormat = "ddd MMM d, yyyy", string TimeFormat = "hh:mm tt" )
        //{
        //    //var dateFormat = "ddd MMM d, yyyy";
        //    //var timeFormat = "hh:mm tt";
        //    if (LocalTimezone == "")
        //    {
        //        var configTz = Config.GetConfig().LocalTimezone;
        //        LocalTimezone = configTz!=""? configTz: TimeZoneInfo.Local.Id;
        //    }
        //    TimeZoneInfo timeZone = TimeZoneInfo.FindSystemTimeZoneById(LocalTimezone);
        //    DateTime localizedDT = TimeZoneInfo.ConvertTimeFromUtc(dt, timeZone);

        //    var zoneFullName = timeZone.IsDaylightSavingTime(localizedDT) ? timeZone.DaylightName : timeZone.StandardName;
        //    var zoneAbbrevName = zoneFullName.Abbreviate();
        //    var zoneInfo = TzFormat == TimezoneFormatOption.Abbreviated ? zoneAbbrevName : zoneFullName;

        //    var stringDate = string.Format("{0} at {1} ({2})", localizedDT.ToString(DateFormat), localizedDT.ToString(TimeFormat), zoneInfo);

        //    return stringDate;
        //}
        #endregion
    }
}

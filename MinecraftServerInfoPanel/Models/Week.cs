using MinecraftServerInfoPanel.Helpers.TypeConverters;
using System;
using System.ComponentModel;
using System.Globalization;

namespace MinecraftServerInfoPanel.Models
{
    [TypeConverter(typeof(WeekConverter))]
    public class Week
    {
        public readonly int Year;
        public readonly int WeekNr;

        public Week(int year, int week)
        {
            Year = year;
            WeekNr = week;
        }

        public DateTime GetFirstDayOfWeek()
        {
            return ISOWeek.ToDateTime(Year, WeekNr, DayOfWeek.Monday);
        }

        public static Week GetCurrentWeek()
        {
            return new Week(DateTime.Now.Year, ISOWeek.GetWeekOfYear(DateTime.Now));
        }

        public Week GetNextWeek()
        {
            int weeksInyear = ISOWeek.GetWeeksInYear(Year);
            int nextWeekNr = WeekNr + 1;

            if (nextWeekNr > weeksInyear)
                return new Week(Year + 1, 1);

            return new Week(Year, nextWeekNr);
        }

        public Week GetPreviousWeek()
        {
            if (WeekNr == 1)
                return new Week(Year - 1, ISOWeek.GetWeeksInYear(Year - 1));

            return new Week(Year, WeekNr - 1);
        }

        public string GetDaysInWeekPeriod()
        {
            var firstDayInWeek = GetFirstDayOfWeek();
            var lastDayInWeek = ISOWeek.ToDateTime(Year, WeekNr, DayOfWeek.Sunday);

            if (firstDayInWeek.Year != lastDayInWeek.Year)
                return $"{firstDayInWeek:dd.MM.yyyy} - {lastDayInWeek:dd.MM.yyyy}";

            return $"{firstDayInWeek:dd.MM} - {lastDayInWeek:dd.MM}.{Year}";
        }

        public override string ToString()
        {
            return $"{Year}-W{WeekNr}";
        }
    }
}

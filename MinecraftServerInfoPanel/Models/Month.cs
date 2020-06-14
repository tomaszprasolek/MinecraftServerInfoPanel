using MinecraftServerInfoPanel.Helpers.TypeConverters;
using System;
using System.ComponentModel;

namespace MinecraftServerInfoPanel.Models
{
    [TypeConverter(typeof(MonthConverter))]
    public class Month
    {
        public int Year { get; }
        public int MonthNr { get; }

        public Month(int year, int month)
        {
            Year = year;
            MonthNr = month;
        }

        public static Month GetCurrentMonth()
        {
            return new Month(DateTime.Now.Year, DateTime.Now.Month);
        }

        public Month GetPreviousMonth()
        {
            if (MonthNr == 1)
                return new Month(Year - 1, 12);

            return new Month(Year, MonthNr - 1);
        }

        public Month GetNextMonth()
        {
            if (MonthNr == 12)
                return new Month(Year + 1, 1);

            return new Month(Year, MonthNr + 1);
        }

        public DateTime GetFirstDayOfMonth()
        {
            return new DateTime(Year, MonthNr, 1);
        }

        public override string ToString()
        {
            return $"{Year}-{MonthNr.ToString().PadLeft(2, '0')}";
        }
    }
}

using MinecraftServerInfoPanel.Models;
using System;
using System.ComponentModel;
using System.Globalization;

namespace MinecraftServerInfoPanel.Helpers.TypeConverters
{
    public class WeekConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
                return true;
            return base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string isoWeek)
            {
                var result = isoWeek.Split("-W");
                return new Week(Convert.ToInt32(result[0]), Convert.ToInt32(result[1]));
            }

            return base.ConvertFrom(context, culture, value);
        }
    }
}

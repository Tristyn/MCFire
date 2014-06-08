using System;
using System.Windows.Data;
using MCFire.Client.Modules.Infrastructure.Enums;

namespace MCFire.Client.Services.Explorer.Converters
{
    [ValueConversion(typeof(DimensionType), typeof(string))]
    public class DimensionTypeToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return @"Unknown";
            switch ((DimensionType)value)
            {
                case DimensionType.Overworld:
                    return @"The Overworld";
                case DimensionType.Nether:
                    return @"The Nether";
                case DimensionType.End:
                    return @"The End";
                default:
                    return @"Unknown";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

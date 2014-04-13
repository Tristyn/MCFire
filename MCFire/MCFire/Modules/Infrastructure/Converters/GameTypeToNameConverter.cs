using System;
using System.Windows.Data;
using Substrate;

namespace MCFire.Modules.Infrastructure.Converters
{
    [ValueConversion(typeof(GameType), typeof(string))]
    public class GameTypeToNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            switch ((GameType)value)
            {
                case GameType.SURVIVAL:
                    return @"Survival World";
                case GameType.CREATIVE:
                    return @"Creative World";
                default:
                    return @"Unknown World";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

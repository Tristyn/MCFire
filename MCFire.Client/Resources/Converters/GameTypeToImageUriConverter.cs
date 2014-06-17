using System;
using System.Windows.Data;
using Substrate;

namespace MCFire.Client.Resources.Converters
{
    [ValueConversion(typeof(GameType), typeof(string))]
    public class GameTypeToImageUriConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return @"pack://application:,,,/MCFire.Client;component/Resources/UnknownBlock.png"; 
            switch ((GameType)value)
            {
                case GameType.SURVIVAL:
                    return @"pack://application:,,,/MCFire.Client;component/Resources/CraftingTable.png";
                case GameType.CREATIVE:
                    return @"pack://application:,,,/MCFire.Client;component/Resources/CommandBlock.png";
                default:
                    return @"pack://application:,,,/MCFire.Client;component/Resources/UnknownBlock.png";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
